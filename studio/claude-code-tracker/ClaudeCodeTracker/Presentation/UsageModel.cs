namespace ClaudeCodeTracker.Presentation;

// Plan, spend and limits. The three tables come from ITrackerService as list feeds and render
// through FeedViews; the plan strings, the spend totals and the token-breakdown payload stay plain
// computed properties, because a scalar that can never be absent or fail binds straight to
// TextBlock.Text and gains nothing from a four-template control.
public partial record UsageModel(ITrackerService Tracker)
{
    public string CurrentPlan => SampleData.CurrentPlan;
    public string PlanDescription => SampleData.PlanDescription;

    public string TotalCostDisplay => Fmt.Money(SampleData.TotalCostUsd);
    public string DailyCostAvgDisplay => Fmt.Money(SampleData.DailyCostAvg);
    public string ProjectedMonthlyDisplay => Fmt.Money(SampleData.ProjectedMonthlyUsd);

    public TokenBreakdown Tokens => SampleData.Tokens;

    /// <summary>Current plan rate limits, rendered through a FeedView.</summary>
    public IListFeed<RateLimitInfo> RateLimits => ListFeed.Async(Tracker.GetRateLimitsAsync);

    /// <summary>Per-model spend and token share, rendered through a FeedView.</summary>
    public IListFeed<ModelUsageBreakdown> ModelBreakdown => ListFeed.Async(Tracker.GetModelBreakdownAsync);

    /// <summary>The model price list, rendered through a FeedView.</summary>
    public IListFeed<ModelInfo> ModelPricing => ListFeed.Async(Tracker.GetModelPricingAsync);
}
