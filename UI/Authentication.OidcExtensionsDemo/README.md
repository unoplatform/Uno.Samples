# Authentication with OpenID Connect via Uno.Extensions

This sample signs users in with any OpenID Connect identity provider using
[`Uno.Extensions.Authentication.Oidc`](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-OidcAuthentication.html)
and calls a protected API with the resulting token, from a single Uno Platform project.

Tested on:

- [x] Desktop (Skia) — Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] WebAssembly — Uno.Sdk 7.0.0-dev.89, Uno.Extensions 8.0.0-dev.135
- [x] Android — last exercised on Uno.Sdk 7.0.0-dev.77, Uno.Extensions 8.0.0-dev.85
- [x] iOS (simulator, iOS 18.5) — Uno.Sdk 7.0.0-dev.89, Uno.Extensions 8.0.0-dev.135

This document is the **setup guide**: what each head needs before it can sign in. For what the
sample *demonstrates* and how the code is arranged, see
[`Authentication.OidcExtensionsDemo/ReadMe.md`](Authentication.OidcExtensionsDemo/ReadMe.md). The
same per-platform requirements are also available on-device, on the app's **Platform setup** page,
with the head you are running marked.

The companion sample [`UI/Authentication.MsalExtensionsDemo`](../Authentication.MsalExtensionsDemo)
does the same thing against Microsoft Entra ID with the MSAL provider.

---

## It runs as is

The sample ships configured for the [Duende IdentityServer demo](https://demo.duendesoftware.com)
(`appsettings.development.json`, section `OidcAuthentication`):

```json
"OidcAuthentication": {
  "Authority": "https://demo.duendesoftware.com/",
  "ClientId": "interactive.public",
  "Scope": "openid profile email api offline_access",
  "RedirectUri": "oidcdemo://callback",
  "PostLogoutRedirectUri": "oidcdemo://callback"
}
```

`interactive.public` is a public client using the authorization code flow with PKCE, and the demo
server accepts any redirect URI for it, so **there is nothing to register**: build a head, press
**Sign in**, and sign in as `alice` / `alice` (or `bob` / `bob`). The *Call the API* page then
calls `https://demo.duendesoftware.com/api/test`, which echoes the claims of the access token it
received.

Two things to know about which file wins:

- **Debug builds run in the Development environment** (`App.xaml.cs` calls
  `.UseEnvironment(Environments.Development)` under `#if DEBUG`), so
  `appsettings.development.json` is the file that applies while you are testing. For a **Release**
  build, put the same values in `appsettings.json`.
- **`appsettings*.json` are embedded resources**, not copied to the output directory. After editing
  them you must **rebuild** — restarting the app is not enough.

### Versions

Desktop (Skia) sign-in needs **Uno.Extensions 8.0.0-dev.135 or later**: that is the first version
whose `AddOidc` registers a loopback `WebAuthenticationBroker` on Windows, macOS and Linux
([unoplatform/uno.extensions#3261](https://github.com/unoplatform/uno.extensions/pull/3261)).
Uno.Sdk **7.0.0-dev.95 or later** pins it, so keep `global.json` at that or newer; the sample uses
7.0.0-dev.115, which pins Uno.Extensions 8.0.0-dev.150.

One thing here you will not find in a template app: the csproj references `Uno.WinUI.Lottie` in
Debug builds only. Even on Uno.Sdk 7.0.0-dev.115, the App MCP client that Debug builds carry
(`Uno.UI.App.Mcp.Client`, App MCP 2.0.0-dev.18) still references `Uno.UI.Lottie`, which Uno.WinUI
no longer ships; without the package a Debug build dies at startup with `FileNotFoundException`
([unoplatform/uno.extensions#3262](https://github.com/unoplatform/uno.extensions/issues/3262)).
Release builds do not carry the tooling and start without it. Remove the reference once the
tooling packages are rebuilt.

### Pointing it at your own identity provider

1. Register a **public client** with the **authorization code flow and PKCE**, no client secret (a
   secret shipped inside an app is not secret). Note the issuer URL (`Authority`) and the client ID.
2. Allow the scopes: `openid` (required), `profile` and `email` (claims in the ID token), your
   API's scope, and **`offline_access`** — without it no refresh token is issued, `RefreshAsync`
   has nothing to renew, and the user signs in again at every launch.
3. Register one redirect URI per head you run (see the table below), as both a redirect and a
   post-logout redirect URI.
4. Put the values in `appsettings.development.json`. The section name matches the `name:` passed
   to `auth.AddOidc(name: "OidcAuthentication")` in `App.xaml.cs`, and binds straight to
   OidcClient's `OidcClientOptions`.
5. Change the `DemoApi` section's `Url` to your API, and `Services/Endpoints/IDemoApi.cs` to its
   endpoint.

| Head | Redirect URI | How it gets back into the app |
| --- | --- | --- |
| Desktop (Skia) | `http://localhost:52001/authentication-callback` | the loopback `WebAuthenticationBroker` the OIDC provider registers (Uno.Extensions 8.0.0-dev.135+) |
| WebAssembly | `http://localhost:5000/authentication-callback` | the `WebAuthenticationBroker` popup, on the app's own origin |
| Android | `oidcdemo://callback` | the intent filter on `WebAuthenticationBrokerActivity` |
| iOS | `oidcdemo://callback` | the `oidcdemo` scheme under `CFBundleURLTypes` in `Info.plist` |

The one value that follows your identity provider rather than this sample is the scheme: if you
change `oidcdemo://callback`, change it in all four places (appsettings, the Android activity,
`Info.plist`, and `Package.appxmanifest` for a Windows head).

---

## Per-head setup

The commands below are run from this folder (`UI/Authentication.OidcExtensionsDemo`). Visual Studio,
Rider (`.run/Authentication.OidcExtensionsDemo.run.xml`) and VS Code (`.vscode/launch.json`)
configurations are also present, and are easier for the mobile heads.

### Desktop (Skia)

**Project changes:** none — but note what the sample already does for you. Uno's own
`WebAuthenticationBroker` has no implementation on the Skia desktop targets; since Uno.Extensions
8.0.0-dev.135 (see [Versions](#versions)) `AddOidc` registers one, `DesktopWebAuthenticationBrokerProvider`,
which opens the sign-in page in the default browser and receives the redirect on a loopback HTTP
listener. All this head adds is a loopback redirect URI: `Platforms/Desktop/PlatformSupport.Desktop.cs`
overrides `RedirectUri` and `PostLogoutRedirectUri` through configuration (an in-memory source
layered over appsettings) with `http://localhost:52001/authentication-callback`, a fixed port so
the value can be registered once. Two rules come with the broker:

- Do not touch `WebAuthenticationBroker` (not even `GetCurrentApplicationCallbackUri()`) before
  `UseAuthentication` has run: the registration is first-wins, and the static broker keeps the
  provider it first resolved — Uno's stub, which throws `NotImplementedException`.
- Its log lines sit under `Uno.Extensions.Authentication`, which the template's `CoreLogLevel`
  filter hides; `App.xaml.cs` adds a filter at Information so the console says which broker is
  handling the sign-in (`WebAuthenticationBroker flows are handled by the desktop loopback broker ...`).

```bash
dotnet run --project Authentication.OidcExtensionsDemo/Authentication.OidcExtensionsDemo.csproj -f net10.0-desktop
```

**While testing:** the tokens are **persisted** (Uno.Extensions' key-value storage, ApplicationData
on desktop), so the second launch signs in silently through `RefreshAsync` with no prompt. To test
the *interactive* path again, press **Sign out** first.

**Sign out:** the provider sends the cached ID token as `id_token_hint`, so the identity provider
can redirect back to the listener. The Duende demo server shows its logged-out page with a link
back to the app; the sign-out completes as soon as you click it. Until then (or until the
broker's authentication timeout elapses) the app shows the call as busy; the tokens are cleared
either way.

### WebAssembly

**Project changes:** none. The provider replaces the configured redirect URIs with
`WebAuthenticationBroker.GetCurrentApplicationCallbackUri()` (`{origin}/authentication-callback`).
Sign-in opens in a popup whose URL Uno polls until it lands on that same-origin path, and
`Platforms/WebAssembly/wwwroot/staticwebapp.config.json` already rewrites unknown paths to
`index.html`. If you host the app somewhere else, give it the same fallback.

```bash
dotnet run --project Authentication.OidcExtensionsDemo/Authentication.OidcExtensionsDemo.csproj -f net10.0-browserwasm
# serves http://localhost:5000
```

**While testing:** the popup must not be blocked, and the serving origin must not set
`Cross-Origin-Opener-Policy`, or the flow opens a window and hangs. On **Sign out**, the popup shows the identity provider's logged-out page with a link back to the app (the provider sends the ID token as `id_token_hint`); click it and the popup lands on the callback and closes, or just close the popup — the app signs out either way. Tokens live in browser storage;
`appsettings.development.json` sets `KeyValueStorageConfiguration:BrowserCacheLocation` to
`SessionStorage`, so a reload signs back in silently and closing the tab signs you out.

### Android

**Project changes:** none for the demo server. The intent filter in
`Authentication.OidcExtensionsDemo/Platforms/Android/WebAuthenticationBrokerActivity.Android.cs`
declares the `oidcdemo` scheme; change `RedirectScheme` there if you change the redirect URI's
scheme. It is a compile-time attribute, so it cannot follow appsettings automatically — leave it
wrong and the browser has nowhere to hand back the authorization code.

```bash
dotnet build Authentication.OidcExtensionsDemo/Authentication.OidcExtensionsDemo.csproj -f net10.0-android -t:Run
```

**On the device:** the broker opens the system browser (Chrome) on the sign-in page; after sign-in
the identity provider redirects to `oidcdemo://callback`, which Android routes back to the app.
Tokens are kept in the KeyStore-backed storage, so a restart goes through the silent path. On
**Sign out**, the identity provider's logged-out page shows a link back to the app (the provider
sends the ID token as `id_token_hint`); following it or pressing Back returns to the app, and the
provider clears the tokens either way.

### iOS

**Project changes:** none for the demo server. Two files already carry what iOS needs:

| File | What it does | Change it when |
| --- | --- | --- |
| `Platforms/iOS/Info.plist` | `CFBundleURLTypes` declares the `oidcdemo` URL scheme. Uno's `WebAuthenticationBroker` reads the schemes declared there and refuses to start a sign-in whose callback scheme is missing. | you change the redirect URI's scheme |
| `Platforms/iOS/Entitlements.plist` | Grants `keychain-access-groups` — `$(AppIdentifierPrefix)$(CFBundleIdentifier)`. Uno.Extensions keeps the tokens in the keychain; without this the first token save fails. | never, for this sample |

The template generates a top-level `CFBundleURLSchemes` key instead, which iOS and the broker both
ignore; this sample moved the scheme under `CFBundleURLTypes`.

```bash
dotnet build Authentication.OidcExtensionsDemo/Authentication.OidcExtensionsDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

> **Edits to `Info.plist` or `Entitlements.plist` can be silently skipped by an incremental build.**
> After editing either, delete the iOS output and rebuild:
>
> ```bash
> rm -rf Authentication.OidcExtensionsDemo/bin/Debug/net10.0-ios Authentication.OidcExtensionsDemo/obj/Debug/net10.0-ios
> ```

### Windows (WinAppSDK)

Not one of this sample's four heads. To add it, add `net10.0-windows10.0.19041.0` to
`<TargetFrameworks>` and build on Windows. The provider opens the browser out of process and
handles the protocol activation itself; `Package.appxmanifest` already declares the `oidcdemo`
protocol.

---

## Checking that it worked

1. **Sign in** page → **Sign in**. The flow log narrates every provider call; the configuration,
   the redirect URI in use and the resulting tokens are shown above it, with the ID token's claims
   decoded (`name`, `email`, `sub`, ...).
2. **Sign out**, then **Silent only** — expect it to fail, which is what the silent path does with
   no refresh token stored.
3. Restart the app: the startup silent refresh should sign you back in with no prompt.
4. **Call the API** page → `GET /api/test`: the Refit client registered in `UseHttp` sends the
   access token, and the demo API answers with the claims it read from it. Signed out, the same
   call answers 401.

The **Redact** switch in the header hides the client ID, the account and anything token-shaped
across all pages — useful when recording.

---

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| The *Sign in* page says no identity provider is configured | `Authority` or `ClientId` is empty | Fill the `OidcAuthentication` section, and **rebuild** (appsettings are embedded) |
| Your edit to appsettings has no effect | Editing `appsettings.json` while running a Debug build | Debug runs the Development environment: edit `appsettings.development.json` |
| **Desktop:** `LoginAsync` returns false and the log names `NotImplementedException` or `DesktopWebAuthenticationBrokerProvider.TryRegister()` | Uno.Extensions older than 8.0.0-dev.135, or something used `WebAuthenticationBroker` before `UseAuthentication` ran | Use Uno.Sdk 7.0.0-dev.95 or later in `global.json` (see [Versions](#versions)); move the earlier broker call after `UseAuthentication` |
| **Desktop:** the window never appears in a Debug build, `FileNotFoundException: Uno.UI.Lottie` | The Debug tooling packages reference `Uno.UI.Lottie` (unoplatform/uno.extensions#3262) | Keep the Debug-only `Uno.WinUI.Lottie` reference in the csproj |
| Redirect URI rejected by your identity provider | The registered URI is not exactly what was sent | Copy the value off the *Sign in* page; it differs per head |
| **Android:** browser opens, sign-in completes, nothing comes back | `RedirectScheme` in `WebAuthenticationBrokerActivity.Android.cs` does not match the redirect URI | Fix it and rebuild |
| **iOS:** `No schemes defined in info.plist` | The scheme is not under `CFBundleURLTypes` | Fix `Info.plist`, then **clean-build** the iOS head |
| Signed in again at every launch | `offline_access` missing from `Scope`, so no refresh token | Add the scope at the identity provider and in configuration |
| **Sign out** stays busy for minutes | The identity provider did not redirect back: the post-logout URI is not registered, or its logged-out page is waiting for you to click the link back to the app | Register the post-logout URI; click the link (closing the browser page also ends the call on WebAssembly) |
| **WebAssembly:** popup opens and never closes | `Cross-Origin-Opener-Policy` on the serving origin, or the callback path is not served | Leave COOP unset; keep the `index.html` fallback |

## Relevant documentation

- [Uno.Extensions OIDC authentication](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-OidcAuthentication.html)
- [Uno.Extensions authentication overview](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/AuthenticationOverview.html)
- [Web Authentication Broker](https://platform.uno/docs/articles/features/web-authentication-broker.html)
- [Duende IdentityModel.OidcClient](https://docs.duendesoftware.com/identitymodel-oidcclient/)
