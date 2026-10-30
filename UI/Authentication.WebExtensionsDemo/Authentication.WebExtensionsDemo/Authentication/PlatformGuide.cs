namespace Authentication.WebExtensionsDemo.Authentication;

/// <summary>Where a setup step has to be performed.</summary>
public enum SetupArea
{
    /// <summary>On the backend that runs the login page.</summary>
    Backend,

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
        SetupArea.Backend => "BACKEND",
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
/// The per-platform Web authentication requirements, as reference material shown inside the app.
/// </summary>
/// <remarks>
/// Deliberately shared code rather than per-platform: the point is to be able to read what
/// <em>every</em> head needs while running on any one of them. The sample ships its own backend,
/// <c>Authentication.WebExtensionsDemo.Server</c>, already set up for every callback below.
/// </remarks>
public static class PlatformGuide
{
    /// <summary>
    /// Must match <c>&lt;ApplicationId&gt;</c> in <c>Authentication.WebExtensionsDemo.csproj</c>.
    /// It is the Android package name and the Apple bundle identifier.
    /// </summary>
    public const string ApplicationId = "com.companyname.authentication-webextensionsdemo";

    /// <summary>The custom scheme the mobile heads use for the callback.</summary>
    public const string MobileScheme = "webdemo";

    /// <summary>The callback of the Android and iOS heads.</summary>
    public const string MobileCallbackUri = $"{MobileScheme}://callback";

    /// <summary>
    /// The loopback callback the desktop head listens on. The path is the one Uno's
    /// WebAuthenticationBroker uses by default; the port is fixed so the backend can allow it
    /// once (and differs from the OIDC sample's 52001, so both can run side by side).
    /// </summary>
    public const string DesktopCallbackUri = "http://localhost:52002/authentication-callback";

    /// <summary>The demo server as the development machine, the iOS simulator and desktop reach it.</summary>
    public const string LocalServer = "http://localhost:5001";

    /// <summary>The demo server as the Android emulator reaches it.</summary>
    public const string AndroidEmulatorServer = "http://10.0.2.2:5001";

    /// <summary>Steps that apply no matter which head you run.</summary>
    public static IReadOnlyList<SetupStep> Common { get; } =
    [
        new(SetupArea.Backend,
            "Redirect back with the tokens in the URL",
            """
            The backend's login page signs the user in (its own form, or a social provider behind
            it) and redirects to the redirect_uri it was given with access_token and, for a
            session that survives restarts, refresh_token in the query or the fragment. This
            sample's server puts them in the fragment, with the state the app sent.
            """),

        new(SetupArea.Backend,
            "Allow each callback exactly",
            """
            A login page that redirects tokens to any redirect_uri hands them to whoever wrote the
            link. The demo server only redirects to the URIs in DemoAuth:AllowedRedirectUris of
            its appsettings.json, compared exactly; one per head (below).
            """),

        new(SetupArea.Project,
            "Configure the WebAuthentication section",
            """
            LoginStartUri and LogoutStartUri, each with its redirect_uri, in the
            WebAuthentication section of appsettings.development.json (Debug builds) and
            appsettings.json (Release). The section name matches the name passed to
            auth.AddWeb<IAuthApi>(name: "WebAuthentication") in App.xaml.cs. With no
            LoginCallbackUri, the provider waits for the redirect_uri of LoginStartUri. Without
            LogoutStartUri, LogoutAsync returns false and keeps the tokens.
            """),

        new(SetupArea.Project,
            "Refresh is yours to write",
            """
            The Web provider has no refresh of its own: without a Refresh callback, RefreshAsync
            reports success while any token is stored and the session only ends on an API 401.
            This sample's callback posts the refresh token to /auth/refresh through IAuthApi,
            whose method carries the no-refresh header so a 401 there cannot loop.
            """),

        new(SetupArea.Project,
            "Keep the client registrations in UseHttp",
            """
            The access token reaches an API only through clients registered with
            AddRefitClient, AddKiotaClient or AddClient inside UseHttp - that is where the
            authentication handler is chained in. A plain HttpClient sends no token and never
            triggers the 401 refresh.
            """)
    ];

    /// <summary>Per-platform requirements, in the order they are shown.</summary>
    public static IReadOnlyList<PlatformGuideEntry> Build() =>
    [
        new(AppPlatform.Desktop,
            "Desktop (Skia)",
            DesktopCallbackUri,
            "The default browser opens the login page; the loopback broker AddWeb registers receives the redirect.",
            [
                new(SetupArea.Project,
                    "Uno.Extensions 8.0.0-dev.135 or later brings the broker",
                    """
                    Uno's own WebAuthenticationBroker has no implementation on the Skia desktop
                    head. Since Uno.Extensions 8.0.0-dev.135 (pinned by Uno.Sdk 7.0.0-dev.95 and
                    later), AddWeb registers one: DesktopWebAuthenticationBrokerProvider opens the
                    system browser and waits on a loopback HTTP listener. The registration is
                    first-wins and the broker keeps the provider it first resolved, so do not
                    touch WebAuthenticationBroker before UseAuthentication has run.
                    """),

                new(SetupArea.Project,
                    "Use a loopback callback on this head",
                    $"""
                    Platforms/Desktop/PlatformSupport.Desktop.cs points redirect_uri at
                    {DesktopCallbackUri}: a custom scheme is not routed back to a desktop app, and
                    the broker rejects anything but a loopback HTTP address. The port is fixed so
                    the backend can allow it once.
                    """),

                new(SetupArea.Project,
                    "Bind the callback to the sign-in with state",
                    """
                    The listener accepts the first request to its path from any local page or
                    process, and the Web provider does not tie it to the flow it started. The app
                    adds a state in PrepareLoginStartUri and refuses tokens without it in
                    PostLogin; only AddWeb<TService>'s PostLogin receives the callback URL.
                    """),

                new(SetupArea.Backend,
                    "Allow the loopback URI",
                    $"""
                    Already in the demo server's AllowedRedirectUris: {DesktopCallbackUri}.
                    """)
            ]),

        new(AppPlatform.WebAssembly,
            "WebAssembly",
            "{origin}/authentication-callback",
            "The login page opens in a popup; the callback must be on the app's own origin.",
            [
                new(SetupArea.Project,
                    "Set redirect_uri to the broker's callback",
                    """
                    The Web provider sends the configured redirect_uri on WebAssembly too; it does
                    not switch to the broker's callback the way the OIDC provider does.
                    Platforms/WebAssembly/PlatformSupport.Wasm.cs sets it to
                    WebAuthenticationBroker.GetCurrentApplicationCallbackUri(), the origin plus
                    /authentication-callback. Uno reads the tokens from the popup's URL once it
                    lands there, so the host must serve index.html for that path.
                    """),

                new(SetupArea.Backend,
                    "Allow the origin's callback",
                    """
                    Run this head through the server project: it hosts the app at
                    http://localhost:5001, so the login page, the API and the callback share one
                    origin and no CORS is needed. http://localhost:5001/authentication-callback is
                    in AllowedRedirectUris; add one entry per origin you deploy to.
                    """),

                new(SetupArea.Runtime,
                    "Popups and cross-origin policy",
                    """
                    The popup must not be blocked, and the serving origin must not send
                    Cross-Origin-Opener-Policy, or Uno cannot poll the popup's URL and the
                    sign-in hangs. Tokens live in browser storage
                    (KeyValueStorageConfiguration:BrowserCacheLocation; this sample's
                    development settings use SessionStorage).
                    """)
            ]),

        new(AppPlatform.Android,
            "Android",
            MobileCallbackUri,
            "A Chrome custom tab opens the login page; an intent filter brings the redirect back.",
            [
                new(SetupArea.Project,
                    $"Declare the {MobileScheme} scheme on WebAuthenticationBrokerActivity",
                    $"""
                    Platforms/Android/WebAuthenticationBrokerActivity.Android.cs derives from
                    Uno's WebAuthenticationBrokerActivityBase. Its [IntentFilter] DataScheme must
                    be the callback's scheme ({MobileScheme}); the template generates the
                    placeholder "myprotocol". Without a match the browser has nowhere to hand the
                    tokens and LoginAsync never completes.
                    """),

                new(SetupArea.Project,
                    "Reach the development machine from the emulator",
                    $"""
                    The emulator sees the development machine at 10.0.2.2, so
                    PlatformSupport.Android.cs points the provider and the API clients at
                    {AndroidEmulatorServer}. The server is plain HTTP, which Android blocks by
                    default: Resources/xml/network_security_config.xml allows cleartext to
                    10.0.2.2 only. A device on the network needs the machine's LAN address
                    instead, in both places and in the server's binding.
                    """),

                new(SetupArea.Runtime,
                    "A browser must be installed",
                    """
                    The broker launches the system browser with custom tabs, which shares cookies
                    with Chrome: a second sign-in skips the server's form until Sign out. Tokens
                    are kept in the Android KeyStore-backed storage, so a restart goes through
                    the silent path.
                    """)
            ]),

        new(AppPlatform.AppleUIKit,
            "iOS",
            MobileCallbackUri,
            "An ASWebAuthenticationSession opens the login page; the URL scheme brings the redirect back.",
            [
                new(SetupArea.Project,
                    $"Declare the {MobileScheme} scheme under CFBundleURLTypes in Info.plist",
                    """
                    Uno's WebAuthenticationBroker reads the schemes declared under
                    CFBundleURLTypes > CFBundleURLSchemes in Platforms/iOS/Info.plist and refuses
                    to start a sign-in whose callback scheme is not one of them. The template
                    writes a top-level CFBundleURLSchemes key instead, which iOS and the broker
                    both ignore.
                    """),

                new(SetupArea.Project,
                    "Allow plain HTTP to localhost",
                    """
                    The simulator reaches the development machine at localhost, so appsettings
                    is written for this head. App Transport Security blocks the app's own HTTP
                    calls (the refresh and the API) unless Info.plist sets NSAllowsLocalNetworking.
                    """),

                new(SetupArea.Project,
                    "Keychain entitlement for the token store",
                    """
                    Tokens are kept in the keychain on iOS, so Platforms/iOS/Entitlements.plist
                    needs keychain-access-groups with $(AppIdentifierPrefix)$(CFBundleIdentifier),
                    or the first token save fails. The template ships an empty one.
                    """)
            ]),

        new(AppPlatform.Windows,
            "Windows (WinAppSDK)",
            "(redirect_uri of LoginStartUri)",
            "Not one of this sample's heads; included for completeness.",
            [
                new(SetupArea.Project,
                    "Add the head, change no code",
                    """
                    Add net10.0-windows10.0.xxxxx.0 to <TargetFrameworks> and build on Windows.
                    On WinAppSDK the Web provider opens the browser out of process and handles the
                    protocol activation itself; Package.appxmanifest must declare the callback's
                    scheme (the template writes "myprotocol" there). The tokens reach PostLogin
                    as name=value pairs, not as a URL.
                    """)
            ])
    ];
}
