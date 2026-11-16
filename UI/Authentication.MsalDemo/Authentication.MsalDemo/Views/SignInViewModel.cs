using Authentication.MsalDemo.Authentication;
using Authentication.MsalDemo.Common;

namespace Authentication.MsalDemo.Views;

/// <summary>
/// View state for <see cref="SignInView"/>. All the MSAL work lives in
/// <see cref="AuthenticationService"/>; this only shapes it for display.
/// </summary>
internal sealed class SignInViewModel : ObservableObject
{
    private readonly AuthenticationService _auth = AuthenticationService.Instance;

    private bool _isBusy;
    private string _accountsSummary = "";

    public SignInViewModel()
    {
        _auth.StateChanged += (_, _) => RaiseResultProperties();
    }

    public AuthFlowLog Log => _auth.Log;

    public bool IsConfigured => MsalConfig.IsConfigured;

    public bool IsNotConfigured => !MsalConfig.IsConfigured;

    public string ConfigurationWarning =>
        $"""
        MsalConfig.ClientId is still the placeholder {MsalConfig.UnconfiguredClientId}, so sign-in
        will fail on purpose and tell you why.

        Register an app in the Microsoft Entra admin center, put its Application (client) ID in
        Authentication/MsalConfig.cs, and register the redirect URI below. The Platform setup page
        lists everything each head needs.
        """;

    public string PlatformName => PlatformSupport.PlatformName;

    public string RedirectUri => PlatformSupport.RedirectUri;

    public string RedirectUriSource => PlatformSupport.RedirectUriSource;

    public string ClientIdDisplay => MsalConfig.IsConfigured
        ? MsalConfig.ClientId
        : $"{MsalConfig.ClientId}  (not configured)";

    public string TenantDisplay => MsalConfig.Tenant;

    public string ScopesDisplay => string.Join(Environment.NewLine, MsalConfig.Scopes);

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

    public bool IsSignedIn => _auth.IsSignedIn;

    public string StatusHeadline => _auth.LastResult?.Account?.Username is { } username
        ? $"Signed in as {username}"
        : "Not signed in";

    public string ResultDetail
    {
        get
        {
            if (_auth.LastResult is not { } result)
            {
                return "No token yet. Use Sign in to run the full flow.";
            }

            var remaining = result.ExpiresOn - DateTimeOffset.Now;

            return $"""
                Tenant        {result.TenantId}
                Scopes        {string.Join(", ", result.Scopes)}
                Token source  {result.AuthenticationResultMetadata.TokenSource}
                Expires       {result.ExpiresOn.ToLocalTime():HH:mm:ss} (in {remaining.TotalMinutes:F0} min)
                Access token  {result.AccessToken.Length} chars
                """;
        }
    }

    public string AccountsSummary
    {
        get => _accountsSummary;
        private set => Set(ref _accountsSummary, value);
    }

    /// <summary>Cache first, prompt only if MSAL says it is required.</summary>
    public Task SignInAsync() => RunAsync(() => _auth.SignInAsync());

    /// <summary>Silent only, to show what a cache hit (or miss) looks like.</summary>
    public Task SignInSilentlyAsync() => RunAsync(() => _auth.SignInSilentlyAsync());

    public Task SignOutAsync() => RunAsync(async () =>
    {
        await _auth.SignOutAsync();
        return null;
    });

    public void ClearLog()
    {
        Log.Clear();
        Log.Info("Log cleared", $"Ready on {PlatformSupport.PlatformName}.");
    }

    public async Task RefreshAccountsAsync()
    {
        var accounts = await _auth.GetAccountsAsync();

        AccountsSummary = accounts.Count == 0
            ? "MSAL token cache: empty"
            : $"MSAL token cache: {string.Join(", ", accounts.Select(a => a.Username))}";
    }

    private async Task RunAsync(Func<Task<Microsoft.Identity.Client.AuthenticationResult?>> operation)
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
            await RefreshAccountsAsync();
        }
    }

    private void RaiseResultProperties()
    {
        Raise(nameof(IsSignedIn));
        Raise(nameof(StatusHeadline));
        Raise(nameof(ResultDetail));
    }
}
