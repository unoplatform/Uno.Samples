# OpenID Connect on Uno Platform, via Uno.Extensions

Signs in against the **public [Duende demo identity server](https://demo.duendesoftware.com)**
using **`Uno.Extensions.Authentication.Oidc`**, and narrates every step on screen so the flow is
visible on all four heads (Desktop/Skia, WebAssembly, Android, iOS).

No registration is needed: the demo server accepts arbitrary redirect URIs and ships a test user
(**bob / bob**). The companion sample `UI/Authentication.OidcDemo` drives Duende's `OidcClient`
by hand; this one lets the Uno.Extensions provider do it — configuration, discovery, the
platform's browser surface, token storage and refresh are all owned by `AddOidc`.

## The page

- **OIDC configuration** — authority, client, scope, and the redirect URI the platform's
  `WebAuthenticationBroker` derives at runtime (`AutoRedirectUriFromWebAuthenticationBroker`).
- **Actions** — `Sign in` (interactive), `Silent only` (refresh-token redemption, never shows
  UI), `Call demo API` (`GET /api/test` with the access token, echoing its claims),
  `Sign out` (end-session flow).
- **Flow log** — every `IAuthenticationService` call, timestamped, with what the provider did.

## What each platform uses for sign-in

| Head | Browser surface | Redirect returns via |
| --- | --- | --- |
| Desktop (Windows/macOS/Linux, Skia) | system browser | the `http://localhost` loopback listener Uno.Extensions registers as the desktop `WebAuthenticationBroker` |
| WebAssembly | browser popup | the app's own origin (allow popups) |
| Android | Chrome custom tab | the `oidc-ext-demo://` intent filter on `WebAuthenticationBrokerActivity` |
| iOS | `ASWebAuthenticationSession` | the `oidc-ext-demo://` URL scheme in `Info.plist` |

## Notes

- `Policy.RequireIdentityTokenSignature = false` is set in `App.xaml.cs`: Duende's `OidcClient`
  only validates id_token signatures when an `IIdentityTokenValidator` is supplied (it ships in a
  separate package), and throws otherwise. The code flow's tokens arrive over TLS from the
  authority, which is what protects them here; supply a validator in production if you rely on
  id_token contents.
- On startup the app runs the silent path first, like a production app should: tokens persist in
  the platform's key-value storage, so a restart signs back in without UI until the refresh token
  expires.
- All four heads use the Skia renderer (`SkiaRenderer` in `UnoFeatures`), which also exercises
  Uno's runtime-asset substitution of the extensions assemblies on the mobile heads.
