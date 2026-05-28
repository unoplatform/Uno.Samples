# 06 — Theme and Visual System

This file is the complete design token specification: colors, typography, spacing, borders, radius, iconography. Implementation should treat the tokens here as authoritative.

Two token layers are defined:
- **Primitive tokens**: raw ramps (neutral slate, accent blue, semantic mint/coral). Used to compose semantic tokens.
- **Semantic tokens**: named by role (`--bg`, `--surface`, `--ink`, `--accent`). Used by components.

Only semantic tokens are referenced by component CSS. Primitive tokens are used only to define semantic tokens for each theme.

## 6.1 Color

### 6.1.1 Primitive ramps

#### Cool slate (neutral)

A cool, slightly-blue-shifted neutral ramp. Used for backgrounds, surfaces, borders, and ink.

| Token | Hex | Notes |
|---|---|---|
| `--slate-50` | `#F1F3F7` | Light theme bg |
| `--slate-100` | `#E7EBF1` | Light theme surface-2 |
| `--slate-200` | `#DCE1EA` | Light theme surface-3 |
| `--slate-300` | `#CFD5DF` | Light theme border |
| `--slate-400` | `#9DA3AE` | Light theme ink-4 |
| `--slate-500` | `#6C7280` | Light theme ink-3 |
| `--slate-600` | `#3E4350` | Light theme ink-2 |
| `--slate-700` | `#232831` | Dark theme border |
| `--slate-800` | `#14171F` | Dark theme surface-2 |
| `--slate-850` | `#0E1015` | Dark theme surface |
| `--slate-900` | `#08090C` | Dark theme bg |
| `--slate-950` | `#0F1115` | Light theme ink |

#### Blue accent

| Token | Hex | Use |
|---|---|---|
| `--blue-300` | `#7DB8FF` | Dark theme accent |
| `--blue-500` | `#3B82F6` | (reserved, not in current use) |
| `--blue-700` | `#2F66C9` | Light theme accent |

#### Mint (positive semantic)

| Token | Hex | Use |
|---|---|---|
| `--mint-400` | `#4FC9A7` | Dark theme positive |
| `--mint-700` | `#1F8870` | Light theme positive |

#### Coral (negative semantic)

| Token | Hex | Use |
|---|---|---|
| `--coral-400` | `#E8806F` | Dark theme negative |
| `--coral-700` | `#B84A38` | Light theme negative |

### 6.1.2 Semantic tokens

These are the only color references used by components.

#### Dark theme (`data-theme="dark"`)

| Token | Value | Role |
|---|---|---|
| `--bg` | `#08090C` | Page background; left rail background |
| `--surface` | `#0E1015` | Card background; rail-note background |
| `--surface-2` | `#14171F` | Box plot fill; skeleton |
| `--surface-3` | `#1A1E27` | Toast background |
| `--border` | `#232831` | Outlined buttons, toggles |
| `--border-soft` | `#181C24` | Card grid hairlines, section dividers |
| `--ink` | `#E6E9EE` | Primary text, primary chart strokes |
| `--ink-2` | `#A8ADB6` | Secondary text, card titles |
| `--ink-3` | `#737880` | Tertiary text, axis ticks, descriptions |
| `--ink-4` | `#494E58` | Quaternary, labels at smallest level |
| `--ink-5` | `#2A2E37` | Disabled bar tracks, gauge background |
| `--accent` | `#7DB8FF` | Active state, live indicators, current point |
| `--accent-soft` | `rgba(125,184,255,0.14)` | Subtle accent backgrounds (currently unused, reserved) |
| `--positive` | `#4FC9A7` | Positive deltas, up triangles |
| `--negative` | `#E8806F` | Negative deltas, down triangles, errors |
| `--grid` | `#1E222B` | Chart grid lines |

#### Light theme (`data-theme="light"`)

| Token | Value | Role |
|---|---|---|
| `--bg` | `#F1F3F7` | Page background |
| `--surface` | `#FAFBFD` | Card background |
| `--surface-2` | `#E7EBF1` | Box plot fill, skeleton |
| `--surface-3` | `#DCE1EA` | Toast background |
| `--border` | `#CFD5DF` | Outlined buttons |
| `--border-soft` | `#DFE3EB` | Hairlines |
| `--ink` | `#0F1115` | Primary text |
| `--ink-2` | `#3E4350` | Secondary text |
| `--ink-3` | `#6C7280` | Tertiary |
| `--ink-4` | `#9DA3AE` | Quaternary |
| `--ink-5` | `#C8CDD7` | Disabled tracks, gauge background |
| `--accent` | `#2F66C9` | Active state |
| `--accent-soft` | `rgba(47,102,201,0.10)` | Reserved |
| `--positive` | `#1F8870` | Positive deltas |
| `--negative` | `#B84A38` | Negative deltas |
| `--grid` | `#D6DBE3` | Chart grid lines |

### 6.1.3 Color rules

These rules are enforceable at code review.

- **Components reference only semantic tokens.** No primitive references in component CSS. No hex literals in component CSS.
- **Accent is reserved.** `--accent` appears only on: range selector active state, rail-button active indicator, live pulse, heatmap peak cells, retention grid high-retention cells, scatter regression marker, line-chart peak point, median dashed line, refresh-spin halo (none in v1), rail-note left border, error toast accent variant (no — error uses `--negative`), focus rings.
- **Positive and negative are reserved for sign-coded data.** Never used as decorative color.
- **No color is used to convey state on its own.** Always paired with shape (triangle, dot, border-style) or text (`+`, `−`).
- **Charts use only the ink ramp and grid color for non-accent elements.** No chart uses `--positive` or `--negative` as a strokes (data has no intrinsic polarity in shape; sign-coding is in numeric labels only).

### 6.1.4 Contrast targets

All targets are WCAG 2.1 AA minimums; many pairings exceed AAA.

| Pairing (dark theme) | Ratio | Target |
|---|---|---|
| `--ink` on `--bg` | 14.8 | AA Normal (4.5) ✓ |
| `--ink-2` on `--surface` | 8.1 | AA Normal ✓ |
| `--ink-3` on `--surface` | 4.6 | AA Normal ✓ |
| `--ink-4` on `--surface` (small caps mono) | 2.9 | AA Large (3.0) — see note |
| `--accent` on `--bg` | 7.2 | AA Normal ✓ |
| `--positive` on `--surface` | 6.4 | AA Normal ✓ |
| `--negative` on `--surface` | 5.1 | AA Normal ✓ |

**Note on `--ink-4`**: This token is reserved for mono caps labels at 9–10px tracked +0.14em. It tests at ~2.9:1 in some surface contexts, just under the 3.0 large-text target. The implementation can either:
1. Raise `--ink-4` to `#525762` (3.2:1) — recommended.
2. Treat tracked-caps mono as "decorative" labeling for non-essential info — acceptable since the labels are supportive, not primary.

See `08-accessibility.md` for the full accessibility audit.

## 6.2 Typography

### 6.2.1 Type families

Two families. Both load from Google Fonts.

```html
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Bricolage+Grotesque:opsz,wght@12..96,200;12..96,300;12..96,400;12..96,500;12..96,600&family=JetBrains+Mono:wght@300;400;500&display=swap" rel="stylesheet">
```

#### Bricolage Grotesque
- A variable font with optical size (opsz 12–96) and weight (200–600) axes.
- Display use leans heavy on opsz 96 + light weight (200–300) for the characteristic high-contrast display feel.
- UI use leans on opsz 12–24 + medium weight (400–500) for legibility at small sizes.

#### JetBrains Mono
- Three weights loaded: 300, 400, 500.
- Used exclusively for data, metadata, axis labels, brackets, status text.

### 6.2.2 Type scale

| Token | Family | Weight | Size | Line-height | Tracking | Use |
|---|---|---|---|---|---|---|
| `display-xl` | Bricolage | 300 | 48px | 0.95 | −0.025em | Page wordmark |
| `display-l` | Bricolage | 300 | 36px | 1.05 | −0.02em | Section title |
| `display-m` | Bricolage | 200 | 56px | 0.90 | −0.035em | Hero metric value |
| `display-s` | Bricolage | 200 | 32px | 1.0 | −0.025em | Metric "unit" when rendered display (e.g. `= 0.87`, `segments`) |
| `display-xs` | Bricolage | 300 | 30px | 1.0 | −0.02em | Ring chart center value |
| `body-l` | Bricolage | 400 | 16px | 1.5 | 0 | (reserved — not used in v1) |
| `body-m` | Bricolage | 400 | 13px | 1.55 | 0 | Body text, section descriptions, RailKV |
| `body-s` | Bricolage | 300 | 13px | 1.5 | 0 | Rail note body |
| `card-title` | Bricolage | 500 | 13px (opsz 24) | 1 | 0 | Card titles |
| `mono-l` | JetBrains Mono | 400 | 11.5px | 1.4 | +0.02em | Rail values |
| `mono-m` | JetBrains Mono | 400 | 10.5px | 1.55 | +0.04em | Deltas, annotations |
| `mono-s` | JetBrains Mono | 400 | 10px | 1.4 | +0.16em UPPERCASE | Metric units, range pills, theme toggle label, subtitle |
| `mono-xs` | JetBrains Mono | 400 | 9px–9.5px | 1.4 | +0.12em UPPERCASE | Axis labels, card meta, card foot, sublabel |
| `mono-pico` | JetBrains Mono | 400 | 9px | 1 | +0.18em UPPERCASE | Section bar, leaderboard idx |

### 6.2.3 Type rules

- **No serif anywhere.** Even though Bricolage has a serif sibling (Bricolage), the dashboard uses only the grotesque.
- **No italic anywhere.** Even though Bricolage has italics, they are not used.
- **All mono labels at `mono-xs` and `mono-pico` are uppercase with positive tracking.** This is the "instrument label" pattern.
- **All Bricolage display sizes (`display-*`) use the optical-size 96 axis.** This produces the high-contrast display look. Body uses opsz 12–24.
- **Bricolage at small sizes uses weight 400 or 500.** Weight 200–300 at body sizes is too thin and fails contrast on `--surface`.

### 6.2.4 OpenType features

- **Tabular numerals on**: all numeric values in mono. Set `font-variant-numeric: tabular-nums`. This ensures vertical alignment in KV lists, leaderboards, retention grids.
- **Stylistic alternates**: do not enable. Bricolage's defaults are the intended look.

## 6.3 Spacing

A 4px base scale.

| Token | Value |
|---|---|
| `space-0` | 0 |
| `space-1` | 4px |
| `space-2` | 8px |
| `space-3` | 12px |
| `space-4` | 16px |
| `space-5` | 20px |
| `space-6` | 24px |
| `space-7` | 28px |
| `space-8` | 32px |
| `space-10` | 40px |
| `space-12` | 48px |
| `space-14` | 56px |
| `space-18` | 72px |
| `space-20` | 80px |

### Standard spacing decisions

| Use | Token |
|---|---|
| Card padding (top, x) | `space-7` (28px) |
| Card padding (bottom) | `space-6` (24px) |
| Main column padding (x) | `space-14` (56px) |
| Main column padding (top) | `space-10` (40px) |
| Main column padding (bottom) | `space-20` (80px) |
| Header bottom margin | `space-18` (72px) |
| Section bar bottom margin | `space-5` (20px) hairline rule between section bar and head |
| Section description max-width | 560px |
| Card grid gap | 1px (filled by background, not gap) |
| Right rail padding | `40px 32px 40px 36px` |
| RailSection bottom margin | `space-8` (32px) |
| MetricBlock bottom margin | `space-6` (24px) |
| CardHead bottom margin | `space-7` (28px) |

### Vertical rhythm

Sections are separated by `space-14` (56px) bottom margin. Within a section, rows are separated by the 1px grid-fill seam — no additional vertical gap between rows.

## 6.4 Borders, radius, elevation

### 6.4.1 Border widths

| Token | Width |
|---|---|
| `border-hairline` | 1px |

Only 1px borders are used in the dashboard. No 2px borders, no double borders, no dashed borders for layout (dashed appears only in chart lines).

### 6.4.2 Radius

| Token | Value | Use |
|---|---|---|
| `radius-none` | 0 | Cards, chart frames, retention cells, signal markers, sections |
| `radius-pill` | 999px | RangeSelector, IconButton, ThemeToggle |
| `radius-circle` | 50% | RailMark, alert dots, gauge needle pivot |

Square corners are the default. Pills and circles are deliberate exceptions for affordances that benefit from a non-rectangular shape (toggles, status dots).

### 6.4.3 Elevation

The dashboard has no shadow-based elevation. Layered surfaces are differentiated by tone shift:

```
--bg       (lowest)
  ↓
--surface
  ↓
--surface-2
  ↓
--surface-3 (highest, toast and gauge well)
```

This four-step ramp is the entire elevation system. No `box-shadow` declarations appear in component CSS except as the focus-ring technique (see `05-interactions-and-motion.md`).

**Why no shadows.** Shadows imply atmospheric depth, light source, and physical material. None of those fit a calibrated instrument aesthetic. Tone shifts feel correct for an instrument readout.

### 6.4.4 Outline (focus)

Documented in `05-interactions-and-motion.md`. Reproduced here for completeness:

```
box-shadow: 0 0 0 2px var(--bg), 0 0 0 3px var(--accent);
```

This produces a 1px gap between element and ring (inner shadow is bg color), then a 1px accent ring.

## 6.5 Iconography

### 6.5.1 Icon style

All icons are line icons drawn on a 16×16px grid (RailButton scale) or 14×14px (IconButton scale).

- Stroke width: 1.25px (RailButton) or 1.4px (IconButton)
- Stroke color: `currentColor` (inherits from element)
- No fill (or fill: `none`)
- No two-tone or filled icons
- Stroke linecap: butt (default), not round
- Stroke linejoin: miter (default), not round
- Square corners on all icon paths

This consistent line-icon vocabulary is mandatory. Mixing filled and outlined icons breaks the system.

### 6.5.2 Icon catalog

Icons used in v1:

| Icon | Use | Description |
|---|---|---|
| `chart` | NavRail § 01 | Trending line on baseline, 16×16 |
| `heart` | NavRail § 02 | Outline heart, 16×16 |
| `grid` | NavRail § 03 | Four-circle 2×2 grid, 16×16 |
| `settings` | Rail foot | Three horizontal lines with offset dots (sliders), 16×16 |
| `refresh` | Header | Circular arrow with terminator triangle, 14×14 |
| `moon` | ThemeToggle (in dark mode) | Crescent, 12×12 |
| `sun` | ThemeToggle (in light mode) | Circle with rays, 12×12 |
| `triangle-up` | Delta positive | Filled solid triangle, geometric |
| `triangle-down` | Delta negative | Filled solid triangle, geometric |
| `dot` | Signal markers, pulse | Filled circle |
| `crosshair` | Section bar | Plus sign, two 1px lines |

### 6.5.3 Icon rules

- No emoji.
- No icon font.
- No multi-color icons.
- All icons are inline SVG with `currentColor` so they retheme automatically.
- Icons are never used as decoration. Every icon either labels an action (refresh, settings) or symbolizes a state (signal marker, delta direction).

## 6.6 Visual hierarchy

The hierarchy from highest to lowest visual weight on a typical screen:

1. **Hero metric values** — Bricolage display, 56px, weight 200 on `--ink`. Largest things by an order of magnitude.
2. **Section titles** — Bricolage 36px weight 300 on `--ink`. Large but quieter than metric values.
3. **Page wordmark** — Bricolage 48px weight 300 on `--ink`. Same `--ink` color as metric values but less weight.
4. **Active accent** — `--accent` color. Reserved for active state, current point, live indicator. Highest *color* weight despite being small.
5. **Card titles** — Bricolage 13px weight 500 on `--ink-2`. Small but bolder weight to anchor each card.
6. **Body / descriptions** — Bricolage 13px weight 400 on `--ink-3`. Most copy.
7. **Mono labels** — JetBrains Mono 9–10px tracked-caps on `--ink-3` / `--ink-4`. Lowest in hierarchy but most visually distinct via the all-caps tracked treatment.

The hierarchy is enforced through scale and weight, not through color brightness alone. Even at full saturation, the accent does not dominate because it appears in tiny amounts (pulse dot, 1px bar, 1 chart marker).

## 6.7 Composition rhythm

A few rules that govern how things sit on the page:

- **Cards always occupy a row in groups of 2 or 3.** Never a single full-width card except § 03.C (Cohort Retention) where the chart structure demands width.
- **Within a row, all cards share the same height.** Min-heights from `card.tall` (380px) or `card.short` (280px).
- **Section content never exceeds two scroll-screens on a 1440×900 desktop.** Section 02 is the upper bound; the others are roughly 1.2 screens.
- **Vertical sequence within a card** is fixed: CardHead → MetricBlock → ChartFrame → CardFoot (absolutely positioned to bottom-left). Any new content type must fit one of these slots.
- **The right rail always closes with a RailNote.** It is the narrative coda — a single sentence per section.

## 6.8 Token export

For implementation, the tokens are exported as CSS custom properties at the root, scoped by `data-theme`. The complete export is in the `observatory.html` prototype. For Uno Platform `ThemeResource` translation, see `09-implementation-notes.md`.
