namespace GrialKitApp.MauiControls;

public partial class BarChartSample : ContentPage
{
	public BarChartSample()
	{
		InitializeComponent();

        chart1.BindingContext = chart2.BindingContext = SampleData.Bar();
    }
}
