# 04 — Components

> Complete component library with all states, variants, and specifications

---

## Component Index

| Category | Components |
|----------|------------|
| **Layout** | Panel, Card Grid, Content Grid |
| **Navigation** | Tab Bar, Tab Item |
| **Data Display** | Metric Card, Table, Data Row, Status Indicator, Progress Bar, Health Ring, Chart |
| **Forms** | Toggle Switch, Select Dropdown, Text Input, Button |
| **Feedback** | Alert Item, Toast Notification |
| **Identity** | Logo Mark, Avatar, Badge |

---

## Layout Components

### Panel

Container for grouped content with optional header and action.

**Structure:**
```
┌─────────────────────────────────────────┐
│ [●] PANEL TITLE              [ACTION]   │ ← Header (48px)
├─────────────────────────────────────────┤
│                                         │
│             Content Area                │ ← Body (variable)
│                                         │
└─────────────────────────────────────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Background | `--bg-secondary` (#111113) |
| Border | 1px solid `--border-primary` (#27272a) |
| Border radius | 0 (square corners) |
| Header padding | 16px 20px |
| Body padding | 0 (children manage padding) |
| Header border | 1px solid `--border-primary` bottom |

**Header Elements:**
- Title node: 4px circle, `--text-tertiary`
- Title text: 11px, `--text-secondary`, letter-spacing 2px
- Action button: Ghost button variant

**Top Edge Accent:**
```css
.panel::before {
  content: '';
  position: absolute;
  top: 0;
  left: 20px;
  right: 20px;
  height: 1px;
  background: linear-gradient(90deg, transparent, var(--border-secondary), transparent);
}
```

**States:**

| State | Behavior |
|-------|----------|
| Default | As specified |
| Loading | Content area shows skeleton |
| Empty | Content area shows empty state message |
| Error | Content area shows error message + retry |
| Collapsed | Header only (if collapsible enabled) |

---

### Metric Card

Displays a single KPI with trend indicator and status.

**Structure:**
```
┌─────────────────────────────────────────┐
│ ○                                     ○ │ ← Corner nodes
│   LABEL                                 │
│   847.2                                 │ ← Large value
│   units/hr                              │
│ ─────────────────────────────────────── │
│   +2.4%              ● NOMINAL          │
│ ○                                     ○ │
└─────────────────────────────────────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Background | `--bg-secondary` |
| Border | 1px solid `--border-primary` |
| Padding | 20px |
| Min width | 200px |
| Min height | 140px |

**Typography:**

| Element | Font | Size | Color |
|---------|------|------|-------|
| Label | IBM Plex Mono | 10px | `--text-tertiary` |
| Value | Space Grotesk | 32px | `--text-primary` |
| Unit | IBM Plex Mono | 11px | `--text-tertiary` |
| Trend | IBM Plex Mono | 11px | Dynamic |

**Corner Nodes:**
- Size: 6px diameter
- Position: -3px from corners (overlapping border)
- Fill: `--bg-primary`
- Stroke: 1px `--border-secondary`

**Trend Colors:**
- Positive: `--status-success` (#4ade80)
- Negative: `--status-warning` (#fbbf24)
- Neutral: `--text-tertiary`

**States:**

| State | Behavior |
|-------|----------|
| Default | As specified |
| Loading | Skeleton for value and trend |
| Warning | Border changes to `--status-warning` |
| Critical | Border changes to `--status-danger` |

---

## Navigation Components

### Tab Bar

Horizontal navigation container.

**Specifications:**

| Property | Value |
|----------|-------|
| Height | 44px |
| Background | Transparent |
| Border bottom | 1px solid `--border-primary` |
| Gap | 0 (tabs touch) |

### Tab Item

Individual navigation tab.

**Specifications:**

| Property | Value |
|----------|-------|
| Padding | 12px 24px |
| Font | IBM Plex Mono, 11px |
| Letter spacing | 2px |
| Text transform | Uppercase |

**States:**

| State | Text Color | Indicator |
|-------|------------|-----------|
| Default | `--text-tertiary` | None |
| Hover | `--text-secondary` | None |
| Active | `--text-primary` | Underline 1px |
| Focus | `--text-primary` | Focus ring |
| Disabled | `--text-tertiary` at 50% | None |

**Active Indicator:**
- Position: Absolute bottom
- Height: 1px
- Color: `--text-secondary`
- Width: 100% of tab

**Active Node:**
- Position: 4px from left edge, vertically centered
- Size: 4px circle
- Color: `--text-secondary`
- Opacity: 0 when inactive, 1 when active

---

## Data Display Components

### Status Indicator

Communicates current state of an entity.

**Structure:**
```
● ACTIVE
```

**Specifications:**

| Property | Value |
|----------|-------|
| Dot size | 5px diameter |
| Gap | 6px |
| Font | IBM Plex Mono, 9px |
| Letter spacing | 1px |
| Text transform | Uppercase |

**Status Colors:**

| Status | Dot Color | Glow |
|--------|-----------|------|
| Active | #4ade80 | 0 0 8px #4ade80 |
| Standby | #fbbf24 | 0 0 8px #fbbf24 |
| Maintenance | #f87171 | 0 0 8px #f87171 |
| Nominal | #4ade80 | 0 0 8px #4ade80 |
| Optimal | #22d3ee | 0 0 8px #22d3ee |
| Connected | #4ade80 | 0 0 8px #4ade80 |
| Scheduled | #60a5fa | 0 0 8px #60a5fa |
| Pending | #fbbf24 | 0 0 8px #fbbf24 |
| OK | #4ade80 | 0 0 8px #4ade80 |
| Low | #fbbf24 | 0 0 8px #fbbf24 |
| Critical | #f87171 | 0 0 8px #f87171 |
| High (priority) | #f87171 | 0 0 8px #f87171 |
| Normal (priority) | #4ade80 | 0 0 8px #4ade80 |

---

### Progress Bar

Horizontal bar showing percentage completion.

**Variants:**

#### Thin (Output Bar)
```
████████████░░░░░░░░░░░░
```

| Property | Value |
|----------|-------|
| Height | 3px |
| Width | 60px (fixed) or 100% (fluid) |
| Track color | `--border-primary` |
| Fill color | `--text-secondary` |
| Border radius | 0 |

#### Medium (Shift Progress)
```
████████████████░░░░░░░░░░░░░░
```

| Property | Value |
|----------|-------|
| Height | 4px |
| Width | 100% |
| Track color | `--border-primary` |
| Fill color | `--text-secondary` |
| Border radius | 0 |

#### With Threshold (Inventory)
```
████████████████░░│░░░░░░░░░░░░
                  ▲ threshold marker
```

| Property | Value |
|----------|-------|
| Threshold marker width | 1px |
| Threshold marker height | 11px |
| Threshold marker color | `--status-warning` |
| Threshold marker position | Calculated percentage |

---

### Health Ring

Circular progress indicator for equipment health.

**Structure:**
```
    ┌───────┐
   ╱         ╲
  │    94%    │
   ╲         ╱
    └───────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Outer diameter | 40px (default), scalable |
| Stroke width | 2px (track), 2px (fill) |
| Track color | `--border-primary` |
| Value font | IBM Plex Mono, 10px |
| Value color | `--text-primary` |

**Fill Colors:**

| Range | Color |
|-------|-------|
| ≥80% | #4ade80 (green) |
| 60-79% | #fbbf24 (yellow) |
| <60% | #f87171 (red) |

**SVG Implementation:**
```svg
<svg viewBox="0 0 36 36">
  <!-- Track -->
  <path
    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
    fill="none"
    stroke="#27272a"
    stroke-width="2"
  />
  <!-- Fill -->
  <path
    d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
    fill="none"
    stroke="[color]"
    stroke-width="2"
    stroke-dasharray="[percentage], 100"
    transform="rotate(-90 18 18)"
  />
</svg>
```

---

### Table

Data table with header and rows.

**Header Row:**

| Property | Value |
|----------|-------|
| Background | `--bg-tertiary` |
| Padding | 14px 20px |
| Font | IBM Plex Mono, 10px |
| Color | `--text-tertiary` |
| Letter spacing | 1px |
| Border bottom | 1px solid `--border-primary` |

**Data Row:**

| Property | Value |
|----------|-------|
| Background | `--bg-secondary` |
| Padding | 14px 20px |
| Font | IBM Plex Mono, 11px |
| Color | `--text-secondary` (default column) |
| Border bottom | 1px solid `--border-primary` |

**States:**

| State | Behavior |
|-------|----------|
| Default | As specified |
| Hover | Background `rgba(255,255,255,0.02)` |
| Selected | Left border 2px `--text-secondary` |
| Loading | Skeleton rows |
| Empty | Single row with message centered |

---

### ID Badge

Small identifier badge for items.

**Variants:**

#### Square (Production Line ID)
```
┌─────┐
│ A1  │●
└─────┘
```

| Property | Value |
|----------|-------|
| Size | 28×28px |
| Background | Transparent |
| Border | 1px solid `--border-secondary` |
| Font | IBM Plex Mono, 9px |
| Color | `--text-tertiary` |
| Corner node | 4px circle at top-right |

#### Pill (Batch ID, Work Order)
```
┌──────────┐
│  B-4893  │
└──────────┘
```

| Property | Value |
|----------|-------|
| Height | 28px |
| Padding | 0 8px |
| Background | Transparent |
| Border | 1px solid `--border-secondary` |
| Font | IBM Plex Mono, 9px |
| Color | `--text-secondary` |

---

## Form Components

### Toggle Switch

Binary on/off control.

**Structure:**
```
OFF: [○────────]     ON: [────────●]
```

**Specifications:**

| Property | Value |
|----------|-------|
| Width | 40px |
| Height | 20px |
| Border radius | 10px |
| Knob size | 16px |
| Knob offset | 2px from edge |
| Transition | 0.2s ease |

**States:**

| State | Track Color | Knob Color | Knob Position |
|-------|-------------|------------|---------------|
| Off | `--border-primary` | `--text-tertiary` | Left |
| On | `--accent` (#52525b) | `--text-primary` | Right |
| Disabled | `--border-primary` at 50% | `--text-tertiary` at 50% | Current |
| Focus | Track + focus ring | — | — |

---

### Select Dropdown

Single-selection dropdown control.

**Structure:**
```
┌─────────────────────────┐
│  90 DAYS             ▾  │
└─────────────────────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Height | 32px |
| Min width | 100px |
| Padding | 8px 12px |
| Background | `--bg-tertiary` |
| Border | 1px solid `--border-primary` |
| Font | IBM Plex Mono, 10px |
| Color | `--text-secondary` |
| Arrow | Chevron, `--text-tertiary` |

**States:**

| State | Behavior |
|-------|----------|
| Default | As specified |
| Hover | Border `--border-secondary` |
| Focus | Border `--border-secondary` + focus ring |
| Open | Native dropdown behavior |
| Disabled | Opacity 50%, no interaction |

---

### Text Input

Numeric input with unit suffix.

**Structure:**
```
┌──────────┐
│       85 │ °C
└──────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Height | 32px |
| Width | 60px |
| Padding | 6px 10px |
| Background | `--bg-tertiary` |
| Border | 1px solid `--border-primary` |
| Font | IBM Plex Mono, 11px |
| Color | `--text-primary` |
| Text align | Right |

**Unit Suffix:**
- Position: Right of input, 8px gap
- Font: IBM Plex Mono, 10px
- Color: `--text-tertiary`

**States:**

| State | Behavior |
|-------|----------|
| Default | As specified |
| Focus | Border `--border-secondary` |
| Invalid | Border `--status-danger` |
| Disabled | Opacity 50%, no interaction |

---

### Button

Action triggers.

**Variant: Ghost**

Primary button style throughout the application.

```
┌────────────────┐
│    ACTION      │
└────────────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Height | 28px (default), 24px (small) |
| Padding | 6px 12px |
| Background | Transparent |
| Border | 1px solid `--border-primary` |
| Font | IBM Plex Mono, 10px |
| Color | `--text-tertiary` |
| Letter spacing | 1px |
| Text transform | Uppercase |

**States:**

| State | Border | Color |
|-------|--------|-------|
| Default | `--border-primary` | `--text-tertiary` |
| Hover | `--border-secondary` | `--text-secondary` |
| Active | `--border-secondary` | `--text-primary` |
| Focus | `--border-secondary` + ring | `--text-secondary` |
| Disabled | `--border-primary` at 50% | `--text-tertiary` at 50% |

---

## Feedback Components

### Alert Item

System log entry.

**Structure:**
```
14:32:08  ●  Batch #4892 completed successfully
```

**Specifications:**

| Property | Value |
|----------|-------|
| Padding | 14px 20px |
| Border bottom | 1px solid `--border-primary` |
| Gap | 12px between elements |

**Elements:**

| Element | Font | Color |
|---------|------|-------|
| Time | IBM Plex Mono, 10px | `--text-tertiary` |
| Indicator | 3px circle | Type-specific |
| Message | IBM Plex Mono, 11px | `--text-secondary` |

**Indicator Colors:**

| Type | Color |
|------|-------|
| Info | #60a5fa |
| Warning | #fbbf24 |
| Success | #4ade80 |
| Critical | #f87171 |

**Critical Alert Behavior:**
- Indicator pulses (animation)
- May trigger sound (if enabled)
- May trigger browser notification (if permitted)

---

### Toast Notification

Temporary feedback message.

**Structure:**
```
┌─────────────────────────────────────┐
│ ●  Action completed successfully    │
└─────────────────────────────────────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Position | Top-right, 24px from edges |
| Min width | 300px |
| Max width | 450px |
| Padding | 16px 20px |
| Background | `--bg-secondary` |
| Border | 1px solid `--border-primary` |
| Shadow | 0 4px 12px rgba(0,0,0,0.3) |
| Duration | 5 seconds (auto-dismiss) |

**Variants:**

| Type | Indicator Color |
|------|-----------------|
| Success | #4ade80 |
| Error | #f87171 |
| Warning | #fbbf24 |
| Info | #60a5fa |

**Animation:**
- Enter: Slide in from right, 200ms
- Exit: Fade out, 200ms

---

## Identity Components

### Logo Mark

Brand identifier in header.

**Structure:**
```
    ●
  ┌───┐
● │ ◇ │ ●
  └───┘
    ●
```

**Specifications:**

| Property | Value |
|----------|-------|
| Container size | 36×36px |
| Border | 1px solid `--border-secondary` |
| Inner diamond | 8×8px, rotated 45° |
| Corner nodes | 4px circles at cardinal points |
| Node color | `--node-color` (#e4e4e7) |

### Logo Text

**Specifications:**

| Element | Font | Size | Weight | Spacing |
|---------|------|------|--------|---------|
| NEXUS | Space Grotesk | 18px | 300 | 4px |
| Subtitle | IBM Plex Mono | 10px | 400 | 2px |

---

### Avatar

User identifier.

**Structure:**
```
┌─────┐
│ AU  │  Admin User
│     │  admin@nexus.io
└─────┘
```

**Specifications:**

| Property | Value |
|----------|-------|
| Size | 32×32px |
| Background | `--bg-tertiary` |
| Border | 1px solid `--border-secondary` |
| Font | IBM Plex Mono, 10px |
| Color | `--text-tertiary` |
| Text | First letter of first + last name |

---

## Component States Summary

### Universal States

All interactive components should support:

| State | Trigger | Visual Change |
|-------|---------|---------------|
| Default | None | Base appearance |
| Hover | Mouse over | Subtle highlight |
| Focus | Keyboard focus | Focus ring |
| Active | Mouse down | Pressed appearance |
| Disabled | `disabled` attr | Reduced opacity, no interaction |
| Loading | `loading` attr | Skeleton or spinner |

### Focus Ring Specification

```css
outline: 2px solid var(--border-secondary);
outline-offset: 2px;
```

### Skeleton Loading

Replace content with animated placeholder:
- Background: `--bg-tertiary`
- Animation: Pulse opacity 0.5-1.0 over 1.5s
- Shape: Match expected content shape
