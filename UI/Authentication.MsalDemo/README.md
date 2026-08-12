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
- [ ] WebAssembly — builds and runs, but interactive sign-in cannot complete under the Skia
      renderer. See [MSAL-SETUP.md](MSAL-SETUP.md#webassembly-and-the-skia-renderer) for the
      diagnosis and the options.

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

## Note: temporary workaround for unoplatform/uno#20601

`GetOrCreateApp` in `Authentication/AuthenticationService.cs` currently supplies the parent
`Activity` / `UIViewController` by hand behind `#if ANDROID` / `#elif IOS`, with `.WithUnoHelpers()`
commented out directly above it:

```csharp
//.WithUnoHelpers()
#if ANDROID
    .WithParentActivityOrWindow(() => Uno.UI.ContextHelper.Current as Android.App.Activity)
#elif IOS
    .WithParentActivityOrWindow(() => UIKit.UIApplication.SharedApplication?.KeyWindow?.RootViewController)
#endif
```

This works around [unoplatform/uno#20601](https://github.com/unoplatform/uno/issues/20601): with the
`SkiaRenderer` feature enabled, every head loads the `skia` flavour of `Uno.UI.MSAL`, where
`.WithUnoHelpers()` is a no-op — so on Android MSAL is left with no parent `Activity` and
`AcquireTokenInteractive` fails with `activity_required`.

The fix is [unoplatform/uno#24055](https://github.com/unoplatform/uno/pull/24055). **Once it has
merged and shipped, the `#if` block should be deleted and the single `.WithUnoHelpers()` call
restored** — that is the shape this sample is meant to demonstrate.

## Relevant documentation

- [Using MSAL with Uno Platform](https://platform.uno/docs/articles/interop/MSAL.html)
- [MSAL.NET documentation](https://learn.microsoft.com/entra/msal/dotnet/)
- [Microsoft Graph `/me` endpoint](https://learn.microsoft.com/graph/api/user-get)
