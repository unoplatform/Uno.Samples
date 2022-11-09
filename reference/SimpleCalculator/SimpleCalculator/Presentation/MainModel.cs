
using SimpleCalculator.Business;
using SimpleCalculator.Keyboard;
using Uno.Extensions.Reactive;

namespace SimpleCalculator.Presentation;

public partial record MainModel
{
    public IState<bool> IsDark => State.Value(this, () => _theme.IsDark);

    public IState<Calculator> Calculator => State.Value(this, () => new Calculator());
	public async ValueTask Input(string key, CancellationToken ct)
			=> await Calculator.Update(c => c?.Input(key), ct);

	public MainModel(IAppThemeService theme)
    {
        _theme = theme;
        IsDark.ForEachAsync((dark, ct) => theme.SetThemeAsync(dark, ct));

    }

    private readonly IAppThemeService _theme;
}
