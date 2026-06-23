namespace ClaudeCodeTracker.Presentation.Data;

/// <summary>
/// The single seeded dataset for the sample. Every page Model projects from here, so the
/// Dashboard, Sessions, Usage and Charts pages always tell one consistent story (e.g. the
/// Charts daily-cost line and sessions bar are derived from <see cref="DailyUsage"/>, the same
/// rollup the headline numbers summarize). Swap this out for a real service to go live.
///
/// All members are independent literals (no cross-references) so static initialization order
/// is never a concern.
/// </summary>
public static class SampleData
{
    // ── Period / budget headline ──────────────────────────────────────────
    public const string PeriodLabel = "This Month — June 2025";
    public const decimal TotalCostUsd = 47.83m;
    public const decimal BudgetLimitUsd = 100.00m;
    public const double BudgetUsedPercent = 47.83;
    public const long TotalTokens = 3_842_100;
    public const int TotalSessions = 134;
    public const int ActiveDays = 18;
    public const string ResetWindowLabel = "Monthly · Resets Jul 1";
    public const string ResetCountdown = "12 days remaining";
    public const double BudgetVsLastMonth = 18.4;
    public const bool BudgetVsLastMonthUp = true;

    // ── Plan / projections (Usage page) ───────────────────────────────────
    public const string CurrentPlan = "Pro";
    public const string PlanDescription = "Unlimited usage with priority access";
    public const decimal DailyCostAvg = 2.66m;
    public const decimal ProjectedMonthlyUsd = 81.46m;

    // ── Sessions (most recent first) ──────────────────────────────────────
    public static IReadOnlyList<SessionEntry> Sessions { get; } = new[]
    {
        new SessionEntry("s-001", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-19T14:22:00Z"), 47, 82_400, 18_900, 31_200, 12_000, 1.24m, "Completed", "refactor-auth-service", 38),
        new SessionEntry("s-002", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-19T11:05:00Z"), 23, 44_100, 9_800, 18_600, 5_400, 0.31m, "Completed", "unit-test-coverage", 21),
        new SessionEntry("s-003", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-18T16:48:00Z"), 61, 103_500, 24_200, 48_900, 18_300, 1.87m, "Completed", "api-integration-layer", 52),
        new SessionEntry("s-004", "claude-haiku-3-5", "Claude Haiku 3.5", DateTimeOffset.Parse("2025-06-18T09:30:00Z"), 12, 28_700, 6_400, 0, 0, 0.04m, "Completed", "quick-doc-fixes", 15),
        new SessionEntry("s-005", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-17T13:15:00Z"), 35, 67_200, 14_500, 29_800, 8_100, 0.52m, "Completed", "feature-planning", 29),
        new SessionEntry("s-006", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-17T08:50:00Z"), 54, 97_800, 21_600, 44_200, 16_500, 1.71m, "Completed", "database-schema-migration", 46),
        new SessionEntry("s-007", "claude-haiku-3-5", "Claude Haiku 3.5", DateTimeOffset.Parse("2025-06-16T15:30:00Z"), 8, 19_200, 4_100, 0, 0, 0.03m, "Completed", "changelog-update", 9),
        new SessionEntry("s-008", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-16T10:20:00Z"), 41, 78_300, 17_200, 34_100, 9_800, 0.63m, "Completed", "ci-pipeline-setup", 35),
        new SessionEntry("s-009", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-15T14:00:00Z"), 73, 124_600, 28_900, 57_800, 21_400, 2.24m, "Completed", "payment-gateway-integration", 63),
        new SessionEntry("s-010", "claude-haiku-3-5", "Claude Haiku 3.5", DateTimeOffset.Parse("2025-06-14T11:45:00Z"), 15, 33_400, 7_200, 0, 0, 0.05m, "Completed", "typo-sweep-docs", 12),
        new SessionEntry("s-011", "claude-sonnet-4-5", "Claude Sonnet 4.5", DateTimeOffset.Parse("2025-06-14T09:10:00Z"), 28, 52_900, 11_800, 23_400, 6_700, 0.44m, "Completed", "error-handling-layer", 24),
        new SessionEntry("s-012", "claude-opus-4-5", "Claude Opus 4.5", DateTimeOffset.Parse("2025-06-13T16:30:00Z"), 58, 109_200, 25_400, 51_600, 19_100, 1.96m, "Completed", "search-indexing-service", 49),
    };

    public static IReadOnlyList<string> ModelFilters { get; } = new[] { "All", "Opus", "Sonnet", "Haiku" };

    // ── Per-model breakdown (share of tokens) ─────────────────────────────
    public static IReadOnlyList<ModelUsageBreakdown> ModelBreakdown { get; } = new[]
    {
        new ModelUsageBreakdown("claude-opus-4-5", "Claude Opus 4.5", "Opus", 2_140_800, 28.64m, 74, 55.7),
        new ModelUsageBreakdown("claude-sonnet-4-5", "Claude Sonnet 4.5", "Sonnet", 1_302_500, 14.89m, 43, 33.9),
        new ModelUsageBreakdown("claude-haiku-3-5", "Claude Haiku 3.5", "Haiku", 398_800, 4.30m, 17, 10.4),
    };

    // ── Aggregate token split (Usage page) ────────────────────────────────
    public static TokenBreakdown Tokens { get; } = new(
        InputTokens: 2_318_400,
        OutputTokens: 538_700,
        CacheReadTokens: 782_400,
        CacheWriteTokens: 202_600,
        TotalTokens: 3_842_100,
        InputPercent: 60.3,
        OutputPercent: 14.0,
        CacheReadPercent: 20.4,
        CacheWritePercent: 5.3);

    // ── Rate limits (Usage page) ──────────────────────────────────────────
    public static IReadOnlyList<RateLimitInfo> RateLimits { get; } = new[]
    {
        new RateLimitInfo("Requests / min", 38, 60, 63.3, "—", "API calls in the last 60 seconds"),
        new RateLimitInfo("Tokens / min", 48_200, 80_000, 60.3, "—", "Token throughput in the last 60 seconds"),
        new RateLimitInfo("Tokens / day", 3_842_100, 10_000_000, 38.4, "Resets in 5h 12m", "Total daily token quota (all models)"),
        new RateLimitInfo("Requests / day", 134, 1_000, 13.4, "Resets in 5h 12m", "Total API request quota per day"),
    };

    // ── Daily rollup over the last 14 days (Charts page) ──────────────────
    public static IReadOnlyList<DailyUsage> DailyUsage { get; } = new[]
    {
        new DailyUsage(new DateOnly(2025, 6, 6),  180_400, 128_900,  51_500, 2.10m, 5),
        new DailyUsage(new DateOnly(2025, 6, 7),  291_700, 207_400,  84_300, 3.44m, 7),
        new DailyUsage(new DateOnly(2025, 6, 8),  246_800, 175_300,  71_500, 2.91m, 6),
        new DailyUsage(new DateOnly(2025, 6, 9),        0,       0,       0, 0.00m, 0),
        new DailyUsage(new DateOnly(2025, 6, 10), 402_100, 285_900, 116_200, 4.80m, 9),
        new DailyUsage(new DateOnly(2025, 6, 11), 441_300, 313_700, 127_600, 5.20m, 10),
        new DailyUsage(new DateOnly(2025, 6, 12),       0,       0,       0, 0.00m, 0),
        new DailyUsage(new DateOnly(2025, 6, 13), 410_200, 290_100, 120_100, 4.21m, 8),
        new DailyUsage(new DateOnly(2025, 6, 14), 528_900, 376_400, 152_500, 5.87m, 11),
        new DailyUsage(new DateOnly(2025, 6, 15),       0,       0,       0, 0.00m, 0),
        new DailyUsage(new DateOnly(2025, 6, 16), 302_400, 214_800,  87_600, 3.14m, 6),
        new DailyUsage(new DateOnly(2025, 6, 17), 614_700, 437_200, 177_500, 6.92m, 13),
        new DailyUsage(new DateOnly(2025, 6, 18), 891_300, 634_100, 257_200, 9.44m, 18),
        new DailyUsage(new DateOnly(2025, 6, 19), 1_094_600, 778_900, 315_700, 12.08m, 21),
    };
}
