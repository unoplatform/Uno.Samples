namespace ClaudeCodeTracker.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="UsagePage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="DashboardPageMockData"/>. The plan strings, the spend totals and the
/// token-breakdown payload are plain materialized values (the page binds paths off
/// <see cref="Tokens"/>); the three tables stay list feeds because FeedViews render them.
/// </summary>
[ReactiveBindable]
public partial record UsagePageMockData
{
    // Declared first: the statics below construct instances that read this (rule 3). A fresh account
    // has spent nothing, so every token bucket is zero.
    private static readonly TokenBreakdown EmptyTokens = new(0, 0, 0, 0, 0, 0d, 0d, 0d, 0d);

    /// <summary>A Pro account mid-month: real spend, three models in use, limits part-consumed.</summary>
    public static UsagePageMockDataViewModel Data => new();

    /// <summary>
    /// A brand-new account: nothing spent, no tokens, and all three tables on their empty state. The
    /// totals at the top agree with the empty tables below, so the preview is one coherent situation
    /// rather than a populated header over blank panels.
    /// </summary>
    public static UsagePageMockDataViewModel FreshAccount =>
        UsagePageMockDataViewModel.ForModel(new()
        {
            TotalCostDisplay = "$0.00",
            DailyCostAvgDisplay = "$0.00",
            ProjectedMonthlyDisplay = "$0.00",
            Tokens = EmptyTokens,
            RateLimits = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<RateLimitInfo>>(
                ImmutableList<RateLimitInfo>.Empty)),
            ModelBreakdown = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ModelUsageBreakdown>>(
                ImmutableList<ModelUsageBreakdown>.Empty)),
            ModelPricing = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ModelInfo>>(
                ImmutableList<ModelInfo>.Empty)),
        });

    public string CurrentPlan { get; init; } = SampleData.CurrentPlan;
    public string PlanDescription { get; init; } = SampleData.PlanDescription;

    public string TotalCostDisplay { get; init; } = Fmt.Money(SampleData.TotalCostUsd);
    public string DailyCostAvgDisplay { get; init; } = Fmt.Money(SampleData.DailyCostAvg);
    public string ProjectedMonthlyDisplay { get; init; } = Fmt.Money(SampleData.ProjectedMonthlyUsd);

    public TokenBreakdown Tokens { get; init; } = SampleData.Tokens;

    // Inline lambdas that capture nothing — they read the seed classes directly — so all instances of
    // this mock share one feed each (rule 4).
    public IListFeed<RateLimitInfo> RateLimits { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(SampleData.RateLimits.ToImmutableList()));

    public IListFeed<ModelUsageBreakdown> ModelBreakdown { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(SampleData.ModelBreakdown.ToImmutableList()));

    public IListFeed<ModelInfo> ModelPricing { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(ModelCatalog.All.ToImmutableList()));
}

// Reaches the generated ViewModel's protected model-taking constructor, so FreshAccount can wrap a
// customized model. See DashboardPageMockDataViewModel for the full explanation.
public partial class UsagePageMockDataViewModel
{
    internal static UsagePageMockDataViewModel ForModel(UsagePageMockData model) => new(model);
}
