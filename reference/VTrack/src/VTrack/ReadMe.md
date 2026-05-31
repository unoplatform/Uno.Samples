# VTrack

Live YOLO object detection and tracking, rendered as a GPU overlay on a video player. Upload a video, watch it play with bounding boxes + IDs drawn over each frame.

## What this shows

- **Multi-project layout.** `VTrack` (app head) + `VTrack.DataContracts` (shared records). Slightly more boilerplate today; zero refactor cost when a backend lands.
- **Platform-gated native engine.** `ITrackingEngine` has two implementations — `TrackingEngine` (real OpenCV + YOLO inference, desktop-only via the `VTRACK_DESKTOP` `DefineConstants` switch and `$(TargetFramework.Contains('-desktop'))` `PackageReference` gates) and `NullTrackingEngine` (chrome-only for non-desktop heads). Non-desktop TFMs build clean and run the UI without inference.
- **Custom GPU overlay.** `VideoOverlayCanvas` is an `SKCanvasElement` (`Uno.WinUI.Graphics2DSK`) — Skia surface drawn directly, not WinUI shapes on a `Canvas`.
- **Honest hot path.** `TrackingEngine.Advance` reuses `_frame` / `_bgraMat` / `_detectFrame` / `_frameBitmap` across ticks (no per-frame allocations), inference is `Task.Run`ed and the UI thread polls the last completed result, `unsafe Buffer.MemoryCopy` from OpenCV `Mat` straight into `SKBitmap` pixels. Producer/consumer pattern with a single in-flight flag.
- **`DataViewMap` for the upload → analysis hand-off.** The selected `VideoFile` rides as typed nav data — the textbook MVUX/Navigation pattern.
- **4 localized locales** (`en`, `es`, `fr`, `pt-BR`) with `x:Uid`.
- **Zero hex literals** in page XAML; zero numeric `FontSize` on text elements. All text styles resolve through Material type-scale resources (`DisplaySmall`, `TitleMedium`, `BodyMedium`, etc.).

## How to run

```powershell
# Desktop (Skia) — the showcase target with real inference
dotnet run --project VTrack/VTrack.csproj -f net10.0-desktop

# Other heads (build clean; UI only, no inference)
dotnet build VTrack/VTrack.csproj -f net10.0-android
dotnet build VTrack/VTrack.csproj -f net10.0-ios
dotnet build VTrack/VTrack.csproj -f net10.0-browserwasm
```

YOLO model weights are loaded by `Services/YoloDetector`. See the file for the expected path / format.

## Target platforms

| TFM                       | Verified | Notes                                                                |
|---------------------------|:--------:|----------------------------------------------------------------------|
| `net10.0-desktop`         | yes      | Showcase target. Real OpenCV + YOLO inference.                       |
| `net10.0-android`         | declared | `NullTrackingEngine` keeps the UI functional; no inference.          |
| `net10.0-ios`             | declared | Same — chrome-only.                                                  |
| `net10.0-browserwasm`     | declared | Same — chrome-only.                                                  |

## Architecture at a glance

- **MVUX.** Models are `partial record`s with feeds and `ValueTask` methods (auto-generated commands).
- `IHostBuilder` + `UseToolkitNavigation` + `UseConfiguration` + `UseLocalization` + `UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)`.
- Routes: `Shell` parent with `Home` (default) and `VideoAnalysis` regions; `VideoAnalysis` uses `DataViewMap<…, VideoFile>` for typed data hand-off.
- Services: `ITrackingEngine` (transient, factory-resolved with the `VTRACK_DESKTOP` swap), `YoloDetector`.
- Custom DaVinci-Resolve-inspired teal palette via `Styles/ColorPaletteOverride.xaml`.

## Known limitations

- **`VideoAnalysisPage` is fixed two-column** — `Width="*"` + `Width="320"`. Below ~1100 px wide the 320-px sidebar squeezes the video. No `utu:Responsive` collapse.
- **No `FeedView`.** Empty / loading / error states are hand-rolled with `Visibility` converters bound to `IsUploading` / `IsProcessing` / `ShowEmptyState` / `MediaError` on the Model. Functional; `FeedView` would consolidate.
- **Icon-only buttons** (`PlayPause`, `ToggleMute`) don't have `AutomationProperties.Name` — `x:Uid` covers most user-facing strings but icon buttons need explicit names.

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, MVUX, Logging, Configuration, Localization, Navigation, ThemeService, SkiaRenderer, Skia.
