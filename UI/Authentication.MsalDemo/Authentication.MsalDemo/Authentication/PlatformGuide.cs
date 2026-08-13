namespace Authentication.MsalDemo.Authentication;

/// <summary>Where a setup step has to be performed.</summary>
internal enum SetupArea
{
    /// <summary>In the Microsoft Entra admin center, on the app registration.</summary>
    EntraId,

    /// <summary>In this project's source or manifests.</summary>
    Project,

    /// <summary>Only matters when actually running the app.</summary>
    Runtime
}

/// <summary>One thing you have to do for a platform to be able to sign in.</summary>
internal sealed record SetupStep(SetupArea Area, string Title, string Detail)
{
    public string AreaLabel => Area switch
    {
        SetupArea.EntraId => "ENTRA ID",
        SetupArea.Project => "PROJECT",
        _ => "RUNTIME"
    };
}

/// <summary>What a single platform head needs.</summary>
internal sealed record PlatformGuideEntry(
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
/// This is deliberately shared code rather than per-platform: the point is to be able to read
/// what <em>every</em> head needs while running on any one of them.
/// </remarks>
internal static class PlatformGuide
{
    /// <summary>
    /// Must match <c>&lt;ApplicationId&gt;</c> in <c>Authentication.MsalDemo.csproj</c>. It is the Android package
    /// name and the Apple bundle identifier, and the iOS redirect URI is derived from it.
    /// </summary>
    public const string ApplicationId = "com.companyname.authentication.msaldemo";

    /// <summary>Steps that apply no matter which head you run.</summary>
    public static IReadOnlyList<SetupStep> Common { get; } =
    [
        new(SetupArea.EntraId,
            "Register an application",
            """
            Microsoft Entra admin center > Applications > App registrations > New registration.
            Pick supported account types to match MsalConfig.Tenant ("common" means work/school
            plus personal Microsoft accounts). Copy the Application (client) ID from Overview.
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
            "Set the client ID",
            """
            Put the Application (client) ID in Authentication/MsalConfig.cs and set Tenant to
            "common", "organizations", "consumers" or your tenant ID.

            Everything else - including the Android intent filter - is derived from that constant.
            """)
    ];

    /// <summary>Per-platform requirements, in the order they are shown.</summary>
    public static IReadOnlyList<PlatformGuideEntry> All { get; } =
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
                    There is no #if branch for this head, and .WithUnoHelpers() is a no-op in the
                    skia build of Uno.UI.MSAL that it loads. MSAL.NET does all of the work: it
                    launches the system browser and collects the code on the loopback listener.
                    """),

                new(SetupArea.Runtime,
                    "The token cache is in memory",
                    """
                    Tokens are lost when the app exits, so every run starts with an interactive
                    sign-in. For a persistent cache on desktop, add the
                    Microsoft.Identity.Client.Extensions.Msal package and register a
                    MsalCacheHelper against the application's UserTokenCache.
                    """)
            ]),

        new(AppPlatform.WebAssembly,
            "WebAssembly",
            "{origin}" + MsalConfig.WasmRedirectPath,
            "Interactive sign-in does not work on this head with the Skia renderer.",
            [
                new(SetupArea.Runtime,
                    "Interactive sign-in cannot complete here",
                    """
                    Uno.WinUI.MSAL ships one Uno.UI.MSAL.dll per runtime flavour. The popup web UI -
                    an ICustomWebUi that opens a window with window.open and polls its URL - exists
                    only in the "webassembly" flavour. Because this app enables the SkiaRenderer
                    feature, the browserwasm head loads the "skia" flavour instead, in which
                    .WithUnoHelpers() is a no-op.

                    With no ICustomWebUi registered, MSAL falls back to its default web UI, which
                    needs to launch a browser process and listen on a loopback port. Neither is
                    possible inside the browser sandbox.

                    The silent path, cached accounts, sign-out and the Graph call are unaffected -
                    only AcquireTokenInteractive is. MSAL-SETUP.md lists the options for restoring a
                    browser flow; none of them is implemented in this sample.
                    """),

                new(SetupArea.EntraId,
                    "Register the redirect URI as a Single-page application",
                    $"""
                    Needed for whenever this head can sign in again - it is a fact about Entra ID,
                    not about the renderer.

                    Authentication > Add a platform > Single-page application, then add the exact
                    origin and path the app is served from, for example:

                        http://localhost:5000{MsalConfig.WasmRedirectPath}

                    This platform type matters: only SPA redirect URIs get CORS enabled on the
                    token endpoint. Registered as "Mobile and desktop" instead, the browser's token
                    request is rejected with "cross-origin token redemption is permitted only for
                    the 'Single-Page Application' client-type".

                    Register one URI per origin you serve from (dev port, staging, production).
                    """),

                new(SetupArea.EntraId,
                    "Match protocol, host and port exactly",
                    """
                    The redirect URI must be on the same origin as the app. Only the path is free.
                    A different port is a different origin, and is the single most common cause of
                    an AADSTS50011 redirect URI mismatch here.
                    """),

                new(SetupArea.Project,
                    "Keep the callback page",
                    """
                    Platforms/WebAssembly/wwwroot/authentication/login-callback.htm is static
                    content that a sign-in popup would land on. Nothing opens or polls a popup under
                    the Skia renderer, so it is vestigial today - but it is still what the
                    registered redirect URI points at, so keep it if you intend to restore a
                    browser flow.
                    """),

                new(SetupArea.Runtime,
                    "The token cache is in memory",
                    """
                    Uno does not persist MSAL's cache in the browser, so a page reload signs out.
                    Moot in practice while interactive sign-in cannot run here: nothing ever
                    populates the cache.
                    """)
            ]),

        new(AppPlatform.Android,
            "Android",
            $"msal{MsalConfig.ClientId}://auth",
            "MSAL opens a Chrome custom tab; the result comes back through an intent filter.",
            [
                new(SetupArea.EntraId,
                    "Register msal{ClientId}://auth",
                    $"""
                    Authentication > Add a platform > Mobile and desktop applications > Custom
                    redirect URI:

                        msal{MsalConfig.ClientId}://auth

                    Use this form, not the portal's Android platform option. The Android option
                    generates msauth://{ApplicationId}/<signature-hash>, which is the format for
                    broker-based sign-in through Microsoft Authenticator. This sample uses the
                    system browser, so no signing-key hash is involved.
                    """),

                new(SetupArea.Project,
                    "MsalActivity catches the redirect",
                    """
                    Platforms/Android/MsalActivity.Android.cs declares an activity deriving from
                    MSAL's BrowserTabActivity with an intent filter for the msal{ClientId} scheme
                    and host "auth". Without a matching intent filter the browser has nowhere to
                    hand the code and the interactive call never completes.

                    The scheme is built from MsalConfig.ClientId at compile time, so changing the
                    client ID is enough - the manifest follows.
                    """),

                new(SetupArea.Project,
                    "MainActivity forwards the result",
                    """
                    MainActivity.OnActivityResult calls
                    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs so MSAL
                    can resume the pending request.
                    """),

                new(SetupArea.Project,
                    "The parent activity is set explicitly",
                    """
                    AuthenticationService sets WithParentActivityOrWindow on the builder under
                    #if ANDROID, resolved from ContextHelper.Current. Calling
                    AcquireTokenInteractive without a parent activity crashes on Android.

                    .WithUnoHelpers() supplies the same value from the same source on this head, so
                    the explicit call duplicates it rather than replacing anything - it only removes
                    the dependency on which flavour of Uno.UI.MSAL the build loaded.
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
            "iOS and Mac Catalyst",
            $"msauth.{ApplicationId}://auth",
            "MSAL uses ASWebAuthenticationSession; the result returns through a URL scheme.",
            [
                new(SetupArea.EntraId,
                    "Register msauth.{BundleId}://auth",
                    $"""
                    Authentication > Add a platform > iOS/macOS and enter the bundle ID
                    {ApplicationId}, which produces:

                        msauth.{ApplicationId}://auth

                    The bundle ID is <ApplicationId> in Authentication.MsalDemo.csproj. Change one and you must
                    change the other, plus Info.plist.
                    """),

                new(SetupArea.Project,
                    "Declare the URL scheme in Info.plist",
                    $"""
                    Platforms/iOS/Info.plist registers CFBundleURLTypes with the scheme
                    msauth.{ApplicationId} so iOS routes the callback back into the app. Without
                    it the browser closes and nothing happens.
                    """),

                new(SetupArea.Project,
                    "The parent view controller is set explicitly",
                    """
                    AuthenticationService sets WithParentActivityOrWindow on the builder under
                    #if IOS, resolved from the key window's RootViewController - the same value
                    .WithUnoHelpers() supplies on this head.
                    """),

                new(SetupArea.Project,
                    "Handle OpenUrl in the app delegate",
                    """
                    Platforms/iOS/MsalAppDelegate.iOS.cs derives from Uno's
                    UnoUIApplicationDelegate and forwards OpenUrl to
                    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs.
                    Main.iOS.cs installs it with .UseAppleUIKit(b => b.UseUIApplicationDelegate<...>()).
                    """),

                new(SetupArea.Project,
                    "Optional: keychain sharing for SSO",
                    """
                    To share the token cache with other apps or with Microsoft Authenticator, add
                    keychain-access-groups with $(AppIdentifierPrefix)com.microsoft.adalcache to
                    Entitlements.plist and call .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                    on the builder. The two must match, and the entitlement requires a provisioning
                    profile with keychain sharing enabled. Not needed for this sample.
                    """),

                new(SetupArea.Runtime,
                    "The token cache persists",
                    """
                    MSAL stores tokens in the iOS keychain, so a restart can go straight through
                    the silent path - unlike Desktop and WebAssembly.
                    """)
            ]),

        new(AppPlatform.Windows,
            "Windows (WinAppSDK)",
            "http://localhost",
            "Not built by this sample's four heads, but included for completeness.",
            [
                new(SetupArea.EntraId,
                    "Register the loopback redirect URI",
                    """
                    Authentication > Add a platform > Mobile and desktop applications >
                    http://localhost, the same as the Desktop head.
                    """),

                new(SetupArea.Project,
                    "Add the head, change no code",
                    """
                    Add net10.0-windows10.0.19041.0 to <TargetFrameworks> and build on Windows.
                    There is no #if branch for this head and .WithUnoHelpers() is a no-op on
                    WinAppSDK, so the authentication code in this sample is unchanged. Like Desktop,
                    it uses the system browser and the loopback listener.
                    """),

                new(SetupArea.Project,
                    "Optional: WAM broker",
                    """
                    For Windows Web Account Manager single sign-on, add the
                    Microsoft.Identity.Client.Broker package, call
                    .WithBroker(new BrokerOptions(BrokerOptions.OperatingSystems.Windows)) and
                    register the redirect URI
                    ms-appx-web://microsoft.aad.brokerplugin/{ClientId} instead.
                    """)
            ])
    ];
}
