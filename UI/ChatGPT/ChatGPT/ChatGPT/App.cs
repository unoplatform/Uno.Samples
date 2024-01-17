using ChatGPT.Services;

namespace ChatGPT;

public class App : Application
{
	protected Window? MainWindow { get; private set; }
	protected IHost? Host { get; private set; }

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var builder = this.CreateBuilder(args)
			.Configure(host => host
#if DEBUG
				// Switch to Development environment when running in DEBUG
				.UseEnvironment(Environments.Development)
#endif
				.UseConfiguration(configure: configBuilder =>
					configBuilder
						.EmbeddedSource<App>()
						.Section<AppConfig>()
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
					// TODO: Register your services
					services.AddSingleton<IChatService, ChatService>();
					services.AddSingleton<BindableMainModel>();
				})
			);
		MainWindow = builder.Window;

#if DEBUG
		MainWindow.EnableHotReload();
#endif

		Host = builder.Build();

		// Do not repeat app initialization when the Window already has content,
		// just ensure that the window is active
		if (MainWindow.Content is null)
		{
			MainWindow.Content = new MainPage(Host.Services);
		}

		// Ensure the current window is active
		MainWindow.Activate();
	}
}
