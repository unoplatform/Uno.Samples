using Uno.Extensions.Reactive;

namespace TaskFlow.Presentation;

public partial record TasksModel(ITaskService TaskService)
{
	public IState<string> NewTaskTitle => State<string>.Empty(this);

	public IState<string> NewTaskNotes => State<string>.Empty(this);

	public IListFeed<TodoTask> FilteredTasks =>
		ListFeed.AsyncEnumerable(TaskService.ObserveTasksAsync)
			.Where(t => !t.IsCompleted);

	public async ValueTask AddTask(CancellationToken ct)
	{
		var title = await NewTaskTitle;
		if (string.IsNullOrWhiteSpace(title))
		{
			return;
		}

		var notes = await NewTaskNotes ?? string.Empty;
		await TaskService.AddTaskAsync(title, notes, TaskCategory.Personal, TaskPriority.Medium, "Today", ct);
		await NewTaskTitle.Set(string.Empty, ct);
		await NewTaskNotes.Set(string.Empty, ct);
	}

	public async ValueTask ToggleTask(string taskId, CancellationToken ct)
	{
		await TaskService.ToggleTaskAsync(taskId, ct);
	}

	public async ValueTask DeleteTask(string taskId, CancellationToken ct)
	{
		await TaskService.DeleteTaskAsync(taskId, ct);
	}
}
