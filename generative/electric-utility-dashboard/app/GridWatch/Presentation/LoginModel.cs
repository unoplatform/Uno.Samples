using Uno.Extensions.Reactive;

namespace GridWatch.Presentation;

public partial record LoginModel(INavigator Navigator)
{
	public IState<string> Username => State.Value(this, () => string.Empty);
	public IState<string> Password => State.Value(this, () => string.Empty);
	public IState<string> ErrorMessage => State.Value(this, () => string.Empty);
	public IState<bool> HasError => State.Value(this, () => false);

	public async ValueTask Login(CancellationToken ct)
	{
		var username = await Username.Value(ct) ?? string.Empty;
		var password = await Password.Value(ct) ?? string.Empty;

		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
		{
			await ErrorMessage.Set("Please enter your username and password.", ct);
			await HasError.Set(true, ct);
			return;
		}

		if (username.Trim() == "admin" && password.Trim() == "grid2024")
		{
			await HasError.Set(false, ct);
			await Navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack, cancellation: ct);
		}
		else
		{
			await ErrorMessage.Set("Invalid credentials. Use: admin / grid2024", ct);
			await HasError.Set(true, ct);
		}
	}
}
