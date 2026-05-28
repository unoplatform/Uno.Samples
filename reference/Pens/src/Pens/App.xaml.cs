using Pens.Presentation;
using Pens.Services;
using Uno.Resizetizer;

namespace Pens;

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
            // Region-based navigation for Toolkit controls (TabBar)
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Loads the gitignored appsettings.development.json (real Supabase creds)
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging(configure: (context, logBuilder) =>
                    logBuilder
                        .SetMinimumLevel(LogLevel.Information)
                        .CoreLogLevel(LogLevel.Warning),
                    enableUnoLogging: true)
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                .ConfigureServices((context, services) =>
                {
                    // Use the real backend only when a project is configured (Url + key
                    // present); otherwise run on built-in mock data. The distributed
                    // sample ships with empty config -> mock, so it never touches a
                    // private backend. See SUPABASE.md.
                    var url = context.Configuration["Supabase:Url"];
                    var key = context.Configuration["Supabase:AnonKey"];
                    if (!string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(key))
                    {
                        services.AddSingleton<ISupabaseService, SupabaseService>();
                    }
                    else
                    {
                        services.AddSingleton<ISupabaseService, MockSupabaseService>();
                    }

                    services.AddSingleton<IPlayerIdentityService, PlayerIdentityService>();

                    // Page ViewModels (Shell/Main/PlayerPicker VMs are registered via their ViewMaps)
                    services.AddTransient<ScheduleViewModel>();
                    services.AddTransient<ChatViewModel>();
                    services.AddTransient<BeersViewModel>();
                    services.AddTransient<DutiesViewModel>();
                    services.AddTransient<RosterViewModel>();
                })
                .UseNavigation(RegisterRoutes));

        MainWindow = builder.Window;
        MainWindow.SetWindowIcon();

        // Capture the UI DispatcherQueue (we're on the UI thread here) so ViewModels
        // — which the navigation system constructs on a background thread — can
        // marshal bound-state updates back to the UI thread.
        UiThread.Dispatcher = MainWindow.DispatcherQueue;

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<Shell, ShellViewModel>(),
            new ViewMap<PlayerPickerPage, PlayerPickerViewModel>(),
            new ViewMap<MainPage, MainViewModel>(),
            new ViewMap<SchedulePage, ScheduleViewModel>(),
            new ViewMap<ChatPage, ChatViewModel>(),
            new ViewMap<BeersPage, BeersViewModel>(),
            new ViewMap<DutiesPage, DutiesViewModel>(),
            new ViewMap<RosterPage, RosterViewModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                Nested:
                [
                    // Login gate (shown by ShellViewModel when no player is selected)
                    new RouteMap("Login", View: views.FindByViewModel<PlayerPickerViewModel>()),

                    // Tab shell
                    new RouteMap("Main", View: views.FindByViewModel<MainViewModel>(),
                        Nested:
                        [
                            new RouteMap("Schedule", View: views.FindByViewModel<ScheduleViewModel>(), IsDefault: true),
                            new RouteMap("Chat", View: views.FindByViewModel<ChatViewModel>()),
                            new RouteMap("Beers", View: views.FindByViewModel<BeersViewModel>()),
                            new RouteMap("Duties", View: views.FindByViewModel<DutiesViewModel>()),
                            new RouteMap("Roster", View: views.FindByViewModel<RosterViewModel>())
                        ])
                ])
        );
    }
}

public class AppConfig
{
    public SupabaseConfig? Supabase { get; set; }
}

public class SupabaseConfig
{
    public string? Url { get; set; }
    public string? AnonKey { get; set; }
}
