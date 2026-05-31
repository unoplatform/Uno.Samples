# FieldOpsPro

A dispatcher dashboard for a fictional field-service business: work orders, agents, a live map, team roster, schedule, reports.

## What this shows

**FieldOpsPro is the MVUX + region-nav reference sample in this shortlist.** If you want the textbook Uno patterns, start here.

- **Textbook MVUX.** `ShellModel` and `DashboardModel` are `partial record`s with `IFeed<T>` / `IListFeed<T>` / `IState<T>` properties; commands auto-generate from `public ValueTask` methods on the models.
- **Region-based navigation.** `App.xaml.cs` wires `UseToolkitNavigation()` + `UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)`. Routes are nested under a `ShellModel` root with 8 section routes (`Dashboard`, `Map`, `Tasks`, `Team`, `Schedule`, `Reports`, `Settings`, `Profile`).
- **Mobile nav uses the route-based API**, not code-behind `Frame.Navigate`: `this.Navigator().NavigateRouteAsync(...)`.
- **Zero raw hex literals**, **zero numeric `FontSize`** in 25 page/control XAMLs. All color resolves through `BgPrimaryBrush` / `AccentPrimaryBrush` / `TextMutedBrush`; all text through `FontHeadlineLarge` / `FontBodyMedium` etc.
- **Binding split done right.** `{Binding}` on `FeedView` surfaces, `x:Bind` only inside `DataTemplate`s with `x:DataType`.
- **Four TFMs declared** — broadest in the shortlist — with `Platforms/{Android,iOS,WebAssembly,Desktop}` folders to match.
- **Responsive shell** on `MainPage` and `DashboardPage` via `AdaptiveTrigger` states at 0/600/1024.

## How to run

```powershell
# Desktop (Skia)
dotnet run --project FieldOpsPro/FieldOpsPro.csproj -f net10.0-desktop

# Other heads
dotnet build FieldOpsPro/FieldOpsPro.csproj -f net10.0-android
dotnet build FieldOpsPro/FieldOpsPro.csproj -f net10.0-ios
dotnet build FieldOpsPro/FieldOpsPro.csproj -f net10.0-browserwasm
```

Data comes from `Services/MockFieldOpsService` — `IFieldOpsService` is the contract; the mock implements every method with `Task.Delay`-shaped latency.

## Target platforms

| TFM                       | Verified | Notes                                          |
|---------------------------|:--------:|------------------------------------------------|
| `net10.0-desktop`         | yes      | Showcase target.                               |
| `net10.0-android`         | declared | Platform folder populated.                     |
| `net10.0-ios`             | declared | Platform folder populated.                     |
| `net10.0-browserwasm`     | declared | Platform folder populated.                     |

## Architecture at a glance

- **MVUX** (`UnoFeatures: MVUX`) — models are `partial record`, source-generated bindable VMs.
- **One DI registration:** `IFieldOpsService` → `MockFieldOpsService` (singleton). Zero service locators in code-behind.
- **Region nav** through Uno.Extensions Navigation; routes registered centrally in `App.xaml.cs`.
- Custom monochromatic theme via `MaterialToolkitTheme ColorOverrideSource` — Material is wired but the brand sits outside Material's defaults.

## Known limitations

- **Responsive coverage is uneven.** `MainPage` + `DashboardPage` adapt at 600/1024 breakpoints, but `MapPage`, `WorkOrdersPage`, `TeamPage`, `SchedulePage`, `ReportsPage`, `SettingsPage`, `ProfilePage` are desktop-only — no narrow content states.
- **Localization covers the shell + section titles, not the long tail.** Four locales (`en|fr|es|pt-BR`) carry ~33 strings via `x:Uid` — sidebar nav, mobile bottom nav, dashboard section headers, page titles, placeholder copy. Data-driven strings from `MockFieldOpsService` (agent names, addresses, activity messages, weather, badge counts) and short numeric labels in the chat / shift / CSAT controls are still English-only.
- **Five build warnings** — 4 nullable annotation noise in `NavItem.xaml.cs` / `TaskCard.xaml.cs`, 1 `Uno0001` on `CharacterSpacing` (not implemented on Skia desktop).
- `FeedView`s have `ValueTemplate` + `ProgressTemplate` but no `ErrorTemplate` / `NoneTemplate` — mock never errors, but states should still be demonstrated.

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, MVUX, Localization, Navigation, ThemeService, SkiaRenderer, Extensions.
