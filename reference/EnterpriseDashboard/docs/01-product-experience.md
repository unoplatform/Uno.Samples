# 01 — Product Experience

This file defines the *feel* of EnterpriseDashboard (codename: Observatory): art direction, voice, copy patterns, and the rules that keep the product consistent in moments the visual system does not directly cover.

## Art direction

### Reference points

The aesthetic sits at the intersection of three influences. They are listed as reference, not as templates to mimic.

- **Scientific instruments.** Telescope control panels, oscilloscope readouts, mission-control screens. Square corners, tick marks, monospaced status text, no decoration that does not signal state.
- **Modernist editorial.** Swiss design discipline applied to data display. Generous negative space, hairline rules, restrained typographic hierarchy.
- **High-end developer tools.** Linear, Vercel, Raycast — the discipline these tools bring to UI chrome without losing personality.

### What EnterpriseDashboard is not

EnterpriseDashboard is not a fintech trading terminal (no chromatic alerting, no aggressive density, no warm ambers on black). It is not editorial luxury (no display serifs, no italic pull quotes, no warm cream backgrounds). It is not playful SaaS (no rounded illustrations, no gradient blobs, no friendly mascots).

If a teammate brings a reference that fits one of those modes, push back. The temptation to drift is highest during cross-team review.

### Decorative language

The dashboard borrows a small vocabulary of decorative marks from instrumentation. They are used sparingly, always to mean something.

- **Bracket pairs `[ ]`** wrap technical metadata, chart annotations, card footers, and toast messages. They signal "this is a system readout, not authored copy."
- **Section sigil `§`** prefixes every section label in the status bar (`§ 01 / 03`). It echoes the legal/scientific convention for section reference and gives the navigation a calibrated feel.
- **Crosshair (`+`)** marks the start of each section bar. It is also used as a hover hint on charts with selectable points.
- **Rule lines** are 1px hairlines in `--border-soft`. They separate sections, divide right-rail blocks, and underline status bars. They never appear as decoration without separating two things.
- **Live pulse** is a single dot in the accent color, expanding outward at 2.4s cadence. It appears only at the Signals panel header and any other strictly real-time feed. Used elsewhere it loses meaning.

### What the decoration is *not*

No drop shadows. No gradients (except a single 1px-tall alpha-fade under the cumulative ARR area chart, where it functions as data, not decoration). No glow effects. No rounded card corners beyond the pill-shaped toggles. No icons used as illustration — every icon is functional.

## Voice and tone

### Voice principles

Three principles, in priority order.

**1. Direct.** State the fact. Skip the qualifier.
> Good: "Activation +4 pt vs Q3 target"
> Avoid: "Activation is currently trending slightly above our Q3 target."

**2. Calibrated.** Numbers come with units. Time comes with timestamps. Uncertainty is acknowledged, not hidden.
> Good: "142 ms p95"
> Avoid: "API latency is good"

**3. Quiet.** No exclamation. No celebration. No urgency theater. A 30% drop in weekend engagement is reported in the same register as a routine status update.
> Good: "Weekend engagement −30% · 1d ago · expected"
> Avoid: "⚠️ Weekend engagement crashed!"

### Tone calibration

| Context | Tone | Example |
|---|---|---|
| Section description | Concise statement of what the section covers | "When accounts are active, how they progress against goals, and a composite health read." |
| Card title | Noun phrase, title case, no verbs | "Recurring Revenue", "Per-Account MRR" |
| Card subtitle / meta | All-caps mono, technical | "12 MO", "BY SIGNUP MONTH · % RETAINED" |
| Metric unit | Lowercase mono, short | "mrr", "median", "min median", "peak hour" |
| Annotation in chart | Bracketed technical observation | "[ peak observed sep — primary channel diverges ]" |
| Right rail note | Single sentence, narrative | "Expansion drove 62% of net new MRR this month, led by Scale and Pro upgrades." |
| Signal item | Subject + value or status, then timestamp | "Activation +4 pt vs Q3 target · 2m ago" |
| Toast / system feedback | Bracketed mono | "[ refreshed — 14:22:03 ]" |
| Empty state | Bracketed, neutral | "[ no signals in window ]" |
| Error state | Bracketed, plus retry affordance | "[ data unavailable — retry ]" |

### Numbers and units

Numbers are presented with discipline. The visual system gives them weight; the formatting rules make them readable.

- **Currency**: always with symbol, never with currency code. `$87.3K`, not `87.3K USD`.
- **Abbreviation thresholds**: K above 1,000; M above 1,000,000; B above 1,000,000,000. One decimal place by default (`$87.3K`); zero decimal places if the integer reads cleanly (`$50`); two decimals only when precision matters (`r² = 0.87`).
- **Percentages**: integer by default (`79%`), one decimal when the delta is meaningfully sub-integer (`+12.4%`).
- **Deltas**: always signed (`+4 pt`, `−12`), never parenthesized. Sign-color is paired with shape — positive uses `▲` and `--positive`, negative uses `▼` and `--negative`. Color and shape both carry the signal; either one alone is sufficient for non-color-vision readers.
- **Times**: relative for recent events (`2m ago`, `1h ago`, `3h ago · at risk`); absolute for daily milestones (`today`); 24-hour clock for timestamps (`14:22:03`, never `2:22 PM`).
- **Ranges**: en-dash, not hyphen (`Q2 → Q3` uses an arrow; numeric ranges use `–`).
- **Comparison arrows**: `→` (right arrow) for sequences over time, `Δ` for "change in".
- **Mathematical operators**: rendered as glyphs (`=`, `≈`, `±`, `σ`, `μ`) in mono, never spelled out.

### Forbidden copy patterns

These patterns will be tempting and should be rejected at review.

- Emoji of any kind, including ✓ ✗ ⚠️ 📈 — use the shape-and-color system instead.
- Sentences ending in `!` outside literal quoted user content (which the dashboard does not display).
- The word "amazing", "great", "awesome", "excited", or any adjective that does not describe a measurable property.
- The phrase "Don't worry" or "we'll handle this" or any reassurance copy. Errors are stated, not apologized for.
- Marketing-style invitation copy: "Discover", "Explore", "Unlock".
- "AI-powered" or "intelligent" or "smart" used as a modifier on dashboard functionality.

## Information density and rhythm

EnterpriseDashboard is intentionally dense. Negative space is generous *between* sections and *around* the dashboard frame, but *inside* a card the data packs tightly. This is the inverse of a typical SaaS dashboard which has loose internal density and tight external chrome.

The reading rhythm the operator should experience:

1. Page loads. Eye lands on the section status bar (`§ 01 / 03 — ACQUISITION & REVENUE`).
2. Eye drops to the section title (display-weight) and one-line description.
3. Eye scans across the metric values in row 1 — three numbers, big.
4. Right rail is read as commentary, not as primary data.
5. Operator either scrolls or switches section via the left rail.

This rhythm is preserved across all three sections. Card layouts and chart types differ; the rhythm does not.

## Personality in motion

The dashboard has very little motion. What motion exists is:

- **Functional** — a hover transition, a toggle state change, a theme transition.
- **Honest** — a live pulse on the Signals header that genuinely indicates live data, not decoration.
- **Restrained** — page-load fade-in is 350ms, not staged with confetti-style stagger. No element bounces, oscillates, or pulses unless it is reporting actual state.

If a proposed animation does not communicate state or aid wayfinding, it does not ship. See `05-interactions-and-motion.md` for the full motion system.

## Cultural and localization considerations

- **English-first**, with French as a near-term secondary (Montréal context). Layout must tolerate ~15–20% string expansion without breaking. Mono labels (all caps, tracking) are most likely to break — the box plot row labels (`FREE`, `STARTER`, `PRO`, `TEAM`, `SCALE`) need flex space.
- **Numeric format**: respect locale — `1,284` in English, `1 284` in French (non-breaking space as thousands separator). The implementation must handle both. Currency symbol position remains prefix in both locales for this dashboard.
- **Time zones**: timestamps show in operator's local zone. Display the zone abbreviation in the subtitle when ambiguity is possible (`Sync 2m ago · EST`).
- **No RTL support in v1.** Mark RTL as a non-goal explicitly to prevent half-built RTL during component development.
