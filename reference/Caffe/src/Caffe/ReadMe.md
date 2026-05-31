# Caffe

A single-page espresso brew configurator. Pick a shot type, dial in temperature / grind / extraction, watch the cup fill.

## What this shows

- **Type-scale discipline.** Every text element resolves through `CaffeFontSize*` tokens in `Styles/AppResources.xaml`. Zero raw `FontSize` on `TextBlock`.
- **Material with brand identity.** Material theme via `MaterialToolkitTheme`, brand colors via `Styles/ColorPaletteOverride.xaml`, brand brushes alias Material tokens — single source of truth.
- **Responsive controls, not just responsive shells.** `utu:Responsive` reflows the espresso card grid + right panel between stacked-mobile and side-by-side-desktop. Inner controls (`TemperatureGauge`, `GrindSelector`, `ExtractionArc`) adapt their thumb sizes / paddings / slider widths.
- **Visual identity from XAML primitives.** Thermometer, extraction arc, grind-particle visuals are pure XAML — no SVG, no PNG.
- **`utu:SafeArea Insets="VisibleBounds"`** at the root for notch / status-bar safety on mobile.

## How to run

```powershell
# Desktop (Skia)
dotnet run --project Caffe/Caffe.csproj -f net10.0-desktop

# Android
dotnet build Caffe/Caffe.csproj -f net10.0-android
```

## Target platforms

| TFM                | Verified | Notes                                                  |
|--------------------|:--------:|--------------------------------------------------------|
| `net10.0-desktop`  | yes      | Showcase target.                                       |
| `net10.0-android`  | declared | Platforms/Android folder populated, not runtime-verified. |

## Architecture at a glance

- **`IHostBuilder` + DI + region-based navigation.** App boots through `CreateBuilder().Configure(...).UseNavigation(RegisterRoutes)` and lands on `Shell` → `MainPage` via a `ViewMap`/`RouteMap` registry. `MainViewModel` is resolved from the DI container, not `new`'d in code-behind.
- **MVVM** with `CommunityToolkit.Mvvm` (`[ObservableProperty]`, `[RelayCommand]`). Single page, no async data — synchronous tool, no `IFeed`/`IListFeed`, so MVUX would be overkill.
- Brew driven by a `DispatcherTimer` at 16 ms ticks; one double assignment per tick.
- `EspressoItem` is an immutable record; `GrindLevel` is an enum with extension methods for labels.

## Known limitations

- **Localization is scaffold-default.** `Strings/en/Resources.resw` contains only `ApplicationName`; all UI strings are hardcoded English. Suitable for the brand-driven look; not yet a localization demo.

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, Logging, Mvvm, Extensions, Navigation, ThemeService, SkiaRenderer.
