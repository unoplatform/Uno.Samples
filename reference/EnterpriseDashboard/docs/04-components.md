# 04 — Components

This file is the component contract. Every reusable unit in the dashboard is specified here with its props, variants, and states.

Components are framework-neutral in spec. For Uno Platform–specific naming, see `09-implementation-notes.md`.

## Component inventory

| # | Component | Used in |
|---|---|---|
| 4.1 | Shell | Every view |
| 4.2 | NavRail | Every view |
| 4.3 | RailButton | NavRail |
| 4.4 | Header | Every view |
| 4.5 | RangeSelector | Header |
| 4.6 | IconButton | Header, several inline locations |
| 4.7 | ThemeToggle | Header |
| 4.8 | ContextRail | Every view |
| 4.9 | RailSection | ContextRail |
| 4.10 | RailKV | RailSection |
| 4.11 | RailList | § 03 leaderboard |
| 4.12 | SignalItem | § 02 |
| 4.13 | RailNote | Every section |
| 4.14 | SectionBar | Every section |
| 4.15 | SectionHead | Every section |
| 4.16 | Card | Every metric/chart container |
| 4.17 | CardHead | Card |
| 4.18 | MetricBlock | Card |
| 4.19 | Delta | MetricBlock, RailKV, RailList, SignalItem |
| 4.20 | Annotation | Inside charts |
| 4.21 | CardFoot | Card |
| 4.22 | ChartFrame | Card |
| 4.23–4.35 | Chart variants | Across cards |
| 4.36 | Skeleton | Loading states |
| 4.37 | Toast | System feedback |

---

## 4.1 Shell

The outermost layout primitive.

**Structure**
```
[ NavRail | MainColumn | ContextRail ]
```

**Props**
- `theme: "dark" | "light"` (data-theme on root)

**Layout**
- CSS grid: `72px 1fr 360px` on desktop
- Background: `var(--bg)`
- No padding; children own their padding

**States** — only theme. No interaction state on the shell itself.

---

## 4.2 NavRail

The left-side primary navigation.

**Structure**
```
[BrandMark]
[RailButton × N]
... flex space ...
[SettingsRailButton]
[VersionLabel]
```

**Props** — none. The component is static.

**Layout**
- Width: 72px
- Height: 100vh
- Background: `var(--bg)` (no border-fill)
- Right edge: 1px hairline in `var(--border-soft)`
- Top padding: 40px
- Bottom padding: 24px
- Flex column, centered horizontally, 4px gap between RailButtons
- BrandMark margin-bottom: 28px

**Sticky**: yes. The rail does not scroll with content.

---

## 4.3 RailButton

A primary nav icon.

**Props**
- `icon: SVGIcon`
- `label: string` (used for tooltip and aria-label only — never rendered)
- `active: boolean`
- `onClick: () => void`

**Dimensions**: 38×38px. Icon: 16×16px, 1.25px stroke, no fill.

**States**

| State | Icon color | Indicator |
|---|---|---|
| Default | `--ink-3` | none |
| Hover | `--ink-2` | none |
| Active | `--ink` | 1px × 18px `--accent` bar flush to left edge, vertically centered |
| Focus-visible | `--ink-2` | 1px `--accent` outline at 2px offset |
| Disabled | `--ink-4` | none, no hover transition |

**Transitions**: color 200ms ease-standard.

---

## 4.4 Header

The page header above the section content. Specified in `02-information-architecture.md`; component structure here.

**Structure**
```
[ Wordmark + Subtitle | RangeSelector | IconButton(refresh) | ThemeToggle ]
```

**Props** — none. State (theme, range) is owned by the Shell and passed to children.

**Layout**: CSS grid `1fr auto`, align-items: end, gap 32px. Bottom margin 72px before the first section.

---

## 4.5 RangeSelector

A four-option pill toggle.

**Props**
- `options: string[]` — typically `["24H", "7D", "30D", "QTD"]`
- `value: string` — currently active option
- `onChange: (value: string) => void`

**Layout**
- Container: 1px border `--border`, 32px tall, pill-shaped (border-radius 999px), 2px inner padding
- Each option: 26px tall, 14px horizontal padding, mono 10px tracking +0.08em, pill-shaped

**States per option**

| State | Background | Text |
|---|---|---|
| Inactive | transparent | `--ink-3` |
| Hover (inactive) | transparent | `--ink-2` |
| Active | `--accent` | `#04101C` (dark high-contrast text on accent) |
| Disabled (range unavailable for current data window) | transparent | `--ink-4`, cursor not-allowed |

**Behavior**
- Clicking an inactive option calls `onChange` and triggers a 200ms transition.
- Clicking the active option is a no-op.
- Exactly one option is always active.

---

## 4.6 IconButton

A circular icon-only button.

**Props**
- `icon: SVGIcon`
- `label: string` (aria-label)
- `onClick: () => void`
- `variant?: "default" | "subtle"` (default = bordered, subtle = transparent border)

**Dimensions**: 32×32px, border-radius 50%, 1px border `--border`. Icon: 14×14px, 1.4px stroke.

**States**

| State | Border | Icon |
|---|---|---|
| Default | `--border` | `--ink-2` |
| Hover | `--ink-4` | `--ink` |
| Pressed | `--ink-3` | `--ink` |
| Disabled | `--border-soft` | `--ink-4` |

**Refresh-specific behavior**: clicking spins the icon 360° at 600ms, ease-standard. While spinning, the icon color stays at `--ink`.

---

## 4.7 ThemeToggle

A pill-shaped toggle with icon and text label.

**Props**
- `value: "dark" | "light"`
- `onChange: (next: "dark" | "light") => void`

**Layout**
- 32px tall, 1px border `--border`, pill-shaped, 12px left padding, 14px right padding, 8px icon-to-text gap
- Icon: 12×12px, 1.4px stroke
- Label: mono 10px, all caps, tracking +0.12em
- Icon variants: crescent moon (dark) / sun (light)

**Behavior**: clicking flips the theme. The transition is global — see `05-interactions-and-motion.md` for the cross-fade.

---

## 4.8 ContextRail

The right-side context sidebar.

**Props**
- `view: "acq" | "eng" | "coh"` — determines content

**Layout**
- Width: 360px
- Left edge: 1px hairline `--border-soft`
- Padding: `40px 32px 40px 36px`
- Position: sticky, full height, independent scroll

**Children**: one or more `RailSection` blocks, in section-specific order.

---

## 4.9 RailSection

A titled group within the ContextRail.

**Structure**
```
[ Heading + optional pulse ]
[ Sublabel ]
[ Body content ]
```

**Heading**
- Mono 10px, tracking +0.18em, uppercase, `--ink-3`
- Prefixed with `§ ` rendered in `--ink-4`
- If `live: true`, a pulsing accent dot is rendered at the right end of the heading (margin-left: auto)

**Sublabel** (optional)
- Mono 10px, `--ink-4`, all caps, tracking +0.1em, left-padded 14px (under the `§`)

**Body**: arbitrary children — typically `RailKV`, `RailList`, `SignalItem`, or a `RailNote`.

**Spacing**: 32px bottom margin to next RailSection or sibling.

---

## 4.10 RailKV

A label / value row.

**Props**
- `label: string`
- `value: string | number | DeltaPair`

**Layout**
- Flex row, justify-content: space-between
- Padding: 8px top + bottom
- Top border: 1px `--border-soft` (omitted on first sibling)
- Label: body sans, 13px, `--ink`
- Value: mono 11.5px

**Delta pair handling**: if the value contains a magnitude and a delta, render the magnitude in `--ink` and the delta inline to its right with sign-color (positive/negative).

---

## 4.11 RailList

A numbered ranked list. Used on § 03 leaderboard.

**Structure** — each row:
```
[ idx | name | value ]
```

**Layout**
- Grid: `18px 1fr auto`, column gap 12px
- Row padding: 10px top + bottom
- Top border: 1px `--border-soft` (omitted on first)
- Idx: mono 10px, `--ink-4`, two-digit zero-padded
- Name: body sans 13px, `--ink`
- Value: `Delta` component (signed, color-coded, with triangle)

---

## 4.12 SignalItem

A single row in the live Signals feed.

**Structure**
```
[ Marker | Title + Meta ]
```

**Props**
- `marker: "up" | "down" | "alert" | "neutral"`
- `title: string`
- `meta: string` (timestamp + optional status modifier)

**Marker rendering**

| `marker` | Shape | Color |
|---|---|---|
| `up` | Filled triangle ▲ | `--positive` |
| `down` | Filled triangle ▼ | `--negative` |
| `alert` | Filled circle, 6px | `--accent` |
| `neutral` | Filled circle, 5px | `--ink-3` |

Marker positioned at 14px grid column. Vertically aligned to first line of title with 6–8px top offset.

**Title**: body sans 13px, `--ink`, line-height 1.4. Wraps onto two lines max.
**Meta**: mono 10px, `--ink-4`, 4px top margin, tracking +0.04em.

**Padding**: 14px top + bottom. Top border: 1px `--border-soft` (omitted on first).

---

## 4.13 RailNote

A bordered callout used at the end of each ContextRail.

**Structure**
```
// note
{body text}
```

**Layout**
- 1px border `--border-soft` (top, right, bottom); left border 1px `--accent` (3px wide visual effect via inset shadow optional, but 1px is the spec)
- Padding: 16px
- Background: `--surface`
- Top margin: 20px

**Typography**
- Eyebrow `// note`: mono 9px, tracking +0.14em, uppercase, `--ink-4`, 8px bottom margin
- Body: body sans 13px, `--ink-2`, line-height 1.5, weight 300

---

## 4.14 SectionBar

The technical status bar above each section.

**Structure**
```
+  § NN  / 03  Section Title  ─────────────  30D
```

**Layout**
- Flex row, align-items: center, gap 14px
- Mono 10px, tracking +0.18em, uppercase
- Bottom margin: 18px

**Children, left to right**
1. **Crosshair**: 9×9px `+` shape rendered via two 1px lines crossed. Color `--ink-3`.
2. **Tag**: `§ NN` in `--accent`. Always two-digit, zero-padded.
3. **Counter**: `/ 03` in `--ink-3`.
4. **Title**: uppercase section name in `--ink-3`.
5. **Rule**: flex-grow hairline `--border-soft`, 1px tall.
6. **Right tag**: range label (`30D`) in `--ink-3`.

---

## 4.15 SectionHead

The section's display title and one-line description block.

**Structure**
```
{title}
{description}
─────────── (1px hairline)
```

**Typography**
- Title: Bricolage Grotesque display weight 300, opsz 96, 36px, letter-spacing −0.02em, line-height 1.05, `--ink`
- Description: body sans 13px, `--ink-3`, line-height 1.55, max-width 560px
- Bottom hairline: 1px `--border-soft`, 28px below the description

---

## 4.16 Card

The container for a metric + chart pair.

**Props**
- `variant: "tall" | "short"` — controls min-height (`tall` = 380px, `short` = 280px)
- `state: "default" | "loading" | "empty" | "error" | "stale"`

**Layout**
- Padding: 28px top + horizontal, 24px bottom
- Background: `var(--surface)`
- Position: relative (anchors `CardFoot` to bottom-left)
- No border, no shadow — separation comes from the 1px gap fill in the parent grid

**Card grids** that contain cards use this construction:

```css
.grid {
  display: grid;
  gap: 1px;
  background: var(--border-soft);
  border: 1px solid var(--border-soft);
}
.grid > .card { background: var(--surface); }
```

This produces hairline separators without per-card borders.

**State styling**

| State | Card body | Border-left |
|---|---|---|
| Default | normal | none |
| Loading | skeletons replace metric & chart | none |
| Empty | `—` metric, bracketed chart callout | none |
| Error | `—` metric, error chart callout | 1px `--negative` |
| Stale | normal | none (footer carries the ⚠) |

---

## 4.17 CardHead

The title + meta header inside a Card.

**Structure**
```
[ Title | Meta ]
```

**Layout**
- Flex row, justify-content: space-between, align-items: flex-start, gap 20px
- Bottom margin: 28px

**Title**: Bricolage Grotesque 13px weight 500, opsz 24, `--ink-2`, line-height 1
**Meta**: mono 9.5px, tracking +0.14em, uppercase, `--ink-4`, optionally followed by a `dots` ellipsis (`···`) that acts as a future actions menu trigger

---

## 4.18 MetricBlock

The hero metric assembly inside a Card.

**Structure**
```
[ Value | Unit | (filler) | Delta ]
```

**Props**
- `value: string` (pre-formatted, e.g. "$87.3K")
- `unit?: string` (e.g. "mrr")
- `delta?: { magnitude: string; direction: "up" | "down" }`

**Layout**
- Flex row, align-items: baseline, gap 12px
- Bottom margin: 24px

**Typography**
- Value: Bricolage Grotesque weight 200, opsz 96, 56px, letter-spacing −0.035em, `--ink`
- Unit: mono 10px, tracking +0.16em, uppercase, `--ink-3`, padded 6px from bottom baseline
- Delta: see component 4.19

**Variants**
- **Compound value**: e.g. `$87.3K` where `K` is rendered smaller (36px) and `--ink-2`. Implementation: split the string into magnitude + suffix and apply different styles.
- **Non-numeric value**: e.g. `Scale` (top-plan name), `2 PM` (peak hour), `r²` (Pearson coefficient). Same display family, same size.
- **Equation as unit**: e.g. `= 0.87` rendered in the unit slot at display weight (32–34px), not mono. Special-case styled inline.

---

## 4.19 Delta

A signed percentage or delta indicator.

**Props**
- `magnitude: string` (e.g. "+12.4%", "−6%", "+4 pt", "+$14.2K")
- `direction: "up" | "down"`
- `size: "default" | "small"` (default 10.5px, small 9px)

**Structure**
```
[ ▲|▼  magnitude ]
```

**Color** — paired with direction:
- `up` → `--positive`
- `down` → `--negative`

**Triangle**
- 0×0 element with `border-left: 4px transparent`, `border-right: 4px transparent`, and either:
  - `border-bottom: 5px solid currentColor` for up
  - `border-top: 5px solid currentColor` for down
- Inline-block, vertically aligned baseline

Color and shape both carry the signal — important for accessibility.

---

## 4.20 Annotation

A bracketed mono callout inside a chart area or beside a metric.

**Props**
- `text: string` (the body, without brackets)
- `position: "top-right" | "bottom-right" | "in-chart"` (positioned via parent CSS)

**Rendering**
- Mono 10.5px, tracking +0.02em, line-height 1.55, `--ink-3`
- Brackets `[ ` and ` ]` rendered as pseudo-elements in `--ink-4`
- Max width 220px (top-right) or 160px (bottom-right inside a card)

---

## 4.21 CardFoot

Source attribution at the bottom-left of a card.

**Structure**: `[ {Source name} · {update relative timestamp} ]`

**Layout**
- Absolutely positioned: bottom 20px, left 28px
- Mono 9.5px, tracking +0.12em, uppercase, `--ink-4`
- Brackets via pseudo-elements

**Stale state addition**: prepend `⚠` glyph in `--negative` before `Source`.

---

## 4.22 ChartFrame

The chart area within a Card. Hosts one of the chart variants below.

**Layout**
- Width: 100% of card content area
- Height: depends on card variant and chart variant — typically 160–240px

**Internal rules**
- All chart axes use mono 9px text, `--ink-4`, tracking +0.12em
- All grid lines are 1px, `--grid`
- Stroke widths: 1.4px for primary lines, 1px for reference / dashed, 1.2px for arcs
- All chart `text` elements use the `axis-text` class

---

## Chart variants

### 4.23 LineChart
- 1.4px stroke, `--ink`
- Markers at every data point: 2.4px filled circles, `--ink`
- One marker may be promoted to 4px filled `--accent` to mark the highlighted/current point
- Optional baseline reference: 1px dashed (2 4 pattern), `--ink-4`, 55% opacity

### 4.24 BarChart (vertical)
- Bar width 14px, gap 6px
- Recency-weighted opacity ramp (older bars at 0.55, recent at 1.0)
- No axis lines, no labels (sparkline-style by default)

### 4.25 AreaChart
- 1.4px stroke `--ink` along the top
- Fill: linear gradient from `--ink` at 0.22 opacity to 0 at the bottom
- Used for monotonic accumulating values

### 4.26 ScatterPlot
- Points 2px radius, `--ink-2`
- Regression line: 1px dashed (3 4 pattern), `--ink-3`
- Optional inline annotation (component 4.20)

### 4.27 Histogram
- 14px-wide bars, 2px gap
- Bar fill intensity from `--ink-4` (tails) to `--ink` (center)
- Optional median line: 1px dashed (2 3 pattern), `--accent`

### 4.28 HorizontalBarList
- See component spec under § 01.D — a structured list of label + bar + value rows

### 4.29 Heatmap
- Cell grid sized to fit container
- Cell fill: `--ink` with opacity ramp 0.07 → 0.85
- Top decile cells: `--accent` with opacity ramp 0.55 → 0.95
- Hour labels above (0, 6, 12, 18 only)
- Day labels left (MON–SUN, right-aligned)

### 4.30 ConcentricArcs
- N concentric circles, decreasing weight outward
- Innermost ring uses `--accent`, others use `--ink` ramp
- Each ring is a partial arc (dasharray-based), rotated −90° to start at 12 o'clock
- Center label: large display value

### 4.31 BoxPlot (horizontal)
- One row per series
- Whisker: 1px horizontal `--ink-2`
- Box: filled `--surface-2` with 1px `--ink` border
- Median: 1px vertical `--ink` inside the box
- Outliers: 2px dots, `--ink-3`, beyond whiskers

### 4.32 Gauge (semicircle)
- 220° arc
- Background arc: `--ink-5`, 1.2px
- Foreground arc to score: `--ink`, 1.4px
- Inner reference arc: `--ink-4`, 1px
- Needle: 1.3px line + 3px `--accent` pivot dot
- Bottom label: mono caps in `--ink-4`

### 4.33 Slopegraph
- Two columns of dots connected by lines
- Dots: 2.6px filled `--ink`
- Lines: 1px `--ink`
- Column headers: mono caps centered above
- Labels: mono 9px, right-aligned (left column) or left-aligned (right column)

### 4.34 StripPlot
- Single horizontal axis
- Dots: 3px filled `--ink-2`, 70% opacity
- Vertical jitter band ~90px
- Median marker: 1px vertical dashed (3 3 pattern), `--accent`
- Tick labels along bottom: mono caps

### 4.35 RetentionGrid
- Specified in § 03.C
- Equal-width column cells, 32px row height
- Background color from a two-zone ramp:
  - `v ≥ 80`: `--accent` blend
  - `v < 80`: `--ink` blend
- Cell text contrast inverted at high blend

---

## 4.36 Skeleton

A loading placeholder.

**Props**
- `width: string | number`
- `height: number`
- `variant: "text" | "block"` (default block)

**Style**
- Background: `--surface-2`
- Border-radius: 0 (text variant: 1px)
- Animation: opacity 0.4 ↔ 0.6 at 2s `ease-in-out` infinite alternate
- Reduced-motion: static at 0.5 opacity

---

## 4.37 Toast

A bottom-center confirmation message.

**Props**
- `text: string` (already formatted, e.g. "refreshed — 14:22:03")
- `tone: "default" | "negative"`
- `duration: number` (ms, default 3000)

**Style**
- Position: fixed, bottom 24px, horizontal center
- Padding: 10px 16px
- Background: `--surface-3`
- Border: 1px `--border`
- Border-left: 1px `--accent` (default) or `--negative`
- Mono 11px, `--ink`
- Brackets wrap the text via pseudo-elements

**Behavior**
- Enter: fade + 6px upward translate at 200ms ease-decel
- Exit: fade at 200ms ease-accel after `duration` ms
- Multiple toasts stack vertically, 8px between

---

## Component composition rules

These rules govern how components combine and must be enforced at code review.

1. **Card always contains exactly one MetricBlock.** Some charts have no metric (rare); use a `MetricBlock` with `value="—"` and no unit rather than omitting.
2. **MetricBlock value is never empty in default state.** Empty → `—` em-dash.
3. **CardFoot is always present in default and stale states.** Even cards with no obvious data source render `[ Computed · live ]` or similar.
4. **Annotations live inside ChartFrame, never inside MetricBlock.**
5. **Delta lives at the right end of MetricBlock or as the value of a RailKV / RailList.** No standalone Delta in a card.
6. **RailNote always appears last in a ContextRail.** It is the closing sentence.
7. **No two components have the same border-left accent rule.** Currently: RailNote (accent) and Card error state (negative). If a third accent-left appears, the system is leaking.

---

## Component anti-patterns

Patterns that look reasonable but break the system:

- **Adding a "primary button" component.** The dashboard has no primary action surfaces. If one is needed, the design needs to be reconsidered before the component is added.
- **A modal Dialog component.** Drill-down is inline (see `02-information-architecture.md`). Modals interrupt scanning.
- **A Tooltip component.** Hover tooltips on charts can be added, but a generic Tooltip with arrow tail, padding, and shadow is wrong. Use `Annotation` instead.
- **An Avatar component.** No people in the dashboard.
- **A Badge / Pill component for status.** Use the marker shapes inside `SignalItem` or signed `Delta` instead.
