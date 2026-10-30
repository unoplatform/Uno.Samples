# A typed REST client via Uno.Extensions.Http.Kiota

This sample calls a REST API from an Uno Platform app through a client that the
[Kiota](https://learn.microsoft.com/openapi/kiota/) CLI generates from an OpenAPI document,
registered with
[`Uno.Extensions.Http.Kiota`](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Http/HowTo-Kiota.html).
It is **design-first**: the contract,
[`Http.KiotaExtensionsDemo/Specs/TasksApi.yaml`](Http.KiotaExtensionsDemo/Specs/TasksApi.yaml),
is the source of truth. The server implements it, the app's client is generated from it, and the
tests run every operation through the app's own registration against the real server.

The contract deliberately goes beyond plain JSON: a `text/plain` response, a
`multipart/form-data` upload, `application/problem+json` validation errors, a typed 404 error, and
an endpoint that answers 503 with `Retry-After` while it warms up.

Tested on:

- [x] Desktop (Skia) on Windows 11, Debug and Release: Uno.Sdk 7.0.0-dev.115, Uno.Extensions 8.0.0-dev.150
- [x] Headless API tests (`Http.KiotaExtensionsDemo.Tests`, NUnit, the server in-process) on Windows 11, .NET SDK 10.0.400

Built but not run: WebAssembly (Debug and Release; the server was checked to serve it), Android
(Debug). Not built or run: iOS (needs macOS).

This document is the **setup guide**. For what the sample *demonstrates* and where, see
[`Http.KiotaExtensionsDemo/ReadMe.md`](Http.KiotaExtensionsDemo/ReadMe.md).

---

## It runs as is

Start the server first, from this folder:

```bash
dotnet run --project Http.KiotaExtensionsDemo.Server
# listens on http://localhost:5001
```

Then run a head (below). The app's configuration points at that server, in **both**
`appsettings.json` (Release builds, the Production environment) and
`appsettings.development.json` (Debug builds):

```json
"ApiClient": {
  "Url": "http://localhost:5001"
}
```

Without a `Url` the generated client does not fail: it silently calls the first `servers` entry
of the contract, `https://tasks.example.com`, a placeholder. For a deployed API, change the value
in `appsettings.json`. The files are embedded resources, so **rebuild** after editing them.

The server keeps its data in memory, seeded with five tasks; restarting it resets them. Its
summary report answers `503` with `Retry-After: 1` to its first two calls after it starts, so the
app's summary card shows its progress state for about two seconds, then the numbers.

### Versions

| Component | Version |
| --- | --- |
| Uno.Sdk (`global.json`) | 7.0.0-dev.115 |
| Uno.Extensions (pinned by the Sdk) | 8.0.0-dev.150 |
| Microsoft.Kiota.* runtime (brought by the `HttpKiota` feature) | 1.22.2 |
| Kiota CLI the client was generated with (`kiota-lock.json`) | 1.37.0 |
| .NET SDK | 10.0 |

Two workarounds are in the app's csproj, both commented there:

- **Debug builds reference `Uno.WinUI.Lottie`.** On Uno.Sdk 7.0.0-dev.115 the Hot Design tooling
  still references `Uno.UI.Lottie`, which Uno.WinUI no longer ships; without the package a Debug
  build on desktop starts with no window and never calls the API
  ([unoplatform/uno.extensions#3262](https://github.com/unoplatform/uno.extensions/issues/3262)).
- **The plain `net10.0` target skips Uno's ICU initializer.** That target only exists so the test
  project can reference the app; without the skip, every test fails with
  `Failed to load 'icuuc77'`.

The registration itself works around
[unoplatform/uno.extensions#2799](https://github.com/unoplatform/uno.extensions/issues/2799); see
the app's ReadMe.

---

## Per-head setup

The commands are run from this folder, with the server already running. Every head reads
`ApiClient:Url`; the heads whose address differs adjust it in `App.xaml.cs` (`ApiUrl`).

### Desktop (Skia)

**Project changes:** none.

```bash
dotnet run --project Http.KiotaExtensionsDemo/Http.KiotaExtensionsDemo.csproj -f net10.0-desktop
```

A Debug build logs every request (`System.Net.Http.HttpClient.TasksApiClient`) to the console.

### WebAssembly

**Project changes:** none. The server also hosts this head, so the app calls its own origin (`ApiUrl`
reads `window.location.origin`): no CORS, and no address to configure when you deploy both
together. Run it through the server:

```bash
dotnet run --project Http.KiotaExtensionsDemo.Server
# open http://localhost:5001
```

Started on its own dev server (`dotnet run -f net10.0-browserwasm`, port 5000) the head has no API
on its origin, and every call fails. The server only serves the head's files from the build output
in the Development environment (its launch profile, which `dotnet run --project` uses); started
with `--no-launch-profile` it runs as Production and `/` answers 404 until you publish the server.

### Android

**Project changes:** none for the emulator. The emulator reaches the development machine at
`10.0.2.2`, so `ApiUrl` replaces `localhost` with it, and
`Platforms/Android/Resources/xml/network_security_config.xml` allows cleartext HTTP to that
address only. A physical device needs the machine's LAN address in both places and the server
bound to it (`--urls http://0.0.0.0:5001`).

```bash
dotnet build Http.KiotaExtensionsDemo/Http.KiotaExtensionsDemo.csproj -f net10.0-android -t:Run
```

### iOS

**Project changes:** none for the simulator, which shares the Mac's network, so the server must
run on the Mac. `Platforms/iOS/Info.plist` sets `NSAppTransportSecurity` >
`NSAllowsLocalNetworking` so App Transport Security allows plain HTTP to `localhost`.

```bash
dotnet build Http.KiotaExtensionsDemo/Http.KiotaExtensionsDemo.csproj -f net10.0-ios -t:Run -p:RuntimeIdentifier=iossimulator-arm64
```

---

## Running the tests

```bash
dotnet test Http.KiotaExtensionsDemo.Tests
```

No server needs to be running: the tests host it in-process (`WebApplicationFactory`), and each
test gets a fresh one. They call every operation of the contract through the app's own
`AddTasksApi` registration (the same Uno and Kiota handlers; only the network handler is swapped
for the in-process server's). `AddKiotaClient_limits` documents why the app does not use plain
`AddKiotaClient`; when those tests start failing, the upstream issue is fixed.

---

## Change the API

The contract comes first. For any change, an agent or a developer follows these steps, in order,
from this folder:

1. **Edit the contract**, `Http.KiotaExtensionsDemo/Specs/TasksApi.yaml`. Keep its `servers`
   entry a placeholder: the app always sets the address itself.
2. **Update the server** to match: the endpoint in
   `Http.KiotaExtensionsDemo.Server/Apis/TasksApi.cs`, the wire records and the store in
   `Apis/TaskStore.cs`. The server has no OpenAPI generator on purpose; never save a generated
   document over the contract.
3. **Regenerate the client** with the Kiota CLI. `dotnet-tools.json` in this folder pins Kiota
   1.37.0 as a local tool, so nothing is installed machine-wide:

   ```bash
   dotnet tool restore
   cd Http.KiotaExtensionsDemo
   dotnet kiota update -o Content/Client/TasksApiClient
   cd ..
   ```

   `kiota update` reads `Content/Client/TasksApiClient/kiota-lock.json` (the contract's location,
   class name `TasksApiClient`, namespace `Http.KiotaExtensionsDemo.Client`). The equivalent full
   command, for a first generation or a renamed client:

   ```bash
   dotnet kiota generate -l CSharp -c TasksApiClient -n Http.KiotaExtensionsDemo.Client \
     -d Specs/TasksApi.yaml -o Content/Client/TasksApiClient --clean-output
   ```

   Never edit the generated files; add no package `kiota info` lists (the `HttpKiota` feature
   brings them).
4. **Map it in the app**: `Http.KiotaExtensionsDemo/Services/TasksService.cs` is the only code that
   sees the generated types. Add or change its method, then the record in `Models/Tasks.cs`. If
   the new operation sends or returns anything other than JSON, it already works: the
   registration keeps every serializer.
5. **Build**. A removed or renamed operation now fails to compile where it was called; fix the
   call sites.

   ```bash
   dotnet build Http.KiotaExtensionsDemo/Http.KiotaExtensionsDemo.csproj -f net10.0-desktop
   ```

6. **Test**: add a test for the operation in `Http.KiotaExtensionsDemo.Tests/TasksApiTests.cs`,
   then run `dotnet test Http.KiotaExtensionsDemo.Tests`.

---

## Troubleshooting

| Symptom | Cause | Fix |
| --- | --- | --- |
| Every load shows its error state; the footer says *Server unreachable* | The server is not running, or not at the address this head uses | Start `Http.KiotaExtensionsDemo.Server`; check the head's address above |
| Calls go to `https://tasks.example.com` | No `ApiClient:Url` reached the client, so the generated client fell back to the contract's `servers` entry | Set it in the appsettings file of the environment you run (Release reads `appsettings.json`), and rebuild |
| `ArgumentOutOfRangeException: ... expected a application/json content type` | The client was registered with plain `AddKiotaClient`, whose adapter only knows JSON | Register with `AddTasksApi` (`Services/TasksApiSetup.cs`) |
| The summary spins for about two seconds after the server starts | The warm-up: two 503s, each retried after `Retry-After: 1` | Expected |
| **Desktop:** Debug build, no window | Hot Design tooling references `Uno.UI.Lottie` (uno.extensions#3262) | Keep the Debug-only `Uno.WinUI.Lottie` reference |
| **Tests:** `Failed to load 'icuuc77'` | Uno's ICU initializer in the app's net10.0 target | Keep `SkipIcuDataInitializerGenerator` in the app's csproj |
| **WebAssembly:** every call fails | The head was started on its own dev server, which has no API | Open the head through the server, `http://localhost:5001` |
| **WebAssembly:** `http://localhost:5001/` answers 404 | The server runs as Production (`--no-launch-profile`), which does not serve the head from its build output | Run it with its launch profile, or publish it |
| **Android:** cleartext error | Plain HTTP to an address `network_security_config.xml` does not allow | Add the address there, or serve HTTPS |

## Relevant documentation

- [Uno.Extensions Kiota how-to](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Http/HowTo-Kiota.html)
- [Uno.Extensions HTTP overview](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Http/HttpOverview.html)
- [Kiota: build API clients for .NET](https://learn.microsoft.com/openapi/kiota/quickstarts/dotnet)
- [MVUX overview](https://platform.uno/docs/articles/external/uno.extensions/doc/Learn/Mvux/Overview.html)
