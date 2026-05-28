using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Nexus.Services;
using SkiaSharp;
using Uno.Extensions.Reactive;

namespace Nexus.Presentation;

public partial record AnalyticsModel(INexusService Nexus)
{
    // OEE / MTBF / MTTR / FPY headline metrics from the service.
    public IFeed<AnalyticsMetrics> Metrics => Feed.Async(Nexus.GetAnalyticsMetricsAsync);

    // Per-line rows for the comparison matrix.
    public IListFeed<ProductionLine> Lines => ListFeed.Async(Nexus.GetProductionLinesAsync);

    private static readonly SKColor SuccessColor = SKColor.Parse("#4ade80");
    private static readonly SKColor InfoColor = SKColor.Parse("#60a5fa");
    private static readonly SKColor DangerColor = SKColor.Parse("#f87171");
    private static readonly SKColor TextTertiaryColor = SKColor.Parse("#6b7280");

    // Sparkline charts for OEE metric cards
    public ISeries[] OeeSeries { get; } = CreateSparklineSeries(
        [82.1, 83.4, 84.2, 84.8, 85.1, 84.9, 85.2, 85.4],
        SuccessColor);

    public ISeries[] AvailabilitySeries { get; } = CreateSparklineSeries(
        [90.2, 91.0, 91.4, 91.8, 92.0, 91.6, 91.9, 92.1],
        SuccessColor);

    public ISeries[] PerformanceSeries { get; } = CreateSparklineSeries(
        [93.5, 94.2, 94.8, 95.1, 95.4, 95.6, 95.5, 95.8],
        SuccessColor);

    public ISeries[] QualitySeries { get; } = CreateSparklineSeries(
        [97.2, 97.1, 97.0, 96.9, 97.1, 96.9, 96.7, 96.8],
        DangerColor);

    public Axis[] SparklineXAxis { get; } =
    [
        new Axis
        {
            IsVisible = false,
            ShowSeparatorLines = false,
            Padding = new LiveChartsCore.Drawing.Padding(0)
        }
    ];

    public Axis[] SparklineYAxis { get; } =
    [
        new Axis
        {
            IsVisible = false,
            ShowSeparatorLines = false,
            Padding = new LiveChartsCore.Drawing.Padding(0)
        }
    ];

    private static ISeries[] CreateSparklineSeries(double[] values, SKColor color)
    {
        return
        [
            new LineSeries<double>
            {
                Values = values,
                Fill = new SolidColorPaint(color.WithAlpha(30)),
                Stroke = new SolidColorPaint(color, 2),
                GeometryFill = null,
                GeometryStroke = null,
                GeometrySize = 0,
                LineSmoothness = 0.5
            }
        ];
    }

    public ISeries[] ProductionTrendSeries { get; } =
    [
        new LineSeries<double>
        {
            Name = "Output",
            Values = [4250, 4180, 4320, 4150, 4480, 4520, 4380],
            Fill = new SolidColorPaint(SuccessColor.WithAlpha(40)),
            Stroke = new SolidColorPaint(SuccessColor, 2),
            GeometryFill = new SolidColorPaint(SuccessColor),
            GeometryStroke = new SolidColorPaint(SuccessColor, 2),
            GeometrySize = 6,
            LineSmoothness = 0.3
        },
        new LineSeries<double>
        {
            Name = "Target",
            Values = [4200, 4200, 4200, 4200, 4200, 4200, 4200],
            Fill = null,
            Stroke = new SolidColorPaint(InfoColor, 2) { PathEffect = new DashEffect([6, 4]) },
            GeometryFill = null,
            GeometryStroke = null,
            GeometrySize = 0,
            LineSmoothness = 0
        }
    ];

    public Axis[] ProductionTrendXAxis { get; } =
    [
        new Axis
        {
            Labels = ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            LabelsPaint = new SolidColorPaint(TextTertiaryColor),
            SeparatorsPaint = new SolidColorPaint(TextTertiaryColor.WithAlpha(30)),
            TicksPaint = new SolidColorPaint(TextTertiaryColor.WithAlpha(50)),
            TextSize = 11
        }
    ];

    public Axis[] ProductionTrendYAxis { get; } =
    [
        new Axis
        {
            LabelsPaint = new SolidColorPaint(TextTertiaryColor),
            SeparatorsPaint = new SolidColorPaint(TextTertiaryColor.WithAlpha(30)),
            TicksPaint = new SolidColorPaint(TextTertiaryColor.WithAlpha(50)),
            TextSize = 11,
            MinLimit = 3800,
            MaxLimit = 4800,
            Labeler = value => $"{value:N0}"
        }
    ];
}
