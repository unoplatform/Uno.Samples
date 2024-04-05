namespace ToDo.Data.Mock;

public class MockTaskListEndpoint : ITaskListEndpoint
{
	private const string ListDataFile = "Mock/lists.json";
	private const string TasksDataFile = "Mock/tasks.json";


	private readonly ISerializer<TaskListData> _listSerializer;
	private readonly ISerializer<TaskData> _taskSerializer;

	private readonly IStorage _dataService;
	public MockTaskListEndpoint(
		ISerializer<TaskListData> listSerializer,
		ISerializer<TaskData> taskSerializer,
		IStorage dataService)
	{
		_listSerializer = listSerializer;
		_taskSerializer = taskSerializer;
		_dataService = dataService;
	}

	private IList<TaskListData>? data;
	private IList<TaskData>? allTasks;
	private IDictionary<string, IList<TaskData>> taskData = new Dictionary<string, IList<TaskData>>();

	private async Task<IList<TaskListData>> Load()
	{
		if (data is null)
		{
			data = (await _dataService.ReadPackageFileAsync<TaskListData[]>(_listSerializer, ListDataFile)).ToList();
		}
		return data;
	}

	internal async Task<IList<TaskData>> LoadListTasks(string listId)
	{
		if (taskData.TryGetValue(listId, out var existingTasks))
		{
			return existingTasks;
		}

		_ = await LoadAllTasks();

		taskData[listId] = allTasks.Where(x=>x.ParentList?.Id==listId).ToList();
		return taskData[listId];
	}

	private async Task<IList<TaskData>> LoadAllTasks()
	{
		if (allTasks is null)
		{
			allTasks = (await _dataService.ReadPackageFileAsync<TaskData[]>(_taskSerializer, TasksDataFile)).ToList();
		}
		return allTasks;
	}

	public async Task<TaskListData> CreateAsync(TaskListRequestData todoList, CancellationToken ct)
	{
		_ = await Load();

		var list = new TaskListData { Id = Guid.NewGuid().ToString(), DisplayName = todoList.DisplayName };
		data?.Add(list);
		return list;
	}

	public async Task<HttpResponseMessage> DeleteAsync(string todoTaskListId, CancellationToken ct)
	{
		_ = await Load();
		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			data?.Remove(list);
		}
		return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
	}

	public async Task<TaskReponseData<TaskListData>> GetAllAsync(CancellationToken ct)
	{
		_ = await Load();
		return new TaskReponseData<TaskListData> { Value = data.ToList() };
	}

	public async Task<TaskListData> GetAsync(string todoTaskListId, CancellationToken ct)
	{
		_ = await Load();
		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			return list;
		}

		return new TaskListData();
	}

	public async Task<TaskListData> UpdateAsync(string todoTaskListId, [Body] TaskListRequestData todoList, CancellationToken ct)
	{
		_ = await Load();

		var list = data.FirstOrDefault(x => x.Id == todoTaskListId);
		if (list is not null)
		{
			list.DisplayName = todoList.DisplayName;
			return list;
		}

		return new TaskListData();
	}


	internal async Task AddTaskToList(string todoTaskListId, TaskData task)
	{
		var list = await LoadListTasks(todoTaskListId);
		list.Add(task);
	}

	internal async Task DeleteTaskFromList(string todoTaskListId, string taskId)
	{
		var list = await LoadListTasks(todoTaskListId);
		var task = list.FirstOrDefault(t => t.Id == taskId);
		if (task is not null)
		{
			list.Remove(task);
		}
	}

	internal async Task UpdateTaskInList(string todoTaskListId, TaskData task)
	{
		await DeleteTaskFromList(todoTaskListId, task.Id!);
		await AddTaskToList(todoTaskListId, task);
	}

	public async Task<TaskReponseData<TaskData>> GetAllTasksAsync(string displayName = "", CancellationToken ct = default)
	{
		var tasks = await LoadAllTasks();

		if(string.IsNullOrWhiteSpace(displayName))
			return new TaskReponseData<TaskData> { Value = tasks.ToList() };

		return new TaskReponseData<TaskData> { Value = tasks.Where(x => x.Title != null && x.Title.Contains(displayName)).ToList() };
	}
}
