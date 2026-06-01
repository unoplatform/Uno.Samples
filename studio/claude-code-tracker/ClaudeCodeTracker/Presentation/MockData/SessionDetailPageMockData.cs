namespace ClaudeCodeTracker.Presentation.MockData;

public partial record SessionDetailData(
    SessionEntry Session,
    decimal InputCostUsd,
    decimal OutputCostUsd,
    decimal CacheReadCostUsd,
    decimal CacheWriteCostUsd,
    long TotalTokens,
    double CacheHitRatePercent,
    double CacheSavingsPercent,
    decimal EstimatedSavingsUsd,
    IReadOnlyList<string> TopicTags);

public static class SessionDetailPageMockData
{
    public static SessionDetailData Data { get; } = new(
        Session: new SessionEntry(
            "s-001",
            "claude-opus-4-5",
            "Claude Opus 4.5",
            DateTimeOffset.Parse("2025-06-19T14:22:00Z"),
            47,
            82_400,
            18_900,
            31_200,
            12_000,
            1.24m,
            "Completed",
            "refactor-auth-service",
            38),
        InputCostUsd: 0.79m,
        OutputCostUsd: 0.92m,
        CacheReadCostUsd: 0.03m,
        CacheWriteCostUsd: 0.03m,
        TotalTokens: 144_500,
        CacheHitRatePercent: 27.4,
        CacheSavingsPercent: 78.5,
        EstimatedSavingsUsd: 0.47m,
        TopicTags: new[] { "auth", "refactor", "typescript", "jwt", "middleware", "unit-tests" }
    );
}
