using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace QuoteCraft;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Uno.UI.FeatureConfiguration.Font.DefaultTextFontFamily = "ms-appx:///Assets/Fonts/DMSans-Regular.ttf#DM Sans";
        this.InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    public static Window MainAppWindow => ((App)Current).MainWindow!;

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
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
                .UseHttp((context, services) => {
#if DEBUG
                // DelegatingHandler will be automatically injected
                services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

})
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<AppDatabase>();
                    services.AddSingleton<IQuoteRepository, QuoteRepository>();
                    services.AddSingleton<IClientRepository, ClientRepository>();
                    services.AddSingleton<IBusinessProfileRepository, BusinessProfileRepository>();
                    services.AddSingleton<ICatalogItemRepository, CatalogItemRepository>();
                    services.AddSingleton<QuoteNumberGenerator>();
                    services.AddSingleton<Services.IPdfGenerator, Services.PdfGenerator>();
                    services.AddSingleton<Services.IShareService, Services.ShareService>();
                    services.AddSingleton<Services.IFeatureGateService, Services.FeatureGateService>();
                    services.AddSingleton<Services.IEmailLauncherService, Services.EmailLauncherService>();
                    services.AddSingleton<Services.IOnboardingService, Services.OnboardingService>();
                    services.AddSingleton<Services.IPhotoService, Services.PhotoService>();

                    services.AddSingleton<IStatusHistoryRepository, StatusHistoryRepository>();

                    // Shared HttpClient for auth, sync, and cloud services
                    services.AddSingleton<HttpClient>();

                    // Auth, sync, and cloud services
                    services.AddSingleton<Services.IAuthService, Services.AuthService>();
                    services.AddSingleton<Data.Remote.SupabaseClient>();
                    services.AddSingleton<Services.ISyncService, Services.SyncService>();
                    services.AddSingleton<Services.ISubscriptionService, Services.SubscriptionService>();
                    services.AddSingleton<Services.INotificationService, Services.NotificationService>();
                    services.AddSingleton<Helpers.ConnectivityHelper>();
                    services.AddSingleton<Services.QuoteExpiryService>();
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"App initialization failed: {ex}");
#if DEBUG
            throw;
#endif
        }
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<OnboardingPage, OnboardingModel>(),
            new ViewMap<AuthPage, AuthModel>(),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<DashboardPage, DashboardModel>(),
            new ViewMap<ClientsPage, ClientsModel>(),
            new ViewMap<CatalogPage, CatalogModel>(),
            new ViewMap<SettingsPage, SettingsModel>(),
            new DataViewMap<ClientEditorPage, ClientEditorModel, ClientEntity>(),
            new DataViewMap<QuoteEditorPage, QuoteEditorModel, QuoteEntity>(),
            new DataViewMap<CatalogEditorPage, CatalogEditorModel, CatalogItemEntity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new ("Onboarding", View: views.FindByViewModel<OnboardingModel>()),
                    new ("Auth", View: views.FindByViewModel<AuthModel>()),
                    new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
                        Nested:
                        [
                            new ("Dashboard", View: views.FindByViewModel<DashboardModel>(), IsDefault: true),
                            new ("Clients", View: views.FindByViewModel<ClientsModel>()),
                            new ("Catalog", View: views.FindByViewModel<CatalogModel>()),
                            new ("Settings", View: views.FindByViewModel<SettingsModel>()),
                            new ("ClientEditor", View: views.FindByViewModel<ClientEditorModel>()),
                            new ("QuoteEditor", View: views.FindByViewModel<QuoteEditorModel>()),
                            new ("CatalogEditor", View: views.FindByViewModel<CatalogEditorModel>()),
                        ]),
                ]
            )
        );
    }
}
