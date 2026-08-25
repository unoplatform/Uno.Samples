using Authentication.WebSteve.AuthFlow;

namespace Authentication.WebSteve.Presentation;

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
    /// Full sign-out: the end-session round trip through the browser, which clears the identity
    /// provider's own session as well as the cached tokens. The next sign-in prompts for
    /// credentials again.
    /// </summary>
    public async ValueTask Logout(CancellationToken token)
    {
        await _flow.SignOutEverywhereAsync(_dispatcher, token);

        await _navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
    }

    /// <summary>
    /// Local sign-out only: clears the cached tokens without the browser end-session round trip.
    /// The identity provider's browser session survives, so the next interactive sign-in
    /// completes without prompting for credentials - the contrast that makes the full sign-out
    /// above worth its extra round trip.
    /// </summary>
    public async ValueTask LogoutLocally(CancellationToken token)
    {
        await _flow.SignOutLocallyAsync(token);

        await _navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
    }

    private readonly IDispatcher _dispatcher;
    private readonly INavigator _navigator;
    private readonly AuthFlowService _flow;
}
