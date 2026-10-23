using System.Diagnostics;
using System.Net;
using System.Text;
using Duende.IdentityModel.OidcClient.Browser;

namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>
/// The browser the OIDC provider uses on the Skia desktop head: opens the sign-in page in the
/// default browser and receives the redirect on a loopback HTTP listener.
/// </summary>
/// <remarks>
/// <para>
/// Uno's <c>WebAuthenticationBroker</c>, which the provider's default <c>IBrowser</c> relies on,
/// is not implemented on the Skia desktop targets, so a desktop app has to bring its own. This
/// is the standard native-app pattern: the redirect URI is <c>http://127.0.0.1:{port}/...</c>,
/// the app listens on it for the one request the identity provider redirects to, answers with
/// a small "you can close this tab" page, and hands the full URL back to OidcClient, which
/// validates the state and exchanges the code.
/// </para>
/// <para>
/// The listener binds to the port of <see cref="BrowserOptions.EndUrl"/>, which is the
/// <c>RedirectUri</c> (or <c>PostLogoutRedirectUri</c>) the provider was configured with, so
/// the value in configuration is the one the identity provider must know. Registered in
/// <c>App.xaml.cs</c> with <c>services.AddTransient&lt;IBrowser, SystemBrowser&gt;()</c>.
/// </para>
/// </remarks>
public sealed class SystemBrowser(ILogger<SystemBrowser> logger, Uno.Extensions.Authentication.ITokenCache tokens) : IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
    {
        var startUrl = await WithIdTokenHintAsync(options.StartUrl, cancellationToken);

        if (!Uri.TryCreate(options.EndUrl, UriKind.Absolute, out var endUrl) || !endUrl.IsLoopback)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = $"The redirect URI must be a loopback http://127.0.0.1:port/... address on desktop; got '{options.EndUrl}'."
            };
        }

        // HttpListener wants a prefix that ends with a slash and covers the callback path.
        var prefix = $"{endUrl.Scheme}://{endUrl.Host}:{endUrl.Port}{endUrl.AbsolutePath.TrimEnd('/')}/";

        using var listener = new HttpListener();
        listener.Prefixes.Add(prefix);

        try
        {
            listener.Start();
        }
        catch (HttpListenerException ex)
        {
            return new BrowserResult
            {
                ResultType = BrowserResultType.UnknownError,
                Error = $"Could not listen on {prefix}: {ex.Message}. Is another instance of the app waiting for a sign-in?"
            };
        }

        // The authorization URL carries the state and the PKCE challenge, nothing secret; logging it
        // makes a redirect-URI mismatch diagnosable from the console.
        logger.LogInformation("Opening the default browser on {StartUrl} and listening on {Prefix}", startUrl, prefix);
        OpenInDefaultBrowser(startUrl);

        using var timeout = new CancellationTokenSource(options.Timeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);

        try
        {
            var context = await listener.GetContextAsync().WaitAsync(linked.Token);

            // Hand the browser something to show, then give OidcClient the URL it redirected to,
            // which carries the code and state (or the error) as query parameters.
            await RespondAsync(context.Response, "You are signed in. You can close this tab and return to the app.");

            return new BrowserResult
            {
                ResultType = BrowserResultType.Success,
                Response = context.Request.Url?.OriginalString ?? string.Empty
            };
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        {
            return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = "No redirect arrived before the browser timeout." };
        }
        catch (OperationCanceledException)
        {
            return new BrowserResult { ResultType = BrowserResultType.UserCancel };
        }
        finally
        {
            listener.Stop();
        }
    }

    /// <summary>
    /// Adds <c>id_token_hint</c> to an end-session URL that lacks it. The OIDC provider signs out
    /// with a bare <c>LogoutRequest</c>, so the URL it hands the browser carries only
    /// <c>post_logout_redirect_uri</c>; an identity provider will not redirect back to an
    /// unverified client, the browser stays on its "logged out" page, and <c>LogoutAsync</c>
    /// waits for the whole browser timeout. With the hint the identity provider can verify the
    /// client and redirects straight to the loopback listener. The cached ID token is exactly
    /// the value the hint expects.
    /// </summary>
    private async Task<string> WithIdTokenHintAsync(string startUrl, CancellationToken ct)
    {
        // An end-session request is the one that carries post_logout_redirect_uri; the endpoint's
        // path differs per identity provider.
        if (!startUrl.Contains("post_logout_redirect_uri=", StringComparison.OrdinalIgnoreCase)
            || startUrl.Contains("id_token_hint=", StringComparison.OrdinalIgnoreCase))
        {
            return startUrl;
        }

        var idToken = await tokens.TokenAsync(Uno.Extensions.Authentication.TokenCacheExtensions.IdTokenKey, ct);
        if (string.IsNullOrEmpty(idToken))
        {
            return startUrl;
        }

        var separator = startUrl.Contains('?') ? "&" : "?";
        return $"{startUrl}{separator}id_token_hint={Uri.EscapeDataString(idToken)}";
    }

    private static void OpenInDefaultBrowser(string url)
    {
        // UseShellExecute hands the URL to the OS, which opens the user's default browser on
        // Windows, macOS and Linux alike.
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    private static async Task RespondAsync(HttpListenerResponse response, string message)
    {
        var html = $"<!doctype html><html><head><meta charset=\"utf-8\"><title>Authentication.OidcExtensionsDemo</title></head>"
            + $"<body style=\"font-family: sans-serif; padding: 2rem\"><p>{WebUtility.HtmlEncode(message)}</p></body></html>";
        var bytes = Encoding.UTF8.GetBytes(html);

        response.ContentType = "text/html; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        await response.OutputStream.WriteAsync(bytes);
        response.Close();
    }
}
