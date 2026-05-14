namespace TestApp;

public sealed partial class MainPage : Page
{
	private int _counter;

	public MainPage()
	{
		this.InitializeComponent();
	}

	private void OnCounterClicked(object sender, RoutedEventArgs e)
	{
		_counter++;
		CounterText.Text = $"Counter: {_counter}";
	}
}
