using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace UnoCRM.Presentation.MockData;

// Design-time DataContexts for Hot Design / Studio, one per page.
//
// Each page renders at least one FeedView, and a FeedView can only subscribe to a FEED — a plain list
// would never reach it and the preview would sit on the empty state. So each mock is
// [ReactiveBindable] and its statics return the GENERATED ViewModel, whose constructor creates the
// SourceContext that makes the feeds pump.
//
// Two rules these all follow: statics are expression-bodied (a fresh instance per access, never a
// cached singleton, because a generated ViewModel has a view-scoped lifecycle); and every static input
// is declared ABOVE the statics that construct instances, because static members initialize in textual
// order and an instance initializer reading a not-yet-assigned field gets null with no exception.
//
// None of these is ever seeded from a page constructor — see those constructors for why.

internal static class MockFeeds
{
    public static IListFeed<T> Of<T>(params T[] items) =>
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(items.ToImmutableList()));

    public static IListFeed<T> Empty<T>() =>
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(ImmutableList<T>.Empty));
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record DashboardPageMockData
{
    // Declared first: the statics below construct instances that read these.
    private static readonly DashboardData Seed = CrmData.Dashboard;

    /// <summary>A healthy tenant: KPIs, the funnel and a busy activity feed.</summary>
    public static DashboardPageMockDataViewModel Data => new();

    /// <summary>A quiet account — the activity FeedView on its NoneTemplate.</summary>
    public static DashboardPageMockDataViewModel NoActivity =>
        DashboardPageMockDataViewModel.ForModel(new() { Activities = MockFeeds.Empty<ActivityItem>() });

    // The Overview payload, so the page's 30 Overview.Funnel[i] indexers resolve against a
    // materialized list exactly as they do at runtime.
    public DashboardData Overview { get; init; } = Seed;

    public string TotalLeadsText { get; init; } = Seed.TotalLeadsText;
    public string TotalLeadsDelta { get; init; } = Seed.TotalLeadsDelta;
    public string ActiveDealsText { get; init; } = Seed.ActiveDealsText;
    public string ActiveDealsDelta { get; init; } = Seed.ActiveDealsDelta;
    public string RevenueText { get; init; } = Seed.RevenueText;
    public string RevenueDelta { get; init; } = Seed.RevenueDelta;
    public string ConversionRateText { get; init; } = Seed.ConversionRateText;
    public string ConversionRateDelta { get; init; } = Seed.ConversionRateDelta;

    public IListFeed<ActivityItem> Activities { get; init; } =
        MockFeeds.Of(CrmData.Dashboard.Activities.ToArray());
}

public partial class DashboardPageMockDataViewModel
{
    internal static DashboardPageMockDataViewModel ForModel(DashboardPageMockData model) => new(model);
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record PipelinePageMockData
{
    private static readonly IReadOnlyList<PipelineStage> SeedStages = CrmData.Stages;

    /// <summary>The full board.</summary>
    public static PipelinePageMockDataViewModel Data => new();

    /// <summary>An empty board — the mobile list's NoneTemplate.</summary>
    public static PipelinePageMockDataViewModel EmptyBoard =>
        PipelinePageMockDataViewModel.ForModel(new() { Stages = MockFeeds.Empty<PipelineStage>() });

    public IListFeed<PipelineStage> Stages { get; init; } = MockFeeds.Of(CrmData.Stages.ToArray());

    // The desktop board's five columns, mirroring the Model's scalar accessors so
    // {Binding NewLead.Deals} resolves at design time too.
    public PipelineStage NewLead { get; init; } = SeedStages[0];
    public PipelineStage Qualified { get; init; } = SeedStages[1];
    public PipelineStage Proposal { get; init; } = SeedStages[2];
    public PipelineStage Negotiation { get; init; } = SeedStages[3];
    public PipelineStage ClosedWon { get; init; } = SeedStages[4];
}

public partial class PipelinePageMockDataViewModel
{
    internal static PipelinePageMockDataViewModel ForModel(PipelinePageMockData model) => new(model);
}

// The Leads mock is the one with no precedent in the other samples: it must supply the LiveCharts
// series and axes as PLAIN arrays, because that is what the page binds and what a chart needs at
// first measure (see LeadsModel's remarks). LeadsChartFactory exists so this mock builds the same
// charts as the runtime Model instead of duplicating them — and because the factory hands out a fresh
// instance per call, the mock's arrays are its own and never shared with the Model's.
[Uno.Extensions.Reactive.ReactiveBindable]
public partial record LeadsPageMockData
{
    private static readonly LeadsAnalytics Seed = CrmData.Leads;

    /// <summary>The analytics as shipped.</summary>
    public static LeadsPageMockDataViewModel Data => new();

    /// <summary>No open leads — that list's NoneTemplate, charts unaffected.</summary>
    public static LeadsPageMockDataViewModel NoOpenLeads =>
        LeadsPageMockDataViewModel.ForModel(new() { TopOpenLeads = MockFeeds.Empty<TopLead>() });

    public string NewLeadsText { get; init; } = Seed.NewLeadsText;
    public string QualificationRateText { get; init; } = Seed.QualificationRateText;
    public string PipelineValueText { get; init; } = Seed.PipelineValueText;
    public string AverageDealSizeText { get; init; } = Seed.AverageDealSizeText;

    public IListFeed<TopLead> TopOpenLeads { get; init; } =
        MockFeeds.Of(CrmData.Leads.TopOpenLeads.ToArray());

    public ISeries[] LeadTrendSeries { get; init; } = LeadsChartFactory.LeadTrendSeries(Seed);
    public ISeries[] LeadsBySourceSeries { get; init; } = LeadsChartFactory.LeadsBySourceSeries(Seed);
    public ISeries[] StageDistributionSeries { get; init; } = LeadsChartFactory.StageDistributionSeries(Seed);
    public Axis[] MonthXAxis { get; init; } = LeadsChartFactory.MonthXAxis(Seed);
    public Axis[] CountYAxis { get; init; } = LeadsChartFactory.CountYAxis();
    public Axis[] SourceXAxis { get; init; } = LeadsChartFactory.SourceXAxis(Seed);
    public Axis[] SourceYAxis { get; init; } = LeadsChartFactory.SourceYAxis();

    public SolidColorPaint LegendTextPaint { get; init; } = LeadsChartFactory.LegendTextPaint();
    public SolidColorPaint TooltipTextPaint { get; init; } = LeadsChartFactory.TooltipTextPaint();
    public SolidColorPaint TooltipBackgroundPaint { get; init; } = LeadsChartFactory.TooltipBackgroundPaint();
}

public partial class LeadsPageMockDataViewModel
{
    internal static LeadsPageMockDataViewModel ForModel(LeadsPageMockData model) => new(model);
}
