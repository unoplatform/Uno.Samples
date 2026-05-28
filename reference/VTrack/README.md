# 🎯 VTrack — *Live YOLO object detection & tracking*

> Upload a video and watch it play with YOLO bounding boxes + track IDs drawn over each frame, rendered as a GPU overlay on the video player.

<!-- 📸 Add a screenshot: drop it in screenshots/VTrack/ and point the src below at it -->
<!-- <img src="../screenshots/VTrack/<file>.png" alt="VTrack detection overlay" width="640" /> -->
> 📸 *Screenshot coming.*

## What you get
The **native-interop + GPU-overlay** sample: real OpenCV + YOLO inference on desktop, an honest zero-allocation hot path, and a clean platform-gating story.

## Highlights
- **Platform-gated native engine** — `ITrackingEngine` has a real desktop-only OpenCV + YOLO implementation and a `NullTrackingEngine` for other heads, so non-desktop TFMs build clean and run the UI without inference.
- **Custom GPU overlay** — `VideoOverlayCanvas` is an `SKCanvasElement` (Skia surface drawn directly), not WinUI shapes on a `Canvas`.
- **Honest hot path** — buffers reused across ticks (no per-frame allocations), inference `Task.Run`-ed off the UI thread, `unsafe Buffer.MemoryCopy` straight into `SKBitmap` pixels, single in-flight flag.
- **Typed nav hand-off** — the selected `VideoFile` rides as `DataViewMap` nav data from upload → analysis.
- **Multi-project layout** (`VTrack` app + `VTrack.DataContracts`), 4 localized locales, zero hex literals in page XAML.

## Stack & platforms
**MVUX** + region nav · OpenCV + YOLO (desktop) · Uno.Sdk 6.5.36 · `net10.0-desktop` ✅ (real inference), `-android` / `-ios` / `-browserwasm` (chrome-only)

## Run it
```powershell
dotnet run --project VTrack/VTrack.csproj -f net10.0-desktop
```
