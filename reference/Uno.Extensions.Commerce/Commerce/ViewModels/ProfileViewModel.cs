using Uno.Extensions.Authentication;

namespace Commerce.ViewModels;

public partial class ProfileViewModel
{
	private readonly INavigator _navigator;

	private readonly IProfileService _profileService;

	private readonly IAppTheme _appTheme;
	private readonly IWritableOptions<CommerceApp> _appSettings;

	private readonly IAuthenticationService _authenticationService;


    public ProfileViewModel(
		INavigator navigator,
		IProfileService profileService,
		IAppTheme appTheme,
		IWritableOptions<CommerceApp> appSettings,
        IAuthenticationService authenticationService)
	{
		_navigator = navigator;
		_profileService = profileService;
		_appTheme = appTheme;
		_appSettings = appSettings;

		IsDarkTheme = State.Value(this, () => appTheme.IsDark);
		IsDarkTheme.ForEachAsync(ChangeAppTheme);
		_authenticationService = authenticationService;

    }

	[Value]
	public IState<bool> IsDarkTheme { get; }

	public IFeed<Profile> Profile => Feed.Async(_profileService.GetProfile);

	public async ValueTask Logout(CancellationToken ct)
	{
		await _authenticationService.LogoutAsync(ct);
		await _navigator.NavigateRouteAsync(this, "/");
	}

	private async ValueTask ChangeAppTheme(bool isDark, CancellationToken ct)
	{
		await _appTheme.SetThemeAsync(isDark);
		await _appSettings.UpdateAsync(s => s with { IsDark = isDark });
	}
}
