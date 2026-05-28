# 05 — Interactions and Motion

> Animations, transitions, micro-interactions, and timing specifications

---

## Motion Philosophy

### Principles

1. **Functional, not decorative** — Every animation serves a purpose: confirming action, indicating state, or guiding attention
2. **Subtle and swift** — Animations should be nearly imperceptible except when drawing attention to important changes
3. **Non-blocking** — Users should never wait for animations to complete before taking action
4. **Respect the context** — Control room operators need stability; minimize motion that could distract during critical moments

### Performance Targets

| Metric | Target |
|--------|--------|
| Animation frame rate | 60 fps |
| Maximum animation duration | 300ms (most <200ms) |
| Time to interactive | No animation delays user input |

---

## Global Timing

### Duration Scale

| Token | Duration | Use Case |
|-------|----------|----------|
| `--duration-instant` | 0ms | Immediate feedback |
| `--duration-fast` | 100ms | Micro-interactions, hovers |
| `--duration-normal` | 200ms | Standard transitions |
| `--duration-slow` | 300ms | Complex state changes |
| `--duration-emphasis` | 500ms | Attention-grabbing |

### Easing Functions

| Token | Value | Use Case |
|-------|-------|----------|
| `--ease-out` | `cubic-bezier(0, 0, 0.2, 1)` | Elements entering view |
| `--ease-in` | `cubic-bezier(0.4, 0, 1, 1)` | Elements leaving view |
| `--ease-in-out` | `cubic-bezier(0.4, 0, 0.2, 1)` | State changes |
| `--ease-linear` | `linear` | Continuous animations |

---

## Interaction Patterns

### Hover States

**Timing:** 100ms ease-out

**Components:**

| Component | Hover Effect |
|-----------|--------------|
| Button | Border color lightens |
| Table Row | Background subtle highlight |
| Tab | Text color lightens |
| Card | Border color lightens |
| Link | Text color lightens |

**CSS Pattern:**
```css
.interactive-element {
  transition: 
    border-color 100ms ease-out,
    background-color 100ms ease-out,
    color 100ms ease-out;
}
```

### Focus States

**Timing:** Immediate (0ms)

Focus states should appear instantly for accessibility. No animation delay.

**CSS Pattern:**
```css
.focusable:focus-visible {
  outline: 2px solid var(--border-secondary);
  outline-offset: 2px;
}
```

### Active/Pressed States

**Timing:** Immediate

**Components:**

| Component | Active Effect |
|-----------|---------------|
| Button | Background darkens slightly |
| Toggle | Knob snaps to position |
| Tab | Text reaches final color |

---

## Component Animations

### Toggle Switch

**Animation:** Knob slides from one position to another

```
OFF → ON:  [○────────] → [────────●]
```

**Specifications:**
- Duration: 200ms
- Easing: ease-out
- Property: `transform: translateX()`

**CSS:**
```css
.toggle-knob {
  transition: transform 200ms ease-out;
}

.toggle.on .toggle-knob {
  transform: translateX(20px);
}
```

### Status Indicator Pulse

**Animation:** Opacity pulses to indicate live status

**Specifications:**
- Duration: 2000ms
- Easing: ease-in-out
- Loop: Infinite
- Range: 1.0 → 0.5 → 1.0

**CSS:**
```css
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}

.status-indicator.live {
  animation: pulse 2s ease-in-out infinite;
}
```

**When to pulse:**
- Connection status in header
- Active/nominal status indicators
- Critical alerts (faster: 1s)

### Network Topology Animation

**Animation:** Dashed lines animate to show data flow

**Specifications:**
- Duration: 20000ms (slow, ambient)
- Easing: linear
- Property: stroke-dashoffset

**SVG/CSS:**
```css
@keyframes dash-flow {
  from { stroke-dashoffset: 0; }
  to { stroke-dashoffset: -20; }
}

.connection-line {
  stroke-dasharray: 4 4;
  animation: dash-flow 5s linear infinite;
}
```

### Node Glow

**Animation:** Subtle glow effect on network nodes

**Specifications:**
- Duration: 2000ms
- Easing: ease-in-out
- Loop: Infinite
- Property: box-shadow or filter

**CSS:**
```css
@keyframes node-glow {
  0%, 100% { filter: drop-shadow(0 0 2px currentColor); }
  50% { filter: drop-shadow(0 0 6px currentColor); }
}

.network-node {
  animation: node-glow 2s ease-in-out infinite;
}
```

### Health Ring Fill

**Animation:** Ring fills from 0 to target percentage on load

**Specifications:**
- Duration: 500ms
- Easing: ease-out
- Property: stroke-dasharray
- Trigger: On component mount or value change

**CSS:**
```css
.health-ring-fill {
  stroke-dasharray: 0 100;
  transition: stroke-dasharray 500ms ease-out;
}

.health-ring-fill.loaded {
  stroke-dasharray: 94 100; /* 94% health */
}
```

---

## Page Transitions

### Tab Navigation

**Behavior:** Instant content swap, no transition

**Rationale:** Control room users need immediate access to information. Page transitions add unnecessary delay.

```
[Click Tab] → [Content immediately visible]
```

**Implementation:**
- No fade or slide animations between pages
- Content renders immediately
- Scroll position resets to top

### URL Navigation (Browser Back/Forward)

**Behavior:** Same as tab navigation — instant

---

## Loading States

### Skeleton Animation

**Animation:** Pulsing opacity on placeholder content

**Specifications:**
- Duration: 1500ms
- Easing: ease-in-out
- Loop: Infinite
- Range: 0.5 → 1.0 → 0.5

**CSS:**
```css
@keyframes skeleton-pulse {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 1; }
}

.skeleton {
  background: var(--bg-tertiary);
  animation: skeleton-pulse 1.5s ease-in-out infinite;
}
```

### Inline Spinner

**Animation:** Rotating spinner for inline loading

**Specifications:**
- Duration: 1000ms
- Easing: linear
- Loop: Infinite
- Size: 16px default

**CSS:**
```css
@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

.spinner {
  width: 16px;
  height: 16px;
  border: 2px solid var(--border-primary);
  border-top-color: var(--text-secondary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}
```

---

## Feedback Animations

### Toast Notification

**Enter Animation:**
- Direction: Slide in from right
- Duration: 200ms
- Easing: ease-out

**Exit Animation:**
- Effect: Fade out
- Duration: 200ms
- Easing: ease-in

**CSS:**
```css
@keyframes toast-enter {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes toast-exit {
  from { opacity: 1; }
  to { opacity: 0; }
}

.toast.entering {
  animation: toast-enter 200ms ease-out;
}

.toast.exiting {
  animation: toast-exit 200ms ease-in;
}
```

**Auto-dismiss timing:**
- Success: 5 seconds
- Error: Manual dismiss required
- Warning: 8 seconds
- Info: 5 seconds

### Button Click Feedback

**Animation:** Subtle scale + background change

**Specifications:**
- Duration: 100ms
- Scale: 0.98 on active

**CSS:**
```css
.button:active {
  transform: scale(0.98);
  background: rgba(255, 255, 255, 0.02);
}
```

### Form Validation

**Invalid State:**
- Border color changes to red (instant)
- Error message fades in (100ms)

**Valid State:**
- Border returns to normal (instant)
- Error message fades out (100ms)

---

## Data Update Animations

### Value Change

**Animation:** Quick fade for changing values

**Specifications:**
- Duration: 150ms
- Easing: ease-out
- Trigger: Value prop changes

**Pattern:**
1. Fade out old value (75ms)
2. Update value
3. Fade in new value (75ms)

**Alternative (preferred for metrics):**
- No animation
- Value updates instantly
- Rationale: Real-time data should feel immediate

### Progress Bar Update

**Animation:** Width transition on value change

**Specifications:**
- Duration: 300ms
- Easing: ease-out

**CSS:**
```css
.progress-fill {
  transition: width 300ms ease-out;
}
```

### New Alert Arrival

**Animation:** Highlight then settle

**Specifications:**
1. New row slides in from top (200ms)
2. Background briefly highlights (300ms)
3. Fades to normal (200ms)

**CSS:**
```css
@keyframes new-alert {
  0% {
    transform: translateY(-100%);
    background: rgba(255, 255, 255, 0.1);
  }
  30% {
    transform: translateY(0);
    background: rgba(255, 255, 255, 0.1);
  }
  100% {
    transform: translateY(0);
    background: transparent;
  }
}

.alert-item.new {
  animation: new-alert 700ms ease-out;
}
```

---

## Scroll Behavior

### Page Scroll

**Behavior:** Native scroll, no momentum modification

### Smooth Scroll (Anchors)

**Behavior:** Not used — instant navigation preferred

### Scroll Shadows

**Animation:** Shadows appear when content overflows

**Specifications:**
- Fade in: 100ms
- Opacity: 0 → 1 based on scroll position

**CSS:**
```css
.scroll-container {
  --shadow-opacity: 0;
}

.scroll-container.has-overflow-top {
  --shadow-opacity: 1;
}

.scroll-container::before {
  content: '';
  position: sticky;
  top: 0;
  height: 1px;
  background: linear-gradient(
    to bottom,
    rgba(0, 0, 0, 0.3),
    transparent
  );
  opacity: var(--shadow-opacity);
  transition: opacity 100ms ease-out;
}
```

---

## Reduced Motion

### Support

Respect user preference for reduced motion:

```css
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

### Exceptions

Some animations remain even with reduced motion:
- Status indicator colors (no motion, just color)
- Focus states (immediate, no motion)

---

## Animation Inventory

| Animation | Component | Duration | Trigger |
|-----------|-----------|----------|---------|
| Pulse | Status Indicator | 2000ms | Always (live) |
| Dash Flow | Network Lines | 20000ms | Always |
| Node Glow | Network Nodes | 2000ms | Always |
| Knob Slide | Toggle | 200ms | User toggle |
| Ring Fill | Health Ring | 500ms | Mount/Update |
| Skeleton | Loading States | 1500ms | Loading |
| Spin | Spinner | 1000ms | Loading |
| Toast Enter | Toast | 200ms | Show |
| Toast Exit | Toast | 200ms | Hide |
| New Alert | Alert Item | 700ms | New alert |
| Hover | Interactive | 100ms | Mouse enter |

---

## Motion Tokens Summary

```css
:root {
  /* Durations */
  --duration-instant: 0ms;
  --duration-fast: 100ms;
  --duration-normal: 200ms;
  --duration-slow: 300ms;
  --duration-emphasis: 500ms;
  
  /* Ambient (looping) */
  --duration-pulse: 2000ms;
  --duration-flow: 20000ms;
  
  /* Easings */
  --ease-out: cubic-bezier(0, 0, 0.2, 1);
  --ease-in: cubic-bezier(0.4, 0, 1, 1);
  --ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);
  --ease-linear: linear;
}
```
