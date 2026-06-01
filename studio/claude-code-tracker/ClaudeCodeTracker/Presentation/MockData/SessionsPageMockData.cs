namespace ClaudeCodeTracker.Presentation.MockData;

public partial record SessionsData(
    int TotalCount,
    string ActiveFilter,
    IReadOnlyList<string> FilterOptions,
    IReadOnlyList<SessionEntry> Sessions);

public static class SessionsPageMockData
{
    public static SessionsData Data { get; } = new(
        TotalCount: 134,
        ActiveFilter: "All",
        FilterOptions: new[] { "All", "Opus", "Sonnet", "Haiku" },
        Sessions: new[]
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
        }
    );
}
