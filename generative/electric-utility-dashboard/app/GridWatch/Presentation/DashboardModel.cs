using Uno.Extensions.Reactive;
using Uno.Extensions.Reactive.Bindings;

namespace GridWatch.Presentation;

public partial record DashboardModel(IGridDataService DataService, IThemeService ThemeService)
{
	public IState<bool> IsDark => State.Value(this, () => ThemeService.IsDark);

	public async ValueTask ToggleTheme(CancellationToken ct)
	{
		var isDark = await IsDark;
		await ThemeService.SetThemeAsync(isDark ? AppTheme.Light : AppTheme.Dark);
		await IsDark.UpdateAsync(_ => !isDark, ct);
	}

	public IListFeed<KpiMetric> KpiMetrics => ListFeed.Async(DataService.GetKpiMetricsAsync);

	public IListFeed<Facility> Facilities => ListFeed.Async(DataService.GetFacilitiesAsync);

	public IListFeed<Alert> Alerts => ListFeed.Async(DataService.GetAlertsAsync);

	public IFeed<int> FacilityCount => Facilities.AsFeed().Select(list => list.Count);

	public IFeed<int> CriticalAlertCount => Alerts.AsFeed().Select(list => list.Count(a => a.Severity == AlertSeverity.Critical));
}
