using Api.Data;
using Api.Domain;
using Api.Dtos;
using Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Api.Tests;

public class ClientsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ILoopsService> _mockLoopsService;

    public ClientsControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockLoopsService = new Mock<ILoopsService>();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Remove existing ILoopsService
                var loopsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ILoopsService));
                if (loopsDescriptor != null)
                {
                    services.Remove(loopsDescriptor);
                }

                // Add test DbContext with in-memory database
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });

                // Add mocked ILoopsService
                services.AddSingleton(_mockLoopsService.Object);
            });
        });
    }

    [Fact]
    public async Task GetClients_ReturnsOkWithEmptyList_WhenNoClients()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/clients");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientListResponse>();
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task CreateClient_WithValidData_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "555-555-1234",
            Company = "Test Corp"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/clients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>();
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("john.doe@example.com", result.Email);
    }

    [Fact]
    public async Task CreateClient_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createDto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Phone = "555-1234"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/clients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateClient_WithDuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "duplicate@example.com",
            Phone = "555-555-1234"
        };

        // Act - Create first client
        var response1 = await client.PostAsJsonAsync("/api/clients", createDto);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        // Act - Try to create duplicate
        var response2 = await client.PostAsJsonAsync("/api/clients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response2.StatusCode);
    }

    [Fact]
    public async Task GetClientById_WithValidId_ReturnsClient()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "555-555-5678"
        };

        var createResponse = await client.PostAsJsonAsync("/api/clients", createDto);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDetailDto>();

        // Act
        var response = await client.GetAsync($"/api/clients/{createdClient!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>();
        Assert.NotNull(result);
        Assert.Equal(createdClient.Id, result.Id);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("Smith", result.LastName);
    }

    [Fact]
    public async Task GetClientById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/clients/{invalidId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateClient_WithValidData_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob.jones@example.com",
            Phone = "555-555-9999"
        };

        var createResponse = await client.PostAsJsonAsync("/api/clients", createDto);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDetailDto>();

        var updateDto = new UpdateClientDto
        {
            FirstName = "Robert",
            LastName = "Jones",
            Email = "bob.jones@example.com",
            Phone = "555-555-0000",
            IsActive = true
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/clients/{createdClient!.Id}", updateDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ClientDetailDto>();
        Assert.NotNull(result);
        Assert.Equal("Robert", result.FirstName);
        Assert.Equal("555-555-0000", result.Phone);
    }

    [Fact]
    public async Task DeleteClient_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "Delete",
            LastName = "Me",
            Email = "delete.me@example.com",
            Phone = "555-555-1111"
        };

        var createResponse = await client.PostAsJsonAsync("/api/clients", createDto);
        var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientDetailDto>();

        // Act
        var response = await client.DeleteAsync($"/api/clients/{createdClient!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify client was deleted
        var getResponse = await client.GetAsync($"/api/clients/{createdClient.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateClient_CallsLoopsService()
    {
        // Arrange
        var client = _factory.CreateClient();
        _mockLoopsService.Setup(x => x.CreateContactAsync(
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var createDto = new CreateClientDto
        {
            FirstName = "Loops",
            LastName = "Test",
            Email = "loops.test@example.com",
            Phone = "555-555-2222"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/clients", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        // Wait a bit for the background task to complete
        await Task.Delay(500);
        
        // Verify Loops service was called (eventually)
        _mockLoopsService.Verify(x => x.CreateContactAsync(
            "loops.test@example.com",
            "Loops",
            "Test",
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), 
            Times.AtLeastOnce());
    }
}
