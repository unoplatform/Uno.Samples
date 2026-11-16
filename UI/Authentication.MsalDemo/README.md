# Authentication with MSAL (Microsoft Entra ID)

This sample signs users in with **MSAL.NET** against Microsoft Entra ID (formerly Azure AD) from a
single Uno Platform project, and then calls Microsoft Graph with the token it acquired.

It uses only [`Uno.WinUI.MSAL`](https://platform.uno/docs/articles/interop/MSAL.html) and
`Microsoft.Identity.Client` — there is deliberately no Uno.Extensions here, no host builder, no DI
container and no navigation framework, so `Authentication/AuthenticationService.cs` can be lifted
into an app of any shape.

Tested on:

- [x] Desktop (Skia)
- [x] Android
- [x] iOS
- [x] WebAssembly

## What it shows

| Page | What it demonstrates |
| --- | --- |
| **Sign in** | The flow a production app should use, narrated step by step: look in the token cache → `AcquireTokenSilent` → `MsalUiRequiredException` → `AcquireTokenInteractive`. Also shows the resolved configuration, the platform, the redirect URI in use, the cached accounts, and local sign-out. |
| **Microsoft Graph** | `GET https://graph.microsoft.com/v1.0/me` with the access token in an `Authorization: Bearer` header, so you can see the token is real and accepted by an API. |
| **Platform setup** | The Entra ID and project requirements for every head, with the one you are running on marked. |

## How to run the sample

1. **Register an application** in the [Microsoft Entra admin center](https://entra.microsoft.com) as
   a public client, with `User.Read` delegated permission.
2. **Paste the Application (client) ID** into
   `Authentication.MsalDemo/Authentication/MsalConfig.cs`. It ships as
   `00000000-0000-0000-0000-000000000000`, and until you replace it the app runs and explains what is
   missing rather than failing silently.
3. **Register the redirect URI** for the head you are running. The *Sign in* page prints the exact
   string to register.
4. Run the app.

[MSAL-SETUP.md](MSAL-SETUP.md) walks through all of this in detail — the registration, the redirect
URI per platform, the per-head project requirements, and a troubleshooting table mapping the MSAL
error codes you are most likely to hit onto their fix.

## Relevant documentation

- [Using MSAL with Uno Platform](https://platform.uno/docs/articles/interop/MSAL.html)
- [MSAL.NET documentation](https://learn.microsoft.com/entra/msal/dotnet/)
- [Microsoft Graph `/me` endpoint](https://learn.microsoft.com/graph/api/user-get)
