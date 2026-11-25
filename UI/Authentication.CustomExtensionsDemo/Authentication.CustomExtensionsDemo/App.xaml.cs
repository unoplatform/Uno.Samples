using System.Diagnostics.CodeAnalysis;
using Authentication.CustomExtensionsDemo.Authentication;
using Authentication.CustomExtensionsDemo.Common;
using Authentication.CustomExtensionsDemo.Client;
using Uno.Resizetizer;

namespace Authentication.CustomExtensionsDemo;

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
            .Configure(host => PlatformSupport.ConfigureHost(host
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
                        .CoreLogLevel(LogLevel.Warning)

                        // The authentication service and provider log under this category,
                        // which CoreLogLevel would otherwise hide below Warning.
                        .AddFilter("Uno.Extensions.Authentication", LogLevel.Information);

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
                    // The Kiota client generated from the demo server's OpenAPI document
                    // (Specs/, Content/Client/). Registering it here, inside UseHttp, is what makes
                    // the authentication handler part of its pipeline: the access token is attached
                    // to every call and a 401 triggers a refresh. Its base address comes from the
                    // section named by `name`: without it, the section would be the client's full
                    // type name.
                    services.AddKiotaClient<DemoServerClient>(context, name: CustomFlowService.ClientName);
                })
                // The Custom provider: AddCustom<DemoServerClient> hands the Kiota client to the
                // Login, Refresh and Logout callbacks in CustomFlowService.Configure. It reads no
                // configuration section; the name only matters with several providers.
                .UseAuthentication(auth =>
                    auth.AddCustom<DemoServerClient>(CustomFlowService.Configure, name: CustomFlowService.ProviderName)
                )
                .ConfigureServices((context, services) =>
                {
                    // Hides identifiers on screen while a demo is being recorded. A singleton, so
                    // the switch in the header covers every section and the flow log at once.
                    services.AddSingleton<SecretRedactor>();

                    // Narrates every IAuthenticationService call into a flow log shown in the UI,
                    // and holds the app's one LoggedOut subscription. A singleton so the log and
                    // sign-in state survive switching sections.
                    services.AddSingleton<CustomFlowService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            ));

        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio(showHotReloadIndicator: false, launchHotDesignOnStart: false);
#endif
        MainWindow.SetWindowIcon();

        async Task InitialNavigate(IServiceProvider services, INavigator navigator)
        {
            // Run (and narrate) the silent path a production app should use at startup: with a
            // refresh token stored from a previous run, this signs back in with no UI. Sign-in
            // itself lives on the page, so navigation happens either way.
            var flow = services.GetRequiredService<CustomFlowService>();
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
            new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true)
        );
    }
}
