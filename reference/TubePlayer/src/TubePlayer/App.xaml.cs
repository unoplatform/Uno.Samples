using System.Text.Json;

using Uno.Resizetizer;

namespace TubePlayer;

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
        // Load WinUI Resources
        Resources.Build(r => r.Merged(
            new XamlControlsResources()));

        // Load Uno.UI.Toolkit and Material Resources
        Resources.Build(r => r.Merged(
            new MaterialToolkitTheme(
                    new Styles.ColorPaletteOverride(),
                    new Styles.MaterialFontsOverride())));
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
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
                .UseSerialization(services =>
                {
                    services.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                })
                .UseHttp(configure: (context, services) =>
                {
                    services.AddRefitClientWithEndpoint<IYoutubeEndpoint, YoutubeEndpointOptions>(
                        context,
                        configure: (clientBuilder, options) => clientBuilder
                            .ConfigureHttpClient(httpClient =>
                            {
                                httpClient.BaseAddress = new Uri(options!.Url!);
                                httpClient.DefaultRequestHeaders.Add("x-goog-api-key", options.ApiKey);
                            }))
                        .AddRefitClient<IYoutubePlayerEndpoint>(context);

                })
                .ConfigureServices((context, services) =>
                {
                    // Register your services
#if USE_MOCKS
                    services.AddSingleton<IYoutubeService, YoutubeServiceMock>();
#else
                    services.AddSingleton<IYoutubeService, YoutubeService>();
#endif
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
            new DataViewMap<VideoDetailsPage, VideoDetailsModel, YoutubeVideo>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested: new RouteMap[]
                {
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>()),
                    new RouteMap("VideoDetails", View: views.FindByViewModel<VideoDetailsModel>()),
                }
            )
        );
    }
}
