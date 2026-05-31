# QuoteCraft — Architecture & Design Brief

**Version 2.0 — February 2026**
**Consolidated specification: architecture, data model, and pixel-level page specs**
**Supersedes:** QuoteCraft-Design-Brief.md (mobile) + QuoteCraft-Desktop-Design-Brief.md

---

## Table of Contents

### Part 1 — App Overview
1. [Purpose & Target User](#11-purpose--target-user)
2. [Core Flows](#12-core-flows)
3. [Navigation Model & Routing Map](#13-navigation-model--routing-map)
4. [State & Data Model](#14-state--data-model)
5. [Key Components & Services](#15-key-components--services)

### Part 2 — Page-by-Page Specifications
6. [Quotes Page](#2-quotes-page)
7. [Quote Builder / Detail](#3-quote-builder--detail)
8. [Clients Page](#4-clients-page)
9. [Catalog Page](#5-catalog-page)
10. [Settings Page](#6-settings-page)

### Appendices
- [A — Design System Tokens](#appendix-a--design-system-tokens)
- [B — PDF Generation Specification](#appendix-b--pdf-generation-specification)
- [C — Monetization & Tier Boundaries](#appendix-c--monetization--tier-boundaries)
- [D — Iconography Reference](#appendix-d--iconography-reference)

---

# Part 1 — App Overview

---

## 1.1 Purpose & Target User

### Product Purpose

QuoteCraft is a mobile-first application that enables independent contractors to create, send, and manage professional client-facing quotes from any device. It replaces handwritten estimates and spreadsheet workflows with a streamlined tool purpose-built for the trades.

### Target User

| Factor | Reality |
|--------|---------|
| Who | Independent contractors: plumbers, electricians, HVAC techs, carpenters, general contractors |
| Primary device | Phone (70%), tablet (20%), desktop (10%) |
| Environment | Job sites (bright sun, dust, noise), truck cab, kitchen table |
| Physical context | Dirty or calloused hands, may be wearing gloves. Large touch targets mandatory on mobile. |
| Tech comfort | Moderate. Comfortable with texting, social media, possibly QuickBooks. Not power users. |
| Time budget | 2–3 minutes maximum to create a quote. Exceeding this loses them to paper. |
| Primary goal | Look professional to the client. Win the job. Get paid. |

### Design Principles

1. **Speed over polish.** Every interaction is optimized for the fewest possible taps. If it takes longer than paper, they won't use it.
2. **Clarity over cleverness.** Every screen answers "What do I do next?" No ambiguous icons, no hidden gestures, no jargon.
3. **Professional output from simple input.** The generated PDF looks better than anything the contractor could produce in Word or Excel.
4. **Information density over navigation depth (desktop).** On desktop, show a list and its detail simultaneously. Never force a full-screen push when a panel update will do.
5. **Offline-first.** All features work without connectivity. Data syncs when reconnected.

---

## 1.2 Core Flows

### Flow 1 — Create & Send a Quote (critical path)

This is the reason the app exists. Every design decision is evaluated against its impact on this flow.

```
1. Open app → lands on Quotes list
2. Tap "+ New Quote" (FAB on mobile, top bar button on desktop)
3. Select or create a client
4. Enter quote title (e.g., "Kitchen Renovation")
5. Add line items:
   a. Tap "+ Add Line Item" → fill description, price, quantity → "Add to Quote"
   b. OR tap "Add from Catalog" → browse/search → tap "+" on items
6. (Optional) Edit quantities or prices inline
7. (Optional) Add notes for the client
8. Review running totals (auto-calculated: subtotal, tax, total)
9. Tap "Preview" → see exact PDF output
10. Tap "Send" → email to client
11. Quote status changes to "Sent"
12. Receive notification when client views the quote
13. Client accepts/declines → status updates
```

**Target time: under 2 minutes from step 2 to step 10.**

### Flow 2 — Manage Existing Quotes

```
1. Open app → Quotes list
2. Filter by status (chip bar: Draft, Sent, Viewed, Accepted, Declined)
3. Search by quote title or client name
4. Tap a quote → view/edit details
5. Edit line items, client, or notes
6. Re-send or download PDF
```

### Flow 3 — Manage Clients

```
1. Navigate to Clients tab
2. Search for or browse client list
3. Tap client → view quote history and contact details
4. Add new client (from Clients tab or inline during quote creation)
5. Edit client details
```

### Flow 4 — Manage Catalog

```
1. Navigate to Catalog tab
2. Browse by category or search
3. Add new items with description, price, category
4. Edit existing item prices or descriptions
5. Delete items no longer offered
```

### Flow 5 — Configure Business Settings

```
1. Navigate to Settings tab
2. Enter business info (name, phone, email, address, logo)
3. Configure quote defaults (tax rate, validity period, currency, prefix)
4. Manage catalogs (add/rename/delete)
5. View subscription status and upgrade
```

### Flow 6 — Offline Workflow

```
1. Lose connectivity on job site
2. Subtle offline banner appears ("You're offline. Quotes will sync when connected.")
3. Create/edit quotes, clients, catalog items normally against local storage
4. Regain connectivity
5. Auto-sync queued changes in background
6. Conflict resolution: last-write-wins with optional review notification
```

---

## 1.3 Navigation Model & Routing Map

### Navigation Architecture

The app has a flat, tab-based navigation model with 4 top-level destinations. Sub-screens push onto a per-tab navigation stack.

```
[Shell]
 ├── Quotes (default / home)
 │    ├── Quote List (root)
 │    ├── Quote Builder / Detail (push)
 │    └── Quote Preview (push on mobile, panel swap on desktop)
 │
 ├── Clients
 │    ├── Client List (root)
 │    ├── Client Detail (push on mobile, panel on desktop)
 │    └── Client Editor (push or modal)
 │
 ├── Catalog
 │    ├── Catalog Grid / List (root)
 │    ├── Catalog Item Editor (modal)
 │    └── Add Catalog (modal)
 │
 └── Settings
      └── Settings (root, single screen, scrollable)
```

### Navigation Component by Breakpoint

| Viewport | Component | Behavior |
|----------|-----------|----------|
| Narrow (0–599px) | Bottom Tab Bar | Fixed bottom, 4 items (icon + label), switches root screen |
| Medium (600–904px) | Bottom Tab Bar | Same as narrow |
| Wide (905–1280px) | Navigation Rail (80px, left) | Fixed left, 4 items, switches main area content |
| Extra Wide (1281px+) | Navigation Rail (80px, left) | Same as wide |

### Routing Table

| Route | Mobile Behavior | Desktop Behavior | URL (if web) |
|-------|----------------|------------------|-------------|
| `/quotes` | Show quote list (full screen) | Show master-detail: list + empty state | `/quotes` |
| `/quotes/:id` | Push quote builder (full screen) | Select in list, show detail in right panel | `/quotes/:id` |
| `/quotes/:id/preview` | Push preview (full screen) | Swap detail panel to preview sub-view | `/quotes/:id/preview` |
| `/quotes/new` | Push blank quote builder | Select nothing in list, show builder in detail | `/quotes/new` |
| `/clients` | Show client list (full screen) | Show master-detail: list + empty state | `/clients` |
| `/clients/:id` | Push client detail (full screen) | Select in list, show detail in right panel | `/clients/:id` |
| `/catalog` | Show catalog list (full screen) | Show full-width card grid | `/catalog` |
| `/settings` | Show settings (full screen) | Show single-column scrollable settings | `/settings` |

### Deep Linking

The app supports deep links to individual quotes and clients. Incoming links resolve the entity first, navigate to the appropriate tab, and select/push the target item.

### Back Navigation

| Trigger | Mobile | Desktop |
|---------|--------|---------|
| System back (gesture / button) | Pops current sub-screen, returns to list. Slide-right transition. | Deselects current item in detail panel. No animation. |
| In-app back arrow | Same as system back | Same |
| Tab switch | Resets that tab's navigation stack to root | Resets main area to root state (deselects, closes preview) |

---

## 1.4 State & Data Model

### 1.4.1 Data Entities

#### Quote

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| `id` | UUID | Auto-generated, immutable | Primary key |
| `number` | String | Auto-generated, immutable | Format: `{prefix}{year}-{sequence}` e.g. `QC-2026-0042` |
| `title` | String | Required, 1–200 chars | User-provided job description |
| `status` | Enum | One of: `draft`, `sent`, `viewed`, `accepted`, `declined` | Defaults to `draft` on creation |
| `clientId` | UUID | Required (nullable during creation) | Foreign key → Client |
| `items` | Collection\<LineItem\> | 0..n, ordered | Child entities, cascade delete |
| `notes` | String | Optional, 0–5000 chars | Free-text notes for the client |
| `validUntil` | Date | Required | Defaults to `createdAt + settings.quoteValidityDays` |
| `taxRate` | Decimal | 0–100, 2 decimal places | Snapshot of business tax rate at creation time |
| `createdAt` | Timestamp | Auto-set, immutable | |
| `updatedAt` | Timestamp | Auto-updated on every change | |

**Calculated fields (not persisted, derived in view model):**

| Field | Formula |
|-------|---------|
| `subtotal` | `SUM(items[].lineTotal)` |
| `tax` | `subtotal × (taxRate / 100)` |
| `total` | `subtotal + tax` |
| `itemCount` | `items.length` |
| `isOverdue` | `status == 'sent' && validUntil < today` |

#### Line Item

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| `id` | UUID | Auto-generated | Primary key |
| `quoteId` | UUID | Required | Foreign key → Quote |
| `description` | String | Required, 1–500 chars | Item name / service description |
| `unitPrice` | Decimal | Required, ≥ 0.00, 2 decimal places | Price per unit |
| `quantity` | Integer | Required, ≥ 1, ≤ 9999 | Number of units |
| `sortOrder` | Integer | 0-based | Position within the quote |

**Calculated:**

| Field | Formula |
|-------|---------|
| `lineTotal` | `unitPrice × quantity` |

#### Client

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| `id` | UUID | Auto-generated | Primary key |
| `name` | String | Required, 1–200 chars | Full name or business name |
| `email` | String | Optional, valid email format | |
| `phone` | String | Optional, 5–20 chars | |
| `address` | String | Optional, 0–500 chars | Street address, city, state |
| `createdAt` | Timestamp | Auto-set | |
| `updatedAt` | Timestamp | Auto-updated | |

**Calculated / aggregated:**

| Field | Formula |
|-------|---------|
| `quoteCount` | `COUNT(quotes WHERE clientId == this.id)` |
| `totalValue` | `SUM(quotes[].total WHERE clientId == this.id AND status IN [sent, viewed, accepted])` |
| `lastQuoteDate` | `MAX(quotes[].createdAt WHERE clientId == this.id)` |
| `initials` | First character of each word in `name`, uppercase, max 2 chars |

#### Catalog Item

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| `id` | UUID | Auto-generated | Primary key |
| `catalogId` | UUID | Required | Foreign key → Catalog |
| `description` | String | Required, 1–500 chars | Item / service name |
| `unitPrice` | Decimal | Required, ≥ 0.00 | Default price (editable after adding to a quote) |
| `category` | String | Required, 1–100 chars | Grouping category (e.g. "Installations", "Repairs") |
| `sortOrder` | Integer | 0-based | Position within category |

#### Catalog

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| `id` | UUID | Auto-generated | Primary key |
| `name` | String | Required, 1–100 chars | Trade name (e.g. "Plumbing", "Electrical") |
| `itemCount` | Integer | Calculated | Count of child CatalogItems |

#### Business Settings (singleton)

| Field | Type | Constraints | Default |
|-------|------|-------------|---------|
| `businessName` | String | 0–200 chars | `""` (empty) |
| `phone` | String | 0–20 chars | `""` |
| `email` | String | Valid email or empty | `""` |
| `address` | String | 0–500 chars | `""` |
| `logoUri` | String/Binary | Optional | `null` |
| `defaultTaxRate` | Decimal | 0–100, 2 decimal places | `8.50` |
| `quoteValidityDays` | Integer | 1–365 | `14` |
| `currency` | String | ISO 4217 code | `"USD"` |
| `quotePrefix` | String | 1–10 chars, alphanumeric + hyphens | `"QC-"` |

#### Subscription (singleton)

| Field | Type | Notes |
|-------|------|-------|
| `tier` | Enum | `free` \| `pro` |
| `quotesUsedThisMonth` | Integer | Resets on billing cycle |
| `quotesAllowed` | Integer | 5 (free) or ∞ (pro) |
| `billingCycleStart` | Date | |

### 1.4.2 Entity Relationships

```
Catalog (1) ──── has many ──── (n) CatalogItem
Client  (1) ──── has many ──── (n) Quote
Quote   (1) ──── has many ──── (n) LineItem
BusinessSettings (singleton)
Subscription (singleton)
```

### 1.4.3 Persistence Strategy

| Concern | Strategy |
|---------|----------|
| Storage engine | Platform-recommended local database: SQLite (via Dapper or EF Core on .NET), Realm, Core Data, or IndexedDB for web. |
| Offline capability | All data persisted locally first. App is fully functional offline. |
| Auto-save | Every field change in the Quote Builder saves to local storage immediately. No explicit "save" action required. The Save button provides manual reassurance and triggers a sync if online. |
| Sync | Stretch goal for v1, required for v2. Queued changes sync in background when connectivity returns. Last-write-wins conflict resolution with optional user review. |
| Quote number sequence | Maintained locally with gap tolerance. Sequence increments on quote creation, not on save. |
| Data export | Quotes exportable as PDF (via PDF generation service). No CSV/JSON export in v1. |

### 1.4.4 App State (View-Level)

These are transient UI states, not persisted across sessions:

| State | Type | Scope | Notes |
|-------|------|-------|-------|
| `activeTab` | Enum | Global | `quotes` \| `clients` \| `catalog` \| `settings` |
| `selectedQuoteId` | UUID? | Quotes tab | `null` = empty state / new quote mode |
| `selectedClientId` | UUID? | Clients tab | `null` = empty state |
| `quoteFilterStatus` | Enum? | Quotes tab | `null` = "All". Otherwise one of the 5 statuses. |
| `quoteSearchQuery` | String | Quotes tab | Filters list by title/client name match |
| `clientSearchQuery` | String | Clients tab | Filters list by name/email/phone match |
| `catalogSearchQuery` | String | Catalog tab | Filters grid by description match |
| `catalogFilterCategory` | String? | Catalog tab | `null` = "All" |
| `isEditMode` | Boolean | Quote detail | `false` = view mode, `true` = edit mode (reveals delete buttons, add-item buttons, editable fields) |
| `isPreviewMode` | Boolean | Quote detail | `false` = detail view, `true` = preview/PDF sub-view |
| `activeModal` | Enum? | Global | `null` \| `lineItemEditor` \| `catalogBrowser` \| `clientEditor` \| `catalogItemEditor` \| `addCatalog` \| `deleteConfirm` |
| `editingLineItemId` | UUID? | Quote detail | `null` = creating new. UUID = editing existing. |
| `isOnline` | Boolean | Global | Connectivity status |
| `toastMessage` | String? | Global | Non-null triggers toast display, auto-clears after 2s |
| `pricingToggle` | Boolean | Pricing (landing only) | `false` = monthly, `true` = annual |

---

## 1.5 Key Components & Services

### 1.5.1 Service Layer

| Service | Responsibility | Dependencies |
|---------|---------------|--------------|
| **QuoteService** | CRUD operations on quotes and line items. Generates quote numbers. Calculates totals. Enforces tier limits. | DataStore, SubscriptionService |
| **ClientService** | CRUD operations on clients. Aggregates quote stats per client. Deduplication warnings. | DataStore |
| **CatalogService** | CRUD operations on catalogs and catalog items. Category management. | DataStore |
| **SettingsService** | Read/write business settings. Provides defaults to QuoteService. | DataStore |
| **SubscriptionService** | Tracks usage against tier limits. Surfaces upgrade prompts. Validates feature access. | DataStore |
| **PdfService** | Generates PDF from a Quote entity. Applies business branding. Handles Pro/Free tier differences. | SettingsService, SubscriptionService |
| **EmailService** | Sends quote PDF to client email. Tracks delivery status. | PdfService, ClientService |
| **SyncService** | Queues local changes. Syncs to remote backend when online. Handles conflict resolution. | DataStore, ConnectivityMonitor |
| **SearchService** | Full-text search across quotes (title, client name), clients (name, email, phone), and catalog items (description). | DataStore |
| **AnalyticsService** | Tracks user actions and app events. See per-page analytics specs. | — |

### 1.5.2 View Model Layer

Each page has a view model that mediates between the UI and the service layer:

| View Model | State Owned | Commands Exposed |
|------------|-------------|-----------------|
| **QuoteListViewModel** | `quotes`, `filterStatus`, `searchQuery`, `selectedQuoteId`, `statusCounts` | `selectQuote(id)`, `createQuote()`, `setFilter(status)`, `setSearch(query)`, `deleteQuote(id)` |
| **QuoteDetailViewModel** | `quote`, `isEditMode`, `isPreviewMode`, `editingLineItemId` | `toggleEdit()`, `togglePreview()`, `addLineItem(item)`, `removeLineItem(id)`, `updateLineItem(id, changes)`, `updateQuoteField(field, value)`, `sendQuote()`, `downloadPdf()` |
| **ClientListViewModel** | `clients`, `searchQuery`, `selectedClientId` | `selectClient(id)`, `createClient()`, `setSearch(query)` |
| **ClientDetailViewModel** | `client`, `quoteHistory` | `editClient()`, `createQuoteForClient()` |
| **CatalogViewModel** | `catalogs`, `activeCatalog`, `items`, `filterCategory`, `searchQuery`, `categories` | `setCategory(cat)`, `setSearch(query)`, `addItem(item)`, `editItem(id, changes)`, `deleteItem(id)`, `addCatalog(name)` |
| **SettingsViewModel** | `settings`, `subscription`, `catalogs` | `updateSetting(key, value)`, `uploadLogo(file)`, `addCatalog(name)`, `deleteCatalog(id)` |

### 1.5.3 Shared UI Components

These components are used across multiple pages and must be built once, themed globally:

| Component | Used By | Props |
|-----------|---------|-------|
| **StatusBadge** | Quotes list, quote detail, client quote history | `status: QuoteStatus` |
| **SearchInput** | Quotes list, Clients list, Catalog grid, Catalog browser modal | `value`, `onChange`, `placeholder` |
| **ChipBar** | Quotes list (status filter), Catalog grid (category filter), Catalog browser modal | `items: {label, value, count?}[]`, `activeValue`, `onSelect` |
| **SectionHeader** | Quote builder form sections | `title`, `count?`, `action?` |
| **EmptyState** | All list/detail empty states | `icon`, `title`, `description`, `actionLabel?`, `onAction?` |
| **Toast** | Global | `message`, `visible` |
| **OfflineBanner** | Global (below app bar) | `isOnline` |
| **QuantityStepper** | Line item editor | `value`, `onChange`, `min`, `max` |
| **CurrencyInput** | Line item editor, catalog item editor | `value`, `onChange`, `prefix` |
| **ConfirmDialog** | Delete confirmations | `title`, `message`, `confirmLabel`, `onConfirm`, `onCancel` |
| **Avatar** | Client cards, client detail | `name: string` (derives initials + color) |
| **Card** | Everywhere | Container with standard border/radius/shadow |
| **UpgradeBanner** | Settings, limit-reached prompt | `tier`, `onUpgrade` |

---

# Part 2 — Page-by-Page Specifications

The following sections document every user-facing element and behavior on each page, covering both mobile and desktop breakpoints. Where behavior differs by breakpoint, both are specified.

**Conventions used in this section:**

- **Mobile** refers to Narrow (0–599px) and Medium (600–904px) breakpoints.
- **Desktop** refers to Wide (905–1280px) and Extra Wide (1281px+) breakpoints.
- Token names (e.g., `Primary`, `Surface Variant`, `radius-md`) reference Appendix A.
- All measurements are in `dp` (density-independent pixels) unless otherwise noted.
- Monospace font is used for all currency values for column alignment.

---

## 2. Quotes Page

### 2.0 Goal & Primary User Tasks

The Quotes page is the home screen. It answers: **"What quotes need my attention?"**

| Task | Priority | Frequency |
|------|----------|-----------|
| Scan all quotes to find one needing attention | P0 | Every session |
| Filter quotes by status | P0 | Most sessions |
| Search for a specific quote by title or client | P0 | Frequent |
| Create a new quote | P0 | Most sessions |
| Open an existing quote to view or edit | P0 | Every session |
| See at-a-glance status distribution | P1 | Periodic |
| Delete a quote | P2 | Rare |

### 2.1 Information Architecture

| Content | Why It Lives Here | Priority |
|---------|------------------|----------|
| Quote list (title, client, amount, status, date) | Primary content — the full inventory of quotes | Dominant |
| Status filter chips with counts | Enables rapid triage without scrolling | High |
| Search input | Instant lookup when the list is long | High |
| Total quote count | Context for the dataset size | Low |
| FAB / New Quote button | Primary creation action, always accessible | High |

### 2.2 Layout Specification

#### Mobile (0–904px)

```
┌─────────────────────────────────────────────┐
│  App Bar: "QuoteCraft"         [+ New]       │  56dp, Primary bg, amber accent bar
├─────────────────────────────────────────────┤
│  Chip Bar: [All] [Draft] [Sent] [Viewed]... │  Horizontal scroll, Surface bg
├─────────────────────────────────────────────┤
│  ┌─────────────────────────────────────┐    │
│  │  Quote Card                         │    │  12px horizontal padding
│  │  Title                 [Status]     │    │  10px vertical gap between cards
│  │  Client Name                        │    │
│  │  ─────────────────────────────      │    │
│  │  $4,611               2 days ago    │    │
│  └─────────────────────────────────────┘    │
│  ┌─────────────────────────────────────┐    │
│  │  Quote Card                         │    │
│  │  ...                                │    │
│  └─────────────────────────────────────┘    │
│                                    [FAB +]   │  56×56dp, bottom-right, above tab bar
├─────────────────────────────────────────────┤
│  [Quotes] [Clients] [Catalog] [Settings]    │  Bottom Tab Bar
└─────────────────────────────────────────────┘
```

- **App Bar:** 56dp height. Primary background. Full-width. Title "QuoteCraft" (white, Title Large / 22px). Right: "+ New" pill button (semi-transparent white bg 15%, white text, 14px bold). Bottom: 3px Secondary (amber) accent bar.
- **Chip Bar:** Below app bar. Surface background. 1px Outline Variant bottom border. 12px horizontal padding, 10px vertical padding. Horizontal scroll with 8px gap between chips.
- **Quote List:** `overflow-y: auto`. Padding: 12px horizontal, 8px top (from chip bar), 80px bottom (clearance for FAB + tab bar). Pull-to-refresh enabled.
- **FAB:** Fixed position, bottom 80px (above tab bar), right 16px. 56×56dp. Secondary (amber) background. White "+" icon (24px). Border radius Large (16dp). Shadow Level 3.
- **Tab Bar:** Fixed bottom. 56dp height. Surface background. 1px Outline Variant top border. 4 evenly spaced items.

#### Desktop (905px+)

```
┌──────┬──────────────────────────────────────────────────────┐
│      │  Top Bar: "Quotes" · 7 total         [+ New Quote]  │  64px
│ Nav  ├───────────────┬──────────────────────────────────────┤
│ Rail │  Search [🔍]  │  (detail panel — see §3)             │
│(80px)│  Chip filters │                                      │
│      │  ──────────── │                                      │
│      │  Quote Card ▸ │                                      │
│      │  Quote Card   │                                      │
│      │  Quote Card   │                                      │
│      │  ...          │                                      │
└──────┴───────────────┴──────────────────────────────────────┘
```

- **Nav Rail:** 80px wide. Full height. Surface background. 1px Outline Variant right border. Brand header (64px, Primary bg, "QC" white 22px bold, 3px amber bar). 4 navigation items below.
- **Top Bar:** 64px height. Surface background. 1px Outline Variant bottom border. 32px horizontal padding. Left: "Quotes" (20px bold), "7 total" subtitle (13px On Surface Variant, 4px left margin). Right: Accent button "+ New Quote".
- **List Panel:** 380px wide (320px at Wide breakpoint). Surface background. 1px Outline Variant right border. Divided into fixed header (search + chips) and scrollable body (cards).
- **Detail Panel:** flex: 1, min-width: 0. See §3 (Quote Builder / Detail) for detail panel specs.

### 2.3 Typography

| Element | Font | Size | Weight | Line-height | Color | Casing |
|---------|------|------|--------|-------------|-------|--------|
| App bar title (mobile) | Body | 22px | 700 | 1.2 | White | Sentence |
| Top bar title (desktop) | Body | 20px | 700 | 1.2 | On Surface | Sentence |
| Top bar subtitle (desktop) | Body | 13px | 400 | 1 | On Surface Variant | Sentence |
| Chip label | Body | 12px (mobile) / 12px (desktop) | 500 (inactive) / 700 (active) | 1 | See chip spec | Sentence |
| Chip count badge | Body | 10px | 700 | 1 | Same as chip text | — |
| Quote card title | Body | 15px (mobile) / 14px (desktop) | 600 | 1.3 | On Surface | Sentence |
| Quote card client name | Body | 13px (mobile) / 12px (desktop) | 400 | 1.2 | On Surface Variant | Sentence |
| Quote card amount | Mono | 18px (mobile) / 16px (desktop) | 700 | 1 | Primary | — |
| Quote card date | Body | 12px (mobile) / 11px (desktop) | 400 | 1 | On Surface Variant | Sentence |
| Status badge text | Body | 11px (mobile) / 10px (desktop) | 700 | 1 | Per-status color | UPPERCASE |
| Search placeholder | Body | 14px | 400 | 1 | On Surface Variant | Sentence |
| Empty state title | Body | 16px | 600 | 1.3 | On Surface | Sentence |
| Empty state description | Body | 14px | 400 | 1.5 | On Surface Variant | Sentence |

### 2.4 Component Inventory

#### 2.4.1 App Bar / Top Bar

**Mobile App Bar:**
- Height: 56dp. Background: Primary. Position: fixed top.
- Left: "QuoteCraft" title (white, 22px, bold).
- Right: "+ New" pill button — `border-radius: pill`, background: `rgba(255,255,255,0.15)`, color: white, padding: 8dp vertical × 16dp horizontal, font-size: 14px, font-weight: 600.
- Bottom edge: 3px Secondary accent bar, full width.

**Desktop Top Bar:**
- Height: 64px. Background: Surface. Position: fixed below nav rail brand header.
- 1px Outline Variant bottom border. 32px horizontal padding (24px at Wide breakpoint).
- Left: "Quotes" (20px bold) + "{count} total" (13px On Surface Variant, 4px left margin).
- Right: Accent button "+ New Quote" — Secondary background, On Surface text (`#1F2937`), 10px vertical + 20px horizontal padding, radius-sm (8dp), 14px font, weight 600, inline-flex with "+" icon (8px gap).

#### 2.4.2 Search Input

- Mobile: Hidden. Uses chip bar for filtering. (Future: add search below chip bar.)
- Desktop: Full-width within list panel header.
- Height: ~40px (10px vertical padding + 14px font + border).
- Left padding: 38px (accommodates search icon).
- Search icon: 16px, On Surface Variant, absolute-positioned left 12px, vertically centered.
- Border: 1.5px Outline, radius-sm (8dp).
- Background: Surface Variant (unfocused) → Surface (focused).
- Focus: border color → Primary.
- Placeholder: "Search quotes..." (14px, On Surface Variant).
- Behavior: Filters list on every keystroke (debounced 200ms). Matches against `quote.title` and `client.name`.
- Clear: "✕" button appears on right when input has value. 16px, On Surface Variant. Clears input and resets filter.

#### 2.4.3 Chip Filter Bar

**Chip items:**

| Label | Filter value | Shows count badge |
|-------|-------------|-------------------|
| All | `null` (show all) | No |
| Draft | `draft` | Yes |
| Sent | `sent` | Yes |
| Viewed | `viewed` | Yes |
| Accepted | `accepted` | Yes |
| Declined | `declined` | Yes |

**Chip spec:**
- Height: 36dp (mobile) / 32dp (desktop).
- Padding: 8dp vertical × 14dp horizontal.
- Border radius: pill (9999px).
- Font: 12px, weight 500 (inactive) / 700 (active).
- Gap: 8px (mobile) / 6px (desktop) between chips.

| State | Background | Border | Text Color |
|-------|-----------|--------|------------|
| Inactive | Surface | 1px Outline | On Surface Variant |
| Active | Primary Container | 1.5px Primary | Primary |
| Hover (desktop, inactive) | Surface | 1px Primary | Primary |

**Count badge (non-"All" chips):**
- Positioned inline after label text, 4px left gap.
- 10px font, bold, same color as chip text.
- Format: just the number (e.g., "3"), no parentheses.

**Behavior:** Tapping a chip sets `quoteFilterStatus`. Only one chip active at a time. Tapping the active chip does nothing (it stays active). Switching chips re-filters the list immediately with no animation.

#### 2.4.4 Quote Card

**Mobile spec:**
- Background: Surface. Border: 1px Outline Variant. Radius: radius-md (12dp). Shadow: Level 1. Padding: 16dp all sides.
- **Top row:** flex row, space-between, align-start. Left: title (15px semibold, On Surface, max 1 line, ellipsis overflow). Right: StatusBadge.
- **Client name:** Below title, 2dp top margin. 13px, On Surface Variant. Max 1 line, ellipsis.
- **Divider:** 1px Outline Variant. 10dp top margin, 10dp top padding.
- **Bottom row:** flex row, space-between, align-center. Left: amount (18px bold monospace, Primary). Right: relative date (12px, On Surface Variant).
- **Tap target:** Entire card is tappable. Navigates to Quote Builder/Detail for that quote.

**Desktop spec (list panel card):**
- Padding: 14dp vertical × 16dp horizontal. Margin-bottom: 6px. Border: 1.5px Outline Variant. Radius: radius-md (12dp). Cursor: pointer.
- **Top row:** Title (14px semibold) left, StatusBadge right. Client (12px On Surface Variant) below title, 2px top margin.
- **Bottom row:** 8px top margin, 8px top padding, 1px Outline Variant top border. Amount (16px bold monospace Primary) left, date (11px On Surface Variant) right.

**States:**

| State | Mobile | Desktop |
|-------|--------|---------|
| Default | As spec'd | As spec'd |
| Hover | — | Border → Primary, background → `#FAFBFF` |
| Pressed | Scale 0.985, 100ms | — |
| Selected | — | Border → Primary, background → Primary Container, shadow-sm |
| Focused (keyboard) | Focus ring (2px Primary outline, 2px offset) | Same |

#### 2.4.5 Status Badge

Shared component. Used in quote cards, quote detail, and client quote history.

| Status | Background | Text Color | Icon | Label |
|--------|-----------|------------|------|-------|
| `draft` | `#E5E7EB` | `#374151` | ✎ (pencil) | DRAFT |
| `sent` | `#DBEAFE` | `#1E40AF` | ↗ (arrow) | SENT |
| `viewed` | `#FEF3C7` | `#92400E` | 👁 (eye) | VIEWED |
| `accepted` | `#D1FAE5` | `#065F46` | ✓ (check) | ACCEPTED |
| `declined` | `#FEE2E2` | `#991B1B` | ✕ (cross) | DECLINED |

Spec: Inline-flex. Padding: 3dp vertical × 8dp horizontal. Radius: pill. Font: 10–11px, weight 700, uppercase, letter-spacing 0.5px. Icon: 10px, 4px right margin.

Accessibility: Badge has `aria-label="{status}"`. Color is never the sole indicator — text and icon are always present.

#### 2.4.6 FAB (Mobile Only)

- Size: 56×56dp. Radius: Large (16dp). Background: Secondary (amber). Shadow: Level 3.
- Icon: "+" in white, 24px.
- Position: fixed, bottom: `tab-bar-height + 16dp`, right: 16dp.
- Z-index: 50 (above content, below modals).
- Tap: Creates new blank quote, navigates to Quote Builder.
- Hover (tablet with trackpad): Scale 1.05, shadow increases.
- Accessibility: `aria-label="Create new quote"`.

### 2.5 Interaction Specification

| Action | Trigger | Behavior |
|--------|---------|----------|
| Create new quote | Tap FAB (mobile) or "+ New Quote" button (desktop) | If free tier and at limit: show upgrade prompt (see §2.8.1). Otherwise: create new Quote entity with `status: draft`, `taxRate: settings.defaultTaxRate`, `validUntil: today + settings.quoteValidityDays`. Navigate to Quote Builder (mobile) or show builder in detail panel (desktop). |
| Filter by status | Tap a chip | Set `quoteFilterStatus`. Re-filter list. Selected chip shows active style. |
| Search | Type in search input (desktop) | Debounce 200ms. Filter list where `quote.title CONTAINS query OR client.name CONTAINS query` (case-insensitive). |
| Open quote | Tap a quote card | Mobile: push Quote Builder with `selectedQuoteId`. Slide-left transition, 250ms ease-out. Desktop: set `selectedQuoteId`, detail panel updates instantly. |
| Swipe to delete (mobile) | Swipe left on quote card | Reveals red delete zone. Release triggers delete confirmation dialog. Stretch goal for v1. |
| Pull to refresh (mobile) | Pull down on list | Triggers sync. Shows refresh indicator. Re-fetches from local storage (and remote if online). |
| Keyboard: ↑/↓ (desktop) | Arrow keys when list is focused | Navigate between quote cards. Updates selection and detail panel. |
| Keyboard: / (desktop) | Slash key when list panel visible | Focuses search input. |
| Keyboard: Enter (desktop) | When a card is focused | Opens/selects that quote. |
| Keyboard: Delete (desktop) | When a card is focused | Opens delete confirmation for that quote. |

### 2.6 States

#### 2.6.1 Loading State

- **First load:** Skeleton screen. Chip bar shows 4 gray pill placeholders (pulsing animation). 3–4 card skeletons below: Surface background, radius-md, 120dp height, pulsing gray blocks for title/badge/amount areas.
- **Subsequent loads:** Data is in local storage; list renders instantly from cache. Sync happens in background.
- Skeleton pulse: `background: linear-gradient(90deg, Surface Variant 25%, #E5E7EB 50%, Surface Variant 75%)`, `background-size: 200% 100%`, `animation: pulse 1.5s ease-in-out infinite`.

#### 2.6.2 Empty State

**No quotes exist (first use):**
- Centered vertically in the list area.
- Icon: 📋 Clipboard, 48px, 40% opacity.
- Title: "No quotes yet" (16px semibold, On Surface).
- Description: "Create your first quote in 60 seconds." (14px, On Surface Variant).
- Button: "Create Quote" — Primary background, white text, 48dp height, radius-md.

**Filtered list is empty:**
- Same layout as above.
- Icon: 📋 Clipboard, 48px, 40% opacity.
- Title: "No {status} quotes" (dynamic per filter).
- Description: "Quotes with {status} status will appear here."
- No action button.

**Desktop detail panel — no selection:**
- Centered in detail panel.
- Icon: 📋, 56px, 25% opacity.
- Title: "Select a quote" (18px semibold).
- Description: "Choose a quote from the list to view details, or create a new one." (14px, On Surface Variant, max-width 320px).

#### 2.6.3 Error State

- **Network error during sync:** Toast: "Sync failed. Changes saved locally." Toast auto-dismisses after 2s. Retry on next connectivity change.
- **Data load error:** Replace list with centered error message: icon ⚠️ (48px, Error color, 40% opacity), title "Something went wrong", description "We couldn't load your quotes. Pull down to try again.", button "Retry".

#### 2.6.4 Offline State

- Banner appears below app bar / top bar: Surface Variant background, 1px Outline Variant bottom border, 12dp vertical × 16dp horizontal padding.
- Text: "You're offline. Quotes will sync when connected." (14px, On Surface Variant).
- Icon: cloud-offline, 16px, On Surface Variant, 8px right margin.
- Does not block interaction. All features work offline.
- Dismisses automatically when connectivity returns.

#### 2.6.5 Quota Limit Reached (Free Tier)

When a free-tier user taps "+ New" and `quotesUsedThisMonth >= 5`:
- **Mobile:** Bottom sheet with upgrade prompt. Title: "Monthly limit reached". Description: "You've used 5 of 5 free quotes this month. Upgrade to Pro for unlimited quotes." Buttons: "Maybe Later" (ghost) + "Upgrade — $15/mo" (accent).
- **Desktop:** Modal dialog (480px). Same content as mobile bottom sheet.

### 2.7 Validation Rules

The Quotes list page has no form inputs (forms live in Quote Builder, §3). Validation on this page is limited to:

| Rule | Enforcement |
|------|------------|
| Search query max length | 200 characters, silently truncated |
| Delete confirmation | Required for all deletes. See §2.8.2. |
| Quota enforcement | Checked on "+ New" action. See §2.6.5. |

### 2.8 Modals & Menus

#### 2.8.1 Quota Limit Modal

- **Trigger:** Tap "+ New" when `quotesUsedThisMonth >= quotesAllowed` on free tier.
- **Type:** Bottom sheet (mobile) / Modal dialog (desktop).
- **Title:** "Monthly limit reached"
- **Body:** "You've used {used} of {allowed} free quotes this month. Upgrade to Pro for unlimited quotes."
- **Actions:** "Maybe Later" (Ghost button, dismisses) + "Upgrade — $15/mo" (Accent button, navigates to subscription management).
- **Dismissal:** Tap scrim (mobile: swipe down also), close button, Escape key (desktop), or "Maybe Later" button.

#### 2.8.2 Delete Quote Confirmation

- **Trigger:** Swipe-to-delete (mobile), Delete key (desktop), or long-press → context menu (mobile stretch goal).
- **Type:** Bottom sheet (mobile) / Modal dialog 400px (desktop).
- **Title:** "Delete quote?"
- **Body:** ""{quote.title}" will be permanently deleted. This action cannot be undone."
- **Actions:** "Cancel" (Ghost button) + "Delete" (Error-color background, white text).
- **Dismissal:** Scrim tap, close button, Escape, or Cancel.
- **On confirm:** Quote is soft-deleted (removed from UI, marked as deleted in storage for sync). Toast: "Quote deleted".

### 2.9 Data Bindings

| UI Element | Reads | Writes | View Model Property / Event |
|------------|-------|--------|---------------------------|
| Quote list | `quotes` (filtered + sorted) | — | `QuoteListVM.filteredQuotes` |
| Chip bar active state | `quoteFilterStatus` | `quoteFilterStatus` | `QuoteListVM.filterStatus` / `setFilter()` |
| Chip count badges | Aggregated status counts | — | `QuoteListVM.statusCounts` |
| Search input (desktop) | `quoteSearchQuery` | `quoteSearchQuery` | `QuoteListVM.searchQuery` / `setSearch()` |
| Quote card title | `quote.title` | — | — |
| Quote card client name | `client.name` (resolved from `quote.clientId`) | — | — |
| Quote card amount | `quote.total` (formatted as currency) | — | — |
| Quote card status badge | `quote.status` | — | — |
| Quote card date | `quote.updatedAt` (formatted as relative time) | — | — |
| FAB / New Quote button | `subscription.quotesUsedThisMonth`, `subscription.quotesAllowed` | Creates new Quote | `QuoteListVM.createQuote()` |
| Selected card highlight (desktop) | `selectedQuoteId` | `selectedQuoteId` | `QuoteListVM.selectQuote()` |
| Top bar count (desktop) | `quotes.length` (unfiltered total) | — | `QuoteListVM.totalCount` |
| Empty state | `filteredQuotes.length == 0` | — | — |
| Offline banner | `isOnline` | — | Global `ConnectivityMonitor.isOnline` |

### 2.10 Analytics Events

| Event Name | Trigger | Payload |
|------------|---------|---------|
| `quotes.viewed` | Page loaded / tab activated | `{ count, filterStatus }` |
| `quotes.filtered` | Chip tapped | `{ status, resultCount }` |
| `quotes.searched` | Search query submitted (debounced) | `{ queryLength, resultCount }` |
| `quotes.created` | "+ New" tapped (and quota passed) | `{ quoteId }` |
| `quotes.opened` | Quote card tapped | `{ quoteId, status }` |
| `quotes.deleted` | Delete confirmed | `{ quoteId, status }` |
| `quotes.quota_hit` | "+ New" blocked by limit | `{ used, allowed, tier }` |
| `quotes.refresh` | Pull-to-refresh triggered | `{ isOnline }` |

### 2.11 Accessibility

| Concern | Specification |
|---------|--------------|
| Page landmark | `<main>` wraps content. `<nav>` for tab bar / rail. |
| Quote list | `role="list"` with `role="listitem"` per card. |
| Card semantics | Each card: `role="listitem"`, `tabindex="0"`, `aria-label="{title}, {client}, {amount}, {status}"`. |
| Status badge | `aria-label="{status}"` (e.g., "Sent"). Color is not the sole indicator. |
| Chip bar | `role="tablist"` with `role="tab"` per chip. `aria-selected` on active chip. |
| FAB | `aria-label="Create new quote"`. |
| Search input | `aria-label="Search quotes"`. |
| Empty state | Read as a group: icon is decorative (`aria-hidden`), title and description are `aria-live="polite"`. |
| Focus order | Tab order: search (desktop) → chip bar → first quote card → FAB (mobile). ↑/↓ navigates between cards when list is focused. |
| Contrast | All text meets 4.5:1 against its background. Status badge text meets 4.5:1 against badge background. |
| Reduced motion | Skip card hover transitions, FAB scale, skeleton pulse. |

---

## 3. Quote Builder / Detail

### 3.0 Goal & Primary User Tasks

This is the most important screen in the application. It is where quotes are constructed, reviewed, and sent.

| Task | Priority | Frequency |
|------|----------|-----------|
| Add line items (manual or from catalog) | P0 | Every quote |
| Assign a client | P0 | Every quote |
| Set quote title | P0 | Every quote |
| Review running totals | P0 | Every quote |
| Preview PDF output | P0 | Most quotes |
| Send quote to client | P0 | Most quotes |
| Edit existing line items (price, quantity) | P1 | Frequent |
| Delete line items | P1 | Frequent |
| Add/edit notes | P1 | Some quotes |
| Change quote validity date | P2 | Rare |
| Download PDF | P2 | Occasional |

### 3.1 Information Architecture

| Content | Why It Lives Here | Priority |
|---------|------------------|----------|
| Client selection / info | Who this quote is for — needed before sending | High |
| Quote title + metadata | Job description, dates, quote number | High |
| Line items (description, qty, price, total) | Core value — the priced items | Dominant |
| Add item controls | Must be immediately accessible during building | High |
| Running totals (subtotal, tax, total) | Real-time feedback on quote value | High |
| Notes | Scope/terms/conditions for the client | Medium |
| Preview / Send actions | Conversion actions — the goal of the entire flow | High |

### 3.2 Layout Specification

#### Mobile (0–904px) — Full-Screen Builder

```
┌─────────────────────────────────────────────┐
│  App Bar: "← New Quote"            [Save]   │  56dp
├─────────────────────────────────────────────┤
│  ┌───────────────────────────────────────┐  │
│  │  CLIENT                               │  │  Form section card
│  │  [Select client ▾]                    │  │
│  └───────────────────────────────────────┘  │
│  ┌───────────────────────────────────────┐  │
│  │  DETAILS                              │  │
│  │  Title: [__________________]          │  │
│  │  Valid Until: [__/__/____]            │  │
│  └───────────────────────────────────────┘  │
│  ┌───────────────────────────────────────┐  │
│  │  LINE ITEMS (3)                       │  │
│  │  Sink Installation    $350  ×1  $350  │  │
│  │  Copper Pipe (ft)     $12   ×25 $300  │  │
│  │  Labor (hr)           $95   ×24 $2280 │  │
│  │  ┌─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─┐  │  │
│  │  │ + Add Line Item                │  │  │
│  │  └─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─┘  │  │
│  │  [Add from Catalog]                   │  │
│  └───────────────────────────────────────┘  │
│  ┌───────────────────────────────────────┐  │
│  │  NOTES                                │  │
│  │  [Add notes for the client...]        │  │
│  └───────────────────────────────────────┘  │
│  ┌───────────────────────────────────────┐  │
│  │  Subtotal              $2,930         │  │
│  │  Tax (8.5%)            $249           │  │
│  │  ═══════════════════════════════════  │  │
│  │  Total                 $3,179         │  │
│  └───────────────────────────────────────┘  │
├─────────────────────────────────────────────┤
│  [Preview]                    [Send ↗]      │  Bottom action bar
└─────────────────────────────────────────────┘
```

- Scrollable content: 16px horizontal margin, 12px vertical gap between card sections.
- Each section: Surface background, radius-md (12dp), shadow Level 1, 1px Outline Variant border, 16dp internal padding.
- Section labels: uppercase 11px, semibold, On Surface Variant, letter-spacing 0.5px, 8dp bottom margin.
- Bottom action bar: fixed bottom, Surface background, 1px Outline Variant top border, 12dp horizontal + 10dp vertical padding. Two buttons side by side with 10dp gap.

#### Desktop (905px+) — Detail Panel

The Quote Builder renders inside the detail panel of the master-detail layout. Layout differs between **view mode** and **edit mode**.

**View Mode (default):**
```
┌──────────────────────────────────────────────────────┐
│  Kitchen Renovation                                  │
│  👤 Bob Johnson · #QC-2026-0042 · Valid Feb 25 · [SENT] │
│                                    [Edit] [Preview] [Send ↗] │
├────────────────────────────┬─────────────────────────┤
│  LINE ITEMS (4)            │  CLIENT                  │
│  ┌──────────────────────┐  │  ┌───────────────────┐  │
│  │ Desc  Qty  Price Tot │  │  │ BJ Bob Johnson    │  │
│  │ ────────────────────  │  │  │ bob@email.com     │  │
│  │ Sink   1   $350 $350 │  │  │ (555) 123-4567    │  │
│  │ Pipe  25   $12  $300 │  │  └───────────────────┘  │
│  │ Faucet 1   $289 $289 │  │  DETAILS                │
│  │ Labor 24   $95 $2280 │  │  ┌───────────────────┐  │
│  └──────────────────────┘  │  │ Quote # QC-..042  │  │
│  NOTES                     │  │ Created  Feb 10   │  │
│  ┌──────────────────────┐  │  │ Valid    Feb 25   │  │
│  │ Includes all fixtures │  │  │ Status   [SENT]   │  │
│  │ and labor. Materials  │  │  └───────────────────┘  │
│  │ subject to...         │  │  TOTALS                 │
│  └──────────────────────┘  │  ┌───────────────────┐  │
│                            │  │ Subtotal   $3,219  │  │
│                            │  │ Tax 8.5%   $274    │  │
│                            │  │ ═══════════════    │  │
│                            │  │ Total      $4,611  │  │
│                            │  └───────────────────┘  │
└────────────────────────────┴─────────────────────────┘
```

- Detail panel padding: 28px top, 36px horizontal (20px/24px at Wide breakpoint).
- Header: flex row, space-between, 28px bottom margin.
- Grid: `grid-template-columns: 1fr 340px` (collapses to `1fr` at Wide breakpoint), gap 24px, `align-items: start`.

**Edit Mode (toggled by "Edit" button):**
- Same layout. Differences: delete buttons appear on line item rows. "Items-table-footer" with add-item buttons appears below items table. Notes card switches from read-only to editable textarea. Title becomes editable. Client becomes changeable.

### 3.3 Component Inventory

#### 3.3.1 Client Section

**Mobile:** Dropdown/combo box with search. Options from client list. Includes "Add New Client" option at top (opens inline creation form: name, email, phone fields + "Save" button).
- Dropdown: full-width, 1.5px Outline border, radius-sm, 12dp vertical + 14dp horizontal padding, 15px font.
- Selected client shows: name (15px semibold) with chevron-down icon.
- Unselected: placeholder "Select a client..." in On Surface Variant.

**Desktop (view mode):** Sidebar card — avatar (42dp circle, Primary Container bg, initials 15px bold Primary) + name (14px semibold) + email + phone (12px On Surface Variant each). 12px gap.

**Desktop (edit mode):** Same dropdown as mobile, rendered in the header area.

#### 3.3.2 Details Section

**Fields:**
- **Title:** Text input. Placeholder: "e.g. Kitchen Renovation". Required.
  - Spec: full-width, 1.5px Outline border, radius-sm, 12dp vertical + 14dp horizontal padding, 15px font.
  - Focus: Primary border. Error: Error border + helper text.
- **Valid Until:** Date input. Defaults to `createdAt + settings.quoteValidityDays`.
  - Uses platform date picker (native on mobile, calendar popup on desktop).

#### 3.3.3 Line Items Section

**Line Item Row (mobile):**
- Full-width, 14dp vertical padding, 1px Outline Variant bottom border.
- Row 1: description (14px semibold, flex 1, ellipsis) + delete "✕" button (48dp target, initially hidden, revealed on swipe or edit mode).
- Row 2: unit price × quantity (12px, On Surface Variant) + line total (15px semibold monospace, right-aligned).
- Tap: opens Line Item Editor modal pre-filled with item data.

**Line Item Table (desktop view mode):**
- Table with headers: Description, Qty, Unit Price, Total.
- Header row: Surface Variant background, 11px uppercase bold, letter-spacing 0.5px, On Surface Variant, 10px vertical + 16px horizontal padding, 2px On Surface bottom border.
- Data rows: 12px vertical + 16px horizontal padding, 1px Outline Variant bottom border, even rows `#FAFBFC`. Hover: `#F0F4FF`.
- Description: 14px 500 weight. Numerics: right-aligned, monospace 13px. Total: weight 600.

**Line Item Table (desktop edit mode):**
- Same table + extra "Delete" column (40px).
- Delete button: 16px ✕ icon, 4px × 8px padding, radius 4px. Hidden by default (`opacity: 0`). Appears on row hover. Hover over button: Error text + `#FEE2E2` bg.
- Below table: items-table-footer with two buttons:
  - "+ Add Line Item": dashed 1.5px Outline border, Primary text, radius-sm, 8dp × 16dp padding, 13px 500 weight. Hover: Primary Container bg, Primary border. Opens Line Item Editor modal.
  - "From Catalog": solid 1.5px Secondary border, `#92400E` text. Hover: Secondary Container bg. Opens Catalog Browser modal.

#### 3.3.4 Notes Section

**Mobile:** Multi-line textarea. Minimum 3 visible rows. Placeholder: "Add notes for the client...". Same input styling as other fields.

**Desktop (view mode):** 14px, On Surface Variant, line-height 1.6. Not editable.
**Desktop (edit mode):** Full-width textarea, 1.5px Outline border, radius-sm, 12dp padding, 14px font, min-height 80px, resize vertical. Focus: Primary border.

#### 3.3.5 Totals Section

- Subtotal row: label "Subtotal" left, amount right. 14px, On Surface Variant. Amount: monospace 500 weight.
- Tax row: label "Tax ({rate}%)" left, amount right. Same styling.
- Grand Total row: separated by 2px On Surface top border. Label: 20px (mobile) / 22px (desktop) bold. Amount: 22px (mobile) / 24px (desktop) bold monospace Primary.

#### 3.3.6 Action Buttons

**Mobile bottom action bar:**
- "Preview" — Secondary style: Surface background, 1.5px Primary border, Primary text, document icon. Flex: 1.
- "Send ↗" — Accent style: Secondary (amber) background, On Surface text, arrow icon. Flex: 1.
- Both: 48dp height, radius-md, 15px font, 600 weight, inline-flex centered, 8px icon gap.

**Desktop header buttons:**
- "Edit" / "Done" — Ghost style. Toggles `isEditMode`.
- "Preview" — Secondary style. Sets `isPreviewMode = true`.
- "Send ↗" — Accent style.
- Gap: 8px between buttons.

### 3.4 Interaction Specification

| Action | Trigger | Behavior |
|--------|---------|----------|
| Toggle edit mode (desktop) | Click "Edit" button | `isEditMode = !isEditMode`. Button label toggles "Edit" ↔ "Done". Reveals/hides delete buttons and add-item footer. Notes become editable. |
| Add line item (manual) | Tap "+ Add Line Item" | Opens Line Item Editor modal (see §3.8.1). |
| Add from catalog | Tap "Add from Catalog" / "From Catalog" | Opens Catalog Browser modal (see §3.8.2). |
| Edit line item | Tap row (mobile) / double-click row (desktop) | Opens Line Item Editor pre-filled with item data. |
| Delete line item | Swipe left + confirm (mobile) / click ✕ (desktop) | Removes item from quote. Toast: "Item removed". Recalculates totals. |
| Reorder line items | Drag handle (stretch goal v1) | Updates `sortOrder` for all affected items. |
| Save (mobile) | Tap "Save" button | Manual save (auto-save already handles this). Visual confirmation: button briefly shows ✓. Toast: "Quote saved". |
| Auto-save | Every field change | 500ms debounce. Saves to local storage. No visual indicator (silent). |
| Preview | Tap "Preview" | Mobile: pushes Preview screen (full-screen). Desktop: swaps detail panel to Preview sub-view. |
| Send | Tap "Send ↗" | Validates: client required, at least 1 line item, title required. If invalid: highlight missing fields with Error border + scroll to first error. If valid: sends email with PDF attachment. Status → `sent`. Toast: "Quote sent to {client.name}". |
| Download PDF | Preview → "Download PDF" | Generates PDF, triggers platform download/save dialog. |
| Email client | Preview → "Email Client" | Same as "Send" action. |
| Back (mobile) | Back arrow or system back | Pops to Quotes list. |
| Back from preview (desktop) | "← Back to Detail" button | `isPreviewMode = false`. Returns to detail view. |
| Escape (desktop) | Escape key | If edit mode: exit edit mode. If preview: back to detail. If modal open: close modal. |

### 3.5 States

#### Loading

- Mobile: The builder pre-populates from local storage. Instant render, no skeleton needed.
- Desktop: Selected quote loads from local storage into detail panel. Instant for cached data. If remote-only: skeleton placeholder in detail panel (title block + 3 table row skeletons).

#### Empty (New Quote)

- All sections visible but unpopulated.
- Client: shows dropdown with placeholder "Select a client...".
- Title: empty input with placeholder.
- Line Items: section visible with count "(0)". No item rows. Only the two add-item buttons.
- Notes: empty textarea with placeholder.
- Totals: all $0.00.

#### Error

- **Missing required field on send:** Error-color border (1.5px) on offending inputs. Helper text below: "Client is required", "Title is required", or "Add at least one line item". Scroll to first error.
- **PDF generation failure:** Toast: "Couldn't generate PDF. Please try again."
- **Email send failure:** Toast: "Email failed to send. Check your connection." Offer "Retry" in toast.

#### Disabled

- "Send ↗" button: visually muted (50% opacity, no hover effect) when title is empty, client is unset, or line items count is 0.
- "Add to Quote" (in line item editor): disabled until description AND price have values.

### 3.6 Validation Rules

| Field | Rule | Error Copy |
|-------|------|-----------|
| Quote title | Required, 1–200 chars | "Title is required" / "Title is too long" |
| Client | Required for send (can be empty during draft editing) | "Select a client before sending" |
| Line items | ≥ 1 required for send | "Add at least one line item" |
| Valid Until date | Must be today or future | "Expiry date must be in the future" |
| Line item description | Required, 1–500 chars | "Description is required" |
| Line item unit price | Required, ≥ 0.00, max 999,999.99 | "Enter a valid price" |
| Line item quantity | Required, integer ≥ 1, max 9,999 | "Quantity must be at least 1" |
| Notes | Optional, max 5,000 chars | "Notes are too long ({count}/5000)" |

Validation is **inline** (real-time as user types), not blocking (no modal alerts). Error state: 1.5px Error-color border on input + helper text below in Error color (12px).

### 3.7 Data Bindings

| UI Element | Reads | Writes | View Model |
|------------|-------|--------|------------|
| Client dropdown/card | `client` (resolved from `quote.clientId`) | `quote.clientId` | `QuoteDetailVM.quote.clientId` / `updateQuoteField('clientId', id)` |
| Title input | `quote.title` | `quote.title` | `QuoteDetailVM.quote.title` / `updateQuoteField('title', val)` |
| Valid Until input | `quote.validUntil` | `quote.validUntil` | `QuoteDetailVM.quote.validUntil` / `updateQuoteField('validUntil', date)` |
| Line items list/table | `quote.items` (sorted by `sortOrder`) | — | `QuoteDetailVM.quote.items` |
| Line item add | — | Appends to `quote.items` | `QuoteDetailVM.addLineItem(item)` |
| Line item delete | — | Removes from `quote.items` | `QuoteDetailVM.removeLineItem(id)` |
| Line item edit | `item.*` | `item.description`, `item.unitPrice`, `item.quantity` | `QuoteDetailVM.updateLineItem(id, changes)` |
| Notes | `quote.notes` | `quote.notes` | `QuoteDetailVM.quote.notes` / `updateQuoteField('notes', val)` |
| Subtotal | `quote.subtotal` (calculated) | — | `QuoteDetailVM.subtotal` |
| Tax | `quote.tax` (calculated) | — | `QuoteDetailVM.tax` |
| Tax rate label | `quote.taxRate` | — | — |
| Total | `quote.total` (calculated) | — | `QuoteDetailVM.total` |
| Quote number | `quote.number` | — | Read-only |
| Status badge | `quote.status` | — | Read-only |
| Edit/Done toggle | `isEditMode` | `isEditMode` | `QuoteDetailVM.toggleEdit()` |
| Preview toggle | `isPreviewMode` | `isPreviewMode` | `QuoteDetailVM.togglePreview()` |
| Send button enabled | `!hasValidationErrors` (derived) | — | `QuoteDetailVM.canSend` |

### 3.8 Modals

#### 3.8.1 Line Item Editor

**Trigger:** "+ Add Line Item" button or tap/double-click existing item row.

**Type:** Bottom sheet (mobile) / Modal dialog 480px (desktop).

**Header:** "Add Line Item" or "Edit Line Item". Close button (32dp circle, Surface Variant bg, ✕ 16px).

**Body fields:**

1. **Description** — text input, full-width, auto-focused on open. Label: "Description". Placeholder: "e.g. Sink Installation".
2. **Unit Price** — numeric input with "$" prefix. Label: "Unit Price". Grid: `1fr auto` with Quantity. Type: number, step 0.01. "$" prefix: absolute-positioned left 14px, On Surface Variant. Input left-padding: 28px.
3. **Quantity** — stepper component.
   - Container: inline-flex, 1.5px Outline border, radius-sm, overflow hidden.
   - "−" button: 44×44dp (mobile) / 38×38dp (desktop). Surface Variant background. Primary text. 18px bold "−".
   - Value display: 56px (mobile) / 48px (desktop) wide input, center-aligned, 16px/15px semibold, 1.5px Outline left + right borders.
   - "+" button: same as "−".
   - Hover (desktop): button bg → Primary Container. Press: Primary bg, white text.
   - Minimum: 1. Maximum: 9,999. Direct input also accepted (select all on focus).
4. **Line Total** — read-only display. 1.5px Outline Variant top border, 14px top padding, 8px top margin. Label (15px semibold) left, value (20px bold monospace Primary) right. Updates live as price/quantity change.

**Footer:** "Cancel" (Ghost) + "Add to Quote" or "Save Changes" (Primary).
- "Add to Quote" disabled until description is non-empty AND price > 0.
- On submit: adds/updates item, closes modal, recalculates totals. Toast: "Item added" or "Item updated".

**Dismiss:** Scrim tap, close button, Escape key, or Cancel.

**Pre-fill behavior (editing):** All fields populated with existing item data. Quantity stepper set to current value. Submit button reads "Save Changes" instead of "Add to Quote".

#### 3.8.2 Catalog Browser

**Trigger:** "Add from Catalog" / "From Catalog" button.

**Type:** Bottom sheet 85% height (mobile) / Modal dialog 560px, maxHeight 85vh (desktop).

**Header:** "{Catalog Name} Catalog" (e.g., "Plumbing Catalog"). Close button.

**Body:**

1. **Search input** — full-width, search icon, 14px bottom margin. Filters catalog items by `description CONTAINS query`.
2. **Category chips** — flex-wrap, 6px gap, 14px bottom margin. "All" + all category names. Same chip styling as Quotes filter. Filters items by category.
3. **Item list** — grouped by category when "All" is active.
   - **Group header:** 11px bold uppercase, On Surface Variant, 14px top + 6px bottom padding, 1px Outline Variant bottom border.
   - **Item row:** flex space-between, 12dp (mobile 14dp) vertical padding, 1px Outline Variant bottom border.
     - Left: description (14px 500 weight).
     - Right: price (14px semibold monospace On Surface Variant) + add button (12px gap).
     - **Add button:** 36dp (mobile) / 32dp (desktop) circle, Primary bg, white "+" 18px.
       - Hover (desktop): Primary Light, scale 1.1.
       - Click: adds item to quote as new LineItem (quantity 1, price from catalog default). Button transitions to Success green + "✓" for 1.5s. Pointer-events disabled during "added" state. Toast: "✓ {item.description} added".
       - After 1.5s: reverts to Primary "+" state.

**No footer** — items are added inline.

**Dismiss:** Scrim tap, close button, Escape key.

### 3.9 Analytics Events

| Event Name | Trigger | Payload |
|------------|---------|---------|
| `quote.viewed` | Builder/detail opened | `{ quoteId, status, itemCount }` |
| `quote.field_edited` | Any field changed | `{ quoteId, field }` |
| `quote.item_added` | Line item added (manual) | `{ quoteId, method: 'manual' }` |
| `quote.item_added_catalog` | Line item added from catalog | `{ quoteId, method: 'catalog', catalogItemId }` |
| `quote.item_removed` | Line item deleted | `{ quoteId, lineItemId }` |
| `quote.previewed` | Preview opened | `{ quoteId, total }` |
| `quote.sent` | Send confirmed | `{ quoteId, total, itemCount, clientId }` |
| `quote.pdf_downloaded` | PDF downloaded | `{ quoteId }` |
| `quote.edit_toggled` | Edit mode toggled (desktop) | `{ quoteId, isEditMode }` |

### 3.10 Accessibility

| Concern | Specification |
|---------|--------------|
| Form labels | Every input has a persistent visible `<label>`. Placeholder text supplements but never replaces. |
| Focus order | Client → Title → Valid Until → Line items → Add buttons → Notes → Preview → Send. |
| Line item table (desktop) | `role="table"` with `role="row"` and `role="cell"`. Column headers use `role="columnheader"`. |
| Stepper | "−" and "+" buttons: `aria-label="Decrease quantity"` / `"Increase quantity"`. Value display: `aria-live="polite"` to announce changes. |
| Delete button | `aria-label="Delete {item.description}"`. |
| Modal focus trap | On open: focus moves to first input. Tab cycles within modal. Escape closes. On close: focus returns to trigger element. |
| Totals | `aria-live="polite"` on total value. Screen reader announces when total changes (debounced 1s). |
| Error states | `aria-invalid="true"` on errored inputs. Error helper text linked via `aria-describedby`. |
| Send button state | `aria-disabled="true"` when conditions not met. `aria-label="Send quote to {client.name}"` when enabled. |

---

## 4. Clients Page

### 4.0 Goal & Primary User Tasks

| Task | Priority |
|------|----------|
| Find a client by name, email, or phone | P0 |
| View a client's quote history and total value | P1 |
| Add a new client | P1 |
| Edit client details | P2 |
| Create a new quote for a specific client | P1 |

### 4.1 Information Architecture

| Content | Why | Priority |
|---------|-----|----------|
| Client list (name, initials avatar, quote count, city, total value) | Primary content — the address book | Dominant |
| Search input | Fast lookup | High |
| Client detail: contact info + quote history | Gives context before creating a quote | High |
| Client summary stats (total quotes, total value, last quote) | At-a-glance client value | Medium |
| "+ Add Client" action | Creation entry point | High |

### 4.2 Layout Specification

#### Mobile (0–904px)

```
┌─────────────────────────────────────────────┐
│  App Bar: "Clients"                [+ Add]  │
├─────────────────────────────────────────────┤
│  [🔍 Search clients...]                     │
├─────────────────────────────────────────────┤
│  ┌─────────────────────────────────────┐    │
│  │ [BJ] Bob Johnson     $12,400       │    │
│  │      3 quotes · Anytown  total val  │    │
│  └─────────────────────────────────────┘    │
│  ┌─────────────────────────────────────┐    │
│  │ [MG] Maria Garcia    $5,200        │    │
│  │      2 quotes · Riverside           │    │
│  └─────────────────────────────────────┘    │
│  ...                                        │
├─────────────────────────────────────────────┤
│  [Quotes] [Clients] [Catalog] [Settings]    │
└─────────────────────────────────────────────┘
```

#### Desktop (905px+) — Master-Detail

```
┌──────┬──────────────────────────────────────────────────────┐
│      │  Top Bar: "Clients" · 8 contacts     [+ Add Client] │
│ Rail ├───────────────┬──────────────────────────────────────┤
│      │ [🔍 Search]   │  (client detail panel)               │
│      │ ──────────── │                                      │
│      │ [BJ] Bob J.  │  Bob Johnson                         │
│      │ [MG] Maria   │  bob@email.com · (555) 123-4567      │
│      │ [TW] Tom W.  │  [Edit] [+ New Quote]                │
│      │ ...          │  ──────────────────────────────       │
│      │              │  Quote History (3)  │ Summary          │
│      │              │  ────────────────   │ ──────           │
│      │              │  Kitchen Reno $4.6k │ Total: 3         │
│      │              │  Bath Remod $1.8k   │ Value: $12,400   │
│      │              │  Deck Build $7.5k   │ Last: Feb 10     │
└──────┴───────────────┴──────────────────────────────────────┘
```

- List panel: 380px (320px at Wide), same header/body structure as Quotes.
- Detail panel: grid `1fr 280px`, gap 24px.

### 4.3 Component Inventory

#### 4.3.1 Search Input

Same spec as Quotes search (§2.4.2). Placeholder: "Search clients...". Matches against `client.name`, `client.email`, `client.phone`.

#### 4.3.2 Client Card / Row

**Mobile card:**
- Horizontal layout: Avatar (left) + Info (center, flex 1) + Value (right).
- Avatar: 44dp circle, Primary Container background, Primary text initials (bold 16px).
- Info: Name (15px semibold), metadata line below (12px On Surface Variant) — "{quoteCount} quotes · {city}".
- Value: Right-aligned. Total value (14px semibold monospace Primary) above "total value" label (11px On Surface Variant).
- Card: Surface bg, radius-md, shadow Level 1, 1px Outline Variant border, 16dp padding, 14dp gap.

**Desktop list row:**
- Flex row, 14dp vertical + 16dp horizontal padding. 1px Outline Variant bottom border. Radius-sm.
- Avatar: 42dp circle (same spec). Info: name (14px semibold), meta (12px On Surface Variant, quote count + city). Value: amount (14px semibold monospace Primary), "total" label (10px On Surface Variant).
- Hover: Surface Variant background. Selected: Primary Container background.

**States (mobile):**
- Tap: pushes Client Detail full screen.
- Hover: shadow Level 2, translate Y −1px.
- Press: scale 0.985.

**States (desktop):**
- Click: selects client, shows detail in right panel.
- Hover: Surface Variant background.
- Selected: Primary Container, Primary border (1.5px).

#### 4.3.3 Client Detail (Desktop Panel / Mobile Full Screen)

**Header:**
- Name: 26px bold (desktop) / 22px bold (mobile).
- Meta row: icon + text pairs for email, phone, address. 13px On Surface Variant. Icons: 16px. 16px gap between items. Flex-wrap.
- Actions: "Edit" (Ghost) + "+ New Quote" (Accent). 8px gap.

**Quote History Card:**
- Card header: "Quote History ({count})".
- Item rows: flex space-between, 12dp vertical + 20dp horizontal padding, 1px Outline Variant bottom border.
  - Left: quote title (14px 500 weight), number + date below (12px On Surface Variant, 2px top margin).
  - Right: amount (15px semibold monospace Primary) + StatusBadge. 16px gap.
  - Tap: navigates to that quote's detail.
- Empty: centered "No quotes for this client yet." (14px On Surface Variant, 24dp padding).

**Summary Card (desktop sidebar):**
- Key-value rows (same style as Quote Details card):
  - Total Quotes: count (14px On Surface).
  - Total Value: formatted currency (monospace, 600 weight, Primary).
  - Last Quote: date or "—".

### 4.4 Interaction Specification

| Action | Trigger | Behavior |
|--------|---------|----------|
| Search | Type in search input | Debounce 200ms. Filters by name, email, phone (case-insensitive). |
| Open client | Tap card (mobile) / click row (desktop) | Mobile: push detail. Desktop: select + show in detail panel. |
| Add new client | Tap "+ Add" / "+ Add Client" | Opens Client Editor modal (see §4.8.1). |
| Edit client | Tap "Edit" on detail | Opens Client Editor modal pre-filled. |
| Create quote for client | Tap "+ New Quote" on detail | Creates new quote with `clientId` pre-set. Navigates to Quote Builder. |
| Open quote from history | Tap a quote history row | Navigates to Quotes tab → selects that quote. |
| Keyboard: ↑/↓ (desktop) | Arrow keys when list focused | Navigate client rows. |
| Keyboard: / (desktop) | Slash | Focus search input. |

### 4.5 States

| State | Behavior |
|-------|----------|
| Loading | Skeleton: 3 client card placeholders with avatar circle + text blocks pulsing. |
| Empty (no clients) | Icon: 👤 Person, 48px, 40% opacity. Title: "No clients yet". Desc: "Add your first client to start quoting." Button: "Add Client". |
| Empty search | "No clients matching \"{query}\"". No button. |
| Desktop empty detail | Icon: 👤 56px 25% opacity. Title: "Select a client". Desc: "Choose a client from the list to view their details and quote history." |
| Offline | Same offline banner as Quotes page. |

### 4.6 Validation Rules

Client validation applies in the Client Editor modal:

| Field | Rule | Error Copy |
|-------|------|-----------|
| Name | Required, 1–200 chars | "Client name is required" |
| Email | Optional. If provided, must match email format. | "Enter a valid email address" |
| Phone | Optional. If provided, 5–20 chars, digits/spaces/hyphens/parentheses/plus only. | "Enter a valid phone number" |
| Address | Optional, 0–500 chars | — |
| Duplicate warning | If name matches existing client (case-insensitive), show warning (not blocking): "A client named '{name}' already exists." | Non-blocking yellow banner. |

### 4.7 Modals

#### 4.7.1 Client Editor Modal

**Trigger:** "+ Add Client", "Edit" on detail, or "Add New" from quote builder client dropdown.

**Type:** Bottom sheet (mobile) / Modal dialog 480px (desktop).

**Header:** "Add Client" or "Edit Client". Close button.

**Fields:**
1. **Name** — text input, required, auto-focused. Label: "Name". Placeholder: "Full name or business name".
2. **Email** — email input. Label: "Email". Placeholder: "client@example.com".
3. **Phone** — tel input. Label: "Phone". Placeholder: "(555) 123-4567".
4. **Address** — text input. Label: "Address". Placeholder: "123 Main St, City, State".

All fields: full-width, 1.5px Outline border, radius-sm, 12dp vertical + 14dp horizontal padding, 15px font. Focus: Primary border. 16dp gap between fields.

**Footer:** "Cancel" (Ghost) + "Save Client" (Primary). Disabled until name is non-empty.

**On save:** Creates/updates client. Closes modal. Toast: "Client saved". If opened from quote builder: auto-selects the new client.

**Dismiss:** Scrim, close, Escape, Cancel.

#### 4.7.2 Delete Client Confirmation

- Not in v1 scope. Clients are not deletable in MVP (they may have associated quotes). Future consideration: soft-delete with orphan quote handling.

### 4.8 Data Bindings

| UI Element | Reads | Writes | View Model |
|------------|-------|--------|------------|
| Client list | `clients` (filtered, sorted by name) | — | `ClientListVM.filteredClients` |
| Search input | `clientSearchQuery` | `clientSearchQuery` | `ClientListVM.searchQuery` / `setSearch()` |
| Selected highlight | `selectedClientId` | `selectedClientId` | `ClientListVM.selectClient()` |
| Detail: name | `client.name` | — (via editor modal) | `ClientDetailVM.client.name` |
| Detail: email/phone/address | `client.email/phone/address` | — (via modal) | `ClientDetailVM.client.*` |
| Quote history | `quoteHistory` (quotes where clientId matches) | — | `ClientDetailVM.quoteHistory` |
| Summary: total quotes | `client.quoteCount` (aggregated) | — | `ClientDetailVM.client.quoteCount` |
| Summary: total value | `client.totalValue` (aggregated) | — | `ClientDetailVM.client.totalValue` |
| Summary: last quote | `client.lastQuoteDate` (aggregated) | — | `ClientDetailVM.client.lastQuoteDate` |
| Avatar | `client.name` (derives initials) | — | — |
| Top bar count | `clients.length` | — | `ClientListVM.totalCount` |

### 4.9 Analytics Events

| Event | Trigger | Payload |
|-------|---------|---------|
| `clients.viewed` | Page loaded | `{ count }` |
| `clients.searched` | Search submitted | `{ queryLength, resultCount }` |
| `clients.opened` | Client selected | `{ clientId, quoteCount }` |
| `clients.created` | Client saved (new) | `{ clientId }` |
| `clients.edited` | Client saved (existing) | `{ clientId, fieldsChanged }` |
| `clients.quote_created` | "+ New Quote" from detail | `{ clientId }` |
| `clients.quote_opened` | History row tapped | `{ clientId, quoteId }` |

### 4.10 Accessibility

| Concern | Spec |
|---------|------|
| Client list | `role="list"`. Each card/row: `role="listitem"`, `tabindex="0"`, `aria-label="{name}, {quoteCount} quotes, {totalValue} total value"`. |
| Avatar | Decorative (`aria-hidden="true"`). Name is always present as text. |
| Contact info (detail) | Email/phone as `<a>` elements where applicable (`mailto:`, `tel:`). |
| Search | `aria-label="Search clients"`. |
| Quote history | `role="list"`. Each row: `role="listitem"`, `aria-label="{title}, {amount}, {status}"`. |
| Modal focus | Auto-focus on Name input. Tab cycles within modal. Escape closes. |

---

## 5. Catalog Page

### 5.0 Goal & Primary User Tasks

| Task | Priority |
|------|----------|
| Browse catalog items by category | P0 |
| Search for a specific item | P0 |
| Add a new item with description, price, category | P1 |
| Edit an item's price or description | P1 |
| Delete an item no longer offered | P2 |
| Manage catalogs (add, rename) | P2 |

### 5.1 Information Architecture

| Content | Why | Priority |
|---------|-----|----------|
| Item grid/list (description + price, grouped by category) | Primary content — the pricing inventory | Dominant |
| Category filter chips | Rapid filtering for large catalogs | High |
| Search input | Fast lookup | High |
| Catalog name (subtitle) | Which catalog is being viewed | Medium |
| "+ Add Item" action | Creation entry point | High |

### 5.2 Layout Specification

#### Mobile (0–904px)

```
┌─────────────────────────────────────────────┐
│  App Bar: "Catalog"                [+ Item] │
├─────────────────────────────────────────────┤
│  [🔍 Search...]                              │
├─────────────────────────────────────────────┤
│  [All] [Installations] [Repairs] [Materials] │  Category chips
├─────────────────────────────────────────────┤
│  INSTALLATIONS                               │  Category header (when "All")
│  ┌─────────────────────────────────────┐    │
│  │ Sink Installation          $350  >  │    │
│  │ Faucet Replacement         $289  >  │    │
│  └─────────────────────────────────────┘    │
│  REPAIRS                                     │
│  ┌─────────────────────────────────────┐    │
│  │ Pipe Leak Repair           $150  >  │    │
│  └─────────────────────────────────────┘    │
│  ...                                        │
├─────────────────────────────────────────────┤
│  [Quotes] [Clients] [Catalog] [Settings]    │
└─────────────────────────────────────────────┘
```

- Item list: vertically scrollable. Items within each category group separated by 1px Outline Variant bottom border.
- Each item row: description left, price + chevron-right right. 14dp vertical + 16dp horizontal padding.
- Tapping: opens item editor.

#### Desktop (905px+) — Full-Width Grid (no master-detail)

```
┌──────┬──────────────────────────────────────────────────────┐
│      │  "Catalog" · Plumbing      [🔍______] [+ Add Item]  │
│ Rail ├──────────────────────────────────────────────────────┤
│      │  [All] [Installations] [Repairs] [Materials] [Labor] │
│      ├──────────────────────────────────────────────────────┤
│      │  INSTALLATIONS                                        │
│      │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌────────┐ │
│      │  │ Sink Inst│ │ Faucet   │ │ Toilet   │ │ Shower │ │
│      │  │ $350     │ │ $289     │ │ $450     │ │ $375   │ │
│      │  └──────────┘ └──────────┘ └──────────┘ └────────┘ │
│      │  REPAIRS                                              │
│      │  ┌──────────┐ ┌──────────┐                           │
│      │  │ Pipe Leak│ │ Valve    │                           │
│      │  │ $150     │ │ $200     │                           │
│      │  └──────────┘ └──────────┘                           │
└──────┴──────────────────────────────────────────────────────┘
```

- Grid: `repeat(auto-fill, minmax(260px, 1fr))` (220px at Wide breakpoint). Gap: 12px.
- Padding: 20px vertical, 36px horizontal.
- Category group headers: 11px bold uppercase, On Surface Variant, letter-spacing 1px, 20px horizontal + 20px top + 8px bottom padding. Shown only when "All" category is active.

### 5.3 Component Inventory

#### 5.3.1 Top Bar (Desktop)

- Left: "Catalog" title (20px bold) + catalog name subtitle (13px On Surface Variant).
- Right: search input (240px wide, 34px left padding for icon) + "+ Add Item" accent button. 10px gap.

#### 5.3.2 Category Chip Bar

Same spec as Quotes chip bar. Chips: "All" + all `DISTINCT(catalogItem.category)` values from active catalog. Active filter: `catalogFilterCategory`. Count badges: item count per category.

#### 5.3.3 Catalog Item Card (Desktop)

| Property | Value |
|----------|-------|
| Background | Surface |
| Border | 1px Outline Variant |
| Radius | radius-md (12dp) |
| Padding | 16dp |
| Layout | flex row, space-between, align-center |
| Cursor | pointer |
| Hover | Primary border, shadow-sm |

- Left: item name (14px, 500 weight, On Surface). Ellipsis if overflows.
- Right: price (14px, 600 weight, monospace, On Surface Variant).
- Click: opens Catalog Item Editor modal.

#### 5.3.4 Catalog Item Row (Mobile)

- Full-width, 14dp vertical + 16dp horizontal padding, 1px Outline Variant bottom border.
- Left: description (14px, 500 weight).
- Right: price (14px, semibold, monospace, On Surface Variant) + chevron-right icon (16px, On Surface Variant, 12px left gap).
- Tap: opens Catalog Item Editor modal.

### 5.4 Interaction Specification

| Action | Trigger | Behavior |
|--------|---------|----------|
| Filter by category | Tap chip | Sets `catalogFilterCategory`. Re-filters grid/list. |
| Search | Type in search (desktop: top bar, mobile: below app bar) | Debounce 200ms. Filters items where `description CONTAINS query` (case-insensitive). |
| Open item editor | Tap/click item | Opens Catalog Item Editor modal pre-filled (see §5.8.1). |
| Add new item | Tap "+ Item" / "+ Add Item" | Opens Catalog Item Editor modal empty (see §5.8.1). |
| Delete item | Delete button inside editor modal | Removes item from catalog. Toast: "Item deleted". |
| Keyboard: / (desktop) | Slash | Focuses top bar search. |

### 5.5 States

| State | Behavior |
|-------|----------|
| Loading | Skeleton: chip bar (4 gray pills pulsing) + 6 card placeholders in grid (Surface, radius-md, 60dp height, pulsing). |
| Empty (no items) | Icon: 📖 Book, 48px, 40% opacity. Title: "Catalog is empty". Desc: "Add items to your pricing catalog for faster quoting." Button: "Add Item". |
| Empty search | "No items matching \"{query}\"". |
| Empty category filter | "No items in {category}." |
| Offline | Same banner. All CRUD operations work offline. |

### 5.6 Validation Rules

Catalog Item validation (in editor modal):

| Field | Rule | Error Copy |
|-------|------|-----------|
| Description | Required, 1–500 chars | "Description is required" |
| Unit Price | Required, ≥ 0.00, max 999,999.99 | "Enter a valid price" |
| Category | Required, 1–100 chars | "Category is required" |

### 5.7 Modals

#### 5.7.1 Catalog Item Editor Modal

**Trigger:** Tap/click any item, or "+ Add Item" button.

**Type:** Bottom sheet (mobile) / Modal dialog 480px (desktop).

**Header:** "Add Catalog Item" or "Edit Catalog Item". Close button.

**Fields:**
1. **Description** — text input, required, auto-focused. Label: "Description". Placeholder: "e.g. Sink Installation".
2. **Unit Price** — numeric input with "$" prefix. Label: "Default Price". Type: number, step 0.01.
3. **Category** — combo box / dropdown with search + free-text creation. Label: "Category". Options: all existing category names. If typed value doesn't match, creates new category.

All fields: same styling as Line Item Editor inputs. 16dp gap between fields.

**Footer (new item):** "Cancel" (Ghost) + "Add Item" (Primary). Disabled until description, price, and category all have values.

**Footer (editing):** "Delete" (Error-colored Ghost, left-aligned) + "Cancel" (Ghost) + "Save Changes" (Primary).

**On save:** Creates/updates catalog item. Toast: "Item saved" / "Item added to catalog".

**Delete flow:** "Delete" button triggers inline confirmation within the modal footer. Footer swaps to: "This will permanently delete this item." + "Cancel" (Ghost) + "Delete" (Error bg, white text). On confirm: deletes item, closes modal. Toast: "Item deleted".

### 5.8 Data Bindings

| UI Element | Reads | Writes | View Model |
|------------|-------|--------|------------|
| Item grid/list | `items` (filtered by category + search) | — | `CatalogVM.filteredItems` |
| Category chips | `categories` (distinct) + `catalogFilterCategory` | `catalogFilterCategory` | `CatalogVM.setCategory()` |
| Search | `catalogSearchQuery` | `catalogSearchQuery` | `CatalogVM.setSearch()` |
| Top bar subtitle | `activeCatalog.name` | — | `CatalogVM.activeCatalog.name` |
| Category chip counts | Aggregated item counts per category | — | `CatalogVM.categoryCounts` |
| Item card: description | `catalogItem.description` | — | — |
| Item card: price | `catalogItem.unitPrice` (formatted) | — | — |
| Modal fields | `catalogItem.*` (when editing) | All fields | `CatalogVM.addItem()` / `editItem()` / `deleteItem()` |

### 5.9 Analytics Events

| Event | Trigger | Payload |
|-------|---------|---------|
| `catalog.viewed` | Page loaded | `{ catalogId, itemCount }` |
| `catalog.filtered` | Category chip tapped | `{ category, resultCount }` |
| `catalog.searched` | Search submitted | `{ queryLength, resultCount }` |
| `catalog.item_created` | New item saved | `{ catalogItemId, category }` |
| `catalog.item_edited` | Existing item saved | `{ catalogItemId, fieldsChanged }` |
| `catalog.item_deleted` | Item deleted | `{ catalogItemId, category }` |
| `catalog.item_opened` | Item card tapped | `{ catalogItemId }` |

### 5.10 Accessibility

| Concern | Spec |
|---------|------|
| Grid (desktop) | `role="grid"`. Each card: `role="gridcell"`, `tabindex="0"`, `aria-label="{description}, {price}"`. |
| List (mobile) | `role="list"`. Each row: `role="listitem"`, `tabindex="0"`. |
| Category chips | `role="tablist"` / `role="tab"`. `aria-selected` on active. |
| Group headers | `role="heading"`, `aria-level="3"`. Decorative when only one group visible. |
| Modal | Same focus trap rules. Auto-focus on Description. |
| Delete confirmation | Focus moves to confirmation text. "Delete" button has `aria-label="Confirm delete {description}"`. |

---

## 6. Settings Page

### 6.0 Goal & Primary User Tasks

| Task | Priority |
|------|----------|
| Enter business info (name, phone, email, address, logo) | P0 (first run) |
| Configure tax rate | P1 |
| Set quote validity period | P2 |
| Set quote number prefix | P2 |
| View subscription status | P1 |
| Upgrade to Pro | P1 (for free users) |
| Manage catalogs | P2 |

### 6.1 Information Architecture

| Content | Why | Priority |
|---------|-----|----------|
| Upgrade banner (free tier only) | Conversion opportunity | High (top) |
| Business Info group | Controls what appears on quotes/PDF | High |
| Quote Settings group | Defaults for new quotes | Medium |
| My Catalogs group | Catalog management | Medium |
| Subscription group | Usage tracking + upgrade | Medium |

### 6.2 Layout Specification

Settings is a **single-column scrollable layout** on all breakpoints. No master-detail.

#### Mobile (0–904px)

```
┌─────────────────────────────────────────────┐
│  App Bar: "Settings"                        │
├─────────────────────────────────────────────┤
│  ┌───────────────────────────────────────┐  │
│  │  🔓 Unlock QuoteCraft Pro     [CTA]  │  │  Upgrade banner
│  └───────────────────────────────────────┘  │
│                                             │
│  BUSINESS INFO                              │
│  ┌───────────────────────────────────────┐  │
│  │  Business Name       Smith Plumbing   │  │
│  │  ─────────────────────────────────────│  │
│  │  Phone              (555) 123-4567    │  │
│  │  ─────────────────────────────────────│  │
│  │  Email              info@smith.com    │  │
│  │  ─────────────────────────────────────│  │
│  │  Address            123 Trade St...   │  │
│  │  ─────────────────────────────────────│  │
│  │  Logo               Upload ↗          │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  QUOTE SETTINGS                             │
│  ┌───────────────────────────────────────┐  │
│  │  Default Tax Rate        [8.50] %     │  │
│  │  ─────────────────────────────────────│  │
│  │  Valid For               [14] days    │  │
│  │  ─────────────────────────────────────│  │
│  │  Currency                USD ($)      │  │
│  │  ─────────────────────────────────────│  │
│  │  Quote Prefix            [QC-]        │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  MY CATALOGS                                │
│  ┌───────────────────────────────────────┐  │
│  │  📖 Plumbing            18 items      │  │
│  │  ─────────────────────────────────────│  │
│  │  + Add Catalog                        │  │
│  └───────────────────────────────────────┘  │
│                                             │
│  SUBSCRIPTION                               │
│  ┌───────────────────────────────────────┐  │
│  │  Current Plan       Free (5 quotes/mo)│  │
│  │  ─────────────────────────────────────│  │
│  │  Quotes Used        3 of 5            │  │
│  └───────────────────────────────────────┘  │
├─────────────────────────────────────────────┤
│  [Quotes] [Clients] [Catalog] [Settings]    │
└─────────────────────────────────────────────┘
```

- Horizontal padding: 16dp (mobile) / 36dp (desktop).
- Max-width: 720px (desktop), centered.
- Vertical gap between groups: 28dp.
- Content area: scrollable.

#### Desktop

Same structure, rendered inside the main content area (right of nav rail, below top bar). Padding: 28px top, 36px horizontal. Max-width: 720px.

### 6.3 Component Inventory

#### 6.3.1 Upgrade Banner

**Visibility:** Only shown when `subscription.tier == 'free'`.

**Spec:**
- Background: `linear-gradient(135deg, Primary, #1A365D)`. Color: white.
- Radius: radius-md (12dp). Padding: 24dp.
- Layout: flex row, space-between, align-center, 24dp gap.
- Left: Title "Unlock QuoteCraft Pro" (18px bold), description "Unlimited quotes, custom branding, and priority support." (13px, 80% opacity).
- Right: CTA button "Upgrade — $15/mo" — amber background, On Surface text, bold 14px, 12dp vertical + 28dp horizontal padding, radius-sm. Flex-shrink: 0.
- CTA hover (desktop): darker amber (`#E8930A`).
- Bottom margin: 28dp.

#### 6.3.2 Settings Group

- **Group title:** 11px bold uppercase, letter-spacing 1px, On Surface Variant. Padding-left 4px. 8dp bottom margin.
- **Group card:** Surface background, radius-md, shadow Level 1 (sm on desktop), 1px Outline Variant border.
- **Bottom margin:** 28dp between groups.

#### 6.3.3 Settings Row

- Flex row, space-between, align-center.
- Padding: 14dp vertical × 16dp horizontal (mobile) / 14dp vertical × 20dp horizontal (desktop).
- Bottom border: 1px Outline Variant. Last row: no border.
- Hover (desktop): `#FAFBFC` background, 100ms transition.

**Static row:**
- Label: 14px, 500 weight, On Surface (left).
- Value: 14px, On Surface Variant, right-aligned (right). Max-width 55%, ellipsis overflow.

**Editable input row:**
- Label: same as static.
- Input: replaces static value. 1px Outline border, radius 6px, 6dp vertical + 10dp horizontal padding, 14px font, right-aligned. Width: varies (50–100px). Focus: Primary border.
- Optional suffix: "%" or "days". On Surface Variant, 4dp left gap from input.

**Tappable row (mobile, for fields that open editors):**
- Entire row is tappable (cursor pointer on desktop).
- Chevron-right icon on far right (16px, On Surface Variant).
- On desktop: some fields are inline-editable (tax rate, validity, prefix). On mobile: all fields tap to open editor.

### 6.4 Settings Fields — Complete Inventory

#### Group 1: Business Info

| Row | Label | Value Display | Edit Mechanism | Validation |
|-----|-------|--------------|----------------|------------|
| Business Name | "Business Name" | Text or "Not set" (On Surface Variant, italic) | Mobile: tap → edit modal. Desktop: tap to focus inline input. | Optional, 0–200 chars |
| Phone | "Phone" | Formatted phone or "Not set" | Same | Optional, 5–20 chars |
| Email | "Email" | Email or "Not set" | Same | Optional, valid email format |
| Address | "Address" | Address (may truncate) or "Not set" | Same | Optional, 0–500 chars |
| Logo | "Logo" | "Upload ↗" link (Primary color) or thumbnail (28dp, radius 4dp) + "Change" link | Tap → native file picker. Accepts PNG, JPG, SVG. Max 5MB. | Image format + size only |

#### Group 2: Quote Settings

| Row | Label | Input | Suffix | Default | Validation |
|-----|-------|-------|--------|---------|------------|
| Default Tax Rate | "Default Tax Rate" | Number input, step 0.01, width 70px | "%" | 8.50 | 0–100, 2 decimal places. Error: "Tax rate must be between 0 and 100" |
| Valid For | "Valid For" | Number input, step 1, width 60px | "days" | 14 | Integer, 1–365. Error: "Must be between 1 and 365 days" |
| Currency | "Currency" | Static display (dropdown in future) | — | "USD ($)" | Not editable in v1 |
| Quote Prefix | "Quote Prefix" | Text input, width 70px | — | "QC-" | 1–10 chars, alphanumeric + hyphens. Error: "Prefix must be 1–10 characters (letters, numbers, hyphens)" |

#### Group 3: My Catalogs

| Row | Content | Behavior |
|-----|---------|----------|
| Catalog row | Left: 📖 icon (16px) + catalog name (14px 500 weight). Right: "{count} items" (14px On Surface Variant). | Tap: navigates to Catalog tab with this catalog selected. |
| + Add Catalog | Left: "+" icon (16px Primary) + "Add Catalog" text (14px Primary 500 weight). Right: empty. | Tap: opens Add Catalog modal (see §6.7.1). |

#### Group 4: Subscription

| Row | Label | Value | Notes |
|-----|-------|-------|-------|
| Current Plan | "Current Plan" | "Free (5 quotes/mo)" or "Pro (Unlimited)" | Static |
| Quotes Used | "Quotes Used" | "{used} of {allowed}" or "Unlimited" (Pro) | Updates when quotes are created. Resets monthly. |

If Pro tier: "Quotes Used" row shows "Unlimited" with a subtle ✓ icon. Upgrade banner is hidden.

### 6.5 Interaction Specification

| Action | Trigger | Behavior |
|--------|---------|----------|
| Edit business info (mobile) | Tap a business info row | Opens modal/sheet with that field editable. Save updates setting. |
| Edit business info (desktop) | Click on value area | Inline edit. Focus → type → blur/Enter saves. Escape reverts. |
| Change tax rate | Type in input | Auto-saves on blur/Enter. Validates range. Updates default for new quotes (doesn't retroactively change existing quotes). |
| Change validity period | Type in input | Same auto-save. |
| Change quote prefix | Type in input | Same auto-save. Affects only future quote numbers. |
| Upload logo | Tap "Upload ↗" or logo thumbnail | Opens native file picker. On selection: validates format + size, saves to local storage, updates thumbnail display. |
| Tap catalog row | Tap | Navigates to Catalog tab, selects that catalog. |
| Add catalog | Tap "+ Add Catalog" | Opens Add Catalog modal (§6.7.1). |
| Upgrade | Tap CTA button | Navigates to subscription management / payment flow (external). |
| Auto-save | Every field change | 300ms debounce. Saves to local storage. No visual confirmation (silent save). |

### 6.6 States

| State | Behavior |
|-------|----------|
| First run | All business info shows "Not set" (italic On Surface Variant). Tax rate: 8.50. Validity: 14. Prefix: "QC-". Catalogs: empty (show "Add your first catalog" prompt inline). |
| Loading | Instant (settings are small, loaded from local storage). No skeleton. |
| Save error | Toast: "Setting couldn't be saved. Please try again." Input reverts to previous value. |
| Logo upload error | Toast: "Image must be PNG, JPG, or SVG under 5MB." |
| Offline | Settings changes save locally. Will sync when online. No visible difference. |
| Pro tier | Upgrade banner hidden. Subscription group shows "Pro (Unlimited)". "Quotes Used" shows "Unlimited". |

### 6.7 Modals

#### 6.7.1 Add Catalog Modal

**Trigger:** "+ Add Catalog" row.

**Type:** Bottom sheet (mobile) / Modal dialog 400px (desktop).

**Header:** "Add Catalog". Close button.

**Fields:**
1. **Catalog Name** — text input, auto-focused. Label: "Catalog Name". Placeholder: "e.g. Plumbing, Electrical, HVAC".

**Footer:** "Cancel" (Ghost) + "Create Catalog" (Primary). Disabled until name is non-empty.

**Validation:**
- Name: required, 1–100 chars. Error: "Catalog name is required".
- Duplicate check: if name matches existing catalog (case-insensitive), error: "A catalog named '{name}' already exists."

**On save:** Creates catalog. Closes modal. Toast: "Catalog created". New catalog appears in My Catalogs list.

#### 6.7.2 Business Info Editor Modal (Mobile Only)

On mobile, tapping a business info row opens a bottom sheet with a single field editor.

**Header:** "Edit {field name}" (e.g., "Edit Business Name"). Close button.
**Body:** Single input field, auto-focused, pre-filled with current value.
**Footer:** "Cancel" (Ghost) + "Save" (Primary).
**On save:** Updates setting. Closes modal. Silent save (no toast for single-field edits).

### 6.8 Data Bindings

| UI Element | Reads | Writes | View Model |
|------------|-------|--------|------------|
| Upgrade banner visibility | `subscription.tier` | — | `SettingsVM.subscription.tier` |
| Upgrade CTA | — | Triggers upgrade flow | `SettingsVM.initiateUpgrade()` |
| Business Name | `settings.businessName` | `settings.businessName` | `SettingsVM.updateSetting('businessName', val)` |
| Phone | `settings.phone` | `settings.phone` | `SettingsVM.updateSetting('phone', val)` |
| Email | `settings.email` | `settings.email` | `SettingsVM.updateSetting('email', val)` |
| Address | `settings.address` | `settings.address` | `SettingsVM.updateSetting('address', val)` |
| Logo | `settings.logoUri` | `settings.logoUri` | `SettingsVM.uploadLogo(file)` |
| Tax Rate input | `settings.defaultTaxRate` | `settings.defaultTaxRate` | `SettingsVM.updateSetting('defaultTaxRate', val)` |
| Validity input | `settings.quoteValidityDays` | `settings.quoteValidityDays` | `SettingsVM.updateSetting('quoteValidityDays', val)` |
| Currency | `settings.currency` | — (not editable v1) | — |
| Prefix input | `settings.quotePrefix` | `settings.quotePrefix` | `SettingsVM.updateSetting('quotePrefix', val)` |
| Catalog list | `catalogs` (sorted by name) | — | `SettingsVM.catalogs` |
| Catalog row item count | `catalog.itemCount` | — | — |
| Current Plan | `subscription.tier`, `subscription.quotesAllowed` | — | `SettingsVM.subscription.*` |
| Quotes Used | `subscription.quotesUsedThisMonth`, `subscription.quotesAllowed` | — | `SettingsVM.subscription.*` |

### 6.9 Analytics Events

| Event | Trigger | Payload |
|-------|---------|---------|
| `settings.viewed` | Page loaded | `{ tier }` |
| `settings.field_updated` | Any setting saved | `{ field, hasValue: bool }` |
| `settings.logo_uploaded` | Logo saved | `{ fileSize, format }` |
| `settings.logo_removed` | Logo cleared | — |
| `settings.catalog_created` | New catalog saved | `{ catalogId, name }` |
| `settings.upgrade_tapped` | CTA button pressed | `{ currentTier, source: 'settings' }` |
| `settings.catalog_opened` | Catalog row tapped | `{ catalogId }` |

### 6.10 Accessibility

| Concern | Spec |
|---------|------|
| Groups | Each group card: `role="group"`, `aria-labelledby` pointing to group title element. |
| Editable rows | Input: `aria-label="{label}"`. Suffix ("%" / "days"): `aria-hidden="true"` (the label conveys the unit). |
| Static rows | Value is `aria-describedby` of the label. Not interactive (no tabindex). |
| Logo upload | Button: `aria-label="Upload business logo"`. Accepts image files only. |
| Upgrade banner | `role="banner"`. CTA: `aria-label="Upgrade to Pro for $15 per month"`. |
| Settings sections | Logical focus order: Upgrade CTA → Business Info fields → Quote Settings fields → Catalog rows → Add Catalog → Subscription info. |
| "Not set" values | Announced as "{label}, not set" by screen reader. |

---

# Appendices

---

## Appendix A — Design System Tokens

### A.1 Color Palette

| Token | Hex | Usage |
|-------|-----|-------|
| Primary | `#2B4C7E` | App bar, primary buttons, active states, links |
| Primary Dark | `#1A365D` | Upgrade banner gradient |
| Primary Light | `#3A6298` | Button hover states |
| Primary Container | `#D6E4F0` | Selected items, active chips, hover states |
| Secondary (Amber) | `#F59E0B` | CTA buttons, accent highlights, FAB |
| Secondary Dark | `#D97706` | Amber hover |
| Secondary Container | `#FEF3C7` | Quote total background, notification badges |
| Surface | `#FFFFFF` | Cards, sheets, modal backgrounds |
| Surface Variant | `#F3F4F6` | Page backgrounds, input fields (unfocused) |
| On Surface | `#1F2937` | Primary text, headings |
| On Surface Variant | `#6B7280` | Secondary text, labels, placeholders |
| Outline | `#D1D5DB` | Card borders, input outlines |
| Outline Variant | `#E5E7EB` | Subtle separators, internal dividers |
| Error | `#DC2626` | Declined quotes, validation errors, destructive actions |
| Success | `#16A34A` | Accepted quotes, confirmations |

### A.2 Typography

| Element | Size | Weight | Line-height | Font |
|---------|------|--------|-------------|------|
| Page title (mobile) | 22px | 700 | 1.2 | Body (DM Sans / system) |
| Page title (desktop) | 20px | 700 | 1.2 | Body |
| Section header | 16px | 600 | 1.3 | Body |
| Body | 14–16px | 400 | 1.5–1.6 | Body |
| Labels | 14px | 500 | 1 | Body |
| Captions | 12px | 400 | 1.2 | Body |
| Overline | 11px | 700 | 1 | Body (UPPERCASE, 0.5–1px tracking) |
| Monospace amounts | Varies | 500–700 | 1 | Mono (JetBrains Mono / system mono) |
| Grand total | 20–24px | 700 | 1 | Mono |

### A.3 Spacing

8px base grid: `xs: 4`, `sm: 8`, `md: 16`, `lg: 24`, `xl: 32`, `2xl: 48`.

### A.4 Radius

`sm: 8dp`, `md: 12dp`, `lg: 16dp`, `xl: 24dp`, `pill: 9999px`.

### A.5 Shadows

| Level | Description | Usage |
|-------|-------------|-------|
| 1 (sm) | 1px offset, 3px blur, 6–8% opacity | Cards, list items |
| 2 (md) | 4px offset, 12px blur, 7–8% opacity | Hovered cards |
| 3 (lg) | 12px offset, 32px blur, 10–12% opacity | FAB, bottom sheets |
| 4 (xl) | 20px offset, 48px blur, 16% opacity | Modal dialogs |

### A.6 Touch Targets

| Element | Mobile Minimum | Desktop Minimum |
|---------|---------------|----------------|
| All interactive elements | 48×48dp | 36×36dp |
| Primary action buttons | 56dp height | 40dp height |
| List item rows | 56dp height | — |
| FAB | 56×56dp | — (replaced by button) |
| Stepper buttons | 44×44dp | 38×38dp |

### A.7 Breakpoints

| Name | Range | Navigation | Layout |
|------|-------|-----------|--------|
| Narrow | 0–599px | Bottom Tab Bar | Single column |
| Medium | 600–904px | Bottom Tab Bar | Single column, wider margins |
| Wide | 905–1280px | Navigation Rail (80px) | Master-detail (reduced density) |
| Extra Wide | 1281px+ | Navigation Rail (80px) | Master-detail (full density) |

---

## Appendix B — PDF Generation Specification

### B.1 Page Setup

- Size: US Letter (8.5" × 11"), portrait. Margins: 0.75" all sides.
- Font: clean sans-serif (same family as app where possible).

### B.2 Layout

Mirrors the Quote Preview screen (§3.3). Key sections: branded header (Primary bg, logo, business info, quote meta), client section, quote title, line items table (alternating rows), totals block (right-aligned), notes block (Surface Variant bg), footer.

### B.3 Tier Differences

| Element | Free | Pro |
|---------|------|-----|
| Header logo | Placeholder / "QC" text | Custom uploaded logo |
| Header color | Primary | Primary (customizable in future) |
| Footer text | "Created with QuoteCraft" | Custom text (e.g., "Licensed & Insured — #12345") |
| Branding watermark | Present | Removed |

---

## Appendix C — Monetization & Tier Boundaries

| Feature | Free | Pro ($15/mo) |
|---------|------|-------------|
| Quotes per month | 5 | Unlimited |
| Clients | Unlimited | Unlimited |
| Catalog items | Unlimited | Unlimited |
| PDF generation | ✓ (with branding) | ✓ (custom branding) |
| Custom logo | ✗ | ✓ |
| Custom footer text | ✗ | ✓ |
| Custom PDF colors | ✗ | ✓ |
| Priority support | ✗ | ✓ |

**Upgrade prompts appear:**
1. In Settings (always visible upgrade banner for free users).
2. Contextually when hitting the monthly quote limit.
3. Never aggressively (no mid-flow interruptions, no pop-ups, no nagware).

---

## Appendix D — Iconography Reference

Use a single icon family consistently. Material Symbols, Fluent UI, SF Symbols, or Lucide are all acceptable. Do not mix families.

| Concept | Semantic | Example Icon Names |
|---------|----------|-------------------|
| Create | Plus | `Add`, `Plus`, `PlusCircle` |
| Edit | Pencil | `Edit`, `Pencil`, `Create` |
| Delete | Trash | `Delete`, `Trash`, `Remove` |
| Send | Arrow outbound | `Send`, `PaperPlane`, `ArrowUpRight` |
| Document / PDF | File | `Document`, `File`, `FileText` |
| Client | Person | `Person`, `User`, `Contact` |
| Settings | Gear | `Settings`, `Gear`, `Cog` |
| Catalog | Book | `Library`, `Book`, `Grid` |
| Search | Magnifying glass | `Search`, `Magnifier` |
| Back | Left chevron | `ChevronLeft`, `ArrowLeft` |
| Close | X mark | `Close`, `X`, `Dismiss` |
| Download | Down arrow | `Download`, `ArrowDown` |
| Offline | Cloud slash | `CloudOff`, `CloudSlash` |
| Check | Checkmark | `Check`, `Checkmark` |
| Warning | Triangle exclamation | `Warning`, `Alert` |
