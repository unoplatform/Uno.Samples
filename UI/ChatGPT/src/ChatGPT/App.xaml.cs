using Uno.Resizetizer;

namespace ChatGPT;

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

	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		// Load WinUI Resources
		Resources.Build(r => r.Merged(
			new XamlControlsResources()));

		// Load Uno.UI.Toolkit and Material Resources
		Resources.Build(r => r.Merged(
			new  MaterialToolkitTheme(
					new Styles.ColorPaletteOverride(),
					new Styles.MaterialFontsOverride())));
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
				.ConfigureServices((context, services) =>
				{
					var section = context.Configuration.GetSection(nameof(AppConfig));
					var apiKey = section[nameof(AppConfig.ApiKey)];
					var useMockService = apiKey is null or { Length: 0 };

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
		MainWindow.SetWindowIcon();

		Host = builder.Build();

		if (MainWindow.Content is null)
		{
			MainWindow.Content ??= new MainPage { DataContext = Host.Services.GetRequiredService<BindableMainModel>() };
		}

		MainWindow.Activate();
	}
}
