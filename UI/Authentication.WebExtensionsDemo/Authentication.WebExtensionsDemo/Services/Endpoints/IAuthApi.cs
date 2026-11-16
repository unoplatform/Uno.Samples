using Authentication.WebExtensionsDemo.DataContracts;
using Refit;

namespace Authentication.WebExtensionsDemo.Services.Endpoints;

/// <summary>
/// The backend's own API, as the app calls it outside the browser. Registered with
/// <c>services.AddRefitClient&lt;IAuthApi&gt;(context)</c> inside <c>UseHttp</c> (base address
/// from the <c>AuthApi</c> section of appsettings), and handed to the Web provider's
/// <c>Refresh</c> callback by <c>AddWeb&lt;IAuthApi&gt;</c>.
/// </summary>
[Headers("Accept: application/json")]
public interface IAuthApi
{
    /// <summary>
    /// Trades a refresh token for new tokens. Marked no-refresh: the authentication handler sits
    /// in this client's pipeline too, and without the header a 401 here would wait on the very
    /// refresh it is part of.
    /// </summary>
    [Post("/auth/refresh")]
    [Headers(Uno.Extensions.Authentication.Headers.NoRefresh)]
    Task<TokenResponse> RefreshAsync([Body] RefreshRequest request, CancellationToken ct);
}
