using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace UnoCRM.Presentation.MockData;

/// <summary>
/// Design-time DataContext for <see cref="LeadsPage"/> in Hot Design / Studio, built to the recipe
/// documented on <see cref="ContactsPageMockData"/>. This is the one mock with no precedent in the
/// other samples: it must supply the LiveCharts series and axes as PLAIN arrays, because that is what
/// the page binds and what a chart needs at first measure (see LeadsModel's remarks).
/// <see cref="LeadsChartFactory"/> exists so this mock builds the same charts as the runtime Model
/// instead of duplicating them — and because the factory hands out a fresh instance per call, the
/// mock's arrays are its own and are never shared with the Model's.
/// </summary>
[ReactiveBindable]
public partial record LeadsPageMockData
{
    // Declared first: the statics below construct instances that read this.
    private static readonly LeadsAnalytics Seed = CrmData.Leads;

    // A tenant that has worked its pipeline out: every deal closed, nothing open left to chase. Told
    // consistently across the whole page rather than only in the list — an empty open-leads list with
    // a rising trend line and a healthy five-way stage mix beside it would contradict itself.
    // Volumes are a tenth of the busy tenant's, the year declines instead of climbing, the pipeline is
    // worth nothing because none of it is open, and every deal sits in Closed Won.
    private static readonly LeadsAnalytics ClearedSeed = new()
    {
        NewLeadsText = "213",
        QualificationRateText = "31%",
        PipelineValueText = "$0",
        AverageDealSizeText = "$12K",
        MonthLabels = Seed.MonthLabels,
        MonthlyLeads = [38, 34, 29, 25, 22, 18, 15, 12, 9, 6, 4, 1],
        SourceLabels = Seed.SourceLabels,
        SourceCounts = [3, 2, 2, 1, 1],
        StageLabels = Seed.StageLabels,
        StageCounts = [0, 0, 0, 0, 9],
        TopOpenLeads = [],
    };

    /// <summary>The analytics as shipped: a busy year, a climbing trend, all five stages in play.</summary>
    public static LeadsPageMockDataViewModel Data => new();

    /// <summary>
    /// A pipeline worked to nothing: the open-leads list on its empty state, and every other surface
    /// agreeing with it — a tenth of the volume, a declining year, no pipeline value, and a stage mix
    /// that is entirely Closed Won.
    /// </summary>
    public static LeadsPageMockDataViewModel PipelineCleared =>
        LeadsPageMockDataViewModel.ForModel(new()
        {
            NewLeadsText = ClearedSeed.NewLeadsText,
            QualificationRateText = ClearedSeed.QualificationRateText,
            PipelineValueText = ClearedSeed.PipelineValueText,
            AverageDealSizeText = ClearedSeed.AverageDealSizeText,
            TopOpenLeads = ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TopLead>>(ImmutableList<TopLead>.Empty)),
            LeadTrendSeries = LeadsChartFactory.LeadTrendSeries(ClearedSeed),
            LeadsBySourceSeries = LeadsChartFactory.LeadsBySourceSeries(ClearedSeed),
            StageDistributionSeries = LeadsChartFactory.StageDistributionSeries(ClearedSeed),
            MonthXAxis = LeadsChartFactory.MonthXAxis(ClearedSeed),
            SourceXAxis = LeadsChartFactory.SourceXAxis(ClearedSeed),
        });

    public string NewLeadsText { get; init; } = Seed.NewLeadsText;
    public string QualificationRateText { get; init; } = Seed.QualificationRateText;
    public string PipelineValueText { get; init; } = Seed.PipelineValueText;
    public string AverageDealSizeText { get; init; } = Seed.AverageDealSizeText;

    // An inline lambda that captures nothing, so all instances share one feed — rule 4 of the recipe.
    public IListFeed<TopLead> TopOpenLeads { get; init; } =
        ListFeed.Async(_ => ValueTask.FromResult<IImmutableList<TopLead>>(CrmData.Leads.TopOpenLeads.ToImmutableList()));

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

// Reaches the generated ViewModel's protected model-taking constructor, so PipelineCleared can wrap a
// customized model. See ContactsPageMockDataViewModel for the full explanation.
public partial class LeadsPageMockDataViewModel
{
    internal static LeadsPageMockDataViewModel ForModel(LeadsPageMockData model) => new(model);
}
