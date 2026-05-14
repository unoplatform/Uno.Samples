using System.Diagnostics.CodeAnalysis;
using Uno.Resizetizer;

namespace TaskFlow;

public partial class App : Application
{
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
			.UseToolkitNavigation()
			.Configure(host => host
#if DEBUG
				.UseEnvironment(Environments.Development)
#endif
				.UseLogging(configure: (context, logBuilder) =>
				{
					logBuilder
						.SetMinimumLevel(
							context.HostingEnvironment.IsDevelopment() ?
								LogLevel.Information :
								LogLevel.Warning)
						.CoreLogLevel(LogLevel.Warning);
				}, enableUnoLogging: true)
				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()
						.Section<AppConfig>()
				)
				.UseLocalization()
				.UseThemeSwitching()
				.ConfigureServices((context, services) =>
				{
					services.AddSingleton<ITaskService, TaskService>();
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
			new ViewMap<MainPage>(),
			new ViewMap<DashboardPage>(ViewModel: typeof(DashboardViewModel)),
			new ViewMap<TasksPage>(ViewModel: typeof(TasksViewModel)),
			new ViewMap<CategoriesPage>(ViewModel: typeof(CategoriesViewModel)),
			new ViewMap<CompletedPage>(ViewModel: typeof(CompletedViewModel))
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellModel>(),
				Nested:
				[
					new RouteMap("Main", View: views.FindByView<MainPage>(),
						IsDefault: true,
						Nested:
						[
							new RouteMap("Dashboard", View: views.FindByView<DashboardPage>(), IsDefault: true),
							new RouteMap("Tasks", View: views.FindByView<TasksPage>()),
							new RouteMap("Categories", View: views.FindByView<CategoriesPage>()),
							new RouteMap("Completed", View: views.FindByView<CompletedPage>()),
						]
					),
				]
			)
		);
	}
}
