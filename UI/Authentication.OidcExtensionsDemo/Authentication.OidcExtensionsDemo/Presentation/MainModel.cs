using Authentication.OidcExtensionsDemo.AuthFlow;

namespace Authentication.OidcExtensionsDemo.Presentation;

public partial record MainModel
{
    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IDispatcher dispatcher,
        INavigator navigator,
        AuthFlowService flow)
    {
        _dispatcher = dispatcher;
        _navigator = navigator;
        _flow = flow;
        Title = "Signed in";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    /// <summary>
    /// Local sign-out only: clears the cached tokens without the browser end-session round trip.
    /// The identity provider's browser session survives, so the next interactive sign-in may
    /// complete without prompting for credentials. Use <see cref="LogoutEverywhere"/> to also end
    /// the identity provider session.
    /// </summary>
    public async ValueTask Logout(CancellationToken token)
    {
        await _flow.SignOutLocallyAsync(token);

        await _navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
    }

    /// <summary>
    /// Full sign-out: drives the identity provider's end-session flow in the browser, then clears
    /// the cached tokens.
    /// </summary>
    public async ValueTask LogoutEverywhere(CancellationToken token)
    {
        var loggedOut = await _flow.SignOutEverywhereAsync(_dispatcher, token);

        if (loggedOut)
        {
            await _navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
        }
    }

    /// <summary>Calls the demo server's test API with the cached access token.</summary>
    public ValueTask CallApi(CancellationToken token) => new(_flow.CallApiAsync(token));

    private readonly IDispatcher _dispatcher;
    private readonly INavigator _navigator;
    private readonly AuthFlowService _flow;
}
