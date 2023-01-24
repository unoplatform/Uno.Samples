using SimpleCalculator.Business;
using Uno.Extensions.Reactive;
using Uno.Extensions.Toolkit;

namespace SimpleCalculator.Presentation;

public partial record MainModel
{
    public IState<bool> IsDark { get; }

    private IThemeService _themeService;

    public IState<Calculator> Calculator { get; }

    public async ValueTask InputCommand(string key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);

    public MainModel(IThemeService themeService)
    {
        _themeService = themeService;

        Calculator = State.Value(this, () => new Calculator());
        IsDark = State.Value(this, () => _themeService.IsDark);

        IsDark.ForEachAsync(async (dark, ct) => await _themeService.SetThemeAsync(dark ? AppTheme.Dark : AppTheme.Light));
    }
}
