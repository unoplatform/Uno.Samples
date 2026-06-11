# Uno CRM — Lessons Learned

A catalogue of the fixes and patterns applied while modernizing the **Uno CRM**
sample ([studio/uno-crm](.)), written so they can be reused on the other samples
in this repo. Most of these are WinUI/Uno layout gotchas that recur across samples.

Each lesson lists the **symptom**, the **cause**, the **fix**, and **where else it
applies**. Code is illustrative — adapt names to the target sample.

## Quick reference

| # | Lesson | One-line takeaway |
|---|--------|-------------------|
| 1 | Responsive layout | Use `{utu:Responsive}` + a shared `ResponsiveLayout`, not `AdaptiveTrigger`. |
| 2 | `TextBlock` + `MaxWidth` centers | A `Stretch` element capped by `MaxWidth` is centered — pin it `Left`. |
| 3 | `ComboBox` won't fill its column | Default `HorizontalAlignment` is `Left`; set `Stretch`. |
| 4 | Mixed control heights | Give a row of inputs one explicit `Height` so edges align. |
| 5 | `TextBox` placeholder centering | VCA doesn't move the watermark — use symmetric vertical `Padding`. |
| 6 | Consistent corner radius | `ComboBox`/`Button` default radius ≠ your custom `TextBox` radius; set it. |
| 7 | Don't fake alignment with margins | Use `VerticalAlignment="Center"`, not `Margin` nudges. |
| 8 | Shared chrome must match across pages | One stray padding value makes shared UI jump on navigation. |
| 9 | Fill width on large screens | Replace fixed-width scrolling columns with equal `*` `Grid` columns. |
| 10 | Uniform cards | Reserve a fixed line-count for wrapping text; pin badges in their own column. |
| 11 | Reusable controls + `x:Bind` | Fold duplicated markup into a `UserControl`; bind `Mode=OneWay`. |
| 12 | Kill the gray `ListViewItem` rectangle | Transparent container states or a bare template. |
| 13 | Button pressed state | Replace the gray rectangle with a rounded theme-aware highlight. |
| 14 | Auto-hide a bar on scroll | Find the host `ScrollViewer`, animate a `TranslateTransform`. |
| 15 | LiveCharts donut & legend | `MaxRadialColumnWidth` over fixed `InnerRadius`; bottom legend in narrow columns. |
| 16 | Theme brushes in code-behind | They live in `ThemeDictionaries`; resolve with a fallback or keep in XAML. |
| 17 | App icon & splash | One icon for **all** platforms & form factors via `Uno.Resizetizer`; `ExtendedSplashScreen` is white on iOS. |
| 18 | Verifying samples | iOS sim for screenshots; WASM + Playwright for fast width checks. |

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
inputs. The CRM has [`Controls/PipelineCard`](UnoCRM/Controls/PipelineCard.xaml) and
[`Controls/BottomNavBar`](UnoCRM/Controls/BottomNavBar.xaml).

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
with no per-page wiring — see [`Controls/BottomNavBar.xaml.cs`](UnoCRM/Controls/BottomNavBar.xaml.cs).

- On `Loaded`, walk to the parent grid and find the sibling page `ScrollViewer`;
  subscribe to `ViewChanged`. Unsubscribe on `Unloaded`.
- Compare `VerticalOffset` to the previous value (small dead-zone threshold ~6px):
  scroll down → hide; scroll up / near top → show.
- Animate a `TranslateTransform` with an eased `Storyboard`
  (`CubicEase`, `EnableDependentAnimation="True"`, ~280ms) for fluid motion.

**Applies to:** any sample with a persistent floating bar over scrollable content.

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
hardcoded fallback** (see [`LeadsPage.xaml.cs`](UnoCRM/LeadsPage.xaml.cs) `ResolveColor`).

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
- `.csproj` props: `UnoIconForegroundFile` + `UnoIconColor`, `UnoSplashScreenFile` +
  `UnoSplashScreenColor`; register extra images with `<UnoImage Include="…" />`.
- A logo PNG with **transparent corners** bleeds white over a dark icon background —
  darken the icon background fill to match.
- `utu:ExtendedSplashScreen` renders **white on iOS** (it doesn't auto-paint the
  native splash) — put the logo in `LoadingContent` so it shows on every platform.
- Android: call `ExtendedSplashScreen.Init(this)` in `MainActivity.OnCreate`.

**Applies to:** any single-project sample that needs branded icon/splash.

## 18. Verifying sample UI changes

- **iOS simulator** is the reliable run/screenshot path (`xcrun simctl` boot / install
  / launch / screenshot); use it for icon/splash checks.
- **WASM** is fastest for layout/width verification: `dotnet build` then
  `dotnet run --no-build -f net10.0-browserwasm`, poll `http://localhost:5000`, then
  drive with Playwright. The app renders to a `<canvas>`, so navigate with
  **coordinate clicks**, and capture at specific viewport widths (and
  `colorScheme: 'dark'`) to validate responsive breakpoints.
- **Android** builds need JDK 17; Skia desktop is windowless from a headless shell.

---

*Generated from the change history on branch `dev/jenny/update-uno-crm-sample`
(PR for unoplatform/studio.live#2416).*
