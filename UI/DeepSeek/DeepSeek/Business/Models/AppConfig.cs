namespace DeepSeek.Business.Models;

public record AppConfig
{
	public string? Environment { get; init; }
	public string? ApiKey { get; init; }
	public string? BaseUrl { get; init; }
}
