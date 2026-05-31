using System;
using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Services;

public sealed class ChartDataService : IChartDataService
{
    // --- 01 Line: Signal Amplitude (primary + reference, 12 months) ---
    public DataPoint[] GetLinePrimary() => new[]
    {
        new DataPoint(0, 42),  new DataPoint(1, 48),  new DataPoint(2, 55),
        new DataPoint(3, 51),  new DataPoint(4, 63),  new DataPoint(5, 71),
        new DataPoint(6, 68),  new DataPoint(7, 76),  new DataPoint(8, 87.3),
        new DataPoint(9, 81),  new DataPoint(10, 74), new DataPoint(11, 79)
    };

    public DataPoint[] GetLineReference() => new[]
    {
        new DataPoint(0, 38),  new DataPoint(1, 41),  new DataPoint(2, 44),
        new DataPoint(3, 46),  new DataPoint(4, 49),  new DataPoint(5, 53),
        new DataPoint(6, 57),  new DataPoint(7, 61),  new DataPoint(8, 65),
        new DataPoint(9, 67),  new DataPoint(10, 70), new DataPoint(11, 72)
    };

    // --- 02 Bar: Monthly Throughput ---
    public double[] GetBarData() => new double[]
    {
        58, 62, 71, 65, 78, 73, 84, 91, 82, 76, 69, 74
    };

    // --- 03 Area: Cumulative Load (two layers) ---
    public DataPoint[] GetAreaLayerA() => new[]
    {
        new DataPoint(0, 20),  new DataPoint(1, 24),  new DataPoint(2, 28),
        new DataPoint(3, 32),  new DataPoint(4, 38),  new DataPoint(5, 45),
        new DataPoint(6, 50),  new DataPoint(7, 56),  new DataPoint(8, 60),
        new DataPoint(9, 64),  new DataPoint(10, 70), new DataPoint(11, 76)
    };

    public DataPoint[] GetAreaLayerB() => new[]
    {
        new DataPoint(0, 8),  new DataPoint(1, 10), new DataPoint(2, 12),
        new DataPoint(3, 14), new DataPoint(4, 16), new DataPoint(5, 18),
        new DataPoint(6, 20), new DataPoint(7, 23), new DataPoint(8, 25),
        new DataPoint(9, 26), new DataPoint(10, 28), new DataPoint(11, 30)
    };

    // --- 04 Scatter: Bivariate Correlation ---
    public DataPoint[] GetScatterPoints()
    {
        var pts = new DataPoint[48];
        for (int i = 0; i < pts.Length; i++)
        {
            double x = (i * 13 % 100);
            double noise = ((i * 7 + 11) % 30) - 15;
            double y = x * 0.85 + noise;
            pts[i] = new DataPoint(x, Math.Clamp(y, 0, 100));
        }
        return pts;
    }

    // --- 05 HBar: Revenue by plan (ranked) ---
    public HBarItem[] GetHBarItems() => new[]
    {
        new HBarItem("SCALE",   92),
        new HBarItem("PRO",     78),
        new HBarItem("TEAM",    65),
        new HBarItem("STARTER", 54),
        new HBarItem("FREE",    41),
        new HBarItem("TRIAL",   28)
    };

    // --- 06 Histogram: Frequency Distribution (20 bins, gaussian-ish) ---
    public double[] GetHistogramBins() => new double[]
    {
        2, 4, 6, 10, 16, 22, 30, 38, 44, 50,
        54, 50, 44, 38, 30, 22, 16, 10, 6, 4
    };

    // --- 07 Heatmap: Temporal Density (7 days × 24 hours = 168 cells) ---
    public HeatmapCell[] GetHeatmapData()
    {
        var cells = new HeatmapCell[7 * 24];
        int i = 0;
        for (int d = 0; d < 7; d++)
        {
            for (int h = 0; h < 24; h++)
            {
                // Daily activity curve: ramp 6-12, peak 12-14, taper 14-22, low otherwise.
                double baseline = h switch
                {
                    < 6 => 8,
                    < 9 => 18 + (h - 6) * 12,
                    < 12 => 50 + (h - 9) * 8,
                    < 15 => 78 + Math.Abs(h - 13) * -6,
                    < 18 => 70 - (h - 15) * 8,
                    < 22 => 46 - (h - 18) * 6,
                    _ => 14
                };
                // Weekday/weekend modulation
                double mod = (d == 5 || d == 6) ? 0.7 : 1.0;
                double jitter = ((d * 17 + h * 23) % 20) - 10;
                double v = Math.Clamp(baseline * mod + jitter, 0, 100);
                cells[i++] = new HeatmapCell(d, h, v);
            }
        }
        return cells;
    }

    // --- 08 Arc: Goal-attainment rings (4 concentric rings, descending radius) ---
    public RingMetric[] GetArcMetrics() => new[]
    {
        new RingMetric("Activation", 79, 70),
        new RingMetric("Retention",  62, 56),
        new RingMetric("Expansion",  91, 42),
        new RingMetric("Adoption",   48, 28)
    };

    // --- 09 Box Plot: Session length by plan (5 plans) ---
    public BoxData[] GetBoxPlotData() => new[]
    {
        new BoxData("FREE",    12, 28, 42, 56, 78, new[] {  6.0, 88 }),
        new BoxData("STARTER", 18, 34, 48, 62, 82, new[] { 92.0    }),
        new BoxData("PRO",     22, 40, 55, 68, 86, new double[0]),
        new BoxData("TEAM",    16, 36, 52, 66, 84, new[] {  4.0, 94 }),
        new BoxData("SCALE",   26, 44, 58, 72, 88, new[] { 10.0    })
    };

    // --- 10 Gauge: System Health (single value) ---
    public double GetGaugeValue() => 74;

    // --- 11 Slope: Retention by segment, period over period ---
    public SlopeItem[] GetSlopeItems() => new[]
    {
        new SlopeItem("SMB",     34, 62),
        new SlopeItem("MID-MKT", 48, 70),
        new SlopeItem("ENT",     56, 44),
        new SlopeItem("EDU",     62, 78),
        new SlopeItem("PUBLIC",  30, 52),
        new SlopeItem("TRIAL",   58, 36)
    };

    // --- 12 Dot Strip: Individual Observation (60 points, gaussian-ish) ---
    public double[] GetDotStripData()
    {
        var pts = new double[60];
        for (int i = 0; i < pts.Length; i++)
        {
            // Box-Muller-ish deterministic distribution centered at 50
            double u = ((i * 7919 + 1009) % 997) / 997.0;
            double v = ((i * 6151 + 491)  % 991) / 991.0;
            double g = Math.Sqrt(-2 * Math.Log(u == 0 ? 0.001 : u)) * Math.Cos(2 * Math.PI * v);
            pts[i] = Math.Clamp(50 + g * 14, 0, 100);
        }
        return pts;
    }
}
