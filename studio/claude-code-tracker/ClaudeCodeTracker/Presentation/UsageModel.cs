namespace ClaudeCodeTracker.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record UsageModel
{
    public string CurrentPlan => SampleData.CurrentPlan;
    public string PlanDescription => SampleData.PlanDescription;

    public string TotalCostDisplay => Fmt.Money(SampleData.TotalCostUsd);
    public string DailyCostAvgDisplay => Fmt.Money(SampleData.DailyCostAvg);
    public string ProjectedMonthlyDisplay => Fmt.Money(SampleData.ProjectedMonthlyUsd);

    public TokenBreakdown Tokens => SampleData.Tokens;
    public IReadOnlyList<RateLimitInfo> RateLimits => SampleData.RateLimits;
    public IReadOnlyList<ModelUsageBreakdown> ModelBreakdown => SampleData.ModelBreakdown;
    public IReadOnlyList<ModelInfo> ModelPricing => ModelCatalog.All;
}
