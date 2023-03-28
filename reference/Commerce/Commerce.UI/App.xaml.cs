
using Application = Microsoft.UI.Xaml.Application;

namespace Commerce;

public sealed partial class App : Application
{
	private Window? _window;

    private IHost? _host;

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

        var appBuilder = this.CreateBuilder(args)
            .ConfigureApp()
            .UseToolkitNavigation();

        _window = appBuilder.Window;
#if NET5_0_OR_GREATER && WINDOWS
		_window.Activate();
#endif

        //await Task.Run(() => _host.StartAsync());
        _host = await _window.InitializeNavigationAsync(async () => appBuilder.Build());

        var notif = _host!.Services.GetRequiredService<IRouteNotifier>();
        notif.RouteChanged += RouteUpdated;

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
