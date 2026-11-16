using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Uno.Resizetizer;

namespace Http.KiotaExtensionsDemo;

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
    [SuppressMessage("Trimming", "IL3050", Justification = "The Tasks API endpoint options are passed explicitly, not bound.")]
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
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                .UseSerialization((context, services) => services.AddContentSerializer(context))
                .UseHttp((context, services) =>
                {
#if DEBUG
                    // DelegatingHandler will be automatically injected
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                    services.AddTasksApi(context, ApiUrl(context.Configuration));
                })
                .ConfigureServices((context, services) => services.AddTransient<FilePicker>())
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio(showHotReloadIndicator: false, launchHotDesignOnStart: false);
#endif
        MainWindow.SetWindowIcon();

        Host = await MainWindow.InitializeNavigationAsync(
            () => Task.FromResult(builder.Build()),
            initialRoute: "Main"
        );
    }

    /// <summary>
    /// The Tasks API's address for this head. <c>ApiClient:Url</c> is set in both appsettings files,
    /// so Debug (Development) and Release (Production) builds have one; without it the generated
    /// client would silently call the contract's placeholder server, https://tasks.example.com.
    /// WebAssembly is served by the API's own server, so it calls its own origin; the Android
    /// emulator reaches the development machine at 10.0.2.2, not localhost.
    /// </summary>
    private static string? ApiUrl(IConfiguration configuration)
    {
        var url = configuration["ApiClient:Url"];
        if (OperatingSystem.IsBrowser())
        {
            return BrowserOrigin();
        }
        if (OperatingSystem.IsAndroid())
        {
            return url?.Replace("//localhost", "//10.0.2.2");
        }
        return url;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("browser")]
    private static string BrowserOrigin() =>
        System.Runtime.InteropServices.JavaScript.JSHost.GlobalThis
            .GetPropertyAsJSObject("location")!
            .GetPropertyAsString("origin")!;

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap<MainPage, MainModel>(),
            new DataViewMap<TaskDetailsPage, TaskDetailsModel, TaskItem>(),
            new DataViewMap<TaskEditPage, TaskEditModel, TaskEditRequest>()
        );

        routes.Register(
            new RouteMap("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true),
            new RouteMap("TaskDetails", View: views.FindByViewModel<TaskDetailsModel>()),
            new RouteMap("TaskEdit", View: views.FindByViewModel<TaskEditModel>())
        );
    }
}
