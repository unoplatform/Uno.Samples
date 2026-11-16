using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using UnoCRM.Presentation.Services;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="LeadsPage"/>. The analytics are ONE cached request to <see cref="ICrmService"/>;
/// the KPI texts are scalar projections of it and the top-open-leads list is a list feed the page
/// renders through a FeedView.
///
/// The LiveCharts objects need SkiaSharp paints, so they can't be pure XAML; each is built by
/// <see cref="LeadsChartFactory"/> from the loaded analytics.
///
/// IMPORTANT — the charts are the one surface here that is deliberately NOT reactive, and this was
/// established by trying the alternative and watching it fail. Typing the chart properties as
/// <c>IFeed&lt;ISeries[]&gt;</c> / <c>IFeed&lt;Axis[]&gt;</c> and binding them exactly as before
/// compiles cleanly and renders BLANK on device: the chart measures before the feed emits and never
/// picks the series up, so "Monthly Lead Flow" draws an empty 0-10 grid and the pie draws nothing.
/// (Putting a chart inside a FeedView's ValueTemplate is worse — re-inflation hands a fresh chart
/// control an already-measured series, which is the wedge lesson 72 documents.)
///
/// So the series, axes and paints stay plain cached arrays built from a synchronously-available
/// snapshot: a LiveCharts chart needs its series fully built at first measure. Each property is
/// cached on an instance field, so the colours re-resolve from the theme whenever a fresh model is
/// created after a light/dark switch, and each is bound to exactly one chart control — which is why
/// <see cref="LeadsPage"/> keeps a single responsive tree rather than duplicated desktop/mobile copies.
///
/// Everything that CAN be reactive is: the KPI texts and the top-open-leads list come from the
/// service. The chart numbers are the same dataset the service serves, read directly because a chart
/// cannot wait for it.
/// </summary>
public partial record LeadsModel(ICrmService Crm)
{
    // Cached: one request, shared by every projection below, so the service is asked once.
    private IFeed<LeadsAnalytics>? _analytics;

    /// <summary>
    /// The analytics payload. A SCALAR feed, so it is never None even when a collection inside it is
    /// empty — which is what makes the plain <c>Select</c> projections below safe (lesson 94 bites the
    /// list-feed form, not this one).
    /// </summary>
    private IFeed<LeadsAnalytics> Analytics => _analytics ??= Feed.Async(Crm.GetLeadsAnalyticsAsync);

    public IFeed<string> NewLeadsText => Analytics.Select(d => d.NewLeadsText);
    public IFeed<string> QualificationRateText => Analytics.Select(d => d.QualificationRateText);
    public IFeed<string> PipelineValueText => Analytics.Select(d => d.PipelineValueText);
    public IFeed<string> AverageDealSizeText => Analytics.Select(d => d.AverageDealSizeText);

    // Its own request, rendered by a FeedView — an account with no open leads is a real state.
    private IListFeed<TopLead>? _topOpenLeads;
    public IListFeed<TopLead> TopOpenLeads =>
        _topOpenLeads ??= ListFeed.Async(Crm.GetTopOpenLeadsAsync);

    // Plain cached arrays, NOT feeds — see the class remarks. Built once on first bind from the
    // snapshot, so one instance per property per model instance, each attached to one chart control.
    private static LeadsAnalytics ChartData => CrmData.Leads;

    private ISeries[]? _leadTrendSeries;
    private ISeries[]? _leadsBySourceSeries;
    private ISeries[]? _stageDistributionSeries;
    private Axis[]? _monthXAxis;
    private Axis[]? _countYAxis;
    private Axis[]? _sourceXAxis;
    private Axis[]? _sourceYAxis;
    private SolidColorPaint? _legendTextPaint;
    private SolidColorPaint? _tooltipTextPaint;
    private SolidColorPaint? _tooltipBackgroundPaint;

    public ISeries[] LeadTrendSeries => _leadTrendSeries ??= LeadsChartFactory.LeadTrendSeries(ChartData);
    public ISeries[] LeadsBySourceSeries => _leadsBySourceSeries ??= LeadsChartFactory.LeadsBySourceSeries(ChartData);
    public ISeries[] StageDistributionSeries => _stageDistributionSeries ??= LeadsChartFactory.StageDistributionSeries(ChartData);
    public Axis[] MonthXAxis => _monthXAxis ??= LeadsChartFactory.MonthXAxis(ChartData);
    public Axis[] CountYAxis => _countYAxis ??= LeadsChartFactory.CountYAxis();
    public Axis[] SourceXAxis => _sourceXAxis ??= LeadsChartFactory.SourceXAxis(ChartData);
    public Axis[] SourceYAxis => _sourceYAxis ??= LeadsChartFactory.SourceYAxis();

    // LiveCharts' default legend/tooltip text paints are not theme-aware, so the pie legend and the
    // hover tooltips resolve the dashboard palette the way the axes do.
    public SolidColorPaint LegendTextPaint => _legendTextPaint ??= LeadsChartFactory.LegendTextPaint();
    public SolidColorPaint TooltipTextPaint => _tooltipTextPaint ??= LeadsChartFactory.TooltipTextPaint();
    public SolidColorPaint TooltipBackgroundPaint => _tooltipBackgroundPaint ??= LeadsChartFactory.TooltipBackgroundPaint();
}
