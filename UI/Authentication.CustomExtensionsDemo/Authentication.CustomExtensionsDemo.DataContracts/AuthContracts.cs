namespace Authentication.CustomExtensionsDemo.DataContracts;

/// <summary>The body the app posts to <c>POST /auth/login</c>.</summary>
/// <param name="UserName">The user name typed on the login page.</param>
/// <param name="Password">The password typed on the login page. Sent once, never stored.</param>
public record LoginRequest(string UserName, string Password);

/// <summary>
/// The body the app posts to <c>POST /auth/refresh</c> to trade a refresh token for new tokens,
/// and to <c>POST /auth/logout</c> to revoke it.
/// </summary>
/// <param name="RefreshToken">The refresh token the server issued with the last sign-in or refresh.</param>
public record RefreshRequest(string RefreshToken);

/// <summary>
/// What <c>POST /auth/login</c> and <c>POST /auth/refresh</c> return. The refresh token rotates:
/// the one sent to <c>/auth/refresh</c> is spent.
/// </summary>
/// <param name="AccessToken">The access token.</param>
/// <param name="RefreshToken">The refresh token.</param>
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
