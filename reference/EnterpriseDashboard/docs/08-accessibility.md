# 08 — Accessibility

EnterpriseDashboard (codename: Observatory) is a reading instrument. Accessibility is not an afterthought layer — it is part of the reading hierarchy. Where contrast, structure, or motion would compromise readability for someone using assistive technology, the design changes, not the accessibility.

## Standards target

- **WCAG 2.1 Level AA** is the minimum bar for production.
- **WCAG 2.1 Level AAA** for primary text contrast pairings — easily achievable given the strong dark/light theme contrasts.
- **Section 508** compliance follows from WCAG AA.

## 8.1 Color and contrast

### Contrast audit

Full pairings are documented in `06-theme-and-visual-system.md` § 6.1.4. Summary:

| Pairing | Dark theme | Light theme | Required |
|---|---|---|---|
| `--ink` on `--bg` | 14.8 | 16.2 | 4.5 (AA Normal) |
| `--ink-2` on `--surface` | 8.1 | 7.4 | 4.5 |
| `--ink-3` on `--surface` | 4.6 | 4.8 | 4.5 |
| `--ink-4` on `--surface` (mono caps) | 2.9 | 3.1 | 3.0 (AA Large) |
| `--accent` on `--bg` | 7.2 | 6.5 | 4.5 |
| `--positive` on `--surface` | 6.4 | 5.2 | 4.5 |
| `--negative` on `--surface` | 5.1 | 4.9 | 4.5 |
| `--accent` on `#04101C` (range pill active) | 11.4 | 8.8 | 4.5 |

### `--ink-4` adjustment

The dark theme `--ink-4` (`#494E58`) on `--surface` tests at 2.9:1 — narrowly under the 3.0 AA Large target for the mono caps tracked labels used at 9–10px.

**Recommended fix**: shift `--ink-4` from `#494E58` to `#525762`. New ratio: 3.2:1. The visual difference is imperceptible; the compliance shift is meaningful.

The implementation should adopt this fix before production. The prototype HTML can remain at the current value during design iteration.

### Color is never the sole signal

| Signal | Color paired with |
|---|---|
| Positive delta | `--positive` + `▲` triangle |
| Negative delta | `--negative` + `▼` triangle |
| Live status | `--accent` + pulse animation + `LIVE` label or `// note` text |
| Active state | `--accent` + 1px bar indicator (rail) or filled background (toggle) |
| Error state | `--negative` + bracketed text label `[ data unavailable — retry ]` + left border |
| Stale state | warning glyph `⚠` + text label |

A screen-reader user gets the full information without color cues. A color-vision-impaired user gets the full information without parsing color.

### Theme contrast preservation

When switching themes, every contrast pairing remains AA Normal or AA Large at minimum. The light and dark token tables are designed to mirror contrast ratios within ±1.0 across themes.

## 8.2 Keyboard navigation

### Tab order

Tab order follows visual reading order top-to-bottom, left-to-right.

1. **NavRail buttons** (§ 01, § 02, § 03, settings) — in document order
2. **Header**: refresh, theme toggle
3. **Range selector**: each option in order (the active option is reachable but a no-op on Enter)
4. **Section content** (no focusable elements in default v1 — charts are not focusable until drill-down lands)
5. **Right rail** (no focusable elements in default v1)
6. **Toast** when present (focus does not auto-trap; toast is reachable via Tab if it has interactive children)

### Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Tab` / `Shift+Tab` | Move focus forward / backward |
| `Enter` / `Space` | Activate focused button or toggle |
| `1` / `2` / `3` | Switch to § 01 / § 02 / § 03 |
| `T` | Toggle theme |
| `R` | Refresh data |
| `Esc` | Close toast (if present); close drill-down panel (when drill-down lands) |
| `?` | (forward planning) Show keyboard shortcuts overlay |

Shortcut keys do not require modifiers. They fire only when no input field is focused (the dashboard has no input fields in v1, so this is forward-protection).

### Focus visibility

Every interactive element has a visible focus indicator. The `:focus-visible` style is the documented focus ring (`box-shadow: 0 0 0 2px var(--bg), 0 0 0 3px var(--accent)`).

`:focus` without `-visible` does not paint a ring — pointer users don't see a ring on click, keyboard users do see one on Tab.

### Skip link

The dashboard provides a skip link at the top of the page (visually hidden, revealed on focus):

```html
<a href="#main-content" class="skip-link">Skip to main content</a>
```

```css
.skip-link {
  position: absolute;
  top: -40px;
  left: 16px;
  background: var(--surface-3);
  color: var(--ink);
  padding: 8px 12px;
  z-index: 100;
}
.skip-link:focus { top: 16px; }
```

Activating the skip link moves focus to the main column's first heading.

## 8.3 Screen reader semantics

### Landmark structure

```html
<body>
  <a class="skip-link" href="#main">Skip to main content</a>
  <div class="shell">
    <nav class="rail" aria-label="Section navigation"> ... </nav>
    <main class="main" id="main">
      <header> ... wordmark, controls ... </header>
      <section aria-labelledby="sec-01-title"> ... </section>
      <section aria-labelledby="sec-02-title" hidden> ... </section>
      <section aria-labelledby="sec-03-title" hidden> ... </section>
    </main>
    <aside class="rail-right" aria-label="Section context"> ... </aside>
  </div>
</body>
```

- `<nav>` for the left rail.
- `<main>` for the central column.
- `<aside>` for the right rail.
- `<section>` for each view, hidden via the `hidden` attribute on non-active sections (better than CSS `display: none` for screen reader announcement of section count).
- Section title in an `<h2>` with `id` referenced by `aria-labelledby`.

### Headings hierarchy

```
h1: Observatory                      (page title)
h2: Acquisition & Revenue            (section title)
h2: Engagement & Health
h2: Cohorts
h3: Recurring Revenue                (card title)
h3: New Signups
... (one h3 per card)
h3: Revenue Breakdown                (rail section heading)
h3: Movement
h3: Signals
h3: Status
h3: Segment Leaderboard
h3: Cohort Size
```

No skipped levels. The hierarchy is consistent across themes and viewports.

### Labels and descriptions

Every interactive element has an accessible name.

- `<button aria-label="Refresh">` for icon-only buttons.
- `<button aria-pressed="true">` for toggle buttons (theme, range selector active option).
- `<button aria-current="page">` for the active rail button.

### Metric values

Each metric value renders with a visually-hidden longer-form label for screen readers.

```html
<div class="metric">
  <span class="metric-value" aria-label="Recurring revenue 87 thousand 3 hundred dollars">$87.3K</span>
  <span class="metric-unit" aria-hidden="true">mrr</span>
  <span class="metric-delta" aria-label="Up 12.4 percent">
    <span class="tri" aria-hidden="true"></span>
    12.4%
  </span>
</div>
```

The visible compact form `$87.3K` reads correctly to most screen readers but is supplemented with the longer aria-label for precision.

Alternative implementation (simpler, recommended for v1): use `<span sr-only>` siblings instead of aria-label, e.g.:

```html
<span class="metric-value">
  <span aria-hidden="true">$87.3K</span>
  <span class="sr-only">$87,300</span>
</span>
```

### Charts

Each chart is exposed as an `<img>`-equivalent with descriptive alt text via `role="img"` and an aria-label or aria-describedby.

```html
<svg
  class="chart-svg"
  role="img"
  aria-labelledby="chart-1-title chart-1-desc"
>
  <title id="chart-1-title">Recurring revenue, 12-month trend</title>
  <desc id="chart-1-desc">
    Monthly recurring revenue rose from $44K in January
    to $87.3K in December, peaking at $90K in September.
  </desc>
  ... visual chart contents ...
</svg>
```

The `<desc>` should summarize the chart's takeaway in one or two sentences. Per chart:

| Card | Desc template |
|---|---|
| Recurring Revenue | "MRR rose from $X to $Y over Z months, peaking at $A in [month]." |
| New Signups | "Monthly new signups range from X to Y, peaking at Z in [month]." |
| Cumulative ARR | "ARR grew monotonically from $X to $Y year-to-date." |
| Usage vs Retention | "Usage and retention are positively correlated, r² = 0.87." |
| Active Usage | "Peak activity occurs at 2 PM weekdays, with reduced weekend engagement." |
| Goal Attainment | "Blended goal attainment is 79%, up 4 points." |
| Session Length by Plan | "Session length grows with plan tier, from median X minutes on Free to Y on Scale." |
| Health Score | "Composite health score is 74 out of 100, in nominal range." |
| Retention by Segment | "All six segments improved retention from Q2 to Q3." |
| Per-Account MRR | "Per-account MRR clusters around the $50 median across 60 accounts." |
| Cohort Retention | "Average month-3 retention is 63% across cohorts." |

### Live region: Signals

The Signals feed is a live region.

```html
<section aria-labelledby="signals-heading" aria-live="polite">
  <h3 id="signals-heading">
    <span aria-hidden="true">§</span> Signals
    <span class="pulse" aria-hidden="true"></span>
    <span class="sr-only">live updating</span>
  </h3>
  ...
</section>
```

New signals announce on append. The `aria-live="polite"` setting ensures announcements don't interrupt the reader mid-sentence.

### Toast

```html
<div role="status" aria-live="polite" class="toast">
  [ refreshed — 14:22:03 ]
</div>
```

For error toasts, `role="alert"` instead of `role="status"` to announce immediately.

### Decorative content

All decorative SVG (icons that label a labeled control, bracket pseudo-elements, pulse dots, triangle shapes) carry `aria-hidden="true"`. The visible bracket characters `[ ]` should be wrapped in `aria-hidden` spans or applied via CSS pseudo-elements (which are not read by most screen readers).

## 8.4 Reduced motion

Documented in `05-interactions-and-motion.md`. Summary:

- Page transitions: instant.
- Theme cross-fade: instant.
- Refresh spin: replaced by a 200ms opacity flash.
- Live pulse: static dot, no expansion ring.
- Skeleton breath: static at 0.5 opacity.
- Hover transitions: instant.

Implementation:

```css
@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}
```

## 8.5 Reduced transparency

The dashboard uses no transparency on functional elements. The only transparent surface is the optional `--accent-soft` background (currently unused). No adaptation needed for `prefers-reduced-transparency`.

## 8.6 High contrast / forced colors

Windows High Contrast Mode and forced-colors environments override the dashboard's color tokens with system colors. The dashboard supports this gracefully:

```css
@media (forced-colors: active) {
  .card { border: 1px solid CanvasText; }
  .rail-btn.active::before { background: Highlight; }
  .range button.active { background: Highlight; color: HighlightText; }
  .pulse, .metric-delta .tri { forced-color-adjust: none; }
}
```

The pulse dot and delta triangles preserve their original colors via `forced-color-adjust: none` because they carry semantic meaning that the forced palette cannot replicate.

## 8.7 Zoom and text resize

The dashboard reflows correctly at:

- 200% browser zoom: all content remains accessible; no horizontal scroll except on the retention grid (which is acceptable per WCAG 1.4.10 since the grid is a table).
- Text-only zoom up to 200%: rem-based sizes scale; pixel-based chart elements remain fixed.

Implementation: all body sizes use rem; chart SVG viewBoxes are fixed (they scale with their container width, which is rem-based on parent).

## 8.8 Screen reader test plan

The implementation should pass the following test scenarios before production:

1. **NVDA + Chrome (Windows)**: navigate the entire dashboard using only keyboard and verify all metrics and chart summaries are announced.
2. **JAWS + Edge (Windows)**: same as above; verify aria-current and aria-pressed announcements.
3. **VoiceOver + Safari (macOS)**: navigate using VO+arrow; verify chart `<desc>` is read on focus.
4. **VoiceOver + Safari (iOS)**: verify mobile layout headings and skip links work.
5. **TalkBack + Chrome (Android)**: verify mobile layout and tab navigation.

A separate test plan document should track findings; this brief specifies the bar, not the test execution.

## 8.9 Cognitive accessibility

- **Plain language**: section descriptions use plain English. No marketing jargon.
- **Predictable patterns**: every card has the same structure (title → meta → metric → chart → footer). No surprise content arrangements.
- **No timed interactions**: nothing auto-dismisses except the success toast (3s). The toast content is non-essential confirmation.
- **No required actions**: the dashboard is read-only in v1. There are no forms to complete, no decisions to make.
- **Consistent navigation**: rail order never changes. Section count never changes.

## 8.10 Accessibility checklist for new features

Before merging any new component or view, the implementation must verify:

- [ ] Color contrast meets AA for every text pairing
- [ ] All interactive elements are keyboard-reachable
- [ ] Focus state is visible
- [ ] Every interactive element has an accessible name
- [ ] Color is paired with shape/text for any meaningful signal
- [ ] New animations have a reduced-motion equivalent
- [ ] New live regions are properly marked
- [ ] Heading hierarchy is preserved
- [ ] Charts have role="img" with title and desc
- [ ] Screen reader testing performed on at least one combination from § 8.8

This checklist lives in the project repo as a pull-request template item.

## 8.11 Known issues and forward planning

### Issue: chart point keyboard access

Charts are not focusable in v1. When drill-down lands in v1.1, individual data points should become focusable in a sub-region. The pattern:

- Tab moves focus to the chart as a whole (single tab stop).
- Once focused, arrow keys move between data points within the chart.
- Enter on a focused point opens the drill-down panel.
- Escape moves focus back to the chart as a whole; Tab moves out.

This pattern follows WAI-ARIA Authoring Practices for grid widgets.

### Issue: retention grid mobile

At mobile widths, the retention grid uses horizontal scroll. Per WCAG 1.4.10, this is acceptable for tabular data, but the experience can be improved with:

- A sticky first column (row labels)
- Snap-scroll to align with column edges
- A small mono caps hint above the grid: `[ scroll horizontally to view all months ]` — visually hidden, only announced to screen readers

### Issue: theme detection

The dashboard does not auto-detect `prefers-color-scheme` by default. A media query–based default is forward-planned:

```css
@media (prefers-color-scheme: dark) {
  :root:not([data-theme="light"]) { /* dark tokens */ }
}
```

This preserves the explicit toggle while respecting system preference for users who haven't toggled.
