using Api.Data;
using Api.Domain;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Xunit;

namespace Api.Tests;

public class ClientsControllerTests : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private HttpClient _client = null!;
    private AppDbContext _dbContext = null!;

    public ClientsControllerTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            });
        });

        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        _factory.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task GetClients_ReturnsOkWithData()
    {
        var response = await _client.GetAsync("/api/clients");
        var result = await response.Content.ReadAsAsync<ClientListResponse>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 0);
    }

    [Fact]
    public async Task GetClients_WithQuery_FiltersResults()
    {
        var response = await _client.GetAsync("/api/clients?query=John");
        var result = await response.Content.ReadAsAsync<ClientListResponse>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsCreated()
    {
        var newClient = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user.new@example.com"
        };

        var response = await _client.PostAsJsonAsync("/api/clients", newClient);
        var result = await response.Content.ReadAsAsync<ClientDetailDto>();

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task CreateClient_WithInvalidEmail_ReturnsBadRequest()
    {
        var invalidClient = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email"
        };

        var response = await _client.PostAsJsonAsync("/api/clients", invalidClient);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetClientById_WithValidId_ReturnsOk()
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe.test@example.com",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();

        var response = await _client.GetAsync($"/api/clients/{client.Id}");
        var result = await response.Content.ReadAsAsync<ClientDetailDto>();

        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(client.Id, result.Id);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_ReturnsNoContent()
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe.delete@example.com",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();

        var response = await _client.DeleteAsync($"/api/clients/{client.Id}");

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        var deleted = await _dbContext.Clients.FindAsync(client.Id);
        Assert.Null(deleted);
    }
}

public static class HttpContentExtensions
{
    public static async Task<T> ReadAsAsync<T>(this HttpContent content)
    {
        var json = await content.ReadAsStringAsync();
        return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}
