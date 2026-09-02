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

    public static IFeed<IImmutableList<T>> Scalar<T>(IReadOnlyList<T> items) =>
        Feed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(items.ToImmutableList()));
}

[Uno.Extensions.Reactive.ReactiveBindable]
public partial record DashboardPageMockData
{
    // Declared first: the statics below construct instances that read these.
    private static readonly DashboardData Seed = CrmData.Dashboard;

    // A tenant a fortnight into a trial: two-figure volumes, a pipeline that thins out to nothing past
    // the middle, and revenue and conversion both slipping. Every number here differs from the busy
    // tenant's, because two previews that share their figures only prove the page renders twice.
    private static readonly DashboardData QuietSeed = new()
    {
        TotalLeadsText = "128",
        TotalLeadsDelta = "+0.4%",
        TotalLeadsTrend = KpiTrend.Up,
        ActiveDealsText = "3",
        ActiveDealsDelta = "0.0%",
        ActiveDealsTrend = KpiTrend.Flat,
        RevenueText = "$18K",
        RevenueDelta = "-6.2%",
        RevenueTrend = KpiTrend.Down,
        ConversionRateText = "2.4%",
        ConversionRateDelta = "-1.8%",
        ConversionRateTrend = KpiTrend.Down,
        Funnel =
        [
            new() { Count = 3, FillFraction = 1d },
            new() { Count = 2, FillFraction = 0.66d },
            new() { Count = 1, FillFraction = 0.33d },
            new() { Count = 0, FillFraction = 0d },
            new() { Count = 0, FillFraction = 0d },
        ],
    };

    /// <summary>A busy tenant: four-figure volumes, every KPI rising, a full funnel and a live feed.</summary>
    public static DashboardPageMockDataViewModel Data => new();

    /// <summary>
    /// A quiet account: small volumes, two KPIs falling and one flat, a funnel that empties out past
    /// Proposal, and the activity FeedView on its empty state. Laid beside the busy tenant it exercises
    /// all three delta directions and both ends of the funnel's range.
    /// </summary>
    public static DashboardPageMockDataViewModel QuietAccount =>
        DashboardPageMockDataViewModel.ForModel(new()
        {
            Overview = QuietSeed,
            TotalLeadsText = QuietSeed.TotalLeadsText,
            ActiveDealsText = QuietSeed.ActiveDealsText,
            RevenueText = QuietSeed.RevenueText,
            ConversionRateText = QuietSeed.ConversionRateText,
            Activities = MockFeeds.Empty<ActivityItem>(),
        });

    // The Overview payload: the page's funnel indexers and its four delta read-outs are all paths off
    // it, so it must be a materialized DashboardData exactly as at runtime.
    public DashboardData Overview { get; init; } = Seed;

    public string TotalLeadsText { get; init; } = Seed.TotalLeadsText;
    public string ActiveDealsText { get; init; } = Seed.ActiveDealsText;
    public string RevenueText { get; init; } = Seed.RevenueText;
    public string ConversionRateText { get; init; } = Seed.ConversionRateText;

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
        PipelinePageMockDataViewModel.ForModel(new()
        {
            Board = MockFeeds.Scalar<PipelineStage>([]),
            Stages = MockFeeds.Empty<PipelineStage>(),
        });

    // Both arrangements' sources, mirroring the Model: the desktop board indexes the scalar feed,
    // the mobile list takes the list form.
    public IFeed<IImmutableList<PipelineStage>> Board { get; init; } = MockFeeds.Scalar(SeedStages);

    public IListFeed<PipelineStage> Stages { get; init; } = MockFeeds.Of(SeedStages.ToArray());

    // The filter bar. Settable strings rather than states: a preview only has to RENDER a selection,
    // and the vocabularies must be materialized or the ComboBoxes would drop it.
    public string SourceFilter { get; set; } = PipelineModel.AllSources;
    public string PeriodFilter { get; set; } = PipelineModel.ThisQuarter;
    public string RepFilter { get; set; } = PipelineModel.AllReps;

    public IReadOnlyList<string> Sources { get; } =
        new[] { PipelineModel.AllSources }
            .Concat(CrmData.Deals.Select(d => d.Source).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();

    public IReadOnlyList<string> Periods { get; } =
        [PipelineModel.ThisWeek, PipelineModel.ThisMonth, PipelineModel.ThisQuarter];

    public IReadOnlyList<string> Reps { get; } =
        new[] { PipelineModel.AllReps }
            .Concat(CrmData.Deals.Select(d => d.Owner).Distinct(StringComparer.OrdinalIgnoreCase))
            .ToArray();
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
