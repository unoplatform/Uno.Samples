# Claude Code Tracker

A cross-platform **usage-analytics dashboard for Claude Code**, built with
[Uno Platform](https://platform.uno/). It tracks coding sessions, token consumption
(input / output / cache read / cache write) and cost across Claude models, from a single
codebase running on **Windows, macOS, Linux (Skia), iOS, Android and WebAssembly**.

The data is mock/seeded (see `Presentation/Data/SampleData.cs`) so the app runs with no
backend — swap that one file for a real service to go live.

## Features

- **Adaptive navigation shell** — a `NavigationView` rail on wide screens that switches to a
  bottom `TabBar` on phones, driven entirely by the Toolkit `{utu:Responsive}` markup extension.
- **Dashboard** — budget gauge with a trend indicator, KPI cards (total tokens, sessions,
  active days, average cost per session), a per-model usage breakdown, and recent sessions.
- **Sessions** — searchable, model-filterable list (filter chips) that navigates to a
  **Session Detail** page showing per-token-type cost, cache efficiency and topic tags for the
  tapped session.
- **Usage** — plan summary, token breakdown, rate-limit gauges, per-model usage & cost, and a
  model pricing reference.
- **Charts** — daily-cost line, sessions-per-day bars, and token-type / model-share donuts
  (LiveCharts2), themed to the active light/dark palette.
- **Theming** — the Uno **Simple** theme with a custom purple/teal palette and a distinct dark
  theme (`ThemeColors.xaml`).
- **Localization** — English, Spanish, French and Portuguese (Brazil) for navigation and page
  titles, via `x:Uid` + `Strings/*/Resources.resw`.

## Architecture

- **MVUX + Uno.Extensions Navigation.** Each page has a `partial record` Model
  (`DashboardModel`, `SessionsModel`, `UsageModel`, `ChartsModel`, `SessionDetailModel`)
  registered via `ViewMap` / `DataViewMap` in `App.xaml.cs`; Navigation resolves and injects the
  Model as the page `DataContext`. The Session → Session Detail flow passes the selected
  `SessionEntry` with `uen:Navigation.Data` and a `DataViewMap`, and the detail Model derives its
  figures from that entry plus `ModelCatalog` pricing.
- **Single data source.** `Presentation/Data/SampleData.cs` seeds one dataset that every page
  projects from, so the headline numbers, lists and charts stay consistent. `Fmt` centralizes
  number/currency/date formatting.

## Running

The app uses the Uno single-project model. Pick a target framework head:

```bash
# WebAssembly
dotnet run -f net10.0-browserwasm

# Desktop (Skia)
dotnet run -f net10.0-desktop

# iOS / Android — launch from your IDE, or:
dotnet build -f net10.0-ios
dotnet build -f net10.0-android
```

See <https://aka.platform.uno/get-started> for prerequisites and IDE setup.
