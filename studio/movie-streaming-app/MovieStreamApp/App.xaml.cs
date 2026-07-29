using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace MovieStreamApp;

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
                }, enableUnoLogging: true)
                .ConfigureServices((context, services) =>
                {
                    // The shared "My List" store: one instance injected into every page-Model that
                    // reads or mutates the watchlist, so a toggle propagates everywhere (lesson 39).
                    services.AddSingleton<WatchlistService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        // Navigate to the Shell, which shows the extended splash screen while the host starts and
        // then reveals the navigated content (Main -> Browse) in its place.
        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<OnboardingPage, OnboardingModel>(),
            new ViewMap<BrowsePage, BrowseModel>(),
            new ViewMap<SearchPage, SearchModel>(),
            new ViewMap<ProfilePage, ProfileModel>(),
            new ViewMap<SocialFeedPage, SocialFeedModel>(),
            // Playback and the movie detail (page + desktop modal) each take the tapped Movie as
            // their nav-data parameter, injected into the model's constructor.
            new DataViewMap<PlaybackPage, PlaybackModel, Movie>(),
            // The page and the desktop-modal dialog use DISTINCT view-model types (see
            // MovieDetailDialogModel): a reactive model cannot be shared across two DataViewMaps.
            new DataViewMap<MovieDetailPage, MovieDetailModel, Movie>(),
            new DataViewMap<MovieDetailDialog, MovieDetailDialogModel, Movie>()
        );

        routes.Register(
            // The Shell hosts the extended splash screen and is the navigation root.
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    // MainPage is the tab shell: it owns the floating TabBar chrome and a content
                    // region the four tab pages are injected into.
                    new RouteMap("Main", View: views.FindByView<MainPage>(),
                        IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Browse", View: views.FindByView<BrowsePage>(), IsDefault: true),
                            new RouteMap("Search", View: views.FindByView<SearchPage>()),
                            new RouteMap("SocialFeed", View: views.FindByView<SocialFeedPage>()),
                            new RouteMap("Profile", View: views.FindByView<ProfilePage>())
                        ]),
                    // Siblings of Main (NOT tabs): shown full-screen over the shell so the TabBar
                    // doesn't overlay them. MovieDetail is the phone/tablet page; MovieDetailModal
                    // is the desktop modal (same model); Back ("-") returns to the originating tab.
                    new RouteMap("Onboarding", View: views.FindByView<OnboardingPage>()),
                    new RouteMap("MovieDetail", View: views.FindByView<MovieDetailPage>()),
                    new RouteMap("MovieDetailModal", View: views.FindByView<MovieDetailDialog>()),
                    new RouteMap("Playback", View: views.FindByView<PlaybackPage>())
                ])
        );
    }

}
