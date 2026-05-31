# QuoteCraft — Design Brief

**Version 1.0 — February 2026**
**Framework & Language Agnostic Specification**

---

## Table of Contents

1. [Product Overview](#1-product-overview)
2. [Design System](#2-design-system)
3. [Responsive Layout Strategy](#3-responsive-layout-strategy)
4. [Screen Specifications](#4-screen-specifications)
5. [Interaction Patterns & Micro-behaviors](#5-interaction-patterns--micro-behaviors)
6. [Data Model](#6-data-model-conceptual)
7. [PDF Generation Specification](#7-pdf-generation-specification)
8. [Accessibility Requirements](#8-accessibility-requirements)
9. [Iconography](#9-iconography)
10. [Empty States & Error States](#10-empty-states--error-states)
11. [Monetization & Tier Boundaries](#11-monetization--tier-boundaries)
12. [Implementation Notes](#12-implementation-notes)

---

## 1. Product Overview

### 1.1 Purpose

QuoteCraft is a mobile-first application that enables independent contractors (plumbers, electricians, HVAC technicians, carpenters, general contractors) to create, send, and manage professional client-facing quotes from any device. The application replaces handwritten estimates and clunky spreadsheet workflows with a streamlined tool purpose-built for the trades.

### 1.2 Core Value Proposition

- Create a professional, branded quote in under 2 minutes
- Maintain a reusable catalog of trade-specific line items with default pricing
- Track quote status from draft through acceptance or decline
- Generate polished PDF output that builds client confidence and wins jobs
- Work offline with automatic sync when connectivity returns

### 1.3 Design Principles

**Speed over polish.** Every interaction is optimized for the fewest possible taps. Contractors create quotes on job sites, in truck cabs, between appointments. If it takes longer than paper, they won't use it.

**Clarity over cleverness.** Every screen answers the question: "What do I do next?" No ambiguous icons, no hidden gestures, no jargon. Labels are explicit, actions are obvious.

**Professional output from simple input.** The generated PDF should look better than anything the contractor could produce in Word or Excel. The app does the design work so the contractor doesn't have to.

### 1.4 Target User Profile

| Factor | Reality |
|--------|---------|
| Primary device | Phone (70%), tablet (20%), desktop (10%) |
| Environment | Job sites (bright sun, dust, noise), truck cab, kitchen table |
| Physical context | Dirty or calloused hands, may be wearing gloves. Large touch targets mandatory. |
| Tech comfort | Moderate. Comfortable with texting, social media, possibly QuickBooks. Not power users. |
| Time budget | 2–3 minutes maximum to create a quote. Exceeding this loses them to paper. |
| Primary goal | Look professional to the client. Win the job. Get paid. |

---

## 2. Design System

All visual specifications in this section define the canonical design language. Implementations should map these values to their framework's theming system (resource dictionaries, CSS custom properties, design tokens, theme objects, etc). No color, spacing, or type size should appear as a hardcoded literal outside the theme definition.

### 2.1 Theme Foundation

The design system is built on Material Design 3 principles. This choice is intentional: the contractor audience skews heavily Android, Material provides strong default touch targets and elevation hierarchy, and most cross-platform frameworks offer Material component libraries. Implementations should use their framework's Material toolkit where available.

### 2.2 Color Palette

Colors convey trust and professionalism while carrying a subtle industrial quality appropriate to the trades. Every color must be defined as a named token in the theme. No hardcoded hex values in UI markup.

| Swatch | Name | Token | Hex | Usage |
|--------|------|-------|-----|-------|
| ![#2B4C7E](https://placehold.co/24x24/2B4C7E/2B4C7E) | Primary | `PrimaryColor` | `#2B4C7E` | App bar, primary buttons, active states, links |
| ![#D6E4F0](https://placehold.co/24x24/D6E4F0/D6E4F0) | Primary Container | `PrimaryContainerColor` | `#D6E4F0` | Selected items, active chips, hover states |
| ![#F59E0B](https://placehold.co/24x24/F59E0B/F59E0B) | Secondary (Amber) | `SecondaryColor` | `#F59E0B` | Call-to-action buttons, accent highlights, warnings |
| ![#FEF3C7](https://placehold.co/24x24/FEF3C7/FEF3C7) | Secondary Container | `SecondaryContainerColor` | `#FEF3C7` | Quote total background, notification badges |
| ![#FFFFFF](https://placehold.co/24x24/FFFFFF/FFFFFF) | Surface | `SurfaceColor` | `#FFFFFF` | Cards, sheets, modal backgrounds |
| ![#F3F4F6](https://placehold.co/24x24/F3F4F6/F3F4F6) | Surface Variant | `SurfaceVariantColor` | `#F3F4F6` | Page backgrounds, input fields (unfocused), dividers |
| ![#1F2937](https://placehold.co/24x24/1F2937/1F2937) | On Surface | `OnSurfaceColor` | `#1F2937` | Primary text, headings, icons on light backgrounds |
| ![#6B7280](https://placehold.co/24x24/6B7280/6B7280) | On Surface Variant | `OnSurfaceVariantColor` | `#6B7280` | Secondary text, labels, timestamps, placeholders |
| ![#DC2626](https://placehold.co/24x24/DC2626/DC2626) | Error | `ErrorColor` | `#DC2626` | Declined quotes, validation errors, destructive actions |
| ![#16A34A](https://placehold.co/24x24/16A34A/16A34A) | Success | `SuccessColor` | `#16A34A` | Accepted quotes, confirmation messages, positive indicators |
| ![#D1D5DB](https://placehold.co/24x24/D1D5DB/D1D5DB) | Outline | `OutlineColor` | `#D1D5DB` | Card borders, input outlines, dividers |
| ![#E5E7EB](https://placehold.co/24x24/E5E7EB/E5E7EB) | Outline Variant | `OutlineVariantColor` | `#E5E7EB` | Subtle separators, internal dividers within cards |

**Dark mode:** Not prioritized for MVP. Contractors primarily work in daylight. Plan for dark mode in v2 by ensuring all colors are referenced through tokens, never hardcoded.

### 2.3 Typography

Use the framework's Material type scale. Do not define custom font sizes outside the type scale.

| Element | Material Style Name | Approximate Size | Usage |
|---------|-------------------|-----------------|-------|
| Page title | Title Large | 22sp / 22px | "My Quotes," "New Quote," "Settings" |
| Section header | Title Medium | 16sp / 16px | "Line Items," "Client Info," "Business Info" |
| Body text | Body Large | 16sp / 16px | Quote descriptions, notes, form values |
| Labels | Body Medium | 14sp / 14px | Input labels, status text, metadata |
| Captions | Body Small | 12sp / 12px | Timestamps, secondary info, helper text |
| Quote total | Headline Medium | 28sp / 28px | The prominent total amount on quotes |
| Monospace amounts | Platform monospace | Varies | All currency values use a monospaced font for column alignment |

### 2.4 Spacing Scale

All spacing follows an 8px base grid. Use the framework's layout system (stack panels, flex containers, auto-layout) with spacing and padding properties. Never set margin directly on child elements.

| Token | Value | Usage |
|-------|-------|-------|
| `xs` | 4px / 4dp | Tight internal spacing (icon to label within a badge) |
| `sm` | 8px / 8dp | Between tightly related items (icon + text pairs) |
| `md` | 16px / 16dp | Card internal padding, gap between form fields |
| `lg` | 24px / 24dp | Between content sections within a screen |
| `xl` | 32px / 32dp | Page margins on mobile viewports |
| `2xl` | 48px / 48dp | Major section separators, empty state padding |

### 2.5 Touch Targets & Interactive Elements

These are absolute minimums. The primary user base has large hands, may be wearing gloves, and operates the app in unstable physical environments.

| Element | Minimum Size | Preferred Size | Notes |
|---------|-------------|---------------|-------|
| All interactive elements | 48×48dp | — | WCAG / Material baseline. Non-negotiable. |
| Primary action buttons | 48dp height | 56dp height | "Send to Client," "Add to Quote," "Save" |
| List item rows | 56dp height | 64dp height | Quote cards, client rows, catalog items |
| FAB | 56×56dp | 56×56dp | Floating action button for primary creation action |
| Icon buttons | 48×48dp | — | Back, close, delete. Include adequate padding around icon. |
| Chip / filter buttons | 36dp height | 40dp height | Status filter chips in the chip bar |
| Stepper buttons (+/−) | 44×44dp | 48×48dp | Quantity increment/decrement in line item editor |

### 2.6 Elevation & Shadows

Use the framework's elevation or shadow system. Four levels are sufficient.

| Level | Element | Shadow Description |
|-------|---------|-------------------|
| Level 1 (low) | Cards, list items, form sections | Soft, barely perceptible. 1px vertical offset, 3px blur, ~8% opacity. |
| Level 2 (medium) | Hovered cards, elevated sheets | Noticeable on interaction. 4px vertical offset, 12px blur, ~8% opacity. |
| Level 3 (high) | FAB, bottom sheets | Prominent floating appearance. 12px vertical offset, 32px blur, ~12% opacity. |
| Level 4 (highest) | Modal dialogs, overlays | Maximum separation. 20px vertical offset, 48px blur, ~16% opacity. |

### 2.7 Border Radius

| Token | Value | Usage |
|-------|-------|-------|
| Small | 8px / 8dp | Input fields, small buttons, inner containers |
| Medium | 12px / 12dp | Cards, form sections, action buttons |
| Large | 16px / 16dp | FAB, larger containers |
| Extra Large | 24px / 24dp | Bottom sheet top corners, modal dialogs |
| Full / Pill | 50% or 9999px | Chips, badges, avatar circles, pill buttons |

---

## 3. Responsive Layout Strategy

### 3.1 Breakpoints

| Breakpoint | Width Range | Typical Device | Layout |
|-----------|------------|---------------|--------|
| Narrow | 0–599px | Phone (portrait) | Single column, bottom tab bar, stacked cards, full-width forms |
| Medium | 600–904px | Tablet (portrait) | Single column, wider cards with increased side margins, bottom tab bar |
| Wide | 905–1280px | Tablet (landscape), small laptop | Two-column: quote list + detail side-by-side. Navigation rail replaces bottom bar. |
| Extra Wide | 1281px+ | Desktop / large laptop | Three-column with persistent navigation rail. Maximum information density. |

### 3.2 Navigation Pattern

| Viewport | Navigation Component | Tabs / Items |
|----------|---------------------|-------------|
| Narrow + Medium | Bottom Tab Bar | Quotes, Clients, Catalog, Settings (4 items, icons + labels) |
| Wide + Extra Wide | Navigation Rail (left side) | Same 4 destinations, vertical layout, icons + labels |

The bottom tab bar is fixed at the bottom of the viewport and does not scroll with content. The active tab is indicated by a filled icon and a 3px accent bar above the icon. Inactive tabs use the On Surface Variant color; the active tab uses the Primary color.

### 3.3 Device Frame Context

The reference prototype renders inside a phone device frame (430px content width, ~85vh height) to simulate the primary usage context. For implementation, the app shell should be a responsive container that adapts from this phone-first layout upward through the breakpoints defined above.

---

## 4. Screen Specifications

Each screen is described in terms of its purpose, user intent, component hierarchy, content, behavior, and edge cases. All screens share the common app bar and bottom navigation described in Sections 3.2 and 4.0.

### 4.0 App Shell (Shared Chrome)

#### 4.0.1 App Bar

- Background: Primary color. Full-width, fixed at top of viewport.
- Height: ~56dp including content padding.
- Left side: Page title (white, Title Large style). On sub-screens, a back arrow (chevron-left) precedes the title.
- Right side: Contextual action buttons (pill-shaped, semi-transparent white background at 15% opacity, white text). E.g. "+ New" on Dashboard, "Save" on Builder.
- Bottom edge: A 3px-tall accent bar in Secondary (Amber) color spans the full width. This is a signature brand element.

#### 4.0.2 Bottom Tab Bar (Narrow/Medium)

- Background: Surface (white). Top border: 1px Outline Variant.
- 4 items evenly distributed: Quotes, Clients, Catalog, Settings.
- Each item: icon (22px) above label (11px). Inactive: On Surface Variant color. Active: Primary color with a 3px top accent bar.
- Tab bar is always visible and does not scroll with content. It sits below the scrollable content area.

---

### 4.1 Dashboard / Quote List

#### 4.1.1 Purpose & User Intent

This is the home screen. It answers: "What quotes need my attention?" The contractor lands here every time they open the app and needs to quickly find a specific quote or create a new one.

#### 4.1.2 Layout Structure (top to bottom)

**App Bar:** Title "QuoteCraft" on the left. "+ New" accent pill button on the right.

**Chip Filter Bar:** Horizontally scrollable row of filter chips. Sits between the app bar and the list. Background: Surface (white). Bottom border: 1px Outline Variant.

- Chips: All (default active), Draft, Sent, Viewed, Accepted, Declined.
- Active chip: Primary Container background, Primary border, Primary text, bold weight.
- Inactive chip: Surface background, Outline border, On Surface Variant text.
- Each chip (except "All") shows a count badge indicating the number of quotes in that status.

**Quote List:** Vertically scrollable list of quote cards. Padding: 12px horizontal, 10px vertical gap between cards. Supports pull-to-refresh for sync.

**FAB:** Floating action button anchored to bottom-right, above the tab bar. 56×56dp, border radius Large (16dp), Secondary (Amber) background. "+" icon. Shadow Level 3. Tapping opens the Quote Builder with a blank quote.

#### 4.1.3 Quote Card Specification

- Background: Surface. Border: 1px Outline Variant. Border radius: Medium (12dp). Shadow: Level 1. Padding: 16dp all sides.
- Top row: Quote title (left, 15px semibold) and status badge (right). Below title: client name (13px, On Surface Variant).
- Bottom row: separated from top by 1px Outline Variant divider with 10dp top padding. Amount (left, 18px bold monospace, Primary color) and relative date (right, 12px On Surface Variant).
- Tap: navigates to Quote Builder for that quote. Hover: elevates to Shadow Level 2, translates up 1px. Press: scales to 98.5%.

#### 4.1.4 Status Badge Colors

| Status | Background | Text Color | Icon |
|--------|-----------|------------|------|
| Draft | `#E5E7EB` (gray) | `#374151` (dark gray) | ✎ (pencil) |
| Sent | `#DBEAFE` (light blue) | `#1E40AF` (blue) | ↗ (arrow) |
| Viewed | `#FEF3C7` (light amber) | `#92400E` (brown) | 👁 (eye) |
| Accepted | `#D1FAE5` (light green) | `#065F46` (green) | ✓ (check) |
| Declined | `#FEE2E2` (light red) | `#991B1B` (red) | ✕ (cross) |

#### 4.1.5 Empty State

When no quotes exist (or filtered list is empty): centered layout with a large muted icon (48px, 40% opacity), title "No quotes yet" (16px semibold), description "Create your first quote in 60 seconds" (14px, On Surface Variant), and a Primary action button "Create Quote." For filtered empty states, the text adapts: "No [status] quotes" with description "Quotes with [status] status will appear here."

---

### 4.2 Quote Builder (Core Screen)

#### 4.2.1 Purpose & User Intent

This is the most important screen in the application. It is where quotes are built line by line. The contractor needs to specify a client, name the job, add priced line items, and see a running total. Every change auto-saves locally.

#### 4.2.2 Layout Structure (top to bottom)

**App Bar:** Back arrow + title ("New Quote" or "Edit Quote") on left. "Save" button on right.

**Scrollable Form Body:** Contains the following card sections, each separated by 12px vertical gap with 16px horizontal margin.

#### 4.2.3 Form Sections

**Section 1 — Client:** A single dropdown/combo box with search capability. Options populated from the client list. Includes an "Add New" option that opens inline client creation. Label: uppercase 11px semibold "CLIENT" in On Surface Variant.

**Section 2 — Details:** Two fields: Title (free text, placeholder "e.g. Kitchen Renovation") and Valid Until (date input, defaults to today + configured validity period from Settings). Label style matches Section 1.

**Section 3 — Line Items:** The core of the quote. Section label includes a count badge showing number of items. Contains:

- Line item rows: each row shows description (14px, semibold, truncated with ellipsis), unit price × quantity (12px, On Surface Variant), line total (15px, semibold, monospace), and a delete button (✕ icon, 48dp target, hover reveals red background).
- "+ Add Line Item" button: full-width dashed border, centered text with + icon. Opens the Line Item Editor bottom sheet.
- "Add from Catalog" button: full-width solid border in Secondary color tones. Opens the Catalog Browser bottom sheet.

**Section 4 — Notes:** A multi-line text area (minimum 3 rows) for scope descriptions, exclusions, terms, and conditions. Placeholder: "Add notes for the client..."

**Section 5 — Totals:** Calculated automatically from line items. Displays Subtotal, Tax (rate from Settings, shown as percentage), and Grand Total. The grand total row is visually separated by a 2px top border, uses larger text (20–22px bold) in Primary color with monospaced figures.

#### 4.2.4 Bottom Action Bar

Fixed at the bottom, above the tab bar (or at the absolute bottom on sub-screens where the tab bar is hidden). Contains two buttons side by side:

- "Preview" (secondary style: Surface background, Primary border and text, document icon)
- "Send" (accent style: Secondary/Amber background, dark text, arrow icon)

#### 4.2.5 Input Specifications

- All text inputs: 1.5px border in Outline color, radius Small (8dp), 12px vertical + 14px horizontal padding, 15px font size.
- Focus state: border color transitions to Primary.
- Labels: always visible above the input (12px, medium weight, On Surface Variant). Never rely solely on placeholder text for field identification.
- All form sections: Surface background, radius Medium (12dp), Shadow Level 1, 1px Outline Variant border, 16dp internal padding.

---

### 4.3 Line Item Editor (Bottom Sheet)

#### 4.3.1 Purpose

A modal bottom sheet for adding or editing individual line items. Opens from the Quote Builder. Must be fast: a contractor should be able to add an item in 3–4 taps.

#### 4.3.2 Bottom Sheet Behavior

- Overlays the current screen with a semi-transparent dark scrim (rgba 0,0,0 at 40% opacity).
- Slides up from the bottom of the viewport with a spring animation (cubic-bezier 0.22, 1, 0.36, 1, ~300ms).
- Top corners: radius Extra Large (24dp). Maximum height: 80% of viewport.
- Has a centered drag handle bar (36px wide, 4px tall, Outline color, 2px radius) at the top.
- Header: Title on the left ("Add Line Item", 18px semibold), close button on the right (32dp circle, Surface Variant background, ✕ icon).
- Tapping the scrim dismisses the sheet.

#### 4.3.3 Form Fields

**Description:** Free text input. Auto-focused on open.

**Unit Price:** Numeric input with a `$` prefix indicator positioned inside the field (absolute-positioned, On Surface Variant color). Input has left padding to accommodate the prefix.

**Quantity:** A stepper component with − button, numeric display, and + button.

- Container: 1.5px Outline border, radius Small (8dp), overflow hidden.
- Buttons: 44×44dp each, Surface Variant background, Primary color text (bold − and +).
- Value display: 56px wide center section with 1.5px Outline left and right borders. 16px semibold text, centered.
- Interactions: hover changes button to Primary Container. Active press fills with Primary and inverts text to white.

**Line Total:** Calculated display (not editable). Shows as a summary row with 1.5px top border. Label on left (16px semibold), value on right (20px bold monospace, Primary color).

**Submit Button:** "Add to Quote" — full-width, Primary background, white text, 14px height. Disabled (grayed out) until both description and price have values.

---

### 4.4 Trade Catalog Browser (Bottom Sheet)

#### 4.4.1 Purpose

A bottom sheet that presents pre-built pricing items organized by trade and category. Enables rapid quote population by tapping items from a saved catalog rather than typing each line item manually.

#### 4.4.2 Layout

- Same bottom sheet container as the Line Item Editor (Section 4.3.2), but taller (maxHeight: 85%).
- Header: "Plumbing Catalog" (or trade-appropriate title), close button.
- Search bar: full-width input with a search icon (magnifier) positioned inside on the left. Surface Variant background, transitions to Surface with Primary border on focus.
- Category chips: horizontally wrapping row of small chips ("All", "Installations", "Repairs", "Materials", "Labor"). Active chip: Primary background, white text. Inactive: Surface background, Outline border.
- Item list: grouped by category (when "All" selected). Group headers: uppercase 11px bold, On Surface Variant, with a 1px bottom border.

#### 4.4.3 Catalog Item Row

- Full-width row with 14px vertical padding, 1px bottom border Outline Variant.
- Left: item description (14px, medium weight).
- Right: price (14px semibold monospace, On Surface Variant) followed by an add button (36dp circle, Primary background, white "+" icon).
- Add interaction: tapping the + button immediately adds the item to the current quote with quantity 1. The button briefly transitions to a green checkmark (Success color) for 1.5 seconds as confirmation, then reverts. A toast notification ("✓ Item added") appears at the bottom.
- Prices are defaults. After adding, the contractor can edit price and quantity from the Quote Builder.

---

### 4.5 Quote Preview / PDF View

#### 4.5.1 Purpose

Shows exactly what the client will receive. This is the contractor's calling card. It must look better than anything they could make in Word. This screen serves both as an in-app preview and as the template for generated PDFs.

#### 4.5.2 Document Layout

**Header Block:** Full-width Primary color background with generous padding (24dp horizontal, 24dp vertical).

- Top row: Business logo (48dp square, rounded corners, semi-transparent white background as placeholder) on the left. Business name, phone, and email (right-aligned, white text) on the right.
- Bottom row (below a 1px semi-transparent white divider): Three columns showing Quote Number, Date, and Valid Until. Each column has a muted label above and bold value below.

**Client Section:** "Prepared For" label (uppercase, 11px), followed by client name (16px semibold) and address (13px muted). Separated from the next section by a 1px Outline Variant bottom border.

**Quote Title:** 18px bold, On Surface color. Appears below client info.

**Line Items Table:**

- Header row: uppercase column labels (Description, Qty, Price, Total) with 2px bottom border On Surface.
- Data rows: alternating background (white / `#F9FAFB`). 1px Outline Variant bottom borders. Description column is left-aligned; Qty is centered; Price and Total are right-aligned with monospaced font.
- Column proportions: Description takes ~50%, Qty ~10%, Price ~20%, Total ~20%.

**Totals Block:** Right-aligned. Subtotal and Tax rows in standard weight. Grand Total row separated by 2px top border, larger text (18px bold), On Surface color. All amounts in monospace.

**Notes Block:** Surface Variant background, radius Small (8dp), 16dp padding. "Notes" label (12px uppercase bold) above body text (13px, On Surface Variant, 1.6 line height).

**Footer:** Centered, 11px muted text: "Created with QuoteCraft" (free tier). Pro tier replaces with custom footer text.

#### 4.5.3 Action Bar

Two buttons in the fixed bottom action bar:

- "Download PDF" (secondary style, download icon)
- "Email Client" (primary style, email icon)

---

### 4.6 Client List

#### 4.6.1 Purpose

Simple contact management. Contractors need to quickly find a client when building a quote, and have a sense of their quoting history with each client.

#### 4.6.2 Layout

- App Bar: Title "Clients" on left, "+ Add" accent button on right.
- Search Bar: Positioned below the app bar. Full-width input with search icon, same styling as catalog search.
- Client List: Vertically scrollable, same card list pattern as quotes.

#### 4.6.3 Client Card Specification

- Horizontal layout: Avatar (left) + Info (center, flex) + Value (right).
- Avatar: 44dp circle, Primary Container background, Primary text. Shows initials (first letter of first + last name, bold 16px).
- Info: Client name (15px semibold) above metadata line (12px, On Surface Variant) showing quote count and city.
- Value: Right-aligned. Total value (14px semibold monospace, Primary color) above "total value" label (11px, On Surface Variant).
- Card styling: Surface background, radius Medium, Shadow Level 1, 1px Outline Variant border, 16dp padding, 14dp gap between elements.

---

### 4.7 Catalog Management (Standalone Tab)

#### 4.7.1 Purpose

Browse and manage the full pricing catalog outside of the quote builder context. The contractor uses this to review, update, and organize their standard pricing.

#### 4.7.2 Layout

- App Bar: Title "Catalog" on left, "+ Item" accent button on right.
- Search Bar: Same as Client List search.
- Category Chip Bar: Same chips as the catalog bottom sheet, but rendered inline below the search bar with horizontal scrolling.
- Item List: Organized by category. Each item shows description, price, and a chevron-right (">") indicator instead of the add button. Tapping opens an edit view for that catalog item.

---

### 4.8 Settings

#### 4.8.1 Purpose

Business configuration and application preferences. Contractors set this up once and rarely return.

#### 4.8.2 Section Structure

Settings uses a grouped-row pattern common on mobile platforms. Each group has an uppercase label above a card containing rows.

**Group 1 — Business Info:** Business Name, Phone, Email, Address, Logo (upload button). Values displayed as right-aligned text. Tapping a row opens editing.

**Group 2 — Quote Settings:** Default Tax Rate (numeric input + % suffix), Quote Validity Period (numeric input + "days" suffix), Currency (dropdown, default USD), Quote Number Prefix (text input, default "QC-"). Inline editing where practical.

**Group 3 — Catalogs:** Lists all saved catalogs (e.g. "Plumbing — 18 items") with item count. "+ Add Catalog" row at the bottom.

**Upgrade Banner:** Gradient background (Primary to dark navy), white text. Title: "Unlock QuoteCraft Pro." Description and a prominent Amber CTA button. Only shown to free-tier users.

**Group 4 — Subscription:** Current Plan (e.g. "Free (5 quotes/mo)") and Quotes Used (e.g. "3 of 5").

#### 4.8.3 Settings Row Specification

- Full-width row: 14dp vertical padding, 16dp horizontal padding.
- Label on left (14px, medium weight, On Surface). Value/input on right (14px, On Surface Variant, right-aligned, max 55% width with ellipsis overflow).
- Rows separated by 1px Outline Variant bottom border. Last row in group has no border.
- Group card: Surface background, radius Medium, Shadow Level 1, 1px Outline Variant border.

---

## 5. Interaction Patterns & Micro-behaviors

### 5.1 Navigation

| Action | Behavior |
|--------|----------|
| Tab bar tap | Switches to the target screen. Resets that screen's navigation stack. No animation between tab screens. |
| Back button / gesture | Pops the current sub-screen and returns to the previous screen in the navigation stack. Slide-right transition. |
| Card tap (Dashboard) | Pushes the Quote Builder with that quote's data. Slide-left transition. |
| FAB tap | Pushes the Quote Builder with a blank new quote. Slide-left transition. |

### 5.2 Toast Notifications

- Appears centered above the bottom tab bar (or action bar).
- Dark background (`#1F2937`), white text, pill-shaped (24dp radius), 12dp vertical + 24dp horizontal padding.
- Entrance: slides up 20px and fades in over 300ms. Exit: fades out over 300ms after a 2-second hold.
- Used for: confirmations ("Item added"), save acknowledgments, and non-critical notifications. Never for errors (those use inline validation).

### 5.3 Bottom Sheet Patterns

- Scrim: instant fade-in (200ms). Sheet: slide-up spring animation (300ms).
- Dismiss: tap scrim, tap close button, or swipe down (stretch goal for v1).
- Sheets are modal: no interaction with underlying content while open.
- Content inside sheets scrolls independently if it overflows the max height.

### 5.4 Form Behavior

- Auto-save: every change in the Quote Builder saves to local storage immediately. No explicit save action required (the "Save" button provides manual reassurance).
- Validation: inline. Error states show Error color on input borders with helper text below. Never block with modal alerts.
- Focus management: opening the Line Item Editor auto-focuses the Description field. Numeric fields use numeric keyboard where platform supports it.

### 5.5 Animations

Keep animations minimal and functional. Nothing decorative.

| Element | Animation |
|---------|-----------|
| Page transitions | Slide left (push) / slide right (pop), ~250ms ease-out |
| Bottom sheets | Slide up from bottom, spring curve, ~300ms |
| Cards on hover | Translate Y −1px + shadow increase, ~200ms ease |
| Cards on press | Scale to 0.985, ~100ms |
| FAB hover | Scale to 1.05 + shadow increase, ~200ms |
| Status badge change | Background color cross-fade, ~200ms |
| Toast | Slide up 20px + fade in (300ms), fade out after 2s hold (300ms) |
| Catalog add button | Background color transition to Success green (150ms), revert after 1.5s |
| Loading states | Skeleton screens (pulsing placeholder shapes), not spinners. Exception: sync indicator may use a spinner. |

---

## 6. Data Model (Conceptual)

This section describes the entities and their relationships. Implementation details (database schema, serialization format) are left to the development team.

### 6.1 Quote

| Field | Type | Notes |
|-------|------|-------|
| `id` | Unique identifier | Auto-generated |
| `number` | String | Auto-generated from prefix + year + sequence (e.g. QC-2026-0042) |
| `title` | String | User-provided job description |
| `status` | Enum | `draft` \| `sent` \| `viewed` \| `accepted` \| `declined` |
| `clientId` | Reference | Links to Client entity |
| `items` | Collection | Ordered list of LineItem entities |
| `notes` | String (multiline) | Free-text notes for the client |
| `validUntil` | Date | Quote expiration date |
| `taxRate` | Decimal | Snapshot of tax rate at time of creation |
| `subtotal` | Decimal (calculated) | Sum of all line item totals |
| `tax` | Decimal (calculated) | subtotal × taxRate |
| `total` | Decimal (calculated) | subtotal + tax |
| `createdAt` | Timestamp | Auto-set on creation |
| `updatedAt` | Timestamp | Auto-updated on every change |

### 6.2 Line Item

| Field | Type | Notes |
|-------|------|-------|
| `id` | Unique identifier | Auto-generated |
| `description` | String | Item name / service description |
| `unitPrice` | Decimal | Price per unit |
| `quantity` | Integer | Number of units (minimum 1) |
| `lineTotal` | Decimal (calculated) | unitPrice × quantity |
| `sortOrder` | Integer | Position within the quote |

### 6.3 Client

| Field | Type | Notes |
|-------|------|-------|
| `id` | Unique identifier | Auto-generated |
| `name` | String | Full name or business name |
| `address` | String | Street address, city, state |
| `phone` | String | Phone number |
| `email` | String | Email address |

### 6.4 Catalog Item

| Field | Type | Notes |
|-------|------|-------|
| `id` | Unique identifier | Auto-generated |
| `description` | String | Item / service name |
| `unitPrice` | Decimal | Default price (editable after adding to a quote) |
| `category` | String | Grouping category (e.g. Installations, Repairs, Materials) |
| `catalogId` | Reference | Which catalog this item belongs to |
| `sortOrder` | Integer | Position within category |

### 6.5 Business Settings

| Field | Type | Notes |
|-------|------|-------|
| `businessName` | String | Company name displayed on quotes |
| `phone` | String | Business phone |
| `email` | String | Business email |
| `address` | String | Business address |
| `logoUri` | String / Binary | Path or data for business logo |
| `defaultTaxRate` | Decimal | Applied to new quotes (default 8.5%) |
| `quoteValidityDays` | Integer | Default validity period (default 14) |
| `currency` | String | Currency code (default USD) |
| `quotePrefix` | String | Prefix for quote numbers (default "QC-") |

---

## 7. PDF Generation Specification

The generated PDF is the contractor's calling card. It must look professional, be printable, and render identically across all PDF viewers.

### 7.1 Page Setup

- Page size: US Letter (8.5" × 11"), portrait orientation.
- Margins: 0.75" all sides (reduced to maximize content area).
- Font: A clean sans-serif. Use the same font family as the app where possible.

### 7.2 PDF Layout Sections

The PDF layout mirrors the Quote Preview screen (Section 4.5.2). Key additions for the print/PDF context:

- The header block uses the actual business logo image (not a placeholder).
- Line items table uses alternating row shading for readability on paper.
- Footer includes the "Created with QuoteCraft" branding (free tier) or custom footer text (Pro tier, e.g. "Licensed & Insured — License #12345").

### 7.3 Pro Tier Branding

- Custom logo placement in the header.
- Primary color applied to the header bar background and accent lines.
- Custom footer text replaces the QuoteCraft branding.
- "Created with QuoteCraft" watermark removed.

---

## 8. Accessibility Requirements

| Requirement | Specification |
|-------------|--------------|
| Touch targets | Minimum 48×48dp for ALL interactive elements. Primary actions prefer 56dp. |
| Color contrast | 4.5:1 ratio for body text on backgrounds. 3:1 for large text (18sp+ or 14sp+ bold). All palette colors validated against their intended surface. |
| Screen reader | Every interactive element has an accessible name (automation properties / aria-label / content description). Status badges announce their text, not just display it visually. |
| Focus order | Logical reading order via tab index. Forms follow top-to-bottom field order. Bottom sheets trap focus when open. |
| Visible labels | Every input has a persistent visible label. Placeholder text supplements but never replaces labels. |
| Localization | All user-visible strings use resource keys / localization IDs. No hardcoded text in UI markup. |
| Reduced motion | If the platform provides a reduced-motion preference, suppress all non-essential animations. Functional transitions (sheet open/close) reduce to instant cut. |
| Color independence | Status is never communicated by color alone. Badges include both color AND text/icon. Error states use both red border AND helper text. |

---

## 9. Iconography

Use the framework's built-in icon set (Fluent, Material Symbols, SF Symbols, etc). Consistency within a single icon family is more important than which family is chosen.

| Action | Semantic Description | Example Names |
|--------|---------------------|--------------|
| Create quote | Plus sign | `Add`, `Plus`, `PlusCircle` |
| Edit item | Pencil / pen | `Edit`, `Pencil`, `Create` |
| Delete item | Trash can | `Delete`, `Trash`, `Remove` |
| Send quote | Paper airplane / outbound arrow | `Send`, `PaperPlane`, `ArrowUpRight` |
| PDF / document | Page / file | `Document`, `File`, `FileText` |
| Client / person | Person silhouette | `Person`, `User`, `Contact` |
| Settings | Gear / cog | `Settings`, `Gear`, `Cog` |
| Catalog / library | Book / grid | `Library`, `Book`, `Grid` |
| Search | Magnifying glass | `Search`, `Magnifier`, `Find` |
| Back / navigate | Left chevron / arrow | `ChevronLeft`, `ArrowLeft`, `Back` |
| Close / dismiss | X mark | `Close`, `X`, `Dismiss` |
| Download | Down arrow into tray | `Download`, `ArrowDown`, `Save` |

---

## 10. Empty States & Error States

### 10.1 Empty List States

Every list screen provides guidance when there is no content. The pattern is consistent: a centered block containing a large muted icon (48px, 40% opacity), a title (16px semibold), a descriptive prompt (14px, On Surface Variant), and a single primary action button.

| Screen | Icon | Title | Description | Action Button |
|--------|------|-------|-------------|--------------|
| Quotes (no quotes) | Clipboard | No quotes yet | Create your first quote in 60 seconds. | Create Quote |
| Quotes (filtered empty) | Clipboard | No [status] quotes | Quotes with [status] status will appear here. | — |
| Clients | Person | No clients yet | Add your first client to start quoting. | Add Client |
| Catalog | Book | Catalog is empty | Add items to your pricing catalog for faster quoting. | Add Item |

### 10.2 Offline State

When the device lacks connectivity, a subtle banner appears at the top of the scrollable content area (below the app bar): "You're offline. Quotes will sync when connected." The banner uses a Surface Variant background with On Surface Variant text (14px). It does not block interaction; all features work offline against local storage.

### 10.3 Error States

- Form validation: Error color (red) border on the offending input + helper text below in Error color.
- Network errors: toast notification with retry suggestion.
- Sync conflicts: non-blocking notification with option to review.

---

## 11. Monetization & Tier Boundaries

| Feature | Free Tier | Pro Tier ($15/mo) |
|---------|-----------|-------------------|
| Quotes per month | 5 | Unlimited |
| Clients | Unlimited | Unlimited |
| Catalog items | Unlimited | Unlimited |
| PDF generation | Yes (with QuoteCraft branding) | Yes (custom branding) |
| Custom logo | No | Yes |
| Custom footer text | No | Yes |
| Custom primary color on PDF | No | Yes |
| Priority support | No | Yes |

The upgrade prompt appears in Settings and contextually when a free-tier user hits their monthly quote limit. The prompt should be informative, not aggressive. A simple gradient banner with clear value proposition and a single CTA button.

---

## 12. Implementation Notes

### 12.1 Framework Agnosticism

This specification is deliberately framework-agnostic. The design system, component hierarchy, spacing, and behavioral specifications apply regardless of whether the implementation uses XAML (WinUI / Uno Platform), SwiftUI, Jetpack Compose, React Native, Flutter, or web technologies. The interactive prototype accompanying this document was built in React JSX and serves as a visual reference only.

### 12.2 Key Mapping Guidance

| Concept in This Spec | XAML / WinUI Example | React / Web Example | Flutter Example |
|---------------------|---------------------|-------------------|----------------|
| Color token | `ResourceDictionary` `ThemeResource` | CSS custom property (`--primary`) | `ThemeData` color scheme |
| Type scale | `TextBlock Style="{StaticResource TitleLargeTextBlockStyle}"` | Typography variant / CSS class | `Theme.of(context).textTheme.titleLarge` |
| Spacing token | `AutoLayout.Spacing`, `AutoLayout.Padding` | CSS `gap`, `padding` with `var()` | `SizedBox` / `EdgeInsets` constant |
| Shadow / elevation | `Translation` + `ThemeShadow` | `box-shadow` CSS property | Material `elevation` / `BoxShadow` |
| Bottom sheet | `DrawerControl` / `ContentDialog` | `position: fixed` + slide animation | `showModalBottomSheet` |
| Chip group | `ChipGroup` (Uno Toolkit) | Flexbox row of buttons | `Wrap` + `FilterChip` |
| Navigation | `Frame.Navigate` / `NavigationView` | React Router / state machine | `Navigator.push` / `GoRouter` |

### 12.3 Local Storage

All data should persist locally using the platform's recommended local database (SQLite, Realm, Core Data, etc). The application must be fully functional offline. Sync to a remote backend is a stretch goal for v1 and a requirement for v2.

### 12.4 Testing Priorities

- Test all screens at 320px, 375px, 414px, 768px, and 1024px widths.
- Validate all touch targets with accessibility scanner.
- Test form flows with screen reader enabled.
- Validate color contrast with automated tooling.
- Test quote creation end-to-end in under 2 minutes.
- Test offline creation and later sync.
