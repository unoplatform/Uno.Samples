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
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }


    public async ValueTask Logout(CancellationToken token)
    {
        await _authentication.LogoutAsync(token);
    }

    private IAuthenticationService _authentication;
}
