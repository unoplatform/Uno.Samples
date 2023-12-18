using System.Text.Json;

namespace TubePlayer;

// TODO: 01: Use EmbeddingApplication
public class App : EmbeddingApplication
{
    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()

             .UseMauiEmbedding<MauiControls.App>(maui => maui
                    .UseMauiControls())

            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseSerialization(services =>
                {
                    services.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                })
                .UseHttp(configure: (context, services) =>
                {
                    services
                    .AddRefitClientWithEndpoint<IYoutubeEndpoint, YoutubeEndpointOptions>(
                        context,
                        configure: (clientBuilder, options) => clientBuilder
                            .ConfigureHttpClient(httpClient =>
                            {
                                httpClient.BaseAddress = new Uri(options!.Url!);
                                httpClient.DefaultRequestHeaders.Add("x-goog-api-key", options.ApiKey);
                            }))
                    .AddRefitClient<IYoutubePlayerEndpoint>(context);
                })
                .UseConfiguration(configure: configBuilder =>
                    configBuilder
                        .EmbeddedSource<App>()
                        .Section<AppConfig>()
                )
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

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new DataViewMap<VideoDetailsPage, VideoDetailsModel, YoutubeVideo>(),
            new ViewMap<VideoAnalyticsPage, VideoAnalyticsModel>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested: new RouteMap[]
                {
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>()),
                    new RouteMap("VideoDetails", View: views.FindByViewModel<VideoDetailsModel>()),
                    new RouteMap("VideoAnalytics", View: views.FindByViewModel<VideoAnalyticsModel>()),
                }
            )
        );
    }
}
