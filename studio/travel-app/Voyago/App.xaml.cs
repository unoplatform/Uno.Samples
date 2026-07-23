using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace Voyago;

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
                .UseHttp((context, services) => {
#if DEBUG
                // DelegatingHandler will be automatically injected
                services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

})
                .ConfigureServices((context, services) =>
                {
                    // One shared trip book for the whole app: DestinationDetailModel books into it
                    // and TripsModel lists it — shared mutable state as a singleton IListState.
                    services.AddSingleton<Presentation.Services.ITripsService, Presentation.Services.TripsService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
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
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<HomePage, HomeModel>(),
            new ViewMap<SearchPage, SearchModel>(),
            new ViewMap<TripsPage, TripsModel>(),
            new ViewMap<FavoritesPage, FavoritesModel>(),
            new ViewMap<ProfilePage, ProfileModel>(),
            // The detail is reached by tapping a destination; the tapped Destination is passed as the
            // model's data (DataViewMap), so each card opens its own. It's a ContentDialog shown with
            // the "!" (dialog) qualifier — a true modal overlaying the dimmed shell on desktop, and
            // full-window on phones.
            new DataViewMap<DestinationDetailDialog, DestinationDetailModel, Destination>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByView<MainPage>(),
                        IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Home", View: views.FindByView<HomePage>(), IsDefault: true),
                            new RouteMap("Search", View: views.FindByView<SearchPage>()),
                            new RouteMap("Trips", View: views.FindByView<TripsPage>()),
                            new RouteMap("Favorites", View: views.FindByView<FavoritesPage>()),
                            new RouteMap("Profile", View: views.FindByView<ProfilePage>())
                        ]
                    ),
                    // Reached with the "!" dialog qualifier (see the cards' Navigation.Request), so it
                    // presents as a modal ContentDialog over the shell rather than replacing it.
                    new RouteMap("DestinationDetail", View: views.FindByView<DestinationDetailDialog>()),
                ]
            )
        );
    }

}
