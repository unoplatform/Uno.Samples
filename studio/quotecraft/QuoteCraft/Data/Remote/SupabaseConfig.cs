namespace QuoteCraft.Data.Remote;

/// <summary>
/// The "Supabase" section of appsettings.json. Only the project URL and the anon (public) key belong in the app:
/// row-level security protects the data. The service-role key bypasses it and stays on the server (supabase/).
/// </summary>
public record SupabaseOptions
{
    public string? Url { get; init; }
    public string? AnonKey { get; init; }

    /// <summary>The Stripe customer portal link shown by "Manage Subscription".</summary>
    public string? CustomerPortalUrl { get; init; }
}

/// <summary>
/// The Supabase project this build talks to, if any. QuoteCraft is local-first: with no URL and anon key it runs on
/// the local database alone, and sign-in, sync and billing stay hidden (docs/DECISIONS-production-readiness.md, D3).
/// </summary>
public sealed class SupabaseConfig
{
    /// <summary>Environment variables that override appsettings.json, for local runs and CI.</summary>
    public const string UrlVariable = "QUOTECRAFT_SUPABASE_URL";
    public const string AnonKeyVariable = "QUOTECRAFT_SUPABASE_ANON_KEY";

    public SupabaseConfig(IOptions<SupabaseOptions> options)
        : this(
            Environment.GetEnvironmentVariable(UrlVariable) is { Length: > 0 } url ? url : options.Value.Url,
            Environment.GetEnvironmentVariable(AnonKeyVariable) is { Length: > 0 } key ? key : options.Value.AnonKey,
            options.Value.CustomerPortalUrl)
    {
    }

    public SupabaseConfig(string? url, string? anonKey, string? customerPortalUrl = null)
    {
        IsConfigured = Uri.TryCreate(url?.Trim(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp)
            && !string.IsNullOrWhiteSpace(anonKey);
        Url = IsConfigured && uri is not null ? uri.GetLeftPart(UriPartial.Authority) : string.Empty;
        AnonKey = IsConfigured ? anonKey?.Trim() ?? string.Empty : string.Empty;
        CustomerPortalUrl = IsConfigured
            && Uri.TryCreate(customerPortalUrl?.Trim(), UriKind.Absolute, out var portal)
            && portal.Scheme == Uri.UriSchemeHttps
                ? portal.ToString()
                : null;
    }

    /// <summary>True when a Supabase URL and anon key are set: sign-in, sync and billing are available.</summary>
    public bool IsConfigured { get; }

    /// <summary>The project's base URL (scheme and authority), or empty when not configured.</summary>
    public string Url { get; }

    public string AnonKey { get; }

    public string? CustomerPortalUrl { get; }

    public const string NotConfiguredMessage =
        "Cloud sync is not set up in this build: QuoteCraft keeps your data on this device.";

    public void EnsureConfigured()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(NotConfiguredMessage);
        }
    }
}
