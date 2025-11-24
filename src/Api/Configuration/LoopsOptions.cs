namespace Api.Configuration;

public class LoopsOptions
{
    public const string SectionName = "Loops";
    
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://app.loops.so/api/v1";
    public bool Enabled { get; set; } = false;
    public string DefaultSource { get; set; } = "CRM API";
    public int TimeoutSeconds { get; set; } = 30;
}
