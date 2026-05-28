# 📈 Meridian — *Portfolio & market dashboard*

> A portfolio / market dashboard: watchlist, holdings, news, and stock detail — running on live or mock market data.

<!-- 📸 Add a screenshot: drop it in screenshots/Meridian/ and point the src below at it -->
<!-- <img src="../screenshots/Meridian/<file>.png" alt="Meridian dashboard" width="640" /> -->
> 📸 *Screenshot coming.*

## What you get
The **exemplary MVUX model** sample — computed feeds, command generation, and a strong three-typeface identity — paired with hand-built ambient visuals.

## Highlights
- **Exemplary MVUX models** — `DashboardModel` / `StockDetailModel` are `partial record`s with `IFeed` / `IListFeed` / `IState`, computed feeds via `.SelectAsync`, and `ValueTask` methods that auto-generate commands.
- **Three-typeface identity** — Instrument Serif for hero numbers, IBM Plex Mono for prices, Outfit for UI, wired through `FontOverrideDictionary`.
- **Hand-built live visuals** — ambient session-warmth orb, braille-glyph activity bars, gauge-needle settle animations, chart breathing glow.
- **Live-or-mock data** — `IMarketDataService` has a no-network mock (default) and a Finnhub free-tier implementation; switch via DI.
- **Companion `Liveline` chart library** referenced as a `ProjectReference` — chart rendering stays a separate library.

## Stack & platforms
**MVUX** + region nav (typed `DataViewMap` for stock detail) · Uno.Sdk 6.5.36 · `net10.0-desktop` ✅, `net10.0-browserwasm` (declared) · Light theme only (v1)

## Run it
```powershell
dotnet run --project Meridian/Meridian.csproj -f net10.0-desktop
```
