namespace ToDo.Data.Models;

public class TaskListRequestData
{
	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }
}
