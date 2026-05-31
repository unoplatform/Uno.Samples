using System.Diagnostics.CodeAnalysis;
using Meridian.Presentation;
using Meridian.Services;
using Meridian.Views;
using Uno.Resizetizer;

namespace Meridian;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>The application host. Kept as a static accessor for utility code (e.g. the
    /// DashboardPage live ticker still resolves <c>FinnhubService</c> here for its push-based
    /// event subscription). Page-level service resolution for the MVUX Models now happens
    /// through the navigation system via the ViewMap registrations below.</summary>
    public static IServiceProvider Services { get; private set; } = null!;
    public Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Required so the Shell ContentControl host (IContentControlProvider) is wired
            // by Uno.Extensions Navigation. Toolkit also enables Region.Attached on
            // Toolkit controls if/when they're added later.
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                }, enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IMarketDataService, MockMarketDataService>();
                    var apiKey = Environment.GetEnvironmentVariable("FINNHUB_API_KEY") ?? "";
                    services.AddSingleton(sp =>
                        new FinnhubService(apiKey, ["AAPL", "NVDA", "MSFT", "GOOGL", "META", "TSLA"]));
                })
                // ReactiveViewModelMappings.ViewModelMappings pairs each MVUX Model
                // (e.g. DashboardModel) with its generated bindable proxy (DashboardViewModel)
                // when a route resolves.
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        // Fixed-width layout target — see ReadMe Known Limitations.
        MainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 1500, Height = 900 });

        // Build the host before NavigateAsync so App.Services is populated *before*
        // the Shell mounts and the first Page's Loaded event fires. NavigateAsync
        // would otherwise emit Loaded while App.Services is still null, and code-
        // behind that calls App.Services.GetRequiredService<T>() in OnPageLoaded
        // throws ArgumentNullException for `provider` (the message is logged by
        // FrameNavigator as "Show - Unable to navigate to page").
        Host = builder.Build();
        Services = Host.Services;

        await builder.NavigateAsync<Shell>();

        MainWindow.Title = "Meridian";
    }

    // Routes:
    //   ""              -> Shell (bootstrap Frame host, no UI of its own)
    //     "Dashboard"   -> DashboardPage [default]
    //     "StockDetail" -> StockDetailPage (typed nav data: string ticker)
    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            // Shell is registered VM-only (no View pairing) per Uno.Extensions Navigation:
            // pairing Shell with ShellModel here would prevent the nested IsDefault route
            // from firing on bootstrap, leaving the ShellContent empty.
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<DashboardPage, DashboardModel>(),
            new DataViewMap<StockDetailPage, StockDetailModel, string>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new RouteMap("Dashboard", View: views.FindByViewModel<DashboardModel>(), IsDefault: true),
                    new RouteMap("StockDetail", View: views.FindByViewModel<StockDetailModel>(), DependsOn: "Dashboard")
                ])
        );
    }
}
