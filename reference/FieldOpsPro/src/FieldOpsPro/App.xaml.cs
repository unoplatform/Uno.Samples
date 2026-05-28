using Uno.Resizetizer;

namespace FieldOpsPro;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
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
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                .UseLocalization()
                .ConfigureServices((context, services) =>
                {
                    // Register FieldOps services
                    services.AddSingleton<IFieldOpsService, MockFieldOpsService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );

        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();

        // Uno Skia desktop doesn't always propagate Application.RequestedTheme to
        // the window content, so mirror it explicitly. With no App.xaml override
        // this follows the system theme; toggle the Windows app-mode setting to
        // switch between Light and Dark.
        if (MainWindow.Content is FrameworkElement root)
        {
            root.RequestedTheme = Application.Current.RequestedTheme switch
            {
                ApplicationTheme.Light => ElementTheme.Light,
                _ => ElementTheme.Dark,
            };
        }
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<DashboardPage, DashboardModel>(),
            new ViewMap<MapPage, MapModel>(),
            new ViewMap<WorkOrdersPage, WorkOrdersModel>(),
            new ViewMap<TeamPage, TeamModel>(),
            new ViewMap(View: typeof(SchedulePage)),
            new ViewMap(View: typeof(ReportsPage)),
            new ViewMap(View: typeof(SettingsPage)),
            new ViewMap(View: typeof(ProfilePage))
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    // MainPage owns the nav chrome (NavigationView / TabBar) and hosts the
                    // section routes as its own nested regions.
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Dashboard", View: views.FindByViewModel<DashboardModel>(), IsDefault: true),
                            new RouteMap("Map", View: views.FindByViewModel<MapModel>()),
                            new RouteMap("Tasks", View: views.FindByViewModel<WorkOrdersModel>()),
                            new RouteMap("Team", View: views.FindByViewModel<TeamModel>()),
                            new RouteMap("Schedule", View: views.FindByView<SchedulePage>()),
                            new RouteMap("Reports", View: views.FindByView<ReportsPage>()),
                            new RouteMap("Settings", View: views.FindByView<SettingsPage>()),
                            new RouteMap("Profile", View: views.FindByView<ProfilePage>()),
                        ]
                    )
                ]
            )
        );
    }
}
