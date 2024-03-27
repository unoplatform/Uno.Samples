namespace ToDo.Data.Models;

public class TaskListData
{
	[JsonPropertyName("@odata.etag")]
	public string? Odata { get; set; }

	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("isOwner")]
	public bool IsOwner { get; set; }

	[JsonPropertyName("isShared")]
	public bool IsShared {get; set; }

	[JsonPropertyName("wellknownListName")]
	public string? WellknownListName { get; set; }
}
