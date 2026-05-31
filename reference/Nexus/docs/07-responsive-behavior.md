# 07 — Responsive Behavior

> Breakpoints, adaptive layouts, and device-specific considerations

---

## Design Context

### Primary Target

NEXUS is designed primarily for **large desktop displays** in control room environments:

- Typical displays: 1920×1080 to 3840×2160 (4K)
- Viewing distance: 0.5m to 3m
- Multiple monitors common
- Touch input rare

### Responsive Strategy

| Priority | Device | Support Level |
|----------|--------|---------------|
| 1 | Desktop (≥1280px) | Full feature support |
| 2 | Large Desktop (≥1920px) | Optimized, primary target |
| 3 | Tablet (768-1279px) | Adapted layout |
| 4 | Mobile (<768px) | Not supported in v1 |

---

## Breakpoints

### Breakpoint Scale

| Token | Value | Description |
|-------|-------|-------------|
| `--bp-sm` | 640px | Small (not used) |
| `--bp-md` | 768px | Tablet portrait |
| `--bp-lg` | 1024px | Tablet landscape |
| `--bp-xl` | 1280px | Desktop minimum |
| `--bp-2xl` | 1536px | Desktop standard |
| `--bp-3xl` | 1920px | Desktop optimal |
| `--bp-4xl` | 2560px | Large display |

### Media Query Pattern

```css
/* Desktop optimal (primary) */
@media (min-width: 1920px) { }

/* Desktop standard */
@media (min-width: 1536px) and (max-width: 1919px) { }

/* Desktop minimum */
@media (min-width: 1280px) and (max-width: 1535px) { }

/* Tablet */
@media (min-width: 768px) and (max-width: 1279px) { }

/* Mobile (show warning) */
@media (max-width: 767px) { }
```

---

## Layout Adaptations

### Page Layout

#### Desktop Optimal (≥1920px)

```
┌─────────────────────────────────────────────────────────────┐
│ Header                                                  80px│
├─────────────────────────────────────────────────────────────┤
│ ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐            │
│ │  Card   │ │  Card   │ │  Card   │ │  Card   │   4-col   │
│ └─────────┘ └─────────┘ └─────────┘ └─────────┘            │
│ ┌───────────────────────────┐ ┌───────────────────────────┐│
│ │                           │ │                           ││
│ │      Panel (1.5fr)        │ │      Panel (1fr)          ││
│ │                           │ │                           ││
│ └───────────────────────────┘ └───────────────────────────┘│
│ ┌─────────────────────────────────────────────────────────┐│
│ │                    Full Width Panel                     ││
│ └─────────────────────────────────────────────────────────┘│
├─────────────────────────────────────────────────────────────┤
│ Footer                                                  48px│
└─────────────────────────────────────────────────────────────┘
```

**Specifications:**
- Page margin: 24px
- Grid gap: 16px
- Panel max-width: None
- Metric cards: 4 columns

#### Desktop Standard (1536px - 1919px)

Same as optimal, minor spacing reduction:
- Page margin: 20px
- Grid gap: 16px

#### Desktop Minimum (1280px - 1535px)

```
┌─────────────────────────────────────────────────────────────┐
│ Header                                                  80px│
├─────────────────────────────────────────────────────────────┤
│ ┌───────────┐ ┌───────────┐ ┌───────────┐ ┌───────────┐    │
│ │   Card    │ │   Card    │ │   Card    │ │   Card    │    │
│ └───────────┘ └───────────┘ └───────────┘ └───────────┘    │
│ ┌─────────────────────────────────────────────────────────┐│
│ │                      Panel (full)                       ││
│ └─────────────────────────────────────────────────────────┘│
│ ┌─────────────────────────────────────────────────────────┐│
│ │                      Panel (full)                       ││
│ └─────────────────────────────────────────────────────────┘│
├─────────────────────────────────────────────────────────────┤
│ Footer                                                  48px│
└─────────────────────────────────────────────────────────────┘
```

**Adaptations:**
- Page margin: 16px
- Grid gap: 12px
- Two-column panels become single column
- Metric cards remain 4 columns

#### Tablet (768px - 1279px)

```
┌────────────────────────────────────────┐
│ Header                             80px│
├────────────────────────────────────────┤
│ ┌──────────────┐ ┌──────────────┐      │
│ │    Card      │ │    Card      │      │
│ └──────────────┘ └──────────────┘      │
│ ┌──────────────┐ ┌──────────────┐      │
│ │    Card      │ │    Card      │      │
│ └──────────────┘ └──────────────┘      │
│ ┌────────────────────────────────────┐ │
│ │          Panel (full)              │ │
│ └────────────────────────────────────┘ │
├────────────────────────────────────────┤
│ Footer                             48px│
└────────────────────────────────────────┘
```

**Adaptations:**
- Page margin: 16px
- Grid gap: 12px
- Metric cards: 2×2 grid
- All panels single column
- Tables may require horizontal scroll
- Network topology simplified or hidden

---

## Component Adaptations

### Header

| Breakpoint | Adaptation |
|------------|------------|
| ≥1920px | Full layout as designed |
| 1536-1919px | Same layout |
| 1280-1535px | Tab labels may truncate |
| 768-1279px | Hamburger menu for tabs |
| <768px | Warning message |

#### Tablet Header

```
┌────────────────────────────────────────┐
│ [☰] [NEXUS]              [SYS] [●]    │
└────────────────────────────────────────┘
       │
       ▼ (drawer)
┌────────────────────────────────────────┐
│  OVERVIEW                              │
│  PRODUCTION                            │
│  ANALYTICS                             │
│  MAINTENANCE                           │
│  SETTINGS                              │
└────────────────────────────────────────┘
```

### Metric Cards

| Breakpoint | Columns | Card Min-Width |
|------------|---------|----------------|
| ≥1280px | 4 | 200px |
| 768-1279px | 2 | 200px |
| <768px | 1 | 100% |

### Tables

| Breakpoint | Behavior |
|------------|----------|
| ≥1536px | All columns visible |
| 1280-1535px | May hide lower-priority columns |
| 768-1279px | Horizontal scroll enabled |
| <768px | Not supported |

**Column Priority (Production Lines):**

| Priority | Column | Hide Below |
|----------|--------|------------|
| 1 | Unit | Never |
| 2 | Status | Never |
| 3 | Output | 1024px |
| 4 | Temp | 1280px |
| 5 | Pressure | 1536px |

### Charts

| Breakpoint | Adaptation |
|------------|------------|
| ≥1536px | Full size with all labels |
| 1280-1535px | Reduced padding, fewer grid lines |
| 768-1279px | Hide secondary charts, simplify primary |
| <768px | Not supported |

### Network Topology

| Breakpoint | Behavior |
|------------|----------|
| ≥1280px | Full visualization |
| 768-1279px | Simplified (fewer nodes) or hidden |
| <768px | Hidden |

---

## Typography Scaling

### Font Size Adjustments

| Token | Desktop | Tablet |
|-------|---------|--------|
| `--text-5xl` | 36px | 28px |
| `--text-4xl` | 32px | 24px |
| `--text-3xl` | 24px | 20px |
| `--text-2xl` | 18px | 16px |
| Other sizes | No change | No change |

### Implementation

```css
:root {
  --text-5xl: 36px;
  --text-4xl: 32px;
}

@media (max-width: 1279px) {
  :root {
    --text-5xl: 28px;
    --text-4xl: 24px;
  }
}
```

---

## Spacing Adjustments

### Gap Reduction

| Context | Desktop | Tablet |
|---------|---------|--------|
| Page margin | 24px | 16px |
| Grid gap | 16px | 12px |
| Panel padding | 20px | 16px |
| Section gap | 24px | 16px |

---

## Touch Considerations

### Touch Targets

For tablet support, ensure minimum touch target sizes:

| Element | Min Size | Desktop Size |
|---------|----------|--------------|
| Button | 44×44px | 28px height |
| Table Row | 48px height | 44px height |
| Toggle | 44×24px | 40×20px |
| Tab | 44px height | 44px height |

### Touch-Specific Behaviors

| Interaction | Desktop | Tablet |
|-------------|---------|--------|
| Hover states | Full support | Disabled |
| Right-click | Context menu | Long-press menu |
| Drag & drop | If applicable | Touch drag |
| Tooltips | On hover | On long-press |

---

## Mobile Warning

For viewports <768px, display a warning message:

```
┌────────────────────────────────────────┐
│                                        │
│           [NEXUS Logo]                 │
│                                        │
│  This application is designed for      │
│  desktop displays. Please access       │
│  from a device with a larger screen.   │
│                                        │
│  Minimum recommended: 1280×720         │
│                                        │
└────────────────────────────────────────┘
```

**Implementation:**

```css
.mobile-warning {
  display: none;
}

@media (max-width: 767px) {
  .app-content {
    display: none;
  }
  
  .mobile-warning {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    min-height: 100vh;
    padding: 24px;
    text-align: center;
  }
}
```

---

## High-DPI Displays

### Retina/4K Support

All visual elements should be resolution-independent:

| Element | Approach |
|---------|----------|
| Icons | SVG (inline or sprite) |
| Logos | SVG |
| Charts | SVG or Canvas |
| Images | 2x assets where needed |
| Borders | CSS (scales automatically) |
| Shadows | CSS (scales automatically) |

### Large Display Optimizations

For 4K displays (≥2560px):

```css
@media (min-width: 2560px) {
  :root {
    /* Optionally increase base sizes for readability at distance */
    --text-base: 12px;
    --text-md: 13px;
  }
}
```

---

## Viewport Units

### Full-Height Layout

```css
.app-container {
  min-height: 100vh;
  min-height: 100dvh; /* Dynamic viewport height */
}
```

### Header Fixed Position

```css
.header {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  height: 80px;
  z-index: var(--z-sticky);
}

.main-content {
  padding-top: 80px;
}
```

---

## Container Queries (Progressive Enhancement)

For component-level responsiveness:

```css
.panel {
  container-type: inline-size;
}

@container (max-width: 400px) {
  .panel-content {
    /* Adapt layout for narrow panels */
  }
}
```

---

## Responsive Testing Checklist

### Desktop Sizes

- [ ] 1920×1080 (Full HD) — Primary target
- [ ] 2560×1440 (QHD)
- [ ] 3840×2160 (4K)
- [ ] 1536×864 (Common laptop)
- [ ] 1280×720 (Minimum supported)

### Edge Cases

- [ ] Extremely wide (ultra-wide monitors)
- [ ] Very tall (vertical monitor)
- [ ] Zoom levels (100%, 125%, 150%)
- [ ] Browser dev tools open (reduced width)

### Browser Testing

- [ ] Chrome (latest 2 versions)
- [ ] Firefox (latest 2 versions)
- [ ] Edge (latest 2 versions)
- [ ] Safari (if applicable)
