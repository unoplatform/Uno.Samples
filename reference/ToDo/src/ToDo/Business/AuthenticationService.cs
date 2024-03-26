namespace ToDo.Business.Services;

public class AuthenticationService : IAuthenticationService
{
	private readonly IPublicClientApplication _pca;
	private readonly string[] _scopes;
	private readonly ILogger _logger;

	private UserContext? _user;

	public AuthenticationService(
		ILogger<AuthenticationService> logger,
		IOptions<Auth> settings)
	{
		_logger = logger;
		var authSettings = settings.Value;
		_scopes = authSettings.Scopes ?? new string[] { };

		var redirectUri = authSettings.RedirectUri;
		if (redirectUri is "%WAB%")
		{
			redirectUri = GetWebRedirectUri();
		}

		var builder = PublicClientApplicationBuilder
				.Create(authSettings.ApplicationId)
				.WithRedirectUri(redirectUri)
				.WithUnoHelpers();
		if (!string.IsNullOrWhiteSpace(authSettings.KeychainSecurityGroup))
		{
			builder = builder.WithIosKeychainSecurityGroup(authSettings.KeychainSecurityGroup);
		}
		_pca = builder.Build();
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private string GetWebRedirectUri()
	{
		return WebAuthenticationBroker.GetCurrentApplicationCallbackUri().OriginalString;
	}

	public async Task<string> GetAccessToken()
	{
		var result = await AcquireSilentTokenAsync();

		return result?.AccessToken ?? string.Empty;
	}

	public async Task<UserContext?> GetCurrentUserAsync() => _user;

	public async Task<UserContext?> AuthenticateAsync(IDispatcher dispatcher)
	{
		try
		{
			var result = await AcquireTokenAsync(dispatcher);
			_user = !string.IsNullOrEmpty(result?.AccessToken)
				? CreateContextFromAuthResult(result!)
				: default;

			return _user;
		}
		catch (MsalClientException ex)
		{
			//This is thrown when the user closes the webview before he can authenticate
			throw new MsalClientException(ex.ErrorCode, ex.Message);
		}
		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	public async Task SignOutAsync()
	{
		var accounts = await _pca.GetAccountsAsync();
		var firstAccount = accounts.FirstOrDefault();
		if (firstAccount == null)
		{
			_logger.LogInformation(
			  "Unable to find any accounts to log out of.");
			return;
		}

		await _pca.RemoveAsync(firstAccount);
		_logger.LogInformation($"Removed account: {firstAccount.Username}, user succesfully logged out.");
	}

	private UserContext CreateContextFromAuthResult(AuthenticationResult authResult)
	{
		var token = new JwtSecurityTokenHandler().ReadJwtToken(authResult.IdToken);
		return new UserContext
		{
			Name = token.Claims.First(c => c.Type.Equals("name")).Value,
			Email = token.Claims.First(c => c.Type.Equals("preferred_username")).Value,
			AccessToken = authResult.AccessToken
		};
	}

	private async Task<AuthenticationResult?> AcquireTokenAsync(IDispatcher dispatcher)
	{
		var authentication = await AcquireSilentTokenAsync();

		if (string.IsNullOrEmpty(authentication?.AccessToken))
		{
			authentication = await AcquireInteractiveTokenAsync(dispatcher);
		}

		return authentication;
	}

	private async ValueTask<AuthenticationResult> AcquireInteractiveTokenAsync(IDispatcher dispatcher)
	{
		return await dispatcher.ExecuteAsync(async ct => await _pca
		  .AcquireTokenInteractive(_scopes)
		  .WithUnoHelpers()
		  .ExecuteAsync());
	}


	private async Task<AuthenticationResult?> AcquireSilentTokenAsync()
	{
		var accounts = await _pca.GetAccountsAsync();
		var firstAccount = accounts.FirstOrDefault();

		if (firstAccount == null)
		{
			_logger.LogInformation("Unable to find Account in MSAL.NET cache");
			return default;
		}

		if (accounts.Any())
		{
			_logger.LogInformation($"Number of Accounts: {accounts.Count()}");
		}

		try
		{
			_logger.LogInformation("Attempting to perform silent sign in . . .");
			_logger.LogInformation($"Authentication Scopes: {JsonSerializer.Serialize(_scopes)}");

			_logger.LogInformation($"Account Name: {firstAccount.Username}");

			return await _pca
			  .AcquireTokenSilent(_scopes, firstAccount)
			  //.WaitForRefresh(false)
			  .ExecuteAsync();
		}
		catch (MsalUiRequiredException ex)
		{
			_logger.LogWarning(ex, ex.Message);
			_logger.LogWarning(
			  "Unable to retrieve silent sign in Access Token");
		}
		catch (Exception ex)
		{
			_logger.LogWarning(ex, ex.Message);
			_logger.LogWarning("Unable to retrieve silent sign in details");
		}

		return default;
	}
}

