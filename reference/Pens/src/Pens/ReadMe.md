# Pens

A beer-league hockey team app — schedule, chat, beer leaderboard, locker-room duties, roster. Built for the (fictional) Dorval Youngtimers Penguins.

## What this shows

- **Responsive shell.** Desktop side rail ↔ mobile bottom-bar swap via `utu:Responsive` on `MainPage.xaml`. `utu:SafeArea.Insets` on the top header and bottom tab strip handles notches and status bars.
- **Real-or-mock backend, config-driven.** `ISupabaseService` has two implementations — `SupabaseService` (real) and `MockSupabaseService` (built-in fixture data). The app reads `Supabase:Url` + `Supabase:AnonKey` from configuration at startup; if both are present it uses the real service, otherwise it falls back to the mock. **The distributed build ships with empty values, so it always runs on mock data and never touches anyone's backend.** See `SUPABASE.md` for opt-in setup.
- **Identity-driven typography.** Bebas Neue for titles, Barlow for body, hand-picked spacing and case. Type styles live in `Themes/TextStyles.xaml`.
- **Region-based navigation.** `App.xaml.cs` uses `UseToolkitNavigation` + nested `RouteMap`s (Login gate → tab shell → 5 section routes). No code-behind `Frame.Navigate`.
- **UI-thread marshalling for background-constructed VMs.** Navigation constructs VMs off the UI thread; `UiThread.Run` marshals bound-state updates back. See `App.xaml.cs:69–71` and `ScheduleViewModel.cs:80`.
- **4 localized locales** (`en`, `es`, `fr`, `pt-BR`) with `x:Uid` on user-facing strings.

## How to run

```powershell
# Desktop (Skia)
dotnet run --project Pens/Pens.csproj -f net10.0-desktop

# Android
dotnet build Pens/Pens.csproj -f net10.0-android
```

Ships running on mock data. To wire your own Supabase project, follow `SUPABASE.md`.

## Target platforms

| TFM                | Verified | Notes                                              |
|--------------------|:--------:|----------------------------------------------------|
| `net10.0-desktop`  | yes      | Showcase target.                                   |
| `net10.0-android`  | declared | Platforms/Android populated, not runtime-verified. |

## Architecture at a glance

- **MVVM** with `CommunityToolkit.Mvvm`.
- `IHostBuilder` + `UseToolkitNavigation` + `UseConfiguration` + `UseNavigation(RegisterRoutes)`.
- Services: `ISupabaseService` (mock/real swap), `IPlayerIdentityService` (singleton).
- ViewModels: registered as transient; page VMs (Shell/Main/PlayerPicker) wired through `ViewMap`.

## Companion docs

- `SUPABASE.md` — opt-in real-backend setup
- `penguins-app-full-specification.md` (solution root) — full design spec

## Known limitations

- **10 cosmetic `Uno0001` warnings** about `AutomationProperties.SetHeadingLevel` / `SetLiveSetting` being not-implemented in Uno on Skia desktop. The XAML calls are correct intent; the API is a no-op on the current target.

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, Logging, Mvvm, Configuration, HttpKiota, Serialization, Localization, Navigation, ThemeService, SkiaRenderer.
