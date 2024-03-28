namespace ToDo.Data;

[Headers("Content-Type: application/json")]
public interface ITaskEndpoint
{
	[Get("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> GetAsync(string listId, string taskId, CancellationToken ct);

	[Post("/todo/lists/{listId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> CreateAsync(string listId, [Body] TaskData newTask, CancellationToken ct);

	[Patch("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskData> UpdateAsync(string listId, string taskId, [Body] TaskData updatedTask, CancellationToken ct);

	[Delete("/todo/lists/{listId}/tasks/{taskId}")]
	[Headers("Authorization: Bearer")]
	Task DeleteAsync(string listId, string taskId, CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}/tasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetAsync(string todoTaskListId, CancellationToken ct);

	[Get("/tasks/allTasks?filter=contains(displayName,'{displayName}')")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetByFilterAsync(string displayName, CancellationToken ct);

	[Get("/tasks/allTasks")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskData>> GetAllAsync(CancellationToken ct);
}
