# Handoff: MSAL auth fixes — continue on macOS

> [!IMPORTANT]
> **Update 2026-08-20 (Windows machine, after this handoff was written):** substantial parts of
> this document are now historical. Since it was written:
>
> - **WASM token-cache persistence landed** (`specs/011-wasm-msal-token-cache` in `uno.extensions`,
>   all items implemented): tokens now persist to `sessionStorage` by default and sign-in survives
>   a page reload. The "in-memory only / does not survive a reload" statements below describe the
>   pre-011 behavior and are no longer expected results — a reload signing the user out is now a bug.
>   Configure via `KeyValueStorageConfiguration:BrowserCacheLocation`
>   (`SessionStorage` default / `LocalStorage` / `MemoryStorage`).
> - **Logout is fixed twice over**: `InternalLogoutAsync` no longer throws when called through the
>   dispatcher-less `LogoutAsync(CancellationToken)` overload, removes *every* account, and deletes
>   the serialized MSAL cache.
> - The uncommitted diffs listed in the appendix and the repo-state table are all committed and
>   pushed (`uno.extensions` @ `dev/sb/msal-auth-fixes` through `2e04d589a`; this repo through the
>   current head of `dev/sb/msa-ext`).
> - The verification checklist and iteration loop remain valid, with one addition: after repacking,
>   verify the *served* assembly (e.g. fetch `/_framework/Uno.Extensions.Authentication.MSAL.WinUI.wasm`
>   and check it for a marker of your change) before validating behavior — see
>   `specs/lessons.md` in `uno.extensions`.
> - Remaining validation: a human sign-in pass on the WASM head (sign in → reload keeps the session
>   → logout clears `MsalCache_*`/`AuthToken_*` from `sessionStorage` → tab close drops the session).


Context for picking up the `uno.extensions` MSAL work on a macOS machine, using this demo
app as the live test bed. Written 2026-08-19 on the Windows machine where the work so far
happened. Read `specs/009-msal-auth-fixes/progress.md` and `AGENTS.md` in `uno.extensions`
before touching that repo — the spec carries the full design history.

## What this is about

`Uno.Extensions.Authentication.MSAL` (branch `dev/sb/msal-auth-fixes` in `uno.extensions`)
fixes a set of MSAL problems; this demo consumes locally-built packages from that branch to
validate them end to end. Fixed and verified so far:

- **WASM `SetupStorage - Process_PlatformNotSupported`** (the original bug): released
  packages ran `MsalCacheHelper.CreateAsync` on WebAssembly, which uses
  `System.Diagnostics.Process` (unsupported in the browser). The branch compiles the whole
  storage path out for the `browserwasm` TFM (`UNO_EXT_MSAL_NOSTORAGE`) and logs one
  Information message instead. **Verified live on WASM.**
- **Platform redirect-URI defaults**: the provider derives the conventional redirect per
  platform (Android `msal{ClientId}://auth`, iOS `msauth.{BundleId}://auth`, WASM
  `{origin}/authentication-callback` via WebAuthenticationBroker, desktop
  `http://localhost`). Precedence: platform default < config `RedirectUri` < `Builder(...)`
  callback. **Verified live on WASM up to the Entra prompt.**
- **Effective RedirectUri logged at Information** on provider build (added this session,
  *uncommitted* — see below): sign-in failures are overwhelmingly redirect-URI problems.
- **`InteractiveTimeout`** (added this session, *uncommitted*): on desktop, closing the
  system browser is undetectable — `AcquireTokenInteractive` waited forever and the login
  button stayed disabled permanently. New `MsalConfiguration.InteractiveTimeout`
  (default 5 min, `00:00:00` = wait forever) cancels an abandoned sign-in; it surfaces as
  `MsalClientException (authentication_canceled)`, same as a user-cancelled sign-in.

## Repo state at handoff

| Repo | Branch | State |
| --- | --- | --- |
| `uno.extensions` | `dev/sb/msal-auth-fixes` | Pushed through `464efe2fd`. **4 files uncommitted** on the Windows machine: `MsalAuthenticationProvider.cs`, `MsalConfiguration.cs`, `Given_MsalAuthentication.cs` (new timeout test), `doc/Learn/Authentication/HowTo-MsalAuthentication.md` (doc edits from an earlier session). |
| `Uno.Samples` | `dev/sb/msa-ext` | Demo committed as `3edea4c5` incl. the local-package wiring. Working tree has one further edit: `NuGet.config` local source changed from `X:\src\uno.extensions\artifacts` to the relative `../../../uno.extensions/artifacts` so it works on any machine. This handoff doc is also new/untracked. |

**Step 0 (on Windows, before switching): commit and push both repos.** If that didn't
happen, the appendix at the bottom carries the full uncommitted `src/` diff for
`uno.extensions`; the samples-side changes are only the relative NuGet path and this file.

## One-time setup on the macOS machine

Assumed layout (the relative NuGet source depends on it): both repos as siblings, e.g.
`~/src/uno.extensions` and `~/src/Uno.Samples`.

1. Environment: .NET 10 SDK (the demo's `global.json` pins `Uno.Sdk 6.8.0-dev.19`; the
   extensions repo has its own `global.json`). Run `uno-check` if the machine isn't already
   set up for Uno development.
2. Checkout: `uno.extensions` @ `dev/sb/msal-auth-fixes`, `Uno.Samples` @ `dev/sb/msa-ext`.
   If step 0 was skipped, apply the appendix diff to `uno.extensions`.
3. **Pack all extensions packages** (the demo pins *every* `Uno.Extensions.*` implicit
   package to `255.255.255.255-local` via `UnoExtensionsVersion` in
   `UI/Authentication.MsalExtensionsDemo/Directory.Build.props`, so the artifacts folder
   needs the full set — on Windows this existed already; on the mac it starts empty):

   ```bash
   cd ~/src/uno.extensions
   dotnet build Uno.Extensions-packageonly.slnf -c Release \
     -p:PackageOutputPath=$PWD/artifacts -p:PackageVersion=255.255.255.255-local
   ```

   `Build_Windows` turns itself off on non-Windows hosts; packages then lack the
   `windows10` TFM, which is fine for wasm/desktop testing. To shorten the build, copy
   `DebugPlatforms.props.sample` → `DebugPlatforms.props` and disable Android/iOS — but
   only if you won't test those heads.
4. Restore the real ClientId: `Authentication.MsalExtensionsDemo/appsettings.development.json`
   has `"ClientId": "[CLIENTID]"` (deliberately redacted in git) — put the real app
   registration id back locally, don't commit it. Steve has it; tenant is `consumers`.
5. Build the app:

   ```bash
   cd ~/src/Uno.Samples/UI/Authentication.MsalExtensionsDemo/Authentication.MsalExtensionsDemo
   dotnet build -f net10.0-browserwasm   # and/or -f net10.0-desktop
   ```

   Verify the local package was resolved: `obj/project.assets.json` must reference
   `Uno.Extensions.Authentication.MSAL.WinUI/255.255.255.255-local` (not `7.4.0-dev.*`,
   which is the broken released packaging from the `uno dev` feed).

## Iteration loop (after editing uno.extensions)

The package version never changes, so NuGet's cache goes stale on every repack. The loop:

```bash
# 1. Repack only the changed project(s):
cd ~/src/uno.extensions
dotnet build src/Uno.Extensions.Authentication.MSAL/Uno.Extensions.Authentication.MSAL.WinUI.csproj \
  -c Release -p:PackageOutputPath=$PWD/artifacts -p:PackageVersion=255.255.255.255-local

# 2. Purge the stale cache copy (all of them, if in doubt):
rm -rf ~/.nuget/packages/uno.extensions.authentication.msal.winui/255.255.255.255-local
# broader: rm -rf ~/.nuget/packages/uno.extensions.*/255.255.255.255-local

# 3. Rebuild the app head.
```

## Running and what to expect

- **WASM**: `dotnet run -f net10.0-browserwasm` → http://localhost:5000. On startup +
  Login click, the browser console must show (Information level):
  - `Using RedirectUri 'http://localhost:5000/authentication-callback'; sign-in requires a matching redirect URI...`
  - `Token cache persistence isn't supported on WebAssembly; tokens are cached in memory only`
  - and must NOT show `SetupStorage... Process_PlatformNotSupported`.
- **Desktop (macOS)**: `dotnet run -f net10.0-desktop`. This head exercises the branch's
  **macOS keychain** storage path (`MsalStorageDefaults` → `WithMacKeyChain`, service
  `uno.extensions.msal.{ClientId}`, account `MSALCache`) — the #3025 fix. Sign-in state
  should survive an app restart (silent refresh on relaunch, no prompt).

## Entra app registration (tenant `consumers`, the [CLIENTID] registration)

- **WASM** needs `http://localhost/authentication-callback` registered under
  **Single-page application** (SPA). Portless is deliberate — Entra ignores the port for
  localhost, but the *path* must match, and the *client-type* must be SPA or browser-side
  token redemption fails with `AADSTS90023` (that exact error was hit at handoff time; the
  URI needed moving out of Web/Mobile+desktop into SPA — **confirm this was completed**).
- **Desktop** uses `http://localhost` under **Mobile and desktop applications** (MSAL's
  loopback listener; port ignored by Entra).

## Verification checklist for the macOS agent (in order)

1. WASM login end-to-end: no storage error, popup completes, `IsAuthenticated` true.
   (Blocked at handoff only on the SPA registration above.)
2. Desktop login end-to-end on macOS, then restart → silent sign-in from the keychain
   (validates #3025 defaults on a second machine).
3. Desktop abandoned login: click Login, close the browser → button re-enables when
   `InteractiveTimeout` elapses, Warning logged. For a fast check set
   `"InteractiveTimeout": "00:00:15"` inside the `MsalAuthentication` section of
   `appsettings.development.json`.
4. Run the test layers in `uno.extensions`: unit tests
   (`dotnet test src/Uno.Extensions.Authentication.MSAL.Tests/...csproj` — 27 green at
   handoff) and, if feasible, the runtime-test head so the **new, not-yet-executed** UI test
   `When_LoginAbandoned_Then_InteractiveTimeoutCancelsTheLogin` actually runs
   (it only compiled on Windows; `Given_MsalAuthentication` needs a real Uno host).
5. Update `doc/Learn/Authentication/HowTo-MsalAuthentication.md` with `InteractiveTimeout`
   and the WASM SPA-registration guidance (`http://localhost/authentication-callback`,
   portless, SPA client-type) — neither is documented yet.

## Gotchas learned the hard way

- The demo renders via **SkiaRenderer**: the whole UI is one `<canvas>`. Synthetic
  browser input (Playwright/CDP, headless *and* headed) reaches the canvas as trusted
  pointer events but the app never reacts — UI-driven verification needs a human click.
  Don't burn time re-discovering this.
- .NET 9+ WASM can enforce **COOP/COEP**, which blocks the `window.open` sign-in popup.
  It did NOT bite in this setup (`dotnet run`'s WasmAppHost), but if the popup is silently
  blocked, serve via `dotnet serve` or an Uno Server project (see uno docs `interop/MSAL`).
- MSAL surfaces a cancelled/abandoned web UI as `MsalClientException`
  (`authentication_canceled`), **not** `OperationCanceledException` — relevant to any
  handling/tests around cancellation.
- On WASM the token cache is in-memory by design (the NOSTORAGE fix makes that explicit):
  sign-in does not survive a page reload. Not a bug.
- `Uno.WinUI 6.8.0-dev.42` is what the demo resolves; interactive MSAL on Skia heads needs
  uno#24055 — if desktop/wasm interactive silently no-ops, check that the resolved
  Uno.WinUI build includes it before suspecting the extensions code.

## Appendix: uncommitted `uno.extensions/src` diff at handoff

(Skip if step 0 pushed these. The `HowTo-MsalAuthentication.md` doc diff is not included —
it predates this session; regenerate doc updates per checklist item 5 instead.)

```diff
diff --git a/src/Uno.Extensions.Authentication.MSAL.UI.Tests/Given_MsalAuthentication.cs b/src/Uno.Extensions.Authentication.MSAL.UI.Tests/Given_MsalAuthentication.cs
--- a/src/Uno.Extensions.Authentication.MSAL.UI.Tests/Given_MsalAuthentication.cs
+++ b/src/Uno.Extensions.Authentication.MSAL.UI.Tests/Given_MsalAuthentication.cs
@@ -61,9 +61,9 @@ public class Given_MsalAuthentication
 	/// results. Logging out drives the same code path the product uses, so it also stays correct if
 	/// the storage location changes.
 	/// </remarks>
-	private static async Task<Harness> CreateHarnessAsync(TimeSpan? webUiDelay = null)
+	private static async Task<Harness> CreateHarnessAsync(TimeSpan? webUiDelay = null, TimeSpan? interactiveTimeout = null)
 	{
-		var harness = CreateHarness(webUiDelay);
+		var harness = CreateHarness(webUiDelay, interactiveTimeout);
 
 		using var purge = new CancellationTokenSource(Timeout);
 		await harness.Authentication.LogoutAsync(harness.Dispatcher, purge.Token);
@@ -78,24 +78,30 @@ public class Given_MsalAuthentication
 	/// Builds a host wired to the stub tenant. <c>InteractiveBuilder</c> is how the stub browser
 	/// reaches the per-request builder - <c>Builder</c> only sees the application builder.
 	/// </summary>
-	private static Harness CreateHarness(TimeSpan? webUiDelay = null)
+	private static Harness CreateHarness(TimeSpan? webUiDelay = null, TimeSpan? interactiveTimeout = null)
 	{
 		var window = new Window();
 		var tenant = new StubEntra();
 		var webUi = new StubWebUi(tenant, webUiDelay);
 		var logs = new CapturingLoggerProvider();
 
+		var configurationValues = new Dictionary<string, string?>
+		{
+			["Msal:ClientId"] = StubEntra.ClientId,
+			["Msal:TenantId"] = StubEntra.TenantId,
+			["Msal:Scopes:0"] = "User.Read",
+		};
+		if (interactiveTimeout is { } timeout)
+		{
+			configurationValues["Msal:InteractiveTimeout"] = timeout.ToString();
+		}
+
 		var host = UnoHost
 			.CreateDefaultBuilder(typeof(Given_MsalAuthentication).Assembly)
 			// AddMsal binds Section<MsalConfiguration>("Msal") itself, so the section just has to
 			// exist in configuration - which also keeps the config-binding path under test.
 			.ConfigureHostConfiguration(configuration => configuration
-				.AddInMemoryCollection(new Dictionary<string, string?>
-				{
-					["Msal:ClientId"] = StubEntra.ClientId,
-					["Msal:TenantId"] = StubEntra.TenantId,
-					["Msal:Scopes:0"] = "User.Read",
-				}))
+				.AddInMemoryCollection(configurationValues))
 			.UseAuthentication(auth => auth
 				.AddMsal(window, msal => msal
 					.Builder(pca => pca
@@ -223,6 +229,31 @@ public class Given_MsalAuthentication
 			"a cancelled sign-in must not leave a partial token behind");
 	}
 
+	[TestMethod]
+	public async Task When_LoginAbandoned_Then_InteractiveTimeoutCancelsTheLogin()
+	{
+		// The system-browser flow can't detect a closed browser window: without the provider's
+		// interactive timeout, an abandoned sign-in never completes and the awaiting command stays
+		// busy (button disabled) forever. The stub browser's delay stands in for the abandoned
+		// sign-in; no caller cancellation is involved.
+		using var harness = await CreateHarnessAsync(
+			webUiDelay: TimeSpan.FromSeconds(30),
+			interactiveTimeout: TimeSpan.FromSeconds(1));
+		using var cts = Cts();
+
+		// Same shape as caller cancellation (see When_LoginCancelled): MSAL rethrows the web UI's
+		// OperationCanceledException as MsalClientException / authentication_canceled.
+		var thrown = await FluentActions.Awaiting(async () =>
+				await harness.Authentication.LoginAsync(harness.Dispatcher, cancellationToken: cts.Token))
+			.Should().ThrowAsync<MsalClientException>();
+		thrown.Which.ErrorCode.Should().Be(MsalError.AuthenticationCanceledError);
+
+		harness.Logs.Text.Should().Contain("did not complete within",
+			"the timeout must be distinguishable from a caller-requested cancellation in the logs");
+		(await harness.Tokens.HasTokenAsync(CancellationToken.None)).Should().BeFalse(
+			"an abandoned sign-in must not leave a partial token behind");
+	}
+
 	[TestMethod]
 	public async Task When_LoginSucceeds_Then_NoTokenMaterialInLogs()
 	{
diff --git a/src/Uno.Extensions.Authentication.MSAL/MsalAuthenticationProvider.cs b/src/Uno.Extensions.Authentication.MSAL/MsalAuthenticationProvider.cs
--- a/src/Uno.Extensions.Authentication.MSAL/MsalAuthenticationProvider.cs
+++ b/src/Uno.Extensions.Authentication.MSAL/MsalAuthenticationProvider.cs
@@ -41,6 +41,11 @@ internal record MsalAuthenticationProvider(
 	// the platform-protected accessor once secure storage becomes available again.
 	private const string UnprotectedCacheFileName = "msal.cache.plaintext-fallback";
 
+	// The system-browser flow can't detect an abandoned sign-in (closed browser window), so an
+	// unbounded wait leaves the awaiting login command busy forever. Five minutes matches Uno's
+	// WebAuthenticationBroker default (WinRTFeatureConfiguration.WebAuthenticationBroker).
+	private static readonly TimeSpan DefaultInteractiveTimeout = TimeSpan.FromMinutes(5);
+
 	private IPublicClientApplication? _pca;
 	private string[]? _scopes;
 
@@ -83,6 +88,15 @@ internal record MsalAuthenticationProvider(
 		builder.WithUnoHelpers();
 
 		_pca = builder.Build();
+
+		// The effective redirect URI (defaults, configuration and the Build callback applied) is
+		// the most common sign-in failure point, so surface it above Trace: this exact URI - path
+		// included, port ignored for localhost - must be registered on the Entra app registration.
+		if (Logger.IsEnabled(LogLevel.Information))
+		{
+			Logger.LogInformationMessage($"Using RedirectUri '{_pca.AppConfig.RedirectUri ?? "(none - platform managed)"}'; sign-in requires a matching redirect URI on the app registration");
+		}
+
 		if (Logger.IsEnabled(LogLevel.Trace)) Logger.LogTraceMessage($"Building MSAL Provider complete");
 	}
 
@@ -384,7 +398,32 @@ internal record MsalAuthenticationProvider(
 			// After WithUnoHelpers so an app can override what the helpers set.
 			Settings?.InteractiveBuild?.Invoke(interactive);
 
-			return await interactive.ExecuteAsync(cancellationToken);
+			var timeout = Configuration.Get(Name)?.InteractiveTimeout ?? DefaultInteractiveTimeout;
+			if (timeout <= TimeSpan.Zero)
+			{
+				return await interactive.ExecuteAsync(cancellationToken);
+			}
+
+			using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
+			timeoutCts.CancelAfter(timeout);
+			try
+			{
+				return await interactive.ExecuteAsync(timeoutCts.Token);
+			}
+			// MSAL surfaces a cancelled web UI as MsalClientException (authentication_canceled)
+			// rather than OperationCanceledException; match both so the timeout is logged whichever
+			// shape this MSAL version produces. A caller-requested cancellation is not logged.
+			catch (Exception ex) when (
+				timeoutCts.IsCancellationRequested &&
+				!cancellationToken.IsCancellationRequested &&
+				ex is OperationCanceledException or MsalClientException)
+			{
+				if (Logger.IsEnabled(LogLevel.Warning))
+				{
+					Logger.LogWarningMessage($"Interactive sign-in did not complete within {timeout} and was treated as cancelled (for example, the browser window was closed). Adjust via 'InteractiveTimeout' in the Msal configuration section");
+				}
+				throw;
+			}
 		});
 	}
 
diff --git a/src/Uno.Extensions.Authentication.MSAL/MsalConfiguration.cs b/src/Uno.Extensions.Authentication.MSAL/MsalConfiguration.cs
--- a/src/Uno.Extensions.Authentication.MSAL/MsalConfiguration.cs
+++ b/src/Uno.Extensions.Authentication.MSAL/MsalConfiguration.cs
@@ -32,6 +32,18 @@ internal class MsalConfiguration
 	/// </remarks>
 	public bool UseDefaultPlatformRedirectUri { get; init; } = true;
 
+	/// <summary>
+	/// Maximum time an interactive sign-in may take before it is treated as abandoned and
+	/// cancelled. Defaults to 5 minutes. Set to <see cref="TimeSpan.Zero"/> or a negative value
+	/// to wait indefinitely.
+	/// </summary>
+	/// <remarks>
+	/// The system-browser flow used on desktop has no way to detect the browser window being
+	/// closed, so without this timeout an abandoned sign-in never completes and the awaiting
+	/// command stays busy forever.
+	/// </remarks>
+	public TimeSpan? InteractiveTimeout { get; init; }
+
 	/// <summary>
 	/// When true, the token cache falls back to an unprotected (plaintext) cache file if the
 	/// platform's secure storage (keychain / keyring / DPAPI) isn't available, so sign-in state
```
