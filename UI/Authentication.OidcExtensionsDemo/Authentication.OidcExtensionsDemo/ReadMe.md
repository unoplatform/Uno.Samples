# OpenID Connect on Uno Platform, via Uno.Extensions

Signs in with any OpenID Connect identity provider using
**`Uno.Extensions.Authentication.Oidc`**, calls a protected API with the token, and narrates
every step on screen so the flow is visible on all four heads (Desktop/Skia, WebAssembly,
Android, iOS).

The companion sample `UI/Authentication.MsalExtensionsDemo` does the same thing against Microsoft
Entra ID with the MSAL provider. Same UI, so the two are directly comparable:

| | This sample | Authentication.MsalExtensionsDemo |
| --- | --- | --- |
| Provider | `AddOidc` (Duende `OidcClient` underneath) | `AddMsal` (MSAL.NET underneath) |
| Configured from | `OidcAuthentication` section in appsettings | `MsalAuthentication` section in appsettings |
| Redirect URI | the configured value, except on WebAssembly | derived per platform by the provider |
| Browser on desktop | an `IBrowser` the sample provides | MSAL's own system browser |
| API call | a Refit client registered in `UseHttp` carries the token | a plain `HttpClient` with the header set by hand |

## The three pages

- **Sign in** — the configuration the provider was handed, the redirect URI in use on this head,
  the buttons (`Sign in`, `Silent only`, `Sign out`), the stored tokens with the claims decoded
  from the ID token, and a timestamped flow log that also records every `LoggedOut` event.
- **Call the API** — `GET /api/test` on the Duende demo server through `IDemoApi`, a Refit
  interface registered inside `UseHttp`; the handler Uno.Extensions chains into that client adds
  the `Authorization: Bearer` header and refreshes on 401. The API echoes the claims it read.
- **Platform setup** — what every head needs at the identity provider and in the project, with
  the head you are running on marked.

## How the code is arranged

- `App.xaml.cs` — the host: `UseHttp` with `AddRefitClient<IDemoApi>`, `UseAuthentication` with
  `AddOidc(name: "OidcAuthentication")`, the two singletons, and `PlatformSupport.ConfigureHost`
  for the head's own additions.
- `Authentication/OidcFlowService.cs` — the only type that talks to `IAuthenticationService`;
  it holds the app's one `LoggedOut` subscription.
- `Authentication/PlatformSupport.cs` + `Platforms/*/PlatformSupport.*.cs` — per-head facts and
  host configuration as partial methods, so a new head without them is a compile error.
- `Platforms/Desktop/SystemBrowser.cs` — the loopback `IBrowser` the desktop head needs, because
  Uno's `WebAuthenticationBroker` is not implemented on Skia desktop.
- `Common/SecretRedactor.cs` — recording mode.

## Running it

The full setup guide — every value to edit, per head, with a troubleshooting table — is in
[../README.md](../README.md). The short version: it runs as is against the Duende demo server
(`alice` / `alice`), on `dotnet run -f net10.0-desktop` (or `net10.0-browserwasm`,
`net10.0-android`, `net10.0-ios`).
