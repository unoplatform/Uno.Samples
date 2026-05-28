using Nexus.Presentation;
using Nexus.Services;
using Uno.Resizetizer;

namespace Nexus;

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

    /// <summary>The application host. Kept as a static accessor for any utility code that
    /// might still need it; page-level service resolution now happens through the navigation
    /// system via the ViewMap registrations below.</summary>
    public static IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            var builder = this.CreateBuilder(args)
            // Region-based navigation for Toolkit controls (TabBar) — required so TabBarItem
            // Region.Name entries resolve to the nested RouteMaps registered below.
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
                .UseHttp((context, services) => {
#if DEBUG
                // DelegatingHandler will be automatically injected
                services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

})
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<INexusService, MockNexusService>();
                })
                // ReactiveViewModelMappings.ViewModelMappings tells the navigation system how to
                // pair each MVUX Model (e.g. OverviewModel) with its generated bindable proxy
                // (OverviewViewModel) when a route resolves. Without this, the ViewMaps below
                // would try to construct the Model directly and miss the proxy wiring.
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();

        MainWindow.Title = "Nexus - Industrial Control System";
        }
        catch (Exception ex)
        {
            var innerEx = ex.InnerException ?? ex;
            Console.WriteLine($"Error in OnLaunched: {innerEx.Message}");
            Console.WriteLine($"Exception Type: {innerEx.GetType().Name}");
            Console.WriteLine($"Stack trace: {innerEx.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner stack: {ex.InnerException.StackTrace}");
            }
            throw;
        }
    }

    // Routes:
    //   ""           -> Shell (bootstrap ContentControl, no UI of its own)
    //     "Main"     -> MainPage (chrome: header + tabs + footer + content region) [default]
    //       "Overview"    -> OverviewPage [default]
    //       "Production"  -> ProductionPage
    //       "Analytics"   -> AnalyticsPage
    //       "Maintenance" -> MaintenancePage
    //       "Settings"    -> SettingsPage
    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            // Shell is registered VM-only (no View pairing) per Uno.Extensions Navigation:
            // pairing Shell with ShellModel would prevent the nested IsDefault route from
            // firing on bootstrap, leaving the ShellContent empty.
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<OverviewPage, OverviewModel>(),
            new ViewMap<ProductionPage, ProductionModel>(),
            new ViewMap<AnalyticsPage, AnalyticsModel>(),
            new ViewMap<MaintenancePage, MaintenanceModel>(),
            new ViewMap<SettingsPage, SettingsModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Overview", View: views.FindByViewModel<OverviewModel>(), IsDefault: true),
                            new RouteMap("Production", View: views.FindByViewModel<ProductionModel>()),
                            new RouteMap("Analytics", View: views.FindByViewModel<AnalyticsModel>()),
                            new RouteMap("Maintenance", View: views.FindByViewModel<MaintenanceModel>()),
                            new RouteMap("Settings", View: views.FindByViewModel<SettingsModel>())
                        ])
                ])
        );
    }
}
