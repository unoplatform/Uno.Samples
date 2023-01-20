using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalculator.Business;
using Uno.Extensions.Toolkit;

namespace SimpleCalculator.Presentation;

public partial class MainViewModel : ObservableObject
{
    private IThemeService _themeService;

    private bool _isDark;
    public bool IsDark
    {
        get => _isDark;
        set 
        { 
            if(SetProperty(ref _isDark, value))
            {
                _themeService.SetThemeAsync(value ? AppTheme.Dark : AppTheme.Light);
            }
        }
    }

    public MainViewModel(IThemeService themeService)
    {
        _themeService= themeService;
        _isDark = _themeService.IsDark;
    }

    [ObservableProperty]
    private Calculator _calculator = new();

    [RelayCommand]
    private void Input (string key)
        => Calculator = Calculator.Input(key);    
}
