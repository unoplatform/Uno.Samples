# 08 — Accessibility

> WCAG compliance, keyboard navigation, screen reader support, and inclusive design

---

## Accessibility Goals

### Compliance Target

NEXUS targets **WCAG 2.1 Level AA** compliance with attention to Level AAA where practical.

### User Considerations

| User Group | Considerations |
|------------|----------------|
| Low vision | High contrast, scalable text |
| Color blindness | Not relying on color alone |
| Motor impairments | Keyboard accessibility, large targets |
| Cognitive | Clear hierarchy, consistent patterns |
| Screen reader | Semantic HTML, ARIA labels |

---

## Color & Contrast

### Contrast Requirements

| Context | Minimum Ratio | Target Ratio |
|---------|---------------|--------------|
| Normal text | 4.5:1 | 7:1 |
| Large text (≥18px) | 3:1 | 4.5:1 |
| UI components | 3:1 | 4.5:1 |
| Focus indicators | 3:1 | 4.5:1 |

### Current Contrast Ratios

| Combination | Ratio | Level |
|-------------|-------|-------|
| `--text-primary` on `--bg-primary` | 19.2:1 | AAA ✓ |
| `--text-primary` on `--bg-secondary` | 17.8:1 | AAA ✓ |
| `--text-secondary` on `--bg-primary` | 7.4:1 | AAA ✓ |
| `--text-secondary` on `--bg-secondary` | 6.8:1 | AA ✓ |
| `--text-tertiary` on `--bg-primary` | 4.7:1 | AA ✓ |
| `--text-tertiary` on `--bg-secondary` | 4.3:1 | AA ✓ |
| `--status-success` on `--bg-secondary` | 8.9:1 | AAA ✓ |
| `--status-warning` on `--bg-secondary` | 10.1:1 | AAA ✓ |
| `--status-danger` on `--bg-secondary` | 5.2:1 | AA ✓ |
| `--status-info` on `--bg-secondary` | 4.8:1 | AA ✓ |

### Color Independence

Never rely on color alone to convey information. Always pair with:

| Information | Color | Additional Indicator |
|-------------|-------|----------------------|
| Status | Green/Yellow/Red | Text label ("ACTIVE") |
| Errors | Red | Icon + text message |
| Success | Green | Icon + text message |
| Required fields | — | Asterisk (*) |
| Trends | Green/Yellow | Arrow icon + text (+2.4%) |
| Priority | Red/Yellow/Green | Text label (HIGH/NORMAL/LOW) |

### Color Blindness Testing

Test the interface with:
- Protanopia (red-blind)
- Deuteranopia (green-blind)
- Tritanopia (blue-blind)
- Achromatopsia (no color)

**Tools:** Sim Daltonism, Color Oracle, Chrome DevTools

---

## Keyboard Navigation

### Focus Order

Focus order follows visual layout, left-to-right, top-to-bottom:

```
Header → Tabs → Content Panels → Panel Actions → Panel Content → Footer
```

### Focus Visibility

All focusable elements must have a visible focus indicator:

```css
:focus-visible {
  outline: 2px solid var(--border-secondary);
  outline-offset: 2px;
}

/* Remove focus ring for mouse users */
:focus:not(:focus-visible) {
  outline: none;
}
```

### Keyboard Interactions

| Element | Keys | Action |
|---------|------|--------|
| Button | Enter, Space | Activate |
| Link | Enter | Navigate |
| Tab | Enter, Space | Switch tab |
| Toggle | Enter, Space | Toggle state |
| Select | Enter, Space, Arrow | Open, navigate |
| Table Row | Enter | Select/expand |
| Modal | Escape | Close |

### Tab Navigation

| Key | Action |
|-----|--------|
| Tab | Move focus to next element |
| Shift+Tab | Move focus to previous element |
| Arrow Left/Right | Move between tabs |
| Home | Focus first tab |
| End | Focus last tab |

### Table Navigation

| Key | Action |
|-----|--------|
| Tab | Move to next focusable cell/row |
| Arrow Up/Down | Move between rows |
| Arrow Left/Right | Move between cells (if cells focusable) |
| Enter | Activate row action |

### Skip Links

Provide skip link for keyboard users:

```html
<a href="#main-content" class="skip-link">
  Skip to main content
</a>
```

```css
.skip-link {
  position: absolute;
  left: -9999px;
  z-index: 999;
}

.skip-link:focus {
  left: 16px;
  top: 16px;
  padding: 8px 16px;
  background: var(--bg-secondary);
  border: 1px solid var(--border-secondary);
}
```

---

## Screen Reader Support

### Semantic HTML

Use proper HTML elements:

| Element | Usage |
|---------|-------|
| `<header>` | Page header |
| `<nav>` | Navigation regions |
| `<main>` | Main content area |
| `<footer>` | Page footer |
| `<section>` | Content sections |
| `<article>` | Self-contained content |
| `<table>` | Tabular data |
| `<button>` | Interactive buttons |
| `<a>` | Navigation links |

### ARIA Landmarks

```html
<header role="banner">
  <nav role="navigation" aria-label="Main navigation">
    <!-- tabs -->
  </nav>
</header>

<main role="main" id="main-content">
  <section aria-labelledby="kpi-heading">
    <h2 id="kpi-heading" class="sr-only">Key Performance Indicators</h2>
    <!-- KPI cards -->
  </section>
</main>

<footer role="contentinfo">
  <!-- footer content -->
</footer>
```

### ARIA Labels

| Component | ARIA Attribute | Example |
|-----------|----------------|---------|
| Metric Card | `aria-label` | `aria-label="Throughput: 847.2 units per hour, up 2.4 percent"` |
| Status Indicator | `aria-label` | `aria-label="Status: Active"` |
| Progress Bar | `aria-valuenow`, `aria-valuemin`, `aria-valuemax` | `aria-valuenow="67" aria-valuemin="0" aria-valuemax="100"` |
| Toggle | `aria-checked` | `aria-checked="true"` |
| Tab | `aria-selected` | `aria-selected="true"` |
| Alert | `role="alert"` | `role="alert" aria-live="polite"` |

### Live Regions

For dynamic content updates:

```html
<!-- Status changes -->
<div aria-live="polite" aria-atomic="true">
  <span class="sr-only">System status:</span>
  Connected
</div>

<!-- Urgent alerts -->
<div role="alert" aria-live="assertive">
  Critical: Line A1 temperature exceeded threshold
</div>

<!-- KPI updates (subtle) -->
<div aria-live="off" aria-atomic="true">
  <!-- Updated silently, user can query -->
</div>
```

### Hidden Content

Content visible but should be hidden from screen readers:

```html
<span aria-hidden="true">●</span> <!-- Decorative status dot -->
```

Content invisible but should be read by screen readers:

```html
<span class="sr-only">Status: Active</span>
```

```css
.sr-only {
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
```

---

## Component Accessibility

### Metric Card

```html
<article 
  class="metric-card"
  role="region"
  aria-labelledby="throughput-label"
>
  <h3 id="throughput-label" class="metric-label">THROUGHPUT</h3>
  <p class="metric-value" aria-describedby="throughput-desc">847.2</p>
  <p id="throughput-desc" class="sr-only">units per hour, up 2.4 percent from yesterday</p>
  <p class="metric-unit" aria-hidden="true">units/hr</p>
  <p class="metric-trend" aria-hidden="true">+2.4%</p>
  <p class="metric-status">
    <span class="status-dot" aria-hidden="true"></span>
    <span>NOMINAL</span>
  </p>
</article>
```

### Data Table

```html
<table role="grid" aria-label="Production Lines">
  <thead>
    <tr>
      <th scope="col">Unit</th>
      <th scope="col">Status</th>
      <th scope="col">Output</th>
      <th scope="col">Temperature</th>
      <th scope="col">Pressure</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <th scope="row">A1 - Assembly Line A1</th>
      <td>
        <span aria-hidden="true">●</span>
        Active
      </td>
      <td>
        <span class="sr-only">89 percent</span>
        <div class="progress-bar" aria-hidden="true">...</div>
      </td>
      <td>42.3 °C</td>
      <td>2.4 bar</td>
    </tr>
  </tbody>
</table>
```

### Toggle Switch

```html
<label class="toggle-label">
  <span class="toggle-text">Auto Backup</span>
  <button 
    role="switch"
    aria-checked="true"
    class="toggle"
  >
    <span class="toggle-knob"></span>
  </button>
</label>
```

### Tab Navigation

```html
<div role="tablist" aria-label="Main navigation">
  <button 
    role="tab" 
    aria-selected="true" 
    aria-controls="overview-panel"
    id="overview-tab"
  >
    Overview
  </button>
  <button 
    role="tab" 
    aria-selected="false" 
    aria-controls="production-panel"
    id="production-tab"
  >
    Production
  </button>
</div>

<div 
  role="tabpanel" 
  id="overview-panel" 
  aria-labelledby="overview-tab"
>
  <!-- Content -->
</div>
```

### Progress Bar

```html
<div 
  role="progressbar"
  aria-valuenow="67"
  aria-valuemin="0"
  aria-valuemax="100"
  aria-label="Shift progress: 67 percent complete"
>
  <div class="progress-fill" style="width: 67%"></div>
</div>
```

### Alert/Notification

```html
<!-- Inline alert in system log -->
<div role="log" aria-label="System log" aria-live="polite">
  <div class="alert-item">
    <time datetime="2024-11-15T14:32:08">14:32:08</time>
    <span aria-hidden="true" class="alert-dot info"></span>
    <span>Batch #4892 completed successfully</span>
  </div>
</div>

<!-- Toast notification -->
<div 
  role="alert" 
  aria-live="assertive"
  class="toast"
>
  User created successfully
</div>
```

---

## Motion & Animation

### Reduced Motion

Respect user preference for reduced motion:

```css
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
    scroll-behavior: auto !important;
  }
}
```

### Essential vs. Decorative Motion

| Type | Examples | Behavior with reduced-motion |
|------|----------|------------------------------|
| Essential | Toggle state change, form feedback | Instant (no animation) |
| Decorative | Status pulse, network flow | Disabled |
| Attention | Critical alert pulse | Reduced or disabled |

---

## Text & Typography

### Minimum Text Size

| Context | Minimum | Recommended |
|---------|---------|-------------|
| Body text | 10px | 11px |
| Labels | 9px | 10px |
| Captions | 8px | 9px |

### Text Spacing

Support WCAG 1.4.12 Text Spacing:

```css
/* Users should be able to apply: */
/* - Line height: 1.5× font size */
/* - Paragraph spacing: 2× font size */
/* - Letter spacing: 0.12× font size */
/* - Word spacing: 0.16× font size */

/* Ensure layout doesn't break with these settings */
.text-container {
  max-width: 100%;
  overflow-wrap: break-word;
}
```

### Text Resize

Interface should remain usable at 200% zoom:

- No horizontal scrolling at 1280px width + 200% zoom
- All text remains visible
- Controls remain accessible

---

## Forms & Inputs

### Labels

All inputs must have associated labels:

```html
<!-- Visible label -->
<label for="temp-threshold">Temperature Max</label>
<input type="number" id="temp-threshold" value="85">
<span>°C</span>

<!-- Hidden label (when visually implied) -->
<label for="search" class="sr-only">Search</label>
<input type="search" id="search" placeholder="Search...">
```

### Error States

```html
<div class="input-group" aria-invalid="true" aria-describedby="temp-error">
  <label for="temp-input">Temperature Max</label>
  <input 
    type="number" 
    id="temp-input" 
    value="150"
    aria-invalid="true"
  >
  <span id="temp-error" class="error-message" role="alert">
    Value must be between 0 and 100
  </span>
</div>
```

### Required Fields

```html
<label for="email">
  Email
  <span aria-hidden="true">*</span>
  <span class="sr-only">(required)</span>
</label>
<input 
  type="email" 
  id="email" 
  required
  aria-required="true"
>
```

---

## Focus Management

### Modal Focus Trap

When a modal opens:
1. Move focus to modal
2. Trap focus within modal
3. Return focus to trigger on close

```javascript
// Example focus trap
function trapFocus(modal) {
  const focusable = modal.querySelectorAll(
    'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
  );
  const first = focusable[0];
  const last = focusable[focusable.length - 1];
  
  modal.addEventListener('keydown', (e) => {
    if (e.key === 'Tab') {
      if (e.shiftKey && document.activeElement === first) {
        e.preventDefault();
        last.focus();
      } else if (!e.shiftKey && document.activeElement === last) {
        e.preventDefault();
        first.focus();
      }
    }
  });
}
```

### Tab Change Focus

When switching tabs, focus moves to the new tab panel:

```javascript
function switchTab(tabId) {
  // Update tab states
  // ...
  
  // Move focus to new panel
  const panel = document.getElementById(tabId + '-panel');
  panel.focus();
}
```

---

## Testing Checklist

### Automated Testing

- [ ] aXe DevTools audit
- [ ] WAVE evaluation
- [ ] Lighthouse accessibility score ≥90

### Manual Testing

- [ ] Keyboard-only navigation (full app)
- [ ] Screen reader testing (NVDA, VoiceOver, JAWS)
- [ ] High contrast mode (Windows)
- [ ] Browser zoom 200%
- [ ] Reduced motion preference
- [ ] Color blindness simulation

### Screen Reader Testing Script

1. Navigate to application
2. Announce page title and landmarks
3. Navigate to KPI cards, verify values announced
4. Navigate to production table, verify row data
5. Activate tab to switch pages
6. Verify alert announcements
7. Complete form interaction in Settings

---

## Accessibility Statement

Include an accessibility statement page:

```markdown
# Accessibility

NEXUS is committed to ensuring digital accessibility for people with disabilities.

## Conformance Status
This application aims to conform to WCAG 2.1 Level AA.

## Feedback
If you encounter accessibility barriers, please contact:
- Email: accessibility@nexus-system.example
- Phone: [number]

## Technical Specifications
This application relies on the following technologies:
- HTML5
- CSS3
- JavaScript
- WAI-ARIA

## Known Limitations
- Complex data visualizations may not be fully accessible to screen readers
- Network topology diagram is decorative; key information is available in tables

## Assessment Approach
We assess accessibility through:
- Automated testing tools
- Manual keyboard testing
- Screen reader testing
```
