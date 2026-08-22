using System.Diagnostics.CodeAnalysis;
using Authentication.OidcExtensionsDemo.Authentication;
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

    [SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "Uno.Extensions APIs are used in a way that is safe for trimming in this template context.")]
    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure((host, window) => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
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
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .UseHttp((context, services) => {
#if DEBUG
                // DelegatingHandler will be automatically injected
                services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                })
                .UseAuthentication(auth =>
                    auth.AddOidc(oidc => oidc
                        // The public Duende demo server: no registration needed, test user bob / bob.
                        .Authority(OidcFlowService.Authority)
                        .ClientId(OidcFlowService.ClientId)
                        .ClientSecret("secret")
                        .Scope(OidcFlowService.Scope)
                        // Let the platform's WebAuthenticationBroker supply the redirect URI:
                        // custom scheme on Android/iOS, the app's origin on WebAssembly, and the
                        // loopback listener Uno.Extensions registers on Skia Desktop. The demo
                        // server accepts arbitrary redirect URIs, so nothing needs registering.
                        .AutoRedirectUriFromWebAuthenticationBroker(true)
                        .ConfigureOidcClientOptions(options =>
                        {
                            // Duende's OidcClient validates id_token signatures only when an
                            // IIdentityTokenValidator is supplied (a separate package); without
                            // one, this must be opted out or sign-in throws. The code flow's
                            // token endpoint response comes over TLS from the authority, which
                            // is what protects it here.
                            options.Policy.RequireIdentityTokenSignature = false;
                        }))
                )
                .ConfigureServices((context, services) =>
                {
                    services
                        // The flow service injects IHttpClientFactory for its raw API call;
                        // UseHttp only registers the factory when named/typed clients are added,
                        // so register it explicitly.
                        .AddHttpClient();

                    // Narrates every IAuthenticationService call into a flow log shown in the UI.
                    // A singleton so the log and sign-in state survive the app's lifetime.
                    services.AddSingleton<OidcFlowService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );

        MainWindow = builder.Window;

        MainWindow.SetWindowIcon();

        async Task InitialNavigate(IServiceProvider services, INavigator navigator)
        {
            // Run (and narrate) the silent path a production app should use at startup: sign back
            // in from the stored refresh token with no UI. Sign-in itself lives on the page, so
            // navigation happens either way.
            var flow = services.GetRequiredService<OidcFlowService>();
            await flow.StartupAsync();

            await navigator.NavigateViewModelAsync<MainModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(builder.Build()),
            initialNavigate: InitialNavigate);
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<MainPage, MainModel>()
        );

        routes.Register(
            new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault:true)
        );
    }
}
