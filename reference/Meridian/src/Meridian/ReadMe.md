# Meridian

A portfolio / market dashboard: watchlist, holdings, news, stock detail. Live or mock market data.

## What this shows

- **Exemplary MVUX models.** `DashboardModel` and `StockDetailModel` are `partial record`s with `IFeed<T>` / `IListFeed<T>` / `IState<T>`, computed feeds via `.SelectAsync`, public `ValueTask` methods that auto-generate commands. See `Presentation/DashboardModel.cs` and `Presentation/StockDetailModel.cs`.
- **Three-typeface identity.** Instrument Serif for hero numbers, IBM Plex Mono for prices, Outfit for UI. Wired through `MaterialToolkitTheme.FontOverrideDictionary` (`App.xaml:21–29`).
- **Hand-built live visuals** — ambient orb session warmth, braille-glyph activity bars, gauge-needle settle animations, chart breathing glow.
- **Companion `Liveline` chart library** is referenced as a `ProjectReference` (`Meridian.csproj:38`) — chart rendering is a separate library project, not tangled into the app.
- **Material overrides done as overrides.** `Themes/ColorPaletteOverride.xaml` remaps Material `PrimaryColor` / `SecondaryColor` / etc. — the theme is genuinely customized, not bypassed.
- **Type-scale tokens.** Zero numeric `FontSize` literals anywhere — every size resolves through `{StaticResource FontSize10}` … `FontSize62` keys in `Themes/FontResources.xaml`.

## How to run

```powershell
# Desktop (Skia)
dotnet run --project Meridian/Meridian.csproj -f net10.0-desktop

# WebAssembly
dotnet run --project Meridian/Meridian.csproj -f net10.0-browserwasm
```

Market data: `IMarketDataService` has two implementations — `MockMarketDataService` (default, no network) and `FinnhubMarketDataService` (Finnhub free tier). Switch via DI registration in `App.xaml.cs`.

## Target platforms

| TFM                       | Verified | Notes                                                          |
|---------------------------|:--------:|----------------------------------------------------------------|
| `net10.0-desktop`         | yes      | Showcase target.                                               |
| `net10.0-browserwasm`     | declared | Platform folder populated, not runtime-verified.               |

## Architecture at a glance

- **MVUX** for the Model layer (`partial record` + feeds).
- **Region-based navigation.** `Shell` → `DashboardPage` (default) → `StockDetailPage` (typed nav data: ticker `string`). `ViewMap` / `DataViewMap` / `RouteMap` registered in `App.xaml.cs:RegisterRoutes`. Page DataContexts are wired by the framework; no manual VM construction or `Frame.Navigate`.
- `IHostBuilder` + `UseLogging` + `ConfigureServices` — textbook Uno.Extensions hosting.
- Services: `IMarketDataService` (`Mock` + `Finnhub` impls, both singletons).
- Companion library: `Liveline` (chart rendering, separate `.csproj`).

## Known limitations

- **Conservative responsive collapse on `DashboardPage` only.** A page-scoped `utu:ResponsiveLayout` (`Narrow=0, Wide=1280`) drives `utu:Responsive` value-swaps on the page-root `MaxWidth` / `Padding`, on Row 2 (portfolio hero) so the 360-px index-cards strip restacks below the hero text on narrow, and on Row 3 (main content) so the 360-px right sidebar (search / watchlist / market pulse) restacks below the main column instead of beside. The 1.2* allocation/holdings split inside the main column intentionally still shares a row at narrow. No `AutoLayout` rewrite; no `SafeArea` (desktop + WASM only). `StockDetailPage` is still fixed-width (`MaxWidth="1400"` + hard 340-px right column) — out of scope for this pass.
- **No `FeedView`** despite 6+ feeds on the Model. `ItemsRepeater` binds directly to feed values; one empty state is visibility-toggled from code-behind.
- **Light theme only.** `RequestedTheme="Light"` on `App.xaml:6`; the Dark dictionary is a copy of Light by design ("no Dark in v1").
- **Build is clean (0 warnings).** One warning class is suppressed at the csproj level with rationale: `Uno0001` (Skia desktop no-ops `Control.CharacterSpacing` in `BrailleRevealControl` and the generated XAML for `DashboardPage`). A two-line `#pragma warning disable CS8714/CS8621` brackets the intentionally-nullable `Position` feed in `StockDetailModel.cs` (a null `Holding` is the "no position held" state).

## Stack

Uno.Sdk `6.5.36` · `UnoFeatures`: Material, Hosting, Toolkit, Logging, MVUX, Navigation, Skia, SkiaRenderer, ExtensionsCore.
