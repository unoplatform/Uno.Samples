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

        // Uno.Extensions Navigation reassigns DataContext when it activates a view that has
        // no mapped view model. This page is its own context, so re-apply it.
        DataContextChanged += (_, _) =>
        {
            if (DataContext != this)
            {
                DataContext = this;
            }
        };
    }

    public string NewLeadsText { get; private set; } = "0";
    public string QualificationRateText { get; private set; } = "0%";
    public string PipelineValueText { get; private set; } = "$0";
    public string AverageDealSizeText { get; private set; } = "$0";

    // The "Top Open Leads" list, derived from the shared dataset (highest-value open deals).
    public IReadOnlyList<TopLead> TopOpenLeads { get; } = CrmData.Leads.TopOpenLeads;

    public ISeries[] LeadTrendSeries { get; private set; } = [];
    public ISeries[] LeadsBySourceSeries { get; private set; } = [];
    public ISeries[] StageDistributionSeries { get; private set; } = [];

    public Axis[] MonthXAxis { get; private set; } = [];
    public Axis[] CountYAxis { get; private set; } = [];
    public Axis[] SourceXAxis { get; private set; } = [];
    public Axis[] SourceYAxis { get; private set; } = [];

    private void BuildCharts()
    {
        // All chart inputs come from the shared, deterministic dataset — so the Leads page is
        // stable across visits and consistent with the Pipeline and Dashboard pages.
        var leads = CrmData.Leads;
        var monthLabels = leads.MonthLabels;
        var monthlyLeads = leads.MonthlyLeads;
        var leadSourceLabels = leads.SourceLabels;
        var sourceValues = leads.SourceCounts;
        var stageLabels = leads.StageLabels;
        var stageValues = leads.StageCounts;

        NewLeadsText = leads.NewLeadsText;
        QualificationRateText = leads.QualificationRateText;
        PipelineValueText = leads.PipelineValueText;
        AverageDealSizeText = leads.AverageDealSizeText;

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
}
