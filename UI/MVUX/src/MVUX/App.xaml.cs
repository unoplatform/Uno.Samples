using CommunityToolkit.Mvvm.Messaging;
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

				}, enableUnoLogging: true)
				.UseSerilog(consoleLoggingEnabled: true, fileLoggingEnabled: true)
				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()
						.Section<AppConfig>()
				)
				// Enable localization (see appsettings.json for supported languages)
				.UseLocalization()
				.ConfigureServices((context, services) =>
				{
					// TODO: Register your services
					services.AddSingleton<IPaginationPeopleService, PaginationPeopleService>();
					services.AddSingleton<IPeopleService, PeopleService>();
					services.AddSingleton<IMessenger, WeakReferenceMessenger>();
				})
				.UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
			);
		MainWindow = builder.Window;

#if DEBUG
		MainWindow.UseStudio();
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
			new ViewMap<UpdateStatePage, UpdateStateModel>(),
			new ViewMap<SelectionPage, SelectionModel>(),
			new ViewMap<PaginationPage, PaginationModel>(),
			new ViewMap<FeedPage, FeedModel>(),
			new ViewMap<FeedViewCommandPage, FeedViewCommandModel>(),
			new ViewMap<FeedViewPage, FeedViewModel>(),
			new ViewMap<MessagingPage, MessagingModel>()
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellModel>(),
				Nested:
				[
					new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault: true,
						Nested:
						[
							new ("ListFeed", View: views.FindByViewModel<ListFeedModel>(), IsDefault: true),
							new ("Feed", View: views.FindByViewModel<FeedModel>()),
							new ("UpdateState", View: views.FindByViewModel<UpdateStateModel>()),
							new ("Selection", View: views.FindByViewModel<SelectionModel>()),
							new ("Pagination", View: views.FindByViewModel<PaginationModel>()),
							new ("FeedViewCommand", View: views.FindByViewModel<FeedViewCommandModel>()),
							new ("FeedView", View: views.FindByViewModel<FeedViewModel>()),
							new ("Messenger", View: views.FindByViewModel<MessagingModel>())
						]
					)
				]
			)
		);
	}
}
