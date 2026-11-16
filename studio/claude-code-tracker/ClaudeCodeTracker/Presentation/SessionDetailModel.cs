namespace ClaudeCodeTracker.Presentation;

/// <summary>
/// Detail view of the session the user tapped. The <see cref="SessionEntry"/> is supplied by
/// navigation (registered as a DataViewMap), and the per-token-type costs / cache stats / topic
/// tags are derived here from that entry and the <see cref="ModelCatalog"/> pricing — so every
/// row opens its own detail rather than a single hardcoded session.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record SessionDetailModel
{
    public SessionDetailModel(SessionEntry session)
    {
        Session = session;
        var pricing = ModelCatalog.Pricing(session.ModelId);

        InputCostUsd = Cost(session.InputTokens, pricing.InputPricePer1M);
        OutputCostUsd = Cost(session.OutputTokens, pricing.OutputPricePer1M);
        CacheReadCostUsd = Cost(session.CacheReadTokens, pricing.CacheReadPricePer1M);
        CacheWriteCostUsd = Cost(session.CacheWriteTokens, pricing.CacheWritePricePer1M);

        TotalTokens = session.InputTokens + session.OutputTokens
            + session.CacheReadTokens + session.CacheWriteTokens;

        var cacheable = session.InputTokens + session.CacheReadTokens;
        CacheHitRatePercent = cacheable == 0
            ? 0
            : Math.Round(session.CacheReadTokens * 100.0 / cacheable, 1);

        // Caching bills cached input at the (cheaper) cache-read rate instead of the full
        // input rate — the saving is the rate delta applied to the cached tokens.
        EstimatedSavingsUsd = Cost(session.CacheReadTokens, pricing.InputPricePer1M - pricing.CacheReadPricePer1M);
        CacheSavingsPercent = pricing.InputPricePer1M == 0
            ? 0
            : Math.Round((double)((pricing.InputPricePer1M - pricing.CacheReadPricePer1M) / pricing.InputPricePer1M) * 100, 1);

        TopicTags = BuildTags(session);
    }

    public SessionEntry Session { get; }

    public decimal InputCostUsd { get; }
    public decimal OutputCostUsd { get; }
    public decimal CacheReadCostUsd { get; }
    public decimal CacheWriteCostUsd { get; }
    public long TotalTokens { get; }
    public double CacheHitRatePercent { get; }
    public double CacheSavingsPercent { get; }
    public decimal EstimatedSavingsUsd { get; }
    public IReadOnlyList<string> TopicTags { get; }

    public string InputCostDisplay => Fmt.Money(InputCostUsd);
    public string OutputCostDisplay => Fmt.Money(OutputCostUsd);
    public string CacheReadCostDisplay => Fmt.Money(CacheReadCostUsd);
    public string CacheWriteCostDisplay => Fmt.Money(CacheWriteCostUsd);
    public string TotalTokensDisplay => Fmt.Tokens(TotalTokens);
    public string EstimatedSavingsDisplay => Fmt.Money(EstimatedSavingsUsd);

    private static decimal Cost(long tokens, decimal pricePer1M) =>
        Math.Round(tokens / 1_000_000m * pricePer1M, 2);

    private static IReadOnlyList<string> BuildTags(SessionEntry session)
    {
        var tags = session.ProjectName
            .Split('-', StringSplitOptions.RemoveEmptyEntries)
            .ToList();
        tags.Add(ModelCatalog.Pricing(session.ModelId).ShortName.ToLowerInvariant());
        return tags;
    }
}
