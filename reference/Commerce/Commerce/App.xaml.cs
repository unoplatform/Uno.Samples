using Uno.Extensions.Toolkit;
using Uno.Resizetizer;
using Windows.Media.Protection.PlayReady;

namespace Commerce;

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
                        .Section<AppInfo>()
                        .Section<Credentials>()
                        .Section<CommerceApp>()
                )
                // Enable localization (see appsettings.json for supported languages)
                .UseLocalization()
                // Register Json serializers (ISerializer and ISerializer)
                .UseSerialization((context, services) => services
                    .AddContentSerializer(context))
                .UseHttp((context, services) => services
                    // Register HttpClient
#if DEBUG
                    // DelegatingHandler will be automatically injected into Refit Client
                    .AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                        )
                .ConfigureServices((context, services) =>
                {
                    services
                    //.AddScoped<IAppTheme, AppTheme>()
                        .AddSingleton<IMessenger, WeakReferenceMessenger>()

                        .AddSingleton<IProductEndpoint, ProductEndpoint>()

                        .AddSingleton<ICartService, CartService>()
                        .AddSingleton<IDealService, DealService>()
                        .AddSingleton<IProductService, ProductService>()
                        .AddSingleton<IProfileService, ProfileService>();
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.EnableHotReload();
#endif
        MainWindow.SetWindowIcon();

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        var forgotPasswordDialog = new MessageDialogViewMap(
                        Content: "Click OK, or Cancel",
                        Title: "Forgot your password!",
                        DelayUserInput: true,
                        DefaultButtonIndex: 1,
                        Buttons: new DialogAction[]
                        {
                            new(Label: "Yeh!",Id:"Y"),
                            new(Label: "Nah", Id:"N")
                        }
                    );

        views.Register(
            new ViewMap(ViewModel: typeof(ShellViewModel)),
            new ViewMap<LoginPage, LoginViewModel>(ResultData: typeof(Credentials)),
            new ViewMap<HomePage>(Data: new DataMap<Credentials>()),
            new ViewMap<ProductsPage, ProductsViewModel>(),
            new ViewMap<ProductDetailsPage, ProductDetailsViewModel>(Data: new DataMap<Product>(
                                                                                    ToQuery: product => new Dictionary<string, string> { { nameof(Product.ProductId), product.ProductId.ToString() } },
                                                                                    FromQuery: async (sp, query) =>
                                                                                    {
                                                                                        if (query.TryGetValue(string.Empty, out var prod) && prod is Product p)
                                                                                        {
                                                                                            return p;
                                                                                        }
                                                                                        var id = int.Parse(query[nameof(Product.ProductId)] + string.Empty);
                                                                                        var ps = sp.GetRequiredService<IProductService>();
                                                                                        var products = await ps.GetAll(default);
                                                                                        return products.FirstOrDefault(p => p.ProductId == id);
                                                                                    })),
            new ViewMap<FilterPage, FiltersViewModel>(Data: new DataMap<Filters>()),
            new ViewMap<DealsPage, DealsViewModel>(),
            new ViewMap<ProfilePage, ProfileViewModel>(),
            new ViewMap<CartPage, CartViewModel>(),
            new ViewMap<ProductDetailsPage, CartProductDetailsViewModel>(Data: new DataMap<CartItem>(
                                                                                    ToQuery: cartItem => new Dictionary<string, string> {
                                                                                        { nameof(Product.ProductId), cartItem.Product.ProductId.ToString() },
                                                                                        { nameof(CartItem.Quantity),cartItem.Quantity.ToString() } },
                                                                                    FromQuery: async (sp, query) =>
                                                                                    {
                                                                                        var id = int.Parse(query[nameof(Product.ProductId)] + string.Empty);
                                                                                        var quantity = int.Parse(query[nameof(CartItem.Quantity)] + string.Empty);
                                                                                        var ps = sp.GetRequiredService<IProductService>();
                                                                                        var products = await ps.GetAll(default);
                                                                                        var p = products.FirstOrDefault(p => p.ProductId == id);
                                                                                        return new CartItem(p!, (uint)quantity);
                                                                                    })),
            new ViewMap<CheckoutPage>(),
            forgotPasswordDialog
            );

        routes
        .Register(
            new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
                    Nested: new RouteMap[]
                    {
                            new RouteMap("Login", View: views.FindByResultData<Credentials>(),
                                    Nested: new RouteMap[]
                                    {
                                        new ("Forgot", forgotPasswordDialog)
                                    }),
                            new RouteMap("Home", View: views.FindByData<Credentials>(),
                                    Nested: new RouteMap[]{
                                        new RouteMap("Deals",
                                                View: views.FindByViewModel<DealsViewModel>(),
                                                IsDefault: true,
                                                Nested: new RouteMap[]{
                                                    new RouteMap("DealsTab", IsDefault: true),
                                                    new RouteMap("FavoritesTab")
                                                }),
                                        new RouteMap("DealsProduct",
                                                View: views.FindByViewModel<ProductDetailsViewModel>(),
                                                DependsOn:"Deals"),
                                        new RouteMap("Products",
                                                View: views.FindByViewModel<ProductsViewModel>(),
                                                Nested: new  RouteMap[]{
                                                    new RouteMap("Filter",  View: views.FindByViewModel<FiltersViewModel>())
                                                }),
                                        new RouteMap("Product",
                                                View: views.FindByViewModel<ProductDetailsViewModel>(),
                                                DependsOn:"Products"),

                                        new RouteMap("Profile", View: views.FindByViewModel<ProfileViewModel>()),

                                        new RouteMap("Cart", View: views.FindByViewModel<CartViewModel>(),
                                                Nested: new []{
                                                    new RouteMap("CartDetails",View: views.FindByViewModel<CartProductDetailsViewModel>()),
                                                    new RouteMap("Checkout", View: views.FindByView<CheckoutPage>())
                                                })
                                        })
                    }));
    }
}
