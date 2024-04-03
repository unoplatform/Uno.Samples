
namespace ToDo.Presentation;

public class ShellViewModel
{
	private readonly INavigator _navigator;
	private readonly IAuthenticationTokenProvider _auth;

	public ShellViewModel(
		INavigator navigator,
		IAuthenticationTokenProvider authentication)
	{
		_navigator = navigator;
		_auth= authentication;

		_ = Start();
	}

	public async Task Start()
	{
		var token = await _auth.GetAccessToken();
		if (string.IsNullOrWhiteSpace(token))
		{
			await _navigator.NavigateViewModelAsync<WelcomeViewModel>(this);
		}
		else
		{
			await _navigator.NavigateViewModelAsync<HomeViewModel>(this);
		}
	}
}
