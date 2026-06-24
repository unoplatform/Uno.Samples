using Microsoft.UI.Xaml;

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

    // Budget-vs-last-month trend indicator, driven entirely off the data flag. "Up" means spend
    // rose vs last month (the unwanted direction for a cost tracker) so the XAML shows it in the
    // error tint; "down" shows in the positive (secondary) tint. Two visibility-toggled variants
    // (each with its own {ThemeResource} foreground in XAML) replace a code-resolved brush so the
    // tint re-themes on a light/dark switch (a brush captured once in code-behind would not update
    // when the theme changes).
    public bool BudgetVsLastMonthUp => SampleData.BudgetVsLastMonthUp;
    public Visibility TrendUpVisibility => BudgetVsLastMonthUp ? Visibility.Visible : Visibility.Collapsed;
    public Visibility TrendDownVisibility => BudgetVsLastMonthUp ? Visibility.Collapsed : Visibility.Visible;
    public string TrendGlyph => BudgetVsLastMonthUp ? "\uE70E" : "\uE70D"; // chevron up / down
    public string TrendDeltaDisplay =>
        $"{(BudgetVsLastMonthUp ? "+" : "−")}{Fmt.Percent(SampleData.BudgetVsLastMonth)}% vs last month";
}
