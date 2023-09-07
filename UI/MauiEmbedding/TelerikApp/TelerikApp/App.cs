using TelerikApp.Business.Services;

namespace TelerikApp;

public class App : EmbeddingApplication
{
	protected Window? MainWindow { get; private set; }
	protected IHost? Host { get; private set; }

	protected async override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var builder = this.CreateBuilder(args)
			.UseMauiEmbedding<MauiControls.App>(maui => maui
				.UseMauiControls())
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
					//// RemoteControl and HotReload related
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
				.ConfigureServices((context, services) => {
                    services.AddSingleton<DataGenerator>()
                       .AddSingleton<IResourceService, AssemblyResourceService>()
                       .AddSingleton<ISerializationService, SerializationService>();
                })
				.UseNavigation(RegisterRoutes)
			);
		MainWindow = builder.Window;

		Host = await builder.NavigateAsync<Shell>();
	}

	private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
	{
		views.Register(
			new ViewMap(ViewModel: typeof(ShellViewModel)),
			new ViewMap<MainPage, MainViewModel>(),
             new ViewMap<MauiHost, AccordionSampleViewModel>(),
            new ViewMap<MauiHost, BadgeViewSampleViewModel>(),
            new ViewMap<MauiHost, CalendarSampleViewModel>(),
            new ViewMap<MauiHost, DataGridSampleViewModel>(),
            new ViewMap<MauiHost, AreaChartSampleViewModel>(),
            new ViewMap<MauiHost, FinancialChartSampleViewModel>(),
            new ViewMap<MauiHost, GuageSampleViewModel>(),
            new ViewMap<MauiHost, PdfViewerSampleViewModel>()
        );

		routes.Register(
			new RouteMap("", View: views.FindByViewModel<ShellViewModel>(),
				Nested: new RouteMap[]
				{
					new RouteMap("Main", View: views.FindByViewModel<MainViewModel>(), Nested: new RouteMap[]
                    {
                        new RouteMap("Accordion", View: views.FindByViewModel<AccordionSampleViewModel>(), IsDefault: true),
                        new RouteMap("AreaChart", View: views.FindByViewModel<AreaChartSampleViewModel>()),
                        new RouteMap("BadgeView", View: views.FindByViewModel<BadgeViewSampleViewModel>()),
                        new RouteMap("Calendar", View: views.FindByViewModel<CalendarSampleViewModel>()),
                        new RouteMap("DataGrid", View: views.FindByViewModel<DataGridSampleViewModel>()),
                        new RouteMap("FinancialChart", View: views.FindByViewModel<FinancialChartSampleViewModel>()),
                        new RouteMap("Guage", View: views.FindByViewModel<GuageSampleViewModel>()),
                        new RouteMap("PdfViewer", View: views.FindByViewModel<PdfViewerSampleViewModel>()),
                    }),
                }
			)
		);
	}
}
