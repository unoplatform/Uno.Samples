using Authentication.OidcExtensionsDemo.Authentication;
using Authentication.OidcExtensionsDemo.Common;

namespace Authentication.OidcExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="SignInView"/>. All the authentication work lives in
/// <see cref="OidcFlowService"/>; this only shapes it for display.
/// </summary>
public sealed class SignInViewModel : ObservableObject
{
    private readonly OidcFlowService _flow;
    private readonly IDispatcher _dispatcher;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _tokensSummary = "";
    private string? _claimsSummary;

    public SignInViewModel(OidcFlowService flow, IDispatcher dispatcher, SecretRedactor redactor)
    {
        _flow = flow;
        _dispatcher = dispatcher;
        _redactor = redactor;

        _flow.StateChanged += (_, _) =>
        {
            RaiseResultProperties();
            // LoggedOut can fire outside a button press (an API 401, a failed refresh), so the
            // TOKEN card follows the state rather than the last click.
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
        The OidcAuthentication section of appsettings does not name an Authority and a ClientId,
        so sign-in will fail on purpose and tell you why.

        Put the values from your identity provider's client registration in
        appsettings.development.json under OidcAuthentication, and register the redirect URI
        below. The Platform setup page lists everything each head needs.
        """;

    public string PlatformName => PlatformSupport.PlatformName;

    public string RedirectUri => _redactor.Apply(_flow.RedirectUri) ?? _flow.RedirectUri;

    public string RedirectUriSource => PlatformSupport.RedirectUriSource;

    public string AuthorityDisplay => string.IsNullOrEmpty(_flow.Authority) ? "(empty)  (not configured)" : _flow.Authority!;

    public string ClientIdDisplay => Redacted(string.IsNullOrEmpty(_flow.ClientId) ? "(empty)  (not configured)" : _flow.ClientId!);

    public string ScopeDisplay => string.IsNullOrEmpty(_flow.Scope)
        ? "(none configured)"
        : string.Join(Environment.NewLine, _flow.Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries));

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
            The Uno.Extensions provider owns the OidcClient; the app sees the tokens, and the
            "Call the API" page proves the access token works.
            """
        : "No token yet. Use Sign in to run the flow.";

    public string TokensSummary
    {
        get => Redacted(_tokensSummary);
        private set => Set(ref _tokensSummary, value);
    }

    /// <summary>The claims decoded from the cached ID token; every value is hidden while recording.</summary>
    public string ClaimsSummary =>
        _claimsSummary is { } json ? $"ID token claims{Environment.NewLine}{_redactor.ApplyToJson(json)}" : "";

    public bool HasClaims => _claimsSummary is not null;

    /// <summary>The interactive code flow: one provider call.</summary>
    public Task SignInAsync() => RunAsync(() => _flow.SignInAsync(_dispatcher));

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

    public async Task RefreshTokensSummaryAsync()
    {
        TokensSummary = await _flow.DescribeTokensAsync();
        _claimsSummary = await _flow.DescribeClaimsAsync();
        Raise(nameof(ClaimsSummary));
        Raise(nameof(HasClaims));
    }

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
        Raise(nameof(ClientIdDisplay));
        Raise(nameof(RedirectUri));
        Raise(nameof(ResultDetail));
        Raise(nameof(TokensSummary));
        Raise(nameof(ClaimsSummary));
    }

    private string Redacted(string value) => _redactor.Apply(value) ?? value;
}
