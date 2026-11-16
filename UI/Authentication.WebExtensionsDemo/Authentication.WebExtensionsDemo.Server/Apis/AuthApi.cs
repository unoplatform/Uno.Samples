using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace Authentication.WebExtensionsDemo.Server.Apis;

/// <summary>
/// The browser-facing side of the sign-in, in the shape <c>AddWeb</c> expects: the app opens
/// <c>/auth/login?redirect_uri=...&amp;state=...</c>, the user signs in here, and the server
/// redirects to <c>redirect_uri</c> with the tokens in the URL fragment. <c>/auth/logout</c>
/// ends the browser session and redirects back. <c>/auth/refresh</c> is the app's API for
/// renewing tokens without the browser.
/// </summary>
/// <remarks>
/// The browser session is a cookie, so a second sign-in in the same browser skips the form: the
/// single sign-on a real backend (or the social provider behind it) gives. That is also why
/// sign-out goes through the browser: it is the only place that cookie can be cleared.
/// </remarks>
internal static class AuthApi
{
    internal static WebApplication MapAuthApi(this WebApplication app)
    {
        app.MapGet("/auth/login", ShowLoginAsync);
        app.MapPost("/auth/login", SubmitLoginAsync);
        app.MapGet("/auth/logout", LogoutAsync);
        app.MapPost("/auth/refresh", Refresh);
        return app;
    }

    private static async Task<IResult> ShowLoginAsync(HttpContext context, string? redirect_uri, string? state, IOptions<DemoAuthOptions> options, DemoTokenStore tokens)
    {
        if (!IsAllowed(redirect_uri, options.Value))
        {
            return RejectRedirect(redirect_uri);
        }

        // Already signed in in this browser: no form, straight back to the app.
        var auth = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (auth.Principal?.Identity?.Name is { } userName && options.Value.Users.ContainsKey(userName))
        {
            return RedirectWithTokens(redirect_uri!, state, tokens.Issue(userName));
        }

        return LoginPage(redirect_uri!, state, error: null, options.Value);
    }

    private static async Task<IResult> SubmitLoginAsync(HttpContext context, IOptions<DemoAuthOptions> options, DemoTokenStore tokens)
    {
        // Read directly rather than bound with [FromForm], which would require antiforgery.
        // ponytail: no CSRF token on this demo form; a real login page needs one.
        var form = await context.Request.ReadFormAsync();
        var redirectUri = form["redirect_uri"].ToString();
        var state = form["state"].ToString();
        var userName = form["username"].ToString().Trim().ToLowerInvariant();
        var password = form["password"].ToString();

        if (!IsAllowed(redirectUri, options.Value))
        {
            return RejectRedirect(redirectUri);
        }

        if (!options.Value.Users.TryGetValue(userName, out var user) || user.Password != password)
        {
            return LoginPage(redirectUri, state, "Wrong user name or password.", options.Value);
        }

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, userName)], CookieAuthenticationDefaults.AuthenticationScheme)));

        return RedirectWithTokens(redirectUri, state, tokens.Issue(userName));
    }

    private static async Task<IResult> LogoutAsync(HttpContext context, string? redirect_uri, IOptions<DemoAuthOptions> options)
    {
        if (!IsAllowed(redirect_uri, options.Value))
        {
            return RejectRedirect(redirect_uri);
        }

        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Redirect(redirect_uri!);
    }

    private static IResult Refresh(RefreshRequest request, DemoTokenStore tokens) =>
        tokens.Refresh(request.RefreshToken) is { } response
            ? Results.Ok(response)
            : Results.Unauthorized();

    private static bool IsAllowed(string? redirectUri, DemoAuthOptions options) =>
        !string.IsNullOrEmpty(redirectUri) && options.AllowedRedirectUris.Contains(redirectUri, StringComparer.Ordinal);

    private static IResult RejectRedirect(string? redirectUri) =>
        Results.BadRequest($"redirect_uri '{redirectUri}' is not in DemoAuth:AllowedRedirectUris (appsettings.json of the server).");

    /// <summary>
    /// Tokens go in the fragment, which browsers never send to a server, so they stay out of
    /// server logs. The Web provider reads them from the query or the fragment alike.
    /// </summary>
    private static IResult RedirectWithTokens(string redirectUri, string? state, TokenResponse tokens)
    {
        var fragment = $"access_token={Uri.EscapeDataString(tokens.AccessToken)}"
            + $"&refresh_token={Uri.EscapeDataString(tokens.RefreshToken)}"
            + $"&expires_in={tokens.ExpiresIn}";

        if (!string.IsNullOrEmpty(state))
        {
            fragment += $"&state={Uri.EscapeDataString(state)}";
        }

        return Results.Redirect($"{redirectUri}#{fragment}");
    }

    private static IResult LoginPage(string redirectUri, string? state, string? error, DemoAuthOptions options)
    {
        var html = HtmlEncoder.Default;
        var users = string.Join(", ", options.Users.Select(u => $"<code>{html.Encode(u.Key)}</code> / <code>{html.Encode(u.Value.Password)}</code>"));
        var errorHtml = error is null ? "" : $"""<p class="error">{html.Encode(error)}</p>""";

        return Results.Content(
            $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Sign in - Web auth demo server</title>
              <style>
                body { font-family: system-ui, sans-serif; max-width: 22rem; margin: 3rem auto; padding: 0 1rem; }
                label { display: block; margin-top: 1rem; }
                input { width: 100%; padding: .5rem; font-size: 1rem; box-sizing: border-box; }
                button { margin-top: 1.5rem; width: 100%; padding: .6rem; font-size: 1rem; }
                .error { color: #b00020; }
                .hint { color: #555; font-size: .9rem; }
              </style>
            </head>
            <body>
              <h1>Sign in</h1>
              <p class="hint">Demo backend for Authentication.WebExtensionsDemo. Accounts: {{users}}.</p>
              {{errorHtml}}
              <form method="post" action="/auth/login">
                <input type="hidden" name="redirect_uri" value="{{html.Encode(redirectUri)}}">
                <input type="hidden" name="state" value="{{html.Encode(state ?? "")}}">
                <label>User name <input name="username" autocomplete="username" autofocus></label>
                <label>Password <input name="password" type="password" autocomplete="current-password"></label>
                <button type="submit">Sign in</button>
              </form>
            </body>
            </html>
            """,
            "text/html; charset=utf-8");
    }
}
