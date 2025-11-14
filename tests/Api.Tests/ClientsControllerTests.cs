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
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;
    private readonly List<Client> _testClients;

    public ClientsControllerTests()
    {
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the app's AppDbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false, // Disable automatic redirects
            HandleCookies = true
        });
        
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _testClients = new List<Client>();
    }

    public async Task InitializeAsync()
    {
        // Ensure the database is created
        await _dbContext.Database.EnsureCreatedAsync();
        
        // Clear any existing data
        _dbContext.Clients.RemoveRange(_dbContext.Clients);
        await _dbContext.SaveChangesAsync();
        
        // Add test data
        _testClients.AddRange(new[]
        {
            new Client
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            },
            new Client
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            }
        });

        await _dbContext.Clients.AddRangeAsync(_testClients);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.DisposeAsync();
        _scope.Dispose();
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetClients_ReturnsOkWithData()
    {
        // Act
        var response = await _client.GetAsync("/api/clients");
        response.EnsureSuccessStatusCode(); // This will throw if not 200-299
        var result = await response.Content.ReadFromJsonAsync<ClientListResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(_testClients.Count, result.Total);
        Assert.Equal(_testClients.Count, result.Items.Count);
    }

    [Fact]
    public async Task GetClients_WithQuery_FiltersResults()
    {
        // Act
        var response = await _client.GetAsync("/api/clients?query=John");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientListResponse>();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Total > 0, "Expected at least one client to match the query");
        Assert.Contains(result.Items, x => x.FirstName.Contains("John"));
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newClient = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user.new@example.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clients", newClient);
        
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>();
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal("test.user.new@example.com", result.Email);
        
        // Verify the client was actually created
        var createdClient = await _dbContext.Clients
            .FirstOrDefaultAsync(c => c.Email == "test.user.new@example.com");
        Assert.NotNull(createdClient);
        Assert.Equal("Test", createdClient.FirstName);
    }

    [Fact]
    public async Task CreateClient_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var invalidClient = new CreateClientDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "invalid-email"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/clients", invalidClient);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetClientById_WithValidId_ReturnsOk()
    {
        // Arrange
        var testClient = _testClients[0];

        // Act
        var response = await _client.GetAsync($"/api/clients/{testClient.Id}");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testClient.Id, result.Id);
        Assert.Equal(testClient.FirstName, result.FirstName);
        Assert.Equal(testClient.LastName, result.LastName);
        Assert.Equal(testClient.Email, result.Email);
    }

    [Fact]
    public async Task GetClientById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/clients/{invalidId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var clientToDelete = _testClients[0];

        // Act
        var response = await _client.DeleteAsync($"/api/clients/{clientToDelete.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the client was actually deleted
        var deleted = await _dbContext.Clients.FindAsync(clientToDelete.Id);
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
