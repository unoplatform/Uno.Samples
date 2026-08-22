using System.Diagnostics.CodeAnalysis;
using Authentication.WebExtensionsDemo.Authentication;
using Uno.Resizetizer;

namespace Authentication.WebExtensionsDemo;

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
                // The Web provider drives the platform's browser surface and stores whatever the
                // callbacks hand back; the OAuth mechanics (PKCE, code exchange, refresh) live in
                // DuendeOAuthClient, which the typed callbacks receive. The public Duende demo
                // server needs no registration; test user bob / bob.
                .UseAuthentication(auth =>
                    auth.AddWeb<DuendeOAuthClient>(web => web
                        // Build the authorization request at sign-in time: the redirect URI comes
                        // from the platform's WebAuthenticationBroker (custom scheme on mobile,
                        // origin on WebAssembly, the loopback listener Uno.Extensions registers
                        // on Skia Desktop), and each request carries a fresh PKCE challenge.
                        .PrepareLoginStartUri(async (oauth, services, cache, credentials, loginStartUri, ct) =>
                            oauth.BuildAuthorizeUri())
                        .PrepareLoginCallbackUri(async (oauth, services, cache, credentials, loginCallbackUri, ct) =>
                            oauth.CallbackUri)
                        // The redirect carries an authorization code, not tokens: exchange it.
                        .PostLogin(async (oauth, services, cache, credentials, redirectUri, tokens, ct) =>
                            await oauth.ExchangeCodeAsync(redirectUri, ct))
                        // Silent path: redeem the stored refresh token.
                        .Refresh(async (oauth, services, cache, tokens, ct) =>
                            await oauth.RefreshTokensAsync(tokens, ct))
                        // End the identity provider session too, not just the local cache.
                        .PrepareLogoutStartUri(async (oauth, services, cache, tokens, logoutStartUri, ct) =>
                            oauth.BuildEndSessionUri(tokens))
                        .PrepareLogoutCallbackUri(async (oauth, services, cache, tokens, logoutCallbackUri, ct) =>
                            oauth.CallbackUri))
                )
                .ConfigureServices((context, services) =>
                {
                    services
                        // The OAuth mechanics AddWeb's typed callbacks receive.
                        .AddSingleton<DuendeOAuthClient>()
                        // Narrates every IAuthenticationService call into a flow log shown in the
                        // UI. A singleton so the log and sign-in state survive the app's lifetime.
                        .AddSingleton<WebFlowService>();
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
            var flow = services.GetRequiredService<WebFlowService>();
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
