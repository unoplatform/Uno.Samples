#nullable enable

namespace TestApp;

public sealed partial class App : Application
{
	private Window? _window;

	public App()
	{
		this.InitializeComponent();
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		_window = new Window();
		_window.Content = new MainPage();
		_window.Activate();
	}

	/// <summary>
	/// Configures global logging (called from Program.cs before host creation).
	/// </summary>
	public static void InitializeLogging()
	{
		// Minimal logging for the test app — no additional configuration needed.
	}
}
