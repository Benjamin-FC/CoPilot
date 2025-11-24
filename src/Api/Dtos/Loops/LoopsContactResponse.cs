using System.Text.Json.Serialization;

namespace Api.Dtos.Loops;

public class LoopsContactResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
