# Web (OAuth) authentication on Uno Platform, via Uno.Extensions

Signs in against the **public [Duende demo identity server](https://demo.duendesoftware.com)**
using **`Uno.Extensions.Authentication`'s Web provider** (`AddWeb`), and narrates every step on
screen so the flow is visible on all four heads (Desktop/Skia, WebAssembly, Android, iOS).

No registration is needed: the demo server accepts arbitrary redirect URIs and ships a test user
(**bob / bob**).

## Web provider vs. OIDC provider

The companion sample `UI/Authentication.OidcExtensionsDemo` signs in against the same server with
`AddOidc`, which owns the whole protocol via Duende's `OidcClient`. `AddWeb` is the
bring-your-own-protocol version: the provider only drives the platform's browser surface
(`WebAuthenticationBroker`) and stores whatever tokens the app's callbacks hand back — which is
what makes it fit any OAuth-ish endpoint, not just OpenID Connect ones. This sample supplies the
protocol pieces in `DuendeOAuthClient` and plugs them in through the `AddWeb` callbacks:

| Callback | What this sample does with it |
| --- | --- |
| `PrepareLoginStartUri` | builds the authorization request with a fresh PKCE challenge and the broker-derived redirect URI |
| `PrepareLoginCallbackUri` | supplies the broker-derived redirect URI |
| `PostLogin` | exchanges the authorization code (plus PKCE verifier) at the token endpoint |
| `Refresh` | redeems the stored refresh token — the silent path `RefreshAsync` runs at startup |
| `PrepareLogoutStartUri` | builds the end-session URL with the id_token hint |

## The page

- **Web provider configuration** — authority, client, scope, and the redirect URI the platform's
  `WebAuthenticationBroker` derives at runtime.
- **Actions** — `Sign in`, `Silent only`, `Call demo API` (`GET /api/test` with the access token,
  echoing its claims), `Sign out`.
- **Flow log** — every `IAuthenticationService` call, timestamped, with what happened.

## What each platform uses for sign-in

| Head | Browser surface | Redirect returns via |
| --- | --- | --- |
| Desktop (Windows/macOS/Linux, Skia) | system browser | the `http://localhost` loopback listener Uno.Extensions registers as the desktop `WebAuthenticationBroker` |
| WebAssembly | browser popup | the app's own origin (allow popups) |
| Android | Chrome custom tab | the `web-ext-demo://` intent filter on `WebAuthenticationBrokerActivity` |
| iOS | `ASWebAuthenticationSession` | the `web-ext-demo://` URL scheme in `Info.plist` |

## Notes

- On startup the app runs the silent path first, like a production app should: tokens persist in
  the platform's key-value storage, so a restart signs back in without UI until the refresh token
  expires.
- All four heads use the Skia renderer (`SkiaRenderer` in `UnoFeatures`), which also exercises
  Uno's runtime-asset substitution of the extensions assemblies on the mobile heads.
