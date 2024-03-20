namespace ToDo.Data.Models;

public partial record TaskBodyData
{
	[JsonPropertyName("content")]
	public string? Content { get; init; }

	[JsonPropertyName("contentType")]
	public string? ContentType { get; init; }
}
