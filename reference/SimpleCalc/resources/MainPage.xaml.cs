namespace SimpleCalculator;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
	public MainPage()
	{
		this.InitializeComponent();
		DataContext = new $ModelName$(this.GetThemeService());
	}
}
