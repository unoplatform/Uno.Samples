using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml.Media;
using Nexus.Services;
using Uno.Extensions.Reactive;

namespace Nexus.Presentation;

/// <summary>
/// MVUX model for the Overview dashboard. Async data is exposed as feeds sourced from
/// <see cref="INexusService"/>; the generated <c>OverviewViewModel</c> bridges them to XAML.
/// </summary>
public partial record OverviewModel(INexusService Nexus)
{
    // Single read-only record -> IFeed; surfaced through a FeedView (loading/error/value).
    public IFeed<DashboardMetrics> Metrics => Feed.Async(Nexus.GetDashboardMetricsAsync);

    // Read-only collections -> IListFeed; surfaced through FeedView + ItemsRepeater.
    public IListFeed<ProductionLine> ProductionLines => ListFeed.Async(Nexus.GetProductionLinesAsync);

    public IListFeed<Alert> RecentAlerts => ListFeed.Async(ct => Nexus.GetRecentAlertsAsync(ct));

    // --- KPI card sparklines ---
    // Rendered as native Polyline (line) + Polygon (area) inside a Viewbox. This draws reliably at
    // any card width; LiveCharts' CartesianChart only rendered the sparkline at large sizes.
    // Points are normalized to a 200 x 44 box with y inverted, bound to the shapes' Points.
    public PointCollection ThroughputLine { get; } = SparkLine([780, 810, 795, 820, 835, 850, 830, 847]);
    public PointCollection ThroughputArea { get; } = SparkArea([780, 810, 795, 820, 835, 850, 830, 847]);
    public PointCollection EfficiencyLine { get; } = SparkLine([92.1, 93.5, 93.8, 94.2, 94.0, 94.5, 94.3, 94.7]);
    public PointCollection EfficiencyArea { get; } = SparkArea([92.1, 93.5, 93.8, 94.2, 94.0, 94.5, 94.3, 94.7]);
    public PointCollection UptimeLine { get; } = SparkLine([99.8, 99.5, 99.6, 99.4, 99.3, 99.5, 99.1, 99.2]);
    public PointCollection UptimeArea { get; } = SparkArea([99.8, 99.5, 99.6, 99.4, 99.3, 99.5, 99.1, 99.2]);
    public PointCollection EnergyLine { get; } = SparkLine([1.35, 1.32, 1.30, 1.28, 1.26, 1.25, 1.24, 1.24]);
    public PointCollection EnergyArea { get; } = SparkArea([1.35, 1.32, 1.30, 1.28, 1.26, 1.25, 1.24, 1.24]);

    private const double SparkW = 200, SparkH = 44, SparkPad = 6;

    private static PointCollection SparkLine(double[] values)
    {
        var points = new PointCollection();
        double min = values.Min(), max = values.Max();
        double range = max - min;
        if (range < 1e-9) range = 1;

        for (int i = 0; i < values.Length; i++)
        {
            double x = values.Length == 1 ? 0 : i * (SparkW / (values.Length - 1));
            double y = SparkH - SparkPad - (values[i] - min) / range * (SparkH - 2 * SparkPad);
            points.Add(new Point(x, y));
        }

        return points;
    }

    private static PointCollection SparkArea(double[] values)
    {
        var points = SparkLine(values);
        // Close the shape down to the baseline so it reads as a filled area.
        points.Add(new Point(SparkW, SparkH));
        points.Add(new Point(0, SparkH));
        return points;
    }
}
