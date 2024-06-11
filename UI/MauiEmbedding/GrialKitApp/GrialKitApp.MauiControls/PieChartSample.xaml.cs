namespace GrialKitApp.MauiControls;

public partial class PieChartSample : ContentPage
{
	public PieChartSample()
	{
		InitializeComponent();

        // TODO: Uncomment
        chart.BindingContext = SampleData.Ring();
    }
}
