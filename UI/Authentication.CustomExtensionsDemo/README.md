# Custom authentication via Uno.Extensions

This sample signs users in against the app's own backend with the Custom provider of
[`Uno.Extensions.Authentication`](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Authentication/AuthenticationOverview.html#custom)
(`AddCustom`), calling it through a Kiota client, from a single Uno Platform project. The backend
is included: `Authentication.CustomExtensionsDemo.Server` serves the login, refresh and logout
endpoints, a protected API, and the WebAssembly head.

Tested on:

- [x] Desktop (Skia) on Windows: Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] WebAssembly: Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] Android (API 36 emulator): Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [ ] iOS: not run yet

This document is the **setup guide**. For what the sample *demonstrates* and how the code is
arranged, see [`Authentication.CustomExtensionsDemo/ReadMe.md`](Authentication.CustomExtensionsDemo/ReadMe.md).

---

## It runs as is

Start the server first, from this folder:

```bash
dotnet run --project Authentication.CustomExtensionsDemo.Server
# listens on http://localhost:5001
```

Then run any head (below), type the password, press **Sign in** (`alice` / `alice`, or
`bob` / `bob`). The app's configuration (`appsettings.development.json`) names the server for the
Kiota client:

```json
"DemoServer": {
  "Url": "http://localhost:5001"
}
```

The flow:

1. `LoginAsync(dispatcher, { "Username": ..., "Password": ... })` hands the credentials to the
   `Login` callback, which posts them to `POST /auth/login`. A 401 is a wrong password: the
   callback returns `null` and `LoginAsync` returns `false`.
2. On success the callback returns a **new** dictionary holding only `AccessToken` and
   `RefreshToken`. Whatever `Login` returns is stored as-is, so returning the credentials
   dictionary (what the template does) would store the password next to the tokens.
3. The Kiota client registered in `UseHttp` sends the access token; on a 401 the handler runs the
   `Refresh` callback (`POST /auth/refresh`, with the `No-Refresh` header so a 401 there cannot
   loop) and retries.
4. **Sign out** runs the `Logout` callback (`POST /auth/logout` revokes the refresh token), then the
   provider clears the tokens and raises `LoggedOut`.

`appsettings*.json` are embedded resources: after editing them, **rebuild**. Debug builds run in
the Development environment, so `appsettings.development.json` is the file that applies.

### Versions

Uno.Sdk 7.0.0-dev.115 (Uno.Extensions 8.0.0-dev.150). The csproj references `Uno.WinUI.Lottie`
in Debug builds only: the Hot Design tooling still references `Uno.UI.Lottie`, and without the
package a Debug build on desktop hangs at startup with no window
([unoplatform/uno.extensions#3262](https://github.com/unoplatform/uno.extensions/issues/3262)).

### Regenerating the Kiota client

`Content/Client/DemoServerClient` is generated code, checked in the way the template checks in its
own client. After changing the server's API, export its OpenAPI document (the server serves it at
`/swagger/v1/swagger.json` in Development) to `Specs/DemoServer.swagger.json` and run, from the
app project folder:

```bash
kiota generate -l CSharp -c DemoServerClient -n Authentication.CustomExtensionsDemo.Client \
  -d Specs/DemoServer.swagger.json -o Content/Client/DemoServerClient --clean-output
```

(`dotnet tool install --global Microsoft.OpenApi.Kiota --version 1.16.0`, the version the template
uses.) The server project references the app's WebAssembly build, so the first time, run a copy of
the server without that reference to export the document.

### Pointing it at your own backend

Change `DemoServer:Url`, regenerate the client from your API's OpenAPI document, and adapt the
three callbacks in `Authentication/CustomFlowService.cs` to your login, refresh and logout
endpoints. Keep the `No-Refresh` header on the refresh call, and return only tokens from `Login`.

---

## Per-head setup

The commands below are run from this folder, with the server already running. There is no
browser and no redirect, so the only per-head difference is the server address.

| Head | Server address | Set in |
| --- | --- | --- |
| Desktop (Skia) | `http://localhost:5001` | appsettings |
| WebAssembly | the page's origin (the server hosts the app) | `Platforms/WebAssembly/PlatformSupport.Wasm.cs` |
| Android | `http://10.0.2.2:5001` (emulator) | `Platforms/Android/PlatformSupport.Android.cs` |
| iOS | `http://localhost:5001` (simulator) | appsettings |

### Desktop (Skia)

```bash
dotnet run --project Authentication.CustomExtensionsDemo/Authentication.CustomExtensionsDemo.csproj -f net10.0-desktop
```

Tokens persist, so the next launch signs in through the `Refresh` callback with no prompt.
Restarting the *server* forgets every token: the next refresh is refused and the app signs out.

### WebAssembly

Run it through the server, which hosts the app on the API's origin (no CORS needed), and open
`http://localhost:5001`. `PlatformSupport.Wasm.cs` points the client at the page's origin.
`appsettings.development.json` keeps the tokens in `SessionStorage`, so a reload signs back in
silently and closing the tab signs out.

### Android

```bash
dotnet build Authentication.CustomExtensionsDemo/Authentication.CustomExtensionsDemo.csproj -f net10.0-android -t:Run
```

The emulator reaches the development machine at `10.0.2.2`, and
`Resources/xml/network_security_config.xml` allows plain HTTP to that address only. A physical
device needs the machine's LAN address in both places, and the server bound to it
(`--urls http://0.0.0.0:5001`).

### iOS

```bash
dotnet build Authentication.CustomExtensionsDemo/Authentication.CustomExtensionsDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

Run the server on the Mac. `Info.plist` sets `NSAllowsLocalNetworking` so App Transport Security
allows plain HTTP to `localhost`, and `Entitlements.plist` grants `keychain-access-groups`, which
the token store needs.

---

## Checking that it worked

1. Sign in with a wrong password: `Login: POST /auth/login answered 401`, `Sign-in failed`.
2. Sign in as `alice` / `alice`: the TOKENS card shows `AccessToken` and `RefreshToken`, nothing
   else.
3. **Call the API**: HTTP 200 with the user. Wait a minute and call again: the handler runs the
   `Refresh` callback and the token's issue time moves on.
4. Restart the app: the startup silent refresh signs you back in.
5. **Sign out**: `Logout: POST /auth/logout revoked the refresh token`, then `LoggedOut`.

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| `HttpRequestException` in the flow log | The server is not running, or not reachable at this head's address | Start the server; check the table above |
| Sign-out throws `HttpRequestException` | The `Logout` callback could not reach the server | The tokens are cleared anyway; the refresh token stays valid on the server until it is used or the server restarts |
| **Desktop:** the window never appears in a Debug build | unoplatform/uno.extensions#3262 | Keep the Debug-only `Uno.WinUI.Lottie` reference |
| **Android:** API call fails with a cleartext error | Plain HTTP to an address `network_security_config.xml` does not allow | Add the address there, or serve HTTPS |
| **WebAssembly:** 404 or a CORS error | The head was started on its own dev server (port 5000) | Run the server project and open `http://localhost:5001` |
| Signed out after every server restart | The demo server keeps its tokens in memory | Expected |
