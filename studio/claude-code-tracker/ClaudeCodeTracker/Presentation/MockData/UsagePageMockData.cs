namespace ClaudeCodeTracker.Presentation.MockData;

public partial record TokenBreakdown(
    long InputTokens,
    long OutputTokens,
    long CacheReadTokens,
    long CacheWriteTokens,
    long TotalTokens,
    double InputPercent,
    double OutputPercent,
    double CacheReadPercent,
    double CacheWritePercent);

public partial record RateLimitInfo(
    string LimitType,
    long Used,
    long Limit,
    double UsedPercent,
    string ResetIn,
    string Description);

public partial record CostProjection(
    string Label,
    decimal ProjectedUsd,
    decimal ActualUsd,
    bool IsProjected);

public partial record UsageData(
    string CurrentPlan,
    string PlanDescription,
    TokenBreakdown Tokens,
    decimal TotalCostUsd,
    decimal DailyCostAvg,
    decimal ProjectedMonthlyUsd,
    IReadOnlyList<RateLimitInfo> RateLimits,
    IReadOnlyList<CostProjection> CostHistory,
    IReadOnlyList<ModelInfo> ModelPricing,
    IReadOnlyList<ModelUsageBreakdown> ModelBreakdown);

public static class UsagePageMockData
{
    public static UsageData Data { get; } = new(
        CurrentPlan: "Pro",
        PlanDescription: "Unlimited usage with priority access",
        Tokens: new TokenBreakdown(
            InputTokens: 2_318_400,
            OutputTokens: 538_700,
            CacheReadTokens: 782_400,
            CacheWriteTokens: 202_600,
            TotalTokens: 3_842_100,
            InputPercent: 60.3,
            OutputPercent: 14.0,
            CacheReadPercent: 20.4,
            CacheWritePercent: 5.3),
        TotalCostUsd: 47.83m,
        DailyCostAvg: 2.66m,
        ProjectedMonthlyUsd: 81.46m,
        RateLimits: new[]
        {
            new RateLimitInfo("Requests / min", 38, 60, 63.3, "—", "API calls in the last 60 seconds"),
            new RateLimitInfo("Tokens / min", 48_200, 80_000, 60.3, "—", "Token throughput in the last 60 seconds"),
            new RateLimitInfo("Tokens / day", 3_842_100, 10_000_000, 38.4, "Resets in 5h 12m", "Total daily token quota (all models)"),
            new RateLimitInfo("Requests / day", 134, 1_000, 13.4, "Resets in 5h 12m", "Total API request quota per day"),
        },
        CostHistory: new[]
        {
            new CostProjection("March 2025", 0m, 31.22m, false),
            new CostProjection("April 2025", 0m, 38.74m, false),
            new CostProjection("May 2025", 0m, 44.19m, false),
            new CostProjection("June 2025 (so far)", 0m, 47.83m, false),
            new CostProjection("June 2025 (projected)", 81.46m, 0m, true),
        },
        ModelPricing: new[]
        {
            new ModelInfo("claude-opus-4-5", "Claude Opus 4.5", "Opus", 15.00m, 75.00m, 1.50m, 3.75m),
            new ModelInfo("claude-sonnet-4-5", "Claude Sonnet 4.5", "Sonnet", 3.00m, 15.00m, 0.30m, 0.75m),
            new ModelInfo("claude-haiku-3-5", "Claude Haiku 3.5", "Haiku", 0.80m, 4.00m, 0.08m, 0.20m),
        },
        ModelBreakdown: new[]
        {
            new ModelUsageBreakdown("claude-opus-4-5", "Claude Opus 4.5", "Opus", 2_140_800, 28.64m, 74, 55.7),
            new ModelUsageBreakdown("claude-sonnet-4-5", "Claude Sonnet 4.5", "Sonnet", 1_302_500, 14.89m, 43, 33.9),
            new ModelUsageBreakdown("claude-haiku-3-5", "Claude Haiku 3.5", "Haiku", 398_800, 4.30m, 17, 10.4),
        }
    );
}
