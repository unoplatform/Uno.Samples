
using CommunityToolkit.Mvvm.Messaging;
using Uno.Extensions.Http;

namespace Commerce;

public sealed partial class App : Application
{
	private readonly IHost _host = BuildAppHost();

	private static IHost BuildAppHost()
	{
		return UnoHost
				.CreateDefaultBuilder()
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif


				// Add platform specific log providers
				.UseLogging(configure: (context, logBuilder) =>
							// Configure log levels for different categories of logging
							logBuilder
									.SetMinimumLevel(
										context.HostingEnvironment.IsDevelopment() ?
											LogLevel.Trace :
											LogLevel.Warning)
									.AddFilter("Uno.Extensions", LogLevel.Trace))

				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()

						.Section<AppInfo>()

						.Section<Credentials>()
						.Section<CommerceApp>()
				)


				// Register Json serializers (ISerializer and ISerializer)
				.UseSerialization()

				// Register services for the application
				.ConfigureServices(services =>
				{
					services
						.AddScoped<IAppTheme, AppTheme>()
						.AddSingleton<IMessenger, WeakReferenceMessenger>()

						.AddSingleton<IReviewsEndpoint, ReviewsEndpoint>()

						.AddSingleton<ICartService, CartService>()
						.AddSingleton<IDealService, DealService>()
						.AddSingleton<IProductService, ProductService>()
						.AddSingleton<IProfileService, ProfileService>();
				})

                //.UseAuthentication(auth =>
                //    auth.AddCustom<ICustomAuthenticationDummyJsonEndpoint>(custom =>
                //        custom
                //            .Login(async (authService, services, dispatcher, cache, credentials, cancellationToken) =>
                //            {
                //                if (credentials is null)
                //                {
                //                    return default;
                //                }

                //                var name = credentials.FirstOrDefault(x => x.Key == nameof(CustomAuthenticationCredentials.Username)).Value;
                //                var password = credentials.FirstOrDefault(x => x.Key == nameof(CustomAuthenticationCredentials.Password)).Value;
                //                var creds = new CustomAuthenticationCredentials { Username = name, Password = password };
                //                var authResponse = await authService.Login(creds, cancellationToken);
                //                if (authResponse?.Token is not null)
                //                {
                //                    credentials[TokenCacheExtensions.AccessTokenKey] = authResponse.Token;

                //                    var w = new Widget { Name = "Bob" };
                //                    var serializer = services.GetRequiredService<ISerializer<Widget>>();
                //                    credentials.Set(serializer, nameof(Widget), w);

                //                    return credentials;
                //                }
                //                return default;
                //            })
                //            .Refresh(async (authService, services, cache, tokenDictionary, cancellationToken) =>
                //            {
                //                var serializer = services.GetRequiredService<ISerializer<Widget>>();
                //                var widget = tokenDictionary.Get(serializer, nameof(Widget));
                //                if (widget is null)
                //                {
                //                    return default;
                //                }

                //                var creds = new CustomAuthenticationCredentials
                //                {
                //                    Username = tokenDictionary.TryGetValue(nameof(CustomAuthenticationCredentials.Username), out var name) ? name : string.Empty,
                //                    Password = tokenDictionary.TryGetValue(nameof(CustomAuthenticationCredentials.Password), out var password) ? password : string.Empty
                //                };

                //                try
                //                {
                //                    var authResponse = await authService.Login(creds, cancellationToken);
                //                    if (authResponse?.Token is not null)
                //                    {
                //                        tokenDictionary[TokenCacheExtensions.AccessTokenKey] = authResponse.Token;
                //                        tokenDictionary.Set<Widget>(serializer, nameof(Widget), widget);
                //                        return tokenDictionary;
                //                    }
                //                }
                //                catch
                //                {
                //                    // Ignore and return null
                //                }
                //                return default;
                //            }))
                //)

                //.UseAuthenticationFlow(builder =>
                //        builder
                //            .OnLoginRequiredNavigateViewModel<CustomAuthenticationLoginViewModel>(this)
                //            .OnLoginCompletedNavigateViewModel<CustomAuthenticationHomeViewModel>(this)
                //            .OnLogoutNavigateViewModel<CustomAuthenticationLoginViewModel>(this)
                //        )

				.UseHttp((context, services) =>
                {
                    services
                            .AddRefitClient<IProductEndpoint>(context, "DummyJsonEndpoint")
                            .AddRefitClient<IAuthenticationEndpoint>(context, "DummyJsonEndpoint");
                })


                // Enable navigation, including registering views and viewmodels
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings,
                    RegisterRoutes,
					configure: cfg => cfg with { AddressBarUpdateEnabled = true })
				
				// Add navigation support for toolkit controls such as TabBar and NavigationView
				.UseToolkitNavigation()

				// Add localization support
				.UseLocalization()

				.Build(enableUnoLogging: true);


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

		;
	}
}
