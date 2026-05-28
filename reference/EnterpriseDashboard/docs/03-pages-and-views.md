# 03 — Pages and Views

This file specifies every section, every card, and the states (default, loading, empty, error) that each must support.

For visual tokens (colors, type, spacing) see `06-theme-and-visual-system.md`. For component contracts see `04-components.md`. This file is the content-level map.

## Page shell (applies to every view)

| Region | Width | Height | Sticky |
|---|---|---|---|
| Left rail | 72px | 100vh | yes |
| Main column | flex | 100vh scrollable | no |
| Right rail | 360px | 100vh | yes |

Main column padding: `40px 56px 80px 56px`. The bottom padding is intentionally larger to keep the last section breathing above any future page-edge UI.

All three views share the **Header** specified in `02-information-architecture.md`. It is rendered once at the top of the main column, above the active section content.

---

## § 01 — Acquisition & Revenue

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│ + § 01 / 03  ACQUISITION & REVENUE  ──────────────────  30D  │  ← Section status bar
├──────────────────────────────────────────────────────────────┤
│ Acquisition & Revenue                                        │  ← Section title (display)
│ Recurring revenue, signups, and how usage tracks…           │  ← Section description
│ ────────────────────────────────────────────────────────────│  ← Hairline rule
│                                                              │
│ ┌─────────────┬─────────────┬─────────────┐                 │
│ │ Recurring   │ New Signups │ Cumulative  │                 │  ← Row 1: three cards
│ │ Revenue     │             │ ARR         │                 │
│ │ (line+anno) │ (bar chart) │ (area)      │                 │
│ └─────────────┴─────────────┴─────────────┘                 │
│ ┌─────────────┬─────────────┬─────────────┐                 │
│ │ Revenue by  │ Account MRR │ Usage vs    │                 │  ← Row 2: three cards
│ │ Plan        │ Spread      │ Retention   │                 │
│ │ (h-bars)    │ (histogram) │ (scatter)   │                 │
│ └─────────────┴─────────────┴─────────────┘                 │
└──────────────────────────────────────────────────────────────┘
```

Cards in a row share the same height. Row 1 cards are `tall` (~380px), Row 2 cards are `short` (~280px). Cards are separated by 1px hairlines (`--border-soft`), not gaps.

### Card 01.A — Recurring Revenue

| Slot | Content |
|---|---|
| Title | "Recurring Revenue" |
| Meta | `12 MO` |
| Hero metric | `$87.3K` |
| Unit | `mrr` |
| Delta | `+12.4%` (positive) |
| Chart | Line chart with annotated baseline |
| Annotation | `[ peak observed sep — primary channel diverges from baseline ]` |
| Footer | `[ Stripe · updated 2m ago ]` |

**Chart spec.** Twelve monthly values plotted as a line. X-axis: months Jan–Dec (mono, all caps, tracking +0.14em). Y-axis: values 0, 25, 50, 75, 100 — labels left-aligned, mono, `--ink-4`. Grid lines at each y-tick, 1px, `--grid`. The line itself is `--ink`, 1.4px stroke, no markers except for one **accent marker** at the peak month (Sep) — a 4px filled `--accent` circle. A 1px dashed reference baseline (`--ink-4`, 2-4 dash pattern, 55% opacity) connects the first and last points to show trend deviation.

**Annotation position.** Top-right of the chart, mono, bracketed, `--ink-3`. Max width 220px. Wraps onto two lines.

### Card 01.B — New Signups

| Slot | Content |
|---|---|
| Title | "New Signups" |
| Meta | `MONTHLY` |
| Hero metric | `1,284` |
| Unit | `this mo` |
| Delta | `+8.0%` (positive) |
| Chart | 12-bar histogram (monthly) |
| Footer | `[ Product DB · updated 2m ago ]` |

**Chart spec.** Twelve vertical bars, equal width, 14px wide with 6px gaps. Heights proportional to monthly signups. The peak month and the trailing four months are `--ink` (full strength); preceding months step down through `--ink-2` at 0.55 → 0.75 opacity to imply recency weighting. No axes, no labels — this is a sparkline-style chart, the metric tells the story.

### Card 01.C — Cumulative ARR

| Slot | Content |
|---|---|
| Title | "Cumulative ARR" |
| Meta | `YTD` |
| Hero metric | `$1.04M` |
| Unit | `arr` |
| Delta | `+22%` (positive) |
| Chart | Area chart (monotonic, ascending) |
| Footer | `[ Finance · updated 5m ago ]` |

**Chart spec.** A single ascending line from bottom-left to top-right with a soft gradient fill below (linear, `--ink` at 22% opacity at top to 0% at bottom). No axes, no annotations. The shape carries the meaning.

### Card 01.D — Revenue by Plan

| Slot | Content |
|---|---|
| Title | "Revenue by Plan" |
| Meta | `RANKED` |
| Hero metric | `Scale` |
| Unit | `top plan` |
| Chart | Horizontal bar list, four rows (Scale, Pro, Team, Starter) |
| Footer | `[ Billing · updated 2m ago ]` |

**Chart spec.** Four rows. Each row: label (mono, 9.5px, tracking +0.14em) + bar + numeric value. Bars are 8px tall, full width of the bar column, `--ink-5` track with `--ink` fill. Values: right-aligned mono. Plan order is sorted descending by value. The hero metric "Scale" matches the top row.

### Card 01.E — Account MRR Spread

| Slot | Content |
|---|---|
| Title | "Account MRR Spread" |
| Meta | `20 BINS` |
| Hero metric | `$512` |
| Unit | `median` |
| Chart | Histogram (20 bins, symmetric) |
| Footer | `[ Billing · updated 1h ago ]` |

**Chart spec.** Twenty vertical bars showing distribution of per-account MRR. A 1px dashed vertical line in `--accent` marks the median. Bar intensities cluster around the center bars (`--ink`), tapering to `--ink-4` at the tails.

### Card 01.F — Usage vs Retention

| Slot | Content |
|---|---|
| Title | "Usage vs Retention" |
| Meta | `BY ACCOUNT` |
| Hero metric | `r²` |
| Unit | `= 0.87` (rendered large, same display family) |
| Chart | Scatter plot with regression line |
| Annotation | `[ positive linear trend — cluster around regression line ]` |
| Footer | `[ Mixpanel · updated 5m ago ]` |

**Chart spec.** ~30 plot points in `--ink-2`, 2px radius. A dashed regression line (`--ink-3`, 3-4 dash pattern) from bottom-left to top-right. Annotation positioned bottom-right inside the chart area, mono bracketed.

### Right rail — § 01

```
§ REVENUE BREAKDOWN
$24.1K  ▲ +18%
NEW MRR · THIS MONTH

Scale       $41.2K +14%
Pro         $22.4K +9%
Team        $11.8K +4%
Starter     $7.1K  +2%
Trial       $4.8K  −6%

§ MOVEMENT
NET CHANGE · 30D

Expansion       +$14.2K
New business    +$12.0K
Reactivation    +$1.4K
Contraction     −$3.1K
Churn           −$0.4K

╭──────────────────────────────╮
│ // note                       │
│ Expansion drove 62% of net    │
│ new MRR this month, led by    │
│ Scale and Pro upgrades.       │
╰──────────────────────────────╯
```

The note callout has a 1px left border in `--accent`, a `--surface` background, and a `// note` mono eyebrow.

---

## § 02 — Engagement & Health

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│ + § 02 / 03  ENGAGEMENT & HEALTH  ──────────────────────  30D │
├──────────────────────────────────────────────────────────────┤
│ Engagement & Health                                          │
│ When accounts are active, how they progress…                 │
│ ────────────────────────────────────────────────────────────│
│                                                              │
│ ┌──────────────────────────┬──────────────────────┐         │
│ │ Active Usage             │ Goal Attainment      │         │  ← Row 1: 1.6fr + 1fr
│ │ (day × hour heatmap)     │ (concentric rings)   │         │
│ └──────────────────────────┴──────────────────────┘         │
│ ┌──────────────────────────┬──────────────────────┐         │
│ │ Session Length by Plan   │ Health Score         │         │  ← Row 2: 1.6fr + 1fr
│ │ (5-row box plot)         │ (semicircle gauge)   │         │
│ └──────────────────────────┴──────────────────────┘         │
└──────────────────────────────────────────────────────────────┘
```

Row layout differs from § 01: a 1.6fr / 1fr split per row. The wider card holds the data-dense chart; the narrower card holds the synthesized metric (a ring score, a gauge).

### Card 02.A — Active Usage

| Slot | Content |
|---|---|
| Title | "Active Usage" |
| Meta | `DAY × HOUR` |
| Hero metric | `2 PM` |
| Unit | `peak hour` |
| Chart | 7×24 heatmap |
| Footer | `[ Events · live ]` |

**Chart spec.** Seven rows (MON–SUN) × 24 columns (00–23). Each cell ~21×20px with 1px gap. Cell intensity = `--ink` with opacity ramp 0.07 → 0.85 based on activity. **Peak cells (top decile) flip to `--accent`** at 55–95% opacity. Hour ticks at 00, 06, 12, 18 above the grid (mono, 9px). Day labels left of the grid (mono, 9px, right-aligned, all caps).

### Card 02.B — Goal Attainment

| Slot | Content |
|---|---|
| Title | "Goal Attainment" |
| Meta | `RINGS · Q3` |
| Hero metric | `79%` |
| Unit | `blended` |
| Delta | `+4 pt` (positive) |
| Chart | Six concentric arcs |
| Footer | `[ OKR tracker · daily ]` |

**Chart spec.** Six concentric circle outlines, centered, decreasing weight from inner to outer:
- r=20 → `--accent` (highest priority goal, fullest fill)
- r=34 → `--ink-2`
- r=48 → `--ink-3`
- r=62 → `--ink-3` at 60% opacity
- r=76 → `--ink-4` at 50%
- r=90 → `--ink-4` at 35%

Each ring is drawn as a partial arc, dasharray-controlled, rotated `-90°` so they start at 12 o'clock. Center text: `79%` in display weight, 30px.

### Card 02.C — Session Length by Plan

| Slot | Content |
|---|---|
| Title | "Session Length by Plan" |
| Meta | `BOX · 5 PLANS` |
| Hero metric | `9.2` |
| Unit | `min median` |
| Chart | Horizontal box plot, 5 rows |
| Footer | `[ Analytics · updated 1h ago ]` |

**Chart spec.** Five rows: FREE, STARTER, PRO, TEAM, SCALE. Each row contains:
- Row label (mono, 9px, tracking +0.14em, left-aligned)
- Whiskers (1px horizontal line, `--ink-2`)
- Box (rectangle with `--surface-2` fill, `--ink` stroke)
- Median line (vertical 1px `--ink` line inside the box)
- Outliers as 2px `--ink-3` dots beyond the whiskers

Five vertical light grid lines (`--grid`, 1px) span the full chart height at percentile gridmarks.

### Card 02.D — Health Score

| Slot | Content |
|---|---|
| Title | "Health Score" |
| Meta | `COMPOSITE` |
| Hero metric | `74` |
| Unit | `of 100` |
| Delta | `+3` (positive) |
| Chart | Semicircular gauge |
| Footer | `[ Composite · daily ]` |

**Chart spec.** A 220° arc spanning roughly -90° to +90° from the bottom of the chart, drawn as two layered arcs:
- Background full arc, `--ink-5`, 1.2px
- Foreground partial arc filled to the score position, `--ink`, 1.4px
- Inner thinner reference arc, `--ink-4`, 1px

Needle: 1.3px line from center to the score position, with a `--accent` 3px circle at the pivot. Label `NOMINAL` below the pivot in mono caps.

### Right rail — § 02

```
§ SIGNALS                                         ● (live pulse)
LIVE

▲ Activation +4 pt vs Q3 target               2m ago
▲ Peak usage 2 PM, Tuesday                    1h ago
● 3 Enterprise accounts idle 14 days          3h ago · at risk
· API latency nominal · 142 ms p95            5m ago
▼ Weekend engagement −30%                     1d ago · expected
▲ NPS 47 · +3 vs Q2                          weekly survey
· Onboarding completion 82%                  today

§ STATUS
Uptime (30d)            99.98%
Open incidents          0
Mean time to resolve    12 min

╭──────────────────────────────╮
│ // note                       │
│ Composite health 74 / 100,    │
│ stable week-over-week.        │
│ No critical incidents.        │
╰──────────────────────────────╯
```

The signal markers carry semantic meaning:
- `▲` (filled triangle up, `--positive`): positive change
- `▼` (filled triangle down, `--negative`): negative change
- `●` (filled circle, `--accent`): alert / requires attention
- `·` (filled circle, `--ink-3`): neutral status update

The pulse dot on the SIGNALS heading animates continuously (2.4s cadence) as long as the feed is connected.

---

## § 03 — Cohorts

### Layout

```
┌──────────────────────────────────────────────────────────────┐
│ + § 03 / 03  COHORTS  ──────────────────────────────  30D    │
├──────────────────────────────────────────────────────────────┤
│ Cohorts                                                      │
│ Period-over-period movement by segment…                      │
│ ────────────────────────────────────────────────────────────│
│                                                              │
│ ┌──────────────────────────┬──────────────────────────┐     │
│ │ Retention by Segment     │ Per-Account MRR          │     │  ← Row 1: 1fr + 1fr
│ │ (slopegraph, Q2→Q3)      │ (strip plot, 60 dots)    │     │
│ └──────────────────────────┴──────────────────────────┘     │
│ ┌──────────────────────────────────────────────────────┐    │
│ │ Cohort Retention                                     │    │  ← Row 2: full width
│ │ (6-month × 7-month retention table)                  │    │
│ └──────────────────────────────────────────────────────┘    │
└──────────────────────────────────────────────────────────────┘
```

### Card 03.A — Retention by Segment

| Slot | Content |
|---|---|
| Title | "Retention by Segment" |
| Meta | `Q2 → Q3` |
| Hero metric | `6` |
| Unit | `segments` (rendered display-weight, large) |
| Chart | Slopegraph, 6 segments, Before/After |
| Annotation | `[ rising slopes — improvement ]` / `[ falling slopes — regression ]` |
| Footer | `[ Cohort model · weekly ]` |

**Chart spec.** Two vertical columns (BEFORE on left, AFTER on right). Six dots in each column connected by lines.

- Column headers: `BEFORE` and `AFTER` in mono caps, centered above their dot column.
- Left labels (segment + value): mono 9px, right-aligned, tight to the dot.
- Right labels: mono 9px, left-aligned, tight to the dot.
- Dots: 2.6px radius, `--ink`.
- Slope lines: 1px stroke, `--ink`.
- Footer annotation: two short bracketed callouts in the lower right.

Segments and values:
| Segment | Before | After | Slope |
|---|---|---|---|
| EDU | 62 | 78 | up |
| ENT | 58 | 70 | up |
| MID-MKT | 48 | 62 | up |
| (cross-line) | 48 | 52 | up (shallow) |
| SMB | 34 | 44 | up |
| PUBLIC | 30 | 36 | up |

### Card 03.B — Per-Account MRR

| Slot | Content |
|---|---|
| Title | "Per-Account MRR" |
| Meta | `60 ACCTS` |
| Hero metric | `$50` |
| Unit | `median` |
| Chart | One-axis strip plot, 60 jittered dots |
| Footer | `[ Billing · updated 1h ago ]` |

**Chart spec.** A horizontal axis from 0 to 10.0 (representing scaled MRR distribution). Sixty dots clustered around 5.0 with normal-ish jitter, both horizontal (around the value) and vertical (band ~90px tall). A 1px dashed vertical `--accent` line marks the median position.

Tick labels: `0`, `2.5`, `5.0`, `7.5`, `10.0` along the bottom in mono caps.

### Card 03.C — Cohort Retention

| Slot | Content |
|---|---|
| Title | "Cohort Retention" |
| Meta | `BY SIGNUP MONTH · % RETAINED` |
| Hero metric | `63%` |
| Unit | `avg m3` |
| Chart | 6-row × 7-column retention grid |
| Footer | `[ Cohort model · weekly ]` |

**Grid spec.** Six rows (JUL through DEC, top to bottom) × seven columns (M0 through M6, left to right). Each cell:
- 32px tall, equal-width columns within the available width
- Mono 10.5px text, value displayed inside
- Background tinted by retention value:
  - `v ≥ 80`: `--accent` blend, 30–85% opacity, white text
  - `v ≥ 60`: `--ink` blend at moderate opacity, surface-color text
  - `v < 60`: `--ink` blend at lower opacity, `--ink` text
- Missing cells (cohorts that haven't aged into that month): empty, no background, no text

Row labels (mono 9.5px, tracking +0.14em): JUL, AUG, SEP, OCT, NOV, DEC.
Column headers: M0, M1, M2, M3, M4, M5, M6.

Data:
| | M0 | M1 | M2 | M3 | M4 | M5 | M6 |
|---|---|---|---|---|---|---|---|
| JUL | 100 | 82 | 71 | 64 | 58 | 54 | 51 |
| AUG | 100 | 80 | 69 | 62 | 56 | 52 | — |
| SEP | 100 | 83 | 73 | 66 | 60 | — | — |
| OCT | 100 | 79 | 68 | 61 | — | — | — |
| NOV | 100 | 81 | 70 | — | — | — | — |
| DEC | 100 | 84 | — | — | — | — | — |

### Right rail — § 03

```
§ SEGMENT LEADERBOARD
Q2 → Q3 RETENTION Δ

01  SMB              ▲ +28
02  Mid-Market       ▲ +22
03  Public Sector    ▲ +22
04  Education        ▲ +16
05  Enterprise       ▼ −12
06  Trial            ▼ −22

§ COHORT SIZE
1,034 ACCTS

SMB              412
Trial            233
Mid-Market       188
Education        96
Enterprise       64

╭──────────────────────────────╮
│ // note                       │
│ SMB cohorts lead expansion;   │
│ Enterprise retention is       │
│ softening and needs           │
│ attention.                    │
╰──────────────────────────────╯
```

Leaderboard ranks segments by Δ (signed change). Rank index is rendered in mono `--ink-4`. Δ values are signed and color/shape-coded (positive: green triangle up; negative: coral triangle down).

---

## State catalog (applies to every card)

Each card supports the following five states. State transitions are specified in `05-interactions-and-motion.md`.

### Default
The state documented above. All data present, all chart geometry rendered.

### Loading

- Hero metric area: 1 line of skeleton, 60px tall, `--surface-2` background, slow opacity breath (0.4 ↔ 0.6 at 2s cadence).
- Unit and delta: 2 small skeleton lines, ~10px tall, ~80px wide.
- Chart area: solid `--surface-2` rectangle filling the chart slot.
- Title, meta, footer: rendered normally (these come from the schema, not the data).

Loading state persists until both metric value and chart series have arrived. Partial data does not render — either the full card or the skeleton, never a mix.

### Empty

- Hero metric area: rendered as `—` in display weight, `--ink-4` color.
- Unit and delta: omitted.
- Chart area: bracketed callout centered in the chart slot, mono, `--ink-3`: `[ no data in window ]` or a variant fitting the chart type (e.g., `[ no signals in window ]`, `[ no accounts measured ]`).
- Footer: replaced with `[ awaiting source · {source name} ]`.

### Error

- Hero metric area: rendered as `—` in display weight, `--ink-4` color.
- Unit and delta: omitted.
- Chart area: bracketed callout, mono, `--negative`: `[ data unavailable — retry ]`. The word "retry" is rendered as an interactive link (underlined on hover).
- Footer: `[ {source} · error {error code if present} ]`.
- The card border-left changes to `1px solid --negative` for the duration of the error state.

### Stale

A "stale" state is used when data is older than the operator's expected freshness window (e.g., a "live" feed that has not updated in 10× its cadence).

- Card renders default state.
- Footer prepends a `⚠` symbol (rendered in `--negative`) before the source name: `[ ⚠ Stripe · updated 2h ago ]`.

No other state for v1. Success state is identical to default — the dashboard does not celebrate successful loads.

---

## Edge cases

- **Negative metric value as hero.** Render with leading minus sign, no parentheses, no color change to the value itself (the delta carries the sign-color, not the absolute metric).
- **Metric over 4 characters wide.** Reduce display size to 48px, then 40px, before truncating. Truncate with an ellipsis only as a last resort.
- **Card with no annotation in default state.** The annotation slot is simply omitted. Layout reflows without an empty container.
- **Single-data-point chart.** Render the single dot at its position. Replace the annotation with `[ insufficient series — single observation ]`.
- **Real-time value change.** If a metric updates in place, fade out at 100ms, swap, fade in at 100ms. No counting animation.
- **Right-rail overflow.** If the rail content exceeds viewport height, the rail scrolls independently. A 1px hairline appears at the top edge of the rail when scrolled to indicate scroll state.

---

## Print / export (out of v1)

Documented for forward compatibility:
- Theme forces to light on print
- Section status bars convert to full-width section headers
- Right rail content is moved below the main column content per section
- All bracketed mono callouts are preserved

No export endpoint in v1.
