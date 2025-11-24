using System.Text.Json.Serialization;

namespace Api.Dtos.Loops;

public class LoopsContactRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }
    
    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }
    
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    
    [JsonPropertyName("subscribed")]
    public bool Subscribed { get; set; } = true;
    
    [JsonPropertyName("userGroup")]
    public string? UserGroup { get; set; }
    
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
}
