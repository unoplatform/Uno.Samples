using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Windows.UI;

namespace UnoCRM.Presentation;

/// <summary>
/// Builds the Leads page's LiveCharts series, axes and paints from a <see cref="LeadsAnalytics"/>
/// payload. Extracted from <see cref="LeadsModel"/> so the design-time mock can build the same charts
/// instead of duplicating ~120 lines that would drift.
///
/// Every method returns a FRESH instance per call, and caching stays with the caller. That is
/// deliberate: a LiveCharts series or axis carries per-chart state and must be attached to exactly
/// one chart control, so each consumer — the Model at runtime, the mock at design time — owns its own
/// set. Never hand one of these arrays to two chart controls.
/// </summary>
internal static class LeadsChartFactory
{
    public static ISeries[] LeadTrendSeries(LeadsAnalytics data)
    {
        var accent = ResolveColor("DashboardAccentColor", new SKColor(13, 110, 110));
        return
        [
            new LineSeries<int>
            {
                Name = "Leads",
                Values = data.MonthlyLeads,
                Fill = null,
                GeometrySize = 10,
                LineSmoothness = 0.8,
                Stroke = new SolidColorPaint(accent, 4),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometryStroke = new SolidColorPaint(accent, 3),
            }
        ];
    }

    public static ISeries[] LeadsBySourceSeries(LeadsAnalytics data)
    {
        var blue = ResolveColor("DashboardBlueColor", new SKColor(59, 130, 246));
        return
        [
            new ColumnSeries<int>
            {
                Name = "Leads",
                Values = data.SourceCounts,
                Fill = new SolidColorPaint(blue),
                Stroke = null,
                MaxBarWidth = 28,
                Rx = 4,
                Ry = 4,
            }
        ];
    }

    public static ISeries[] StageDistributionSeries(LeadsAnalytics data)
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

        return data.StageCounts
            .Select((count, i) => (ISeries)new PieSeries<int>
            {
                Name = data.StageLabels[i],
                Values = [count],
                Fill = new SolidColorPaint(stageColors[i]),
                // No MaxRadialColumnWidth: that caps the slice radius (fine for a thin donut ring, but
                // it shrank this full pie to a tiny disc) — let it fill the chart area.
            })
            .ToArray();
    }

    public static Axis[] MonthXAxis(LeadsAnalytics data) =>
    [
        new Axis
        {
            Labels = data.MonthLabels,
            MinStep = 1,
            LabelsRotation = 0,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = new SolidColorPaint(ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229))) { StrokeThickness = 1 },
        }
    ];

    public static Axis[] CountYAxis() =>
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

    public static Axis[] SourceXAxis(LeadsAnalytics data) =>
    [
        new Axis
        {
            Labels = data.SourceLabels,
            MinStep = 1,
            LabelsRotation = 0,
            TextSize = 12,
            LabelsPaint = new SolidColorPaint(ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138))),
            SeparatorsPaint = null,
        }
    ];

    public static Axis[] SourceYAxis() =>
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

    public static SKColor ResolveColor(string resourceKey, SKColor fallback)
    {
        if (Application.Current?.Resources.TryGetValue(resourceKey, out var resource) is true
            && resource is Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        return fallback;
    }

    public static SolidColorPaint LegendTextPaint() =>
        new(ResolveColor("DashboardMutedTextColor", new SKColor(110, 110, 110)));

    public static SolidColorPaint TooltipTextPaint() =>
        new(ResolveColor("DashboardPrimaryTextColor", new SKColor(26, 26, 26)));

    public static SolidColorPaint TooltipBackgroundPaint() =>
        new(ResolveColor("DashboardControlColor", new SKColor(240, 240, 240)));
}
