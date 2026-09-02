using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace UnoCRM.Presentation.MockData;

// Design-time DataContexts for Hot Design / Studio, one per page.
//
// Each page renders at least one FeedView, and a FeedView can only subscribe to a FEED — a plain
// list would never reach it. So every list surface below is a real MVUX feed, exposed on a PLAIN
// record.
//
// Plain — deliberately NOT [ReactiveBindable] — and that distinction is the whole point of this
// file. Marking a mock [ReactiveBindable] generates a ViewModel for it, and the generator replaces
// every feed-shaped member with a bindable-collection proxy: an observable collection view that a
// background pump fills by posting to the UI dispatcher. That proxy is right for a Model, because it
// is what lets a live list re-emit into a bound control. It is wrong for a design-time mock: the
// proxy's contents arrive one dispatcher hop after the feed's message does, so on a design surface a
// POPULATED list can hand the template an empty collection view and render as a zero-height panel —
// while the same mock's EMPTY variant renders correctly, because a list feed with no items reports
// "none" and the empty path never touches the proxy at all. That asymmetry is exactly what makes it
// easy to miss.
//
// FeedView.Source is typed `object` and takes any MVUX feed directly, so handing it the RAW feed
// skips the proxy: the feed's own value — the immutable list itself — is what the ValueTemplate's
// ItemsSource binds to, with no collection view and no dispatcher hop in between. Nothing about the
// runtime path changes; at runtime the DataContext is the navigation-injected Model ViewModel, whose
// feeds keep their proxies and their live re-emission.
//
// Two more rules these all follow: statics are expression-bodied (a fresh instance per access, so
// two previews of the same page never share one feed's cached subscription); and every static input
// is declared ABOVE the statics that construct instances, because static members initialize in
// textual order and an instance initializer reading a not-yet-assigned field gets null with no
// exception.
//
// ContactsPageMockData deliberately does NOT follow this file, and the difference is worth knowing.
// That page hands its filtered list to the map as well as to its FeedViews, and the map reads the
// value as an IEnumerable and listens to it as an INotifyCollectionChanged so it can re-pin when a
// filter changes. A raw feed is neither of those, so the map would draw an empty basemap. The
// bindable-collection proxy supplies both, so that mock keeps its generated ViewModel and accepts the
// list-rendering caveat above in exchange for pins. Making the map consume the feed directly would let
// it move here too.
//
// None of these is ever seeded from a page constructor — see those constructors for why.

internal static class MockFeeds
{
    // Each factory hoists its payload into a local before the lambda closes over it, which is load
    // bearing: a feed factory caches the feed it builds against the delegate instance it is handed,
    // and a lambda that captures nothing is cached by the compiler in a static field — so a
    // no-capture version would hand every caller in the process the same feed. Capturing a local
    // gives each call its own delegate, and so its own feed.

    /// <summary>A list feed carrying <paramref name="items"/>.</summary>
    public static IListFeed<T> Of<T>(params T[] items)
    {
        var value = items.ToImmutableList();

        return ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(value));
    }

    /// <summary>
    /// A list feed with nothing in it. A list feed reports <c>none</c> when it has no items, and
    /// <c>none</c> is what selects a FeedView's empty-state template.
    /// </summary>
    public static IListFeed<T> Empty<T>() => Of<T>();

    /// <summary>
    /// The scalar form, for a surface that indexes the loaded list (<c>Data[0]</c>) instead of
    /// binding it as an ItemsSource. A scalar feed is never <c>none</c>, so it survives an empty
    /// payload and the indexed surface keeps rendering.
    /// </summary>
    public static IFeed<IImmutableList<T>> Scalar<T>(IReadOnlyList<T> items)
    {
        var value = items.ToImmutableList();

        return Feed.Async(_ => ValueTask.FromResult<IImmutableList<T>>(value));
    }
}

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
    public static DashboardPageMockData Data => new();

    /// <summary>
    /// A quiet account: small volumes, two KPIs falling and one flat, a funnel that empties out past
    /// Proposal, and the activity FeedView on its empty state. Laid beside the busy tenant it exercises
    /// all three delta directions and both ends of the funnel's range.
    /// </summary>
    public static DashboardPageMockData QuietAccount => new()
    {
        Overview = QuietSeed,
        TotalLeadsText = QuietSeed.TotalLeadsText,
        ActiveDealsText = QuietSeed.ActiveDealsText,
        RevenueText = QuietSeed.RevenueText,
        ConversionRateText = QuietSeed.ConversionRateText,
        Activities = MockFeeds.Empty<ActivityItem>(),
    };

    // The Overview payload: the page's funnel indexers and its four delta read-outs are all paths off
    // it, so it must be a materialized DashboardData exactly as at runtime.
    public DashboardData Overview { get; init; } = Seed;

    public string TotalLeadsText { get; init; } = Seed.TotalLeadsText;
    public string ActiveDealsText { get; init; } = Seed.ActiveDealsText;
    public string RevenueText { get; init; } = Seed.RevenueText;
    public string ConversionRateText { get; init; } = Seed.ConversionRateText;

    // What both arrangements' activity FeedViews subscribe to: one feed, so the desktop panel and the
    // mobile panel cannot disagree. Init-settable so a variant can supply an empty set.
    public IListFeed<ActivityItem> Activities { get; init; } = MockFeeds.Of(Seed.Activities.ToArray());
}

public partial record PipelinePageMockData
{
    private static readonly IReadOnlyList<PipelineStage> SeedStages = CrmData.Stages;

    /// <summary>The full board.</summary>
    public static PipelinePageMockData Data => new();

    /// <summary>An empty board — the mobile list's NoneTemplate.</summary>
    public static PipelinePageMockData EmptyBoard => new()
    {
        Board = MockFeeds.Scalar<PipelineStage>([]),
        Stages = MockFeeds.Empty<PipelineStage>(),
    };

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

// The Leads mock is the one with no precedent in the other samples: it must supply the LiveCharts
// series and axes as PLAIN arrays, because that is what the page binds and what a chart needs at
// first measure (see LeadsModel's remarks). LeadsChartFactory exists so this mock builds the same
// charts as the runtime Model instead of duplicating them — and because the factory hands out a fresh
// instance per call, the mock's arrays are its own and never shared with the Model's.
public partial record LeadsPageMockData
{
    private static readonly LeadsAnalytics Seed = CrmData.Leads;

    /// <summary>The analytics as shipped.</summary>
    public static LeadsPageMockData Data => new();

    /// <summary>No open leads — that list's NoneTemplate, charts unaffected.</summary>
    public static LeadsPageMockData NoOpenLeads => new() { TopOpenLeads = MockFeeds.Empty<TopLead>() };

    public string NewLeadsText { get; init; } = Seed.NewLeadsText;
    public string QualificationRateText { get; init; } = Seed.QualificationRateText;
    public string PipelineValueText { get; init; } = Seed.PipelineValueText;
    public string AverageDealSizeText { get; init; } = Seed.AverageDealSizeText;

    public IListFeed<TopLead> TopOpenLeads { get; init; } = MockFeeds.Of(Seed.TopOpenLeads.ToArray());

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
