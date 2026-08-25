using System.Diagnostics.CodeAnalysis;
using Authentication.MsalExtensionsDemo.Authentication;
using Authentication.MsalExtensionsDemo.Common;
using Uno.Resizetizer;

namespace Authentication.MsalExtensionsDemo;

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
                .UseAuthentication(auth =>
                     auth.AddMsal(window, name: "MsalAuthentication")
                )
                .ConfigureServices((context, services) =>
                {
                    // Hides identifiers on screen while a demo is being recorded. A singleton, so
                    // the switch in the header covers every section and the flow log at once.
                    services.AddSingleton<SecretRedactor>();

                    // Narrates every IAuthenticationService call into a flow log shown in the UI.
                    // A singleton so the log and sign-in state survive switching sections.
                    services.AddSingleton<MsalFlowService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
            
        MainWindow = builder.Window;


        MainWindow.SetWindowIcon();

        async Task InitialNavigate(IServiceProvider services, INavigator navigator)
        {
            // Run (and narrate) the silent path a production app should use at startup: on
            // platforms where the provider persists the token cache, this signs back in with no
            // UI. Sign-in itself lives on the page, so navigation happens either way.
            var flow = services.GetRequiredService<MsalFlowService>();
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
