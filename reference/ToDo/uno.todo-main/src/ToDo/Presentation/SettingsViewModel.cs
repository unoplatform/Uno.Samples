namespace ToDo.Presentation;

public partial class SettingsViewModel
{
	private readonly IAuthenticationService _authService;
	private readonly IUserProfilePictureService _userSvc;
	private readonly INavigator _sourceNavigator;
	private readonly INavigator _navigator;
	private IAppTheme _appTheme;
	private IWritableOptions<ToDoApp> _appSettings;

	public IWritableOptions<LocalizationSettings> LocalizationSettings { get; }

	public DisplayCulture[] Cultures { get; }

	public string[] AppThemes { get; }


	private SettingsViewModel(
		NavigationRequest request,
		INavigator navigator,
		IAuthenticationService authService,
		IUserProfilePictureService userSvc,
		IOptions<LocalizationConfiguration> localizationConfiguration,
		IWritableOptions<LocalizationSettings> localizationSettings,
		IStringLocalizer localizer,
		IAppTheme appTheme,
		IWritableOptions<ToDoApp> appSettings)
	{
		_sourceNavigator = request?.Source ?? navigator;
		_navigator = navigator;
		_authService = authService;
		_userSvc = userSvc;
		LocalizationSettings = localizationSettings;
		_appTheme = appTheme;
		_appSettings = appSettings;

		AppThemes = new string[] { localizer["SettingsFlyout_ThemeLight"], localizer["SettingsFlyout_ThemeDark"] };
		SelectedAppTheme = State.Value(this, () => AppThemes[appTheme.IsDark ? 1 : 0]);

		SelectedAppTheme.Execute(ChangeAppTheme);

		Cultures = localizationConfiguration.Value!.Cultures!.Select(c => new DisplayCulture(localizer[$"SettingsFlyout_LanguageLabel_{c}"], c)).ToArray();
		SelectedCulture = State.Value(this, () => Cultures.FirstOrDefault(c => c.Culture == LocalizationSettings.Value?.CurrentCulture) ?? Cultures.First());

		SelectedCulture.Execute(ChangeLanguage);
	}

	public IFeed<UserContext?> CurrentUser => Feed<UserContext?>.Async(async ct => await _authService.GetCurrentUserAsync());
	public IFeed<byte[]?> ProfilePicture => Feed<byte[]?>.Async(async ct => await _userSvc.GetAsync(await CurrentUser, ct));

	[Value]
	public IState<DisplayCulture> SelectedCulture { get; }

	[Value]
	public IState<string> SelectedAppTheme { get; }


	public async ValueTask SignOut(CancellationToken ct)
	{
		var result = await _navigator.ShowMessageDialogAsync<object>(this, Dialog.ConfirmSignOut, cancellation: ct);
		if (result == DialogResults.Affirmative)
		{
			await _authService.SignOutAsync();

			await _sourceNavigator.NavigateViewModelAsync<HomeViewModel>(this);
		}
	}

	private async ValueTask ChangeLanguage(DisplayCulture? culture, CancellationToken ct)
	{
		if (culture is not null)
		{
			await LocalizationSettings.UpdateAsync(settings => settings with { CurrentCulture = culture.Culture });
		}
	}


	private async ValueTask ChangeAppTheme(string? appTheme, CancellationToken ct)
	{
		if (appTheme is { Length: > 0 })
		{
			var isDark = Array.IndexOf(AppThemes, appTheme) == 1;
			await _appTheme.SetThemeAsync(isDark);
			await _appSettings.UpdateAsync(s => s with { IsDark = isDark });
		}
	}

	public partial record DisplayCulture(string Display, string Culture);
}
