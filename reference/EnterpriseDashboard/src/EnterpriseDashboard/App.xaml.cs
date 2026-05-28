using System.Diagnostics.CodeAnalysis;
using EnterpriseDashboard.Observatory.Services;
using EnterpriseDashboard.Observatory.Views;
using EnterpriseDashboard.Views;
using Uno.Extensions.Navigation;
using Uno.Resizetizer;

namespace EnterpriseDashboard;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Region-based navigation for Toolkit + WinUI nav controls.
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
                    services.AddSingleton<IChartDataService, ChartDataService>();

                    // VMs — Shell/ShellPage VMs are resolved through their ViewMaps.
                    // Section VMs are transient so each navigated page gets its own.
                    services.AddTransient<ShellViewModel>();
                    services.AddTransient<ShellPageViewModel>();
                    services.AddTransient<AcquisitionViewModel>();
                    services.AddTransient<EngagementViewModel>();
                    services.AddTransient<CohortsViewModel>();
                })
                .UseNavigation(RegisterRoutes));
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();

        if (MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = ElementTheme.Dark;
        }
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            // Shell is registered VM-only (no View pairing) per Uno.Extensions Navigation:
            // pairing Shell with ShellViewModel would prevent the nested IsDefault route from
            // firing on bootstrap, leaving the ShellContent empty.
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<ShellPage, ShellPageViewModel>(),
            new ViewMap<AcquisitionPage, AcquisitionViewModel>(),
            new ViewMap<EngagementPage, EngagementViewModel>(),
            new ViewMap<CohortsPage, CohortsViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByViewModel<ShellPageViewModel>(), IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Acquisition", View: views.FindByViewModel<AcquisitionViewModel>(), IsDefault: true),
                            new RouteMap("Engagement", View: views.FindByViewModel<EngagementViewModel>()),
                            new RouteMap("Cohorts", View: views.FindByViewModel<CohortsViewModel>())
                        ])
                ])
        );
    }
}
