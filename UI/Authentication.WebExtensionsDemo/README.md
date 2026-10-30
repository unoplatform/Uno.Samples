# Web authentication via Uno.Extensions

This sample signs users in through a backend's own login page using the Web provider of
[`Uno.Extensions.Authentication`](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-WebAuthentication.html)
(`AddWeb`), and calls a protected API with the resulting token, from a single Uno Platform project.
The backend is included: `Authentication.WebExtensionsDemo.Server` serves the login page, the
token refresh, the protected API, and the WebAssembly head.

Tested on:

- [x] Desktop (Skia) on Windows: Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] WebAssembly: Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] Android (API 36 emulator): Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [ ] iOS: not run yet

This document is the **setup guide**: what each head needs before it can sign in. For what the
sample *demonstrates* and how the code is arranged, see
[`Authentication.WebExtensionsDemo/ReadMe.md`](Authentication.WebExtensionsDemo/ReadMe.md). The
same per-platform requirements are also available on-device, on the app's **Platform setup** page,
with the head you are running marked.

The companion sample [`UI/Authentication.OidcExtensionsDemo`](../Authentication.OidcExtensionsDemo)
does the same thing against a standard OpenID Connect provider with `AddOidc`.

---

## It runs as is

Start the server first, from this folder:

```bash
dotnet run --project Authentication.WebExtensionsDemo.Server
# listens on http://localhost:5001
```

Then run any head (below), press **Sign in**, and sign in as `alice` / `alice` (or `bob` / `bob`).
The app's configuration (`appsettings.development.json`, section `WebAuthentication`) points at
that server:

```json
"WebAuthentication": {
  "LoginStartUri": "http://localhost:5001/auth/login?redirect_uri=webdemo://callback",
  "LogoutStartUri": "http://localhost:5001/auth/logout?redirect_uri=webdemo://callback"
}
```

The flow, in the shape `AddWeb` expects:

1. The provider opens `LoginStartUri` in the browser. The app's `PrepareLoginStartUri` callback
   adds a random `state`.
2. The server's login page signs the user in and redirects to the `redirect_uri` with
   `#access_token=...&refresh_token=...&expires_in=60&state=...`.
3. The provider waits for the browser to reach that `redirect_uri` (no `LoginCallbackUri` is
   configured, so it takes the one in `LoginStartUri`), and reads the two tokens from the URL.
   The app's `PostLogin` callback refuses them unless the `state` matches.
4. The tokens are stored in `ITokenCache`. The Refit clients registered in `UseHttp` send the
   access token; on a 401 the handler runs the app's `Refresh` callback (`POST /auth/refresh`)
   and retries.
5. **Sign out** opens `LogoutStartUri`, which ends the server's browser session (a cookie that
   lets a second sign-in skip the form) and redirects back; the provider then clears the tokens.

Two things to know about which file wins:

- **Debug builds run in the Development environment** (`App.xaml.cs` calls
  `.UseEnvironment(Environments.Development)` under `#if DEBUG`), so
  `appsettings.development.json` is the file that applies while you are testing. For a **Release**
  build, put the same values in `appsettings.json`.
- **`appsettings*.json` are embedded resources**, not copied to the output directory. After editing
  them you must **rebuild**; restarting the app is not enough.

### Versions

Desktop (Skia) sign-in needs **Uno.Extensions 8.0.0-dev.135 or later**: that is the first version
whose `AddWeb` registers a loopback `WebAuthenticationBroker` on Windows, macOS and Linux
([unoplatform/uno.extensions#3261](https://github.com/unoplatform/uno.extensions/pull/3261)).
Uno.Sdk 7.0.0-dev.95 and later pin a version that has it; this sample's `global.json` uses
7.0.0-dev.115 (Uno.Extensions 8.0.0-dev.150), so nothing is overridden.

One workaround remains: the csproj references `Uno.WinUI.Lottie` in Debug builds only. On
Uno.Sdk 7.0.0-dev.115 the Hot Design tooling (Uno.UI.HotDesign 2.0.0-dev.31) still references
`Uno.UI.Lottie`, which Uno.WinUI no longer ships; without the package a Debug build on desktop
hangs at startup with no window
([unoplatform/uno.extensions#3262](https://github.com/unoplatform/uno.extensions/issues/3262)).

### Pointing it at your own backend

1. Your backend's login page must redirect to the `redirect_uri` it is given with `access_token`
   (and `refresh_token`, for a session that survives restarts) in the query or the fragment, and
   echo the `state` it received. Different parameter names go in `AccessTokenKey` and
   `RefreshTokenKey` of the `WebAuthentication` section.
2. It must only redirect to callbacks it knows: allow one per head (table below), compared
   exactly. The demo server's list is `DemoAuth:AllowedRedirectUris` in its `appsettings.json`.
3. Put the login and logout URLs in `appsettings.development.json`. The section name matches the
   `name:` passed to `auth.AddWeb<IAuthApi>(...)` in `App.xaml.cs`.
4. Change `IAuthApi.RefreshAsync` and `WebFlowService.RefreshTokensAsync` to your refresh endpoint,
   and the `AuthApi` and `ProfileApi` sections' `Url` to your API.

| Head | Callback (`redirect_uri`) | Server address | How it gets back into the app |
| --- | --- | --- | --- |
| Desktop (Skia) | `http://localhost:52002/authentication-callback` | `http://localhost:5001` | the loopback `WebAuthenticationBroker` `AddWeb` registers |
| WebAssembly | `{origin}/authentication-callback` | the origin (the server hosts the app) | the `WebAuthenticationBroker` popup, on the app's own origin |
| Android | `webdemo://callback` | `http://10.0.2.2:5001` (emulator) | the intent filter on `WebAuthenticationBrokerActivity` |
| iOS | `webdemo://callback` | `http://localhost:5001` (simulator) | the `webdemo` scheme under `CFBundleURLTypes` in `Info.plist` |

The heads whose values differ from appsettings set them in `Platforms/<head>/PlatformSupport.*.cs`
through configuration. If you change the `webdemo` scheme, change it in the Android activity,
`Info.plist`, `Package.appxmanifest` (for a Windows head) and `PlatformGuide.cs` too.

---

## Per-head setup

The commands below are run from this folder (`UI/Authentication.WebExtensionsDemo`), with the
server already running.

### Desktop (Skia)

**Project changes:** none. Uno's own `WebAuthenticationBroker` has no implementation on the Skia
desktop targets; `AddWeb` registers one, `DesktopWebAuthenticationBrokerProvider`, which opens the
login page in the default browser and receives the redirect on a loopback HTTP listener (it relays
a fragment response back to itself, since browsers never send fragments to a server).
`Platforms/Desktop/PlatformSupport.Desktop.cs` sets the loopback callback, on a fixed port so the
backend can allow it once. Two rules come with the broker:

- The listener accepts the first request to its path from any local page or process, and the Web
  provider does not tie it to the sign-in it started. That is what the `state` check in
  `PostLogin` is for: a request that does not carry the `state` this sign-in sent is refused.
  Only `AddWeb<TService>`'s `PostLogin` receives the callback URL, which is why the sample uses it.
- Do not touch `WebAuthenticationBroker` before `UseAuthentication` has run: the registration is
  first-wins, and the static broker keeps the provider it first resolved.

```bash
dotnet run --project Authentication.WebExtensionsDemo/Authentication.WebExtensionsDemo.csproj -f net10.0-desktop
```

**While testing:** the tokens are persisted, so the second launch signs in silently through the
`Refresh` callback. Restarting the *server* forgets every token, so the next refresh is refused and
the app signs out (`LoggedOut`).

### WebAssembly

**Project changes:** none. Unlike the OIDC provider, the Web provider sends the configured
`redirect_uri` on WebAssembly too, so `Platforms/WebAssembly/PlatformSupport.Wasm.cs` sets it to
`WebAuthenticationBroker.GetCurrentApplicationCallbackUri()` (the origin plus
`/authentication-callback`). Run this head through the server, which hosts it on the same origin as
the login page and the API, so no CORS is needed and the callback lands on a page the server serves
(its `index.html` fallback):

```bash
dotnet run --project Authentication.WebExtensionsDemo.Server
# open http://localhost:5001
```

**While testing:** the popup must not be blocked, and the serving origin must not set
`Cross-Origin-Opener-Policy`. `appsettings.development.json` sets
`KeyValueStorageConfiguration:BrowserCacheLocation` to `SessionStorage`, so a reload signs back in
silently and closing the tab signs you out.

### Android

**Project changes:** none for the emulator. The intent filter in
`Platforms/Android/WebAuthenticationBrokerActivity.Android.cs` declares the `webdemo` scheme. The
emulator reaches the development machine at `10.0.2.2`, so `PlatformSupport.Android.cs` points the
provider and the API clients there, and `Resources/xml/network_security_config.xml` allows
cleartext HTTP to that address only (Android blocks it by default). A physical device needs the
machine's LAN address instead, in both places, and the server bound to it
(`--urls http://0.0.0.0:5001`).

```bash
dotnet build Authentication.WebExtensionsDemo/Authentication.WebExtensionsDemo.csproj -f net10.0-android -t:Run
```

**On the device:** the broker opens the system browser on the login page; after sign-in the server
redirects to `webdemo://callback`, which Android routes back to the app. Tokens are kept in the
KeyStore-backed storage, so a restart goes through the silent path. If the app's process is killed
while the browser is in front (a low-memory emulator running a Debug build), the redirect starts
a fresh process with no sign-in waiting for it: sign in again.

### iOS

**Project changes:** none for the simulator. Three files already carry what iOS needs:

| File | What it does |
| --- | --- |
| `Platforms/iOS/Info.plist` | `CFBundleURLTypes` declares the `webdemo` URL scheme. Uno's `WebAuthenticationBroker` reads the schemes declared there and refuses to start a sign-in whose callback scheme is missing. The template generates a top-level `CFBundleURLSchemes` key instead, which iOS and the broker both ignore. |
| `Platforms/iOS/Info.plist` | `NSAppTransportSecurity` > `NSAllowsLocalNetworking`, so App Transport Security lets the app's own HTTP calls (refresh, API) reach the server at `http://localhost:5001`. Not yet confirmed on a simulator run. |
| `Platforms/iOS/Entitlements.plist` | Grants `keychain-access-groups`: Uno.Extensions keeps the tokens in the keychain, and without this the first token save fails. |

```bash
dotnet build Authentication.WebExtensionsDemo/Authentication.WebExtensionsDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

The simulator shares the Mac's network, so the server must run on the Mac.

> **Edits to `Info.plist` or `Entitlements.plist` can be silently skipped by an incremental build.**
> After editing either, delete the iOS output and rebuild:
>
> ```bash
> rm -rf Authentication.WebExtensionsDemo/bin/Debug/net10.0-ios Authentication.WebExtensionsDemo/obj/Debug/net10.0-ios
> ```

---

## Checking that it worked

1. **Sign in** page > **Sign in**. The flow log shows `PrepareLoginStartUri: state added`,
   `PostLogin: state matches` and the stored tokens.
2. **Call the API** > `GET /api/profile`: the API answers with the user and the token's issue
   time. Wait a minute and call again: the token has expired, the handler runs the `Refresh`
   callback, and the issue time moves on.
3. Restart the app: the startup silent refresh signs you back in with no prompt.
4. **Sign out**, then **Silent only**: nothing to refresh. **Sign in** again shows the form, since
   sign-out ended the server's browser session.

The **Redact** switch in the header hides anything token-shaped across all pages, which is useful
when recording.

---

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| `HttpRequestException` in the flow log, or the API call fails | The server is not running, or not reachable at the address this head uses | Start `Authentication.WebExtensionsDemo.Server`; check the head's address in the table above |
| The login page says `redirect_uri ... is not in DemoAuth:AllowedRedirectUris` | The callback this head sends is not allowed by the server | Add it to the server's `appsettings.json`, exactly as the *Sign in* page shows it |
| **Desktop:** the window never appears in a Debug build | The Hot Design tooling references `Uno.UI.Lottie` (unoplatform/uno.extensions#3262) | Keep the Debug-only `Uno.WinUI.Lottie` reference in the csproj |
| **Desktop:** `LoginAsync` fails with `NotImplementedException` | Uno.Extensions older than 8.0.0-dev.135, or something used `WebAuthenticationBroker` before `UseAuthentication` ran | Check the Uno.Sdk in `global.json`; move the earlier broker call after `UseAuthentication` |
| `PostLogin: state mismatch, tokens refused` | The callback did not come from the sign-in in flight (another local request reached the desktop listener, or the backend did not echo `state`) | Sign in again; make your backend echo `state` |
| **Android:** browser completes, app shows no sign-in | `DataScheme` in `WebAuthenticationBrokerActivity.Android.cs` does not match the callback, or the app process was killed while the browser was in front | Fix the scheme and rebuild; or sign in again |
| **Android:** API call fails with a cleartext error | Plain HTTP to an address `network_security_config.xml` does not allow | Add the address there, or serve HTTPS |
| **iOS:** `No schemes defined in info.plist` | The scheme is not under `CFBundleURLTypes` | Fix `Info.plist`, then **clean-build** the iOS head |
| Signed out after every server restart | The demo server keeps its tokens in memory | Expected; a real backend persists them |
| **WebAssembly:** the popup shows 404 for `/auth/login` | The head was started on its own dev server (`dotnet run -f net10.0-browserwasm`, port 5000), which has no backend | Run the server project and open `http://localhost:5001` |
| **WebAssembly:** popup opens and never closes | `Cross-Origin-Opener-Policy` on the serving origin, or the callback path is not served | Leave COOP unset; keep the `index.html` fallback |

## Relevant documentation

- [Uno.Extensions Web authentication](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-WebAuthentication.html)
- [Uno.Extensions authentication overview](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/AuthenticationOverview.html)
- [Web Authentication Broker](https://platform.uno/docs/articles/features/web-authentication-broker.html)
