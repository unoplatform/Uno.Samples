using MVUX.Presentation.FeedViewSample;
using Uno.Resizetizer;
using SelectionModel = MVUX.Presentation.SelectionSample.SelectionModel;

namespace MVUX;

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
				.UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()
						.Section<AppConfig>()
				)
				// Enable localization (see appsettings.json for supported languages)
				.UseLocalization()
				// Register Json serializers (ISerializer and ISerializer)
				.UseSerialization((context, services) => services
					.AddContentSerializer(context)
					.AddJsonTypeInfo(WeatherForecastContext.Default.IImmutableListWeatherForecast))
				.UseHttp((context, services) => services
					// Register HttpClient
#if DEBUG
					// DelegatingHandler will be automatically injected into Refit Client
					.AddTransient<DelegatingHandler, DebugHttpHandler>()
#endif
					.AddSingleton<IWeatherCache, WeatherCache>()
					.AddRefitClient<IApiClient>(context))
				.ConfigureServices((context, services) =>
				{
					// TODO: Register your services
					services.AddSingleton<IStateService, StateService>();
					services.AddSingleton<IPaginationPeopleService, PaginationPeopleService>();
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
		views.Register(
			new ViewMap(ViewModel: typeof(ShellModel)),
			new ViewMap<MainPage, MainModel>(),
			new ViewMap<ListFeedPage, ListFeedModel>(),
			new ViewMap<SignalPage, RefreshSignalModel>(),
			new ViewMap<StatePage, StateModel>(),
			new ViewMap<UpdateStatePage, UpdateStateModel>(),
			new ViewMap<SelectionPage, SelectionModel>(),
			new ViewMap<PaginationMainPage, PaginationPeopleModel>(),
			new ViewMap<FeedPage, FeedModel>(),
			new ViewMap<FeedViewCommandPage, FeedViewCommandModel>(),
			new ViewMap<FeedViewPage, FeedViewModel>()
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellModel>(),
				Nested:
				[
					new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault:true),
					new ("ListFeed", View: views.FindByViewModel<ListFeedModel>()),
					new ("RefreshSignal", View: views.FindByViewModel<RefreshSignalModel>()),
					new ("Feed", View: views.FindByViewModel<FeedModel>()),
					new ("State", View: views.FindByViewModel<StateModel>()),
					new ("UpdateState", View: views.FindByViewModel<UpdateStateModel>()),
					new ("Selection", View: views.FindByViewModel<SelectionModel>()),
					new ("Pagination", View: views.FindByViewModel<PaginationPeopleModel>()),
					new ("FeedViewCommand", View: views.FindByViewModel<FeedViewCommandModel>()),
					new ("FeedView", View: views.FindByViewModel<FeedViewModel>()),
				]
			)
		);
	}
}
