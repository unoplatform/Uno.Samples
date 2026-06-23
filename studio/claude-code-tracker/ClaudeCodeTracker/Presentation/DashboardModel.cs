using Microsoft.UI.Xaml.Media;

namespace ClaudeCodeTracker.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record DashboardModel
{
    public DashboardModel()
    {
        // "Up" means spend rose vs last month — the unwanted direction for a cost tracker —
        // so it reads in the error tint; "down" reads in the positive (secondary) tint.
        // Resolved here (after the theme is applied) rather than hardcoded in XAML, with a
        // safe fallback per lesson 16.
        TrendBrush = ResolveBrush(
            SampleData.BudgetVsLastMonthUp ? "ErrorBrush" : "SecondaryBrush",
            SampleData.BudgetVsLastMonthUp ? Microsoft.UI.Colors.IndianRed : Microsoft.UI.Colors.Teal);
    }

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

    // Budget-vs-last-month trend indicator, driven entirely off the data flag.
    public bool BudgetVsLastMonthUp => SampleData.BudgetVsLastMonthUp;
    public string TrendGlyph => BudgetVsLastMonthUp ? "\uE70E" : "\uE70D"; // chevron up / down
    public string TrendDeltaDisplay =>
        $"{(BudgetVsLastMonthUp ? "+" : "−")}{Fmt.Percent(SampleData.BudgetVsLastMonth)}% vs last month";
    public Brush TrendBrush { get; }

    private static Brush ResolveBrush(string key, Windows.UI.Color fallback) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is Brush brush
            ? brush
            : new SolidColorBrush(fallback);
}
