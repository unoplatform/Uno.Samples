
namespace Commerce.ViewModels;

public class ShellViewModel
{
	private readonly INavigator _navigator;

	private IWritableOptions<Credentials> CredentialsSettings { get; }

	public ShellViewModel(
		ILogger<ShellViewModel> logger,
		INavigator navigator,
		IOptions<HostConfiguration> configuration,
		IWritableOptions<Credentials> credentials)
	{
		_navigator = navigator;
		CredentialsSettings = credentials;

		if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation($"Launch url '{configuration.Value?.LaunchUrl}'");
		// TODO: Fix launch URL
		var initialRoute = default(Route); // configuration.Value?.LaunchRoute();

		// Go to the login page on app startup
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		Start(initialRoute);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	}

	public async Task Start(Route? initialRoute = null)
	{
		var currentCredentials = CredentialsSettings.Value;

		if (currentCredentials?.UserName is { Length: > 0 })
		{
			if (initialRoute is not null)
			// TODO: Check for empty route
			//&& !initialRoute.IsEmpty())
			{
				var initialResponse = await _navigator.NavigateRouteForResultAsync<object>(this, initialRoute.ToString());
				if (initialResponse is not null)
				{
					_ = await initialResponse.Result;
				}
			}
			else
			{
				var homeResponse = await _navigator.NavigateDataAsync(this, currentCredentials, Qualifiers.ClearBackStack);
			}
		}
		else
		{
			// Navigate to Login page, requesting Credentials
			var response = await _navigator.NavigateForResultAsync<Credentials>(this, Qualifiers.ClearBackStack);

			if (response?.Result is null)
			{
				_ = Start();
				return;
			}

			var loginResult = await response.Result;
			if (loginResult.IsSome(out var creds) && creds?.UserName is { Length: > 0 })
			{
				await CredentialsSettings.UpdateAsync(c => creds);

				_ = Start();
			}
		}
	}
}
