# BrewHouse — apply `.claude/lessons.md` (full modernization)

Review the coffee-shop sample (`studio/coffee-shop`, project **BrewHouse**) against the 21
lessons in [`.claude/lessons.md`](../../.claude/lessons.md) and adjust. Scope chosen by the
user: **full modernization**.

## Baseline (already good)
- Uses the Uno.Extensions Navigation shell (lessons 20). ✅
- Each page sets `DataContext` on an inner `Grid`, not the `Page`, so it sidesteps the
  lesson-21 "framework nulls the page DataContext" trap. ✅

## Findings → tasks

- [ ] **0. Baseline build** (desktop) compiles before changes.
- [ ] **1. App.xaml resources** — add `{utu:Responsive}` `DefaultResponsiveLayout`
      (Narrow=0, Wide=900, lesson 1); a named brown palette (`Brew*` brushes); and
      `PrimaryButtonStyle`/`SecondaryButtonStyle` with a custom template that swaps a
      rounded, theme-aware highlight on PointerOver/Pressed (lesson 13).
- [ ] **2. csproj icon/splash** — wire `UnoIconForegroundFile` / `UnoIconBackgroundColor`
      / `UnoSplashScreenFile` / `UnoSplashScreenColor` (lesson 17). Assets already exist.
- [ ] **3. MainPage shell** — responsive `NavigationView` (Wide pane) + bottom `TabBar`
      (Narrow) driving the same content region via `{utu:Responsive}` (lessons 20/21),
      copied from the uno-crm reference shell.
- [ ] **4. ProductCard UserControl** (lesson 11) — fold the duplicated horizontal product
      row (HomePage Featured + MenuPage list) into one control with `Product` + `AddCommand`
      DPs and `x:Bind … Mode=OneWay`. This also **fixes broken command bindings**
      (`RelativeSource Self`/`TemplatedParent` inside data templates resolve to nothing).
- [ ] **5. MenuPage** — lesson 12 (bare-`ContentPresenter` `ItemContainerStyle` so the
      gray hover/press rectangle stops painting over the rounded cards), fix the filter +
      add command bindings, use `ProductCard`, cap content width on wide.
- [ ] **6. Home / Cart / Orders** — cap content in a centered max-width column on wide
      (lesson 9 / desktop target), apply the shared button styles, fix HomePage's broken
      hero/specials command bindings.
- [ ] **7. Verify** — build (desktop + wasm); run + screenshot narrow & wide if possible
      (lesson 18). Update `.claude/lessons.md` with any new lesson; write review below.

## Decisions / trade-offs
- **Wide layout = centered max-width column**, not a multi-column grid. A constant
  `MaxWidth` + stretch gives full-width on phones and a centered readable column on
  desktop with zero magic numbers and minimal risk. Multi-column (lesson 9's stricter
  reading) is noted as a possible future enhancement.

## Review

All seven tasks done. Files touched:
- `App.xaml` — `DefaultResponsiveLayout`, `Brew*` palette brushes, `PrimaryButtonStyle` /
  `SecondaryButtonStyle` (custom templates, theme-aware hover/press, lesson 13).
- `BrewHouse.csproj` — `UnoIcon*` / `UnoSplashScreen*` wiring (lesson 17).
- `Assets/Icons/icon.svg`, `icon_foreground.svg`, `Assets/Splash/splash_screen.svg` —
  replaced the **default Uno placeholder logo** with a branded coffee-cup mark.
- `Presentation/MainPage.xaml` — responsive `NavigationView` (wide) + `TabBar` (narrow)
  shell via `{utu:Responsive}` (lessons 1, 20, 21).
- `Presentation/Controls/ProductCard.xaml(.cs)` — new reusable row (lesson 11).
- `MenuPage.xaml` — bare `ItemContainerStyle` (lesson 12), `ProductCard`, fixed bindings,
  capped width.
- `HomePage.xaml` / `CartPage.xaml` / `OrdersPage.xaml` — capped width (lesson 23), shared
  button styles, fixed item-template command bindings.

### Bugs found & fixed along the way
Several `Command` bindings used `RelativeSource Self` / `TemplatedParent` inside data
templates and **silently never fired** (Menu add + category filter, Home hero/specials add,
Cart +/-). All now use `{utu:ItemsControlBinding Path=DataContext.<Cmd>}` — see new
**lesson 22**. `RelativeSource FindAncestor` is *not* valid in WinUI/Uno (build error); that
detour and the `x:Bind` static-helper `CS0176` are also captured in lesson 22.

### Verification (lesson 18)
- `dotnet build` green on `net10.0-desktop` and `net10.0-browserwasm`.
- Ran WASM (localhost:5000) + Playwright:
  - **Wide (1400px):** left `NavigationView` pane; content centred in a max-width column.
  - **Narrow (390px):** bottom `TabBar`; full-width content.
  - **End-to-end:** added items from the Menu `ListView` → Cart showed "2 items",
    Subtotal $10.25 / Tax $0.82 / **Total $11.07**; cart "+" → qty 3, **Total $17.82**.
    Proves `{utu:ItemsControlBinding}` commands fire from virtualized item templates.
  - Console clean (no binding errors/exceptions).

### Known environmental issue (NOT changed)
Fresh restore fails `NU1605`: the private SDK `6.6.0-alc.150` implicitly pulls
`Uno.Toolkit.WinUI 8.5.0-dev.29`, which demands `Uno.WinUI dev.624` > the pinned
`alc.150` (a feed/SDK inconsistency, independent of these edits — the cached baseline
assets already had `dev.29`+`alc.150`). Builds/runs here used `-p:NoWarn=NU1605` only;
**no package files were changed.** Flag to the Uno SDK team if it persists on CI.

### Deferred (out of scope)
- `GoToCartCommand` / `GoToMenuCommand` / `OrderNowCommand` on `HomePageData` are still
  no-ops (navigation from mock VMs needs `INavigator`); the nav chrome covers movement.
- Stricter lesson 9 multi-column grid on wide (centred column chosen for low risk).
