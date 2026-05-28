# 02 — Information Architecture

## Section model

EnterpriseDashboard (codename: Observatory) is a single application surface organized as three sections. The sections are peer-level: there is no section nesting, no sub-navigation, no breadcrumb hierarchy beyond `§ NN / 03`.

| § | Section | Domain | Primary question it answers |
|---|---|---|---|
| 01 | Acquisition & Revenue | Money in, signups, MRR trajectory | "Is the business growing?" |
| 02 | Engagement & Health | Active usage, goals, composite health | "Are accounts using it?" |
| 03 | Cohorts | Period-over-period segmentation, retention | "Which segments are working?" |

### Why three sections, in this order

The order mirrors the operator's natural reading: top-of-funnel and revenue first (the lagging financial truth), then engagement (the leading behavioral indicator), then cohorts (the diagnostic layer that explains the first two). An operator can stop after section 1 and have a useful read. Sections 2 and 3 add resolution.

### Section atomicity

Each section is one scroll-page worth of dashboard. No section requires horizontal scroll, no section requires more than two screens of vertical scroll on a standard 1440×900 desktop. If a section accumulates more cards than fits, the cards are pruned, not extended into pagination.

## Navigation model

### Primary navigation — the left rail

A 72px vertical rail anchored to the viewport's left edge, always visible on desktop and tablet.

```
┌──┐
│◇ │  ← brand mark (decorative, not interactive)
│  │
│📊│  ← § 01 Acquisition & Revenue
│♥ │  ← § 02 Engagement & Health  (heart = "health")
│⋮⋮│  ← § 03 Cohorts (four-dot grid = "groups")
│  │
│  │
│☰ │  ← settings (anchored bottom)
│v1│  ← version label
└──┘
```

Active section indicator: a 1px vertical bar in `--accent`, 18px tall, flush to the left edge of the rail beside the active icon. Inactive icons are `--ink-3`; hovered icons are `--ink-2`; active icons are `--ink`.

Why a single bar and not a filled background: filled backgrounds compete visually with the cards. A bar is a tick mark — instrument-appropriate.

### Section navigation behavior

- Click rail icon → switch section.
- No URL routing in scope; if added later, slugs are `/acquisition`, `/engagement`, `/cohorts`.
- Section switch is an in-place content swap (see `05-interactions-and-motion.md` for the transition).
- The right rail content swaps in coordination with the section (each section has a distinct right rail composition — see below).

### Secondary navigation — none

There are no tabs within sections, no sub-pages, no modal stacks. Drill-down from a card opens an inline drawer pattern (specced in `04-components.md` under `CardDrawer`) if needed, but the v1 dashboard does not require drill-down.

## Context rail (right side)

A 360px sidebar that swaps content with the active section. It is not navigation — it is *context*: aggregations, leaderboards, signals, and a narrative one-sentence note relevant to the active section.

| § | Right-rail content (top to bottom) |
|---|---|
| 01 | Revenue Breakdown (KV list) → Movement (signed KV list) → Note |
| 02 | Signals (live feed, time-ordered) → Status (uptime, incidents, MTTR) → Note |
| 03 | Segment Leaderboard (ranked, Δ-scored) → Cohort Size (KV list) → Note |

The right rail is always sticky, always scrolls independently of the main column, never collapses on desktop. On smaller viewports it collapses — see `07-responsive-behavior.md`.

### Why context, not navigation, in the right rail

A dashboard with three sections does not need a 360px navigation column. It needs commentary. The right rail is the place where rolled-up numbers, sorted lists, and the single qualitative note live — material that would clutter the main column if forced into a card.

## Information hierarchy

Within a section, content layers from primary to tertiary:

1. **Section status bar** (highest scan priority, lowest visual weight) — orients the operator.
2. **Section title and one-line description** — sets domain context.
3. **Hero metrics** (large display number per card) — the read.
4. **Charts** — texture for the metric, evidence.
5. **Card meta and footers** (small mono) — provenance, data source, refresh time.
6. **Right-rail context** — supporting numbers.
7. **Right-rail note** — narrative interpretation.

The visual system maps tightly to this hierarchy. The largest type on the screen is always a metric value, not a heading. The most prominent color (accent blue) is always reserved for actionable or live state, not for any of the hierarchy layers.

## Page chrome

### Header (top of every section)

A single header sits above the section content, anchored to the top of the main column (not the page — the rail does not have one).

```
┌─────────────────────────────────────────────────────────────────┐
│ Observatory                                                     │
│ SAAS ANALYTICS · SYNC 2M AGO · REALTIME              [24H][7D][30D][QTD] ⟳ ☾ DARK │
└─────────────────────────────────────────────────────────────────┘
```

- **Wordmark**: `Observatory` in display-weight Bricolage Grotesque, 48px. No tagline. No logo mark adjacent — the rail mark serves.
- **Subtitle**: All-caps mono, `--ink-3`, with `--accent` dots as separators. Three datums: product class, sync state, mode.
- **Range selector**: pill toggle of four options (24H · 7D · 30D · QTD). One always active. Currently active option is accent-filled.
- **Refresh icon**: circular icon button. On click: spin 360° at 600ms, then trigger data refresh, then settle.
- **Theme toggle**: pill-shape button with icon + label (`DARK` or `LIGHT`). On click: cross-fade theme tokens at 350ms.

The header does not scroll with content. It does not stick — it occupies its fixed slot at the top, and the section content scrolls beneath the page padding.

### Footer

None. EnterpriseDashboard has no footer. The card-level footer (`[ Source · updated Nm ago ]`) carries the provenance information that would otherwise live in a page footer.

## Search

Out of scope for v1. The dashboard is a fixed reading surface, not a queryable interface. If search is added later, it lives in the right rail as a global filter, not as a new column.

## Empty section, empty dashboard

If a section has no data (a new account that hasn't connected sources yet):

- Section status bar renders normally.
- Section title and description render normally.
- Each card frame renders with the empty state pattern (`[ awaiting data ]`) at chart position, no metric value, no delta.
- Right rail renders with a single setup-state note: `[ connect a data source to populate this view — see settings ]`.

If the entire account has no data, the dashboard does not redirect to a setup wizard. The dashboard is shown as the dashboard would appear, empty, with a single setup affordance in the right rail of section 01. This preserves orientation — the operator sees what they are configuring toward.

## Drill-down model (forward planning, out of v1 scope)

When drill-down is added in a later version:

- Clicking a card title or the chart body opens an inline panel from the right edge of the card, pushing the card to ~50% width and revealing detail at the remaining width.
- The detail panel never opens as a modal overlay. Modals interrupt scanning.
- The detail panel closes via Escape, an explicit close affordance, or clicking outside the card.

This is documented here only so the v1 component structure does not preclude it.

## Persistence

State that persists across sessions (per-user, client-stored is acceptable):

- Active theme (dark / light)
- Active range (24H / 7D / 30D / QTD)
- Last active section

State that does not persist:

- Scroll position within a section (always returns to top on section switch)
- Hover or transient state

## Information density audit

The dashboard, at standard desktop width, displays:

- Section 01: 6 cards, ~14 displayed numbers, 2 chart annotations, 12 right-rail numbers, 1 note.
- Section 02: 4 cards, ~8 displayed numbers, 7 signal items, 3 status KVs, 1 note.
- Section 03: 3 cards, ~6 displayed numbers, 1 retention grid (42 values), 6 leaderboard rows, 5 size rows, 1 note.

This density is the upper bound for the v1 design. Adding a card to any section without removing one is considered a regression — pruning is part of design discipline. New cards trigger a sectional review.
