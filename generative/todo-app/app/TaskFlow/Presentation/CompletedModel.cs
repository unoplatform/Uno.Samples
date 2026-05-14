using Uno.Extensions.Reactive;

namespace TaskFlow.Presentation;

public partial record CompletedModel(ITaskService TaskService)
{
	public IListFeed<TodoTask> CompletedTasks => ListFeed.AsyncEnumerable(TaskService.ObserveTasksAsync)
		.Where(t => t.IsCompleted);

	public async ValueTask ClearCompleted(CancellationToken ct)
	{
		await TaskService.ClearCompletedAsync(ct);
	}

	public async ValueTask ToggleTask(string taskId, CancellationToken ct)
	{
		await TaskService.ToggleTaskAsync(taskId, ct);
	}
}
