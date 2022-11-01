
using SimpleCalculator.Business;
using SimpleCalculator.Keyboard;
using Uno.Extensions.Reactive;

namespace SimpleCalculator.Presentation;

public partial class MainModel
{
	public IState<Calculator> Calculator => State.Value(this, () => new Calculator());
	public async ValueTask Input(string key, CancellationToken ct)
			=> await Calculator.Update(c => c?.Input(key), ct);

	public async ValueTask KeyboardInput(string key, CancellationToken ct)
	{
		string? currentKey = KeyValues.Keys[key];

		if(!string.IsNullOrEmpty(key))
			await Calculator.Update(c => c?.Input(currentKey), ct);
	}

	public MainModel()
	{ 
	}
}
