using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ClaudeCodeTracker.Presentation;

/// <summary>
/// Builds the LiveCharts series from the shared <see cref="SampleData"/> rollup. Axis and series
/// colors are resolved from the active theme palette (see <see cref="Resolve"/>) so the charts
/// stay legible and on-brand in both light and dark; paints are built here at navigation time,
/// after resources load, avoiding a static-initialization-order trap.
///
/// Donuts use a hidden built-in legend plus a custom <see cref="LegendEntry"/> legend (rendered in
/// XAML) so both donuts draw at the same fixed size and each item shows its percentage.
/// </summary>
[Uno.Extensions.Reactive.ReactiveBindable(false)]
public partial record ChartsModel
{
    public ChartsModel()
    {
        var labelClr = Resolve("OnSurfaceVariantColor", Rgb(0x51, 0x46, 0x3B));
        var primaryClr = Resolve("PrimaryColor", Rgb(0xB2, 0x3A, 0x0A));
        var secondaryClr = Resolve("SecondaryColor", Rgb(0x0F, 0x76, 0x6E));
        var surfaceClr = Resolve("SurfaceColor", Rgb(0xFF, 0xFF, 0xFF));

        // Categorical chart palette — spread hues (terracotta / teal / gold / violet) so adjacent
        // donut slices and legend rows stay distinguishable; the semantic Primary and Tertiary
        // roles alone are too close in hue to tell apart. Fallbacks mirror the Light ThemeColors
        // values for design-time (before the theme resources load).
        var cat1Clr = Resolve("Chart1Color", Rgb(0xB2, 0x3A, 0x0A));
        var cat2Clr = Resolve("Chart2Color", Rgb(0x0F, 0x76, 0x6E));
        var cat3Clr = Resolve("Chart3Color", Rgb(0xCA, 0x8A, 0x04));
        var cat4Clr = Resolve("Chart4Color", Rgb(0x7E, 0x4E, 0x9E));

        var axisLabel = new SolidColorPaint(Sk(labelClr));
        SKColor primary = Sk(primaryClr), secondary = Sk(secondaryClr);
        SKColor cat1 = Sk(cat1Clr), cat2 = Sk(cat2Clr), cat3 = Sk(cat3Clr), cat4 = Sk(cat4Clr);

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
                GeometryFill = new SolidColorPaint(Sk(surfaceClr)), // hollow marker = card surface, per theme
            },
        };
        DailyCostXAxes = new[]
        {
            new Axis { Labels = days.Select(d => d.DayLabel).ToArray(), LabelsRotation = -30, TextSize = 11, LabelsPaint = axisLabel },
        };
        DailyCostYAxes = new[] { new Axis { MinLimit = 0, TextSize = 11, LabelsPaint = axisLabel } };

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
            new Axis { Labels = week.Select(d => d.DayLabel).ToArray(), TextSize = 11, LabelsPaint = axisLabel },
        };
        WeeklySessionsYAxes = new[] { new Axis { MinLimit = 0, TextSize = 11, LabelsPaint = axisLabel } };

        var tokens = SampleData.Tokens;
        TokenTypeSeries = new ISeries[]
        {
            Slice("Input", tokens.InputPercent, cat1),
            Slice("Output", tokens.OutputPercent, cat2),
            Slice("Cache Read", tokens.CacheReadPercent, cat3),
            Slice("Cache Write", tokens.CacheWritePercent, cat4),
        };
        TokenTypeLegend = new[]
        {
            new LegendEntry("Input", Pct(tokens.InputPercent), cat1Clr),
            new LegendEntry("Output", Pct(tokens.OutputPercent), cat2Clr),
            new LegendEntry("Cache Read", Pct(tokens.CacheReadPercent), cat3Clr),
            new LegendEntry("Cache Write", Pct(tokens.CacheWritePercent), cat4Clr),
        };

        var paletteSk = new[] { cat1, cat2, cat3 };
        var paletteClr = new[] { cat1Clr, cat2Clr, cat3Clr };
        ModelShareSeries = SampleData.ModelBreakdown
            .Select((m, i) => Slice(m.ShortName, m.SharePercent, paletteSk[i % paletteSk.Length]))
            .ToArray();
        ModelShareLegend = SampleData.ModelBreakdown
            .Select((m, i) => new LegendEntry(m.ShortName, Pct(m.SharePercent), paletteClr[i % paletteClr.Length]))
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
    public IReadOnlyList<LegendEntry> TokenTypeLegend { get; }
    public IReadOnlyList<LegendEntry> ModelShareLegend { get; }

    private static PieSeries<double> Slice(string name, double value, SKColor color) => new()
    {
        Name = name,
        Values = new[] { value },
        Fill = new SolidColorPaint(color),
        MaxRadialColumnWidth = 38, // pin band thickness; let the hole scale with the chart
        DataLabelsPaint = null,
    };

    private static string Pct(double value) => $"{Fmt.Percent(value)}%";
    private static SKColor Sk(Windows.UI.Color c) => new(c.R, c.G, c.B, c.A);
    private static Windows.UI.Color Rgb(byte r, byte g, byte b) => Windows.UI.Color.FromArgb(0xFF, r, g, b);

    private static Windows.UI.Color Resolve(string key, Windows.UI.Color fallback) =>
        Application.Current.Resources.TryGetValue(key, out var value) && value is Windows.UI.Color color
            ? color
            : fallback;
}

/// <summary>One row of a chart legend: a swatch colour (data only — XAML wraps it in a brush), a
/// label, and its percentage.</summary>
public partial record LegendEntry(string Label, string ValueDisplay, Windows.UI.Color Swatch);
