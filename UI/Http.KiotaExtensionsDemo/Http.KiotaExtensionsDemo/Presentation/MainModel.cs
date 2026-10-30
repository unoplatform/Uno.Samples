namespace Http.KiotaExtensionsDemo.Presentation;

public partial record MainModel(TasksService Tasks, INavigator Navigator)
{
    public IState<string> Search => State.Value(this, () => string.Empty);

    public IImmutableList<DoneFilter> Filters { get; } = [DoneFilter.All, DoneFilter.Open, DoneFilter.Done];

    public IState<DoneFilter> Filter => State.Value(this, () => DoneFilter.All);

    /// <summary>
    /// Reloads when the search or the filter changes, after any write (<see cref="TasksService.Changed"/>),
    /// and on the FeedView's Refresh. SelectAsync cancels the previous query, so a slow older
    /// search never overwrites a newer one.
    /// </summary>
    public IListFeed<TaskItem> Items => Feed
        .Combine(Search, Filter, Feed.Async(_ => ValueTask.FromResult(DateTime.UtcNow.Ticks), Tasks.Changed))
        .SelectAsync((query, ct) => Tasks.GetTasksAsync(new TaskQuery(query.Item1, TaskQuery.ToDone(query.Item2)), ct))
        .AsListFeed();

    public IFeed<TaskSummary> Summary => Feed.Async(Tasks.GetSummaryAsync, Tasks.Changed);

    public IFeed<string> Status => Feed.Async(Tasks.GetStatusAsync, Tasks.Changed);

    public async ValueTask NewTask(CancellationToken ct) =>
        await Navigator.NavigateDataAsync(this, new TaskEditRequest(null), cancellation: ct);
}
