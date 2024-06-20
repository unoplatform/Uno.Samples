namespace ToDo.Business;

public interface IAuthenticationService: IAuthenticationTokenProvider
{
	Task<UserContext?> GetCurrentUserAsync();

	Task<UserContext?> AuthenticateAsync(IDispatcher dispatcher);

	Task SignOutAsync();
}
