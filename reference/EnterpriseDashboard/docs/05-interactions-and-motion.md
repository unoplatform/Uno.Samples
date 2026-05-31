# 05 — Interactions and Motion

This file specifies behavior over time: how interactions feel, when transitions fire, and the timing tokens that keep motion coherent.

## Motion principle

Motion is functional. Every transition communicates state — load completion, focus change, theme switch, hover acknowledgement. No motion is decorative. No element animates because animation looks good in isolation.

Three rules:

1. **Confirm state, do not announce it.** A successful refresh confirms with a 200ms fade-in of fresh data and a 3-second toast. It does not bounce, sparkle, or play a sound.
2. **Restrain duration.** Most transitions are 200ms or less. Anything over 350ms must justify itself (theme cross-fade, page load).
3. **Honor `prefers-reduced-motion`.** Every animation specced here has a reduced-motion equivalent. The default reduced-motion behavior: replace transitions with instant state changes; replace continuous animations (pulse, skeleton breath) with static states.

## Motion tokens

### Duration

| Token | Duration | Use |
|---|---|---|
| `motion-instant` | 0ms | Immediate state change; reduced-motion fallback |
| `motion-fast` | 120ms | Small UI state changes (hover color, focus ring) |
| `motion-base` | 200ms | Most transitions (toggle, button press, toast enter) |
| `motion-slow` | 350ms | Page-level transitions (view switch, theme cross-fade) |
| `motion-deliberate` | 600ms | High-payload transitions (refresh icon spin) |
| `motion-pulse` | 2400ms | Live indicator pulse (one cycle) |
| `motion-breath` | 2000ms | Skeleton opacity breath (one cycle) |

### Easing

| Token | Curve | Use |
|---|---|---|
| `ease-standard` | `cubic-bezier(0.4, 0, 0.2, 1)` | Most transitions in both directions |
| `ease-decel` | `cubic-bezier(0, 0, 0.2, 1)` | Element entering view |
| `ease-accel` | `cubic-bezier(0.4, 0, 1, 1)` | Element leaving view |
| `ease-linear` | `linear` | Continuous loops (pulse, breath, spin) |

Use `ease-standard` by default. Reach for `ease-decel` / `ease-accel` only for entrance / exit pairs that benefit from the asymmetric curve.

## Interaction catalog

### Hover

The dashboard has a small hover vocabulary. Hover never introduces new content; it only acknowledges the cursor.

| Surface | Property changed | Token | Notes |
|---|---|---|---|
| `RailButton` (inactive) | `color: --ink-3 → --ink-2` | `motion-fast` | |
| `RailButton` (active) | no change | — | Already at full strength |
| `RangeSelector` option (inactive) | `color: --ink-3 → --ink-2` | `motion-fast` | |
| `IconButton` | `color: --ink-2 → --ink`, `border-color: --border → --ink-4` | `motion-fast` | |
| `ThemeToggle` | same as IconButton | `motion-fast` | |
| `Card` | no hover state in v1 | — | Drill-down adds one later |
| Chart point (hover) | radius `2px → 3px`, color `--ink-2 → --ink` | `motion-fast` | Charts with selectable points |
| `SignalItem` | no hover state in v1 | — | Read-only |

No box-shadow appears on hover anywhere. No background-color change on hover anywhere except `RangeSelector` active state.

### Focus

Keyboard focus is visually distinct from hover. Every interactive surface has a `:focus-visible` style.

**Default focus ring**: 1px `--accent` outline, 2px offset. Rendered via `box-shadow` to avoid layout shift:
```
box-shadow: 0 0 0 2px var(--bg), 0 0 0 3px var(--accent);
```

This creates a 1px gap between the element and the ring (the inner shadow is the background color), then a 1px accent ring.

| Surface | Focus treatment |
|---|---|
| `RailButton` | Default ring |
| `RangeSelector` option | Default ring |
| `IconButton` | Default ring |
| `ThemeToggle` | Default ring |
| `Card` (when drill-down lands) | Default ring on the entire card |

Focus moves with Tab in document order. See `08-accessibility.md` for the full keyboard map.

### Pressed / active

Pressed state is brief and visual-only.

| Surface | Pressed |
|---|---|
| `IconButton` | `border-color: --ink-3`, no color shift |
| `RangeSelector` option (during selection) | Background animates from current to next color over `motion-base` |
| `ThemeToggle` | Theme cross-fade begins on press, not on release |

### Disabled

| Property | Value |
|---|---|
| Opacity | 0.4 |
| Cursor | not-allowed |
| Pointer events | none |
| Hover transition | none |

Disabled state is rare. The only documented use is a RangeSelector option whose data is unavailable for that range.

## Page-level transitions

### Initial load

When the dashboard mounts:

1. Shell renders. Background is `--bg`. Header skeleton + section bar render immediately (these come from the schema).
2. Section title + description fade in at `motion-slow`, ease-decel. No translate, no stagger between title and description.
3. Cards render their frames immediately (border-fill grid is structural).
4. Inside each card: skeleton state for metric and chart until data arrives.
5. When data arrives, the skeleton elements cross-fade to their populated counterparts at `motion-base`, ease-standard. Cards do not stagger relative to each other — they each transition as their data arrives, which produces a natural cascade without artificial stagger.

Right rail follows the same pattern, independently.

### Section switch (rail icon click)

1. On click, the active state on the rail button updates instantly.
2. The main column content cross-fades: old section fades out at `motion-base` ease-accel, new section fades in at `motion-base` ease-decel, with a 50ms overlap.
3. New section enters from `transform: translateY(4px)` to `translateY(0)` over the same duration. This is the only translate in the system and is intentionally subtle.
4. Right rail content swaps in coordination — same timing, no translate.
5. Scroll position resets to top on the new section.

```css
.view {
  display: none;
}
.view.active {
  display: block;
  animation: section-enter 350ms cubic-bezier(0, 0, 0.2, 1);
}
@keyframes section-enter {
  from { opacity: 0; transform: translateY(4px); }
  to   { opacity: 1; transform: translateY(0); }
}
```

### Theme cross-fade

When the operator clicks the theme toggle:

1. The toggle's label and icon update instantly to the destination theme.
2. The `data-theme` attribute on root flips.
3. Every property that references a CSS variable transitions over `motion-slow` (350ms), ease-standard.

This is achieved by declaring transition rules on the elements most sensitive to theme change:
```css
body {
  transition: background-color 350ms ease, color 350ms ease;
}
.card, .grid, .rail, .rail-right {
  transition: background-color 350ms ease, border-color 350ms ease;
}
```

Charts must also retheme. SVG strokes and fills referencing CSS variables update automatically. The accent point on the line chart, the median dashed line on the strip plot, and any other accent-colored chart elements all retheme without bespoke animation.

### Refresh

When the operator clicks the refresh icon:

1. The icon spins 360° at `motion-deliberate` (600ms), `ease-standard`. The icon color stays `--ink` for the duration.
2. While spinning, every metric value receives a subtle dim (opacity 0.6) for the spin duration.
3. On spin completion, metric values cross-fade to their new values at `motion-base`, ease-standard. Old value fades out, new value fades in — no number-counting animation.
4. A toast appears at bottom-center: `[ refreshed — HH:MM:SS ]`. Toast enters at `motion-base` ease-decel, exits at `motion-base` ease-accel after 3000ms.

If refresh fails, the toast tone switches to negative: `[ refresh failed — retry ]` with a 1px `--negative` left border. Toast duration extends to 5000ms.

## Continuous animations

### Live pulse

The pulse on the Signals heading conveys live data connection.

```css
@keyframes pulse {
  0%   { box-shadow: 0 0 0 0 rgba(125, 184, 255, 0.45); }
  70%  { box-shadow: 0 0 0 7px rgba(125, 184, 255, 0); }
  100% { box-shadow: 0 0 0 0 rgba(125, 184, 255, 0); }
}
.pulse {
  width: 5px; height: 5px;
  border-radius: 50%;
  background: var(--accent);
  animation: pulse 2400ms infinite;
}
```

Cadence: 2.4s per cycle. The pulse runs only while the feed is connected. If the feed disconnects, the pulse stops at end-of-cycle and the dot fades to `--ink-4`.

### Skeleton breath

```css
@keyframes breath {
  0%, 100% { opacity: 0.4; }
  50%      { opacity: 0.6; }
}
.skeleton {
  animation: breath 2000ms ease-in-out infinite alternate;
}
```

### Active usage heatmap "live" indicator

Card footer reads `[ Events · live ]`. The word `live` carries a small pulsing dot to its right, same animation as the Signals pulse. Cadence 2.4s.

## Micro-interactions

These are small interactions that, individually, are barely noticeable. Collectively they make the dashboard feel calibrated rather than flat.

### Range selector switch
- Active background slides between options? **No.** The active state simply applies to the new option at `motion-base`. No sliding pill.

The reason: sliding pills are a recognizable "modern SaaS" pattern. The motionless swap is more instrument-like and avoids a tropey idiom.

### Chart point hover (forward planning, out of v1)
- On hover, the point grows from radius 2 → 3px, color shifts `--ink-2 → --ink`, at `motion-fast`.
- A small annotation appears above the point with the value, using the `Annotation` component style.
- On leave, the point and annotation reverse at `motion-fast`.

### Real-time value change
- When a metric value updates while the dashboard is open (no refresh action):
  1. Old value's opacity transitions from 1 → 0 over 100ms.
  2. Text content swaps.
  3. New value's opacity transitions from 0 → 1 over 100ms.
- No counting animation. The transition is honest: the value changed, it didn't tween.

### Toast stack
- New toasts push older toasts upward by their height + 8px gap.
- Push transition: `motion-base`, ease-standard.

## Reduced motion

When `prefers-reduced-motion: reduce` is set:

| Animation | Reduced behavior |
|---|---|
| Page load fade | Instant render |
| Section switch | Instant content swap (no fade, no translate) |
| Theme cross-fade | Instant theme swap |
| Refresh spin | Icon does not spin; replaced by 0.6 opacity flash for 200ms |
| Live pulse | Static dot at full strength, no pulse |
| Skeleton breath | Static at 0.5 opacity |
| Hover transitions | Apply instantly, no transition curve |
| Toast enter / exit | Instant appear / disappear |

This is implemented at the root:
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

Then specific affordances are restored where they aid orientation:
- The pulse dot still exists (just static) so its meaning is preserved.
- Toasts still appear (just without animation).

## Scroll behavior

- Smooth scroll is disabled globally. Snap-scrolling between sections is not used.
- The main column scrolls; the rails do not scroll with it.
- Right rail has its own independent scroll, with a 1px top hairline that appears in `--border` when scrolled, to signal scroll state.
- No scroll-linked animations. No parallax. No fade-in-on-scroll. The page is the page.

## Touch behavior (tablet and mobile)

See `07-responsive-behavior.md` for layout adaptations. Touch-specific motion notes:

- Tap on rail button: same as click. No long-press behavior.
- Tap on a chart (when drill-down lands): same as hover-then-click on desktop.
- Touch scroll: native momentum scroll. No custom inertia.
- Pull-to-refresh: not implemented in v1. If added, it uses the same refresh transition (icon spin, toast confirm).

## Cursor

- Default cursor everywhere.
- Pointer cursor on interactive elements (RailButton, RangeSelector, IconButton, ThemeToggle, Toast close).
- `not-allowed` cursor on disabled elements.
- Crosshair cursor on chart areas where drill-down lands (forward planning).

## Animation review checklist

For any new animation proposed in the future, the answer to all five must be yes:

1. Does it communicate a state change the operator would otherwise miss?
2. Is its duration ≤ 350ms (or justified above that)?
3. Does it have a reduced-motion equivalent?
4. Does it use one of the documented motion tokens (no bespoke timing)?
5. Does it avoid sliding pills, bouncing elements, gradient washes, or other recognizable AI/SaaS animation tropes?

If any answer is no, the animation does not ship.
