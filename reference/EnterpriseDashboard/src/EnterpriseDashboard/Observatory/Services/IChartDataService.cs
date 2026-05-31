using EnterpriseDashboard.Observatory.Models;

namespace EnterpriseDashboard.Observatory.Services;

public interface IChartDataService
{
    DataPoint[] GetLinePrimary();
    DataPoint[] GetLineReference();
    double[] GetBarData();
    DataPoint[] GetAreaLayerA();
    DataPoint[] GetAreaLayerB();
    DataPoint[] GetScatterPoints();
    HBarItem[] GetHBarItems();
    double[] GetHistogramBins();
    HeatmapCell[] GetHeatmapData();
    RingMetric[] GetArcMetrics();
    BoxData[] GetBoxPlotData();
    SlopeItem[] GetSlopeItems();
    double[] GetDotStripData();
    double GetGaugeValue();
}
