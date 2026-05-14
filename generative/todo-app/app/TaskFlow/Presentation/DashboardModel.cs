using Uno.Extensions.Reactive;
using Uno.Extensions.Reactive.Bindings;

namespace TaskFlow.Presentation;

public partial record DashboardModel(ITaskService TaskService, INavigator Navigator, IThemeService ThemeService)
{
	public IState<bool> IsDark => State.Value(this, () => ThemeService.IsDark);

	public async ValueTask ToggleTheme(CancellationToken ct)
	{
		var isDark = await IsDark;
		await ThemeService.SetThemeAsync(isDark ? AppTheme.Light : AppTheme.Dark);
		await IsDark.UpdateAsync(_ => !isDark, ct);
	}

	public IFeed<DashboardStats> Stats => Feed.AsyncEnumerable(TaskService.ObserveStatsAsync);

	public IListFeed<TodoTask> UpcomingTasks => ListFeed.AsyncEnumerable(TaskService.ObserveTasksAsync)
		.Where(t => !t.IsCompleted && t.Priority == TaskPriority.High);

	public async ValueTask ToggleTask(string taskId, CancellationToken ct)
	{
		await TaskService.ToggleTaskAsync(taskId, ct);
	}

	public async ValueTask GoToTasks(CancellationToken ct)
	{
		await Navigator.NavigateRouteAsync(this, route: "/Main/-/Tasks", cancellation: ct);
	}
}
