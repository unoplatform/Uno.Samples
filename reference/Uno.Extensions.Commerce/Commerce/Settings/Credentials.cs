using System.Text.Json.Serialization;

namespace Commerce.ViewModels;

public record Credentials
{
	[JsonPropertyName("username")]
	public string? UserName { get; init; }

	public string? Password { get; init; }
}
