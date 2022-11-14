
using Microsoft.Extensions.DependencyInjection;

namespace SimpleCalculator;

public sealed partial class App : Application
{
	private IHost? Host { get; set; }

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
				{
					// Configure log levels for different categories of logging
					logBuilder
							.SetMinimumLevel(
								context.HostingEnvironment.IsDevelopment() ?
									LogLevel.Debug :
									LogLevel.Warning);
					logBuilder.AddFilter("Uno.UI.RemoteControl", LogLevel.Debug);
					logBuilder.AddFilter("Uno", LogLevel.Warning);
					logBuilder.AddFilter("Windows", LogLevel.Warning);
					logBuilder.AddFilter("Microsoft", LogLevel.Warning);

#if !__WASM__
					logBuilder.AddConsole(); 
#endif
				})

				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()
						.Section<AppConfig>()
				)

				// Enable localization (see appsettings.json for supported languages)
				.UseLocalization()

				// Register Json serializers (ISerializer and ISerializer)
				.UseSerialization()

				// Register services for the application
				.ConfigureServices(services =>
				{
					services.AddScoped<IAppThemeService, AppThemeService>();
				})


				// Enable navigation, including registering views and viewmodels
				.UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)

				// Add navigation support for toolkit controls such as TabBar and NavigationView
				.UseToolkitNavigation()

				.Build(enableUnoLogging: true);

	}
	private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
	{
		views.Register(
			new ViewMap( ViewModel: typeof(ShellViewModel)),
            new ViewMap<MainPageMarkup, MainModel>()
            //new ViewMap<MainPage, MainModel>()
            );

		routes
			.Register(
				new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
						Nested: new RouteMap[]
						{
										new RouteMap("Main", View: views.FindByViewModel<MainModel>()),
						}));
	}
}
