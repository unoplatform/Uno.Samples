namespace Authentication.MsalDemo.Authentication;

/// <summary>
/// The one file you need to edit to point this sample at your own Microsoft Entra ID
/// (formerly Azure AD) app registration.
/// </summary>
/// <remarks>
/// Everything else in the app derives from these values, including the Android intent filter
/// (see <c>Platforms/Android/MsalActivity.Android.cs</c>), so there is a single source of truth
/// for the client ID.
/// </remarks>
internal static class MsalConfig
{
    /// <summary>
    /// The value <see cref="ClientId"/> ships with. Used to detect an unconfigured app.
    /// </summary>
    public const string UnconfiguredClientId = "00000000-0000-0000-0000-000000000000";

    /// <summary>
    /// Application (client) ID of your app registration, from the Entra admin center
    /// (Overview page of the registration).
    /// </summary>
    /// <remarks>
    /// This must be a public client / native app registration with "Allow public client flows"
    /// enabled, and it needs one registered redirect URI per platform you intend to run.
    /// See <c>MSAL-SETUP.md</c> for the exact list.
    /// </remarks>
    public const string ClientId = "00000000-0000-0000-0000-000000000000";

    /// <summary>
    /// Directory (tenant) the app signs users in to. Use:
    /// <list type="bullet">
    /// <item><description><c>common</c> - work/school accounts and personal Microsoft accounts</description></item>
    /// <item><description><c>organizations</c> - any work/school account</description></item>
    /// <item><description><c>consumers</c> - personal Microsoft accounts only</description></item>
    /// <item><description>a tenant ID or domain (e.g. <c>contoso.onmicrosoft.com</c>) - single tenant</description></item>
    /// </list>
    /// The value must match the "Supported account types" of your registration.
    /// </summary>
    public const string Tenant = "common";

    /// <summary>
    /// Scopes requested at sign-in. <c>User.Read</c> is what the Microsoft Graph page needs to
    /// call <c>/me</c>. Fully-qualified scope URIs are used so the target resource is unambiguous.
    /// </summary>
    public static readonly string[] Scopes =
    [
        "https://graph.microsoft.com/User.Read"
    ];

    /// <summary>
    /// URL scheme MSAL uses to hand the authorization code back to the app on Android.
    /// MSAL.NET requires exactly the form <c>msal{ClientId}</c>.
    /// </summary>
    /// <remarks>
    /// This is a <c>const</c> because Android intent filters are declared with attributes, which
    /// only accept compile-time constants.
    /// </remarks>
    public const string AndroidRedirectScheme = "msal" + ClientId;

    /// <summary>
    /// Path of the static callback page a browser sign-in popup would land on for WebAssembly. It
    /// has to be a real file served from the app's own origin - see
    /// <c>wwwroot/authentication/login-callback.htm</c>.
    /// </summary>
    /// <remarks>
    /// The popup Uno opens for MSAL lands here, and Uno polls the popup's URL until it matches.
    /// The page itself does nothing - it only has to exist and be served from this origin.
    /// </remarks>
    public const string WasmRedirectPath = "/authentication/login-callback.htm";

    /// <summary>
    /// Whether <see cref="ClientId"/> has been replaced with a real one.
    /// </summary>
    public static bool IsConfigured =>
        !string.Equals(ClientId, UnconfiguredClientId, StringComparison.OrdinalIgnoreCase)
        && Guid.TryParse(ClientId, out _);
}
