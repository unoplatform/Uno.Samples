namespace Authentication.OidcExtensionsDemo.Authentication;

/// <summary>Where a setup step has to be performed.</summary>
public enum SetupArea
{
    /// <summary>At the identity provider, on the client registration.</summary>
    IdentityProvider,

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
        SetupArea.IdentityProvider => "IDP",
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
/// The per-platform OIDC requirements, as reference material shown inside the app.
/// </summary>
/// <remarks>
/// <para>
/// This is deliberately shared code rather than per-platform: the point is to be able to read
/// what <em>every</em> head needs while running on any one of them.
/// </para>
/// <para>
/// The sample is configured for the Duende demo server (<c>https://demo.duendesoftware.com</c>),
/// whose public demo clients accept any redirect URI, so it signs in with no registration work
/// at all. The steps marked IDP are what a real identity provider will ask for instead.
/// </para>
/// </remarks>
public static class PlatformGuide
{
    /// <summary>
    /// Must match <c>&lt;ApplicationId&gt;</c> in <c>Authentication.OidcExtensionsDemo.csproj</c>.
    /// It is the Android package name and the Apple bundle identifier.
    /// </summary>
    public const string ApplicationId = "com.companyname.authentication-oidcextensionsdemo";

    /// <summary>The custom scheme the mobile heads use for the redirect URI.</summary>
    public const string MobileScheme = "oidcdemo";

    /// <summary>The loopback redirect URI the desktop head listens on.</summary>
    public const string DesktopRedirectUri = "http://127.0.0.1:52001/callback";

    /// <summary>Steps that apply no matter which head you run.</summary>
    public static IReadOnlyList<SetupStep> Common { get; } =
    [
        new(SetupArea.IdentityProvider,
            "Register a public client with PKCE",
            """
            Create a client (an "application") at your identity provider using the
            authorization code flow with PKCE and no client secret. A secret shipped inside an
            app is not secret; the Uno.Extensions OIDC provider does PKCE by default. Note the
            Authority (the issuer URL) and the client ID.
            """),

        new(SetupArea.IdentityProvider,
            "Allow the scopes and the redirect URIs",
            """
            Grant openid (required), profile and email (claims in the ID token), the API's own
            scope, and offline_access - without that one the provider gets no refresh token,
            RefreshAsync has nothing to renew, and the user signs in again at every launch.
            Register one redirect URI per head you ship: they differ by platform (below).
            """),

        new(SetupArea.Project,
            "Configure the OidcAuthentication section",
            """
            Put Authority, ClientId, Scope, RedirectUri and PostLogoutRedirectUri in the
            OidcAuthentication section of appsettings.development.json (Debug builds) and
            appsettings.json (Release). The section name matches the name passed to
            auth.AddOidc(name: "OidcAuthentication") in App.xaml.cs, and binds straight to
            OidcClient's OidcClientOptions. Leave ClientSecret out.
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

    /// <summary>
    /// Per-platform requirements, in the order they are shown. The redirect URI depends on the
    /// configured value, so the list is built rather than static.
    /// </summary>
    public static IReadOnlyList<PlatformGuideEntry> Build(string? configuredRedirectUri)
    {
        var mobileRedirect = PlatformSupport.ConfiguredOrPlaceholder(configuredRedirectUri);

        return
        [
            new(AppPlatform.Desktop,
                "Desktop (Skia)",
                DesktopRedirectUri,
                "The default browser opens the sign-in page; a loopback listener in the app receives the code.",
                [
                    new(SetupArea.Project,
                        "Provide an IBrowser: the broker is not implemented on this head",
                        """
                        Uno's WebAuthenticationBroker throws NotImplementedException on the Skia
                        desktop head, so the OIDC provider's default browser cannot complete a
                        sign-in there. Platforms/Desktop/SystemBrowser.cs implements OidcClient's
                        IBrowser: it opens the system browser on the authorization URL and waits
                        on an HttpListener bound to the loopback redirect URI for the code.
                        App.xaml.cs registers it with services.AddTransient<IBrowser,
                        SystemBrowser>() - the OIDC provider picks up whatever IBrowser is
                        registered.
                        """),

                    new(SetupArea.Project,
                        "Use the loopback redirect URI on this head",
                        $"""
                        Platforms/Desktop/PlatformSupport.Desktop.cs overrides RedirectUri and
                        PostLogoutRedirectUri in code with {DesktopRedirectUri}: a custom scheme
                        is not routed back to a desktop app that was not installed through a
                        package, and the port is fixed so the value can be registered once.
                        """),

                    new(SetupArea.IdentityProvider,
                        "Register the loopback URI",
                        $"""
                        Register {DesktopRedirectUri} as both a redirect URI and a post-logout
                        redirect URI. The Duende demo server accepts it as is.
                        """),

                    new(SetupArea.Runtime,
                        "The session persists",
                        """
                        Tokens are stored by Uno.Extensions' key-value storage: ApplicationData
                        on desktop. With offline_access in the scope, relaunching the app goes
                        through RefreshAsync and signs back in with no prompt.
                        """)
                ]),

            new(AppPlatform.WebAssembly,
                "WebAssembly",
                "{origin}/authentication-callback",
                "Sign-in opens in a popup; the provider always uses the broker's callback on the app's origin.",
                [
                    new(SetupArea.Project,
                        "Nothing to change",
                        """
                        On WebAssembly the OIDC provider replaces RedirectUri and
                        PostLogoutRedirectUri with WebAuthenticationBroker's callback URI,
                        {origin}/authentication-callback, whatever configuration says. The popup
                        only has to land on that same-origin path for Uno to read the code out
                        of its URL; wwwroot/staticwebapp.config.json already rewrites unknown
                        paths to index.html, and another host needs the same fallback.
                        """),

                    new(SetupArea.IdentityProvider,
                        "Register the origin's callback",
                        """
                        Register http://localhost:5000/authentication-callback (the port comes
                        from Properties/launchSettings.json) and one entry per origin you deploy
                        to. A custom scheme cannot work in a browser.
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
                mobileRedirect,
                "A Chrome custom tab opens the sign-in page; an intent filter brings the redirect back.",
                [
                    new(SetupArea.Project,
                        $"Declare the {MobileScheme} scheme on WebAuthenticationBrokerActivity",
                        $"""
                        Platforms/Android/WebAuthenticationBrokerActivity.Android.cs derives from
                        Uno's WebAuthenticationBrokerActivityBase. Its [IntentFilter] DataScheme
                        must be the scheme of the configured RedirectUri ({MobileScheme}); the
                        template generates the placeholder "myprotocol". Without a match the
                        browser has nowhere to hand the code and LoginAsync never completes.
                        Intent filters are compile-time attributes, so the scheme cannot follow
                        appsettings automatically.
                        """),

                    new(SetupArea.IdentityProvider,
                        "Register the custom-scheme URI",
                        $"""
                        Register {mobileRedirect} as redirect and post-logout redirect URI.
                        """),

                    new(SetupArea.Runtime,
                        "A browser must be installed",
                        """
                        The broker launches the system browser with custom tabs. Tokens are kept
                        in the Android KeyStore-backed storage, so a restart goes through the
                        silent path.
                        """)
                ]),

            new(AppPlatform.AppleUIKit,
                "iOS",
                mobileRedirect,
                "An ASWebAuthenticationSession opens the sign-in page; the URL scheme brings the redirect back.",
                [
                    new(SetupArea.Project,
                        $"Declare the {MobileScheme} scheme under CFBundleURLTypes in Info.plist",
                        $"""
                        Uno's WebAuthenticationBroker reads the schemes declared under
                        CFBundleURLTypes > CFBundleURLSchemes in Platforms/iOS/Info.plist and
                        refuses to start a sign-in whose callback scheme is not one of them. The
                        template writes a top-level CFBundleURLSchemes key instead, which iOS
                        and the broker both ignore: move the scheme under CFBundleURLTypes.
                        """),

                    new(SetupArea.IdentityProvider,
                        "Register the custom-scheme URI",
                        $"""
                        Register {mobileRedirect} as redirect and post-logout redirect URI.
                        """),

                    new(SetupArea.Project,
                        "Keychain entitlement for the token store",
                        """
                        Tokens are kept in the keychain on iOS, so Platforms/iOS/Entitlements.plist
                        needs keychain-access-groups with $(AppIdentifierPrefix)$(CFBundleIdentifier),
                        or the first token save fails. Already present in this sample.
                        """)
                ]),

            new(AppPlatform.Windows,
                "Windows (WinAppSDK)",
                "(configured RedirectUri)",
                "Not one of this sample's heads; included for completeness.",
                [
                    new(SetupArea.Project,
                        "Add the head, change no code",
                        """
                        Add net10.0-windows10.0.xxxxx.0 to <TargetFrameworks> and build on
                        Windows. On WinAppSDK the OIDC provider opens the browser out of process
                        and handles the protocol activation for the configured RedirectUri
                        itself; the Package.appxmanifest must declare that scheme (the template
                        writes "myprotocol" there).
                        """)
                ])
        ];
    }
}
