
using SimpleCalculator.Business;
using Uno.Extensions.Reactive;

namespace SimpleCalculator.Presentation;

public partial class MainViewModel
{
	public string? Title { get; }
    public IState<Calculator> Calculator => State.Value(this, () => new Calculator());
    public async ValueTask Input(string key, CancellationToken ct)
            => await Calculator.Update(c => c?.Input(key), ct);

    public MainViewModel(
		INavigator navigator,
		IOptions<AppConfig> appInfo)
	{ 
	
		_navigator = navigator;
		Title = $"Main - {appInfo?.Value?.Title}";
	}

	private INavigator _navigator;
}
