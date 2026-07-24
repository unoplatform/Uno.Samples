using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Windows.UI;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="LeadsPage"/>. The KPI strings and "Top Open Leads" list come straight from the
/// shared dataset; the LiveCharts <c>ISeries</c>/<c>Axis</c> objects are assembled once in the
/// constructor (they need SkiaSharp paints, so they can't be pure XAML) from the same dataset, and
/// resolve their colors from the theme resources so they re-theme with light/dark. A pure
/// projection with no reactive members, so it opts out of the bindable generator.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record LeadsModel
{
    public LeadsModel()
    {
        var leads = CrmData.Leads;

        NewLeadsText = leads.NewLeadsText;
        QualificationRateText = leads.QualificationRateText;
        PipelineValueText = leads.PipelineValueText;
        AverageDealSizeText = leads.AverageDealSizeText;
        TopOpenLeads = leads.TopOpenLeads;

        var accent = ResolveColor("DashboardAccentColor", new SKColor(13, 110, 110));
        var blue = ResolveColor("DashboardBlueColor", new SKColor(59, 130, 246));
        var border = ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229));
        var subtleText = ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138));

        // Dedicated categorical palette for the stage pie so adjacent slices stay distinct.
        var stage1 = ResolveColor("Chart1Color", new SKColor(13, 110, 110));
        var stage2 = ResolveColor("Chart2Color", new SKColor(139, 92, 246));
        var stage3 = ResolveColor("Chart3Color", new SKColor(245, 158, 11));
        var stage4 = ResolveColor("Chart4Color", new SKColor(239, 68, 68));
        var stage5 = ResolveColor("Chart5Color", new SKColor(16, 185, 129));
        SKColor[] stageColors = [stage1, stage2, stage3, stage4, stage5];

        LeadTrendSeries =
        [
            new LineSeries<int>
            {
                Name = "Leads",
                Values = leads.MonthlyLeads,
                Fill = null,
                GeometrySize = 10,
                LineSmoothness = 0.8,
                Stroke = new SolidColorPaint(accent, 4),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometryStroke = new SolidColorPaint(accent, 3),
            }
        ];

        LeadsBySourceSeries =
        [
            new ColumnSeries<int>
            {
                Name = "Leads",
                Values = leads.SourceCounts,
                Fill = new SolidColorPaint(blue),
                Stroke = null,
                MaxBarWidth = 28,
                Rx = 4,
                Ry = 4,
            }
        ];

        StageDistributionSeries = leads.StageCounts
            .Select((count, i) => (ISeries)new PieSeries<int>
            {
                Name = leads.StageLabels[i],
                Values = [count],
                Fill = new SolidColorPaint(stageColors[i]),
                MaxRadialColumnWidth = 28,
            })
            .ToArray();

        MonthXAxis =
        [
            new Axis
            {
                Labels = leads.MonthLabels,
                MinStep = 1,
                LabelsRotation = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 },
            }
        ];

        CountYAxis =
        [
            new Axis
            {
                MinLimit = 0,
                MinStep = 25,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 },
            }
        ];

        SourceXAxis =
        [
            new Axis
            {
                Labels = leads.SourceLabels,
                MinStep = 1,
                LabelsRotation = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = null,
            }
        ];

        SourceYAxis =
        [
            new Axis
            {
                MinLimit = 0,
                MinStep = 20,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 },
            }
        ];
    }

    public string NewLeadsText { get; }
    public string QualificationRateText { get; }
    public string PipelineValueText { get; }
    public string AverageDealSizeText { get; }
    public IReadOnlyList<TopLead> TopOpenLeads { get; }

    public ISeries[] LeadTrendSeries { get; }
    public ISeries[] LeadsBySourceSeries { get; }
    public ISeries[] StageDistributionSeries { get; }

    public Axis[] MonthXAxis { get; }
    public Axis[] CountYAxis { get; }
    public Axis[] SourceXAxis { get; }
    public Axis[] SourceYAxis { get; }

    private static SKColor ResolveColor(string resourceKey, SKColor fallback)
    {
        if (Application.Current.Resources.TryGetValue(resourceKey, out var resource)
            && resource is Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        return fallback;
    }
}
