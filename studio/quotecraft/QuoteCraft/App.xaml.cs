using System.Diagnostics.CodeAnalysis;

namespace QuoteCraft;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
#if HAS_UNO
        // Uno-only: WinAppSDK has no FeatureConfiguration and takes DM Sans from the theme's DefaultFontFamily alone.
        Uno.UI.FeatureConfiguration.Font.DefaultTextFontFamily = "ms-appx:///Assets/Fonts/DMSans-Regular.ttf#DM Sans";
#endif
        this.InitializeComponent();

        // The safety net under the pages' own error handling: log what nothing else caught.
        UnhandledException += OnUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            ErrorReporter.Log(e.ExceptionObject as Exception, "Unhandled exception");
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            ErrorReporter.Log(e.Exception, "Unobserved task exception");
            e.SetObserved();
        };
    }

    /// <summary>
    /// An exception on the UI thread that no handler caught: logged, and marked handled so one failed action does not
    /// close the app with the user's work in it. Page actions report their own failures (ErrorReporter).
    /// </summary>
    // xaml-lint: allow codebehind - the app-wide exception hook; Application has no XAML or command surface for it
    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        ErrorReporter.Log(e.Exception, "Unhandled exception on the UI thread");
        e.Handled = true;
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    /// <summary>The services once the host has started; null before (and if startup failed).</summary>
    public static IServiceProvider? ServicesOrNull => (Current as App)?.Host?.Services;

    /// <summary>The app's window, for the pickers that need their owner window or its UI thread.</summary>
    public static Window MainAppWindow =>
        (Current as App)?.MainWindow ?? throw new InvalidOperationException("The app's window is not open yet.");

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
        // The business's currency (registered below). App.xaml's CurrencyFormatConverter is created by XAML, not by the
        // container, so it gets the same instance here.
        var currency = new Services.CurrencyService();
        if (Resources.TryGetValue("CurrencyFormatConverter", out var converter) && converter is CurrencyFormatConverter currencyConverter)
        {
            currencyConverter.Currency = currency;
        }

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
                        .Section<Data.Remote.SupabaseOptions>("Supabase")
                )
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<AppDatabase>();
                    services.AddSingleton<IQuoteRepository, QuoteRepository>();
                    services.AddSingleton<IClientRepository, ClientRepository>();
                    services.AddSingleton<IBusinessProfileRepository, BusinessProfileRepository>();
                    services.AddSingleton<ICatalogItemRepository, CatalogItemRepository>();
                    services.AddSingleton<QuoteNumberGenerator>();
                    services.AddSingleton<Services.QuoteCreationService>();
                    services.AddSingleton<Services.IPdfGenerator, Services.PdfGenerator>();
                    services.AddSingleton<Services.IShareService, Services.ShareService>();
                    services.AddSingleton<Services.IFeatureGateService, Services.FeatureGateService>();
                    services.AddSingleton<Services.IEmailLauncherService, Services.EmailLauncherService>();
                    services.AddSingleton<Services.IOnboardingService, Services.OnboardingService>();
                    services.AddSingleton<Services.IPhotoService, Services.PhotoService>();

                    services.AddSingleton<IStatusHistoryRepository, StatusHistoryRepository>();
                    services.AddSingleton<Services.QuoteStatusService>();
                    services.AddSingleton<Services.QuoteDelivery>();
                    services.AddSingleton<QuoteLineItemDialogs>();

                    services.AddSingleton<HttpClient>();

                    // Auth, sync, and cloud services. Local-first: they stay idle and hidden unless
                    // appsettings.json (or QUOTECRAFT_SUPABASE_URL / _ANON_KEY) names a Supabase project.
                    services.AddSingleton<Data.Remote.SupabaseConfig>();
                    services.AddSingleton<Services.IAuthService, Services.AuthService>();
                    services.AddSingleton<Data.Remote.SupabaseClient>();
                    services.AddSingleton<Services.ISyncService, Services.SyncService>();
                    services.AddSingleton<Services.ISubscriptionService, Services.SubscriptionService>();
                    services.AddSingleton<Services.INotificationService, Services.NotificationService>();
                    services.AddSingleton<Helpers.ConnectivityHelper>();
                    services.AddSingleton<Services.QuoteExpiryService>();
                    services.AddSingleton<Services.ThemeChoiceNotifier>();
                    services.AddSingleton<Services.QuoteBoard>();
                    services.AddSingleton(currency);
                    services.AddSingleton<Services.LogoStore>();
                    // Settings' account and theme parts, made in the page's scope with it (SettingsViewModel.cs)
                    services.AddTransient<AccountSettingsModel>();
                    services.AddTransient<ThemeSettingsModel>();
                    // The open quote's view, created by the Quotes page's detail region (route ./QuoteDetail)
                    services.AddTransient<QuoteDetailView>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
                // "!" routes to a Page open in this drawer instead of a full-window flyout. After UseNavigation, which
                // registers its own Flyout: the last registration wins.
                .ConfigureServices(services => services.AddTransient<Flyout, DrawerFlyout>())
                // Light, Dark or the system's theme (the default); the choice is saved and restored at launch.
                .UseThemeSwitching()
            );
        MainWindow = builder.Window;

#if DEBUG && !NO_UNO_STUDIO
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        // The database, photos and flag files are read as soon as the shell loads; on WebAssembly the browser file
        // system has to be restored first.
        await AppStorage.EnsureReadyAsync();

        // Debug: sample data and faults for the runtime checks (App.Verification.cs); nothing in Release.
        await PrepareVerificationDataAsync();

        Host = await builder.NavigateAsync<Shell>();

        // Overdue quotes expire, and reminders go out 3 days before (PRD F17), while the app runs.
        Host.Services.GetRequiredService<Services.QuoteExpiryService>().Start();

        await ShowOnboardingOnFirstRunAsync();
        await ApplyVerificationViewAsync();
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "QuoteCraft could not start");
            ShowStartupError(ex);
        }
    }

    /// <summary>
    /// Startup failed (the database could not open, say): say so in the window instead of leaving it blank.
    /// </summary>
    private void ShowStartupError(Exception ex)
    {
        MainWindow ??= new Window();
        MainWindow.Content = new StartupErrorView { DataContext = ex.Message };
        MainWindow.Activate();
    }

    /// <summary>
    /// On first run, onboarding replaces Main; it navigates to Main when it is done. This runs once the first
    /// navigation has finished: a check in ShellModel's constructor raced the default route and never showed, and
    /// one started while Main was still loading left the WebAssembly splash screen up for good.
    /// </summary>
    private async Task ShowOnboardingOnFirstRunAsync()
    {
        if (Host?.Services is not { } services)
        {
            return;
        }
        if (await services.GetRequiredService<Services.IOnboardingService>().IsOnboardingCompleteAsync())
        {
            return;
        }

        if (MainWindow?.Content is IContentControlProvider { ContentControl: { } root } && root.Navigator() is { } navigator)
        {
            await navigator.NavigateRouteAsync(this, "Onboarding", qualifier: Qualifiers.ClearBackStack);
        }
        else
        {
            services.GetRequiredService<ILogger<App>>().LogError("No navigator to open onboarding");
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
            new ViewMap<NotificationsPage, NotificationsModel>(),
            // Dialogs, opened from models with the "!" qualifier (Helpers/DialogNavigation)
            new DataViewMap<ConfirmDialog, ConfirmModel, ConfirmRequest>(),
            new ResultDataViewMap<NewQuoteDialog, NewQuoteModel, QuoteEntity>(),
            new ResultDataViewMap<SendQuoteDialog, SendQuoteModel, SendQuoteChoice>(Data: new DataMap<QuoteSendDetails>()),
            new ResultDataViewMap<LineItemEditorDialog, LineItemEditorModel, LineItemEditResult>(Data: new DataMap<LineItemEditRequest>()),
            new DataViewMap<CatalogBrowserDialog, CatalogBrowserModel, CatalogBrowserRequest>(),
            new ResultDataViewMap<ClientEditorDialog, ClientEditorModel, ClientEntity>(Data: new DataMap<ClientEditRequest>()),
            new ResultDataViewMap<CatalogItemEditorDialog, CatalogItemEditorModel, CatalogItemEditResult>(Data: new DataMap<CatalogItemEditRequest>()),
            new DataViewMap<PhotoViewerDialog, PhotoViewerModel, PhotoViewRequest>(),
            new DataViewMap<QuoteEditorPage, QuoteEditorModel, QuoteEntity>(),
            new DataViewMap<QuoteDetailView, QuoteDetailModel, QuoteEntity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new ("Onboarding", View: views.FindByViewModel<OnboardingModel>()),
                    new ("Auth", View: views.FindByViewModel<AuthModel>()),
                    new ("Notifications", View: views.FindByViewModel<NotificationsModel>()),
                    new ("Confirm", View: views.FindByViewModel<ConfirmModel>()),
                    new ("NewQuote", View: views.FindByViewModel<NewQuoteModel>()),
                    new ("SendQuote", View: views.FindByViewModel<SendQuoteModel>()),
                    new ("LineItemEditor", View: views.FindByViewModel<LineItemEditorModel>()),
                    new ("CatalogBrowser", View: views.FindByViewModel<CatalogBrowserModel>()),
                    new ("ClientEditor", View: views.FindByViewModel<ClientEditorModel>()),
                    new ("CatalogItemEditor", View: views.FindByViewModel<CatalogItemEditorModel>()),
                    new ("PhotoViewer", View: views.FindByViewModel<PhotoViewerModel>()),
                    new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
                        Nested:
                        [
                            new ("Dashboard", View: views.FindByViewModel<DashboardModel>(), IsDefault: true,
                                Nested:
                                [
                                    // The open quote, in the Quotes page's detail region (DashboardModel.OpenQuote)
                                    new ("QuoteDetail", View: views.FindByViewModel<QuoteDetailModel>()),
                                ]),
                            new ("Clients", View: views.FindByViewModel<ClientsModel>()),
                            new ("Catalog", View: views.FindByViewModel<CatalogModel>()),
                            new ("Settings", View: views.FindByViewModel<SettingsModel>()),
                            new ("QuoteEditor", View: views.FindByViewModel<QuoteEditorModel>()),
                        ]),
                ]
            )
        );
    }
}
