namespace Authentication.MsalExtensionsDemo.Authentication;

/// <summary>Where a setup step has to be performed.</summary>
public enum SetupArea
{
    /// <summary>In the Microsoft Entra admin center, on the app registration.</summary>
    EntraId,

    /// <summary>In this project's source, configuration or manifests.</summary>
    Project,

    /// <summary>Only matters when actually running the app.</summary>
    Runtime
}

/// <summary>One thing you have to do for a platform to be able to sign in.</summary>
public sealed record SetupStep(SetupArea Area, string Title, string Detail)
{
    public string AreaLabel => Area switch
    {
        SetupArea.EntraId => "ENTRA ID",
        SetupArea.Project => "PROJECT",
        _ => "RUNTIME"
    };
}

/// <summary>What a single platform head needs.</summary>
public sealed record PlatformGuideEntry(
    AppPlatform Platform,
    string Name,
    string RedirectUri,
    string Summary,
    IReadOnlyList<SetupStep> Steps)
{
    /// <summary>True when the app is currently running on this platform.</summary>
    public bool IsCurrent => Platform == PlatformSupport.Current;
}

/// <summary>
/// The per-platform MSAL requirements, as reference material shown inside the app.
/// </summary>
/// <remarks>
/// <para>
/// This is deliberately shared code rather than per-platform: the point is to be able to read
/// what <em>every</em> head needs while running on any one of them.
/// </para>
/// <para>
/// Unlike the plain-MSAL sample (Authentication.MsalDemo), this app uses
/// <c>Uno.Extensions.Authentication.MSAL</c>: the provider derives each platform's conventional
/// redirect URI, persists the token cache where the platform allows it, and is configured from
/// the <c>MsalAuthentication</c> section of appsettings rather than from code.
/// </para>
/// </remarks>
public static class PlatformGuide
{
    /// <summary>
    /// Must match <c>&lt;ApplicationId&gt;</c> in <c>Authentication.MsalExtensionsDemo.csproj</c>.
    /// It is the Android package name and the Apple bundle identifier, and the iOS redirect URI is
    /// derived from it.
    /// </summary>
    public const string ApplicationId = "com.companyname.authentication-msalextensionsdemo";

    /// <summary>Steps that apply no matter which head you run.</summary>
    public static IReadOnlyList<SetupStep> Common { get; } =
    [
        new(SetupArea.EntraId,
            "Register an application",
            """
            Microsoft Entra admin center > Applications > App registrations > New registration.
            Pick supported account types to match the TenantId in configuration ("consumers"
            means personal Microsoft accounts only). Copy the Application (client) ID from
            Overview.
            """),

        new(SetupArea.EntraId,
            "Allow public client flows",
            """
            Authentication > Advanced settings > Allow public client flows > Yes.

            A desktop or mobile app cannot keep a secret, so it must be registered as a public
            client. Without this, sign-in fails with unauthorized_client or invalid_client.
            """),

        new(SetupArea.EntraId,
            "Grant the Microsoft Graph User.Read permission",
            """
            API permissions > Microsoft Graph > Delegated permissions > User.Read.

            New registrations usually have it already. It is what the Microsoft Graph page in this
            app needs to call /me. User.Read is user-consentable, so no admin consent is required.
            """),

        new(SetupArea.Project,
            "Configure the MsalAuthentication section",
            """
            Put the Application (client) ID, TenantId and Scopes in the MsalAuthentication section
            of appsettings.json (or appsettings.development.json). The section name matches the
            name passed to auth.AddMsal(window, name: "MsalAuthentication") in App.xaml.cs.

            No redirect URI is needed in configuration: the provider derives each platform's
            conventional value. Setting "RedirectUri" explicitly overrides that default.
            """)
    ];

    /// <summary>
    /// Per-platform requirements, in the order they are shown. The Android redirect URI depends
    /// on the configured client ID, so the list is built rather than static.
    /// </summary>
    public static IReadOnlyList<PlatformGuideEntry> Build(string? clientId)
    {
        var clientIdOrPlaceholder = string.IsNullOrEmpty(clientId) ? "{ClientId}" : clientId;

        return
        [
            new(AppPlatform.Desktop,
                "Desktop (Skia)",
                "http://localhost",
                "MSAL opens the default system browser and waits for the code on a loopback listener.",
                [
                    new(SetupArea.EntraId,
                        "Register the loopback redirect URI",
                        """
                        Authentication > Add a platform > Mobile and desktop applications, then tick
                        http://localhost (or add it as a custom URI).

                        Any port is accepted for localhost, which matters because MSAL picks a free
                        one at runtime. Registering http://localhost covers all of them.
                        """),

                    new(SetupArea.Project,
                        "Nothing to change",
                        """
                        The provider calls MSAL.NET's WithDefaultRedirectUri() on desktop and MSAL
                        does all of the work: it launches the system browser and collects the code
                        on the loopback listener.
                        """),

                    new(SetupArea.Runtime,
                        "The token cache persists",
                        """
                        The provider wires up MSAL's cache persistence (MsalCacheHelper): DPAPI on
                        Windows, the keychain on macOS, the keyring on Linux. Sign-in state
                        survives an app restart - relaunching goes through the silent path with no
                        prompt. This is a key difference from the plain-MSAL sample, where the
                        desktop cache is in memory only.
                        """),

                    new(SetupArea.Runtime,
                        "Abandoned sign-ins time out",
                        """
                        Closing the system browser is undetectable, so an abandoned interactive
                        sign-in would otherwise wait forever. The provider cancels it after
                        InteractiveTimeout (default 5 minutes; configurable in the
                        MsalAuthentication section, 00:00:00 = wait forever), surfacing the same
                        authentication_canceled error as a user-cancelled sign-in.
                        """)
                ]),

            new(AppPlatform.WebAssembly,
                "WebAssembly",
                "{origin}/authentication-callback",
                "MSAL shows the sign-in UI in a popup opened by Uno's ICustomWebUi.",
                [
                    new(SetupArea.Project,
                        "Nothing to change",
                        """
                        The provider derives the redirect URI from Uno's WebAuthenticationBroker
                        ({origin}/authentication-callback) and applies Uno's WithUnoHelpers(),
                        which supplies the ICustomWebUi that opens the popup with window.open and
                        polls its URL, plus an IMsalHttpClientFactory so MSAL's HTTP calls go
                        through the browser.
                        """),

                    new(SetupArea.EntraId,
                        "Register the redirect URI as a Single-page application",
                        """
                        Authentication > Add a platform > Single-page application, then add the
                        origin and path the app is served from, for example:

                            http://localhost/authentication-callback

                        Entra ID ignores the port for localhost, but the path must match and the
                        platform type matters: only SPA redirect URIs get CORS enabled on the token
                        endpoint. Registered as "Mobile and desktop" or "Web" instead, the
                        browser's token request is rejected (AADSTS90023: cross-origin token
                        redemption is permitted only for the Single-Page Application client-type).
                        """),

                    new(SetupArea.Runtime,
                        "The token cache is in memory",
                        """
                        MSAL's cache persistence relies on APIs that do not exist in the browser,
                        so the provider keeps tokens in memory on WebAssembly (and logs one
                        Information message saying so). A page reload signs out and the next run
                        starts with an interactive sign-in. Not a bug.
                        """),

                    new(SetupArea.Runtime,
                        "The popup must not be blocked",
                        """
                        If the sign-in popup is silently blocked (browser popup blocker, or
                        COOP/COEP response headers on the hosting server), the interactive call
                        never completes. Serve without cross-origin isolation headers, or allow
                        popups for the app's origin.
                        """)
                ]),

            new(AppPlatform.Android,
                "Android",
                $"msal{clientIdOrPlaceholder}://auth",
                "MSAL opens a Chrome custom tab; the result comes back through an intent filter.",
                [
                    new(SetupArea.EntraId,
                        "Register msal{ClientId}://auth",
                        $"""
                        Authentication > Add a platform > Mobile and desktop applications > Custom
                        redirect URI:

                            msal{clientIdOrPlaceholder}://auth

                        Use this form, not the portal's Android platform option. The Android option
                        generates msauth://{ApplicationId}/<signature-hash>, which is the format for
                        broker-based sign-in through Microsoft Authenticator. This sample uses the
                        system browser, so no signing-key hash is involved.
                        """),

                    new(SetupArea.Project,
                        "Point MsalActivity's intent filter at the msal{ClientId} scheme",
                        $"""
                        Platforms/Android/MsalActivity.Android.cs declares an activity deriving
                        from MSAL's BrowserTabActivity. Its DataScheme must be
                        msal{clientIdOrPlaceholder} with host "auth" - without a matching intent
                        filter the browser has nowhere to hand the code and the interactive call
                        never completes.

                        The client ID lives in appsettings, but intent filters are compile-time
                        attributes, so the scheme constant in MsalActivity.Android.cs has to be
                        updated by hand when the client ID changes.
                        """),

                    new(SetupArea.Project,
                        "MainActivity forwards the result",
                        """
                        MainActivity.OnActivityResult calls
                        AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs so
                        MSAL can resume the pending request. Already wired up in this project.
                        """),

                    new(SetupArea.Runtime,
                        "A browser must be installed",
                        """
                        With no browser on the device MSAL throws AndroidActivityNotFound. Browsers
                        without custom tab support (DuckDuckGo, UC Browser) report the flow as
                        cancelled.
                        """)
                ]),

            new(AppPlatform.AppleUIKit,
                "iOS",
                $"msauth.{ApplicationId}://auth",
                "MSAL uses ASWebAuthenticationSession; the result returns through a URL scheme.",
                [
                    new(SetupArea.EntraId,
                        "Register msauth.{BundleId}://auth",
                        $"""
                        Authentication > Add a platform > iOS/macOS and enter the bundle ID
                        {ApplicationId}, which produces:

                            msauth.{ApplicationId}://auth

                        The provider derives the same value from the bundle at runtime. The bundle
                        ID is <ApplicationId> in Authentication.MsalExtensionsDemo.csproj - change
                        one and you must change the other, plus Info.plist.
                        """),

                    new(SetupArea.Project,
                        "Declare the URL scheme in Info.plist",
                        $"""
                        Platforms/iOS/Info.plist must register a CFBundleURLSchemes entry named
                        msauth.{ApplicationId} so iOS routes the callback back into the app.
                        Without it the browser closes and nothing happens.
                        """),

                    new(SetupArea.Project,
                        "Optional: keychain sharing for SSO",
                        """
                        To share the token cache with other apps or with Microsoft Authenticator,
                        set "KeychainSecurityGroup" in the MsalAuthentication configuration section
                        and add the matching keychain-access-groups entitlement. Not needed for
                        this sample.
                        """),

                    new(SetupArea.Runtime,
                        "The token cache persists",
                        """
                        MSAL stores tokens in the iOS keychain, so a restart can go straight
                        through the silent path.
                        """)
                ]),

            new(AppPlatform.Windows,
                "Windows (WinAppSDK)",
                "(broker managed)",
                "Not built by this sample's four heads, but included for completeness.",
                [
                    new(SetupArea.Project,
                        "Add the head, change no code",
                        """
                        Add net10.0-windows10.0.xxxxx.0 to <TargetFrameworks> and build on Windows.
                        On WinAppSDK the provider leaves the redirect URI to the Web Account
                        Manager (WAM) broker rather than deriving one.
                        """),

                    new(SetupArea.EntraId,
                        "Register the broker redirect URI",
                        """
                        For WAM, register
                        ms-appx-web://microsoft.aad.brokerplugin/{ClientId} under Mobile and
                        desktop applications.
                        """)
                ])
        ];
    }
}
