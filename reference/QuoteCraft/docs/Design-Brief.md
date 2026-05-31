# QuoteCraft - Design Brief

## Design Philosophy

**Speed over polish. Clarity over cleverness. Professional output from simple input.**

Contractors create quotes on job sites, in trucks, between appointments. The UI must be operable with dirty hands, readable in sunlight, and navigable without thinking. Every screen should answer: "What do I do next?"

---

## Target User Context

| Factor | Reality |
|--------|---------|
| **Device** | Phone (70%), tablet (20%), desktop/laptop (10%) |
| **Environment** | Job sites (bright sun, dust, noise), truck cab, kitchen table |
| **Hands** | Often dirty, calloused, wearing gloves. Large touch targets mandatory. |
| **Tech comfort** | Moderate. Uses Facebook, texting, maybe QuickBooks. Not power users. |
| **Time** | 2-3 minutes max to create a quote. If it takes longer, they'll go back to paper. |
| **Goal** | Look professional to the client. Win the job. Get paid. |

---

## Design System

### Theme: Uno Platform Material (Material Design 3)

Material is the right choice here:
- Familiar to Android-heavy contractor audience
- Strong component library via Uno Toolkit
- Clear elevation and surface hierarchy
- Excellent touch target defaults

### Color Palette

Colors should convey trust and professionalism while nodding to the trades industry.

| Role | Token | Value | Usage |
|------|-------|-------|-------|
| **Primary** | `PrimaryColor` | Slate Blue (#2B4C7E) | App bar, primary buttons, active states |
| **Primary Container** | `PrimaryContainerColor` | Light Blue (#D6E4F0) | Selected items, active chips |
| **Secondary** | `SecondaryColor` | Amber (#F59E0B) | CTAs, accent highlights, warnings |
| **Secondary Container** | `SecondaryContainerColor` | Light Amber (#FEF3C7) | Quote total background, badges |
| **Surface** | `SurfaceColor` | White (#FFFFFF) | Cards, sheets, backgrounds |
| **Surface Variant** | `SurfaceVariantColor` | Warm Gray (#F3F4F6) | Page backgrounds, dividers |
| **On Surface** | `OnSurfaceColor` | Dark Gray (#1F2937) | Primary text |
| **On Surface Variant** | `OnSurfaceVariantColor` | Medium Gray (#6B7280) | Secondary text, labels |
| **Error** | `ErrorColor` | Red (#DC2626) | Declined quotes, validation errors |
| **Success** | Custom | Green (#16A34A) | Accepted quotes, confirmation |
| **Outline** | `OutlineColor` | Border Gray (#D1D5DB) | Card borders, input outlines |

**Dark mode:** Not prioritized for MVP. Contractors work in daylight. Add in v2 if requested.

**Important:** All colors defined in `ColorPaletteOverride.xaml`. Never use hardcoded hex in XAML.

### Typography

Use Material type scale via Uno Platform Material styles. No custom font sizes.

| Element | Style | Usage |
|---------|-------|-------|
| Page title | `TitleLargeTextBlockStyle` | "My Quotes," "New Quote" |
| Section header | `TitleMediumTextBlockStyle` | "Line Items," "Client Info" |
| Body text | `BodyLargeTextBlockStyle` | Quote descriptions, notes |
| Labels | `BodyMediumTextBlockStyle` | Input labels, status text |
| Captions | `BodySmallTextBlockStyle` | Timestamps, secondary info |
| Quote total | `HeadlineMediumTextBlockStyle` | The big number on the quote |

### Spacing Scale

Follow 8px grid. Use `AutoLayout` with `Spacing` and `Padding` - never set margin on children.

| Token | Value | Usage |
|-------|-------|-------|
| xs | 4px | Tight internal spacing |
| sm | 8px | Between related items (icon + label) |
| md | 16px | Card padding, section gaps |
| lg | 24px | Between sections |
| xl | 32px | Page margins (mobile) |
| 2xl | 48px | Major section separators |

### Touch Targets

- Minimum 48x48dp for all interactive elements
- Prefer 56dp height for primary action buttons
- List items: minimum 64dp row height
- FAB (Floating Action Button): 56dp

### Elevation

- Cards: `Translation="0,0,4"` with `ThemeShadow`
- FAB: `Translation="0,0,12"` with `ThemeShadow`
- Bottom sheet: `Translation="0,0,16"` with `ThemeShadow`
- Modal dialogs: `Translation="0,0,24"` with `ThemeShadow`

---

## Responsive Layout Strategy

### Breakpoints (via `Responsive` markup extension)

| Breakpoint | Width | Layout |
|------------|-------|--------|
| **Narrow** | 0-599px | Single column, bottom nav, stacked cards. Mobile phone. |
| **Medium** | 600-904px | Still single column but wider cards, side margins increase. Tablet portrait. |
| **Wide** | 905-1280px | Two-column layout. Quote list + detail side-by-side. Tablet landscape / small desktop. |
| **Extra Wide** | 1281px+ | Three-column with navigation rail. Full desktop experience. |

### Navigation Pattern

| Breakpoint | Navigation |
|------------|------------|
| Narrow + Medium | Bottom `TabBar` with 4 tabs: Quotes, Clients, Catalogs, Settings |
| Wide + Extra Wide | `NavigationView` rail on the left side |

---

## Key Screens

### 1. Dashboard / Quote List (Home)

The first thing contractors see. Answers: "What quotes need my attention?"

```
+----------------------------------+
|  QuoteCraft            [+ New]   |
+----------------------------------+
|  [All] [Draft] [Sent] [Accepted] |  <- ChipGroup filter
+----------------------------------+
|  +------------------------------+|
|  | Johnson Kitchen Reno    SENT  ||  <- Card with status badge
|  | $4,250  -  2 days ago        ||
|  +------------------------------+|
|  +------------------------------+|
|  | Smith Bathroom Fix   ACCEPTED ||
|  | $1,800  -  5 days ago        ||
|  +------------------------------+|
|  +------------------------------+|
|  | Garcia Deck Build     DRAFT   ||
|  | $7,500  -  Today             ||
|  +------------------------------+|
|                                   |
|              [+ Create Quote]     |  <- FAB
+----------------------------------+
| [Quotes] [Clients] [Cat.] [Set.] |  <- TabBar (mobile)
+----------------------------------+
```

**Key elements:**
- `ChipGroup` for status filtering (All, Draft, Sent, Viewed, Accepted, Declined)
- `ListView` with `Card` items showing: client name, description, amount, date, status badge
- Status badges color-coded: Draft (gray), Sent (blue), Viewed (amber), Accepted (green), Declined (red)
- FAB for "Create Quote" (primary action)
- Pull-to-refresh for sync
- Empty state: illustration + "Create your first quote in 60 seconds"

### 2. Quote Builder (Core Screen)

The most important screen. Where quotes are built line by line.

```
+----------------------------------+
|  <- Back        New Quote  [Save]|
+----------------------------------+
|  CLIENT                          |
|  [Select or add client    v]     |
+----------------------------------+
|  DETAILS                         |
|  Title: [Kitchen Renovation    ] |
|  Valid until: [Feb 25, 2026   v] |
+----------------------------------+
|  LINE ITEMS                      |
|  +------------------------------+|
|  | Sink Installation        $350||
|  | Qty: 1                   [x] ||
|  +------------------------------+|
|  +------------------------------+|
|  | Copper Pipe (per ft)     $12 ||
|  | Qty: 25          Total: $300 ||
|  +------------------------------+|
|  [+ Add Line Item]               |
|  [+ Add from Catalog]            |
+----------------------------------+
|  PHOTOS                          |
|  [+Photo]  [img] [img] [img]    |  <- Thumbnail strip, tap to view
+----------------------------------+
|  NOTES                           |
|  [Add notes for the client...  ] |
+----------------------------------+
|  +------------------------------+|
|  |  Subtotal          $650.00   ||
|  |  Tax (8.5%)         $55.25   ||
|  |  ========================    ||
|  |  TOTAL             $705.25   ||
|  +------------------------------+|
+----------------------------------+
|  [Preview PDF]    [Send to Client]|
+----------------------------------+
```

**Key elements:**
- Client picker: combo box with search, or "Add New" inline
- Line item rows: description, unit price, quantity, line total. Swipe to delete.
- "Add from Catalog" opens a bottom sheet with trade-specific items
- Photo gallery: horizontal thumbnail strip. Tap [+Photo] to capture or pick from gallery. Tap thumbnail to view full-size. Long press to delete. Max 5 photos.
- Running total always visible (sticky at bottom or in-line)
- Tax rate configurable in Settings, applied automatically
- Notes field: free text for scope, exclusions, terms
- "Preview PDF" and "Send" as bottom action bar
- Auto-save on every change (local SQLite)

### 3. Line Item Editor (Bottom Sheet)

Opens when adding or editing a line item.

```
+----------------------------------+
|  Add Line Item              [x]  |
+----------------------------------+
|  Description                     |
|  [Sink installation            ] |
+----------------------------------+
|  Unit Price                      |
|  [$] [350.00                   ] |
+----------------------------------+
|  Quantity                        |
|  [-]  [1]  [+]                   |
+----------------------------------+
|  Line Total:  $350.00            |
+----------------------------------+
|  [Add to Quote]                  |
+----------------------------------+
```

### 4. Trade Catalog Browser (Bottom Sheet)

Pre-built pricing for common services by trade.

```
+----------------------------------+
|  Plumbing Catalog           [x]  |
+----------------------------------+
|  [Search items...              ] |
+----------------------------------+
|  INSTALLATIONS                   |
|  +------------------------------+|
|  | Sink Installation      $350  || [+]
|  | Toilet Installation    $275  || [+]
|  | Water Heater Install   $800  || [+]
|  +------------------------------+|
|  REPAIRS                         |
|  +------------------------------+|
|  | Faucet Repair          $150  || [+]
|  | Pipe Leak Fix          $200  || [+]
|  +------------------------------+|
+----------------------------------+
```

**Key elements:**
- Grouped by category within trade
- Tap [+] adds item directly to current quote
- Prices are defaults; editable after adding
- User can customize catalog prices in Settings
- Search/filter within catalog

### 5. Quote Preview / PDF View

Shows exactly what the client will receive.

```
+----------------------------------+
|  <- Back    Preview     [Share]  |
+----------------------------------+
|  +------------------------------+|
|  |    [YOUR LOGO]               ||
|  |    Smith Plumbing LLC        ||
|  |    (555) 123-4567            ||
|  |    smith@plumbing.com        ||
|  |                              ||
|  |  QUOTE #QC-2026-0042        ||
|  |  Date: Feb 11, 2026         ||
|  |  Valid Until: Feb 25, 2026   ||
|  |                              ||
|  |  Prepared for:               ||
|  |  Bob Johnson                 ||
|  |  123 Main St, Anytown       ||
|  |                              ||
|  |  Kitchen Renovation          ||
|  |  ----------------------------||
|  |  Sink Install    1   $350.00 ||
|  |  Copper Pipe    25   $300.00 ||
|  |  ----------------------------||
|  |  Subtotal           $650.00  ||
|  |  Tax (8.5%)          $55.25  ||
|  |  TOTAL              $705.25  ||
|  |                              ||
|  |  Notes:                      ||
|  |  Includes all materials...   ||
|  |                              ||
|  |  [ACCEPT]    [DECLINE]       ||
|  +------------------------------+|
+----------------------------------+
|  [Download PDF] [Email to Client]|
+----------------------------------+
```

**Key elements:**
- Clean, professional layout
- Contractor's branding (logo, colors, business info)
- Line items table with clear formatting
- Accept/Decline buttons (for the client-facing web version)
- Share via email, SMS, or copy link
- Download as PDF

### 6. Client List

Simple contact management.

```
+----------------------------------+
|  Clients              [+ Add]    |
+----------------------------------+
|  [Search clients...            ] |
+----------------------------------+
|  +------------------------------+|
|  | Bob Johnson                  ||
|  | 3 quotes - $12,450 total    ||
|  +------------------------------+|
|  +------------------------------+|
|  | Maria Garcia                 ||
|  | 1 quote - $7,500 total      ||
|  +------------------------------+|
+----------------------------------+
```

### 7. Settings

Business configuration and preferences.

```
+----------------------------------+
|  Settings                        |
+----------------------------------+
|  BUSINESS INFO                   |
|  Business Name: [Smith Plumbing] |
|  Phone: [(555) 123-4567       ]  |
|  Email: [smith@plumbing.com   ]  |
|  Address: [123 Trade St...    ]  |
|  Logo: [Upload]  [Preview]       |
+----------------------------------+
|  QUOTE SETTINGS                  |
|  Default tax rate: [8.5] %       |
|  Quote valid for: [14] days      |
|  Currency: [USD v] / [CAD v]     |
|  Quote number prefix: [QC-]     |
+----------------------------------+
|  CATALOGS                        |
|  [Plumbing]  [Electrical]  [+]   |
|  Manage your pricing templates   |
+----------------------------------+
|  SUBSCRIPTION                    |
|  Plan: Free (5 quotes/month)     |
|  [Upgrade to Pro - $15/mo]       |
+----------------------------------+
```

---

## PDF Design

The PDF is the contractor's calling card. It must look better than anything they could make in Word.

### Layout
- Clean header with logo (left) and business info (right)
- Quote metadata (number, date, validity) in a subtle header bar
- Client info section
- Line items in a clean table with alternating row shading
- Totals section right-aligned, bold total
- Notes section at bottom
- Footer: "Created with QuoteCraft" (free tier) or custom footer (Pro)

### Branding (Pro tier)
- Custom logo placement
- Primary color applied to header bar and accents
- Business name in header
- Custom footer text (e.g., "Licensed & Insured - License #12345")

---

## Iconography

Use Fluent System Icons (built into Uno Platform) or Material Symbols via `FontIcon`.

| Action | Icon |
|--------|------|
| Create quote | `Add` / Plus |
| Edit | `Edit` / Pencil |
| Delete | `Delete` / Trash |
| Send | `Send` / Paper plane |
| PDF | `Document` / File |
| Client | `Person` |
| Settings | `Settings` / Gear |
| Catalog | `Library` / Book |
| Search | `Search` / Magnifier |
| Back | `ChevronLeft` / Arrow |

---

## Empty States

Every list needs an empty state that guides the user forward.

| Screen | Empty State |
|--------|-------------|
| Quotes | Illustration + "No quotes yet. Create your first one in 60 seconds." + [Create Quote] button |
| Clients | "Add your first client to start quoting." + [Add Client] button |
| Catalog | "Add items to your pricing catalog for faster quoting." + [Add Item] button |

---

## Interaction Patterns

| Pattern | Implementation |
|---------|---------------|
| Create quote | FAB on dashboard, or "+" in app bar |
| Add line item | "Add Line Item" button at bottom of items list |
| Add from catalog | Opens bottom sheet (`DrawerControl`) with catalog browser |
| Delete line item | Swipe left to reveal delete action |
| Change status | Tap status badge to open status picker |
| Send quote | Bottom action bar "Send to Client" opens share sheet |
| Offline indicator | Subtle banner at top: "You're offline. Quotes will sync when connected." |

---

## Animation & Motion

Keep it minimal and functional:
- Page transitions: slide left/right for navigation
- Bottom sheet: slide up from bottom
- Card press: subtle scale down on press (Material ripple)
- Status change: badge color fade transition
- FAB: standard Material FAB animation
- Loading: skeleton screens, not spinners (except sync)

---

## Accessibility

| Requirement | Implementation |
|-------------|---------------|
| Touch targets | Minimum 48x48dp, prefer 56dp for primary actions |
| Contrast | 4.5:1 for body text, 3:1 for large text. Slate blue on white passes. |
| Screen reader | `AutomationProperties.Name` on all buttons, inputs, and list items |
| Focus order | Logical tab order via `TabIndex` |
| Labels | All inputs have visible labels (not just placeholders) |
| x:Uid | All visible text uses `x:Uid` for localization readiness |
