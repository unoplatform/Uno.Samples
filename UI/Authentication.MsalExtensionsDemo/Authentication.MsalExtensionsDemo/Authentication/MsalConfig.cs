namespace Authentication.MsalExtensionsDemo.Authentication;

internal static class MsalConfig
{
    public const string ClientId = "00000000-0000-0000-0000-000000000000";

    public const string AndroidRedirectScheme = "msal" + ClientId;

    public const string IosRedirectUri = "msauth.com.companyname.authentication.msalextensionsdemo://auth";

    public const string DesktopRedirectUri = "http://localhost";

    public const string WasmRedirectPath = "/authentication/login-callback.htm";
}
