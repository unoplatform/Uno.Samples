using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace Authentication.WebSteve;

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
    /// The host's service provider. Assigned before navigation starts, so the pages' code-behind
    /// can resolve the shared <see cref="AuthFlow.AuthFlowService"/> for the flow log and token
    /// summaries - including on the very first page navigation builds (App.Host is only assigned
    /// once InitializeNavigationAsync returns, which is after that page already exists).
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
                // Nearly everything comes from the "Web" section of appsettings.json - the
                // login/logout page URIs, the callback URI, and the query keys the provider reads
                // the tokens from on the redirect. The provider opens LoginStartUri in the
                // platform browser surface and stores whatever tokens ride back on the redirect
                // to LoginCallbackUri.
                .UseAuthentication(auth =>
                    auth.AddWeb(web => web
                        // The one thing static configuration cannot carry: RP-initiated logout
                        // (OpenID Connect Session Management) identifies the session to end with
                        // id_token_hint, and that token only exists once someone has signed in.
                        // Without it Duende (like most providers) shows a "do you want to log
                        // out?" prompt and will not honour post_logout_redirect_uri, so the app
                        // never gets control back.
                        .PrepareLogoutStartUri((services, cache, tokens, logoutStartUri, ct) =>
                            ValueTask.FromResult(WithIdTokenHint(logoutStartUri, tokens))))
                )
                .ConfigureServices((context, services) =>
                {
                    services
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
            // The navigation scope, now that there is one - a narrower provider than the host's,
            // and the one the pages should resolve from once navigation is under way.
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

        // Build the host before navigation starts: Uno.Extensions navigates to the default route
        // ("Main") before it calls initialNavigate, so a page constructor runs before the callback
        // below could publish the provider. Pages resolve AuthFlowService in their constructor, so
        // Services has to be set by then - otherwise that first page fails to construct.
        var host = builder.Build();
        Services = host.Services;

        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(host),
            initialNavigate: InitialNavigate);
    }

    /// <summary>
    /// Appends <c>id_token_hint</c> to the end-session URI, taking the id_token out of the token
    /// cache. The Web section maps <c>AccessTokenKey</c> to <c>id_token</c>, so the value cached
    /// under the standard access-token key is the id_token this flow issues.
    /// </summary>
    private static string WithIdTokenHint(string? logoutStartUri, IDictionary<string, string>? tokens)
    {
        if (string.IsNullOrWhiteSpace(logoutStartUri))
        {
            return string.Empty;
        }

        var idToken = tokens?.TryGetValue(Uno.Extensions.Authentication.TokenCacheExtensions.AccessTokenKey, out var cached) == true
            ? cached
            : null;

        return string.IsNullOrWhiteSpace(idToken)
            ? logoutStartUri
            : $"{logoutStartUri}&id_token_hint={Uri.EscapeDataString(idToken)}";
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
