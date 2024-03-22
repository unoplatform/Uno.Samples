using CommunityToolkit.Mvvm.Messaging;
using Commerce.Infrastructure;
using Uno.Extensions.Toolkit;

namespace Commerce
{
    public partial class App : Application
    {
        protected Window? MainWindow { get; private set; }
        public static IHost? Host { get; private set; }

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
                        logBuilder.SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                                    .AddFilter("Uno.Extensions", LogLevel.Trace);
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
                    // Register HttpClient
                    .UseHttp(
#if DEBUG
                        (context, services) =>
						// DelegatingHandler will be automatically injected into Refit Client
						services.AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
                            )
                    .UseThemeSwitching()
                    .ConfigureServices((context, services) =>
                    {
                        services
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

            Host = await builder.NavigateAsync<Shell>();

            //Host = await MainWindow.InitializeNavigationAsync(async () => builder.Build());
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
                new ViewMap<Shell, ShellViewModel>(),
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
}
