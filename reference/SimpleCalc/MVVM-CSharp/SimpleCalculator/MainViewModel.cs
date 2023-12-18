namespace SimpleCalculator;

public partial class MainViewModel : ObservableObject
{
	private readonly IThemeService _themeService;

	[ObservableProperty]
	private bool _isDark;

	partial void OnIsDarkChanged(bool value) =>
		_themeService.SetThemeAsync(value ? AppTheme.Dark : AppTheme.Light);

	public MainViewModel(IThemeService themeService)
	{
		_themeService = themeService;
		_isDark = themeService.IsDark;

		themeService.ThemeChanged += (_, _) =>
		{
			IsDark = themeService.IsDark;
		};
	}

	[ObservableProperty]
	private Calculator _calculator = new();

	[RelayCommand]
	private void Input(string key)
		=> Calculator = Calculator.Input(key);
}
