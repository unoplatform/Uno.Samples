using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace UnoCRM;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // Open ComboBox drop-downs below the control (with the list content beneath it)
        // instead of the WinUI default that overlays the selected item on the box.
        Uno.UI.FeatureConfiguration.ComboBox.DefaultDropDownPreferredPlacement =
            Uno.UI.Xaml.Controls.DropDownPlacement.Below;

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
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif
        MainWindow.SetWindowIcon();

        // Navigate to the Shell, which shows the extended splash screen while the host
        // starts and then reveals the navigated content (Main -> Dashboard) in its place.
        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<DashboardPage, DashboardModel>(),
            new ViewMap<PipelinePage, PipelineModel>(),
            new ViewMap<LeadsPage, LeadsModel>(),
            new ViewMap<ContactsPage, ContactsModel>(),
            // Deal detail: the tapped Deal is bound as the model's nav-data parameter.
            new DataViewMap<DealDetailPage, DealDetailModel, Deal>()
        );

        routes.Register(
            // The Shell hosts the extended splash screen and is the navigation root.
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    // MainPage is the navigation shell: it owns the NavigationView / TabBar
                    // chrome and a content region the pages below are injected into.
                    new RouteMap("Main", View: views.FindByView<MainPage>(),
                        IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Dashboard", View: views.FindByView<DashboardPage>(), IsDefault: true),
                            new RouteMap("Pipeline", View: views.FindByView<PipelinePage>()),
                            new RouteMap("Leads", View: views.FindByView<LeadsPage>()),
                            new RouteMap("Contacts", View: views.FindByView<ContactsPage>())
                        ]),
                    // Sibling of Main (not a tab): shown full-screen in the shell's content area so
                    // the detail isn't overlaid by the TabBar / nav pane. Back returns to the tab.
                    new RouteMap("DealDetail", View: views.FindByView<DealDetailPage>())
                ])
        );
    }

    /// <summary>
    /// Configures global Uno Platform logging. Invoked from each platform entry point.
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on the web or
        // desktop targets, you can use URL or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());

            // Log to the Visual Studio Debug console
            builder.AddConsole();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
