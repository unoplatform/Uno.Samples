# Destination Rolodex · Uno Platform

A reusable destination card for Uno Platform, revision 2 "Living Postcard": one photograph fills a warm-rimmed card
behind an infinite country list, an integrated serif caption and quiet overlaid controls. One continuous picker
position drives the rows, the photo effect between the adjacent pair (**Morph Blend** or **Liquid Warp**, runtime SkSL
on an `SKCanvasElement`), the geographic globe, the photo-filled moving letters, the caption treatment and the material
colors. Pointer parallax, Look around (constrained pan/zoom/tilt, entered through `ViewMode`; no in-card button), an explicit tour (starts paused), loading/failure/retry,
reduced motion.

## Stack

| | |
| --- | --- |
| .NET SDK | 10.0.303 |
| Uno.Sdk | 7.0.22 (pinned in `global.json`) |
| SkiaSharp | 4.156.0 (bundled by the SDK; `SKRuntimeEffect` + `SKRuntimeShaderBuilder`) |
| MVVM | CommunityToolkit.Mvvm 8.4.2 (demo page only; the card uses dependency properties) |
| Heads | `net10.0-desktop` (Skia, Win32 host), `net10.0-android` (Skia renderer), `net10.0-browserwasm` (Skia) |
| Fonts | Inter 4.1 static instances + Source Serif 4 Regular (place caption), both SIL OFL 1.1 (`DestinationRolodex/Assets/Fonts`) |
| Globe | Natural Earth 110m land mask (public domain), `Assets/Globe/landmask.bin` |

## Build and run

```powershell
cd DestinationRolodex
dotnet build DestinationRolodex/DestinationRolodex.csproj -f net10.0-desktop -c Debug
dotnet run --project DestinationRolodex/DestinationRolodex.csproj -f net10.0-desktop
dotnet test DestinationRolodex.Tests/DestinationRolodex.Tests.csproj
```

Android: `dotnet build -f net10.0-android` then `dotnet build -f net10.0-android -t:Run` with an emulator running.
WebAssembly: `dotnet run -f net10.0-browserwasm`.

## Component API

```xml
xmlns:controls="using:DestinationRolodex.Controls"

<controls:DestinationRolodexCard x:Name="Card"
    Destinations="{x:Bind ViewModel.Destinations, Mode=OneWay}"
    SelectedIndex="{x:Bind ViewModel.SelectedIndex, Mode=TwoWay}"
    SelectedDestinationId="{x:Bind ViewModel.SelectedId, Mode=TwoWay}"
    TransitionKind="MorphBlend"            <!-- MorphBlend | LiquidWarp -->
    TransitionDuration="0:0:1.2"           <!-- clamped 350–3000 ms -->
    TransitionStrength="0.6"               <!-- clamped 0–1 -->
    IsAutoPlayEnabled="False"              <!-- the tour starts paused; Play is explicit -->
    AutoPlayInterval="0:0:5.5"             <!-- minimum 2.5 s -->
    IsReducedMotionEnabled="{x:Bind ViewModel.ReduceMotion, Mode=OneWay}" />
```

Read-only state: `ViewMode` (Browse/Look), `RendererStatus` (Shader / PlainImage / Unavailable), `MediaStatus`
(Empty / Loading / Ready / Failed), `IsPlaying`, `SelectedDestination`, `CurrentFrame` (q, pair, fraction, travel).
Lab-only: `DiagnosticCrossfade` (plain dissolve for comparison; never an advertised mode).
Methods: `Next()`, `Previous()`, `GoTo(int)`, `Play()`, `Pause()`, `ZoomIn()`, `ZoomOut()`, `ResetCamera()`, `Retry()`;
debug-only `ReplayTransitionAsync()` and `SeekTransitionAsync(double)`.
Events: `SelectionChanged` (candidate, at the half step), `SelectionSettled` (arrival), `TransitionStarted`,
`TransitionProgress`, `TransitionCompleted` (lab previews carry `IsPreview`), `MediaFailed`, `RendererFallback`, `ViewModeChanged`.

`Destination` records come from `DestinationRepository.LoadPackagedAsync()` (`Assets/Data/destinations.json`, photos in
`Assets/Destinations/{id}.jpg`) or from any host-built list. Two cards on one page share nothing.

## Layout

```text
DestinationRolodex/
  Models/        Destination, FocalPoint, TransitionSettings, enums, CircularMath
  Services/      DestinationRepository (JSON → model, ms-appx photo URIs), DestinationMediaCache (async decode, LRU budget, pinning, deferred disposal)
  Rendering/     TransitionShaderSource (SkSL), PhotoRenderer (photo effect, scrim, pointer light, country rows + photo-in-type), SceneFrame + MaterialColors (one position drives everything), GlobeRenderer/GlobeElement (orthographic globe), CardShadowElement, PhotoTransitionSurface, TransitionCoordinator (lab replay/seek)
  Input/         CountryPickerController (spring, drag, wheel, keys), CameraController, Tween
  Controls/      DestinationRolodexCard, CountryPicker, GlyphLayout
  Presentation/  DestinationDemoViewModel, SpikePage (DEBUG shader diagnostic)
  Harness/       DEBUG command-line switches, FixtureExport, CaptureHelper
  Styles/        DestinationRolodexResources.xaml (tokens, fonts, icons)
```

## Verification harness (Debug builds)

```powershell
$exe = "DestinationRolodex/bin/Debug/net10.0-desktop/DestinationRolodex.exe"
& $exe --export-fixtures=fixtures                         # raw shader frames + endpoint/camera checks + report.json
& $exe --capture=out.png --viewport=390x844 --look        # full-card capture in a given state
& $exe --capture=out.png --select=jp --media-fail=jp      # failure + Retry state
& $exe --capture=out.png --position=0.5                   # position-driven scene held at q = 0.5 (LP03)
& $exe --capture=out.png --steps=1000                     # 1,000-step loop; writes out.png.json with cache/memory stats
& $exe --perf=perf.json --viewport=1440x1040              # 12 s autoplay frame statistics
& $exe --spike                                            # raw photo-layer diagnostic page
```

