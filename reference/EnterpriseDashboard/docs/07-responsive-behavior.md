# 07 — Responsive Behavior

EnterpriseDashboard (codename: Observatory) is desktop-first. The dashboard is most useful at 1280px and above. Below that, the layout adapts in three progressive steps: tablet, narrow tablet, mobile.

The dashboard is not a phone-first product. The mobile breakpoint exists to make the dashboard *readable on a phone* — for a quick check during a meeting or a stand-up — not to be the primary surface.

## Breakpoints

| Name | Width range | Layout class |
|---|---|---|
| `desktop` | ≥ 1280px | Full three-column |
| `tablet-wide` | 1024–1279px | Three-column, compact rails |
| `tablet-narrow` | 768–1023px | Two-column (rail collapses), context rail moves inline |
| `mobile` | < 768px | Single column, top nav, context inline |

Breakpoints align with common device width clusters. The dashboard does not optimize for in-between sizes — it locks to the breakpoint above the current viewport.

## Desktop (≥ 1280px) — primary

This is the reference layout, fully specified in `02-information-architecture.md` and `03-pages-and-views.md`.

```
┌──────────────────────────────────────────────────────────┐
│       │                                       │          │
│ [Nav] │            Main column                │ Context  │
│ 72px  │            flex                       │  rail    │
│       │                                       │  360px   │
│       │                                       │          │
└──────────────────────────────────────────────────────────┘
```

- Left rail: 72px, sticky, always visible
- Main column: flexible width, scrolls
- Right rail: 360px, sticky, scrolls independently

Main column minimum content width: 760px. At 1280px viewport, this leaves ample padding (56px on each side after rails: `1280 − 72 − 360 = 848`, less 112px padding = 736px content). Tight but acceptable.

## Tablet wide (1024–1279px)

```
┌──────────────────────────────────────────────────────────┐
│       │                                  │              │
│ [Nav] │         Main column              │  Context     │
│ 56px  │         flex                     │   rail       │
│       │                                  │   320px      │
└──────────────────────────────────────────────────────────┘
```

Adaptations:
- Left rail width: 56px (icons remain 16px, padding tightens)
- Right rail width: 320px
- Main column padding: `32px 40px 64px 40px` (reduced from 56px horizontal)
- Hero metric value sizes step down: 56px → 48px

Card grids retain the same column counts (3-col for § 01 row 1, 1.6fr/1fr for § 02, 2-col for § 03). Charts re-flow to the narrower card widths automatically.

The header range selector and theme toggle remain pill-shaped and full-size. The subtitle truncates with ellipsis at viewport edge if necessary; preferred truncation point is at the second dot separator.

## Tablet narrow (768–1023px)

```
┌──────────────────────────────────────────────────────────┐
│       │                                                  │
│ [Nav] │          Main column                             │
│ 56px  │          flex                                    │
│       │                                                  │
│       │  [ ContextRail content inline below sections ]   │
│       │                                                  │
└──────────────────────────────────────────────────────────┘
```

Adaptations:
- Left rail: unchanged from tablet-wide
- **Right rail is removed.** Its content appears inline at the bottom of each section, rendered as full-width blocks.
- Card grids reduce: 3-column rows become 2-column with the last card flowing to a second row at full width.

### Right rail content placement at this breakpoint

Each ContextRail composition becomes a sibling block of the section it accompanies:

```
[ Section status bar ]
[ Section head ]
[ Section cards grid ]
[ ContextRail block — Revenue Breakdown / Movement / Note for § 01 ]
[ Spacer 56px ]
[ Next section ... ]
```

Layout within the inline ContextRail block:
- Three sub-columns when width allows: BreakdownList | MovementList | Note
- Two sub-columns at lower widths: List | List, then Note full width below
- Single column at the bottom of this breakpoint range: each RailSection stacks full width

### Card adaptations at this breakpoint

| Section | Desktop | Tablet narrow |
|---|---|---|
| § 01 row 1 (3-col) | Recurring / Signups / ARR | Recurring full width, then Signups + ARR side by side |
| § 01 row 2 (3-col) | Revenue by Plan / MRR Spread / Usage vs Retention | All three stack 2-col, then 1-col |
| § 02 (1.6fr/1fr) | Heatmap | Goal Attainment | Heatmap full width, then Goal Attainment 1-col below |
| § 03 row 1 (2-col) | Slopegraph / Strip plot | Stack 1-col |
| § 03 row 2 (full) | Retention grid | Retention grid full width with horizontal scroll if needed |

## Mobile (< 768px)

```
┌──────────────────────────┐
│ [Top nav with tabs]      │
├──────────────────────────┤
│                          │
│      Main column         │
│      single col          │
│                          │
│  [ ContextRail inline ]  │
│                          │
└──────────────────────────┘
```

Adaptations:

### Navigation

The left rail is removed. Section navigation moves to a top tab bar.

```
┌─────────────────────────────┐
│ Observatory                 │
│ SAAS · 2M AGO     ⟳    ☾    │
│ ┌────┬────┬────┐            │
│ │§01 │§02 │§03 │            │
│ └────┴────┴────┘            │
│ 24H  7D  30D  QTD           │
└─────────────────────────────┘
```

- Wordmark and subtitle sit on the first line.
- Refresh and theme toggle right-align next to the wordmark.
- Section tabs sit below the wordmark — three equal-width buttons, the active one underlined with a 1px `--accent` bar.
- Range selector moves to its own row below the tabs, full width pill.

### Main column

- Single column for all cards
- Card padding: `20px 20px 16px 20px`
- Card grid gap fill remains 1px hairlines
- Hero metric value scales: 56px → 40px
- Section title: 36px → 28px
- Charts re-render to fit narrower widths

### Right rail content

Always inline, below the section's last card. Each RailSection becomes full width:

```
[ section cards ]
[ RailSection: Revenue Breakdown ]
[ RailSection: Movement ]
[ RailNote ]
```

### Specific chart adaptations on mobile

| Chart | Mobile behavior |
|---|---|
| Line chart (Recurring Revenue) | Re-render to mobile width; X-axis labels show only Jan / Jul / Dec |
| Bar chart (New Signups) | Bar widths shrink proportionally; recency opacity ramp preserved |
| Area chart (Cumulative ARR) | Re-renders; no axis labels needed |
| Horizontal bar list (Revenue by Plan) | Stack labels above bars instead of side-by-side |
| Histogram (MRR Spread) | Bin count reduces from 20 to 12 |
| Scatter plot (Usage vs Retention) | Re-renders; annotation moves above chart |
| Heatmap (Active Usage) | Cell width shrinks to ~9px; hour labels show 00 / 12 only |
| Concentric arcs (Goal Attainment) | Center value scales down to 24px |
| Box plot (Session Length by Plan) | Row height shrinks to 28px; outliers preserved |
| Gauge (Health Score) | Scales proportionally; needle remains 1.3px |
| Slopegraph (Retention by Segment) | Re-renders; segment labels truncate to 3-letter abbreviations |
| Strip plot (Per-Account MRR) | Re-renders; tick labels show 0 / 5 / 10 only |
| Retention grid | Horizontal scroll within card; row labels (months) sticky to left edge |

## Touch targets

On tablet and mobile, all interactive elements meet WCAG touch-target minimums:

- IconButton: 32px (minimum) — meets WCAG AA at 24×24px and approaches AAA at 44×44px. On mobile, pad to 44×44px tap area while keeping visible 32×32px.
- Top nav tabs (mobile): 44px tall
- Range selector pills: 32px tall, full width on mobile
- RailButton: 38px (desktop) — on tablet narrow, IconButtons in the top nav use 40px to accommodate touch

The implementation should add a `padding: 6px;` or use an invisible larger hit area to bring effective touch targets to 44×44px on mobile devices, regardless of visible size.

## Density tradeoffs

The dashboard is dense by design. On mobile, that density becomes a problem at extreme widths (320px). The implementation makes these density compromises on mobile:

- Card meta (`12 MO`, etc.) hides when card width < 280px. The meta is informational, not essential.
- Card footer (`[ Source · updated 2m ago ]`) hides when card width < 240px.
- Chart annotations hide when card width < 320px.
- Right rail KV value-aligned columns: at narrow widths, the value wraps to a second line below the label rather than truncating.

These are pruning rules, not transformations. Hidden content stays hidden; it is not collapsed into expandable details.

## Print

When the user prints (or "save as PDF"):

- Theme is forced to light, regardless of current setting.
- Page margins: 1cm.
- All three sections render in order (no tab-switch on print).
- Right rail content appears below each section's main content, not in a sidebar column.
- Pulses, refresh spin, skeletons are not rendered (they would be static anyway).
- A page break is inserted before each section status bar.
- Page header on each page: `Observatory — § NN / 03 SECTION TITLE — Page N`.

The CSS `@media print` rule lives in the dashboard stylesheet, not in a separate print stylesheet.

## High-DPI / retina

The dashboard uses no raster imagery. All charts are SVG, all icons are SVG, all type renders natively. No retina adaptations are needed.

If a screenshot is exported (out of scope for v1), the export should target 2× DPI rendering and use the appropriate font subsetting.

## Container queries (forward planning)

The current implementation uses media queries on viewport width. If embedded in a future shell that constrains the dashboard to a portion of the viewport (e.g., a sidebar drawer in a parent app), the implementation should migrate to container queries on the Shell component:

```css
.shell {
  container-type: inline-size;
  container-name: dashboard;
}
@container dashboard (max-width: 768px) {
  /* mobile rules */
}
```

This is forward-planning, not a v1 requirement.

## Viewport meta

```html
<meta name="viewport" content="width=device-width, initial-scale=1, viewport-fit=cover">
```

No `user-scalable=no`. The dashboard tolerates user zoom. At 200% zoom on mobile, text reflows naturally within cards; no horizontal scroll except on the retention grid card.

## Orientation

- **Landscape preferred** at all viewport sizes.
- **Portrait** is supported but tablet-narrow + portrait reverts to mobile layout at < 768px effective width.
- No specific orientation lock or warning is presented.

## Browser compatibility targets

| Browser | Versions supported |
|---|---|
| Chromium-based | last 2 major versions |
| Safari | last 2 major versions, including iOS Safari |
| Firefox | last 2 major versions |

CSS features required: CSS custom properties, CSS grid, `color-mix()`, `font-variation-settings`, `prefers-reduced-motion`. All are supported in all listed browsers.

`color-mix()` is used in the retention grid intensity calculation. Fallback for older browsers: pre-calculate the 8-step ramp as static colors. Documented in `09-implementation-notes.md`.
