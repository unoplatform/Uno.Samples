using ScottPlot;

namespace ScottPlotSample;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        for (int i = 1; i <= 5; i++)
        {
            double[] data = Generate.RandomWalk(1_000_000);
            WinUIPlot1.Plot.Add.Signal(data);
        }
        WinUIPlot1.Plot.Title("Signal plot with 5 million points");
    }
}
