using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace TaskFlow.Services;

public interface ITaskService
{
	IAsyncEnumerable<IImmutableList<TodoTask>> ObserveTasksAsync(CancellationToken ct);
	ValueTask<IImmutableList<TodoTask>> GetPendingTasksAsync(CancellationToken ct);
	ValueTask<IImmutableList<TodoTask>> GetCompletedTasksAsync(CancellationToken ct);
	ValueTask<IImmutableList<TodoTask>> GetHighPriorityTasksAsync(CancellationToken ct);
	ValueTask AddTaskAsync(string title, string notes, TaskCategory category, TaskPriority priority, string dueDate, CancellationToken ct);
	ValueTask ToggleTaskAsync(string taskId, CancellationToken ct);
	ValueTask DeleteTaskAsync(string taskId, CancellationToken ct);
	ValueTask ClearCompletedAsync(CancellationToken ct);
	ValueTask<DashboardStats> GetStatsAsync(CancellationToken ct);
	IAsyncEnumerable<DashboardStats> ObserveStatsAsync(CancellationToken ct);
	ValueTask<IImmutableList<CategoryCount>> GetCategoryCountsAsync(CancellationToken ct);
	IAsyncEnumerable<IImmutableList<CategoryCount>> ObserveCategoryCountsAsync(CancellationToken ct);
}

public class TaskService : ITaskService
{
	private const string TaskIdPrefix = "TASK-";

	private readonly object _lock = new();
	private int _taskCounter;
	private ImmutableList<TodoTask> _tasks;
	private TaskCompletionSource _changed = new(TaskCreationOptions.RunContinuationsAsynchronously);

	public TaskService()
	{
		_tasks = ImmutableList.Create(
			new TodoTask($"{TaskIdPrefix}1001", "Buy groceries", "Milk, eggs, bread, avocados", TaskCategory.Shopping, TaskPriority.High, "Today", false),
			new TodoTask($"{TaskIdPrefix}1002", "Team standup meeting", "Daily sync at 9:30 AM", TaskCategory.Work, TaskPriority.Medium, "Today", false),
			new TodoTask($"{TaskIdPrefix}1003", "Morning jog", "30 minutes around the park", TaskCategory.Health, TaskPriority.Medium, "Today", true),
			new TodoTask($"{TaskIdPrefix}1004", "Read chapter 5", "Design Patterns book", TaskCategory.Personal, TaskPriority.Low, "Tomorrow", false),
			new TodoTask($"{TaskIdPrefix}1005", "Prepare presentation", "Q2 results for leadership review", TaskCategory.Work, TaskPriority.High, "Tomorrow", false),
			new TodoTask($"{TaskIdPrefix}1006", "Schedule dentist", "Annual checkup — call Dr. Rivera", TaskCategory.Health, TaskPriority.Low, "This week", false),
			new TodoTask($"{TaskIdPrefix}1007", "Fix login bug", "Users report timeout on SSO flow", TaskCategory.Work, TaskPriority.High, "Today", false),
			new TodoTask($"{TaskIdPrefix}1008", "Order supplements", "Vitamin D and Omega-3", TaskCategory.Shopping, TaskPriority.Low, "This week", true),
			new TodoTask($"{TaskIdPrefix}1009", "Call Mom", "Catch up — Sunday evening", TaskCategory.Personal, TaskPriority.Medium, "This week", true),
			new TodoTask($"{TaskIdPrefix}1010", "Code review PR #42", "Review the refactoring branch", TaskCategory.Work, TaskPriority.Medium, "Today", false));

		_taskCounter = 1010;
	}

	private void NotifyChanged()
	{
		var old = Interlocked.Exchange(ref _changed, new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously));
		old.TrySetResult();
	}

	public ValueTask<IImmutableList<TodoTask>> GetPendingTasksAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(_tasks.Where(t => !t.IsCompleted).ToImmutableList() as IImmutableList<TodoTask>);
		}
	}

	public ValueTask<IImmutableList<TodoTask>> GetCompletedTasksAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(_tasks.Where(t => t.IsCompleted).ToImmutableList() as IImmutableList<TodoTask>);
		}
	}

	public ValueTask<IImmutableList<TodoTask>> GetHighPriorityTasksAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(_tasks.Where(t => !t.IsCompleted && t.Priority == TaskPriority.High).ToImmutableList() as IImmutableList<TodoTask>);
		}
	}

	public async IAsyncEnumerable<IImmutableList<TodoTask>> ObserveTasksAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			ImmutableList<TodoTask> snapshot;
			Task waitTask;
			lock (_lock)
			{
				snapshot = _tasks;
				waitTask = _changed.Task;
			}

			yield return snapshot;
			await waitTask.WaitAsync(ct);
		}
	}

	public ValueTask AddTaskAsync(string title, string notes, TaskCategory category, TaskPriority priority, string dueDate, CancellationToken ct)
	{
		lock (_lock)
		{
			var id = $"{TaskIdPrefix}{++_taskCounter}";
			var task = new TodoTask(id, title, notes, category, priority, dueDate, false);
			_tasks = _tasks.Insert(0, task);
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask ToggleTaskAsync(string taskId, CancellationToken ct)
	{
		lock (_lock)
		{
			var index = _tasks.FindIndex(t => t.Id == taskId);
			if (index >= 0)
			{
				var task = _tasks[index];
				_tasks = _tasks.SetItem(index, task with { IsCompleted = !task.IsCompleted });
			}
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask DeleteTaskAsync(string taskId, CancellationToken ct)
	{
		lock (_lock)
		{
			_tasks = _tasks.RemoveAll(t => t.Id == taskId);
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask ClearCompletedAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			_tasks = _tasks.RemoveAll(t => t.IsCompleted);
		}

		NotifyChanged();
		return ValueTask.CompletedTask;
	}

	public ValueTask<DashboardStats> GetStatsAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(BuildStats());
		}
	}

	public async IAsyncEnumerable<DashboardStats> ObserveStatsAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			DashboardStats stats;
			Task waitTask;
			lock (_lock)
			{
				stats = BuildStats();
				waitTask = _changed.Task;
			}

			yield return stats;
			await waitTask.WaitAsync(ct);
		}
	}

	public ValueTask<IImmutableList<CategoryCount>> GetCategoryCountsAsync(CancellationToken ct)
	{
		lock (_lock)
		{
			return new(BuildCategoryCounts());
		}
	}

	public async IAsyncEnumerable<IImmutableList<CategoryCount>> ObserveCategoryCountsAsync(
		[EnumeratorCancellation] CancellationToken ct)
	{
		while (!ct.IsCancellationRequested)
		{
			IImmutableList<CategoryCount> counts;
			Task waitTask;
			lock (_lock)
			{
				counts = BuildCategoryCounts();
				waitTask = _changed.Task;
			}

			yield return counts;
			await waitTask.WaitAsync(ct);
		}
	}

	private DashboardStats BuildStats()
	{
		var total = _tasks.Count;
		var completed = _tasks.Count(t => t.IsCompleted);
		var pending = total - completed;
		var highPriority = _tasks.Count(t => !t.IsCompleted && t.Priority == TaskPriority.High);
		return new DashboardStats(total, completed, pending, highPriority);
	}

	private IImmutableList<CategoryCount> BuildCategoryCounts()
	{
		return Enum.GetValues<TaskCategory>()
			.Select(cat =>
			{
				var inCategory = _tasks.Where(t => t.Category == cat).ToList();
				var icon = cat switch
				{
					TaskCategory.Work => "\uE821",
					TaskCategory.Personal => "\uE77B",
					TaskCategory.Shopping => "\uE7BF",
					TaskCategory.Health => "\uE95E",
					_ => "\uE8A5"
				};
				return new CategoryCount(cat, cat.ToString(), icon, inCategory.Count, inCategory.Count(t => !t.IsCompleted), inCategory.Count(t => t.IsCompleted));
			})
			.ToImmutableList();
	}
}
