

namespace ToDo.Presentation;

public partial class TaskListViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskListService _listSvc;
	private readonly ITaskService _taskSvc;
	private readonly ILogger _logger;

	private TaskListViewModel(
		ILogger<TaskListViewModel> logger,
		INavigator navigator,
		ITaskListService listSvc,
		ITaskService taskSvc,
		IMessenger messenger,
		TaskList entity)
	{
		_logger = logger;
		_navigator = navigator;
		_listSvc = listSvc;
		_taskSvc = taskSvc;

		Entity = State.Value(this, () => entity);

		messenger.Observe(Entity, list => list.Id);
		messenger.Observe(Tasks, Entity, (list, task) => list.Id == task.ListId, task => task.Id);
	}

	public IState<TaskList> Entity { get; }

	public IListState<ToDoTask> Tasks => ListState<ToDoTask>.Async(this, async ct => await _taskSvc.GetAllAsync((await Entity)!, ct));

	public IListFeed<ToDoTask> ActiveTasks => Tasks.Where(task => !task.IsCompleted);

	public IListFeed<ToDoTask> CompletedTasks => Tasks.Where(task => task.IsCompleted);

	public async ValueTask ToggleIsImportant(ToDoTask task, CancellationToken ct)
		=> await _taskSvc.UpdateAsync(task.ToggleIsImportant(), ct);

	public async ValueTask ToggleIsCompleted(ToDoTask task, CancellationToken ct)
		=> await _taskSvc.UpdateAsync(task.ToggleIsCompleted(), ct);


	public async ValueTask CreateTask(CancellationToken ct)
	{
		var list = await Entity;
		if (list is null)
		{
			return;
		}

		var taskName = await _navigator.GetDataAsync<AddTaskViewModel, string>(this, qualifier: Qualifiers.Dialog, cancellation: ct);
		if (taskName is { Length: > 0 })
		{
			var newTask = new ToDoTask { Title = taskName };
			await _taskSvc.CreateAsync(list, newTask, ct);
		}
	}

	public async ValueTask DeleteList(CancellationToken ct)
	{
		var list = await Entity;
		if (list is null)
		{
			return;
		}

		var result = await _navigator.ShowMessageDialogAsync<object>(this, Dialog.ConfirmDeleteList, cancellation: ct);
		if (result == DialogResults.Affirmative)
		{
			await _listSvc.DeleteAsync(list, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	public async ValueTask RenameList(CancellationToken ct)
	{
		var list = await Entity;
		if (list is null)
		{
			return;
		}

		var result= await _navigator.NavigateViewModelForResultAsync<RenameListViewModel, string>(this, qualifier: Qualifiers.Dialog, data: list, cancellation: ct).AsResult();
		if (result.IsSome(out var newListName) && !string.IsNullOrWhiteSpace(newListName))
		{
			list = list with { DisplayName = newListName };
			await _listSvc.UpdateAsync(list, ct);
		}
	}
}

