namespace Commerce.Presentation;

public partial class ProfileViewModel
{
    private readonly INavigator _navigator;

    private readonly IWritableOptions<Credentials> _credentials;

    private readonly IProfileService _profileService;

    private readonly IThemeService _appTheme;
    private readonly IWritableOptions<CommerceApp> _appSettings;

    public ProfileViewModel(
        INavigator navigator,
        IWritableOptions<Credentials> credentials,
        IProfileService profileService,
        IThemeService appTheme,
        IWritableOptions<CommerceApp> appSettings)
    {
        _navigator = navigator;
        _credentials = credentials;
        _profileService = profileService;
        _appTheme = appTheme;
        _appSettings = appSettings;

        var isDark = appTheme.IsDark;
        IsDarkTheme = State.Value(this, () => isDark);
        IsDarkTheme.ForEachAsync(ChangeAppTheme);
    }

    [Value]
    public IState<bool> IsDarkTheme { get; }

    public IFeed<Profile> Profile => Feed.Async(_profileService.GetProfile);

    public async ValueTask Logout()
    {
        await _credentials.UpdateAsync(c => new Credentials());
        await _navigator.NavigateRouteAsync(this, "/");
    }

    private async ValueTask ChangeAppTheme(bool isDark, CancellationToken ct)
    {
        await _appTheme.SetThemeAsync(isDark ? Uno.Extensions.Toolkit.AppTheme.Dark : Uno.Extensions.Toolkit.AppTheme.Light);
        await _appSettings.UpdateAsync(s => s with { IsDark = isDark });
    }
}
