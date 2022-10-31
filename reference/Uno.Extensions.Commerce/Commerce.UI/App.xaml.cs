
using Application = Microsoft.UI.Xaml.Application;

namespace Commerce;

public sealed partial class App : Application
{
	private Window? _window;

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
#if DEBUG
		if (System.Diagnostics.Debugger.IsAttached)
		{
			// this.DebugSettings.EnableFrameRateCounter = true;
		}
#endif

#if NET5_0_OR_GREATER && WINDOWS
            _window = new Window();
#else
		_window = Microsoft.UI.Xaml.Window.Current;
#endif

        var notif = _host.Services.GetRequiredService<IRouteNotifier>();
		notif.RouteChanged += RouteUpdated;

#pragma warning disable CS0618
        _window.AttachNavigation(_host.Services);
#pragma warning restore CS0618
		_window.Activate();

		await Task.Run(() => _host.StartAsync());

		var appSettings = _host.Services.GetRequiredService<IWritableOptions<CommerceApp>>();
		var isDark = appSettings.Value?.IsDark ?? false;
		SystemThemeHelper.SetRootTheme(_window.Content.XamlRoot, isDark);

	}

	public void RouteUpdated(object? sender, RouteChangedEventArgs e)
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
