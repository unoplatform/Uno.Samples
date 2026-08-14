# MSAL setup for Uno.Extensions.Authentication.MSAL

## Register the app in Microsoft Entra ID

Create a new public client application (desktop/mobile) in Microsoft Entra ID and grant the delegated `User.Read` permission.

Use the same client ID in both places:

- `Authentication.MsalExtensionsDemo/Authentication/MsalConfig.cs`
- `Authentication.MsalExtensionsDemo/appsettings.json`

The sample ships with a placeholder value so the app can run and explain what is missing without failing silently. This sample targets the `Uno.Sdk` 6.8.0-dev line, where the Skia runtime-asset fix for `.WithUnoHelpers()` is present.

## Redirect URIs by platform

| Platform | Redirect URI |
| --- | --- |
| Desktop | `http://localhost` |
| Android | `msal{ClientId}://auth` |
| iOS | `msauth.com.companyname.authentication.msalextensionsdemo://auth` |
| WebAssembly | `http://localhost:5000/authentication/login-callback.htm` |

The app prints the exact redirect URI it is using at runtime. Register the value shown in the UI for the target you are launching.

## Notes

- Desktop uses the system browser and loopback listener.
- Android uses the custom-tab flow and needs the `msal{ClientId}://auth` intent filter.
- WebAssembly must be registered as a single-page application redirect.
- The app logs `Uno.Extensions.Authentication` traces at `Trace` level so the sign-in and cache setup path remains visible.

## Troubleshooting

If interactive sign-in fails, check the following:

- the client ID is correct in the app and in Entra
- the redirect URI matches the platform you are testing
- the app is built for the same runtime identifier you deploy
- the log output is showing the provider trace messages from `Uno.Extensions.Authentication`

