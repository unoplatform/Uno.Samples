# Authentication with MSAL via Uno.Extensions.Authentication.MSAL

This sample demonstrates how to sign users in with `Uno.Extensions.Authentication.MSAL` and then use the resulting access token against Microsoft Graph.

It targets the `Uno.Sdk` 6.8.0-dev line, where the Skia MSAL fix is in place and `.WithUnoHelpers()` is no longer the no-op it was before the runtime asset fix. The provider is wired through the Uno Extensions host with `UseAuthentication(auth => auth.AddMsal(...))`, so the provider setup and token-cache lifecycle stay in the host while the app logic remains intentionally small and portable.

## What it shows

- Sign in with Microsoft Entra ID through the provider's interactive flow
- Silent refresh with `RefreshAsync()`
- Logout and token-cache inspection
- A platform-appropriate redirect URI and a small setup guide for each head

## How to run

1. Register a public client app in the Microsoft Entra admin center and grant the `User.Read` delegated permission.
2. Paste the Application (client) ID into `Authentication.MsalExtensionsDemo/Authentication/MsalConfig.cs` and into the `Msal.ClientId` value in `appsettings.json`.
3. Register the redirect URI for the head you are running.
4. Run the app on the desired target.

See [MSAL-SETUP.md](MSAL-SETUP.md) for the complete platform matrix and troubleshooting notes.

## Notes

This sample deliberately keeps the app logic thin and uses the default Uno.Extensions host configuration. The provider derives the platform redirect URI automatically on the latest 6.8.0-dev bits, so the app does not need a custom `Builder(...)` redirect override; the only app-specific override is the WebAssembly callback path used by `WebAuthenticationBroker`.

The actual provider integration is the point of the sample: MSAL sign-in is configured with the extensions pipeline and the token cache is consumed through `IAuthenticationService` and `ITokenCache`.
