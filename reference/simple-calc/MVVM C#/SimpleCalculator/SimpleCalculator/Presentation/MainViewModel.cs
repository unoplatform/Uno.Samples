using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleCalculator.Business;
using SimpleCalculator.ThemeService;
using Windows.System;
using System.Linq;

namespace SimpleCalculator.Presentation;

public partial class MainViewModel : ObservableObject
{
    private bool _isDark = AppThemeService.Instance.IsDark;
    public bool IsDark
    {
        get => _isDark;
        set 
        { 
            if(SetProperty(ref _isDark, value))
            {
                AppThemeService.Instance.SetThemeAsync(value, default);
            }
        }
    }

    [ObservableProperty]
    private Calculator _calculator = new();

    [RelayCommand]
    private void Input (string key)
        => Calculator = Calculator.Input(key);
}
