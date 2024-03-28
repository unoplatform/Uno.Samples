namespace ToDo.Presentation;

public partial class TaskViewModel
{
	private readonly INavigator _navigator;
	private readonly ITaskService _svc;

	private TaskViewModel(
		INavigator navigator,
		ITaskService svc,
		ToDoTask entity)
	{
		_navigator = navigator;
		_svc = svc;

		Entity = State.Value(this, () => entity);
		Entity.Execute(async (task, ct) =>
		{
			if (task is not null)
			{
				await _svc.UpdateAsync(task, ct);
			}
		});
	}

	public IState<ToDoTask> Entity { get; }

	public async ValueTask ToggleIsCompleted(CancellationToken ct)
	{
		if (await Entity is { } task)
		{
			await _svc.UpdateAsync(task.ToggleIsCompleted(), ct);
		}
	}

	public async ValueTask ToggleIsImportant(CancellationToken ct)
	{
		if (await Entity is { } task)
		{
			await _svc.UpdateAsync(task.ToggleIsImportant(), ct);
		}
	}

	public async ValueTask Delete(CancellationToken ct)
	{
		var task = await Entity;
		if (task is null)
		{
			return;
		}

		var result = await _navigator.ShowMessageDialogAsync<object>(this, Dialog.ConfirmDeleteTask, cancellation: ct);
		if (result == DialogResults.Affirmative)
		{
			await _svc.DeleteAsync(task, ct);
			await _navigator.NavigateBackAsync(this, cancellation: ct);
		}
	}

	public async ValueTask DeleteDueDate(CancellationToken ct)
	{
		if (await Entity is { } task)
		{
			await _svc.UpdateAsync(task with { DueDateTime = null }, ct);
		}
	}

	public async ValueTask AddDueDate(CancellationToken ct)
	{
		var task = await Entity;
		if (task is null)
		{
			return;
		}

		var result = await _navigator
			.NavigateViewModelForResultAsync<ExpirationDateViewModel, PickedDate>(this, qualifier:Qualifiers.Dialog, data: new PickedDate(task.DueDateTime), cancellation: ct)
			.AsResult();

		if (result.SomeOrDefault()?.Date is {} date)
		{
			await _svc.UpdateAsync(task with { DueDateTime = date }, ct);
			await Entity.UpdateValue(opt => opt.Map(task => task with { DueDateTime = date }), ct);
		}
	}
}
