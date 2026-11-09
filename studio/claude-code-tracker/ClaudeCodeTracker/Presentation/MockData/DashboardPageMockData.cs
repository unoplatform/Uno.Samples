namespace ClaudeCodeTracker.Presentation.MockData;

// The recipe every mock in this folder follows.
//
// Presentation/MockData holds one design-time DataContext per page, for Hot Design / Studio, and
// nothing else references them: the previews bind these statics in XAML, and no page constructor
// seeds one. DashboardPageMockData below is the reference; its siblings are built to the same four
// rules, recorded here once and cited from each of them.
//
// 1. The statics hand out the GENERATED ViewModel, never the record. Each of these pages renders at
//    least one FeedView, and a FeedView can only subscribe to a FEED — a plain list would never reach
//    it and the preview would sit on the empty state. So each mock is [ReactiveBindable], and the
//    ViewModel the analyzer emits for it creates, in its constructor, the SourceContext that makes
//    those feeds pump and materializes them into bindable lists.
//
// 2. Those statics are expression-bodied — a fresh instance per access, never a cached singleton. A
//    generated ViewModel has a view-scoped lifecycle: its SourceContext is created with the instance
//    and disposed when the hosting view unloads. A shared singleton can be built before the design
//    host's dispatcher is ready, or be already dead from a previous render, leaving feeds that never
//    emit.
//
// 3. Every static input is declared ABOVE the statics that construct instances. Static members
//    initialize in textual order, and an instance initializer that reads a not-yet-assigned static
//    field gets null with no exception at all.
//
// 4. And the load-bearing one: every feed is built by an INLINE lambda that CAPTURES NOTHING — it
//    reads static seed data directly. A no-capture lambda's delegate is cached by the compiler, and
//    ListFeed.Async caches the feed it builds against that delegate instance, so every instance of a
//    mock shares ONE feed, created once. Routing these through a shared helper that hoisted the
//    payload into a local would mint a fresh delegate, and so a fresh feed, per mock instance — and
//    those previews render their lists empty. Do not "deduplicate" these lambdas into a helper.
//
// A variant needs the generated ViewModel's model-taking constructor, which is protected, so each
// file also carries a small partial on its ViewModel exposing a factory that reaches it.
//
// These mocks exist because the pages read their data through ITrackerService: the runtime Models take
// the service by constructor injection, so a page cannot build one, and the design-time DataContext
// has to come from here via the preview XAML.

/// <summary>
/// Design-time DataContext for <see cref="DashboardPage"/> in Hot Design / Studio, and the reference
/// mock for this folder — the recipe above applies to all of them. The headline figures are plain
/// materialized values, exactly as DashboardModel exposes them; the model-usage and recent-session
/// strips stay list feeds because FeedViews render them.
/// </summary>
[ReactiveBindable]
public partial record DashboardPageMockData
{
    // Declared first: the statics below construct instances that read this (rule 3).
    private const int RecentCount = 5;

    /// <summary>A busy month: budget approaching half, all three models in use, a full activity strip.</summary>
    public static DashboardPageMockDataViewModel Data => new();

    /// <summary>
    /// A month that has barely started: a few dollars spent, a budget bar just off zero, and both
    /// strips on their empty state. The figures at the top change with the lists at the bottom, so the
    /// two previews differ in the first screenful rather than only below the fold.
    /// </summary>
    public static DashboardPageMockDataViewModel QuietMonth =>
        DashboardPageMockDataViewModel.ForModel(new()
        {
            PeriodLabel = "This Month — July 2025",
            TotalCostDisplay = "$1.12",
            BudgetUsedPercent = 1.12,
            BudgetUsedDisplay = "1%",
            TotalTokensDisplay = "84,600",
            TotalSessionsDisplay = "3",
            ActiveDaysDisplay = "2",
            AvgCostDisplay = "$0.37",
            ResetCountdown = "29 days remaining",
            BudgetVsLastMonthUp = false,
            TrendDeltaDisplay = "−97.7% vs last month",
            RecentSessions = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<SessionEntry>>(
                ImmutableList<SessionEntry>.Empty)),
            ModelBreakdown = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<ModelUsageBreakdown>>(
                ImmutableList<ModelUsageBreakdown>.Empty)),
        });

    public string PeriodLabel { get; init; } = SampleData.PeriodLabel;

    public string TotalCostDisplay { get; init; } = Fmt.Money(SampleData.TotalCostUsd);
    public string BudgetLimitDisplay { get; init; } = Fmt.Money(SampleData.BudgetLimitUsd);
    public double BudgetUsedPercent { get; init; } = SampleData.BudgetUsedPercent;
    public string BudgetUsedDisplay { get; init; } = $"{SampleData.BudgetUsedPercent:0}%";

    public string TotalTokensDisplay { get; init; } = Fmt.Tokens(SampleData.TotalTokens);
    public string TotalSessionsDisplay { get; init; } = Fmt.Count(SampleData.TotalSessions);
    public string ActiveDaysDisplay { get; init; } = Fmt.Count(SampleData.ActiveDays);
    public string AvgCostDisplay { get; init; } =
        Fmt.Money(SampleData.TotalCostUsd / SampleData.TotalSessions);

    public string ResetWindowLabel { get; init; } = SampleData.ResetWindowLabel;
    public string ResetCountdown { get; init; } = SampleData.ResetCountdown;

    public bool BudgetVsLastMonthUp { get; init; } = SampleData.BudgetVsLastMonthUp;
    public string TrendDeltaDisplay { get; init; } =
        $"+{Fmt.Percent(SampleData.BudgetVsLastMonth)}% vs last month";

    // Inline lambdas that capture nothing — they read SampleData directly — so all instances of this
    // mock share one feed (rule 4).
    public IListFeed<SessionEntry> RecentSessions { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<SessionEntry>>(
            SampleData.Sessions.Take(RecentCount).ToImmutableList()));

    public IListFeed<ModelUsageBreakdown> ModelBreakdown { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult(SampleData.ModelBreakdown.ToImmutableList()));
}

// The MVUX analyzer generates DashboardPageMockDataViewModel for the [ReactiveBindable] mock above.
// Its public constructor always wraps a *default* model and its model-taking constructor is protected,
// so this partial adds a factory that reaches it from inside the class — which is how QuietMonth (and
// the siblings' variants) wrap a customized model.
public partial class DashboardPageMockDataViewModel
{
    internal static DashboardPageMockDataViewModel ForModel(DashboardPageMockData model) => new(model);
}
