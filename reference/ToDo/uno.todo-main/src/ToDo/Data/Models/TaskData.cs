namespace ToDo.Data.Models;

public class TaskData
{
	[JsonPropertyName("id")]
	public string? Id { get; set; }

	[JsonPropertyName("importance")]
	public string? Importance { get; set; }

	[JsonPropertyName("isReminderOn")]
	public bool IsReminderOn { get; set; }

	[JsonPropertyName("status")]
	public string? Status { get; set; }

	[JsonPropertyName("title")]
	public string? Title { get; set; }

	[JsonPropertyName("displayName")]
	public string? DisplayName { get; set; }

	[JsonPropertyName("createdDateTime")]
	public DateTime CreatedDateTime { get; set; }

	[JsonPropertyName("lastModifiedDateTime")]
	public DateTime LastModifiedDateTime { get; set; }

	[JsonPropertyName("completedDateTime")]
	[JsonConverter(typeof(DateTimeDataConverter))]
	public DateTimeOffset? CompletedDateTime { get; set; }

	[JsonPropertyName("body")]
	public TaskBodyData? Body { get; set; }

	[JsonPropertyName("dueDateTime")]
	[JsonConverter(typeof(DateTimeDataConverter))]
	public DateTimeOffset? DueDateTime { get; set; }


	[JsonPropertyName("linkedResources@odata.context")]
	public string? LinkedResourcesOdataContext { get; set; }

	[JsonPropertyName("linkedResources")]
	public List<LinkedResourceData>? LinkedResources { get; set; }

	[JsonPropertyName("@odata.etag")]
	public string? OdataEtag { get; set; }

	[JsonPropertyName("parentList")]
	public ParentListTaskData? ParentList { get; set; }
}
