# 🛠️ FieldOpsPro — *Dispatcher command center*

> A dispatcher dashboard for a fictional field-service business: work orders, agents, a live map, team roster, schedule, reports.

<img src="../screenshots/FieldOps/FieldOps/Screenshot%202026-05-28%20123659.png" alt="FieldOpsPro Command Center" width="640" />

## What you get
**The MVUX + region-navigation reference sample in this shortlist.** If you want the textbook Uno patterns end-to-end, start here.

## Highlights
- **Textbook MVUX** — `ShellModel` / `DashboardModel` are `partial record`s with `IFeed<T>` / `IListFeed<T>` / `IState<T>`; commands auto-generate from `public ValueTask` methods.
- **Region-based navigation** — a `ShellModel` root with 8 section routes; mobile nav uses `Navigator().NavigateRouteAsync(...)`, never code-behind `Frame.Navigate`.
- **Disciplined XAML** — zero raw hex literals and zero numeric `FontSize` across 25 page/control XAMLs; `{Binding}` on FeedView surfaces, `x:Bind` only inside templated `DataTemplate`s.
- **Broadest platform reach** — four TFMs declared with matching `Platforms/` folders.
- **Responsive shell** — `AdaptiveTrigger` states at 0 / 600 / 1024 on the main + dashboard pages.

## Stack & platforms
**MVUX** · one DI registration (`IFieldOpsService` → mock, singleton) · Uno.Sdk 6.5.36 · `net10.0-desktop` ✅, `-android` / `-ios` / `-browserwasm` (declared)

## Run it
```powershell
dotnet run --project FieldOpsPro/FieldOpsPro.csproj -f net10.0-desktop
```
