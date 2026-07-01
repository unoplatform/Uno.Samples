namespace ProjectionGallery;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
	}

	private void OnReset(object sender, RoutedEventArgs e)
	{
		RotX.Value = 0;
		RotY.Value = 0;
		RotZ.Value = 0;
	}
}
