using Windows.Security.Authentication.Web;
using ITokenCache = Uno.Extensions.Authentication.ITokenCache;

namespace Authentication.MsalExtensionsDemo;

public sealed partial class MainPage : Page
{
    private static IHost Host => App.AppHost ?? throw new InvalidOperationException("Host not built");

    private IAuthenticationService Auth => Host.Services.GetRequiredService<IAuthenticationService>();
    private IDispatcher AppDispatcher => Host.Services.GetRequiredService<IDispatcher>();
    private ITokenCache TokenCache => Host.Services.GetRequiredService<ITokenCache>();

    public MainPage()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Log($"Providers: {string.Join(", ", Auth.Providers)}");
            LogRedirectInfo();

            var authenticated = await Auth.IsAuthenticated();
            SetStatus(authenticated);
            Log($"IsAuthenticated at startup: {authenticated}");
            await ShowTokensAsync();
        }
        catch (Exception ex)
        {
            LogError("Startup check failed", ex);
        }
    }

    private async void OnSignIn(object sender, RoutedEventArgs e)
    {
        try
        {
            Log("LoginAsync ...");
            var ok = await Auth.LoginAsync(AppDispatcher);
            SetStatus(ok);
            Log($"LoginAsync -> {ok}");
            await ShowTokensAsync();
        }
        catch (Exception ex)
        {
            LogError("LoginAsync threw", ex);
        }
    }

    private async void OnRefresh(object sender, RoutedEventArgs e)
    {
        try
        {
            Log("RefreshAsync (silent) ...");
            var ok = await Auth.RefreshAsync();
            SetStatus(ok);
            Log($"RefreshAsync -> {ok}");
            await ShowTokensAsync();
        }
        catch (Exception ex)
        {
            LogError("RefreshAsync threw", ex);
        }
    }

    private async void OnSignOut(object sender, RoutedEventArgs e)
    {
        try
        {
            Log("LogoutAsync ...");
            var ok = await Auth.LogoutAsync(AppDispatcher);
            SetStatus(authenticated: !ok && await Auth.IsAuthenticated());
            Log($"LogoutAsync -> {ok}");
            await ShowTokensAsync();
        }
        catch (Exception ex)
        {
            LogError("LogoutAsync threw", ex);
        }
    }

    private async void OnCacheState(object sender, RoutedEventArgs e)
    {
        try
        {
            await ShowTokensAsync();
        }
        catch (Exception ex)
        {
            LogError("Reading token cache failed", ex);
        }
    }

    private async Task ShowTokensAsync()
    {
        var tokens = await TokenCache.GetAsync(CancellationToken.None);
        if (tokens.Count == 0)
        {
            TokenText.Text = "Token cache: empty";
            return;
        }

        var summary = string.Join(
            Environment.NewLine,
            tokens.Select(pair => $"{pair.Key}: {pair.Value.Length} chars"));
        TokenText.Text = $"Token cache ({tokens.Count} entries):{Environment.NewLine}{summary}";
    }

    private void LogRedirectInfo()
    {
        if (PlatformHelper.IsWebAssembly)
        {
            Log($"WASM redirect URI: {WebAuthenticationBroker.GetCurrentApplicationCallbackUri()}");
        }
        else
        {
#if ANDROID
            Log($"Redirect URI: {MsalConfig.AndroidRedirectScheme}://auth");
#elif IOS
            Log($"Redirect URI: {MsalConfig.IosRedirectUri}");
#else
            Log($"Redirect URI: {MsalConfig.DesktopRedirectUri} (loopback, any port)");
#endif
        }
    }

    private void SetStatus(bool authenticated) =>
        StatusText.Text = authenticated ? "Signed in" : "Not signed in";

    private void Log(string message)
    {
        LogText.Text += $"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}";
        LogScroll.ChangeView(null, LogScroll.ScrollableHeight, null, disableAnimation: true);
    }

    private void LogError(string context, Exception ex) =>
        Log($"{context}: {ex.GetType().Name}: {ex.Message}");
}
