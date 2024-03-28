namespace ToDo.Data.Models;


public class LinkedResourceData
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("webUrl")]
	public string? WebUrl { get; set; }

	[JsonPropertyName("applicationName")]
	public string? ApplicationName { get; set; }

	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }
}
