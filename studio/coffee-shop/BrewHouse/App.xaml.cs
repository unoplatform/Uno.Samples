using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace BrewHouse;

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
                .ConfigureServices((context, services) =>
                {
                    // One shared cart/orders state for the whole app: a DI singleton owning the
                    // cart + order IListStates, injected into every page-Model so a mutation made on
                    // one page propagates to every feed derived from it.
                    services.AddSingleton<ICartService, CartService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new ViewMap<HomePage, HomeModel>(),
            new ViewMap<MenuPage, MenuModel>(),
            new ViewMap<CartPage, CartModel>(),
            new ViewMap<OrdersPage, OrdersModel>(),
            new DataViewMap<ProductDetailPage, ProductDetailModel, ProductItem>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByView<MainPage>(),
                        IsDefault: true,
                        Nested:
                        [
                            new RouteMap("Home", View: views.FindByView<HomePage>(), IsDefault: true),
                            new RouteMap("Menu", View: views.FindByView<MenuPage>()),
                            new RouteMap("Cart", View: views.FindByView<CartPage>()),
                            new RouteMap("Orders", View: views.FindByView<OrdersPage>()),
                        ]
                    ),
                    // Sibling of Main (not a tab): shown full-screen in the shell's content area, so
                    // the product detail isn't overlaid by the TabBar / nav pane. Back returns to Main.
                    new RouteMap("ProductDetail", View: views.FindByView<ProductDetailPage>()),
                ]
            )
        );
    }
}
