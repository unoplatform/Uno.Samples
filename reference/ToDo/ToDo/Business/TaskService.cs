namespace ToDo.Business;

public class TaskService : ITaskService
{
	private readonly ITaskEndpoint _client;
	private readonly IMessenger _messenger;

	public TaskService(ITaskEndpoint client, IMessenger messenger)
	{
		_client = client;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async Task<ToDoTask> GetAsync(string listId, string taskId, CancellationToken ct)
		=> new(listId, await _client.GetAsync(listId, taskId, ct) ?? throw new InvalidOperationException($"Cannot get task with id {taskId} (list: {listId})"));

	/// <inheritdoc />
	public async Task CreateAsync(TaskList list, ToDoTask newTask, CancellationToken ct)
	{
		var createdTask = await _client.CreateAsync(list.Id, newTask.ToData(), ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Created, new(list, createdTask)));
	}

	/// <inheritdoc />
	public async Task UpdateAsync(ToDoTask task, CancellationToken ct)
	{
		var updatedTask = await _client.UpdateAsync(task.ListId, task.Id, task.ToData(), ct);

		// Send updates to listeners of both the list and the individual task (in case the task page is open)
		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Updated, new(task.ListId, updatedTask)));
	}

	/// <inheritdoc />
	public async Task DeleteAsync(ToDoTask task, CancellationToken ct)
	{
		await _client.DeleteAsync(task.ListId, task.Id, ct);

		_messenger.Send(new EntityMessage<ToDoTask>(EntityChange.Deleted, task));
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> GetAllAsync(TaskList list, CancellationToken ct)
	{
		if (list.WellknownListName == TaskList.WellknownListNames.Important)
		{
			return ToEntity(await _client.GetAllAsync(ct))
				.Where(task => task.IsImportant)
				.ToImmutableList();
		}
		else
		{
			return ToEntity(await _client.GetAsync(list.Id, ct));
		}

		IImmutableList<ToDoTask> ToEntity(TaskReponseData<TaskData> response)
			=> response
				.Value
				?.Select(data => new ToDoTask(list.Id, data))
				.ToImmutableList()
				?? ImmutableList<ToDoTask>.Empty;
	}

	/// <inheritdoc />
	public async ValueTask<IImmutableList<ToDoTask>> SearchAsync(string term, CancellationToken ct)
		// Note: If we don't have a valid search term, we return an empty list instead of loading all tasks
		=> term is { Length: > 0 } && await _client.GetByFilterAsync(term, ct) is { Value.Count: >0 } response
			? response
				.Value
				.Where(data => data.ParentList?.Id is not null)
				.Select(data => new ToDoTask(data.ParentList!.Id!, data))
				.ToImmutableList()
			: ImmutableList<ToDoTask>.Empty;
}
