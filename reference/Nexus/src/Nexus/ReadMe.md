# Nexus

An industrial-SCADA-style monitoring dashboard: production lines, maintenance, alerts, KPIs. Dark / light theme aware.

## What this shows

- **MVUX Model layer done well.** `OverviewModel.cs` is a `partial record` with `IFeed<DashboardMetrics>` / `IListFeed<ProductionLine>` / `IListFeed<Alert>` sourced from `INexusService` (singleton, mock implementation).
- **`FeedView` with all four templates.** `ProgressTemplate` + `ErrorTemplate` + `NoneTemplate` + `ValueTemplate` populated on every list and metric surface — exemplary state coverage.
- **Industrial-SCADA identity.** Space Grotesk + IBM Plex Mono type system. KPI cards with sparkline area-fills, status pills, a connection-dot pulse in the shell.
- **Theme-aware Dark / Light.** Full Light dictionary at `NexusTheme.xaml:93–166`; default is Dark.
- **Lightweight TabBar styling** for the navigation chrome (`MainPage.xaml`, scoped `<utu:TabBar.Resources>`).

## How to run

```powershell
dotnet run --project Nexus/Nexus.csproj -f net10.0-desktop
```

Mock data lives in `Services/NexusService` behind `INexusService`. No backend; nothing to configure.

## Target platforms

| TFM                | Verified | Notes                       |
|--------------------|:--------:|-----------------------------|
| `net10.0-desktop`  | yes      | Desktop-only by construction. |

## Architecture at a glance

- **MVUX** for the Model layer (`UnoFeatures: MVUX`).
- `IHostBuilder` with `INexusService` → mock service registered as singleton.
- **Region-based navigation (Uno.Extensions).** `Shell.xaml` is a bootstrap `ContentControl`; `MainPage.xaml` carries the chrome (header / tabs / content region / footer) with `uen:Region.Attached="True"` on the root grid and `uen:Region.Navigator="Visibility"` on the content area. Each `utu:TabBarItem` carries `uen:Region.Name="<RouteName>"` matching a nested `RouteMap`. Routes are registered in `App.xaml.cs:RegisterRoutes` with `ReactiveViewModelMappings.ViewModelMappings` so MVUX `Model` records bind through their generated bindable proxies. Default route: `/Main/Overview`.
- Page code-behind is minimal: `InitializeComponent()` only, except `OverviewPage` which attaches `LineItemHoverBehavior` to its visual tree once on `Loaded`. The framework wires each page's `DataContext` via the `ViewMap<Page, Model>` registration.

## Known limitations

- **Single TFM.** Desktop only; rubric caps Platform Behavior here. `Platforms/Android` / `iOS` / `Wasm` folders are not present.
- **Localization covers the shell + signature panel titles, not the whole UI.** `Strings/en|es|fr|pt-BR/Resources.resw` carry 16 strings each — five tab labels (`Shell_Tab_*`), the brand subtitle, sys-time/connected/footer labels, and one panel title per page (e.g. `Pages_Overview_ProductionLines`). Table headers, mock-data row content, and template-internal text remain unlocalized.
- **No `SafeArea`, no `utu:Responsive`.** Pages handle narrow widths via `<ScrollViewer HorizontalScrollBarVisibility="Auto">` over `<Grid MinWidth="800">` — the dashboard scrolls horizontally below ~800 px instead of collapsing.
- **Connection-dot pulse uses an explicit three-keyframe loop** (`AutoReverse` is not implemented on Skia desktop). `MainPage.xaml.cs:StartConnectionPulse` drives Opacity 1.0 -> 0.5 -> 1.0 over 2 s with `SineEase`, on a Storyboard with `RepeatBehavior=Forever`.
- **5 `FontSize="10"` literals** on `utu:TabBarItem Content` strings in `MainPage.xaml` — should be a `NexusTabLabelStyle`.
- **20 hex literals** in page XAML, most duplicating values already in the theme dictionary (`MainPage.xaml` scoped TabBar resources, `OverviewPage.xaml` gradient stops).

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, Logging, MVUX, Localization, Navigation, ThemeService, SkiaRenderer.
