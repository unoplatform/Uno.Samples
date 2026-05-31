# 06 — Theme and Visual System

> Colors, typography, spacing, elevation, borders, shadows, icons, and visual hierarchy

---

## Design Philosophy

### Visual Identity

NEXUS presents as a **technical control interface** — precise, utilitarian, and engineered. The aesthetic draws from:

- Industrial control panels and HMI systems
- Command-line interfaces and terminal UIs
- Aviation and aerospace instrumentation
- Scientific visualization tools

### Art Direction

| Attribute | Expression |
|-----------|------------|
| Tone | Professional, precise, confident |
| Mood | Calm vigilance, controlled alertness |
| Personality | Competent, reliable, no-nonsense |
| Visual density | High information density, clear hierarchy |

### Design Tokens Overview

The system uses semantic tokens that map to specific values. This enables:
- Consistent application across components
- Easy theme modifications
- Clear documentation of intent

---

## Color System

### Background Colors

| Token | Hex | RGB | Usage |
|-------|-----|-----|-------|
| `--bg-primary` | #0a0a0b | 10, 10, 11 | Page background |
| `--bg-secondary` | #111113 | 17, 17, 19 | Panels, cards |
| `--bg-tertiary` | #18181b | 24, 24, 27 | Inputs, headers |
| `--bg-elevated` | #1f1f23 | 31, 31, 35 | Dropdowns, popovers |

### Border Colors

| Token | Hex | RGB | Usage |
|-------|-----|-----|-------|
| `--border-primary` | #27272a | 39, 39, 42 | Default borders |
| `--border-secondary` | #3f3f46 | 63, 63, 70 | Hover, focus, dividers |
| `--border-tertiary` | #52525b | 82, 82, 91 | Active states |

### Text Colors

| Token | Hex | RGB | Usage |
|-------|-----|-----|-------|
| `--text-primary` | #fafafa | 250, 250, 250 | Primary content, values |
| `--text-secondary` | #a1a1aa | 161, 161, 170 | Secondary content |
| `--text-tertiary` | #71717a | 113, 113, 122 | Labels, captions |
| `--text-disabled` | #52525b | 82, 82, 91 | Disabled text |

### Status Colors

| Token | Hex | RGB | Usage |
|-------|-----|-----|-------|
| `--status-success` | #4ade80 | 74, 222, 128 | Active, operational, positive |
| `--status-warning` | #fbbf24 | 251, 191, 36 | Standby, attention, caution |
| `--status-danger` | #f87171 | 248, 113, 113 | Maintenance, critical, negative |
| `--status-info` | #60a5fa | 96, 165, 250 | Informational, scheduled |

### Special Colors

| Token | Hex | Usage |
|-------|-----|-------|
| `--node-color` | #e4e4e7 | Network nodes, diagram points |
| `--accent` | #52525b | Toggle tracks, interactive accents |

### Color Application Rules

1. **Status colors are sacred** — Only use green, yellow, red, blue for their defined semantic meanings
2. **No decorative color** — All color must communicate meaning
3. **Grayscale for structure** — All layout, borders, and non-semantic elements use grayscale
4. **High contrast for values** — Important metrics use `--text-primary` against dark backgrounds

### Contrast Ratios

| Combination | Ratio | WCAG Level |
|-------------|-------|------------|
| `--text-primary` on `--bg-primary` | 19.2:1 | AAA |
| `--text-secondary` on `--bg-primary` | 7.4:1 | AAA |
| `--text-tertiary` on `--bg-primary` | 4.7:1 | AA |
| `--status-success` on `--bg-secondary` | 8.9:1 | AAA |
| `--status-warning` on `--bg-secondary` | 10.1:1 | AAA |
| `--status-danger` on `--bg-secondary` | 5.2:1 | AA |

---

## Typography

### Font Families

| Token | Font Stack | Usage |
|-------|------------|-------|
| `--font-display` | 'Space Grotesk', system-ui, sans-serif | Large values, logo |
| `--font-mono` | 'IBM Plex Mono', 'SF Mono', monospace | UI text, data |

### Font Loading

```html
<link rel="preconnect" href="https://fonts.googleapis.com">
<link href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;500&family=Space+Grotesk:wght@300;400&display=swap" rel="stylesheet">
```

### Type Scale

| Token | Size | Line Height | Usage |
|-------|------|-------------|-------|
| `--text-xs` | 8px | 12px | Footnotes, chart labels |
| `--text-sm` | 9px | 14px | Status labels, badges |
| `--text-base` | 10px | 16px | Body text, table headers |
| `--text-md` | 11px | 18px | Table cells, inputs |
| `--text-lg` | 12px | 18px | Section headers |
| `--text-xl` | 14px | 20px | Panel titles |
| `--text-2xl` | 18px | 24px | Logo text |
| `--text-3xl` | 24px | 32px | Medium values |
| `--text-4xl` | 32px | 40px | Large values |
| `--text-5xl` | 36px | 44px | Hero metrics |

### Font Weights

| Token | Weight | Usage |
|-------|--------|-------|
| `--font-light` | 300 | Display values (Space Grotesk) |
| `--font-regular` | 400 | All other text |
| `--font-medium` | 500 | Emphasis (rare) |

### Letter Spacing

| Token | Value | Usage |
|-------|-------|-------|
| `--tracking-tight` | 0 | Body text |
| `--tracking-normal` | 1px | Labels, buttons |
| `--tracking-wide` | 2px | Panel titles |
| `--tracking-wider` | 3px | Logo subtitle |
| `--tracking-widest` | 4px | Main logo |

### Typography Application

| Element | Font | Size | Weight | Tracking |
|---------|------|------|--------|----------|
| Metric Value | Space Grotesk | 32-36px | 300 | 0 |
| Panel Title | IBM Plex Mono | 11px | 400 | 2px |
| Table Header | IBM Plex Mono | 10px | 400 | 1px |
| Table Cell | IBM Plex Mono | 11px | 400 | 0 |
| Button | IBM Plex Mono | 10px | 400 | 1px |
| Status Label | IBM Plex Mono | 9px | 400 | 1px |
| Body Text | IBM Plex Mono | 10-11px | 400 | 0 |

---

## Spacing System

### Base Unit

All spacing derives from a 4px base unit.

### Spacing Scale

| Token | Value | Usage |
|-------|-------|-------|
| `--space-1` | 4px | Minimal gaps |
| `--space-2` | 8px | Tight spacing |
| `--space-3` | 12px | Default gaps |
| `--space-4` | 16px | Standard padding |
| `--space-5` | 20px | Panel padding |
| `--space-6` | 24px | Section gaps |
| `--space-8` | 32px | Large gaps |
| `--space-10` | 40px | Major sections |
| `--space-12` | 48px | Page margins |

### Common Spacing Patterns

| Pattern | Value |
|---------|-------|
| Panel padding | 20px |
| Panel header padding | 16px 20px |
| Table cell padding | 14px 20px |
| Button padding | 6px 12px |
| Input padding | 6px 10px |
| Grid gap | 16px |
| Card gap | 16px |
| Section gap | 24px |

---

## Layout Grid

### Page Grid

```
┌──────────────────────────────────────────────────────────────┐
│  24px margin                                         24px   │
│  ┌────────────────────────────────────────────────────────┐ │
│  │                                                        │ │
│  │  Content area (100% - 48px)                            │ │
│  │                                                        │ │
│  └────────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────┘
```

### Content Grid

Flexible CSS Grid for content areas:

```css
.content-grid {
  display: grid;
  gap: 16px;
}

/* 4 equal columns */
.grid-4 {
  grid-template-columns: repeat(4, 1fr);
}

/* 3 equal columns */
.grid-3 {
  grid-template-columns: repeat(3, 1fr);
}

/* 2 columns with ratio */
.grid-2-1 {
  grid-template-columns: 1.5fr 1fr;
}

/* Full width span */
.span-full {
  grid-column: 1 / -1;
}
```

---

## Elevation & Depth

### Shadow Scale

NEXUS uses minimal shadows — the interface is intentionally flat.

| Token | Value | Usage |
|-------|-------|-------|
| `--shadow-none` | none | Default |
| `--shadow-sm` | 0 2px 4px rgba(0,0,0,0.2) | Subtle lift |
| `--shadow-md` | 0 4px 12px rgba(0,0,0,0.3) | Dropdowns, toasts |
| `--shadow-lg` | 0 8px 24px rgba(0,0,0,0.4) | Modals |

### Z-Index Scale

| Token | Value | Usage |
|-------|-------|-------|
| `--z-base` | 0 | Default content |
| `--z-dropdown` | 100 | Dropdowns, select menus |
| `--z-sticky` | 200 | Sticky headers |
| `--z-overlay` | 300 | Overlay backgrounds |
| `--z-modal` | 400 | Modal dialogs |
| `--z-toast` | 500 | Toast notifications |
| `--z-tooltip` | 600 | Tooltips |

---

## Borders & Radius

### Border Widths

| Token | Value | Usage |
|-------|-------|-------|
| `--border-width-thin` | 1px | Default borders |
| `--border-width-medium` | 2px | Focus states, emphasis |

### Border Radius

| Token | Value | Usage |
|-------|-------|-------|
| `--radius-none` | 0 | Default (square corners) |
| `--radius-sm` | 2px | Badges, small elements |
| `--radius-md` | 4px | Buttons (if rounded) |
| `--radius-full` | 9999px | Pills, toggles |

**Note:** NEXUS uses square corners (`radius-none`) for most components to reinforce the industrial aesthetic.

---

## Visual Elements

### Node System

Nodes are small circles used as visual anchors throughout the interface.

**Node Sizes:**

| Size | Diameter | Usage |
|------|----------|-------|
| XS | 2px | Row indicators, subtle accents |
| SM | 3px | Status indicators |
| MD | 4px | Active tab indicators |
| LG | 5px | Header status |
| XL | 6px | Corner nodes on cards |

**Node Styles:**

| Style | Fill | Stroke |
|-------|------|--------|
| Solid | Color | None |
| Hollow | Transparent | 1px color |
| Glow | Color | Box-shadow glow |

### Grid Pattern

Background grid pattern for visual depth:

```css
.grid-background {
  background-image: 
    linear-gradient(rgba(39, 39, 42, 0.3) 1px, transparent 1px),
    linear-gradient(90deg, rgba(39, 39, 42, 0.3) 1px, transparent 1px);
  background-size: 40px 40px;
}
```

### Gradient Overlays

Subtle radial gradient for visual interest:

```css
.gradient-overlay {
  background: radial-gradient(
    ellipse at 50% 0%,
    rgba(63, 63, 70, 0.15) 0%,
    transparent 70%
  );
}
```

### Top Edge Accent

Horizontal gradient line on panels:

```css
.panel::before {
  content: '';
  position: absolute;
  top: 0;
  left: 20px;
  right: 20px;
  height: 1px;
  background: linear-gradient(
    90deg,
    transparent,
    var(--border-secondary),
    transparent
  );
}
```

---

## Icons

### Icon Style

NEXUS uses minimal iconography. Where icons appear:

- **Style:** Line icons, 1.5px stroke
- **Size:** 16px default, 12px small, 20px large
- **Color:** Inherits from text color

### Icon Set

| Icon | Usage |
|------|-------|
| Chevron Down | Dropdowns, collapsibles |
| Plus | Add actions |
| X | Close, remove |
| Check | Success, confirmation |
| Alert Triangle | Warnings |
| Info | Information |

### Icon Implementation

Prefer inline SVG for color inheritance:

```html
<svg width="16" height="16" viewBox="0 0 16 16" fill="none" stroke="currentColor" stroke-width="1.5">
  <path d="M4 6l4 4 4-4"/>
</svg>
```

---

## Visual Hierarchy

### Information Priority

| Level | Treatment | Example |
|-------|-----------|---------|
| 1 (Critical) | Large size, primary color, status color if applicable | Metric values |
| 2 (Important) | Medium size, secondary color | Panel titles, table headers |
| 3 (Standard) | Base size, secondary color | Table data, labels |
| 4 (Supporting) | Small size, tertiary color | Captions, timestamps |
| 5 (Ambient) | Minimal size, muted color | Footer info, version |

### Emphasis Techniques

| Technique | Usage |
|-----------|-------|
| Size increase | Primary values, headers |
| Color (semantic) | Status indication only |
| Color (brightness) | Active vs. inactive states |
| Weight | Reserved for logo only |
| Spacing | Group related elements |
| Borders | Define regions, not emphasis |

### Do's and Don'ts

**Do:**
- Use size to establish hierarchy
- Use spacing to group related items
- Use status colors only for status
- Use muted colors for supporting info

**Don't:**
- Use bold text for emphasis (except logo)
- Use decorative colors
- Use shadows for emphasis
- Use borders excessively

---

## Theme Tokens (Complete)

```css
:root {
  /* ===== COLORS ===== */
  
  /* Backgrounds */
  --bg-primary: #0a0a0b;
  --bg-secondary: #111113;
  --bg-tertiary: #18181b;
  --bg-elevated: #1f1f23;
  
  /* Borders */
  --border-primary: #27272a;
  --border-secondary: #3f3f46;
  --border-tertiary: #52525b;
  
  /* Text */
  --text-primary: #fafafa;
  --text-secondary: #a1a1aa;
  --text-tertiary: #71717a;
  --text-disabled: #52525b;
  
  /* Status */
  --status-success: #4ade80;
  --status-warning: #fbbf24;
  --status-danger: #f87171;
  --status-info: #60a5fa;
  
  /* Special */
  --node-color: #e4e4e7;
  --accent: #52525b;
  
  /* ===== TYPOGRAPHY ===== */
  
  /* Fonts */
  --font-display: 'Space Grotesk', system-ui, sans-serif;
  --font-mono: 'IBM Plex Mono', 'SF Mono', monospace;
  
  /* Sizes */
  --text-xs: 8px;
  --text-sm: 9px;
  --text-base: 10px;
  --text-md: 11px;
  --text-lg: 12px;
  --text-xl: 14px;
  --text-2xl: 18px;
  --text-3xl: 24px;
  --text-4xl: 32px;
  --text-5xl: 36px;
  
  /* Weights */
  --font-light: 300;
  --font-regular: 400;
  --font-medium: 500;
  
  /* Tracking */
  --tracking-tight: 0;
  --tracking-normal: 1px;
  --tracking-wide: 2px;
  --tracking-wider: 3px;
  --tracking-widest: 4px;
  
  /* ===== SPACING ===== */
  --space-1: 4px;
  --space-2: 8px;
  --space-3: 12px;
  --space-4: 16px;
  --space-5: 20px;
  --space-6: 24px;
  --space-8: 32px;
  --space-10: 40px;
  --space-12: 48px;
  
  /* ===== BORDERS ===== */
  --border-width-thin: 1px;
  --border-width-medium: 2px;
  --radius-none: 0;
  --radius-sm: 2px;
  --radius-md: 4px;
  --radius-full: 9999px;
  
  /* ===== SHADOWS ===== */
  --shadow-none: none;
  --shadow-sm: 0 2px 4px rgba(0,0,0,0.2);
  --shadow-md: 0 4px 12px rgba(0,0,0,0.3);
  --shadow-lg: 0 8px 24px rgba(0,0,0,0.4);
  
  /* ===== Z-INDEX ===== */
  --z-base: 0;
  --z-dropdown: 100;
  --z-sticky: 200;
  --z-overlay: 300;
  --z-modal: 400;
  --z-toast: 500;
  --z-tooltip: 600;
}
```

---

## Light Theme (Future)

NEXUS v1 is dark theme only. If a light theme is added:

### Light Theme Considerations

| Aspect | Approach |
|--------|----------|
| Backgrounds | Invert: light grays for surfaces |
| Text | Dark grays for readability |
| Status colors | May need saturation adjustment |
| Shadows | More visible, may need reduction |
| Ambient animations | May need opacity adjustment |

### Token Overrides

```css
[data-theme="light"] {
  --bg-primary: #f4f4f5;
  --bg-secondary: #ffffff;
  --bg-tertiary: #e4e4e7;
  --border-primary: #d4d4d8;
  --border-secondary: #a1a1aa;
  --text-primary: #18181b;
  --text-secondary: #3f3f46;
  --text-tertiary: #71717a;
}
```
