# Uno CRM — Lessons Learned

A catalogue of the fixes and patterns applied while modernizing the **Uno CRM**
sample ([studio/uno-crm](../studio/uno-crm)), written so they can be reused on the other
samples in this repo. Most of these are WinUI/Uno layout gotchas that recur across samples.

Each lesson lists the **symptom**, the **cause**, the **fix**, and **where else it
applies**. Code is illustrative — adapt names to the target sample.

## Quick reference

| # | Lesson | One-line takeaway |
|---|--------|-------------------|
| 1 | Responsive layout | Use `{utu:Responsive}` + a shared `ResponsiveLayout`, not `AdaptiveTrigger`. |
| 2 | `TextBlock` + `MaxWidth` centers | A `Stretch` element capped by `MaxWidth` is centered — pin it `Left`. |
| 3 | `ComboBox` won't fill its column | Default `HorizontalAlignment` is `Left`; set `Stretch`. |
| 4 | Mixed control heights | Give a row of inputs one explicit `Height` so edges align. |
| 4b | Stacked rows share column ratios | Two multi-column rows must use the *same* `*` ratios to line up. |
| 5 | `TextBox` placeholder centering | VCA doesn't move the watermark — use symmetric vertical `Padding`. |
| 6 | Consistent corner radius | `ComboBox`/`Button` default radius ≠ your custom `TextBox` radius; set it. |
| 6b | Re-skin built-in controls | Override `ComboBox*` (etc.) lightweight-styling brushes in `ThemeDictionaries`, per theme. |
| 7 | Don't fake alignment with margins | Use `VerticalAlignment="Center"`, not `Margin` nudges. |
| 8 | Shared chrome must match across pages | One stray padding value makes shared UI jump on navigation. |
| 9 | Fill width on large screens | Replace fixed-width scrolling columns with equal `*` `Grid` columns. |
| 10 | Uniform cards | Reserve a fixed line-count for wrapping text; pin badges in their own column. |
| 11 | Reusable controls + `x:Bind` | Fold duplicated markup into a `UserControl`; bind `Mode=OneWay`. |
| 12 | Kill the gray `ListViewItem` rectangle | Transparent container states or a bare template. |
| 13 | Button pressed state | Replace the gray rectangle with a rounded theme-aware highlight. |
| 14 | Auto-hide a bar on scroll | Find the host `ScrollViewer`, animate a `TranslateTransform`. |
| 14b | No nested scroll inside a page | A `ListView`/inner `ScrollViewer` breaks scroll-driven UI; use `ItemsControl`. |
| 14c | Tab nav back stack | Clear `Frame.BackStack` after a tab navigate; keep the nav control decoupled. |
| 15 | LiveCharts donut & legend | `MaxRadialColumnWidth` over fixed `InnerRadius`; bottom legend in narrow columns. |
| 16 | Theme brushes in code-behind | They live in `ThemeDictionaries`; resolve with a fallback or keep in XAML. |
| 17 | App icon & splash | One icon for **all** platforms & form factors via `Uno.Resizetizer`; `ExtendedSplashScreen` is white on iOS. |
| 18 | Verifying samples | iOS sim for screenshots; WASM + Playwright for fast width checks. |
| 19 | Static field init order | A static prop initializer that reads a field declared lower crashes at first access; declare consumed fields first. |
| 20 | Shell nav via Uno.Extensions | Nav chrome once in a shell page; pages become content-only and the framework injects them into a region. |
| 21 | Extensions nav gotchas | View-only pages lose their constructor `DataContext`; adaptive default `VisualState` isn't applied to injected pages. |
| 22 | Commands from item templates | `RelativeSource FindAncestor`/`Self` don't reach the page VM in WinUI/Uno; use `{utu:ItemsControlBinding Path=DataContext.Cmd}`. |
| 23 | Cap content width on desktop | A constant `MaxWidth` + `HorizontalAlignment="Stretch"` = full-width on phones, centred column on desktop, no magic numbers. |
| 24 | Emoji render as tofu on Skia | The Skia renderer's bundled fonts have no color-emoji; use `FontIcon` (Segoe Fluent Icons) glyphs, verified per glyph. |
| 25 | Distinct light/dark from one palette | Put **semantic** brushes in `ThemeDictionaries` (referenced via `{ThemeResource}`); split any brush used as both bg and fg. |

---

## 1. Responsive layout: `{utu:Responsive}` over `AdaptiveTrigger`

**Pattern.** Switch desktop/mobile layouts with the Toolkit `ResponsiveExtension`
bound to a single app-wide breakpoint, instead of per-page
`AdaptiveTrigger`/`VisualStateManager` blocks.

```xml
<!-- App.xaml -->
<utu:ResponsiveLayout x:Key="DefaultResponsiveLayout" Narrow="0" Wide="900" />

<!-- page -->
<Grid x:Name="DesktopRoot" Visibility="{utu:Responsive Narrow=Collapsed, Wide=Visible}" />
<Grid x:Name="MobileRoot"  Visibility="{utu:Responsive Narrow=Visible, Wide=Collapsed}"
      utu:SafeArea.Insets="Top,Bottom" />
```

- Requires the `Toolkit` UnoFeature in the `.csproj`.
- Honor the safe area with `utu:SafeArea.Insets` — **do not** hardcode a fake
  status bar.
- The switch toggles `Visibility` but keeps **both** layout subtrees alive (and their
  controls live). Work driven off shared state (map refreshes, list rebuilds) runs for the
  hidden layout too; mirroring selection/search between the two layouts re-raises the
  counterpart's change event — guard re-entrancy with a flag, and prefer doing per-instance
  work only for the visible layout.

**Applies to:** any sample with hand-rolled adaptive triggers or a faked status bar.

## 2. A `TextBlock` with `MaxWidth` gets centered, not left-aligned

**Symptom.** A subtitle/caption floats toward the middle of its column instead of
sitting under its title.

**Cause.** When a `Stretch`-aligned element (the default) is constrained by
`MaxWidth`, WinUI centers it in the leftover space.

**Fix.** Pin the alignment explicitly.

```xml
<TextBlock Text="…" MaxWidth="620" HorizontalAlignment="Left" />
```

**Applies to:** any capped-width text/Image/Border that should hug an edge.

## 3. `ComboBox` (and most controls) won't fill a `*` grid column

**Symptom.** Dropdowns float at the left of their columns, leaving uneven gaps and
breaking the right edge of a toolbar/filter row.

**Cause.** `ComboBox` default `HorizontalAlignment` is `Left`, not `Stretch`.

**Fix.** `HorizontalAlignment="Stretch"` on each control in a proportional grid.

**Applies to:** any filter/toolbar row built with a `Grid` of `*` columns.

## 4. Align a row of inputs to a common height

**Symptom.** A search `TextBox` and neighboring `ComboBox`es have different heights,
so their top/bottom edges don't line up.

**Cause.** A `TextBox`'s height is padding-driven (~42px) while a default `ComboBox`
is ~32px.

**Fix.** Give every control in the row the same explicit `Height` and center content.

```xml
<TextBox  Height="44" />
<ComboBox Height="44" VerticalContentAlignment="Center" />
```

**Applies to:** any sample mixing `TextBox` / `ComboBox` / `Button` on one line.

## 4b. Stacked multi-column rows must share the same column ratios

**Symptom.** Panels in a dashboard's second row don't line up with the row above —
the vertical divider and panel edges shift between rows.

**Cause.** Each row's `Grid` declares its own column proportions, e.g. row 2 is
`1.35*/1*` while row 3 is `1.15*/1*`, so the split lands at a different x per row.

**Fix.** Use identical `ColumnDefinitions` ratios on every row that should align into
columns (or share one definition). Same applies to `ColumnSpacing`.

**Applies to:** dashboards/report pages that stack several multi-column panel rows.

## 5. `TextBox` placeholder ("watermark") centering

**Symptom.** With a fixed height, the placeholder floats to the top of the box.

**Cause.** Uno's `TextBox` template applies `VerticalContentAlignment` to the
*editable text* but **not** to the placeholder presenter. So zeroing the vertical
padding and relying on `VerticalContentAlignment="Center"` mis-centers the watermark.

**Fix.** Use **symmetric vertical padding** — it centers both the placeholder and the
typed text reliably.

```xml
<!-- Height="44" with balanced top/bottom padding instead of VCA -->
<TextBox Height="44" Padding="36,11,14,11" />
```

**Applies to:** any search/input box that sets a fixed height.

## 6. Give all input controls the same corner radius

**Symptom.** Search boxes look more rounded than dropdowns next to them.

**Cause.** A custom `TextBox` style sets `CornerRadius="12"`, but `ComboBox`/`Button`
fall back to the framework default `ControlCornerRadius` (~4px).

**Fix.** Set `CornerRadius` on each input (it template-binds straight through), or
override `ControlCornerRadius` once in app resources to round everything globally.

```xml
<ComboBox CornerRadius="12" />
```

**Applies to:** any sample with a mix of styled and default input controls.

## 6b. Re-skin a built-in control with lightweight styling (not a custom template)

**Goal.** Make a framework control (e.g. `ComboBox`) match your palette — the closed box,
chevron, drop-down popup and item hover/selected states — without rewriting its
`ControlTemplate`.

**Fix.** Override the control's lightweight-styling brush resources inside the app
`ThemeDictionaries` (one set per theme), mapped to your palette. For `ComboBox`:
`ComboBoxBackground`/`…PointerOver`/`…Pressed`/`…Focused`, `ComboBoxBorderBrush…`,
`ComboBoxForeground…`, `ComboBoxDropDownGlyphForeground…` (the chevron),
`ComboBoxDropDownBackground`/`…BorderBrush`, and `ComboBoxItem…Selected`/`…PointerOver`.
Define them in **both** the Light and Default dictionaries so they swap with the theme.

> A misspelled key is a silent no-op (that state keeps the framework default), not a build
> error — so verify each state (rest, hover, open, selected) visually.

Related `ComboBox` improvements:
- **Open the drop-down *below* the box** (WinUI overlays the selected item on the control).
  Uno-only: `Uno.UI.FeatureConfiguration.ComboBox.DefaultDropDownPreferredPlacement =
  DropDownPlacement.Below;` in `App` startup (applies to all), or per control
  `not_win:ComboBox.DropDownPreferredPlacement="Below"` (`xmlns:not_win="using:Uno.UI.Xaml.Controls"`).
- **Bind the items, don't hardcode `ComboBoxItem`s** — derive options from the data
  (`_items.Select(x => x.Field).Distinct()`, with an "All …" entry first) and bind
  `ItemsSource`; set `FontSize` so the drop-down content is legible. With classic
  `{Binding}`, populate the collection **before** setting `DataContext` (or raise
  `PropertyChanged`) — otherwise the binding captures an empty value and the box looks empty.
- **Set the initial selection in code-behind**, not via a parse-time `SelectedIndex="0"` in
  XAML. With a late-bound `ItemsSource` the index is coerced to `-1` on the then-empty combo
  and isn't re-applied when items arrive (varies per target) — the box renders blank. Assign
  `SelectedIndex`/`SelectedItem` after binding (e.g. in `Loaded`); guard the resulting
  `SelectionChanged` so it doesn't churn during init.

**Applies to:** any sample using default `ComboBox`/`CheckBox`/`Slider`/etc. that should be on-brand.

## 7. Don't fake alignment with `Margin` nudges

**Symptom.** A status dot / label / count badge sit slightly off a common line.

**Cause.** A `Margin="0,5,0,0"` hack used to approximate baseline alignment.

**Fix.** Put the items in a horizontal `StackPanel` and `VerticalAlignment="Center"`
all of them.

**Applies to:** any "dot + label + badge" header row.

## 8. Shared chrome must use identical values on every page

**Symptom.** Navigating to/from one page makes a shared element (sidebar logo, nav,
header) jump vertically.

**Cause.** One page used a different padding (`20,48,20,24`) than its siblings
(`20,24,20,24`).

**Fix.** Audit the shared layout values across all pages and converge them. Better:
move the shared chrome into one control (see #11) so there's a single source of truth.

**Applies to:** multi-page samples that repeat a sidebar/header per page.

## 9. Use the available width on large screens

**Symptom.** A board/grid bunches on the left with a big empty gap on wide displays.

**Cause.** A horizontally-scrolling `StackPanel` of fixed-width columns.

**Fix.** Replace it with a `Grid` of equal star columns so columns and their content
stretch to fill.

```xml
<Grid ColumnSpacing="14">
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*" /> <!-- ×N -->
  </Grid.ColumnDefinitions>
</Grid>
```

**Applies to:** kanban boards, dashboards, any fixed-column layout that should be fluid.

## 10. Make repeated cards/tiles a uniform size

**Symptom.** Cards in a column have ragged heights; badges overlap long titles.

**Cause.** Card height tracks how the title wraps; the badge shares a single grid
cell with the title and is drawn on top of it.

**Fix.** Reserve a fixed line-count for the variable element, and give the badge its
own column.

```xml
<Grid>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*" />     <!-- title -->
    <ColumnDefinition Width="Auto" />  <!-- indicator -->
  </Grid.ColumnDefinitions>
  <!-- two-line reserve keeps every card the same height -->
  <TextBlock Grid.Column="0" Text="{x:Bind Company}" TextWrapping="Wrap" MaxLines="2" Height="46" VerticalAlignment="Top" />
  <Ellipse  Grid.Column="1" VerticalAlignment="Top" />
</Grid>
```

**Applies to:** any list/board of content cards with variable-length text.

## 11. Fold duplicated markup into a `UserControl` (and bind `Mode=OneWay`)

**Pattern.** Repeated card/nav markup → a `UserControl` with `DependencyProperty`
inputs. The CRM has [`Controls/PipelineCard`](../studio/uno-crm/UnoCRM/Controls/PipelineCard.xaml) and
[`Controls/BottomNavBar`](../studio/uno-crm/UnoCRM/Controls/BottomNavBar.xaml).

**Gotchas:**
- Use **`{x:Bind …, Mode=OneWay}`** in the control. The default `OneTime` evaluates
  during `InitializeComponent()` — *before* the parent sets the DP — so it captures
  defaults. `OneWay` (and `{x:Bind Fn(Prop), Mode=OneWay}` function bindings)
  re-evaluate when the parent assigns the value.
- `{ThemeResource …}` passed into a DP at the *call site* resolves correctly (XAML),
  sidestepping the code-behind theme-resource problem in #16.
- Type names: the **`FontWeight` struct is `Windows.UI.Text.FontWeight`**; the
  **`FontWeights` statics are `Microsoft.UI.Text.FontWeights`**. Mixing them up is a
  `CS0246`.
- x:Bind's generated code lives in the same partial class, so its target methods/properties
  may be **`private`**. x:Bind also converts `bool`→`Visibility` implicitly
  (`Visibility="{x:Bind SomeBool}"` needs no converter) — though the *inverse* still needs a
  helper/negated property.

**Applies to:** any sample repeating a card/row/nav many times.

## 12. Remove the gray `ListViewItem` hover/press rectangle

**Symptom.** Hovering a list row shows a square, container-sized gray rectangle that
overhangs the rounded card and fills the item's bottom-margin gap.

**Cause.** The default WinUI `ListViewItem` chrome paints a per-state background.

**Fix (pick one):**
- For `SelectionMode="None"` touch lists: an `ItemContainerStyle` whose template is a
  bare `ContentPresenter` (no per-state background at all).
- To keep a hover affordance: a custom template with transparent `CommonStates` plus
  your own overlay (e.g. an accent "hover ring" `Border` that is
  `IsHitTestVisible="False"` and matches the card's corner radius/margin).
- Always zero the container `Padding`/`Margin`/`MinHeight` so rows sit flush.

> Note: XAML resource/style/template edits **don't reliably hot-reload** — do a clean
> rebuild before judging the result.

**Applies to:** any sample using `ListView` to render custom rounded cards.

## 13. Replace the default Button pressed state

**Symptom.** Nav/sidebar buttons flash a gray rectangle on press/hover.

**Fix.** A custom `ControlTemplate` that swaps a rounded, theme-aware highlight
(e.g. `DashboardControlBrush`) in the `PointerOver`/`Pressed` `VisualState`s, with
`CornerRadius` matching the item.

**Applies to:** any sample with custom-styled buttons (nav rails, toolbars, chips).

## 14. Auto-hide a floating bar on scroll (self-wiring)

**Pattern.** A bottom/top bar that slides away on scroll-down and back on scroll-up,
with no per-page wiring — see [`Controls/BottomNavBar.xaml.cs`](../studio/uno-crm/UnoCRM/Controls/BottomNavBar.xaml.cs).

- On `Loaded`, walk to the parent grid and find the sibling page `ScrollViewer`;
  subscribe to `ViewChanged`. Unsubscribe on `Unloaded`.
- Compare `VerticalOffset` to the previous value (small dead-zone threshold ~6px):
  scroll down → hide; scroll up / near top → show.
- Animate a `TranslateTransform` with an eased `Storyboard`
  (`CubicEase`, `EnableDependentAnimation="True"`, ~280ms) for fluid motion.

**Applies to:** any sample with a persistent floating bar over scrollable content.

## 14b. Don't nest a scrolling list inside a scrolling page

**Symptom.** A scroll-driven behavior (e.g. the auto-hide bar in #14) doesn't react
when you scroll a list, and the page shows two independent scrollbars (double scroll).

**Cause.** A `ListView` (or any inner `ScrollViewer`, e.g. one created by `MaxHeight`)
inside the page's `ScrollViewer` scrolls internally, so the page's `VerticalOffset`
never changes and the outer `ViewChanged` never fires.

**Fix.** For an in-page list that should flow with the page, use an `ItemsControl`
(no built-in scroll) instead of a `ListView`, and drop the `MaxHeight`. The page
becomes the single scroll surface.

```xml
<ItemsControl ItemsSource="{Binding Items}">
  <ItemsControl.ItemTemplate><DataTemplate>…</DataTemplate></ItemsControl.ItemTemplate>
</ItemsControl>
```

> Trade-off: `ItemsControl` doesn't virtualize — fine for small/filtered lists. For very
> long lists, keep the `ListView` and instead let *only* the list scroll (don't wrap the
> page in an outer `ScrollViewer`).

**Applies to:** any mobile/page layout that wraps a list in a page-level `ScrollViewer`,
especially when combined with a scroll-driven behavior like #14.

## 14c. Tab navigation: clear the back stack, keep the nav control decoupled

**Symptom.** `Frame.Navigate` on every tab tap grows the back stack without bound — on
Android the hardware Back button then replays the whole tab history.

**Fix.** Clear the back stack after a successful tab navigation:

```csharp
if (frame.Navigate(pageType)) frame.BackStack.Clear();
```

**Also.** A genuinely reusable nav control shouldn't hard-code the app's page types (that
inverts the dependency) — raise a `TabSelected` event / expose tab metadata and let the host
navigate. For a single-app sample a direct tab→page-type switch is acceptable, but note the
trade-off; and a `string` tab id whose valid set lives in several places is a smell (an enum
or shared constants is sturdier).

**Applies to:** any tab bar / nav rail that drives a `Frame`.

## 15. LiveCharts2 donut thickness & legend placement

- **Donut collapses to a thin ring on resize** — a fixed pixel `InnerRadius` doesn't
  scale. Use `MaxRadialColumnWidth` to pin a constant band thickness (the hole scales
  with the chart instead).
- **Legend squeezes the chart in a narrow column** — `LegendPosition="Right"` steals
  width; use `LegendPosition="Bottom"` so the plot uses the full column.

**Applies to:** any sample embedding LiveCharts pie/donut charts.

## 16. Theme brushes aren't reliably resolvable in code-behind

**Cause.** Dashboard color brushes live in `App.xaml` `ThemeDictionaries`
(Light/Default), so `Application.Current.Resources[key]` from code-behind may miss
them or return the wrong theme variant.

**Fix.** Keep color in XAML via `{ThemeResource …}` where possible. If you must read
it in code (e.g. to feed a chart `SKColor`), resolve with `TryGetValue` **and a
hardcoded fallback** (see [`LeadsPage.xaml.cs`](../studio/uno-crm/UnoCRM/LeadsPage.xaml.cs) `ResolveColor`).

**Applies to:** any sample that drives non-XAML drawing (SkiaSharp, charts) from
theme colors.

## 17. App icon & splash via `Uno.Resizetizer`

- **A real app icon is mandatory on every target.** Every sample must ship a proper
  app icon that is applied across **all platforms** (iOS, Android, Windows, WebAssembly,
  macOS/Catalyst, Linux/Skia) and **all form factors** (phone, tablet, desktop) — never
  the default Uno placeholder, and never a per-platform one-off. Define it **once** with
  `Uno.Resizetizer` from a single source asset; Resizetizer generates the required sizes,
  densities and platform formats so the one icon shows everywhere (home screen, task
  switcher, taskbar, browser favicon/PWA tile, dock).
- `.csproj` props: `UnoIconForegroundFile` with `UnoIconBackgroundColor`, and
  `UnoSplashScreenFile` with `UnoSplashScreenColor`; register extra images with `<UnoImage Include="…" />`.
  > **`UnoIconColor` is a no-op** — the SDK reads **`UnoIconBackgroundColor`** (it maps to the
  > `UnoIcon` `Color`). `UnoIconColor` appears only in an erroneous doc example and silently
  > leaves the Android adaptive-icon background **transparent**. Sanity-check the generated
  > `obj/<config>/<tfm>/UnoImage.inputs` — the `IsAppIcon` row should show your `Color=#…`, not `#00000000`.
- **Prefer SVG sources** — Resizetizer rasterizes per density. The app icon is the auto-detected
  **background** `icon.svg` (`IsAppIcon`) merged with the **foreground** `UnoIconForegroundFile`,
  over `UnoIconBackgroundColor`. Keep `icon.svg` a solid background, put the logo (transparent bg)
  in the foreground so adaptive masking / safe-zone behave; the colour fills any transparent corners.
- `utu:ExtendedSplashScreen` renders **white on iOS** (it doesn't auto-paint the
  native splash) — put the logo in `LoadingContent` so it shows on every platform.
- The splash `ILoadable` must **always** clear `IsExecuting` — a fire-and-forget
  `Task.Delay().ContinueWith(d => d.TryEnqueue(…))` hangs on the splash forever if the enqueue
  returns false / the continuation faults. `await` the delay then complete defensively (fall
  back to flipping the flag inline). Don't hardcode a long fake delay in a sample — name a
  `SplashMinimumDuration` constant (demo-only) or gate on real readiness.
- Android: `MainActivity.OnCreate` only needs
  `AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this)`. **There is no
  `Uno.Toolkit.UI.ExtendedSplashScreen.Init` method** — calling it is a `CS0117` build
  break (it slipped past local desktop/iOS builds and only failed the `net10.0-android`
  CI leg). `ExtendedSplashScreen` works purely from its XAML usage; the other studio
  samples (Voyago, etc.) install the native splash and nothing more.
- **Showing the logo *inside* the UI** (brand mark next to the app name, splash content):
  draw it with **native XAML shapes** — e.g. `Rectangle`s + a `LinearGradientBrush` in a
  `Viewbox` — **not** an SVG `<Image>`. A gradient/`<defs>` SVG can fail through the runtime
  `Image` path and render as a **solid black tile**; native shapes always render and scale
  crisply. See [`Controls/AppLogo.xaml`](../studio/uno-crm/UnoCRM/Controls/AppLogo.xaml).

**Applies to:** any single-project sample that needs branded icon/splash, or that shows
its logo in-app.

## 18. Verifying sample UI changes

- **iOS simulator** is the reliable run/screenshot path (`xcrun simctl` boot / install
  / launch / screenshot); use it for icon/splash checks.
- **WASM** is fastest for layout/width verification: `dotnet build` then
  `dotnet run --no-build -f net10.0-browserwasm`, poll `http://localhost:5000`, then
  drive with Playwright. The app renders to a `<canvas>`, so navigate with
  **coordinate clicks**, and capture at specific viewport widths (and
  `colorScheme: 'dark'`) to validate responsive breakpoints.
- **Android** can be run/screenshotted from the headless shell, with two prerequisites:
  - **JDK 17.** `net10.0-android` needs JDK 17, but the machine default is JDK 11. Point
    the build at the Homebrew `openjdk@17`:
    `JAVA_HOME=/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home dotnet build -f net10.0-android -p:JavaSdkDirectory="$JAVA_HOME"`.
  - **Embed the assemblies for `adb install`.** A plain Debug APK uses Fast Deployment —
    the managed assemblies are pushed *separately* by `dotnet ... -t:Run`, so a manual
    `adb install` of the APK aborts at launch with `SIGABRT` / *"No assemblies found in
    `…/.__override__/arm64-v8a` … Assuming this is part of Fast Deployment. Exiting…"*.
    Rebuild with **`-p:EmbedAssembliesIntoApk=true`** before `adb install`. Then boot the
    AVD (`emulator -avd <name>`), poll `adb shell getprop sys.boot_completed`,
    `adb install -r <…-Signed.apk>`, launch with `adb shell am start -n <pkg>/<activity>`
    (resolve the mangled `crc…MainActivity` via `adb shell cmd package resolve-activity
    --brief -c android.intent.category.LAUNCHER <pkg>`), and capture with
    `adb exec-out screencap -p > out.png`.
- **Skia desktop** is windowless from a headless shell.

## 19. Order static fields before the members that consume them

**Symptom.** The app crashed at launch (only surfaced on iOS — Skia desktop is windowless
from the shell) with `TypeInitializationException` → `ArgumentNullException (Parameter
'source')` from `System.Linq`.

**Cause.** Static members initialize in **declaration order**. A `static` auto-property
initializer (`Dashboard { get; } = BuildDashboard();`) ran a builder that read a
`static readonly int[]` declared ~100 lines *below* it — so the array was still `null`
when the builder's `.Sum()` ran.

**Fix.** Declare any `static` field a property initializer depends on **above** that
property. (A `static` ctor that assigns everything in one place sidesteps the ordering
trap too.)

**Applies to:** any `static` data class whose properties are computed from other static
fields — easy to hit when refactoring data into a `CrmData`-style singleton.

## 20. Move shared nav chrome into a shell page (Uno.Extensions Navigation)

**Symptom.** Every page repeats the same navigation chrome (sidebar / bottom bar), so a
tweak has to be made N times and the pages drift.

**Fix.** Adopt the shell pattern the other `studio/` samples (Voyago, fit-app) use:

- Add `Hosting; Navigation;` to `<UnoFeatures>` (keep `Toolkit; SkiaRenderer;`).
- `App.OnLaunched`: `this.CreateBuilder(args).UseToolkitNavigation().Configure(host =>
  host.UseNavigation(RegisterRoutes))`, then `Host = await builder.NavigateAsync<Shell>();`.
  No MVUX needed — `UseNavigation(RegisterRoutes)` works without the reactive mappings,
  and pages can be **view-only** (`new ViewMap<DashboardPage>()`).
- A `MainPage` shell owns a `NavigationView` (wide) + `utu:TabBar` (narrow, style
  `BottomTabBarStyle`) wrapping an empty content region
  (`uen:Region.Attached="True" uen:Region.Navigator="Visibility"`); `uen:Region.Name`
  on each item maps to a nested route. Content pages then carry **no** nav chrome.
- Routes nest: `RouteMap("", View: Shell, Nested:[ RouteMap("Main", View: MainPage,
  IsDefault, Nested:[ Dashboard(IsDefault), Pipeline, Leads, Contacts ]) ])`.

**Splash + Navigation.** Keep the `ExtendedSplashScreen` in `Shell.xaml` but with **no
`Content` and no `Region.Attached`** inside it (the host isn't ready during Shell
construction). Register the Shell at root route `""` via a tiny `ShellViewModel` whose
`Start()` calls `NavigateRouteAsync(this, "Main")`; the framework injects the navigated
content into the splash and reveals it once loading completes.

**Applies to:** any multi-page sample still using manual `Frame.Navigate` + a per-page nav
control. Trade-off: this replaces bespoke nav controls with `NavigationView`/`TabBar`
(auto selection-sync) and drops scroll-driven auto-hide (lesson 14).

## 21. Two traps when pages are injected by Uno.Extensions Navigation

**DataContext is reset.** A page with a **view-only** route (no mapped view model) has its
`DataContext` reassigned (to null) by the framework during activation — *after* the
constructor — so `DataContext = CrmData.X` set in the ctor is lost and only hardcoded
text renders. Re-apply it with a guard that survives the reassignment:

```csharp
DataContextChanged += (_, _) => { if (DataContext is not DashboardData) DataContext = CrmData.Dashboard; };
```

(For a self-context page, guard `DataContext != this`.)

**Responsive switching: never use `AdaptiveTrigger` / `VisualStateManager`.** Drive
size-based layout with the Toolkit `{utu:Responsive}` extension set directly on each
property — `Visibility="{utu:Responsive Narrow=Visible, Wide=Collapsed}"`,
`PaneDisplayMode="{utu:Responsive Narrow=LeftMinimal, Wide=Auto}"`,
`IsPaneToggleButtonVisible="{utu:Responsive Narrow=False, Wide=True}"` (it handles
enums/bools, not just `Visibility`). A `VisualState` with **no** `StateTrigger` (the
implicit narrow default) is **not** reliably applied to a page the navigator *injects*
into a region — that's how the bottom `TabBar` ended up hidden on phones. `{utu:Responsive}`
has no such load-order trap and keeps breakpoints consistent with the content pages
(which already use `Narrow`/`Wide`). This is a hard rule — see lesson 1.

This is **not** unique to the CRM: running `travel-app/Voyago` on an iPhone shows the
same symptom — its `MainPage` uses a triggerless `NarrowState`, so its bottom `TabBar`
doesn't appear on phones (its `HomePage` content still renders fine). The fix there is the
same: replace the `VisualStateManager` with `{utu:Responsive}` on the affected properties.

**Applies to:** any Uno.Extensions Navigation shell with view-only pages and/or a
size-adaptive layout on a navigated page.

## 22. Invoking a page-VM command from inside an item template

**Symptom.** A button inside a `ListView`/`ItemsControl`/`FlipView` `ItemTemplate`
(e.g. "Add to Cart", a category filter, cart +/-) does nothing — the `Command` binding
silently resolves to nothing.

**Cause.** Two dead-ends that *compile* but don't work:
- `Command="{Binding DataContext.Cmd, RelativeSource={RelativeSource Mode=Self}}"` — in a
  `DataTemplate` `Self.DataContext` is the **item model**, not the page VM, so
  `…Cmd` doesn't exist (silent no-op).
- `RelativeSource={RelativeSource Mode=FindAncestor, AncestorType=…}` — **`FindAncestor`
  is WPF only.** In WinUI/Uno `RelativeSourceMode` is just `Self` / `TemplatedParent`, so
  this is a hard build error (`UXAML0001: 'FindAncestor' is not a valid 'RelativeSourceMode'`).
  `ElementName` into a *virtualized* `ListView` template is also unreliable.

**Fix.** Use the Toolkit ancestor-binding markup extension (needs the `Toolkit` UnoFeature
and `xmlns:utu`), which is purpose-built for "I'm in an item but need my page's data":

```xml
<!-- closest ItemsControl's DataContext = the page VM -->
<Button Command="{utu:ItemsControlBinding Path=DataContext.AddToCartCommand}"
        CommandParameter="{Binding}" />
```

`{utu:ItemsControlBinding Path=…}` is shorthand for
`{utu:AncestorBinding AncestorType=ItemsControl, Path=…}`. It works inside a virtualized
`ListView` template too (verified end-to-end). When the command lives on a reusable
`UserControl` (e.g. a `ProductCard` with an `AddCommand` DP), bind that DP at the call
site the same way: `AddCommand="{utu:ItemsControlBinding Path=DataContext.AddToCartCommand}"`.

**Also (x:Bind helpers):** an `x:Bind` function binding calls the method as an **instance**
member of the page/control — a `static` helper fails with `CS0176` ("cannot be accessed
with an instance reference"). Make `Money(...)`/`BadgeVisibility(...)`-style x:Bind helpers
**instance** methods.

**Applies to:** any list/grid whose item rows raise commands handled by the page/VM, and
any reusable item `UserControl` that surfaces a command DP.

## 23. Cap content width so a phone layout doesn't sprawl on desktop

**Symptom.** A sample designed phone-first (single column, fixed-width cards) targets
`desktop`/`wasm` too; on a wide window the column stretches edge-to-edge into very wide
bands, or fixed-width cards bunch at the left.

**Fix (low-risk).** Put the page content in a container with a constant `MaxWidth` and
`HorizontalAlignment="Stretch"`. Below the cap the element fills the screen (phones);
above it, WinUI centres the capped element in the leftover space (lesson 2) — a tidy
centred column on desktop. **No `{utu:Responsive}` and no magic numbers** for this part.

```xml
<ScrollViewer>
  <Grid>                                  <!-- fills the viewport -->
    <StackPanel MaxWidth="1100" HorizontalAlignment="Stretch"> … </StackPanel>
  </Grid>
</ScrollViewer>
```

Pair it with a responsive shell (lessons 20/21) for the nav chrome.

**Multi-column on wide (the fuller lesson-9 win).** For card/list content, let an
`ItemsRepeater` + `UniformGridLayout` reflow the items by available width — **1 column on
phones, 2-3 on desktop, no breakpoints**:

```xml
<ScrollViewer>
  <ItemsRepeater ItemsSource="{Binding Items}">
    <ItemsRepeater.Layout>
      <UniformGridLayout MinItemWidth="340" MinColumnSpacing="12" MinRowSpacing="12" ItemsStretch="Fill" />
    </ItemsRepeater.Layout>
    <ItemsRepeater.ItemTemplate><DataTemplate> … </DataTemplate></ItemsRepeater.ItemTemplate>
  </ItemsRepeater>
</ScrollViewer>
```

Two `ItemsRepeater` notes that bite:
- It is **not** an `ItemsControl`, so `{utu:ItemsControlBinding}` (lesson 22) finds nothing —
  reach the page VM with **`{utu:AncestorBinding AncestorType=ItemsRepeater, Path=DataContext.Cmd}`**.
- It has **no item-container chrome**, so the lesson-12 grey hover rectangle never appears —
  no `ItemContainerStyle` needed (it replaces the `ListView`+bare-template dance).

**Applies to:** any phone-first sample that also ships a desktop/WASM head.

## 24. Emoji glyphs render as "tofu" (☐ NO GLYPH) on the Skia renderer

**Symptom.** Unicode emoji used as decorative icons — `&#x2615;` ☕ for an empty cart,
`&#x1F4CB;` 📋 for "no orders", `&#x279C;` ➜ on a button, inline `☕/🥐/🍵` chips — show
as a missing-glyph box. (`FontIcon`s in the nav render fine, which is the tell.)

**Cause.** Uno's **Skia renderer** (desktop **and** WASM — both rasterize with SkiaSharp,
not the browser) ships a limited set of bundled fonts with **no colour-emoji font**, so any
emoji codepoint has no glyph. The symbol font it *does* bundle — **Segoe Fluent Icons /
Segoe MDL2 Assets**, used by `FontIcon` — is what makes the nav icons work.

**Fix.** Replace decorative emoji with `<FontIcon Glyph="&#x….;" />` using a glyph that
exists in Segoe Fluent Icons (no `FontFamily` needed — the default `SymbolThemeFontFamily`
resolves). **Verify each glyph** — a missing codepoint is just more tofu. Quick way: drop a
temporary strip of `FontIcon`s with candidate glyphs onto a visible page and screenshot.
Verified-good here: `EC32` (coffee cup), `E7C3` (page/receipt), `E73E` (checkmark),
`E81C` (history), `EA37`/`E71D` (lists).

> The symbol font has **no food icons** (croissant, ice, etc.). Where no sensible glyph
> exists (category chips: Hot/Cold/Pastries), drop the icon and keep a **text-only** chip
> rather than shipping tofu.

**Applies to:** any sample using emoji as UI icons that targets Skia (desktop/WASM/mobile).

## 25. Make light and dark visibly different from one brand palette

**Goal.** A single earthy brand that reads as a warm cream/espresso scheme in Light and a
deep dark-roast scheme in Dark — not the same colours in both.

**Do it with `ThemeDictionaries`.** Define the palette once per theme in
`ResourceDictionary.ThemeDictionaries` with keys **`Light`** and **`Default`** (`Default`
is the dark theme), same brush keys in each, and reference them with **`{ThemeResource …}`**
everywhere — `{StaticResource …}` resolves once at parse time and will **not** swap when the
theme changes (this includes brush refs inside `Style` setters and `ControlTemplate`s).

**Name brushes by role, not colour, and split conflicting roles.** A colour-named brush
(`BrewEspressoBrush`) used as *both* a background and a foreground can't theme cleanly — a
background wants to contrast the page, a foreground wants to contrast its surface, and in
dark those pull opposite ways. Use semantic keys — `BrewPage`, `BrewSurface`, `BrewText`,
`BrewAccent`, `BrewPrimary` (CTA bg) + **`BrewOnPrimary`** (CTA text), `BrewHeader`,
`BrewBorder` — and:
- For a single-accent brand, flip the **accent** across themes (espresso ↔ caramel) **and**
  flip its text (`BrewOnPrimary` white ↔ near-black) so button contrast survives both.
- Keep header/banner bars on a dedicated `BrewHeader` brush that stays dark in *both* themes,
  so their light text (and any inset white button) keep contrast without per-theme tweaks.
- Borders go translucent-black in Light, translucent-**white** in Dark (`#1A000000` ↔
  `#2EFFFFFF`) so cards stay separated on a dark page.

**Verify both themes** — drive WASM with Playwright `colorScheme: 'light'` and `'dark'`.

**Applies to:** any sample that should support a real dark theme (not just a tinted light one).

---

*Generated from the change history on branch `dev/jenny/update-uno-crm-sample`
(PR unoplatform/Uno.Samples#918).*
