
using SimpleCalculator.Business;
using SimpleCalculator.Keyboard;
using Uno.Extensions.Reactive;

namespace SimpleCalculator.Presentation;

public partial class MainModel
{
	public IState<Calculator> Calculator => State.Value(this, () => new Calculator());

	public async ValueTask Input(string key, CancellationToken ct)
			=> await Calculator.Update(c => c?.Input(key), ct);
}
