namespace Authentication.WebExtensionsDemo.DataContracts;

/// <summary>
/// The body the app posts to <c>POST /auth/refresh</c> to trade a refresh token for new tokens.
/// </summary>
/// <param name="RefreshToken">The refresh token the server issued with the last sign-in or refresh.</param>
public record RefreshRequest(string RefreshToken);

/// <summary>
/// What <c>POST /auth/refresh</c> returns. The refresh token rotates: the one sent is spent.
/// </summary>
/// <param name="AccessToken">The new access token.</param>
/// <param name="RefreshToken">The new refresh token, replacing the one sent.</param>
/// <param name="ExpiresIn">Lifetime of the access token, in seconds.</param>
public record TokenResponse(string AccessToken, string RefreshToken, int ExpiresIn);

/// <summary>
/// What <c>GET /api/profile</c> returns: who the access token belongs to, as the server sees it.
/// </summary>
/// <param name="UserName">The signed-in user.</param>
/// <param name="DisplayName">The user's display name.</param>
/// <param name="TokenIssuedAt">When the access token used for this call was issued.</param>
/// <param name="TokenExpiresAt">When the access token used for this call expires.</param>
public record Profile(string UserName, string DisplayName, DateTimeOffset TokenIssuedAt, DateTimeOffset TokenExpiresAt);
