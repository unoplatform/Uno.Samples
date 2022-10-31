
using SimpleCalculator.Business;
using SimpleCalculator.Keyboard;
using Uno.Extensions.Reactive;
using Windows.System;

namespace SimpleCalculator.Presentation;

public partial class MainViewModel
{
    public string? Title { get; }

    public IState<bool> IsDark => State.Value(this, () => _theme.IsDark);

    public IState<Calculator> Calculator => State.Value(this, () => new Calculator());
    public async ValueTask Input(string key, CancellationToken ct)
        => await Calculator.Update(c => c?.Input(key), ct);

    public async ValueTask KeyboardInput(string key, CancellationToken ct)
    {
        string? currentKey = KeyValues.Keys[key];

        if (!string.IsNullOrEmpty(key))
            await Calculator.Update(c => c?.Input(currentKey), ct);
    }

    public MainViewModel(
        INavigator navigator,
        IOptions<AppConfig> appInfo,
        IAppThemeService theme)
    {

        _navigator = navigator;
        _theme = theme;

        Title = $"Main - {appInfo?.Value?.Title}";
        IsDark.ForEachAsync(theme.SetThemeAsync);
    }

    private INavigator _navigator;
    private readonly IAppThemeService _theme;
}
