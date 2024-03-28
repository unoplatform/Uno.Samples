
#pragma warning disable 109 // Remove warning for Window property on iOS


namespace ToDo;

public sealed partial class App : Application
{
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	private Window? _window;
	public new Window? Window => _window;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

	public App()
	{
		this.InitializeComponent();
	}

	/// <summary>
	/// Invoked when the application is launched normally by the end user.  Other entry points
	/// will be used such as when the application is launched to open a specific file.
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected async override void OnLaunched(LaunchActivatedEventArgs args)
	{
#if USE_UITESTS
		Uno.UI.FeatureConfiguration.UIElement.AssignDOMXamlName = true;
#endif


#if DEBUG
		if (System.Diagnostics.Debugger.IsAttached)
		{
			// this.DebugSettings.EnableFrameRateCounter = true;
		}
#endif

#if NET5_0_OR_GREATER && WINDOWS && !HAS_UNO
		_window = new Window();
#else
		_window = Microsoft.UI.Xaml.Window.Current;
#endif
		var appRoot = new Shell();
		appRoot.SplashScreen.Initialize(_window, args);

		_window.Content = appRoot;
		_window.Activate();

		_host = await _window.InitializeNavigationAsync(
					async () =>
					{
						return BuildAppHost();
					},
					navigationRoot: appRoot.SplashScreen
				);

		var notif = _host.Services.GetRequiredService<IRouteNotifier>();
		notif.RouteChanged += RouteUpdated;


		var appSettings = _host.Services.GetRequiredService<IWritableOptions<ToDoApp>>();
		var isDark = appSettings.Value?.IsDark ?? false;
		SystemThemeHelper.SetRootTheme(_window.Content.XamlRoot, isDark);
	}

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	public void RouteUpdated(object? sender, RouteChangedEventArgs e)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
	{
		try
		{
			var rootRegion = e.Region.Root();
			var route = rootRegion.GetRoute();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Error: " + ex.Message);
		}
	}
}
