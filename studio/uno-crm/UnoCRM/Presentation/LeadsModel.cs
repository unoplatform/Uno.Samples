using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Windows.UI;

namespace UnoCRM.Presentation;

/// <summary>
/// Backs <see cref="LeadsPage"/>. A pure projection of the shared, read-only CRM data — like
/// <see cref="DashboardModel"/> everything is exposed through computed getters with no
/// constructor-assigned state, so the page renders identically whether Navigation injects the model
/// at runtime or the Studio / Hot Design design surface materialises it for a preview (that surface
/// does not run the model's constructor body, so any data assigned there would be missing).
///
/// The LiveCharts <c>ISeries</c>/<c>Axis</c> objects need SkiaSharp paints, so they can't be pure
/// XAML; each is built once on first bind and cached on an instance field. Building lazily in the
/// getter — rather than in a constructor — keeps the charts populated on every path the framework
/// uses to realise this model, while the per-instance cache still lets the colours re-resolve from
/// the theme when a fresh model is created after a light/dark switch.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record LeadsModel
{
    private static LeadsAnalytics Data => CrmData.Leads;

    public string NewLeadsText => Data.NewLeadsText;
    public string QualificationRateText => Data.QualificationRateText;
    public string PipelineValueText => Data.PipelineValueText;
    public string AverageDealSizeText => Data.AverageDealSizeText;
    public IReadOnlyList<TopLead> TopOpenLeads => Data.TopOpenLeads;

    private ISeries[]? _leadTrendSeries;
    private ISeries[]? _leadsBySourceSeries;
    private ISeries[]? _stageDistributionSeries;
    private Axis[]? _monthXAxis;
    private Axis[]? _countYAxis;
    private Axis[]? _sourceXAxis;
    private Axis[]? _sourceYAxis;

    public ISeries[] LeadTrendSeries => _leadTrendSeries ??= BuildLeadTrendSeries();
    public ISeries[] LeadsBySourceSeries => _leadsBySourceSeries ??= BuildLeadsBySourceSeries();
    public ISeries[] StageDistributionSeries => _stageDistributionSeries ??= BuildStageDistributionSeries();
    public Axis[] MonthXAxis => _monthXAxis ??= BuildMonthXAxis();
    public Axis[] CountYAxis => _countYAxis ??= BuildCountYAxis();
    public Axis[] SourceXAxis => _sourceXAxis ??= BuildSourceXAxis();
    public Axis[] SourceYAxis => _sourceYAxis ??= BuildSourceYAxis();

    private static ISeries[] BuildLeadTrendSeries()
    {
        var accent = ResolveColor("DashboardAccentColor", new SKColor(13, 110, 110));
        return
        [
            new LineSeries<int>
            {
                Name = "Leads",
                Values = Data.MonthlyLeads,
                Fill = null,
                GeometrySize = 10,
                LineSmoothness = 0.8,
                Stroke = new SolidColorPaint(accent, 4),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometryStroke = new SolidColorPaint(accent, 3),
            }
        ];
    }

    private static ISeries[] BuildLeadsBySourceSeries()
    {
        var blue = ResolveColor("DashboardBlueColor", new SKColor(59, 130, 246));
        return
        [
            new ColumnSeries<int>
            {
                Name = "Leads",
                Values = Data.SourceCounts,
                Fill = new SolidColorPaint(blue),
                Stroke = null,
                MaxBarWidth = 28,
                Rx = 4,
                Ry = 4,
            }
        ];
    }

    private static ISeries[] BuildStageDistributionSeries()
    {
        // Dedicated categorical palette for the stage pie so adjacent slices stay distinct.
        SKColor[] stageColors =
        [
            ResolveColor("Chart1Color", new SKColor(13, 110, 110)),
            ResolveColor("Chart2Color", new SKColor(139, 92, 246)),
            ResolveColor("Chart3Color", new SKColor(245, 158, 11)),
            ResolveColor("Chart4Color", new SKColor(239, 68, 68)),
            ResolveColor("Chart5Color", new SKColor(16, 185, 129)),
        ];

        return Data.StageCounts
            .Select((count, i) => (ISeries)new PieSeries<int>
            {
                Name = Data.StageLabels[i],
                Values = [count],
                Fill = new SolidColorPaint(stageColors[i]),
                MaxRadialColumnWidth = 28,
            })
            .ToArray();
    }

    private static Axis[] BuildMonthXAxis() =>
    [
        new Axis
        {
            Labels = Data.MonthLabels,
            MinStep = 1,
            LabelsRotation = 0,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = new SolidColorPaint(ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229))) { StrokeThickness = 1 },
        }
    ];

    private static Axis[] BuildCountYAxis() =>
    [
        new Axis
        {
            MinLimit = 0,
            MinStep = 25,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = new SolidColorPaint(ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229))) { StrokeThickness = 1 },
        }
    ];

    private static Axis[] BuildSourceXAxis() =>
    [
        new Axis
        {
            Labels = Data.SourceLabels,
            MinStep = 1,
            LabelsRotation = 0,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = null,
        }
    ];

    private static Axis[] BuildSourceYAxis() =>
    [
        new Axis
        {
            MinLimit = 0,
            MinStep = 20,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = new SolidColorPaint(ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229))) { StrokeThickness = 1 },
        }
    ];

    private static SKColor ResolveColor(string resourceKey, SKColor fallback)
    {
        if (Application.Current?.Resources.TryGetValue(resourceKey, out var resource) == true
            && resource is Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        return fallback;
    }
}
