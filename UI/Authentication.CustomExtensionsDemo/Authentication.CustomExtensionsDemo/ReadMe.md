# Custom authentication on Uno Platform, via Uno.Extensions

Signs in against the app's own backend with the **Custom provider** of
**`Uno.Extensions.Authentication`** (`AddCustom`): the app collects the user name and password
itself, its callbacks call the backend through a **Kiota** client, and the provider stores the
tokens, applies them to API calls and refreshes them. Every step is narrated on screen, on all
four heads (Desktop/Skia, WebAssembly, Android, iOS).

The backend is included: `Authentication.CustomExtensionsDemo.Server` (login, refresh, logout, a
protected API), so the sample runs with nothing to register.

The companion samples `UI/Authentication.WebExtensionsDemo` and `UI/Authentication.OidcExtensionsDemo`
use the same UI with the browser-based providers:

| | This sample | Web / OIDC samples |
| --- | --- | --- |
| Provider | `AddCustom<DemoServerClient>` | `AddWeb<IAuthApi>` / `AddOidc` |
| Who collects the credentials | the app's own form | the backend's or identity provider's page, in the browser |
| Per-head setup | the server address only | a callback per head (custom scheme, loopback, origin) |
| API client | Kiota, generated from the server's OpenAPI document | Refit |

## The three pages

- **Sign in**: the server in use on this head, the login form, the buttons (`Sign in`,
  `Silent only`, `Sign out`), the stored tokens (only `AccessToken` and `RefreshToken`: never the
  password), and a timestamped flow log that records each provider callback and every
  `LoggedOut` event.
- **Call the API**: `GET /api/profile` through the Kiota client registered inside `UseHttp`; the
  handler Uno.Extensions chains into that client adds the `Authorization: Bearer` header and, on a
  401, runs the `Refresh` callback and retries. Access tokens live 60 seconds.
- **Platform setup**: what each head needs, with the head you are running on marked.

## How the code is arranged

- `App.xaml.cs`: the host. `UseHttp` with `AddKiotaClient<DemoServerClient>(context, name: "DemoServer")`,
  `UseAuthentication` with `AddCustom<DemoServerClient>(CustomFlowService.Configure, name: "CustomAuth")`,
  the two singletons, and `PlatformSupport.ConfigureHost` for the head's server address.
- `Authentication/CustomFlowService.cs`: the only type that talks to `IAuthenticationService`. Its
  `Configure` adds the three callbacks: `Login` (posts the credentials, returns only the tokens,
  `null` on a 401), `Refresh` (posts the refresh token with the `No-Refresh` header) and `Logout`
  (revokes the refresh token). It holds the app's one `LoggedOut` subscription.
- `Content/Client/DemoServerClient` and `Specs/DemoServer.swagger.json`: the generated Kiota client
  and the OpenAPI document it was generated from.
- `Authentication/PlatformSupport.cs` + `Platforms/*/PlatformSupport.*.cs`: per-head facts and the
  server address for the heads that differ from appsettings (Android, WebAssembly).
- `../Authentication.CustomExtensionsDemo.Server`: the backend, which also hosts the WebAssembly
  head.

## Running it

The full setup guide is in [../README.md](../README.md). The short version: start the server
(`dotnet run --project Authentication.CustomExtensionsDemo.Server`), run a head, and sign in as
`alice` / `alice`.
