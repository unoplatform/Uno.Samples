# EnterpriseDashboard ("Observatory")

A monochromatic, identity-driven analytics dashboard. Three sections — Acquisition, Engagement, Cohorts — each a bento grid of hand-built Skia charts.

## What this shows

- **Bento composition with intent.** Chart tile sizes are explicitly chosen per chart: the MRR line is a `RowSpan="2" ColumnSpan="2"` hero, the cohort triangle spans full-row, signup/ARR tiles are 1×1. Composition is editorial, not generic-grid.
- **14 custom Skia charts.** Live in `Observatory/Charts/`. No charting library — pure `SKCanvasElement` rendering. Includes line, bar, stacked-bar, sparkline, distribution, slope, heatmap, ring, dot-plot variants.
- **Identity-driven typography.** Bricolage Grotesque (3 static weight instances) + JetBrains Mono with characterful negative letter-spacing on display sizes. See `Themes/ObservatoryTypography.xaml`.
- **Theme-aware brightness mapping.** Cohort heatmap mirrors luminance across Light / Dark (`CohortsPage.xaml.cs:78–79`) so the same data reads correctly in both themes.
- **Scroll-triggered chart animation.** `anim:ScrollTriggerBehavior` defers chart animation until the chart enters the viewport.
- **Theme overrides done right.** `Styles/ColorPaletteOverride.xaml` cleanly remaps Material MD3 slots to the Observatory palette in both `Light` and `Dark` dictionaries.

## How to run

```powershell
# Desktop (Skia)
dotnet run --project EnterpriseDashboard/EnterpriseDashboard.csproj -f net10.0-desktop

# WebAssembly
dotnet run --project EnterpriseDashboard/EnterpriseDashboard.csproj -f net10.0-browserwasm
```

## Target platforms

| TFM                       | Verified | Notes                                                                |
|---------------------------|:--------:|----------------------------------------------------------------------|
| `net10.0-desktop`         | yes      | Showcase target.                                                     |
| `net10.0-browserwasm`     | declared | 14 Skia charts are a real WASM stress test — verify before promoting. |

## Architecture at a glance

- `IHostBuilder` wires DI, logging, and `Uno.Extensions.Navigation` (`UseToolkitNavigation` + `UseNavigation(RegisterRoutes)`).
- Services: `IChartDataService` → `ChartDataService` (synthetic data); section VMs registered transient.
- ViewModels: `ObservatoryViewModel` (plain `INotifyPropertyChanged`) plus three thin subclasses (`AcquisitionViewModel`, `EngagementViewModel`, `CohortsViewModel`) so each `ViewMap` can target a distinct VM type. Charts bind through classic `{Binding}`.
- Navigation: region-based — `Shell.xaml` bootstraps a `ContentControl` host; `ShellPage.xaml` is the chrome (`NavigationView` + content region) where each `NavigationViewItem` carries `uen:Region.Name`; the content `Grid` uses `uen:Region.Navigator="Visibility"`. No code-behind `Frame.Navigate`.
- Charts data: `T[]` arrays computed once in the VM constructor; no observable collections.

## Companion docs

The design system is documented in depth at the solution root and under `docs/`:

- `01-architecture-brief.md`, `02-design-brief.md`, `03-animation-interaction-brief.md` — solution root
- `docs/00-overview.md` … `docs/09-implementation-notes.md` — full type system, color tokens, motion grammar, IA, responsive intent, accessibility, implementation notes.

Note: the project on disk is named `EnterpriseDashboard`. An internal `Observatory` sub-namespace and in-app wordmark are preserved by design.

## Known limitations

- **Partial responsive coverage — outer split only.** Each section page (`Acquisition`, `Engagement`, `Cohorts`) now collapses its outer `3*/1*` chart/info-rail split into a single column below 1280 px (custom `utu:ResponsiveLayout` resource `ObsBentoBreakpoints` in `App.xaml`, `Narrow=0 / Wide=1280`). At Narrow the info rail drops below the chart scroller and takes full width; the top hairline replaces the left hairline. At Wide the layout is byte-equivalent to before. **The inner bento itself is intentionally unchanged** — the 4-column Acquisition bento, 4-column Engagement bento, and 2-column upper Cohorts row still use their original hard row heights (240 / 260 / 300 / 340 / 440) and explicit `RowSpan` / `ColumnSpan` assignments. Below ~900 px the inner bento charts will still render narrow (Skia charts are not designed below ~400 px wide); the rail collapse is a partial mitigation, not a full responsive design. `ShellPage` `NavigationView` keeps its default `OpenPaneLength="248"` and built-in compact-mode behavior. No `SafeArea` — this app is desktop-shaped.
- **Synchronous data path.** No loading / error / empty states — fine for the synthetic-data showcase, not a demo of `FeedView`.
- **One locale.** `Strings/en/Resources.resw` only.

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures` (declared): Material, Hosting, Toolkit, Logging, MVUX, Extensions, Configuration, Navigation, Serialization, Skia, SkiaRenderer.
