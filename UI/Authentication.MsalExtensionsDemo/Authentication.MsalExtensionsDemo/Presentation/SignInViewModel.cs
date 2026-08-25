using Authentication.MsalExtensionsDemo.Authentication;
using Authentication.MsalExtensionsDemo.Common;

namespace Authentication.MsalExtensionsDemo.Presentation;

/// <summary>
/// View state for <see cref="SignInView"/>. All the authentication work lives in
/// <see cref="MsalFlowService"/>; this only shapes it for display.
/// </summary>
public sealed class SignInViewModel : ObservableObject
{
    private readonly MsalFlowService _flow;
    private readonly IDispatcher _dispatcher;
    private readonly SecretRedactor _redactor;

    private bool _isBusy;
    private string _tokensSummary = "";

    public SignInViewModel(MsalFlowService flow, IDispatcher dispatcher, SecretRedactor redactor)
    {
        _flow = flow;
        _dispatcher = dispatcher;
        _redactor = redactor;

        _flow.StateChanged += (_, _) => RaiseResultProperties();

        // Recording mode hides identifiers at display time, so every property that can carry one
        // has to be re-read when it is switched.
        _redactor.Changed += (_, _) => RaiseRedactedProperties();
    }

    public AuthFlowLog Log => _flow.Log;

    public bool IsConfigured => _flow.IsConfigured;

    public bool IsNotConfigured => !_flow.IsConfigured;

    public string ConfigurationWarning =>
        """
        The MsalAuthentication section of appsettings does not contain a valid ClientId, so
        sign-in will fail on purpose and tell you why.

        Register an app in the Microsoft Entra admin center, put its Application (client) ID in
        appsettings.development.json under MsalAuthentication, and register the redirect URI
        below. The Platform setup page lists everything each head needs.
        """;

    public string PlatformName => PlatformSupport.PlatformName;

    public string RedirectUri => _redactor.Apply(_flow.RedirectUri) ?? _flow.RedirectUri;

    public string RedirectUriSource => PlatformSupport.RedirectUriSource;

    public string ClientIdDisplay => Redacted(_flow.IsConfigured
        ? _flow.ClientId!
        : $"{(string.IsNullOrEmpty(_flow.ClientId) ? "(empty)" : _flow.ClientId)}  (not configured)");

    public string TenantDisplay => Redacted(string.IsNullOrEmpty(_flow.TenantId) ? "(not set)" : _flow.TenantId!);

    public string ScopesDisplay => _flow.Scopes.Length == 0
        ? "(none configured)"
        : string.Join(Environment.NewLine, _flow.Scopes);

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
            The Uno.Extensions provider owns the MSAL result; the app sees the token itself,
            and the Microsoft Graph page proves it works.
            """
        : "No token yet. Use Sign in to run the full flow.";

    public string TokensSummary
    {
        get => Redacted(_tokensSummary);
        private set => Set(ref _tokensSummary, value);
    }

    /// <summary>Cache first, prompt only if MSAL says it is required - one provider call.</summary>
    public Task SignInAsync() => RunAsync(() => _flow.SignInAsync(_dispatcher));

    /// <summary>Silent only, to show what a cache hit (or miss) looks like.</summary>
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

    public async Task RefreshTokensSummaryAsync() =>
        TokensSummary = await _flow.DescribeTokensAsync();

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
        Raise(nameof(TenantDisplay));
        Raise(nameof(RedirectUri));
        Raise(nameof(ResultDetail));
        Raise(nameof(TokensSummary));
    }

    private string Redacted(string value) => _redactor.Apply(value) ?? value;
}
