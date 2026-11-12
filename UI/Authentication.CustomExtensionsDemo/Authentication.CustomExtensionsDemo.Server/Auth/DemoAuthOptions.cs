namespace Authentication.CustomExtensionsDemo.Server.Auth;

/// <summary>The <c>DemoAuth</c> section of appsettings.json.</summary>
public sealed class DemoAuthOptions
{
    /// <summary>Access token lifetime. Kept short so the app's 401-then-refresh path is easy to see.</summary>
    public int AccessTokenLifetimeSeconds { get; set; } = 60;

    /// <summary>Demo accounts, by user name.</summary>
    public Dictionary<string, DemoUser> Users { get; set; } = [];
}

/// <summary>A demo account.</summary>
public sealed class DemoUser
{
    public string Password { get; set; } = "";

    public string DisplayName { get; set; } = "";
}
