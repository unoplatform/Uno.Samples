using Authentication.WebExtensionsDemo.DataContracts;
using Refit;

namespace Authentication.WebExtensionsDemo.Services.Endpoints;

/// <summary>
/// The backend's protected API. <c>GET /api/profile</c> answers 401 without a live access token
/// and says whose token it was otherwise, which makes it a convenient proof the token was sent.
/// </summary>
/// <remarks>
/// Registered with <c>services.AddRefitClient&lt;IProfileApi&gt;(context)</c> inside
/// <c>UseHttp</c> in <c>App.xaml.cs</c>. That registration is what chains the authentication
/// handler into the client: the access token in <c>ITokenCache</c> is added as an
/// <c>Authorization: Bearer</c> header on every call, and a 401 triggers a token refresh and one
/// retry. The base address comes from the <c>ProfileApi</c> section of appsettings (the
/// interface name without its leading <c>I</c>).
/// </remarks>
[Headers("Accept: application/json")]
public interface IProfileApi
{
    [Get("/api/profile")]
    Task<Profile> GetProfileAsync(CancellationToken ct);
}
