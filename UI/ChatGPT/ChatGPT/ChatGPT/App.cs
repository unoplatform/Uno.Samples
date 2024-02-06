using ChatGPT.Services;
using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;

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
				)
				.ConfigureServices(
					(context, services) =>
					{
						var section = context.Configuration.GetSection(nameof(AppConfig));

						//Get API Key from appsettings.json
						var apiKey = section[nameof(AppConfig.ApiKey)];
						var useMockService = apiKey is null or { Length: 0 };

						//If no API Key isn't provided a MockService with
						//hard-coded responses will be provided to simulate AI's responses
						if (useMockService)
						{
							services.AddSingleton<IChatService, MockChatService>();
						}
						else
						{
							services
								.AddSingleton<OpenAiOptions, ChatAiOptions>()
								.AddSingleton<IChatCompletionService, OpenAIService>()
								.AddSingleton<IChatService, ChatService>();
						}

						services.AddSingleton<BindableMainModel>();
					})
			);
		MainWindow = builder.Window;

#if DEBUG
		MainWindow.EnableHotReload();
#endif
		Host = builder.Build();

		if (MainWindow.Content is null)
		{
			MainWindow.Content ??= new MainPage { DataContext = Host.Services.GetRequiredService<BindableMainModel>() };
		}

		MainWindow.Activate();
	}
}
