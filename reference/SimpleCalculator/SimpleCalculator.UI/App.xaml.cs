using System.Runtime.InteropServices;

namespace SimpleCalculator;

public sealed partial class App : Application
{
	private Window? _window;

	public App()
	{
		this.InitializeComponent();

#if HAS_UNO || NETFX_CORE
		this.Suspending += OnSuspending;
#endif
	}

	/// <summary>
	/// Invoked when the application is launched normally by the end user.  Other entry points
	/// will be used such as when the application is launched to open a specific file.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected async override void OnLaunched(LaunchActivatedEventArgs args)
	{

#if NET6_0_OR_GREATER && WINDOWS && !HAS_UNO
		_window = new Window();
#else
		_window = Microsoft.UI.Xaml.Window.Current;
#endif

		var appRoot = new Shell();
		appRoot.SplashScreen.Initialize(_window, args);

		_window.Content = appRoot;
		_window.Activate();

		Host = await _window.InitializeNavigationAsync(
					async () =>
					{
						return BuildAppHost();
					},
					navigationRoot: appRoot.SplashScreen
				);
	}

	/// <summary>
	/// Invoked when application execution is being suspended.  Application state is saved
	/// without knowing whether the application will be terminated or resumed with the contents
	/// of memory still intact.
	/// </summary>
	/// <param name="sender">The source of the suspend request.</param>
	/// <param name="e">Details about the suspend request.</param>
	private void OnSuspending(object sender, SuspendingEventArgs e)
	{
		var deferral = e.SuspendingOperation.GetDeferral();
		// TODO: Save application state and stop any background activity
		deferral.Complete();
	}
}
