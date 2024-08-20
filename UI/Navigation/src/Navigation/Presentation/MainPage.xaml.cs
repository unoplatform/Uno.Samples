namespace Navigation.Presentation;

public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
	}

	private void NavViewToggleButton_Click(object sender, RoutedEventArgs e)
	{
		NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
	}
}
