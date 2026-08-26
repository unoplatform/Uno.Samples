# Authentication with MSAL via Uno.Extensions

This sample signs users in with Microsoft Entra ID using
[`Uno.Extensions.Authentication.MSAL`](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-MsalAuthentication.html)
and calls Microsoft Graph with the resulting token, from a single Uno Platform project.

Tested on:

- [x] Desktop (Skia)
- [x] WebAssembly
- [x] Android
- [x] iOS

This document is the **setup guide**: everything you have to change before the sample can sign in,
head by head. For what the sample *demonstrates* and how the code is arranged, see
[`Authentication.MsalExtensionsDemo/ReadMe.md`](Authentication.MsalExtensionsDemo/ReadMe.md). The
same per-platform requirements are also available on-device, on the app's **Platform setup** page,
with the head you are running marked.

The companion sample [`UI/Authentication.MsalDemo`](../Authentication.MsalDemo) does the same thing
with `Uno.WinUI.MSAL` and no Uno.Extensions.

---

## What you actually have to edit

The sample ships with an empty client ID and a placeholder Android scheme, and runs anyway — it
explains what is missing rather than throwing. Rows 1 and 2 are done once; row 3 once per head you
want to test; rows 4 and 5 only concern the mobile heads:

| # | Where | What | Needed for |
| --- | --- | --- | --- |
| 1 | Microsoft Entra admin center | An app registration: public client, `User.Read` | every head |
| 2 | `Authentication.MsalExtensionsDemo/appsettings.development.json` | `MsalAuthentication:ClientId` and `TenantId` | every head |
| 3 | Microsoft Entra admin center | One redirect URI **per head you run**, under the right platform type | every head |
| 4 | `Authentication.MsalExtensionsDemo/Platforms/Android/MsalActivity.Android.cs` | `RedirectScheme` — `"msal"` + your client ID | **Android only** |
| 5 | `Authentication.MsalExtensionsDemo/Platforms/iOS/Info.plist` | `CFBundleURLSchemes` — only if you change `<ApplicationId>` | **iOS only** |

Desktop and WebAssembly need **no project edits at all** — only the registration (steps 1–3).

---

## 1. Register the application

In the [Microsoft Entra admin center](https://entra.microsoft.com):

1. **Applications → App registrations → New registration.**
   - **Supported account types** must match the `TenantId` you configure in step 2. The sample
     ships with `consumers`, which is *personal Microsoft accounts only*:

     | `TenantId` | Supported account types to pick |
     | --- | --- |
     | `consumers` | Personal Microsoft accounts only |
     | `organizations` | Accounts in any organizational directory |
     | `common` | Any organizational directory **and** personal Microsoft accounts |
     | a tenant ID or domain | Accounts in this organizational directory only |

   - Leave the redirect URI blank for now — you add it in step 3, once the app has told you the
     exact string.
2. Copy the **Application (client) ID** from **Overview**.
3. **Authentication → Advanced settings → Allow public client flows → Yes.** A desktop or mobile
   app cannot keep a secret, so it must be a public client. Without this, sign-in fails with
   `unauthorized_client` or `invalid_client`.
4. **API permissions** must include **Microsoft Graph → Delegated → `User.Read`** (new
   registrations usually have it already). It is what the *Microsoft Graph* page calls `/me` with.
   `User.Read` is user-consentable, so no admin consent is required.

## 2. Configure the app

Edit **`Authentication.MsalExtensionsDemo/appsettings.development.json`**:

```json
"MsalAuthentication": {
  "ClientId": "00000000-0000-0000-0000-000000000000",
  "TenantId": "consumers",
  "Scopes": [ "User.Read" ]
}
```

The section name is not arbitrary — it is the `name:` passed to
`auth.AddMsal(window, name: "MsalAuthentication")` in `App.xaml.cs`.

Two things to know about which file wins:

- **Debug builds run in the Development environment** (`App.xaml.cs` calls
  `.UseEnvironment(Environments.Development)` under `#if DEBUG`), so
  `appsettings.development.json` is the file that applies while you are testing. For a **Release**
  build, put the same values in `appsettings.json`.
- **`appsettings*.json` are embedded resources**, not copied to the output directory. After editing
  them you must **rebuild** — restarting the app is not enough.

No redirect URI goes in configuration. The provider derives each platform's conventional value;
setting `"RedirectUri"` explicitly overrides that and is not needed here.

## 3. Register the redirect URI for the head you are running

**Run the app first, and read the value off the *Sign in* page** — it shows the exact string the
provider computed, under **Redirect URI to register**. Paste that, rather than typing it: a
one-character difference is the single most common cause of MSAL failures.

| Head | Redirect URI | Register under (**Authentication → Add a platform**) |
| --- | --- | --- |
| Desktop (Skia) | `http://localhost` | Mobile and desktop applications — tick the `http://localhost` box |
| WebAssembly | `http://localhost:5000/authentication-callback` | **Single-page application** |
| Android | `msal{ClientId}://auth` | Mobile and desktop applications → **Custom redirect URI** |
| iOS | `msauth.com.companyname.authentication-msalextensionsdemo://auth` | iOS/macOS — enter the bundle ID and the portal builds the URI |

The platform type is not cosmetic: see the WebAssembly and Android notes below.

---

## Per-head setup

The commands below are run from this folder (`UI/Authentication.MsalExtensionsDemo`). Visual Studio,
Rider (`.run/Authentication.MsalExtensionsDemo.run.xml`) and VS Code (`.vscode/launch.json`)
configurations are also present, and are easier for the mobile heads.

### Desktop (Skia)

**Project changes:** none.

**Entra ID:** register `http://localhost` under *Mobile and desktop applications*. MSAL picks a
free port at runtime and Entra ID ignores the port on `localhost`, so the one entry covers every
run.

```bash
dotnet run --project Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo.csproj -f net10.0-desktop
```

**While testing:** the desktop token cache is **persisted** (DPAPI on Windows, keychain on macOS,
keyring on Linux), so the second launch signs in silently with no prompt. That is the point of the
sample's startup step — but it also means that to test the *interactive* path again you have to
press **Sign out** first.

An abandoned sign-in (you closed the browser tab) cannot be detected, so the provider cancels it
after `InteractiveTimeout` — 5 minutes by default, settable in the `MsalAuthentication` section
(`00:00:00` waits forever).

### WebAssembly

**Project changes:** none. The provider derives the redirect URI from Uno's
`WebAuthenticationBroker` (`{origin}/authentication-callback`) and applies `WithUnoHelpers()`,
which supplies the popup-based web UI. There is no callback page to write: the popup only has to
land on a same-origin URL for Uno to read the code back out of it, and
`Platforms/WebAssembly/wwwroot/staticwebapp.config.json` already rewrites unknown paths to
`index.html`. If you host the app somewhere else, give it the same fallback.

**Entra ID:** register the origin you serve from, plus `/authentication-callback`, as a
**Single-page application**:

```
http://localhost:5000/authentication-callback
```

Only SPA redirect URIs get CORS enabled on the token endpoint. Registered as *Mobile and desktop*
or *Web* instead, the browser's token request is rejected with `AADSTS90023: cross-origin token
redemption is permitted only for the 'Single-Page Application' client-type`.

Entra ID ignores the port for `localhost`, but **the path must match exactly**. The port `5000`
comes from `Authentication.MsalExtensionsDemo/Properties/launchSettings.json`; each non-localhost
origin you serve from (staging, production) needs its own entry.

```bash
dotnet run --project Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo.csproj -f net10.0-browserwasm
# serves http://localhost:5000
```

**While testing:** the token cache is **in memory** — MSAL's cache persistence relies on APIs the
browser does not have. A page reload signs you out and the next run needs an interactive sign-in.
That is expected, and the app logs one Information message saying so.

Sign-in happens in a popup whose URL Uno polls, so the popup must not be blocked and the serving
origin must not set `Cross-Origin-Opener-Policy` (leave it unset, or `unsafe-none`) — otherwise the
flow opens a window and hangs.

### Android

**Project change — required.** Open
`Authentication.MsalExtensionsDemo/Platforms/Android/MsalActivity.Android.cs` and replace the
placeholder with your own client ID:

```csharp
// Must be "msal" + the ClientId from the MsalAuthentication configuration section.
private const string RedirectScheme = "msal00000000-0000-0000-0000-000000000000";
```

This is the one value that **cannot** follow appsettings automatically: intent filters are declared
with attributes, which only accept compile-time constants. Leave it at the placeholder and the
browser has nowhere to hand back the authorization code — the interactive sign-in simply never
completes.

**Entra ID:** register the custom URI

```
msal{your-client-id}://auth
```

under *Mobile and desktop applications → Custom redirect URI*. Do **not** use the portal's Android
platform option: it generates `msauth://{package}/{signature-hash}`, which is the format for
broker-based sign-in through Microsoft Authenticator. This sample uses the system browser, so no
signing-key hash is involved.

```bash
dotnet build Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo.csproj -f net10.0-android -t:Run
```

**Already wired up, nothing to change:** `MainActivity.OnActivityResult` forwards to
`AuthenticationContinuationHelper` so MSAL can resume the pending request.

**On the device:** a browser must be installed, or MSAL throws `AndroidActivityNotFound`. Browsers
without Chrome custom tab support (DuckDuckGo, UC Browser) report the flow as cancelled. The token
cache is persisted, so a restart goes through the silent path.

### iOS

**Project changes:** none, *unless you change the bundle identifier* — see
[Changing the application ID](#changing-the-application-id) below. Both iOS-specific files already
carry the right values for the shipped `<ApplicationId>`:

| File | What it does | Change it when |
| --- | --- | --- |
| `Platforms/iOS/Info.plist` | `CFBundleURLTypes` declares the `msauth.{BundleId}` URL scheme so iOS routes the callback back into the app. Without it the browser closes and nothing happens. | you change `<ApplicationId>` |
| `Platforms/iOS/Entitlements.plist` | Grants `keychain-access-groups` — `$(AppIdentifierPrefix)$(CFBundleIdentifier)` and `$(AppIdentifierPrefix)com.microsoft.adalcache`. MSAL keeps its token cache in the keychain; without this the first token save fails with `missing_entitlements` (or, on some MSAL versions, `cannot_access_publisher_keychain` even earlier). | never, for this sample |

`$(AppIdentifierPrefix)` expands at build time to the Team ID from the provisioning profile, so
nothing here names your team. Development profiles carry `{TeamId}.*` and sign without any extra
capability; a **distribution** build needs Keychain Sharing enabled on the App ID.

**Entra ID:** *Authentication → Add a platform → iOS/macOS*, enter the bundle ID
`com.companyname.authentication-msalextensionsdemo`, and the portal produces
`msauth.com.companyname.authentication-msalextensionsdemo://auth`.

```bash
dotnet build Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

> **Edits to `Info.plist` or `Entitlements.plist` can be silently skipped by an incremental build.**
> Both are consumed by build steps whose up-to-date checks do not include the source file, so the
> app can run with the previous contents and the edit looks like it did nothing. After editing
> either, delete the iOS output and rebuild:
>
> ```bash
> rm -rf Authentication.MsalExtensionsDemo/bin/Debug/net10.0-ios Authentication.MsalExtensionsDemo/obj/Debug/net10.0-ios
> ```

**On the device:** the token cache is persisted in the keychain, so a restart goes through the
silent path.

### Windows (WinAppSDK)

Not one of this sample's four heads. To add it, add `net10.0-windows10.0.19041.0` to
`<TargetFrameworks>` and build on Windows — **no code changes**. On WinAppSDK the provider leaves
the redirect URI to the Web Account Manager broker, so register
`ms-appx-web://microsoft.aad.brokerplugin/{ClientId}` under *Mobile and desktop applications*.

---

## Changing the application ID

`<ApplicationId>` in `Authentication.MsalExtensionsDemo.csproj` is both the Android package name and
the Apple bundle identifier. If you change it from
`com.companyname.authentication-msalextensionsdemo`, change all four together:

1. `<ApplicationId>` in `Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo.csproj`
2. `CFBundleURLSchemes` in `Authentication.MsalExtensionsDemo/Platforms/iOS/Info.plist` — must be
   `msauth.{your id}` (and `CFBundleURLName` alongside it, which is display only)
3. `PlatformGuide.ApplicationId` in
   `Authentication.MsalExtensionsDemo/Authentication/PlatformGuide.cs` — display only, used by the
   *Platform setup* page
4. The iOS redirect URI on the app registration — `msauth.{your id}://auth`

The Android redirect URI is derived from the **client ID**, not the application ID, so it is
unaffected.

---

## Checking that it worked

1. **Sign in** page → **Sign in**. The flow log narrates every provider call; the resolved
   configuration, the derived redirect URI and the resulting token are shown above it.
2. **Sign out**, then **Silent only** — expect it to fail, which is what the silent path does with
   an empty cache.
3. Restart the app on Desktop, Android or iOS: the startup silent refresh should sign you back in
   with no prompt. On WebAssembly it will not, by design.
4. **Microsoft Graph** page → `GET /v1.0/me` with the token in an `Authorization: Bearer` header,
   proving a real API accepts it.

The **Redact** switch in the header hides client and tenant IDs, the account and the Graph response
across all pages — useful when recording, but remember it is on if a value you expect to see is
masked.

---

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| The *Sign in* page says no app registration is configured | `ClientId` is empty or not a GUID | Step 2 — and **rebuild**, since appsettings are embedded resources |
| Your edit to appsettings has no effect | Editing `appsettings.json` while running a Debug build | Debug runs the Development environment: edit `appsettings.development.json` |
| `AADSTS50011` / redirect URI mismatch | The registered URI is not exactly what was sent | Copy the value off the *Sign in* page |
| `AADSTS90023` cross-origin token redemption… | The WebAssembly URI is registered under the wrong platform type | Re-register it under **Single-page application** |
| `unauthorized_client` / `invalid_client` | Not a public client, or the client ID is not in that tenant | *Allow public client flows → Yes*; check `ClientId` and `TenantId` |
| `AADSTS50020` / account not found in tenant | `TenantId` and the registration's supported account types disagree | Match them — see the table in step 1 |
| **Android:** browser opens, sign-in completes, nothing comes back | `RedirectScheme` in `MsalActivity.Android.cs` is still the placeholder, or does not match the client ID | Step 4 in the summary table, then rebuild |
| **Android:** `AndroidActivityNotFound` | No browser on the device | Install one with custom tab support |
| **iOS:** browser closes and nothing happens | `CFBundleURLSchemes` missing or not `msauth.{BundleId}` | Fix `Info.plist`, then **clean-build** the iOS head |
| **iOS:** `missing_entitlements` / `cannot_access_publisher_keychain` | The keychain access group did not reach the app | It is already in `Entitlements.plist` — this is almost always the incremental-build caveat above |
| **WebAssembly:** popup opens and never closes | `Cross-Origin-Opener-Policy` on the serving origin | Leave COOP unset, or `unsafe-none` |
| **WebAssembly:** signed out after a reload | The browser cache is in memory | Expected — MSAL cache persistence is not available in the browser |
| **Desktop:** browser completes, app keeps waiting | `http://localhost` not registered | Tick it under *Mobile and desktop applications* |
| `authentication_canceled` on desktop with no cancel | The 5-minute `InteractiveTimeout` elapsed | Finish the sign-in sooner, or raise `InteractiveTimeout` in the `MsalAuthentication` section |
| Graph returns `403` | Scope not granted | Add **Microsoft Graph → Delegated → `User.Read`** |

---

## What you do *not* have to configure

Because the provider owns the `IPublicClientApplication`, several things the plain-MSAL sample makes
you write are handled for you:

- **The redirect URI per platform** — derived, and shown on screen for you to register.
- **`WithUnoHelpers()`** — applied by the provider, including the WebAssembly popup web UI and the
  parent activity/view controller on mobile.
- **Token cache persistence** — wired up on desktop (DPAPI / keychain / keyring), Android and iOS.
- **Silent-then-interactive acquisition** — one `LoginAsync` call; `RefreshAsync` is the silent path.

## Relevant documentation

- [Uno.Extensions MSAL authentication](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/HowTo-MsalAuthentication.html)
- [Using MSAL with Uno Platform](https://platform.uno/docs/articles/interop/MSAL.html)
- [MSAL.NET documentation](https://learn.microsoft.com/entra/msal/dotnet/)
- [Microsoft Graph `/me` endpoint](https://learn.microsoft.com/graph/api/user-get)
