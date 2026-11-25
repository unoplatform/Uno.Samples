using Microsoft.Extensions.Options;

namespace Authentication.CustomExtensionsDemo.Server.Apis;

/// <summary>
/// The protected API: <c>GET /api/profile</c> answers 401 without a live access token, and says
/// whose token it was otherwise.
/// </summary>
internal static class ProfileApi
{
    internal static WebApplication MapProfileApi(this WebApplication app)
    {
        app.MapGet("/api/profile", GetProfile).WithTags("Profile").WithName(nameof(GetProfile));
        return app;
    }

    /// <summary>Who the access token sent with this call belongs to.</summary>
    /// <response code="200">The token is live.</response>
    /// <response code="401">No token, or an unknown or expired one.</response>
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    private static IResult GetProfile(HttpContext context, DemoTokenStore tokens, IOptions<DemoAuthOptions> options)
    {
        var header = context.Request.Headers.Authorization.ToString();
        var session = header.StartsWith("Bearer ", StringComparison.Ordinal)
            ? tokens.Validate(header["Bearer ".Length..].Trim())
            : null;

        if (session is null)
        {
            context.Response.Headers.WWWAuthenticate = "Bearer";
            return Results.Unauthorized();
        }

        var displayName = options.Value.Users.TryGetValue(session.UserName, out var user) ? user.DisplayName : session.UserName;
        return Results.Ok(new Profile(session.UserName, displayName, session.IssuedAt, session.ExpiresAt));
    }
}
