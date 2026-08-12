# MSAL in an Uno Platform app

A worked example of signing users in with **MSAL.NET** (Microsoft Entra ID, formerly Azure AD) from a
single Uno Platform project that targets Android, iOS, WebAssembly and Desktop.

It uses only:

| Package | Why |
| --- | --- |
| `Uno.WinUI.MSAL` | Uno's MSAL add-in. Supplies `.WithUnoHelpers()`, which resolves the parent activity/view controller on Android and iOS. Its WebAssembly helpers — a popup-based web UI and an `HttpClient` factory — are **not** available under the Skia renderer this app uses. |
| `Microsoft.Identity.Client` | MSAL.NET itself. |

There is deliberately **no Uno.Extensions** here — no host builder, no DI container, no navigation
framework. The authentication service is a plain singleton, so you can lift
`Authentication/AuthenticationService.cs` into an app of any shape.

The app enables Uno's `SkiaRenderer` feature (`<UnoFeatures>` in `Authentication.MsalDemo.csproj`), so every head —
WebAssembly included — is drawn with Skia rather than native platform elements. That choice decides
which flavour of `Uno.UI.MSAL.dll` gets loaded and therefore what `.WithUnoHelpers()` actually does.
It is why **interactive sign-in does not work on the WebAssembly head** as this sample stands; see
[WebAssembly and the Skia renderer](#webassembly-and-the-skia-renderer). The other three heads are
unaffected.

Reference: <https://platform.uno/docs/articles/interop/MSAL.html>

---

## Quick start

1. **Register an application** in the [Microsoft Entra admin center](https://entra.microsoft.com)
   (details in [1. Register the application](#1-register-the-application)).
2. **Paste the Application (client) ID** into `Authentication.MsalDemo/Authentication/MsalConfig.cs`.
3. **Run the app.** The *Sign in* page prints the exact redirect URI that head will use — register
   that string on the app registration, then press **Sign in**.

Until step 2 is done the app runs and explains itself rather than failing silently: the *Sign in*
page shows a "No app registration configured" banner, and pressing **Sign in** logs exactly what is
missing.

---

## What the sample shows

| Page | What it demonstrates |
| --- | --- |
| **Sign in** | The flow a production app should use, narrated step by step: look in the token cache → `AcquireTokenSilent` → `MsalUiRequiredException` → `AcquireTokenInteractive`. Also shows the resolved configuration, the platform, the redirect URI in use, the cached accounts, and local sign-out. |
| **Microsoft Graph** | `GET https://graph.microsoft.com/v1.0/me` with the access token in an `Authorization: Bearer` header, so you can see that the token is real and accepted by an API. |
| **Platform setup** | The Entra ID and project requirements for every head, with the one you are running on marked. Same content as this document, available on-device. |

---

## 1. Register the application

In the [Microsoft Entra admin center](https://entra.microsoft.com):

1. **Applications → App registrations → New registration.**
   - Choose **Supported account types** to match `MsalConfig.Tenant`. The sample ships with
     `common`, which means work/school accounts *and* personal Microsoft accounts — so pick
     *"Accounts in any organizational directory and personal Microsoft accounts"*. For a
     single-tenant app, select *"…this organizational directory only"* and set `MsalConfig.Tenant`
     to your tenant ID or domain.
   - Leave the redirect URI blank for now.
2. Copy the **Application (client) ID** from **Overview**.
3. **Authentication → Advanced settings → Allow public client flows → Yes.**
   A desktop or mobile app cannot keep a secret, so it has to be registered as a public client.
   Without this, sign-in fails with `unauthorized_client` or `invalid_client`.
4. **API permissions** should include **Microsoft Graph → Delegated → `User.Read`** (new
   registrations usually have it). It is what the Microsoft Graph page needs. `User.Read` is
   user-consentable, so no admin consent is required.

---

## 2. Redirect URIs — one per platform

The redirect URI is where almost all MSAL setup problems live. Each head needs its own, and each
must be registered under the right **platform** in **Authentication → Add a platform**.

| Head | Redirect URI | Register under | Notes |
| --- | --- | --- | --- |
| Desktop (Skia) | `http://localhost` | Mobile and desktop applications | Tick the `http://localhost` checkbox. MSAL picks a free port at runtime, and Entra ID allows any port on `localhost`, so one entry covers them all. |
| WebAssembly | `http://localhost:5000/authentication/login-callback.htm` | **Single-page application** | Must match your origin exactly. One entry per origin you serve from (dev, staging, production). Registering it does not make this head sign in — see [WebAssembly and the Skia renderer](#webassembly-and-the-skia-renderer). |
| Android | `msal{ClientId}://auth` | Mobile and desktop applications → *Custom redirect URI* | For example `msal00000000-0000-0000-0000-000000000000://auth`. |
| iOS / Mac Catalyst | `msauth.com.companyname.authentication.msaldemo://auth` | iOS/macOS | Enter the bundle ID (`<ApplicationId>` in `Authentication.MsalDemo.csproj`) and the portal builds the URI. |
| Windows (WinAppSDK) | `http://localhost` | Mobile and desktop applications | Not one of this sample's four heads; see [Adding a Windows head](#adding-a-windows-head). |

The running app always shows the value it computed, under **Redirect URI to register** on the
*Sign in* page. Copy from there rather than typing it — that is the string MSAL will actually send.

### WebAssembly must be registered as a Single-page application

Background for whenever the WebAssembly head can sign in again — it is a fact about Entra ID, not
about the renderer, so it stays true. In the browser, MSAL redeems the authorization code with a
`fetch` call from your origin, and **only SPA-type redirect URIs have CORS enabled on the token
endpoint**. Register the same URI under *Mobile and desktop applications* instead and sign-in fails
with:

> cross-origin token redemption is permitted only for the 'Single-Page Application' client-type

### Android uses `msal{ClientId}://auth`, not a signature hash

The portal's **Android** platform option asks for a package name and signing-key signature hash and
produces `msauth://<package>/<hash>`. That format is for **broker**-based sign-in through Microsoft
Authenticator. This sample uses the system browser, so use a **custom redirect URI** of
`msal{ClientId}://auth` instead and no signing key is involved.

---

## 3. Configure the app

Everything you need to edit is in **`Authentication.MsalDemo/Authentication/MsalConfig.cs`**:

```csharp
public const string ClientId = "00000000-0000-0000-0000-000000000000"; // ← your Application (client) ID
public const string Tenant   = "common";  // or "organizations", "consumers", or a tenant ID/domain

public static readonly string[] Scopes =
[
    "https://graph.microsoft.com/User.Read"
];
```

The Android intent filter is generated from `ClientId` at compile time
(`Platforms/Android/MsalActivity.Android.cs` uses `MsalConfig.AndroidRedirectScheme`, which is a
`const`), so the manifest can never drift from the configured client ID.

### If you change the application ID

`<ApplicationId>` in `Authentication.MsalDemo.csproj` is the Android package name and the Apple bundle identifier.
If you change it from `com.companyname.authentication.msaldemo`, update all three of these together:

1. `<ApplicationId>` in `Authentication.MsalDemo/Authentication.MsalDemo.csproj`
2. `CFBundleURLSchemes` in `Authentication.MsalDemo/Platforms/iOS/Info.plist` — must be `msauth.<your id>`
3. `PlatformGuide.ApplicationId` in `Authentication.MsalDemo/Authentication/PlatformGuide.cs` (display only)
4. The iOS redirect URI on the app registration

---

## Running each head

From the repository root. Visual Studio, Rider (`.run/Authentication.MsalDemo.run.xml`) and VS Code
(`.vscode/launch.json`) configurations are also present and are easier for the mobile heads.

**Desktop (Skia)**

```bash
dotnet run --project Authentication.MsalDemo/Authentication.MsalDemo.csproj -f net10.0-desktop
```

**WebAssembly**

```bash
dotnet run --project Authentication.MsalDemo/Authentication.MsalDemo.csproj -f net10.0-browserwasm
# serves http://localhost:5000
```

The port matters — it is part of the origin, and therefore part of the redirect URI you registered.
`http://localhost:5000` comes from `Authentication.MsalDemo/Properties/launchSettings.json`.

> **Interactive sign-in does not work on this head.** The app builds, runs and navigates, and the
> *Sign in* page still shows the resolved configuration, the platform and the redirect URI — but
> pressing **Sign in** cannot show a sign-in UI, because under the Skia renderer Uno supplies no
> web UI to MSAL in the browser. [WebAssembly and the Skia
> renderer](#webassembly-and-the-skia-renderer) explains why and what the options are.
>
> This also makes the old `Cross-Origin-Opener-Policy` advice moot: COOP mattered only because Uno
> used to read a popup's URL to extract the authorization code, and there is no popup any more.

**Android** (emulator or device connected)

```bash
dotnet build Authentication.MsalDemo/Authentication.MsalDemo.csproj -f net10.0-android -t:Run
```

**iOS** (simulator)

```bash
dotnet build Authentication.MsalDemo/Authentication.MsalDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

---

## Where the code is

| File | What it does |
| --- | --- |
| `Authentication/MsalConfig.cs` | The only file you edit: client ID, tenant, scopes. |
| `Authentication/AuthenticationService.cs` | All MSAL usage. Builds the `IPublicClientApplication`, runs silent-then-interactive acquisition, signs out, and narrates every step. |
| `Authentication/PlatformSupport.cs` | `partial` declaration of the per-platform facts (redirect URI, platform name). |
| `Platforms/<platform>/PlatformSupport.*.cs` | One implementation per head. Uno's single project compiles only the folder matching the target framework, so these files need no `#if` blocks — and adding a head without giving MSAL a redirect URI is a compile error. (`AuthenticationService` itself does use `#if`, for the parent activity/window.) |
| `Authentication/PlatformGuide.cs` | The reference content shown on the *Platform setup* page. |
| `Authentication/AuthFlowLog.cs` | The observable step-by-step log. |
| `Services/GraphClient.cs` | The Microsoft Graph `/me` call. |
| `Views/` | The three pages and their view models. No MVVM framework — a 20-line `ObservableObject`. |

Platform plumbing:

| File | What it does |
| --- | --- |
| `Platforms/Android/MsalActivity.Android.cs` | Activity deriving from MSAL's `BrowserTabActivity` with an intent filter for `msal{ClientId}://auth`. Without a matching intent filter the browser has nowhere to hand back the code. |
| `Platforms/Android/MainActivity.Android.cs` | Forwards `OnActivityResult` to `AuthenticationContinuationHelper` so the pending request completes. |
| `Platforms/iOS/MsalAppDelegate.iOS.cs` | Derives from Uno's `UnoUIApplicationDelegate` and forwards `OpenUrl` to `AuthenticationContinuationHelper`. |
| `Platforms/iOS/Main.iOS.cs` | Installs it with `.UseAppleUIKit(b => b.UseUIApplicationDelegate<MsalAppDelegate>())`. |
| `Platforms/iOS/Info.plist` | Declares the `msauth.<bundle id>` URL scheme. |
| `Platforms/iOS/Entitlements.plist` | Grants the `$(AppIdentifierPrefix)com.microsoft.adalcache` keychain access group. MSAL stores its token cache in the keychain and reads the publisher's Team ID back out of it, so without this entitlement `PublicClientApplicationBuilder.Build()` throws `cannot_access_publisher_keychain`. The Uno SDK picks this file up automatically as `CodesignEntitlements`. |
| `Platforms/WebAssembly/wwwroot/authentication/login-callback.htm` | Static page on our own origin for a sign-in popup to land on. Vestigial under the Skia renderer — nothing opens or polls a popup any more — but it is still what the registered SPA redirect URI points at, so keep it if you intend to restore a browser flow. |

---

## How the flow works

The whole point of MSAL is that you should almost never show a sign-in prompt. The pattern, from
`AuthenticationService.SignInAsync`:

```csharp
// 1. Is there an account in the cache?
var accounts = await app.GetAccountsAsync();

if (accounts.FirstOrDefault() is { } account)
{
    try
    {
        // 2. Serve from the cache, refreshing silently if needed. No UI.
        return await app.AcquireTokenSilent(scopes, account).ExecuteAsync(ct);
    }
    catch (MsalUiRequiredException)
    {
        // Expected: consent, MFA, a password change or an expired refresh token needs the user.
    }
}

// 3. Only now show UI.
return await app.AcquireTokenInteractive(scopes)
    .WithPrompt(Prompt.SelectAccount)
    .WithUnoHelpers()          // ← Uno: parent activity/view controller on Android and iOS
    .ExecuteAsync(ct);
```

`MsalUiRequiredException` is a control-flow signal, not a bug. The *Sign in* page shows it as a
warning step, not an error, precisely to make that clear.

### What supplies the platform-specific pieces

Two things need a platform answer: **who is the parent** of the sign-in UI on mobile, and **what
shows the web UI** in the browser. Normally `.WithUnoHelpers()` answers both. Right now it cannot,
so the parent is set explicitly on the builder in `GetOrCreateApp`:

```csharp
_app = PublicClientApplicationBuilder
    .Create(MsalConfig.ClientId)
    .WithAuthority(AzureCloudInstance.AzurePublic, MsalConfig.Tenant)
    .WithRedirectUri(PlatformSupport.RedirectUri)
    //.WithUnoHelpers()        // ← temporary workaround, see below
#if ANDROID
    .WithParentActivityOrWindow(() => Uno.UI.ContextHelper.Current as Android.App.Activity)
#elif IOS
    .WithParentActivityOrWindow(() => UIKit.UIApplication.SharedApplication?.KeyWindow?.RootViewController)
#endif
    .Build();
```

> [!IMPORTANT]
> **This `#if` block is a temporary workaround for
> [unoplatform/uno#20601](https://github.com/unoplatform/uno/issues/20601).** With the `SkiaRenderer`
> feature enabled, every head loads the `skia` flavour of `Uno.UI.MSAL`, where `.WithUnoHelpers()` is
> a no-op — so on Android MSAL is left with no parent `Activity` and `AcquireTokenInteractive` fails
> with `activity_required`. The fix is
> [unoplatform/uno#24055](https://github.com/unoplatform/uno/pull/24055), which makes the build
> deploy the platform flavour of `Uno.UI.MSAL` under `SkiaRenderer`. **Once it has merged and
> shipped, delete the `#if` block and restore the single `.WithUnoHelpers()` call.**

On Android and iOS these are the same values a working `.WithUnoHelpers()` supplies — the package's
mobile builds resolve them from `ContextHelper.Current` and the key window's `RootViewController`
respectively — so the explicit calls stand in for the helper rather than doing anything different.
Desktop and Windows need neither branch: MSAL.NET launches the system browser and collects the code
on an `http://localhost` loopback listener. There is no WebAssembly branch either, because the popup
web UI and `WasmHttpFactory` exist only in the `webassembly` flavour of the assembly and cannot be
referenced from a head that compiles against the `skia` one — see the next section.

`.WithUnoHelpers()` on the interactive request (step 3 above) is still called, unconditionally, and
is the only Uno-specific line left in the flow. It is a no-op today for the same reason, and starts
working again with the same fix.

---

## WebAssembly and the Skia renderer

`.WithUnoHelpers()` is not one implementation. `Uno.WinUI.MSAL` ships a different `Uno.UI.MSAL.dll`
per runtime flavour, and the build picks one:

| Flavour in the package | What `.WithUnoHelpers()` does there |
| --- | --- |
| `lib/net10.0-android`, `lib/net10.0-ios26.0` | Supplies `WithParentActivityOrWindow` from `ContextHelper.Current` / the key window's `RootViewController`. |
| `uno-runtime/net10.0/webassembly` | Supplies a `WasmWebUi` — an `ICustomWebUi` that opens a popup with `window.open` and polls its URL — plus an `IMsalHttpClientFactory`. |
| `uno-runtime/net10.0/skia` | Nothing. It is a no-op. |

Because `<UnoFeatures>SkiaRenderer</UnoFeatures>` is enabled, the **`skia` flavour is what the
`net10.0-browserwasm` head deploys** — the `Uno.UI.MSAL.dll` in
`bin/Debug/net10.0-browserwasm/` is byte-for-byte the one from `uno-runtime/net10.0/skia`, not the
`webassembly` one. The popup web UI is in a file this app never loads.

So on the WebAssembly head no `ICustomWebUi` is registered, and `AcquireTokenInteractive` falls back
to MSAL.NET's default web UI — which launches a browser process and listens on a loopback port.
Neither is possible from inside the browser sandbox, so the interactive call cannot complete. The
silent path, `GetAccountsAsync`, sign-out and the Graph call are all unaffected; only interactive
sign-in is.

### Options, none of them implemented here

- **Build the WebAssembly head without the Skia renderer**, so the `webassembly` flavour of the
  package is loaded and the popup flow returns. `<UnoFeatures>` can be set per target framework.
  This gives up Skia rendering on that head, and it has not been tried in this repository.
- **Supply your own `ICustomWebUi`** and pass it with `.WithCustomWebUi(...)`. It has to open a
  window, wait until its URL is the redirect URI, and return that URI to MSAL — the same contract
  `WasmWebUi` implements. This is renderer-independent, but it is code you write and maintain.

Both are directions rather than verified recipes. What *is* verified is the diagnosis above: the
deployed assembly is the no-op one.

---

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| `AADSTS50011` / redirect URI mismatch | The registered URI is not exactly what MSAL sent | Copy the value from **Redirect URI to register** on the *Sign in* page. On WebAssembly, protocol, host and port must all match; a different port is a different origin. |
| "cross-origin token redemption is permitted only for the 'Single-Page Application' client-type" | The WebAssembly URI is registered under the wrong platform | Register it under **Single-page application**. |
| `unauthorized_client` or `invalid_client` | Not registered as a public client, or the client ID does not exist in the tenant | **Authentication → Allow public client flows → Yes**, and check `MsalConfig.ClientId` / `Tenant`. |
| `access_denied` | Consent declined | The user or an administrator refused the requested scopes. |
| `authentication_canceled` | The user closed the sign-in window | On Android, iOS and Desktop this means what it says. |
| **WebAssembly:** pressing *Sign in* fails or does nothing after the silent step | Not a configuration problem — no web UI is available to MSAL in the browser under the Skia renderer | [WebAssembly and the Skia renderer](#webassembly-and-the-skia-renderer). No redirect URI or Entra setting fixes this. |
| **Android:** app hangs after signing in | `OnActivityResult` not forwarded, or no intent filter for the scheme | Both are wired in this sample; if you copied only part of it, check `MsalActivity.Android.cs` and `MainActivity.OnActivityResult`. |
| **Android:** `AndroidActivityNotFound` | No browser on the device | Install one; prefer a browser with custom tab support. |
| **iOS:** browser closes and nothing happens | `CFBundleURLSchemes` missing or not `msauth.<bundle id>`, or `OpenUrl` not forwarded | Check `Info.plist` and `MsalAppDelegate.iOS.cs`. |
| **iOS:** `cannot_access_publisher_keychain` — "the TeamId is null" — thrown from `Build()` before any UI | No `keychain-access-groups` entitlement, so MSAL cannot probe the keychain for the publisher's Team ID | Check that `Platforms/iOS/Entitlements.plist` has the `keychain-access-groups` array. If it is already there, **rebuild** — see the note below. |
| **Desktop:** browser opens, sign-in completes, app keeps waiting | `http://localhost` not registered | Tick it under *Mobile and desktop applications*. |
| `MsalUiRequiredException` on **Silent only** | No usable token in the cache | Expected. Sign in interactively first — and note the cache is in memory on Desktop and WebAssembly. |
| Graph returns `401` | The token was not accepted | Check that the token was requested for Graph — the scope must be a Graph scope. |
| Graph returns `403` | Scope not granted | Add **Microsoft Graph → Delegated → `User.Read`**. |

### Editing entitlements needs a clean iOS build

Entitlements reach an iOS **simulator** build differently than a device build. `codesign` cannot
apply team-scoped entitlements without a real signing identity, so the build signs the bundle with
an empty entitlements file and instead embeds the real ones — with `$(AppIdentifierPrefix)` expanded
to your Team ID — into a `__TEXT/__entitlements` section of the app executable.

That embedding happens during the native link, and the native link is cached on inputs that do not
include the entitlements file. So after editing `Entitlements.plist`, an incremental build recompiles
the entitlements but reuses the cached executable, and the app runs with the old set — which looks
exactly like the edit having no effect. Delete the iOS output and rebuild:

```bash
rm -rf Authentication.MsalDemo/bin/Debug/net10.0-ios Authentication.MsalDemo/obj/Debug/net10.0-ios
dotnet build Authentication.MsalDemo/Authentication.MsalDemo.csproj -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64
```

To confirm what the app actually got — `codesign -d --entitlements` reports the empty signing set on
simulator builds and tells you nothing useful, so read the executable instead:

```bash
strings -a Authentication.MsalDemo/bin/Debug/net10.0-ios/iossimulator-arm64/Authentication.MsalDemo.app/Authentication.MsalDemo | grep adalcache
```

---

## Token cache persistence

MSAL caches tokens so it can refresh them silently. Where that cache lives differs per platform, and
it changes what a restart does:

| Head | Cache | Effect |
| --- | --- | --- |
| Android | Persisted by MSAL in platform secure storage | A restart can go straight through the silent path. |
| iOS / Mac Catalyst | Persisted in the iOS keychain | Same. |
| Desktop (Skia) | In memory only | Every run starts with an interactive sign-in. |
| WebAssembly | In memory only — Uno does not persist it in the browser | A page reload signs out. Moot in practice while interactive sign-in cannot run there, so the cache never gets populated. |

To persist the cache on desktop, add the `Microsoft.Identity.Client.Extensions.Msal` package and
register a `MsalCacheHelper` against the application's `UserTokenCache`. This sample leaves it out to
keep the dependency list to MSAL alone.

---

## Adding a Windows head

There is no `#if` branch for Windows and `.WithUnoHelpers()` is a no-op on WinAppSDK, so **no
authentication code changes**. Like Desktop, it uses the system browser and the loopback listener.
Add the target framework and build on Windows:

```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-browserwasm;net10.0-desktop;net10.0-windows10.0.19041.0</TargetFrameworks>
```

Register `http://localhost` under *Mobile and desktop applications*, the same as the Desktop head.

> The four heads in this repository are all built and verified. The Windows head is documented but
> not included, because it cannot be compiled from macOS.

---

## Optional: broker and single sign-on

Broker sign-in routes authentication through Microsoft Authenticator / Company Portal (mobile) or the
Windows Web Account Manager, which enables SSO across apps, Conditional Access enforcement and device
compliance checks. It needs the `Microsoft.Identity.Client.Broker` package, `.WithBroker(...)`, and
different redirect URIs — `msauth://<package>/<signature-hash>` on Android and
`ms-appx-web://microsoft.aad.brokerplugin/{ClientId}` on Windows — plus the extra `Info.plist` and
`AndroidManifest.xml` entries flagged in comments in this sample.

See [Authenticate users with MSAL.NET](https://learn.microsoft.com/dotnet/maui/data-cloud/authentication)
for the details; the platform requirements are the same for Uno as for .NET MAUI.

---

## What this sample deliberately does not do

- **No Uno.Extensions.** No generic host, DI, navigation or MVUX — so the MSAL parts are not tangled
  up with a particular app architecture. In a real app, register `AuthenticationService` as a
  singleton instead of using `AuthenticationService.Instance`.
- **No token or claims dump.** The access token's length and expiry are shown, not its contents.
  Treat access tokens as secrets: do not log them.
- **No persistent cache on desktop or WebAssembly**, as described above.
- **No refresh-token handling of your own.** `AcquireTokenSilent` does it. Never store or manage
  refresh tokens yourself.
