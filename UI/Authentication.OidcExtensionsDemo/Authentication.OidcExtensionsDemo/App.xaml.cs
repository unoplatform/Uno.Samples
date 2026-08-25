using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace Authentication.OidcExtensionsDemo;

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

    /// <summary>
    /// The host's service provider, available from the first navigation on. Public so the pages'
    /// code-behind can resolve the shared <see cref="AuthFlow.AuthFlowService"/> for the
    /// flow log and token summaries (App.Host is only assigned after the first navigation has
    /// already constructed a page).
    /// </summary>
    public IServiceProvider? Services { get; private set; }

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
                .UseHttp((context, services) =>
                {
#if DEBUG
                    // DelegatingHandler will be automatically injected
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif

                })
                .UseAuthentication(auth =>
                    // The OidcAuthentication section of appsettings.json points at the public
                    // Duende demo server (test user: bob / bob). The redirect URI is not in
                    // configuration: the platform's WebAuthenticationBroker supplies it - the
                    // oidc-ext-demo:// scheme on Android/iOS, the app's origin on WebAssembly, and
                    // the loopback listener Uno.Extensions registers on Skia Desktop. The demo
                    // server accepts arbitrary redirect URIs, so nothing needs registering.
                    auth.AddOidc(
                        oidc => oidc.AutoRedirectUriFromWebAuthenticationBroker(true),
                        name: "OidcAuthentication")
                )
                .ConfigureServices((context, services) =>
                {
                    services
                        // AuthFlowService calls the demo API with IHttpClientFactory; UseHttp only
                        // registers the factory when named/typed clients are added, so register
                        // it explicitly.
                        .AddHttpClient()
                        // Narrates every authentication call into the flow log shown on both
                        // pages; a singleton so the log survives Login/Main navigation.
                        .AddSingleton<AuthFlow.AuthFlowService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

        MainWindow.SetWindowIcon();

        async Task InitialNavigate(IServiceProvider services, INavigator navigator)
        {
            Services = services;

            // Run (and narrate) the silent path a production app should use at startup.
            var flow = services.GetRequiredService<AuthFlow.AuthFlowService>();
            var authenticated = await flow.StartupAsync();
            if (authenticated)
            {
                await navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
            }
            else
            {
                await navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
            }
        }

        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(builder.Build()),
            initialNavigate: InitialNavigate);
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<LoginPage, LoginModel>(),
            new ViewMap<MainPage, MainModel>()
        );

        routes.Register(
            new RouteMap("Login", View: views.FindByViewModel<LoginModel>()),
            new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true)
        );
    }
}
