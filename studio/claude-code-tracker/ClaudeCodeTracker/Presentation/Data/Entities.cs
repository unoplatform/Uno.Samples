namespace ClaudeCodeTracker.Presentation.Data;

// Core session entity — used on Dashboard, Sessions, and SessionDetail pages.
// Display-formatted members keep view bindings simple and consistent (see Fmt).
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
    int MessageCount)
{
    public string CostDisplay => Fmt.Money(CostUsd);
    public string InputTokensDisplay => Fmt.Tokens(InputTokens);
    public string OutputTokensDisplay => Fmt.Tokens(OutputTokens);
    public string CacheReadTokensDisplay => Fmt.Tokens(CacheReadTokens);
    public string CacheWriteTokensDisplay => Fmt.Tokens(CacheWriteTokens);
    public string StartedAtDisplay => Fmt.DateTimeShort(StartedAt);
}

// Model pricing info (per 1M tokens) — used on Usage page and to derive per-session costs.
public partial record ModelInfo(
    string ModelId,
    string DisplayName,
    string ShortName,
    decimal InputPricePer1M,
    decimal OutputPricePer1M,
    decimal CacheReadPricePer1M,
    decimal CacheWritePricePer1M)
{
    public string InputPriceDisplay => Fmt.Money(InputPricePer1M);
    public string OutputPriceDisplay => Fmt.Money(OutputPricePer1M);
    public string CacheReadPriceDisplay => Fmt.Money(CacheReadPricePer1M);
    public string CacheWritePriceDisplay => Fmt.Money(CacheWritePricePer1M);
}

// Daily usage rollup — the single source for the Charts daily-cost line and sessions bar.
public partial record DailyUsage(
    DateOnly Date,
    long TotalTokens,
    long InputTokens,
    long OutputTokens,
    decimal CostUsd,
    int SessionCount)
{
    public string DayLabel => Date.ToString("MMM d", System.Globalization.CultureInfo.CurrentCulture);
}

// Per-model usage breakdown (share of tokens) — used on Dashboard and Usage pages.
public partial record ModelUsageBreakdown(
    string ModelId,
    string ModelDisplayName,
    string ShortName,
    long TotalTokens,
    decimal TotalCostUsd,
    int SessionCount,
    double SharePercent)
{
    public string TotalCostDisplay => Fmt.Money(TotalCostUsd);
    public string SharePercentDisplay => Fmt.Percent(SharePercent);
}

// Aggregate token split — used on the Usage page token-breakdown card.
public partial record TokenBreakdown(
    long InputTokens,
    long OutputTokens,
    long CacheReadTokens,
    long CacheWriteTokens,
    long TotalTokens,
    double InputPercent,
    double OutputPercent,
    double CacheReadPercent,
    double CacheWritePercent)
{
    public string InputTokensDisplay => Fmt.Tokens(InputTokens);
    public string OutputTokensDisplay => Fmt.Tokens(OutputTokens);
    public string CacheReadTokensDisplay => Fmt.Tokens(CacheReadTokens);
    public string CacheWriteTokensDisplay => Fmt.Tokens(CacheWriteTokens);
}

// A single rate-limit gauge — used on the Usage page.
public partial record RateLimitInfo(
    string LimitType,
    long Used,
    long Limit,
    double UsedPercent,
    string ResetIn,
    string Description);
