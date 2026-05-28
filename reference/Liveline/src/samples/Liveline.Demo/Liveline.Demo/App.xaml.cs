using Uno.Resizetizer;

namespace Liveline.Demo;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Region-based navigation (Shell hosts navigated content via IContentControlProvider).
            .UseToolkitNavigation()
            .Configure(host => host
                .UseLogging(configure: (context, logBuilder) =>
                    logBuilder
                        .SetMinimumLevel(LogLevel.Information)
                        .CoreLogLevel(LogLevel.Warning),
                    enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    // Page VMs are constructed by the navigation system via DI.
                    services.AddTransient<MainViewModel>();
                })
                .UseNavigation(RegisterRoutes));

        MainWindow = builder.Window;
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<Shell, ShellViewModel>(),
            new ViewMap<MainPage, MainViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByViewModel<MainViewModel>(), IsDefault: true)
                ])
        );
    }
}
