namespace GrialKitApp.MauiControls;

public partial class BarMultiSeriesSample : ContentPage
{
	public BarMultiSeriesSample()
	{
		InitializeComponent();

		chart.BindingContext = SampleData.MultiSeriesBar();

    }
}
