namespace Authentication.CustomExtensionsDemo.Authentication;

/// <summary>Where a setup step has to be performed.</summary>
public enum SetupArea
{
    /// <summary>On the backend the app signs in against.</summary>
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
    string ServerAddress,
    string Summary,
    IReadOnlyList<SetupStep> Steps)
{
    /// <summary>True when the app is currently running on this platform.</summary>
    public bool IsCurrent => Platform == PlatformSupport.Current;
}

/// <summary>
/// The per-platform requirements, as reference material shown inside the app.
/// </summary>
/// <remarks>
/// The Custom provider has no browser and no redirect, so most of what matters applies to every
/// head; per head, only the server address and the token store differ.
/// </remarks>
public static class PlatformGuide
{
    /// <summary>The demo server as the Android emulator reaches it.</summary>
    public const string AndroidEmulatorServer = "http://10.0.2.2:5001";

    /// <summary>Steps that apply no matter which head you run.</summary>
    public static IReadOnlyList<SetupStep> Common { get; } =
    [
        new(SetupArea.Project,
            "Return only tokens from Login",
            """
            Whatever the Login callback returns is stored as-is, every key, and the user counts as
            signed in while any token is stored. Return a new dictionary with AccessToken and
            RefreshToken (TokenCacheExtensions keys). The template returns the credentials
            dictionary it received, which stores the password next to the tokens. Return null to
            fail the sign-in; a 401 from the login endpoint is a wrong password, not an error.
            """),

        new(SetupArea.Project,
            "Write the Refresh callback",
            """
            Without one, RefreshAsync reports success while any token is stored and the session
            only ends on an API 401. This sample's callback posts the refresh token to
            /auth/refresh with the No-Refresh header (a Kiota request adds it as a header; Refit
            uses the Headers.NoRefresh attribute), so a 401 there cannot loop. null ends the
            session; an exception (server unreachable) comes out of RefreshAsync with the tokens
            kept.
            """),

        new(SetupArea.Project,
            "Register the client in UseHttp",
            """
            The access token reaches an API only through clients registered with AddKiotaClient,
            AddRefitClient or AddClient inside UseHttp. AddCustom<DemoServerClient> then resolves
            that client for each callback. AddKiotaClient's configuration section is its name
            argument (DemoServer here); without one, it is the client's full type name.
            """),

        new(SetupArea.Backend,
            "Login, refresh and logout endpoints",
            """
            The demo server's are POST /auth/login (user name and password in, tokens out, 401 on
            a wrong password), POST /auth/refresh (rotates the refresh token) and POST /auth/logout
            (revokes it). The app's Kiota client is generated from its OpenAPI document; the README
            has the command.
            """)
    ];

    /// <summary>Per-platform requirements, in the order they are shown.</summary>
    public static IReadOnlyList<PlatformGuideEntry> Build() =>
    [
        new(AppPlatform.Desktop,
            "Desktop (Skia)",
            "http://localhost:5001",
            "Nothing specific: no browser, no redirect.",
            [
                new(SetupArea.Runtime,
                    "Tokens persist",
                    """
                    Uno.Extensions' key-value storage keeps the tokens, so the next launch signs in
                    through the Refresh callback with no prompt.
                    """)
            ]),

        new(AppPlatform.WebAssembly,
            "WebAssembly",
            "{origin}",
            "Run through the server, which hosts the app on the API's origin.",
            [
                new(SetupArea.Project,
                    "Same origin as the API",
                    """
                    Platforms/WebAssembly/PlatformSupport.Wasm.cs points the client at the page's
                    origin. Run the server project and open http://localhost:5001: the app and the
                    API share one origin, so no CORS is needed. Served from elsewhere, the server
                    needs a CORS policy for that origin.
                    """),

                new(SetupArea.Runtime,
                    "Browser storage",
                    """
                    Tokens live in browser storage (KeyValueStorageConfiguration:BrowserCacheLocation;
                    this sample's development settings use SessionStorage, so closing the tab
                    signs out).
                    """)
            ]),

        new(AppPlatform.Android,
            "Android",
            AndroidEmulatorServer,
            "The emulator reaches the development machine at 10.0.2.2.",
            [
                new(SetupArea.Project,
                    "Server address and cleartext",
                    $"""
                    PlatformSupport.Android.cs points the client at {AndroidEmulatorServer}. The
                    server is plain HTTP, which Android blocks by default:
                    Resources/xml/network_security_config.xml allows it to 10.0.2.2 only. A device
                    on the network needs the machine's LAN address in both places.
                    """)
            ]),

        new(AppPlatform.AppleUIKit,
            "iOS",
            "http://localhost:5001",
            "The simulator reaches the Mac at localhost.",
            [
                new(SetupArea.Project,
                    "Allow plain HTTP to localhost",
                    """
                    Info.plist sets NSAppTransportSecurity > NSAllowsLocalNetworking so App
                    Transport Security allows the calls to the demo server.
                    """),

                new(SetupArea.Project,
                    "Keychain entitlement for the token store",
                    """
                    Tokens are kept in the keychain on iOS, so Platforms/iOS/Entitlements.plist
                    needs keychain-access-groups with $(AppIdentifierPrefix)$(CFBundleIdentifier),
                    or the first token save fails. The template ships an empty one.
                    """)
            ])
    ];
}
