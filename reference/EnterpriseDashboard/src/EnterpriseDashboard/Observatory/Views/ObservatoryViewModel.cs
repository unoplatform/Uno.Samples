using System.ComponentModel;
using EnterpriseDashboard.Observatory.Models;
using EnterpriseDashboard.Observatory.Services;

namespace EnterpriseDashboard.Observatory.Views;

// Plain INPC view model — MVUX FeedView wasn't projecting the Data binding into
// our chart DPs reliably with synchronous source data, so we use direct properties.
public class ObservatoryViewModel : INotifyPropertyChanged
{
    public ObservatoryViewModel(IChartDataService data)
    {
        LineSeriesA = data.GetLinePrimary();
        LineSeriesB = data.GetLineReference();
        BarValues = data.GetBarData();
        AreaLayerA = data.GetAreaLayerA();
        AreaLayerB = data.GetAreaLayerB();
        ScatterPoints = data.GetScatterPoints();
        HBarItems = data.GetHBarItems();
        HistogramBins = data.GetHistogramBins();
        HeatmapCells = data.GetHeatmapData();
        ArcMetrics = data.GetArcMetrics();
        BoxPlotGroups = data.GetBoxPlotData();
        GaugeValue = data.GetGaugeValue();
        SlopeItems = data.GetSlopeItems();
        DotStripPoints = data.GetDotStripData();
    }

    public DataPoint[] LineSeriesA { get; }
    public DataPoint[] LineSeriesB { get; }
    public double[] BarValues { get; }
    public DataPoint[] AreaLayerA { get; }
    public DataPoint[] AreaLayerB { get; }
    public DataPoint[] ScatterPoints { get; }
    public HBarItem[] HBarItems { get; }
    public double[] HistogramBins { get; }
    public HeatmapCell[] HeatmapCells { get; }
    public RingMetric[] ArcMetrics { get; }
    public BoxData[] BoxPlotGroups { get; }
    public double GaugeValue { get; }
    public SlopeItem[] SlopeItems { get; }
    public double[] DotStripPoints { get; }

    private int _resetKey;
    public int ResetKey
    {
        get => _resetKey;
        set { _resetKey = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResetKey))); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
