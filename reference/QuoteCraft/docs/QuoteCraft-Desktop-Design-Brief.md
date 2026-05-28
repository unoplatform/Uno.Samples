# QuoteCraft Desktop — Design Brief

**Version 1.0 — February 2026**
**Framework & Language Agnostic Specification**
**Breakpoint Coverage: Wide (905–1280px) and Extra Wide (1281px+)**

---

## Table of Contents

1. [Overview & Relationship to Mobile Brief](#1-overview--relationship-to-mobile-brief)
2. [Desktop Shell & Layout Architecture](#2-desktop-shell--layout-architecture)
3. [Navigation Rail](#3-navigation-rail)
4. [Top Bar](#4-top-bar)
5. [Master-Detail Pattern](#5-master-detail-pattern)
6. [Screen Specifications](#6-screen-specifications)
7. [Modal Dialogs (replaces Bottom Sheets)](#7-modal-dialogs-replaces-bottom-sheets)
8. [Interaction Patterns — Desktop Adaptations](#8-interaction-patterns--desktop-adaptations)
9. [Responsive Behavior Between Wide & Extra Wide](#9-responsive-behavior-between-wide--extra-wide)
10. [Keyboard & Pointer Interactions](#10-keyboard--pointer-interactions)
11. [Implementation Notes](#11-implementation-notes)

---

## 1. Overview & Relationship to Mobile Brief

This document specifies the desktop layout for QuoteCraft, covering the **Wide** (905–1280px) and **Extra Wide** (1281px+) breakpoints defined in the mobile brief. It is a companion to the mobile design brief, not a replacement.

### 1.1 What Carries Over Unchanged

The following systems are identical across mobile and desktop. Refer to the mobile brief for their complete specifications:

- **Color palette** — all tokens, hex values, and usage rules (Mobile Brief §2.2)
- **Typography scale** — all Material type styles and semantic roles (Mobile Brief §2.3)
- **Spacing scale** — all tokens from `xs` (4px) through `2xl` (48px) (Mobile Brief §2.4)
- **Border radius** — all tokens from Small (8px) through Full/Pill (Mobile Brief §2.7)
- **Status badge system** — all 5 statuses with colors, text, and icons (Mobile Brief §4.1.4)
- **Data model** — all entities and fields (Mobile Brief §6)
- **PDF generation** — page setup, layout, and tier branding (Mobile Brief §7)
- **Accessibility requirements** — all WCAG compliance rules (Mobile Brief §8)
- **Iconography** — all semantic icons and naming conventions (Mobile Brief §9)
- **Monetization tiers** — free vs. Pro feature boundaries (Mobile Brief §11)

### 1.2 What Changes on Desktop

| Concern | Mobile | Desktop |
|---------|--------|---------|
| Primary navigation | Bottom tab bar (fixed, 4 items) | Navigation rail (left side, 80px wide) |
| Screen layout | Single column, full-width cards | Master-detail split panels |
| Modal interactions | Bottom sheets (slide up from bottom) | Centered modal dialogs (scale + fade) |
| Touch targets | 48dp minimum (glove-friendly) | 36dp minimum for secondary actions; 40dp+ for primary. Pointer precision allows smaller targets. |
| FAB | Floating action button, bottom-right | Replaced by top bar "New" button |
| Content density | Generous padding, large gaps | Tighter padding, more information on screen |
| Hover states | Not applicable | Required on all interactive elements |
| Scrolling | Full-page scroll per screen | Independent scroll regions per panel |

### 1.3 Design Principle Additions for Desktop

**Information density over navigation depth.** On desktop, users have enough viewport to see a list and its detail simultaneously. Never force a full-screen navigation push when a panel update will do.

**Hover as a disclosure mechanism.** Destructive and secondary actions (delete buttons, edit controls) can be hidden until the user hovers over the parent element. This reduces visual noise without sacrificing discoverability.

**Keyboard-first for power users.** Desktop users expect keyboard shortcuts and tab-order focus management. Every action achievable by click must also be achievable by keyboard.

---

## 2. Desktop Shell & Layout Architecture

### 2.1 Shell Structure

The desktop shell is a horizontal flexbox that fills the full viewport (`100vh × 100vw`). It contains three structural regions:

```
┌──────┬──────────────────────────────────────────────────────┐
│      │  Top Bar (64px)                                      │
│ Nav  ├───────────────┬──────────────────────────────────────┤
│ Rail │  List Panel   │  Detail Panel                        │
│(80px)│  (380px)      │  (flex: 1)                           │
│      │               │                                      │
│      │  scrollable   │  scrollable                          │
│      │               │                                      │
└──────┴───────────────┴──────────────────────────────────────┘
```

- **Navigation Rail** — fixed width, full viewport height, leftmost column.
- **Main Area** — everything to the right of the rail. Contains the Top Bar and the Content Area.
- **Content Area** — below the top bar, fills remaining height. Layout varies per screen (master-detail, single scroll, grid).

### 2.2 Overflow Rules

- The shell itself never scrolls. `overflow: hidden` on the root container.
- Each panel (list panel, detail panel) scrolls independently.
- Scrollbars: thin (4–6px), rounded thumb in Outline color, track hidden. Appear on hover over the scrollable region.

---

## 3. Navigation Rail

### 3.1 Dimensions & Position

- Width: **80px**, fixed. Does not collapse or expand.
- Height: full viewport height.
- Background: Surface (white).
- Right border: 1px Outline Variant.
- Z-index: above content panels (20+) to prevent shadow overlap.

### 3.2 Brand Header

- Occupies the top **64px** of the rail (matches top bar height for alignment).
- Background: **Primary** color.
- Contains a bold brand mark ("QC" in the prototype, or a logo icon in production). White, 22px bold.
- Bottom edge: **3px Secondary (Amber) accent bar** spanning full width (mirrors the mobile app bar accent).

### 3.3 Navigation Items

- Arranged vertically below the brand header, centered horizontally.
- Padding: 16px top, 4px gap between items.
- Each item: **64px wide**, flex column layout.

**Item anatomy (per destination):**

| Element | Spec |
|---------|------|
| Container | 64×56dp, flex-column, centered, border-radius Medium (12dp) |
| Icon | 22px, line-height 1, centered |
| Label | 11px, font-weight 500, 4px below icon |
| Inactive color | On Surface Variant for both icon and label |
| Active color | Primary for both icon and label, font-weight 600 |
| Active background | Primary Container |
| Active indicator | 4px-wide vertical bar, Primary color, positioned on the left edge of the rail (−8px from item left), 24px tall, vertically centered, border-radius 0 4px 4px 0 |
| Hover (inactive) | Surface Variant background fill, 150ms ease transition |

### 3.4 Destinations

| Order | ID | Icon (semantic) | Label |
|-------|-----|-----------------|-------|
| 1 | `quotes` | Clipboard / document list | Quotes |
| 2 | `clients` | Person | Clients |
| 3 | `catalog` | Book / grid | Catalog |
| 4 | `settings` | Gear | Settings |

Tapping a destination:
- Switches the entire Main Area to that screen's content.
- Resets any sub-navigation state (e.g., closes preview, deselects items).
- No animation between tab screens.

---

## 4. Top Bar

### 4.1 Dimensions & Position

- Height: **64px**, full width of Main Area.
- Background: Surface (white).
- Bottom border: 1px Outline Variant.
- Fixed at the top of the Main Area (does not scroll).
- Horizontal padding: **32px** left and right.

### 4.2 Layout

```
┌─────────────────────────────────────────────────────────────┐
│  [Title]  [Subtitle]                      [Search] [Action] │
│  ← left-aligned                           right-aligned →   │
└─────────────────────────────────────────────────────────────┘
```

**Left side:**
- Page title: 20px, font-weight 700, On Surface color.
- Subtitle (optional): 13px, On Surface Variant, 4px left margin. Shows context like item counts ("7 total") or breadcrumb context ("Plumbing").

**Right side:**
- Contextual action buttons (right-aligned, 10px gap between buttons).
- Uses standard button styles (see §4.3).
- Some screens include a search input in the top bar right side (Catalog screen).

### 4.3 Button Styles

All buttons share: 10px vertical + 20px horizontal padding, border-radius Small (8dp), 14px font, font-weight 600, `inline-flex` with 8px icon gap.

| Style | Background | Text | Border | Hover |
|-------|-----------|------|--------|-------|
| Primary | Primary | White | None | Primary Light (`#3A6298`) |
| Secondary | Surface | Primary | 1.5px Primary | Primary Container background |
| Accent | Secondary (Amber) | `#1F2937` (dark) | None | Darker amber (`#E8930A`) |
| Ghost | Transparent | On Surface Variant | 1.5px Outline | Surface Variant background, border → On Surface Variant |

---

## 5. Master-Detail Pattern

This is the dominant layout pattern on desktop, used by the **Quotes** and **Clients** screens. It divides the content area into two independently scrollable panels.

### 5.1 Structure

```
┌───────────────┬──────────────────────────────────────────┐
│  List Panel   │  Detail Panel                            │
│  (380px)      │  (flex: 1, min-width: 0)                 │
│               │                                          │
│  Fixed width  │  Expands to fill remaining space         │
│  border-right │                                          │
│  1px outline  │  Internal padding: 28px top, 36px horiz  │
│  variant      │                                          │
│               │                                          │
│  Scrollable ↕ │  Scrollable ↕                            │
└───────────────┴──────────────────────────────────────────┘
```

### 5.2 List Panel

- Width: **380px**, fixed, no resize.
- Background: Surface (white).
- Right border: 1px Outline Variant.
- Divided into Header (fixed, non-scrolling) and Body (scrollable).

**List Panel Header:**
- Padding: 16px horizontal, 16px top, 12px bottom.
- Contains: search input (full-width) and optionally a chip filter bar below it.
- Header does not scroll — it remains pinned as the list body scrolls.

**List Panel Body:**
- Padding: 0 12px 12px.
- Scrollbar: thin (4px wide), Outline-colored thumb, appears on hover.

### 5.3 Detail Panel

- Flex: 1 with `min-width: 0` to prevent overflow.
- Padding: **28px top, 36px horizontal**.
- Independently scrollable.
- Scrollbar: 6px wide, Outline-colored thumb, appears on hover.

**Empty state (no selection):**
- Centered vertically and horizontally.
- Icon: 56px, 25% opacity.
- Title: 18px semibold, On Surface.
- Description: 14px, On Surface Variant, max-width 320px, 1.5 line-height.
- Example: "📋 Select a quote — Choose a quote from the list to view details, or create a new one."

### 5.4 Detail Panel Internal Layout — Two-Column Grid

When a detail item is selected, the detail panel uses an internal CSS grid:

```
┌──────────────────────────────┬──────────────┐
│  Detail Main                 │  Sidebar     │
│  (flex: 1)                   │  (340px)     │
│                              │              │
│  Line items table            │  Client card │
│  Notes card                  │  Details card│
│                              │  Totals card │
└──────────────────────────────┴──────────────┘
```

- Grid: `grid-template-columns: 1fr 340px`, gap 24px, `align-items: start`.
- On the Wide breakpoint (905–1280px), the sidebar collapses below the main content if viewport width is insufficient. See §9.

---

## 6. Screen Specifications

### 6.0 Shared Card Component

Cards are used throughout the desktop layout for grouping content.

| Property | Value |
|----------|-------|
| Background | Surface (white) |
| Border | 1px Outline Variant |
| Border radius | Medium (12dp) |
| Box shadow | Shadow Level 1 (sm) |
| Overflow | Hidden (for child background fills like table headers) |

**Card Header:**
- Padding: 14px vertical, 20px horizontal.
- Bottom border: 1px Outline Variant.
- Title: 13px, font-weight 700, uppercase, letter-spacing 0.8px, On Surface Variant.
- Optional right-side element (count badge, action button).

**Card Body:**
- Padding: 16px vertical, 20px horizontal.

---

### 6.1 Quotes Screen

#### 6.1.1 Top Bar Content

- Title: "Quotes"
- Subtitle: "{count} total"
- Actions: Accent button "+ New Quote"

#### 6.1.2 List Panel — Quote Cards

**Search Input:**
- Full-width, 10px vertical + 14px horizontal + 38px left padding (for search icon).
- Border: 1.5px Outline, radius Small (8dp).
- Background: Surface Variant (unfocused) → Surface (focused) with Primary border.
- Search icon: 16px, On Surface Variant, absolute-positioned left 12px.

**Chip Filter Bar:**
- Below search, 12px top gap.
- Flex-wrap with 6px gap between chips.
- Chips: all, draft, sent, viewed, accepted, declined.
- Active chip: Primary Container background, Primary border + text, bold.
- Inactive chip: Surface background, Outline border, On Surface Variant text.
- Non-"All" chips show count of matching quotes.

**Quote Card (list item):**

| Property | Value |
|----------|-------|
| Background | Surface |
| Border | 1.5px Outline Variant |
| Border radius | Medium (12dp) |
| Padding | 14px vertical, 16px horizontal |
| Margin bottom | 6px |
| Cursor | Pointer |

**Card layout:**
- Top row: title (14px, semibold) on left, status badge on right. Client name (12px, On Surface Variant) below title, 2px margin-top.
- Bottom row: 8px margin-top, 8px padding-top, 1px Outline Variant top border. Amount (16px, bold, monospace, Primary) on left, date (11px, On Surface Variant) on right.

**States:**
- Hover: border color → Primary, background → `#FAFBFF`.
- Selected: border color → Primary, background → Primary Container, box-shadow sm.
- Default: as specified above.

#### 6.1.3 Detail Panel — Quote Detail View

**Detail Header:**
- Flex row, `justify-content: space-between`, `align-items: flex-start`, 28px bottom margin, 24px gap.
- Left side: Title (26px, bold), meta row below (flex wrap, 16px gap).
- Meta items: 13px, On Surface Variant, icon + text pairs with 5px gap. Shows client name, quote number, valid-until date, and status badge inline.
- Right side: action buttons (Ghost "Edit", Secondary "Preview", Accent "Send"), 8px gap.

**Detail Grid:**
- Two-column grid as described in §5.4.

**Main Column — Line Items Card:**

Table element with full-width `border-collapse: collapse`.

| Part | Spec |
|------|------|
| Header row | Background: Surface Variant. 11px uppercase bold, letter-spacing 0.5px, On Surface Variant. Padding: 10px 16px. Bottom border: 2px On Surface. |
| Columns | Description (left-aligned, flex), Qty (right, 80px), Unit Price (right, 110px), Total (right, 110px), Delete (40px, edit mode only) |
| Data rows | Padding: 12px 16px. Bottom border: 1px Outline Variant. Even rows: background `#FAFBFC`. Hover: background `#F0F4FF`. |
| Description cell | 14px, font-weight 500, On Surface |
| Numeric cells | Right-aligned, monospace font, 13px |
| Total cell | font-weight 600 |
| Delete button | Hidden by default (`opacity: 0`). Appears on row hover. 16px ✕ icon, padding 4px 8px, radius 4px. Hover: Error color text, `#FEE2E2` background. |

**Edit Mode:**
- Toggled by "Edit" button in header (toggles between "Edit" and "Done" labels).
- Reveals: delete buttons on all item rows, and an "items-table-footer" div below the table.
- Footer contains two add-item buttons (see below).

**Add Item Buttons (edit mode footer):**
- Padding: 12px 16px top area, flex row with 8px gap.
- "+ Add Line Item": dashed 1.5px Outline border, Primary text, radius Small. Hover: Primary Container background, Primary border.
- "From Catalog": solid 1.5px Secondary border, `#92400E` text. Hover: Secondary Container background.
- Both: 8px 16px padding, 13px font, 500 weight, inline-flex with 6px icon gap.

**Main Column — Notes Card:**
- 16px top margin from items card.
- View mode: `notes-block` class, 14px, On Surface Variant, line-height 1.6.
- Edit mode: full-width textarea, 1.5px Outline border, radius Small, 12px padding, 14px font, min-height 80px, resize vertical. Focus: Primary border.

**Sidebar Column — Client Card:**
- Horizontal layout: avatar (42dp circle, Primary Container, Primary initials 15px bold) + text (name 14px semibold, email + phone each 12px On Surface Variant). 12px gap.

**Sidebar Column — Quote Details Card:**
- Key-value rows: label (12px, 500 weight, On Surface Variant) on left, value (14px, 500 weight, On Surface) on right.
- Rows: Quote #, Created, Valid Until, Status (renders StatusBadge).
- Rows separated by 1px Outline Variant bottom border, 10px vertical padding. Last row: no border.

**Sidebar Column — Totals Card:**
- Subtotal and Tax rows: 14px, On Surface Variant, flex space-between, 8px vertical padding. Values: monospace, 500 weight.
- Grand Total row: 14px top padding, 8px top margin, 2px top border On Surface. Label: 22px, bold, On Surface. Value: 24px, bold, monospace, Primary color.

#### 6.1.4 Quote Preview Sub-View

Activated by the "Preview" button. Replaces the detail panel content (not a modal).

**Header:**
- Title: "Quote Preview"
- Meta: quote title + client name.
- Actions: Ghost "← Back to Detail", Secondary "Download PDF", Primary "Email Client".

**PDF Document Card:**
- Max-width: **680px** within the detail panel. Centered or left-aligned.
- Border radius Medium (12dp), box-shadow md, 1px Outline Variant border.
- Internal layout mirrors the mobile brief §4.5.2 exactly (header block, client section, line items table, totals, notes, footer).

---

### 6.2 Clients Screen

Uses the same master-detail pattern as Quotes.

#### 6.2.1 Top Bar Content

- Title: "Clients"
- Subtitle: "{count} contacts"
- Actions: Accent button "+ Add Client"

#### 6.2.2 List Panel — Client Rows

**Search:** Same spec as Quotes list search.

**Client Row (list item):**
- Flex row, 14px vertical + 16px horizontal padding.
- Bottom border: 1px Outline Variant. Last item: no border.
- Hover: Surface Variant background. Selected: Primary Container background.
- Border radius: Small (8dp).

| Element | Spec |
|---------|------|
| Avatar | 42dp circle, Primary Container background, Primary text, initials (first letter of each name part), 15px bold, flex-shrink 0 |
| Info (flex 1, min-width 0) | Name: 14px semibold. Meta: 12px On Surface Variant, shows quote count + city. 1px margin-top. |
| Value (right, flex-shrink 0) | Amount: 14px semibold monospace, Primary. Label ("total"): 10px, On Surface Variant. Right-aligned. |
| Gap between elements | 16px |

#### 6.2.3 Detail Panel — Client Detail View

**Header:**
- Title: client name (26px bold).
- Meta: email, phone, address as icon+text pairs.
- Actions: Ghost "Edit", Accent "+ New Quote".

**Grid:** `grid-template-columns: 1fr 280px`, gap 24px.

**Main Column — Quote History Card:**
- Card header: "Quote History ({count})".
- Body: no padding (items bleed to card edges).
- Each row: flex space-between, 12px vertical + 20px horizontal padding, 1px Outline Variant bottom border.
- Left: title (14px, 500 weight), number + date below (12px, On Surface Variant, 2px margin-top).
- Right: amount (15px semibold monospace, Primary) + status badge, 16px gap.
- Empty: centered text "No quotes for this client yet." (14px, On Surface Variant, 24px padding).

**Sidebar Column — Summary Card:**
- Key-value rows: Total Quotes (count), Total Value (monospace Primary, 600 weight), Last Quote (date or "—").
- Same row styling as Quote Details card.

---

### 6.3 Catalog Screen

The catalog screen does **not** use the master-detail pattern. It uses a **full-width grid layout**.

#### 6.3.1 Top Bar Content

- Title: "Catalog"
- Subtitle: catalog name (e.g., "Plumbing")
- Actions: search input (240px wide, padded left 34px for icon) + Accent button "+ Add Item"

#### 6.3.2 Category Chip Bar

- Positioned below the top bar, inside the content area.
- Padding: 16px horizontal (36px from content edge), 16px top, 8px bottom.
- Flex-wrap, 6px gap.
- Same chip styling as Quotes filter chips.
- Categories: "All" + all catalog category names.

#### 6.3.3 Item Grid

- CSS grid: `grid-template-columns: repeat(auto-fill, minmax(260px, 1fr))`, gap 12px.
- Padding: 20px vertical, 36px horizontal (matches detail panel horizontal padding).
- Scrollable vertically.

**Category Group Header (when "All" is selected):**
- 11px, bold, uppercase, letter-spacing 1px, On Surface Variant.
- Padding: 20px horizontal (36px from edge), 20px top, 8px bottom.
- Only shown when "All" category is active.

**Catalog Item Card:**

| Property | Value |
|----------|-------|
| Background | Surface |
| Border | 1px Outline Variant |
| Border radius | Medium (12dp) |
| Padding | 16px |
| Layout | Flex row, space-between, align-center |
| Cursor | Pointer |
| Hover | Primary border, box-shadow sm |

- Left: item name (14px, 500 weight, On Surface).
- Right: price (14px, 600 weight, monospace, On Surface Variant).
- Tapping opens an edit view (not specified in this MVP — shows a modal or inline editor).

---

### 6.4 Settings Screen

Single-column scrollable layout. No master-detail.

#### 6.4.1 Top Bar Content

- Title: "Settings"
- No subtitle, no action buttons.

#### 6.4.2 Layout

- Padding: 28px top, 36px horizontal.
- Max-width: **720px** (settings forms shouldn't stretch across ultra-wide screens).
- Content scrolls vertically within the content area.

#### 6.4.3 Upgrade Banner

- Placed at the top of settings content, before any groups.
- Background: linear-gradient 135deg from Primary to dark navy (`#1A365D`).
- Color: white. Border radius: Medium (12dp). Padding: 24px.
- Layout: flex row, space-between, align-center, 24px gap.
- Left: title (18px, bold) + description (13px, 80% opacity).
- Right: Amber CTA button ("Upgrade — $15/mo"), bold 14px, 12px vertical + 28px horizontal padding, radius Small. Flex-shrink 0.
- Hover: darker amber (`#E8930A`).
- Bottom margin: 28px.

#### 6.4.4 Settings Groups

Each group:
- **Group title:** 11px, bold, uppercase, letter-spacing 1px, On Surface Variant. Padding-left 4px. 8px bottom margin.
- **Group card:** Surface background, radius Medium, shadow sm, 1px Outline Variant border.
- Bottom margin: **28px** between groups.

**Settings Row:**
- Flex row, space-between, align-center.
- Padding: 14px vertical, 20px horizontal.
- Bottom border: 1px Outline Variant. Last row: no border.
- Hover: `#FAFBFC` background, 100ms transition.
- Label: 14px, 500 weight, On Surface (left).
- Value: 14px, On Surface Variant, right-aligned (right).

**Editable Rows:**
- Replace the static value with an `<input>` element.
- Input: 1px Outline border, radius 6px, 6px vertical + 10px horizontal padding, 14px font, right-aligned text, width varies by content (50–100px).
- Focus: Primary border.
- Some rows show a suffix outside the input ("%" for tax rate, "days" for validity period). Suffix: On Surface Variant color, 4px left gap.

**Groups in order:**

1. **Business Info** — Business Name, Phone, Email, Address (all static values), Logo ("Upload ↗" link in Primary color, cursor pointer).
2. **Quote Settings** — Default Tax Rate (input + "%"), Valid For (input + "days"), Currency (static "USD ($)"), Quote Prefix (input, 70px).
3. **My Catalogs** — Catalog rows (icon + name, item count on right), "+ Add Catalog" row (Primary color label, no value).
4. **Subscription** — Current Plan (static), Quotes Used (static "3 of 5").

---

## 7. Modal Dialogs (replaces Bottom Sheets)

On desktop, all interactions that used bottom sheets on mobile are presented as **centered modal dialogs**. This is a platform-appropriate adaptation — bottom sheets feel wrong on desktop where there's no bottom-edge context.

### 7.1 Modal Container

**Overlay (scrim):**
- Position: fixed, inset 0. Background: rgba(0, 0, 0, 0.35). Z-index: 100.
- Flex center (both axes) to position the dialog.
- Animation: fade-in, 150ms ease.
- Click on scrim: dismisses the modal.

**Dialog:**
- Background: Surface. Border radius: Large (16dp). Box-shadow: lg.
- Width: **480px** (line item editor) or **560px** (catalog browser).
- Max-height: **80vh**.
- Flex column layout.
- Animation: scale from 0.96 + translateY(8px) to identity + fade in, 200ms ease.

### 7.2 Modal Anatomy

**Header:**
- Padding: 20px horizontal, 20px top, 16px bottom.
- Bottom border: 1px Outline Variant.
- Title: 18px, 600 weight, On Surface (left).
- Close button: 32dp circle, Surface Variant background, ✕ icon (16px, On Surface Variant). Hover: Outline Variant background. Positioned right.

**Body:**
- Padding: 20px horizontal, 20px vertical.
- Overflow-y: auto (scrolls if content exceeds max-height).
- Flex: 1.

**Footer:**
- Padding: 16px horizontal, 16px vertical.
- Top border: 1px Outline Variant.
- Flex row, justify-content: flex-end, 10px gap.
- Contains cancel (Ghost) and confirm (Primary) buttons.

### 7.3 Line Item Editor Modal (480px)

Replaces mobile brief §4.3.

**Body contents:**
- Description field: full-width, auto-focused on modal open. 16px bottom margin.
- Unit Price + Quantity: CSS grid `1fr auto`, 16px gap, 16px bottom margin.
  - Unit Price: relative container with "$" prefix (absolute, left 14px, centered vertically, `#9CA3AF`). Input left-padding 28px. Number type, step 0.01.
  - Quantity: stepper component (see below).
- Line Total: summary row with 1.5px Outline Variant top border, 14px top padding, 8px top margin. Label (15px semibold) left, value (20px bold monospace, Primary) right.

**Quantity Stepper (desktop):**
- Container: inline-flex, 1.5px Outline border, radius Small, overflow hidden.
- − button: 38×38dp, Surface Variant background, Primary text, 18px bold. Hover: Primary Container.
- Value: 48px wide input, center-aligned, 15px semibold, 1.5px Outline left and right borders, Surface background.
- + button: same as − button.

**Footer:** Ghost "Cancel" + Primary "Add to Quote" (disabled until description AND price have values).

### 7.4 Catalog Browser Modal (560px, maxHeight 85vh)

Replaces mobile brief §4.4.

**Body contents:**
- Search input: full-width, search icon left, 14px bottom margin.
- Category chips: flex-wrap, 6px gap, 14px bottom margin. Same chip styles as elsewhere.
- Item list: grouped by category when "All" is active.
  - Group header: 11px bold uppercase, On Surface Variant, 14px top + 6px bottom padding, 1px Outline Variant bottom border.
  - Item rows: flex space-between, 12px vertical padding, 1px Outline Variant bottom border (last: none).
    - Left: description (14px, 500 weight).
    - Right: price (14px, semibold monospace, On Surface Variant) + add button (12px gap).
    - Add button: 32dp circle, Primary background, white "+" (18px). Hover: Primary Light, scale 1.1. After click: transitions to Success green with "✓" for 1.5 seconds, then reverts. Pointer-events disabled during "added" state.

**No footer** — items are added inline via the row buttons.

---

## 8. Interaction Patterns — Desktop Adaptations

### 8.1 Navigation

| Action | Behavior |
|--------|----------|
| Rail item click | Switches main area to target screen. Resets sub-state (deselect, close preview). No transition animation. |
| List item click | Updates detail panel with selected item. Selected item shows Primary Container background in list. No full-screen navigation. |
| "Edit" toggle | Toggles edit mode in the detail panel. Reveals/hides delete buttons and add-item footer. Does not navigate. |
| "Preview" button | Replaces detail panel content with PDF preview sub-view. "Back" button returns to detail view. |
| Modal open | Renders centered dialog with scrim overlay. Focus traps inside the modal. |
| Modal close | Scrim click, close button, or Escape key. |

### 8.2 Hover States

Every interactive element on desktop must have a visible hover state. Hover changes must be subtle (background tint, border color, shadow increase) and transition smoothly (100–200ms ease).

| Element | Hover Effect |
|---------|-------------|
| Quote card (list) | Border → Primary, background → `#FAFBFF` |
| Client row (list) | Background → Surface Variant |
| Catalog item card | Border → Primary, box-shadow → sm |
| Table row | Background → `#F0F4FF` |
| Delete button (table) | Appears (opacity 0 → 1), hover: Error text + `#FEE2E2` background |
| Nav rail item | Background → Surface Variant |
| All buttons | See §4.3 button hover specs |
| Chip | Border → Primary, text → Primary |
| Modal close button | Background → Outline Variant |
| Settings row | Background → `#FAFBFC` |

### 8.3 Toast Notifications

- Position: fixed, bottom 32px, centered horizontally.
- Same styling as mobile: dark background (`#1F2937`), white text, pill (24dp radius), 12px vertical + 28px horizontal padding, shadow lg.
- Animation: translateY(16px) + fade in (250ms), fade out after 2s (250ms).
- Z-index: 200.

### 8.4 Animations

| Element | Desktop Animation |
|---------|-------------------|
| Screen switches (rail) | Instant (no transition) |
| List → detail updates | Instant content swap in detail panel |
| Modal open | Scrim: fadeIn 150ms. Dialog: scale(0.96) translateY(8px) → identity, 200ms ease |
| Modal close | Reverse of open, 150ms |
| Card hover | Background + border transition, 150ms ease |
| Table row hover | Background transition, 100ms |
| Delete button reveal | Opacity 0 → 1, 150ms |
| Toast | Same as mobile |
| Catalog add button | Background → Success 150ms, revert after 1.5s |
| Detail panel scroll | Native smooth scroll, no custom animation |

---

## 9. Responsive Behavior Between Wide & Extra Wide

### 9.1 Wide Breakpoint (905–1280px)

At this breakpoint, horizontal space is tighter. The following adjustments apply:

| Adjustment | Spec |
|-----------|------|
| List panel width | Reduces from 380px to **320px** |
| Detail sidebar | Collapses below the main column (`grid-template-columns: 1fr`). Sidebar cards stack below the items table and notes in a single-column flow. |
| Detail panel padding | Reduces to 20px top, 24px horizontal |
| Top bar padding | Reduces to 24px horizontal |
| Catalog grid | `minmax(220px, 1fr)` instead of `minmax(260px, 1fr)` |
| Settings max-width | Unchanged (720px fits comfortably) |

### 9.2 Extra Wide Breakpoint (1281px+)

Full specifications as described throughout this document apply at extra-wide. No reductions.

### 9.3 Below 905px

At viewport widths below 905px, the desktop layout should not be used. The application should switch to the mobile layout described in the mobile design brief (bottom tab bar, single-column, full-screen navigation).

---

## 10. Keyboard & Pointer Interactions

### 10.1 Focus Management

- All interactive elements must be reachable via Tab key.
- Focus ring: 2px outline, Primary color, 2px offset. Applied via `:focus-visible` (not `:focus`) to avoid showing rings on mouse click.
- Tab order follows visual reading order: left to right, top to bottom within each panel.
- Modals trap focus: Tab cycles through modal content only while open. Escape closes the modal.

### 10.2 Keyboard Shortcuts

| Shortcut | Action | Context |
|----------|--------|---------|
| `Escape` | Close modal / exit edit mode / deselect | Global |
| `Enter` | Submit form / confirm dialog | When modal is focused |
| `↑ / ↓` | Navigate list items | When list panel is focused |
| `Delete` or `Backspace` | Delete selected line item | When item row is focused in edit mode |
| `/` | Focus search input | When list panel is visible |

### 10.3 Right-Click Context Menus

Not implemented for MVP. Standard browser context menu should not be suppressed.

### 10.4 Text Selection

Allow text selection on all read-only content (quote details, client info, amounts). Disable text selection on interactive elements (buttons, chips, nav items) via `user-select: none`.

---

## 11. Implementation Notes

### 11.1 Framework Mapping — Desktop-Specific Patterns

| Concept | XAML / WinUI | React / Web | Flutter |
|---------|-------------|-------------|---------|
| Navigation rail | `NavigationView` with `PaneDisplayMode="LeftCompact"` | Flex column with fixed width | `NavigationRail` widget |
| Master-detail | `TwoPaneView` or manual `Grid` columns | CSS flexbox with fixed + flex children | `Row` with constrained + expanded children |
| Modal dialog | `ContentDialog` | `position: fixed` overlay + centered div | `showDialog` / `AlertDialog` |
| Independent scroll | `ScrollViewer` per panel | `overflow-y: auto` per panel div | `ListView` / `SingleChildScrollView` per panel |
| Hover states | `PointerEntered` / `PointerExited` visual states | CSS `:hover` pseudo-class | `MouseRegion` / `InkWell` hover callbacks |
| Focus ring | `FocusVisualPrimaryBrush` / `FocusVisualSecondaryBrush` | CSS `:focus-visible` outline | `FocusNode` + custom border painting |
| Grid layout | `Grid` with `ColumnDefinitions` | CSS Grid | `GridView` or manual `Row` + `Expanded` |
| Thin scrollbar | `ScrollViewer` theming or `ScrollBar` styling | CSS `scrollbar-width: thin` + `::-webkit-scrollbar` | `Scrollbar` with custom `ScrollbarThemeData` |

### 11.2 Performance Considerations

- The quote list and line items table should use virtualized/recycled rendering for lists exceeding ~50 items.
- Modals should be rendered lazily (only when opened), not hidden with `display: none`.
- Independent scroll panels should use `will-change: scroll-position` sparingly and only on panels that are actively scrolling.

### 11.3 Testing Priorities

- Test at exactly 905px, 1024px, 1280px, 1440px, and 1920px widths.
- Verify list panel + detail panel do not overlap or cause horizontal scroll at 905px.
- Verify sidebar collapses below main content at the Wide breakpoint.
- Verify all hover states with mouse, and that they do not appear on touch devices.
- Tab through the entire application — every interactive element must be reachable and have a visible focus indicator.
- Open and close modals with both mouse and keyboard (Escape). Verify focus trap.
- Verify scrollbars appear independently per panel and do not affect the other panel.
- Test with 50+ quotes in the list to verify scroll performance.

### 11.4 Relationship to Mobile Prototype

The desktop prototype (React JSX) accompanying this document serves as a **visual reference only**. It demonstrates the layout patterns, component hierarchy, and interaction behaviors described in this brief. It is not production code and should not be ported directly.

The mobile prototype remains the primary reference for components that are visually identical across breakpoints (status badges, color tokens, typography, card structure). This desktop brief documents only the differences in layout, navigation, interaction model, and density.
