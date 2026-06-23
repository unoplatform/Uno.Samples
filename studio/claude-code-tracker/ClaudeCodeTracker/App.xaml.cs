using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace ClaudeCodeTracker;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    // Configure log levels for different categories of logging
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)

                        // Default filters for core Uno Platform namespaces
                        .CoreLogLevel(LogLevel.Warning);

                    // Uno Platform namespace filter groups
                    // Uncomment individual methods to see more detailed logging
                    //// Generic Xaml events
                    //logBuilder.XamlLogLevel(LogLevel.Debug);
                    //// Layout specific messages
                    //logBuilder.XamlLayoutLogLevel(LogLevel.Debug);
                    //// Storage messages
                    //logBuilder.StorageLogLevel(LogLevel.Debug);
                    //// Binding related messages
                    //logBuilder.XamlBindingLogLevel(LogLevel.Debug);
                    //// Binder memory references tracking
                    //logBuilder.BinderMemoryReferenceLogLevel(LogLevel.Debug);
                    //// DevServer and HotReload related
                    //logBuilder.HotReloadCoreLogLevel(LogLevel.Information);
                    //// Debug JS interop
                    //logBuilder.WebAssemblyLogLevel(LogLevel.Debug);

                }, enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;
        MainWindow.Title = "Claude Code Tracker";

        #if DEBUG
        MainWindow.UseStudio();
#endif
                MainWindow.SetWindowIcon();
        // Show the app icon in the macOS Dock when launched unbundled (Windows/Linux use the
        // window icon above). No-op on other platforms — see Platforms/Desktop/AppDockIcon.cs.
        SetMacOSDockIcon();

        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(builder.Build()),
            initialRoute: "Main"
        );
    }

    /// <summary>macOS Dock icon hook, implemented for the desktop head only; no-op elsewhere.</summary>
    partial void SetMacOSDockIcon();

    	private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
	{
		views.Register(
			new ViewMap<MainPage, MainModel>(),
			new ViewMap<DashboardPage, DashboardModel>(),
			new ViewMap<SessionsPage, SessionsModel>(),
			new ViewMap<UsagePage, UsageModel>(),
			new ViewMap<ChartsPage, ChartsModel>(),
			new DataViewMap<SessionDetailPage, SessionDetailModel, SessionEntry>()
		);

		routes.Register(
			new RouteMap("Main", View: views.FindByViewModel<MainModel>(),
				IsDefault: true,
				Nested:
				[
					new RouteMap("Dashboard", View: views.FindByView<DashboardPage>(), IsDefault: true),
					new RouteMap("Sessions", View: views.FindByView<SessionsPage>()),
					new RouteMap("Usage", View: views.FindByView<UsagePage>()),
					new RouteMap("Charts", View: views.FindByView<ChartsPage>()),
					new RouteMap("SessionDetail", View: views.FindByView<SessionDetailPage>())
				]
			)
		);
	}

}
