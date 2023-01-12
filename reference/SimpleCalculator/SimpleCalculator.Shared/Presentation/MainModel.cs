using SimpleCalculator.Business;
using System.Threading.Tasks;
using System.Threading;
using Uno.Extensions.Reactive;
using SimpleCalculator.ThemeService;
using Windows.System;

namespace SimpleCalculator.Presentation;

public partial record MainModel
{
    public IState<bool> IsDark { get; }

    public IState<Calculator> Calculator { get; }

    public async ValueTask Input(KeyInput key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);
    
    public async ValueTask InputVirtualKey(VirtualKey key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);
    public MainModel()
    {
        Calculator = State.Value(this, () => new Calculator());
        IsDark = State.Value(this, () => _theme.IsDark);

        IsDark.ForEachAsync((dark, ct) => _theme.SetThemeAsync(dark, ct));
    }

    private IAppThemeService _theme => AppThemeService.Instance;
}
