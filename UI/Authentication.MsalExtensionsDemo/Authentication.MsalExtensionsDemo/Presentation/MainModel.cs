namespace Authentication.MsalExtensionsDemo.Presentation;

public partial record MainModel
{
    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        IAuthenticationService authentication,
        INavigator navigator)
    {
        _authentication = authentication;
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }


    public async ValueTask Logout(CancellationToken token)
    {
        // LogoutAsync only clears the token cache - the app still has to leave the authenticated
        // page, or a completely successful sign-out looks like the button did nothing.
        if (await _authentication.LogoutAsync(token))
        {
            // First argument is the *sender* (object), not the cancellation token: passing `token`
            // there compiles, because sender is typed `object`, and then navigation has no view
            // model to resolve a region from.
            await _navigator.NavigateViewModelAsync<LoginModel>(
                this,
                qualifier: Qualifiers.ClearBackStack,
                cancellation: token);
        }
    }

    private IAuthenticationService _authentication;
    private INavigator _navigator;
}
