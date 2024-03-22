using System.Windows.Input;

namespace Commerce.Presentation;

public partial record LoginViewModel
{
	private readonly INavigator _navigator;

	public LoginViewModel(
		INavigator navigator,
		IOptions<AppInfo> appInfo)
	{
		Title = appInfo.Value.Title;
		_navigator = navigator;
	}

	public string? Title { get; }

	public IState<Credentials> Credentials => State<Credentials>.Empty(this);

	public ICommand Login => Command.Create(b => b.Given(Credentials).When(CanLogin).Then(DoLogin));

	private bool CanLogin(Credentials credentials)
		=> credentials is { UserName.Length: > 0 } and { Password.Length: > 0 };

	private async ValueTask DoLogin(Credentials credentials, CancellationToken ct)
		=> await _navigator.NavigateBackWithResultAsync(this, data: Option.Some(credentials), cancellation: ct);
}
