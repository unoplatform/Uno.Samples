namespace ToDo.Business;

public interface ITaskService
{
	Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct);

	Task CreateAsync(TaskList list, ToDoTask newTask, CancellationToken ct);

	Task UpdateAsync(ToDoTask task, CancellationToken ct);

	Task DeleteAsync(ToDoTask task, CancellationToken ct);

	ValueTask<IImmutableList<ToDoTask>> GetAllAsync(TaskList list, CancellationToken ct);

	ValueTask<IImmutableList<ToDoTask>> SearchAsync(string term, CancellationToken ct);
}
