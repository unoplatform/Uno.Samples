using Uno.Resizetizer;
using Windows.Security.Authentication.Web;

namespace Authentication.MsalExtensionsDemo;

public partial class App : Application
{
    public App()
    {
        this.InitializeComponent();
    }

    public static IHost? AppHost { get; private set; }

    protected Window? MainWindow { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            ConfigureWasmRedirect();

            var builder = this.CreateBuilder(args)
                .Configure((host, window) => host
                    .UseLogging(configure: (context, logBuilder) => logBuilder
                        .SetMinimumLevel(LogLevel.Information)
                        .AddFilter("Uno.Extensions.Authentication", LogLevel.Trace))
                    .UseConfiguration(configure: configBuilder => configBuilder
                        .EmbeddedSource<App>())
                    .UseAuthentication(auth => auth
                        .AddMsal(window, msal => msal
                            .Builder(ConfigurePlatformRedirect)))
                    .ConfigureServices(services => services
                        .AddSingleton<IDispatcher>(new Dispatcher(window))));

            MainWindow = builder.Window;
#if DEBUG
            MainWindow.UseStudio();
#endif

            AppHost = builder.Build();
            await AppHost.StartAsync();

            if (MainWindow.Content is null)
            {
                MainWindow.Content = new MainPage();
            }

            MainWindow.SetWindowIcon();
            MainWindow.Activate();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"App launch failed: {ex}");
        }
    }

    private static void ConfigureWasmRedirect()
    {
        if (!PlatformHelper.IsWebAssembly)
        {
            return;
        }

        var origin = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().GetLeftPart(UriPartial.Authority);
        Uno.WinRTFeatureConfiguration.WebAuthenticationBroker.DefaultReturnUri =
            new Uri($"{origin}{MsalConfig.WasmRedirectPath}");
    }

    private static void ConfigurePlatformRedirect(PublicClientApplicationBuilder builder)
    {
#if ANDROID
        builder.WithRedirectUri($"{MsalConfig.AndroidRedirectScheme}://auth");
#elif IOS
        builder.WithRedirectUri(MsalConfig.IosRedirectUri);
#else
        if (!PlatformHelper.IsWebAssembly)
        {
            builder.WithRedirectUri(MsalConfig.DesktopRedirectUri);
        }
#endif
    }

    public static void InitializeLogging()
    {
    }
}
