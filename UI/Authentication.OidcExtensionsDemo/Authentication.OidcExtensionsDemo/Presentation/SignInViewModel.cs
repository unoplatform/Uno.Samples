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

    private bool _isBusy;
    private string _tokensSummary = "";
    private string _apiResponse = "";

    public SignInViewModel(OidcFlowService flow, IDispatcher dispatcher)
    {
        _flow = flow;
        _dispatcher = dispatcher;

        _flow.StateChanged += (_, _) => RaiseResultProperties();
    }

    public AuthFlowLog Log => _flow.Log;

    public string PlatformName => PlatformSupport.PlatformName;

    public string Authority => OidcFlowService.Authority;

    public string ClientDisplay => $"{OidcFlowService.ClientId}  (public Duende demo server - sign in with bob / bob)";

    public string ScopeDisplay => OidcFlowService.Scope;

    public string RedirectUri => _flow.RedirectUri;

    public string SignInSurface => PlatformSupport.SignInSurface;

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
            The Uno.Extensions provider owns the OidcClient; the app only sees the token.
            Call demo API sends it to the demo server's test endpoint to prove it works.
            """
        : "No token yet. Use Sign in to run the full flow (test user: bob / bob).";

    public string TokensSummary
    {
        get => _tokensSummary;
        private set => Set(ref _tokensSummary, value);
    }

    public string ApiResponse
    {
        get => _apiResponse;
        private set
        {
            Set(ref _apiResponse, value);
            Raise(nameof(HasApiResponse));
        }
    }

    public bool HasApiResponse => !string.IsNullOrEmpty(_apiResponse);

    /// <summary>Discovery, browser sign-in, code redemption - one provider call.</summary>
    public Task SignInAsync() => RunAsync(() => _flow.SignInAsync(_dispatcher));

    /// <summary>Silent only, to show what a refresh-token hit (or miss) looks like.</summary>
    public Task RefreshAsync() => RunAsync(() => _flow.RefreshAsync());

    public Task SignOutAsync() => RunAsync(async () =>
    {
        await _flow.SignOutAsync(_dispatcher);
        return true;
    });

    public Task CallApiAsync() => RunAsync(async () =>
    {
        ApiResponse = await _flow.CallApiAsync();
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
}
