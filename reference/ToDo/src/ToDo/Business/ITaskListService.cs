namespace ToDo.Business;

public interface ITaskListService
{
	ValueTask<IImmutableList<TaskList>> GetAllAsync(CancellationToken ct);

	Task<TaskList> GetAsync(string listId, CancellationToken ct);

	Task CreateAsync(string displayName, CancellationToken ct);

	Task UpdateAsync(TaskList list, CancellationToken ct);

	Task DeleteAsync(TaskList list, CancellationToken ct);
}
