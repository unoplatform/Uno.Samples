using Uno.Extensions.Reactive;

namespace GridWatch.Presentation;

public partial record AlertsModel(IGridDataService DataService)
{
	public IState<string> SelectedFilter => State.Value(this, () => "All");

	public IListFeed<Alert> FilteredAlerts =>
		SelectedFilter
			.SelectAsync(async (filter, ct) =>
			{
				var all = await DataService.GetAlertsAsync(ct);
				var f = filter ?? "All";
				return f == "All"
					? all
					: (IImmutableList<Alert>)all.Where(a => a.Severity.ToString() == f).ToImmutableList();
			})
			.AsListFeed();

	public IFeed<int> CriticalCount =>
		Feed.Async(async ct => (await DataService.GetAlertsAsync(ct)).Count(a => a.Severity == AlertSeverity.Critical));

	public async ValueTask FilterAll(CancellationToken ct)
		=> await SelectedFilter.Set("All", ct);

	public async ValueTask FilterCritical(CancellationToken ct)
		=> await SelectedFilter.Set("Critical", ct);

	public async ValueTask FilterWarning(CancellationToken ct)
		=> await SelectedFilter.Set("Warning", ct);

	public async ValueTask FilterInfo(CancellationToken ct)
		=> await SelectedFilter.Set("Info", ct);
}
