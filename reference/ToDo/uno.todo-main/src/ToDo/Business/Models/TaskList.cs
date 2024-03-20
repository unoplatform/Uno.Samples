namespace ToDo.Business.Models;

public partial record TaskList
{
	public static class WellknownListNames
	{
		public const string Important = "important";
		public const string Tasks = "tasks";
	}

	internal TaskList(string wellknownListName, string displayName)
	{
		WellknownListName = wellknownListName;
		DisplayName = displayName;
		Id = wellknownListName;
	}

	internal TaskList(TaskListData data)
	{
		Id = data.Id ?? throw new ArgumentNullException("data.Id", "List must have a valid ID.");
		Odata = data.Odata;
		DisplayName = data.DisplayName;
		IsOwner = data.IsOwner;
		IsShared = data.IsShared;
		WellknownListName = data.WellknownListName;
	}

	public string Id { get; } // No public init: this can be set only from a data

	public string? Odata { get; init; }

	public string? DisplayName { get; init; }

	public bool IsOwner { get; init; }

	public bool IsShared { get; init; }

	public string? WellknownListName { get; init; }

	public bool IsCustom => WellknownListName is null or "none";
}
