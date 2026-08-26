# MSAL on Uno Platform, via Uno.Extensions

Signs in with Microsoft Entra ID using **`Uno.Extensions.Authentication.MSAL`**, and narrates
every step on screen so the flow is visible on all four heads (Desktop/Skia, WebAssembly,
Android, iOS).

The companion sample `UI/Authentication.MsalDemo` does the same thing with the
`Uno.WinUI.MSAL` add-in and no Uno.Extensions, calling MSAL.NET directly. Same UI, so the two
are directly comparable — this one is the "let the provider do it" version:

| | This sample | Authentication.MsalDemo |
| --- | --- | --- |
| Configured from | `MsalAuthentication` section in appsettings | `MsalConfig.cs` constants |
| Token acquisition | one `IAuthenticationService.LoginAsync` call | `AcquireTokenSilent` then `AcquireTokenInteractive` by hand |
| Redirect URI | derived per platform by the provider | computed per platform by the app |
| Desktop token cache | persisted (DPAPI / keychain / keyring) | in memory only |

## The three pages

- **Sign in** — the configuration the provider was handed, the derived redirect URI, the
  buttons (`Sign in`, `Silent only`, `Sign out`), the resulting token, and a timestamped flow log.
- **Microsoft Graph** — `GET /v1.0/me` with the access token in an `Authorization: Bearer`
  header, to prove a real API accepts the token.
- **Platform setup** — what every head needs in Entra ID and in the project, with the head
  you are running on marked.

## Running it

The full setup guide — every value to edit, per head, with the Entra ID registration and a
troubleshooting table — is in [../README.md](../README.md). The short version:

1. Register a public-client app in the [Microsoft Entra admin center](https://entra.microsoft.com):
   enable **Allow public client flows**, grant Microsoft Graph **User.Read**.
2. Put the Application (client) ID and tenant in `appsettings.development.json`:

   ```json
   "MsalAuthentication": {
     "ClientId": "<your-client-id>",
     "TenantId": "consumers",
     "Scopes": [ "User.Read" ]
   }
   ```

3. Register the redirect URI for the head you are running. The Sign in page shows the exact
   string to paste; the Platform setup page explains the platform type each one needs
   (WebAssembly in particular must be registered as a **Single-page application**).
4. `dotnet run -f net10.0-desktop` (or `net10.0-browserwasm`, `net10.0-android`, `net10.0-ios`).

Without a client ID the app still starts and explains what is missing rather than throwing.

Two values cannot follow appsettings automatically, because they are compile-time manifest
entries — the Platform setup page flags both:

- `Platforms/Android/MsalActivity.Android.cs` — the intent filter scheme must be
  `msal` + your client ID.
- `Platforms/iOS/Info.plist` — `CFBundleURLTypes` must contain `msauth.{BundleId}`.

On iOS a third one is not a value but a permission: `Platforms/iOS/Entitlements.plist` grants
`keychain-access-groups` `$(AppIdentifierPrefix)com.microsoft.adalcache`, without which MSAL
cannot write its token cache and sign-in ends in `missing_entitlements`.

## Recording a demo

The **Redact** switch in the header hides everything that identifies the app registration or the
signed-in user — client and tenant IDs, the account, the Graph response, anything token-shaped —
across all three pages and the flow log at once. It works at display time, so it can be switched
mid-demo and still covers steps already on screen, and the choice survives a restart, which the
silent-sign-in demo needs.

It cannot reach the sign-in UI itself: that runs in the system browser or an
`ASWebAuthenticationSession`, outside the app, so the account picker shows real addresses. Use a
throwaway account for the recording, or cut that part.
