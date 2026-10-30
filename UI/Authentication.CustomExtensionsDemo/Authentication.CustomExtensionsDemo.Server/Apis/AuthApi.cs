using Microsoft.Extensions.Options;

namespace Authentication.CustomExtensionsDemo.Server.Apis;

/// <summary>
/// The sign-in API <c>AddCustom</c> talks to: the app posts the user name and password it
/// collected itself and gets tokens back, renews them with the refresh token, and revokes the
/// refresh token on sign-out.
/// </summary>
internal static class AuthApi
{
    private const string Tag = "Auth";

    internal static WebApplication MapAuthApi(this WebApplication app)
    {
        app.MapPost("/auth/login", Login).WithTags(Tag).WithName(nameof(Login));
        app.MapPost("/auth/refresh", Refresh).WithTags(Tag).WithName(nameof(Refresh));
        app.MapPost("/auth/logout", Logout).WithTags(Tag).WithName(nameof(Logout));
        return app;
    }

    /// <summary>Signs a user in with a user name and password.</summary>
    /// <response code="200">The credentials are valid; the tokens to use.</response>
    /// <response code="401">Wrong user name or password.</response>
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    private static IResult Login(LoginRequest request, IOptions<DemoAuthOptions> options, DemoTokenStore tokens)
    {
        var userName = request.UserName.Trim().ToLowerInvariant();
        return options.Value.Users.TryGetValue(userName, out var user) && user.Password == request.Password
            ? Results.Ok(tokens.Issue(userName))
            : Results.Unauthorized();
    }

    /// <summary>Trades a refresh token for new tokens. The refresh token rotates.</summary>
    /// <response code="200">The new tokens.</response>
    /// <response code="401">The refresh token is unknown, spent or revoked.</response>
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    private static IResult Refresh(RefreshRequest request, DemoTokenStore tokens) =>
        tokens.Refresh(request.RefreshToken) is { } response
            ? Results.Ok(response)
            : Results.Unauthorized();

    /// <summary>Revokes a refresh token.</summary>
    /// <response code="204">Revoked, or already unknown.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    private static IResult Logout(RefreshRequest request, DemoTokenStore tokens)
    {
        tokens.Revoke(request.RefreshToken);
        return Results.NoContent();
    }
}
