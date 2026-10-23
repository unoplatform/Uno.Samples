namespace ClaudeCodeTracker.Presentation;

// The at-a-glance page. Its two collections come from ITrackerService as list feeds, so the page
// renders them through FeedViews with real loading, empty and failure states.
//
// The headline figures stay plain computed properties rather than feeds. They are compile-time
// constants of the seed reduced to display strings, and a scalar that can never be absent or fail
// binds straight to TextBlock.Text — wrapping a stat label in a four-template control would be pure
// ceremony (and a formatted total is exactly the case the guidance calls out).
public partial record DashboardModel(ITrackerService Tracker)
{
    public string PeriodLabel => SampleData.PeriodLabel;

    public string TotalCostDisplay => Fmt.Money(SampleData.TotalCostUsd);
    public string BudgetLimitDisplay => Fmt.Money(SampleData.BudgetLimitUsd);
    public double BudgetUsedPercent => SampleData.BudgetUsedPercent;
    public string BudgetUsedDisplay => $"{SampleData.BudgetUsedPercent:0}%";

    public string TotalTokensDisplay => Fmt.Tokens(SampleData.TotalTokens);
    public string TotalSessionsDisplay => Fmt.Count(SampleData.TotalSessions);
    public string ActiveDaysDisplay => Fmt.Count(SampleData.ActiveDays);

    public decimal AvgCostPerSession => SampleData.TotalSessions == 0
        ? 0m
        : SampleData.TotalCostUsd / SampleData.TotalSessions;
    public string AvgCostDisplay => Fmt.Money(AvgCostPerSession);

    public string ResetWindowLabel => SampleData.ResetWindowLabel;
    public string ResetCountdown => SampleData.ResetCountdown;

    /// <summary>The newest five sessions, rendered through a FeedView.</summary>
    public IListFeed<SessionEntry> RecentSessions =>
        ListFeed.Async(ct => Tracker.GetRecentSessionsAsync(RecentSessionCount, ct));

    /// <summary>Per-model spend and token share, rendered through a FeedView.</summary>
    public IListFeed<ModelUsageBreakdown> ModelBreakdown =>
        ListFeed.Async(Tracker.GetModelBreakdownAsync);

    // Budget-vs-last-month trend flag. "Up" means spend rose vs last month (the unwanted direction
    // for a cost tracker): XAML shows the error-tinted up-arrow variant and hides the down one (and
    // vice-versa) by binding both to this flag through a BooleanToVisibility converter. Each variant
    // carries its own {ThemeResource} foreground so the tint re-themes on a light/dark switch.
    // Exposing the plain bool (not a Visibility) keeps the view decision in XAML.
    public bool BudgetVsLastMonthUp => SampleData.BudgetVsLastMonthUp;
    public string TrendDeltaDisplay =>
        $"{(BudgetVsLastMonthUp ? "+" : "−")}{Fmt.Percent(SampleData.BudgetVsLastMonth)}% vs last month";

    private const int RecentSessionCount = 5;
}
