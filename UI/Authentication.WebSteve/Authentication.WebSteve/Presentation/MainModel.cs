using Authentication.WebSteve.AuthFlow;

namespace Authentication.WebSteve.Presentation;

public partial record MainModel
{
    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator,
        AuthFlowService flow)
    {
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
    /// complete without prompting for credentials.
    /// </summary>
    public async ValueTask Logout(CancellationToken token)
    {
        await _flow.SignOutLocallyAsync(token);

        await _navigator.NavigateViewModelAsync<LoginModel>(this, qualifier: Qualifiers.ClearBackStack);
    }

    private readonly INavigator _navigator;
    private readonly AuthFlowService _flow;
}
