namespace QuoteCraft.Data.Remote;

/// <summary>
/// Supabase project configuration.
/// Replace these values with your actual Supabase project credentials.
/// </summary>
public static class SupabaseConfig
{
    // For production, move these values to appsettings.json or environment variables.
    public const string Url = "https://YOUR_PROJECT.supabase.co";
    public const string AnonKey = "YOUR_ANON_KEY";
    public const string ServiceRoleKey = "YOUR_SERVICE_ROLE_KEY"; // Server-side only
}
