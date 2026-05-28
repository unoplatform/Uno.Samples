# 〰️ Liveline — *Real-time animated line-chart control*

> A reusable SkiaSharp line-chart control for Uno Platform that smoothly lerps between incoming values, auto-scales its Y axis, and idles when the data settles.

<!-- 📸 Add a screenshot of Liveline.Demo: drop it in screenshots/Liveline/ and point the src below at it -->
<!-- <img src="../screenshots/Liveline/<file>.png" alt="Liveline chart" width="640" /> -->
> 📸 *Screenshot coming.*

## What you get
A **control library**, not just an app — `src/Liveline/` ships as `OutputType=Library`, and `samples/Liveline.Demo/` wires every property to a live control panel. This is the chart engine behind **Meridian**.

## Highlights
- **`SKCanvasElement` rendering** — gradient area fill, tracking line, value badge, and a momentum indicator, all on a Skia surface.
- **Driven by `CompositionTarget.Rendering`** — animates toward new values via `LerpSpeed`, then **stops invalidating once values converge** (idle-friendly).
- **Allocation-light steady state** — paints/fonts are reused; native Skia resources release on `Unloaded` and recreate on re-`Loaded`, so it's safe to navigate away and back.
- **Drop-in API** — `Data` (`IList<LivelinePoint>`), `Value`, `Theme`, plus `ShowGrid` / `ShowBadge` / `ShowFill` / `Momentum` / `IsLoading` / `IsPaused` toggles.
- **Demo host** wired with the full Uno.Extensions stack (`IHostBuilder`, region nav, MVVM) so it behaves like a real sample.

## Stack & platforms
Reusable library + MVVM demo host · Uno.Sdk 6.5.36+ · `net10.0-desktop` · `net10.0-browserwasm`

---

# Liveline — full documentation

A real-time animated line-chart control for [Uno Platform](https://platform.uno/), built on **SkiaSharp** (`SKCanvasElement`). Liveline smoothly lerps between incoming values, auto-scales its Y axis, and renders a gradient area fill, tracking line, value badge, and momentum indicator — driven by `CompositionTarget.Rendering` so it idles when the data settles.

It runs anywhere the Uno Skia renderer runs (Desktop, WebAssembly, mobile).

## Projects

| Path | Description |
|------|-------------|
| `src/Liveline/` | The reusable control library (`OutputType=Library`). |
| `samples/Liveline.Demo/` | A demo app that wires every property to a control panel. |

Open `Liveline.sln` and run **Liveline.Demo** to see it live.

## Usage

Add a reference to the `Liveline` project (or package), then:

```xml
<Page xmlns:liveline="using:Liveline">
    <liveline:LivelineChart x:Name="Chart"
                            ShowGrid="True"
                            ShowBadge="True"
                            ShowFill="True"
                            Momentum="Auto"
                            LerpSpeed="0.04" />
</Page>
```

Feed it data from code-behind or a view model:

```csharp
using Liveline.Models;

Chart.Theme = new LivelineTheme { Color = "#4CAF50", IsDark = true };
Chart.Data = points;     // IList<LivelinePoint>
Chart.Value = latest;    // current value highlighted by the badge/dot
```

`LivelinePoint` is an immutable record of `(DateTimeOffset Time, double Value)`. Push a new
list (or append and reassign) as data arrives; the chart animates to the new shape.

## Properties

| Property | Type | Default | Purpose |
|----------|------|---------|---------|
| `Data` | `IList<LivelinePoint>` | `null` | The series to render. |
| `Value` | `double` | `0` | Current value for the badge / live dot. |
| `Theme` | `LivelineTheme` | `null` | Accent color (`#RRGGBB`) + light/dark. |
| `ShowGrid` | `bool` | `true` | Draw gridlines + axis labels. |
| `ShowBadge` | `bool` | `true` | Draw the floating value badge. |
| `ShowFill` | `bool` | `true` | Draw the gradient area fill. |
| `Momentum` | `MomentumDirection` | `Auto` | Momentum indicator: `Off`, `Auto` (derive from delta), or forced `Up`/`Down`/`Flat`. |
| `LerpSpeed` | `double` | `0.04` | Interpolation rate toward new values (0–1). |
| `IsLoading` | `bool` | `false` | Show the breathing loading state. |
| `IsPaused` | `bool` | `false` | Freeze the animation. |

## Notes

- Rendering is allocation-light in the steady state (paints/fonts are reused) and stops
  invalidating once values converge.
- Native Skia resources are released on `Unloaded` and recreated on re-`Loaded`, so the
  control is safe to navigate away from and back to.
- Targets `net10.0-desktop;net10.0-browserwasm`; requires Uno.Sdk 6.5.36+.

## Demo app architecture

The `Liveline.Demo` host is wired with the standard Uno.Extensions stack so it
behaves like a real Uno sample and not a WinUI control sandbox:

- **`IHostBuilder`** bootstrap in `App.xaml.cs` (`CreateBuilder` ->
  `UseToolkitNavigation` -> `UseLogging` -> `ConfigureServices` ->
  `UseNavigation`).
- **Region-based navigation** via `Shell` + `IContentControlProvider`, with
  routes registered through `ViewMap` / `RouteMap` (`Shell -> MainPage`).
- **MVVM** with `CommunityToolkit.Mvvm` (`ObservableObject`,
  `[ObservableProperty]`, `[RelayCommand]`). `MainViewModel` owns the
  random-walk timer, accent color, dark/light, and the layer toggles; the
  XAML binds chart properties directly to the VM.
- **DI** registers the page VM (`AddTransient<MainViewModel>`); the
  navigation system constructs it for each navigation.

The chart control itself (`src/Liveline/`) is the showcase asset and stays
host-agnostic.
