# Web authentication on Uno Platform, via Uno.Extensions

Signs in through a backend's own login page using the **Web provider** of
**`Uno.Extensions.Authentication`** (`AddWeb`), calls a protected API with the token, and narrates
every step on screen so the flow is visible on all four heads (Desktop/Skia, WebAssembly,
Android, iOS).

`AddWeb` is for a backend that runs its own sign-in (a form, or a social provider behind it) and
redirects back to the app with `access_token` and `refresh_token` in the URL. The sample ships
that backend, `Authentication.WebExtensionsDemo.Server`, so it runs with nothing to register.

The companion sample `UI/Authentication.OidcExtensionsDemo` does the same thing against a standard
OpenID Connect provider. Same UI, so the two are directly comparable:

| | This sample | Authentication.OidcExtensionsDemo |
| --- | --- | --- |
| Provider | `AddWeb<IAuthApi>` (no protocol library) | `AddOidc` (Duende `OidcClient` underneath) |
| Configured from | `WebAuthentication` section in appsettings | `OidcAuthentication` section in appsettings |
| Callback | the `redirect_uri` of `LoginStartUri`, set per head | the configured value, except on WebAssembly |
| Tokens come back | in the callback URL, read by the provider | from the token endpoint, after a code exchange |
| Refresh | the app's `Refresh` callback calls the backend | OidcClient's refresh-token grant |
| Binding the callback to the sign-in | the app's `state`, checked in `PostLogin` | the protocol's `state`, nonce and PKCE |

## The three pages

- **Sign in**: the configuration the provider was handed, the callback in use on this head, the
  buttons (`Sign in`, `Silent only`, `Sign out`), the stored tokens, and a timestamped flow log
  that records each provider callback (`PrepareLoginStartUri`, `PostLogin`, `Refresh`) and every
  `LoggedOut` event.
- **Call the API**: `GET /api/profile` on the demo server through `IProfileApi`, a Refit interface
  registered inside `UseHttp`; the handler Uno.Extensions chains into that client adds the
  `Authorization: Bearer` header and, on a 401, runs the `Refresh` callback and retries. Access
  tokens live 60 seconds, so the refresh is easy to watch.
- **Platform setup**: what every head needs on the backend and in the project, with the head you
  are running on marked.

## How the code is arranged

- `App.xaml.cs`: the host. `UseHttp` with `AddRefitClient<IAuthApi>` and
  `AddRefitClient<IProfileApi>`, `UseAuthentication` with
  `AddWeb<IAuthApi>(name: "WebAuthentication", configure: WebFlowService.Configure)`, the two
  singletons, and `PlatformSupport.ConfigureHost` for the head's own additions.
- `Authentication/WebFlowService.cs`: the only type that talks to `IAuthenticationService`. Its
  `Configure` adds the three provider callbacks: a `state` on the login URL, the check of that
  `state` on the way back, and the refresh call. It holds the app's one `LoggedOut` subscription.
- `Services/Endpoints/IAuthApi.cs`: `POST /auth/refresh`, marked with the no-refresh header so a
  401 there cannot loop back into another refresh.
- `Authentication/PlatformSupport.cs` + `Platforms/*/PlatformSupport.*.cs`: per-head facts and the
  server address and callback for the heads that differ from appsettings (Android, WebAssembly,
  desktop), layered over appsettings through configuration.
- `../Authentication.WebExtensionsDemo.Server`: the backend (login page, logout, refresh,
  protected API), which also hosts the WebAssembly head.
- `Common/SecretRedactor.cs`: recording mode.

## Running it

The full setup guide (every value to edit, per head, with a troubleshooting table) is in
[../README.md](../README.md). The short version: start the server
(`dotnet run --project Authentication.WebExtensionsDemo.Server`), then run a head and sign in as
`alice` / `alice`.
