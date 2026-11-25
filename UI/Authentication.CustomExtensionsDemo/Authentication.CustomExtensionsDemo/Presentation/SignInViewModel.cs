using Authentication.CustomExtensionsDemo.Authentication;
using Authentication.CustomExtensionsDemo.Common;

namespace Authentication.CustomExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="SignInView"/>. All the authentication work lives in
/// <see cref="CustomFlowService"/>; this only shapes it for display.
/// </summary>
public sealed class SignInViewModel : ObservableObject
{
    private readonly CustomFlowService _flow;
    private readonly IDispatcher _dispatcher;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _tokensSummary = "";

    public SignInViewModel(CustomFlowService flow, IDispatcher dispatcher, SecretRedactor redactor)
    {
        _flow = flow;
        _dispatcher = dispatcher;
        _redactor = redactor;

        _flow.StateChanged += (_, _) =>
        {
            RaiseResultProperties();
            // LoggedOut can fire outside a button press (an API 401, a failed refresh), so the
            // TOKENS card follows the state rather than the last click.
            _ = RefreshTokensSummaryAsync();
        };

        // Recording mode hides identifiers at display time, so every property that can carry one
        // has to be re-read when it is switched.
        _redactor.Changed += (_, _) => RaiseRedactedProperties();
    }

    public AuthFlowLog Log => _flow.Log;

    public bool IsConfigured => _flow.IsConfigured;

    public bool IsNotConfigured => !_flow.IsConfigured;

    public string ConfigurationWarning =>
        """
        The DemoServer section of appsettings has no absolute Url, so sign-in will fail on
        purpose and tell you why. Put the demo server's address there; the Platform setup page
        lists what each head needs.
        """;

    public string PlatformName => PlatformSupport.PlatformName;

    public string ApiUrlDisplay => Display(_flow.ApiUrl);

    public string ServerSource => PlatformSupport.ServerSource;

    /// <summary>Bound two-way to the login form; the demo accounts are alice/alice and bob/bob.</summary>
    public string Username { get; set; } = "alice";

    /// <summary>Bound two-way to the PasswordBox. Passed to LoginAsync once, never stored.</summary>
    public string Password { get; set; } = "";

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            Set(ref _isBusy, value);
            Raise(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !_isBusy;

    public bool IsSignedIn => _flow.IsSignedIn;

    public string StatusHeadline => _flow.IsSignedIn ? "Signed in" : "Not signed in";

    public string ResultDetail => _flow.AccessToken is { } token
        ? $"""
            Access token  {token.Length} chars, held in ITokenCache
            The Login callback returned the server's tokens and nothing else; the provider stored
            them, and the "Call the API" page proves the access token works.
            """
        : "No token yet. Use Sign in to run the flow.";

    public string TokensSummary
    {
        get => Redacted(_tokensSummary);
        private set => Set(ref _tokensSummary, value);
    }

    /// <summary>The interactive flow: the credentials from the form, one provider call.</summary>
    public Task SignInAsync() => RunAsync(() => _flow.SignInAsync(_dispatcher, Username, Password));

    /// <summary>Silent only, to show what a stored refresh token does (or its absence).</summary>
    public Task RefreshAsync() => RunAsync(() => _flow.RefreshAsync());

    public Task SignOutAsync() => RunAsync(async () =>
    {
        await _flow.SignOutAsync(_dispatcher);
        return true;
    });

    public void ClearLog()
    {
        Log.Clear();
        Log.Info("Log cleared", $"Ready on {PlatformSupport.PlatformName}.");
    }

    public async Task RefreshTokensSummaryAsync() => TokensSummary = await _flow.DescribeTokensAsync();

    private async Task RunAsync(Func<Task<bool>> operation)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;

        try
        {
            await operation();
        }
        finally
        {
            IsBusy = false;
            await RefreshTokensSummaryAsync();
        }
    }

    private void RaiseResultProperties()
    {
        Raise(nameof(IsSignedIn));
        Raise(nameof(StatusHeadline));
        Raise(nameof(ResultDetail));
    }

    private void RaiseRedactedProperties()
    {
        Raise(nameof(ResultDetail));
        Raise(nameof(TokensSummary));
    }

    private static string Display(string? value) => string.IsNullOrEmpty(value) ? "(empty)  (not configured)" : value;

    private string Redacted(string value) => _redactor.Apply(value) ?? value;
}
