namespace ClaudeCodeTracker.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record DashboardModel
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

    public IReadOnlyList<SessionEntry> RecentSessions => SampleData.Sessions.Take(5).ToList();
    public IReadOnlyList<ModelUsageBreakdown> ModelBreakdown => SampleData.ModelBreakdown;

    // Budget-vs-last-month trend flag. "Up" means spend rose vs last month (the unwanted direction
    // for a cost tracker): XAML shows the error-tinted up-arrow variant and hides the down one (and
    // vice-versa) by binding both to this flag through a BooleanToVisibility converter. Each variant
    // carries its own {ThemeResource} foreground so the tint re-themes on a light/dark switch.
    // Exposing the plain bool (not a Visibility) keeps the view decision in XAML.
    public bool BudgetVsLastMonthUp => SampleData.BudgetVsLastMonthUp;
    public string TrendDeltaDisplay =>
        $"{(BudgetVsLastMonthUp ? "+" : "−")}{Fmt.Percent(SampleData.BudgetVsLastMonth)}% vs last month";
}
