using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Api.Configuration;
using Api.Dtos.Loops;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class LoopsService : ILoopsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LoopsService> _logger;
    private readonly LoopsOptions _options;

    public LoopsService(
        HttpClient httpClient,
        IOptions<LoopsOptions> options,
        ILogger<LoopsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<bool> CreateContactAsync(
        string email,
        string? firstName = null,
        string? lastName = null,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogDebug("Loops.so integration is disabled. Skipping contact creation for {Email}", email);
            return false;
        }

        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            _logger.LogWarning("Loops.so API key is not configured. Cannot create contact for {Email}", email);
            return false;
        }

        try
        {
            var request = new LoopsContactRequest
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                UserId = userId,
                Source = _options.DefaultSource,
                Subscribed = true
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/contacts/create")
            {
                Content = content
            };
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

            _logger.LogInformation("Creating contact in Loops.so for {Email}", email);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var loopsResponse = JsonSerializer.Deserialize<LoopsContactResponse>(responseContent);

                _logger.LogInformation(
                    "Successfully created contact in Loops.so for {Email}. Response: {Message}", 
                    email, 
                    loopsResponse?.Message);
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning(
                    "Failed to create contact in Loops.so for {Email}. Status: {StatusCode}, Response: {Response}",
                    email,
                    response.StatusCode,
                    errorContent);
                return false;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while creating contact in Loops.so for {Email}", email);
            return false;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout while creating contact in Loops.so for {Email}", email);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating contact in Loops.so for {Email}", email);
            return false;
        }
    }
}
