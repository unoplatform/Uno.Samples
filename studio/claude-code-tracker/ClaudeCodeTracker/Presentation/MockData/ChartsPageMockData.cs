using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ClaudeCodeTracker.Presentation.MockData;

public partial record ChartsData(
    string PeriodLabel,
    ISeries[] DailyCostSeries,
    Axis[] DailyCostXAxes,
    Axis[] DailyCostYAxes,
    ISeries[] TokenTypeSeries,
    ISeries[] ModelShareSeries,
    ISeries[] WeeklySessionsSeries,
    Axis[] WeeklySessionsXAxes,
    Axis[] WeeklySessionsYAxes,
    SolidColorPaint LegendTextPaint);

public static class ChartsPageMockData
{
    private static readonly SolidColorPaint AxisLabelPaint = new(new SKColor(0x33, 0x33, 0x33));
    private static readonly SolidColorPaint LegendTextPaint = new(new SKColor(0x33, 0x33, 0x33));

    public static ChartsData Data { get; } = new(
        PeriodLabel: "Last 14 Days",
        DailyCostSeries: new ISeries[]
        {
            new LineSeries<double>
            {
                Name = "Daily Cost ($)",
                Values = new double[] { 1.82, 3.44, 2.91, 0.0, 4.21, 5.87, 0.0, 3.14, 6.92, 9.44, 12.08, 7.31, 0.0, 2.21 },
                Fill = new SolidColorPaint(new SKColor(0xF0, 0x7A, 0x3A, 0x30)),
                Stroke = new SolidColorPaint(new SKColor(0xF0, 0x7A, 0x3A), 2),
                GeometrySize = 6,
                GeometryStroke = new SolidColorPaint(new SKColor(0xF0, 0x7A, 0x3A), 2),
                GeometryFill = new SolidColorPaint(SKColors.White),
            }
        },
        DailyCostXAxes: new Axis[]
        {
            new Axis
            {
                Labels = new[] { "Jun 6","Jun 7","Jun 8","Jun 9","Jun 10","Jun 11","Jun 12","Jun 13","Jun 14","Jun 15","Jun 16","Jun 17","Jun 18","Jun 19" },
                LabelsRotation = -30,
                TextSize = 10,
                LabelsPaint = AxisLabelPaint,
            }
        },
        DailyCostYAxes: new Axis[]
        {
            new Axis { MinLimit = 0, TextSize = 10, LabelsPaint = AxisLabelPaint }
        },
        TokenTypeSeries: new ISeries[]
        {
            new PieSeries<double> { Name = "Input",       Values = new double[] { 60.3 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
            new PieSeries<double> { Name = "Output",      Values = new double[] { 14.0 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
            new PieSeries<double> { Name = "Cache Read",  Values = new double[] { 20.4 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
            new PieSeries<double> { Name = "Cache Write", Values = new double[] { 5.3 },  InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
        },
        ModelShareSeries: new ISeries[]
        {
            new PieSeries<double> { Name = "Opus",   Values = new double[] { 55.7 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
            new PieSeries<double> { Name = "Sonnet", Values = new double[] { 33.9 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
            new PieSeries<double> { Name = "Haiku",  Values = new double[] { 10.4 }, InnerRadius = 70, MaxRadialColumnWidth = 40, DataLabelsPaint = null },
        },
        WeeklySessionsSeries: new ISeries[]
        {
            new ColumnSeries<int>
            {
                Name = "Sessions",
                Values = new int[] { 8, 11, 0, 6, 13, 18, 21 },
                Fill = new SolidColorPaint(new SKColor(0x4E, 0xCF, 0xE0, 0xCC)),
                MaxBarWidth = 28,
            }
        },
        WeeklySessionsXAxes: new Axis[]
        {
            new Axis
            {
                Labels = new[] { "Jun 13", "Jun 14", "Jun 15", "Jun 16", "Jun 17", "Jun 18", "Jun 19" },
                TextSize = 10,
                LabelsPaint = AxisLabelPaint,
            }
        },
        WeeklySessionsYAxes: new Axis[]
        {
            new Axis { MinLimit = 0, TextSize = 10, LabelsPaint = AxisLabelPaint }
        },
        LegendTextPaint: LegendTextPaint
    );
}
