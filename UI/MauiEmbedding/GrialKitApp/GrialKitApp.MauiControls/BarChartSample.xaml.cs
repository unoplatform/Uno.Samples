namespace GrialKitApp.MauiControls;

public partial class BarChartSample : ContentPage
{
	public BarChartSample()
	{
		InitializeComponent();

        // TODO: Uncomment
        chart1.BindingContext = chart2.BindingContext = SampleData.Bar();
    }
}
