namespace ClaudeCodeTracker.Presentation.MockData;

// Core session entity — used on Dashboard, Sessions, and SessionDetail pages
public partial record SessionEntry(
    string Id,
    string ModelId,
    string ModelDisplayName,
    DateTimeOffset StartedAt,
    int DurationMinutes,
    long InputTokens,
    long OutputTokens,
    long CacheReadTokens,
    long CacheWriteTokens,
    decimal CostUsd,
    string Status,
    string ProjectName,
    int MessageCount);

// Model pricing info — used on Dashboard and Usage pages
public partial record ModelInfo(
    string ModelId,
    string DisplayName,
    string ShortName,
    decimal InputPricePer1M,
    decimal OutputPricePer1M,
    decimal CacheReadPricePer1M,
    decimal CacheWritePricePer1M);

// Daily usage rollup — used on Dashboard and Charts pages
public partial record DailyUsage(
    DateOnly Date,
    long TotalTokens,
    long InputTokens,
    long OutputTokens,
    decimal CostUsd,
    int SessionCount);

// Model usage breakdown — used on Usage and Charts pages
public partial record ModelUsageBreakdown(
    string ModelId,
    string ModelDisplayName,
    string ShortName,
    long TotalTokens,
    decimal TotalCostUsd,
    int SessionCount,
    double SharePercent);
