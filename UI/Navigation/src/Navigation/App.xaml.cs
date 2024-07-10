using Uno.Resizetizer;

namespace Navigation;
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
				.ConfigureServices((context, services) =>
				{
					// TODO: Register your services
					//services.AddSingleton<IMyService, MyService>();
				})
				.UseNavigation(RegisterRoutes)
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
			new ViewMap(ViewModel: typeof(ShellViewModel)),
			new ViewMap<MainPage, MainViewModel>(),
			new ViewMap<PageNavigation>(),
			new ViewMap<SamplePage>(),
			new ViewMap<CControlNavigationPage>(),
			new DataViewMap<CControlRightPage, CControlRightViewModel, Entity>(),
			new ViewMap<TabBarNavigationPage>(),
			new ViewMap<TabBarItem3>()
		);

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
				Nested:
				[
					new ("Main", View: views.FindByViewModel<MainViewModel>(),
						Nested:
						[
							#region Page Navigation
							new ("PageNavigation", View: views.FindByView<PageNavigation>(), IsDefault: true),
							new ("Sample", View: views.FindByView<SamplePage>(), DependsOn: "PageNavigation"),
							#endregion

							#region ContentControl Navigation
							new ("CControlNavigation", View: views.FindByView<CControlNavigationPage>()),
							new ("CControlRight", View: views.FindByView<CControlRightPage>(), DependsOn: "CControlNavigation"),
							#endregion
						
							#region TabBar Navigation
							new ("TabBarNavigation", View: views.FindByView<TabBarNavigationPage>(),
								Nested:
								[
									new ("TBOne"),
									new ("TBTwo", IsDefault: true),
									new ("TBThree", View: views.FindByView<TabBarItem3>())
								]
							)
							#endregion
						]
					)
				]
			)
		);
	}
}
