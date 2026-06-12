using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Windows.UI;

namespace UnoCRM;

public sealed partial class LeadsPage : Page
{
    public LeadsPage()
    {
        InitializeComponent();
        BuildCharts();
        DataContext = this;
    }

    public string NewLeadsText { get; private set; } = "0";
    public string QualificationRateText { get; private set; } = "0%";
    public string PipelineValueText { get; private set; } = "$0";
    public string AverageDealSizeText { get; private set; } = "$0";

    public ISeries[] LeadTrendSeries { get; private set; } = [];
    public ISeries[] LeadsBySourceSeries { get; private set; } = [];
    public ISeries[] StageDistributionSeries { get; private set; } = [];

    public Axis[] MonthXAxis { get; private set; } = [];
    public Axis[] CountYAxis { get; private set; } = [];
    public Axis[] SourceXAxis { get; private set; } = [];
    public Axis[] SourceYAxis { get; private set; } = [];

    private void BuildCharts()
    {
        var random = new Random();
        var monthLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        var monthlyLeads = Enumerable
            .Range(0, monthLabels.Length)
            .Select(_ => random.Next(90, 240))
            .ToArray();

        var leadSourceLabels = new[] { "Web", "Email", "Referral", "Ads", "Event" };
        var sourceValues = Enumerable
            .Range(0, leadSourceLabels.Length)
            .Select(_ => random.Next(40, 160))
            .ToArray();

        var stageLabels = new[] { "Prospecting", "Qualified", "Proposal", "Negotiation", "Won" };
        var stageValues = Enumerable
            .Range(0, stageLabels.Length)
            .Select(_ => random.Next(18, 120))
            .ToArray();

        var totalLeads = monthlyLeads.Sum();
        var qualifiedCount = stageValues[1] + stageValues[2] + stageValues[3] + stageValues[4];
        var qualificationRate = totalLeads == 0 ? 0 : (double)qualifiedCount / totalLeads;
        var avgDealSize = random.Next(18000, 54000);
        var pipelineValue = (stageValues[2] + stageValues[3] + stageValues[4]) * avgDealSize;

        NewLeadsText = totalLeads.ToString("N0");
        QualificationRateText = $"{qualificationRate:P0}";
        PipelineValueText = $"${pipelineValue / 1000d:N0}K";
        AverageDealSizeText = $"${avgDealSize / 1000d:N0}K";

        var accent = ResolveColor("DashboardAccentColor", new SKColor(13, 110, 110));
        var blue = ResolveColor("DashboardBlueColor", new SKColor(59, 130, 246));
        var purple = ResolveColor("DashboardPurpleColor", new SKColor(139, 92, 246));
        var amber = ResolveColor("DashboardAmberColor", new SKColor(245, 158, 11));
        var red = ResolveColor("DashboardRedColor", new SKColor(239, 68, 68));
        var green = ResolveColor("DashboardGreenColor", new SKColor(16, 185, 129));
        var border = ResolveColor("DashboardBorderColor", new SKColor(229, 229, 229));
        var subtleText = ResolveColor("DashboardSubtleTextColor", new SKColor(138, 138, 138));

        LeadTrendSeries =
        [
            new LineSeries<int>
            {
                Name = "Leads",
                Values = monthlyLeads,
                Fill = null,
                GeometrySize = 10,
                LineSmoothness = 0.8,
                Stroke = new SolidColorPaint(accent, 4),
                GeometryFill = new SolidColorPaint(SKColors.White),
                GeometryStroke = new SolidColorPaint(accent, 3)
            }
        ];

        LeadsBySourceSeries =
        [
            new ColumnSeries<int>
            {
                Name = "Leads",
                Values = sourceValues,
                Fill = new SolidColorPaint(blue),
                Stroke = null,
                MaxBarWidth = 28,
                Rx = 4,
                Ry = 4
            }
        ];

        StageDistributionSeries =
        [
            new PieSeries<int> { Name = stageLabels[0], Values = [stageValues[0]], Fill = new SolidColorPaint(blue), MaxRadialColumnWidth = 28 },
            new PieSeries<int> { Name = stageLabels[1], Values = [stageValues[1]], Fill = new SolidColorPaint(purple), MaxRadialColumnWidth = 28 },
            new PieSeries<int> { Name = stageLabels[2], Values = [stageValues[2]], Fill = new SolidColorPaint(amber), MaxRadialColumnWidth = 28 },
            new PieSeries<int> { Name = stageLabels[3], Values = [stageValues[3]], Fill = new SolidColorPaint(red), MaxRadialColumnWidth = 28 },
            new PieSeries<int> { Name = stageLabels[4], Values = [stageValues[4]], Fill = new SolidColorPaint(green), MaxRadialColumnWidth = 28 }
        ];

        MonthXAxis =
        [
            new Axis
            {
                Labels = monthLabels,
                MinStep = 1,
                LabelsRotation = 0,
                TextSize = 12,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 }
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
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 }
            }
        ];

        SourceXAxis =
        [
            new Axis
            {
                Labels = leadSourceLabels,
                MinStep = 1,
                TextSize = 12,
                LabelsRotation = 0,
                LabelsPaint = new SolidColorPaint(subtleText),
                SeparatorsPaint = null
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
                SeparatorsPaint = new SolidColorPaint(border) { StrokeThickness = 1 }
            }
        ];
    }

    private static SKColor ResolveColor(string resourceKey, SKColor fallback)
    {
        if (Application.Current.Resources.TryGetValue(resourceKey, out var resource)
            && resource is Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        return fallback;
    }

    private void NavigateToDashboard_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainPage));
    }

    private void NavigateToPipeline_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PipelinePage));
    }

    private void NavigateToContacts_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(ContactsPage));
    }
}
