using Api.Configuration;
using Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace Api.Tests;

public class LoopsServiceTests
{
    private readonly Mock<ILogger<LoopsService>> _mockLogger;
    private readonly Mock<IOptions<LoopsOptions>> _mockOptions;
    private readonly LoopsOptions _loopsOptions;

    public LoopsServiceTests()
    {
        _mockLogger = new Mock<ILogger<LoopsService>>();
        _mockOptions = new Mock<IOptions<LoopsOptions>>();
        _loopsOptions = new LoopsOptions
        {
            ApiKey = "test-api-key",
            BaseUrl = "https://app.loops.so/api/v1",
            Enabled = true,
            DefaultSource = "CRM API",
            TimeoutSeconds = 30
        };
        _mockOptions.Setup(x => x.Value).Returns(_loopsOptions);
    }

    [Fact]
    public async Task CreateContactAsync_WithValidData_ReturnsTrue()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"success\":true,\"id\":\"123\",\"message\":\"Contact created\"}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_loopsOptions.BaseUrl)
        };

        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await service.CreateContactAsync("test@example.com", "John", "Doe", "user123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CreateContactAsync_WhenDisabled_ReturnsFalse()
    {
        // Arrange
        _loopsOptions.Enabled = false;
        var httpClient = new HttpClient { BaseAddress = new Uri(_loopsOptions.BaseUrl) };
        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await service.CreateContactAsync("test@example.com", "John", "Doe");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateContactAsync_WithoutApiKey_ReturnsFalse()
    {
        // Arrange
        _loopsOptions.ApiKey = string.Empty;
        var httpClient = new HttpClient { BaseAddress = new Uri(_loopsOptions.BaseUrl) };
        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await service.CreateContactAsync("test@example.com", "John", "Doe");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateContactAsync_WithHttpError_ReturnsFalse()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"success\":false,\"message\":\"Invalid request\"}")
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_loopsOptions.BaseUrl)
        };

        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await service.CreateContactAsync("invalid-email", "John", "Doe");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateContactAsync_WithHttpException_ReturnsFalse()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_loopsOptions.BaseUrl)
        };

        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        var result = await service.CreateContactAsync("test@example.com", "John", "Doe");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task CreateContactAsync_SendsCorrectRequestData()
    {
        // Arrange
        HttpRequestMessage? capturedRequest = null;
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                capturedRequest = request;
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"success\":true,\"id\":\"123\",\"message\":\"Contact created\"}")
                };
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri(_loopsOptions.BaseUrl)
        };

        var service = new LoopsService(httpClient, _mockOptions.Object, _mockLogger.Object);

        // Act
        await service.CreateContactAsync("test@example.com", "John", "Doe", "user123");

        // Assert
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Contains("/contacts/create", capturedRequest.RequestUri?.ToString());
        Assert.Equal("Bearer test-api-key", capturedRequest.Headers.Authorization?.ToString());
        
        var content = await capturedRequest.Content!.ReadAsStringAsync();
        Assert.Contains("test@example.com", content);
        Assert.Contains("John", content);
        Assert.Contains("Doe", content);
        Assert.Contains("user123", content);
        Assert.Contains("CRM API", content);
    }
}
