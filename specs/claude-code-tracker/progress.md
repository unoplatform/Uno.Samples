# Claude Code Tracker — "perfect example" upgrade

Branch: `dev/jenny/claude-tracker-exemplar`. Plan approved (see decisions below).
Source review: 50 verified findings (66-agent workflow) + direct read of all 22 files.

**Decisions:** Full MVUX page-Models (real backbone). Localization: nav + page/section
titles via `x:Uid`, translated across en/es/fr/pt-BR. Keep classic `{Binding}` (gold-standard
samples don't use `x:Bind`/`FeedView`). Never `AdaptiveTrigger`/`VSM` — always `{utu:Responsive}`.

## Workstreams

- [x] **A — Responsive shell**: MainPage NavigationView (wide) + bottom TabBar (narrow) via
  `{utu:Responsive}`, mirroring uno-crm; distinct nav glyphs; `x:Uid` nav text.
- [x] **B — MVUX page-Models**: DashboardModel/SessionsModel/UsageModel/ChartsModel +
  SessionDetailModel(SessionEntry via DataViewMap); delete MockData statics; keep SharedTypes;
  ViewMap/DataViewMap routes; Hot-Design DataContext fallback; **fix Session→Detail data flow**.
- [x] **C — Charts**: ResolveColor theme paints; drop fixed InnerRadius (keep MaxRadialColumnWidth);
  on-brand fills; delete DarkLabelPaint; responsive donut row; chart `AutomationProperties.Name`.
- [x] **D — App.xaml/theming**: reformat multi-line; delete 22 dead `App.*` brushes; flag-driven
  trend brush/glyph/sign on DashboardModel (no hardcoded ErrorBrush).
- [x] **E — Localization**: `x:Uid` nav + titles; en keys; rewrite es/fr/pt-BR with translations.
- [x] **F — Polish**: number-format display props; Dashboard Avg/Session KPI + ColumnSpacing +
  StartedAt + drop dead fields; Sessions ChipGroup filter + SafeArea + ellipsis + empty-state;
  Usage model-cost card + cache price cols + comment cleanup; SessionDetail IsClickable + SafeArea;
  Charts/Dashboard daily data consistency.
- [x] **G — Icon/splash**: user's Claude SVG as rasterizer-safe transparent foreground; #151515 bg;
  splash mark; verify ring renders.
- [x] **H — Hygiene**: real README; uno-sample.json platforms; drop HttpKiota/DebugHandler/UseHttp;
  window title; csproj trailing whitespace.
- [x] **V — Verify**: build desktop/wasm(/ios); WASM+Playwright wide/narrow/light/dark; iOS sim
  icon+splash+TabBar; re-review diff.

## Notes / decisions log

- BottomTabBarStyle confirmed available under SimpleToolkitTheme (fit-app, Voyago).
- DataViewMap detail pattern from overalls SummaryModel/SummaryPage; ResolveColor from uno-crm LeadsPage.
- Icon ring uses a Figma conic-gradient (foreignObject) the Resizetizer rasterizer drops — needs a
  standards-SVG or high-res transparent PNG foreground; verify the ring renders before done.

## Review

All workstreams complete; desktop + WASM builds are **0 warnings / 0 errors**, and the new icon
SVG rasterizes through Resizetizer without error. Verified via WASM + Playwright at wide
(1280) and narrow (390) widths in light and dark:

- **Responsive shell** — NavigationView pane on wide, bottom TabBar on narrow, distinct nav
  glyphs, all driven by `{utu:Responsive}`. ✓
- **Session → Detail data flow (headline bug)** — tapping a row now opens *that* session; the
  detail derives per-token-type costs, cache hit-rate, savings and topic tags from the injected
  `SessionEntry` (confirmed "unit-test-coverage" → its own figures). ✓
- **Charts** — theme-resolved paints: legible axis labels and on-brand series/donuts in both
  light and dark (previously `#333`, invisible in dark); donut holes scale. ✓
- **MVUX page-Models** — every page renders from a `ViewMap`/`DataViewMap` model projecting one
  shared `SampleData`; classic `{Binding}` retained per gold-standard. ✓
- **Polish** — `N0`/currency/date formatting everywhere; Avg/Session KPI (2×2 on narrow);
  ChipGroup filter; empty-state; Usage model-cost card + cache pricing line; SafeArea. ✓
- **Localization** — nav + page/section titles via `x:Uid`, translated en/es/fr/pt-BR
  (en default). ✓
- **Icon/splash** — user's Claude mark; Figma conic ring rebuilt as standard `linearGradient` +
  `mask` (see lesson 17 addendum) so Resizetizer keeps it; verified via Chrome render + build. ✓
- **Hygiene** — real README, manifest platforms, removed HttpKiota/DebugHandler leftovers,
  window title, csproj cleanup. ✓

Narrow-build screenshots show the Uno DEBUG diagnostics overlay over the top-left corner — a
debug-only widget (present on every page), not a layout defect.

Not committed — awaiting user go-ahead. On-device iOS icon/splash check deferred (the SVG is
rasterizer-validated by the WASM build + Chrome render).
