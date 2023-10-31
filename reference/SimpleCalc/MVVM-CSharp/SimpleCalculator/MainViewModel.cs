using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SimpleCalculator;

public partial class MainViewModel : ObservableObject
{
	private readonly IThemeService _themeService;

	private bool _isDark;
    public bool IsDark
    {
        get => _isDark;
        set
        {
            if (SetProperty(ref _isDark, value))
            {
                _themeService.SetThemeAsync(value ? AppTheme.Dark : AppTheme.Light);
            }
        }
    }

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
