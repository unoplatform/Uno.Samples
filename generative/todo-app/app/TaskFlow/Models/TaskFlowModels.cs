using System.Collections.Immutable;

namespace TaskFlow.Models;

public enum TaskPriority { Low, Medium, High }

public enum TaskCategory { Work, Personal, Shopping, Health }

public partial record TodoTask(
	string Id,
	string Title,
	string Notes,
	TaskCategory Category,
	TaskPriority Priority,
	string DueDate,
	bool IsCompleted)
{
	public string PriorityDisplay => Priority switch
	{
		TaskPriority.High => "High",
		TaskPriority.Medium => "Medium",
		TaskPriority.Low => "Low",
		_ => "Unknown"
	};

	public string CategoryDisplay => Category switch
	{
		TaskCategory.Work => "Work",
		TaskCategory.Personal => "Personal",
		TaskCategory.Shopping => "Shopping",
		TaskCategory.Health => "Health",
		_ => "Unknown"
	};

	public string CategoryIcon => Category switch
	{
		TaskCategory.Work => "\uE821",
		TaskCategory.Personal => "\uE77B",
		TaskCategory.Shopping => "\uE7BF",
		TaskCategory.Health => "\uE95E",
		_ => "\uE8A5"
	};
}

public partial record CategoryCount(TaskCategory Category, string Name, string Icon, int TotalCount, int PendingCount, int CompletedCount);

public partial record DashboardStats(int TotalTasks, int CompletedTasks, int PendingTasks, int HighPriorityCount)
{
	public string CompletionText => TotalTasks == 0
		? "No tasks yet"
		: $"{CompletedTasks} of {TotalTasks} completed";

	public double CompletionPercent => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks * 100;

	public string CompletionPercentText => $"{CompletionPercent:F0}%";
}

public partial record FilterItem(string Id, string Name, string Icon);
