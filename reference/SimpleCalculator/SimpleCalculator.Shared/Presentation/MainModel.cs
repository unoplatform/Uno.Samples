using SimpleCalculator.Business;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Uno.Extensions.Reactive;
using SimpleCalculator.ThemeService;

namespace SimpleCalculator.Presentation;

public partial record MainModel
{
    public IState<bool> IsDark => State.Value(this, () => _theme.IsDark);

    public IState<Calculator> Calculator => State.Value(this, () => new Calculator());
    public async ValueTask Input(KeyInput key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);
    public MainModel()
    {
        IsDark.ForEachAsync((dark, ct) => _theme.SetThemeAsync(dark, ct));
    }

    private IAppThemeService _theme => AppThemeService.Instance;
}
