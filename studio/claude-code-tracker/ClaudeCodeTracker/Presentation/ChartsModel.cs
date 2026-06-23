using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ClaudeCodeTracker.Presentation;

/// <summary>
/// Builds the LiveCharts series from the shared <see cref="SampleData"/> rollup. Axis/legend
/// and series paints are resolved from the active theme palette (see <see cref="ResolveColor"/>)
/// so the charts stay legible in both light and dark — never the hardcoded gray that vanished
/// in dark mode (lesson 16). Paints are built here at navigation time (after resources load),
/// avoiding the static-initialization-order trap of lesson 19.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ChartsModel
{
    public ChartsModel()
    {
        var labelColor = ResolveColor("OnSurfaceVariantColor", new SKColor(0x52, 0x45, 0x3D));
        var axisLabel = new SolidColorPaint(labelColor);
        LegendTextPaint = new SolidColorPaint(labelColor);

        var primary = ResolveColor("PrimaryColor", new SKColor(0x7C, 0x3A, 0xED));
        var secondary = ResolveColor("SecondaryColor", new SKColor(0x1A, 0x7A, 0x8A));
        var tertiary = ResolveColor("TertiaryColor", new SKColor(0x6B, 0x3F, 0xA0));
        var outline = ResolveColor("OutlineColor", new SKColor(0x85, 0x70, 0x68));

        var days = SampleData.DailyUsage;

        DailyCostSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Name = "Daily Cost",
                Values = days.Select(d => (double)d.CostUsd).ToArray(),
                Fill = new SolidColorPaint(primary.WithAlpha(0x22)), // light area under the line
                Stroke = new SolidColorPaint(primary, 2),
                GeometrySize = 6,
                GeometryStroke = new SolidColorPaint(primary, 2),
                GeometryFill = new SolidColorPaint(SKColors.White),
            },
        };
        DailyCostXAxes = new[]
        {
            new Axis { Labels = days.Select(d => d.DayLabel).ToArray(), LabelsRotation = -30, TextSize = 10, LabelsPaint = axisLabel },
        };
        DailyCostYAxes = new[] { new Axis { MinLimit = 0, TextSize = 10, LabelsPaint = axisLabel } };

        var week = days.TakeLast(7).ToArray();
        WeeklySessionsSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Sessions",
                Values = week.Select(d => d.SessionCount).ToArray(),
                Fill = new SolidColorPaint(secondary),
                MaxBarWidth = 28,
            },
        };
        WeeklySessionsXAxes = new[]
        {
            new Axis { Labels = week.Select(d => d.DayLabel).ToArray(), TextSize = 10, LabelsPaint = axisLabel },
        };
        WeeklySessionsYAxes = new[] { new Axis { MinLimit = 0, TextSize = 10, LabelsPaint = axisLabel } };

        var tokens = SampleData.Tokens;
        TokenTypeSeries = new ISeries[]
        {
            Slice("Input", tokens.InputPercent, primary),
            Slice("Output", tokens.OutputPercent, secondary),
            Slice("Cache Read", tokens.CacheReadPercent, tertiary),
            Slice("Cache Write", tokens.CacheWritePercent, outline),
        };

        var sharePalette = new[] { primary, secondary, tertiary };
        ModelShareSeries = SampleData.ModelBreakdown
            .Select((m, i) => Slice(m.ShortName, m.SharePercent, sharePalette[i % sharePalette.Length]))
            .ToArray();
    }

    public string PeriodLabel => "Last 14 Days";

    public ISeries[] DailyCostSeries { get; }
    public Axis[] DailyCostXAxes { get; }
    public Axis[] DailyCostYAxes { get; }
    public ISeries[] WeeklySessionsSeries { get; }
    public Axis[] WeeklySessionsXAxes { get; }
    public Axis[] WeeklySessionsYAxes { get; }
    public ISeries[] TokenTypeSeries { get; }
    public ISeries[] ModelShareSeries { get; }
    public SolidColorPaint LegendTextPaint { get; }

    private static PieSeries<double> Slice(string name, double value, SKColor color) => new()
    {
        Name = name,
        Values = new[] { value },
        Fill = new SolidColorPaint(color),
        MaxRadialColumnWidth = 40, // pin band thickness; let the hole scale (lesson 15)
        DataLabelsPaint = null,
    };

    private static SKColor ResolveColor(string key, SKColor fallback) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is Windows.UI.Color color
            ? new SKColor(color.R, color.G, color.B, color.A)
            : fallback;
}
