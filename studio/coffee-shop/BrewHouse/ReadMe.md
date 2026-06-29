# BrewHouse

A cross-platform **coffee-shop ordering** sample built with [Uno Platform](https://platform.uno/).
Browse the menu, filter by category, build a cart with live quantity controls, place an order, and
track it in order history — all from a single project that runs on iOS, Android, WebAssembly and
desktop (Windows / macOS / Linux).

## Features

- **Home** — a hero carousel with an *Order Now* CTA, Today's Specials, category chips, featured
  products, and a cart-summary strip / rich empty-cart state.
- **Menu** — a working category filter and an add-to-cart list.
- **Cart** — line items, +/- quantity controls, subtotal / tax / total, and *Place Order*.
- **Orders** — seeded order history with status, updated live when you place an order.
- A warm **light and dark** theme from one palette.

## Architecture

- **Single project** (`BrewHouse/BrewHouse.csproj`) targeting
  `net10.0-android;net10.0-ios;net10.0-browserwasm;net10.0-desktop`, built on the Uno **Simple** theme.
- **Navigation shell** — `Presentation/Shell.xaml` is the navigation root; `Presentation/MainPage.xaml`
  is a responsive bottom `utu:TabBar` shell over a region-based content area that
  [Uno.Extensions Navigation](https://aka.platform.uno/navigation) injects pages into. The phone-first
  pages cap to a centred column on wide desktop/WASM windows.
- **Pages & view-models** — each page (`HomePage` / `MenuPage` / `CartPage` / `OrdersPage`) binds a page
  view-model registered via `ViewMap` in `App.xaml.cs` and constructed by DI with an `INavigator` for
  cross-tab navigation. Item-template buttons reach the page VM through the Toolkit
  `{utu:ItemsControlBinding}` markup extension.
- **Shared state** — `Presentation/MockData/AppState.cs` (a DI singleton) owns the cart and order
  history, so the screens stay in sync. `Models.cs` holds the entities; `RelayCommand.cs` the command
  helper.
- **Theming** — `ThemeColors.xaml` defines the cream/espresso (light) and dark-roast (dark) palettes as
  `ThemeDictionaries`, merged onto the Simple theme via `SimpleToolkitTheme ColorOverrideSource` and
  referenced with `{ThemeResource}` throughout. The app icon and splash are generated from the SVGs in
  `Assets/` by [Uno.Resizetizer](https://aka.platform.uno/resizetizer).

## Running

```bash
# Desktop (Skia — Windows / macOS / Linux)
dotnet run --project BrewHouse/BrewHouse.csproj -f net10.0-desktop

# WebAssembly
dotnet run --project BrewHouse/BrewHouse.csproj -f net10.0-browserwasm

# iOS (simulator)
dotnet build BrewHouse/BrewHouse.csproj -f net10.0-ios -p:RuntimeIdentifier=iossimulator-arm64

# Android (requires JDK 17)
dotnet build BrewHouse/BrewHouse.csproj -f net10.0-android
```

For more on the Uno.Sdk and upgrading Uno Platform packages, see
https://aka.platform.uno/using-uno-sdk.
