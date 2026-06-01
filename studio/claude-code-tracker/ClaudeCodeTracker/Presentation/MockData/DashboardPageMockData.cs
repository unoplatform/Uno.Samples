namespace ClaudeCodeTracker.Presentation.MockData;

public partial record DashboardData(
    string PeriodLabel,
    decimal TotalCostUsd,
    decimal BudgetLimitUsd,
    double BudgetUsedPercent,
    long TotalTokens,
    int TotalSessions,
    int ActiveDays,
    string ResetWindowLabel,
    string ResetCountdown,
    string TopModel,
    IReadOnlyList<SessionEntry> RecentSessions,
    IReadOnlyList<DailyUsage> WeeklyUsage,
    IReadOnlyList<ModelUsageBreakdown> ModelBreakdown,
    double BudgetVsLastMonth,
    bool BudgetVsLastMonthUp);

public static class DashboardPageMockData
{
    public static DashboardData Data { get; } = new(
        PeriodLabel: "This Month — June 2025",
        TotalCostUsd: 47.83m,
        BudgetLimitUsd: 100.00m,
        BudgetUsedPercent: 47.83,
        TotalTokens: 3_842_100,
        TotalSessions: 134,
        ActiveDays: 18,
        ResetWindowLabel: "Monthly · Resets Jul 1",
        ResetCountdown: "12 days remaining",
        TopModel: "claude-opus-4-5",
        RecentSessions: new[]
        {
            new SessionEntry("s-001", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-19T14:22:00Z"), 47, 82_400, 18_900, 31_200, 12_000, 1.24m, "Completed", "refactor-auth-service", 38),
            new SessionEntry("s-002", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-19T11:05:00Z"), 23, 44_100, 9_800, 18_600, 5_400, 0.31m, "Completed", "unit-test-coverage", 21),
            new SessionEntry("s-003", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-18T16:48:00Z"), 61, 103_500, 24_200, 48_900, 18_300, 1.87m, "Completed", "api-integration-layer", 52),
            new SessionEntry("s-004", "claude-haiku-3-5", "Claude Haiku 3.5", DateTimeOffset.Parse("2025-06-18T09:30:00Z"), 12, 28_700, 6_400, 0, 0, 0.04m, "Completed", "quick-doc-fixes", 15),
            new SessionEntry("s-005", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-17T13:15:00Z"), 35, 67_200, 14_500, 29_800, 8_100, 0.52m, "Completed", "feature-planning", 29),
        },
        WeeklyUsage: new[]
        {
            new DailyUsage(new DateOnly(2025, 6, 13), 410_200, 290_100, 120_100, 4.21m, 8),
            new DailyUsage(new DateOnly(2025, 6, 14), 528_900, 376_400, 152_500, 5.87m, 11),
            new DailyUsage(new DateOnly(2025, 6, 15), 0, 0, 0, 0m, 0),
            new DailyUsage(new DateOnly(2025, 6, 16), 302_400, 214_800, 87_600, 3.14m, 6),
            new DailyUsage(new DateOnly(2025, 6, 17), 614_700, 437_200, 177_500, 6.92m, 13),
            new DailyUsage(new DateOnly(2025, 6, 18), 891_300, 634_100, 257_200, 9.44m, 18),
            new DailyUsage(new DateOnly(2025, 6, 19), 1_094_600, 778_900, 315_700, 12.08m, 21),
        },
        ModelBreakdown: new[]
        {
            new ModelUsageBreakdown("claude-opus-4-5", "Claude Opus 4.5", "Opus", 2_140_800, 28.64m, 74, 55.7),
            new ModelUsageBreakdown("claude-sonnet-4-5", "Claude Sonnet 4.5", "Sonnet", 1_302_500, 14.89m, 43, 33.9),
            new ModelUsageBreakdown("claude-haiku-3-5", "Claude Haiku 3.5", "Haiku", 398_800, 4.30m, 17, 10.4),
        },
        BudgetVsLastMonth: 18.4,
        BudgetVsLastMonthUp: true
    );
}
