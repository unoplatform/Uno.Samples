namespace ChatGPT.Business.Models;

public record AppConfig
{
    public string? Environment { get; init; }
    public string? ApiKey { get; init; }
}
