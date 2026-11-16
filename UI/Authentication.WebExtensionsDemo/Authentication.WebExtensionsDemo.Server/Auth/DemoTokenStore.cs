using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace Authentication.WebExtensionsDemo.Server.Auth;

/// <summary>An access token the server issued: whose it is and how long it lives.</summary>
public sealed record TokenSession(string UserName, DateTimeOffset IssuedAt, DateTimeOffset ExpiresAt);

/// <summary>
/// Issues opaque random tokens and remembers them in memory. Restarting the server forgets every
/// token, which is a handy way to watch the app's refresh fail and its session end.
/// </summary>
/// <remarks>
/// ponytail: in memory and opaque on purpose; a real backend issues signed tokens or keeps
/// sessions in a database, so they survive restarts and scale out.
/// </remarks>
public sealed class DemoTokenStore(IOptions<DemoAuthOptions> options, TimeProvider time)
{
    private readonly ConcurrentDictionary<string, TokenSession> _accessTokens = new();
    private readonly ConcurrentDictionary<string, string> _refreshTokens = new();

    public TokenResponse Issue(string userName)
    {
        var now = time.GetUtcNow();
        var lifetime = options.Value.AccessTokenLifetimeSeconds;

        var accessToken = RandomNumberGenerator.GetHexString(64, lowercase: true);
        var refreshToken = RandomNumberGenerator.GetHexString(64, lowercase: true);

        _accessTokens[accessToken] = new(userName, now, now.AddSeconds(lifetime));
        _refreshTokens[refreshToken] = userName;

        return new(accessToken, refreshToken, lifetime);
    }

    /// <summary>
    /// Spends <paramref name="refreshToken"/> and issues a new pair, or returns <c>null</c> when the
    /// token is unknown or already spent.
    /// </summary>
    public TokenResponse? Refresh(string refreshToken) =>
        _refreshTokens.TryRemove(refreshToken, out var userName) ? Issue(userName) : null;

    /// <summary>The session behind a live access token, or <c>null</c> when it is unknown or expired.</summary>
    public TokenSession? Validate(string accessToken) =>
        _accessTokens.TryGetValue(accessToken, out var session) && session.ExpiresAt > time.GetUtcNow()
            ? session
            : null;
}
