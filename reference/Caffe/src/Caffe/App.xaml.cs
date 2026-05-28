using Caffe.ViewModels;
using Uno.Resizetizer;

namespace Caffe;

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
    public IHost? Host { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Region-based navigation for Toolkit controls.
            .UseToolkitNavigation()
            .Configure(host => host
                .UseLogging(configure: (context, logBuilder) =>
                    logBuilder
                        .SetMinimumLevel(LogLevel.Information)
                        .CoreLogLevel(LogLevel.Warning),
                    enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    // Single-page tool: one page VM, transient. Shell VM is registered
                    // by its ViewMap and resolved by the navigation system.
                    services.AddTransient<MainViewModel>();
                })
                .UseNavigation(RegisterRoutes));

        MainWindow = builder.Window;
#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            // Shell registered VM-only — pairing with its View prevents the nested
            // IsDefault route from firing on bootstrap (ShellContent stays empty,
            // window renders blank). Same gotcha noted in Meridian and Nexus.
            new ViewMap(ViewModel: typeof(ShellViewModel)),
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
