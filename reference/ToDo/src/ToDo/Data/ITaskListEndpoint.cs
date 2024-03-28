namespace ToDo.Data;

[Headers("Content-Type: application/json")]
public interface ITaskListEndpoint
{
	[Get("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<TaskReponseData<TaskListData>> GetAllAsync(CancellationToken ct);

	[Get("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskListData> GetAsync(string todoTaskListId, CancellationToken ct);

	[Post("/todo/lists")]
	[Headers("Authorization: Bearer")]
	Task<TaskListData> CreateAsync([Body] TaskListRequestData todoList, CancellationToken ct);

	[Patch("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<TaskListData> UpdateAsync(string todoTaskListId, [Body] TaskListRequestData todoList, CancellationToken ct);

	[Delete("/todo/lists/{todoTaskListId}")]
	[Headers("Authorization: Bearer")]
	Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct);
}
