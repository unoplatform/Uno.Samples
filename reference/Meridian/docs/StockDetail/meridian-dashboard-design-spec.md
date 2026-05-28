# Design Specification: Meridian Terminal v4.0 — Stock Detail Dashboard (AAPL)

| Field | Value |
|-------|-------|
| **Document Type** | Design Specification |
| **Source Files** | `meridian-dashboard-full.png`, `meridian-iframe.png`, `meridian-header-chart.png` |
| **Date** | 2026-03-25 |
| **Status** | Draft — review open questions before implementation |
| **Viewport/Canvas** | ~1456 x 1580px (full page), visible viewport ~1456 x 800px |
| **Scale Factor** | 1x (CSS pixels) |
| **Platform Target** | Web — Desktop-first financial dashboard |

> This document is a pixel-accurate design specification intended for direct implementation.
> All measurements are in logical pixels (px) unless otherwise noted. Values prefixed with `~` are estimates.

---

## 1. High-Level Intent

### Product Goal
Provide a professional, information-dense stock detail view within a portfolio management terminal, enabling users to evaluate holdings, review market data, and execute trades in a single view.

### User Goal
Assess the current state of a specific stock holding (AAPL), review price history, financials, analyst sentiment, and related news — then decide whether to trade, hold, or sell.

### Screen Purpose
A stock detail page aggregating price chart, key statistics, financial data, portfolio position, analyst ratings, and news for a single equity (Apple Inc. / AAPL).

### Context
This is a drill-down view from a portfolio dashboard. The user arrives here by clicking a stock ticker from a portfolio overview or watchlist. The "Back to Dashboard" breadcrumb confirms navigation hierarchy. Exit points include returning to dashboard, executing a trade, or viewing related portfolio holdings.

---

## 2. Scope

### Screens Present
| Screen | Description | States Visible |
|--------|-------------|----------------|
| Stock Detail | Full AAPL detail page | Populated/success state with all data loaded |

### States Covered
- [ ] Empty state
- [ ] Loading state
- [ ] Error state
- [x] Success/populated state
- [ ] Partial/degraded state

### States Missing or Assumed
- **Loading/skeleton state**: No skeleton or shimmer shown — needed for chart, financials table, and statistics panel
- **Error state**: No error UI for failed data fetches (price feed down, API timeout)
- **Empty position state**: What if the user has no shares? The "Your Position" card needs an empty/zero-share variant
- **Market closed state**: No indication of pre-market/after-hours pricing or "Market Closed" badge
- **Trade confirmation modal**: TRADE and SELL ALL buttons imply a modal/flow not shown
- **Watchlist toggled state**: Star icon should show filled state when stock is in watchlist
- **News expanded state**: No expanded article view
- **Responsive/mobile states**: No mobile breakpoints shown

### Out of Scope
- Trade execution flow and order book
- Watchlist management
- Portfolio dashboard (parent view)
- User account/settings
- Real-time WebSocket price updates (implied but not shown)

---

## 3. Information Architecture

### Page Hierarchy

```
Stock Detail Page
├── Top Navigation Bar
│   ├── Back to Dashboard (breadcrumb/back link)
│   ├── Ticker Identity (AAPL · Apple Inc.)
│   └── Action Buttons (TRADE, Watchlist)
├── Price Hero Section
│   ├── Current Price ($247.63)
│   ├── Change Badge (+$3.42 / +1.40%)
│   └── Quick Stats Row (Open, High, Low, Vol, Mkt Cap)
├── Main Content (Two-Column Layout)
│   ├── LEFT COLUMN (~73%)
│   │   ├── Price Chart Card
│   │   │   ├── Timeframe Selector (1D–ALL)
│   │   │   ├── Chart Type Toggle (Line/Area)
│   │   │   ├── Area Chart (SVG)
│   │   │   └── Volume Bar Chart
│   │   ├── About Company Card
│   │   │   ├── Description (truncated + "Read more")
│   │   │   ├── Key Facts Grid (Sector, Industry, Founded, CEO, Employees)
│   │   │   └── External Links (Website, SEC Filings)
│   │   ├── Your Position Card
│   │   │   ├── Share Count + Avg Cost
│   │   │   ├── Metrics Row (Market Value, Unrealized P/L, Return %)
│   │   │   ├── Portfolio Weight Bar
│   │   │   └── Action Buttons (TRADE, SELL ALL)
│   │   └── Financials Card
│   │       ├── Period Toggle (Annual/Quarterly)
│   │       ├── Data Table (Revenue, Net Income, EPS × 4 years)
│   │       └── Mini Bar Chart (Revenue by Year)
│   └── RIGHT COLUMN (~27%)
│       ├── Statistics Card (12 key-value rows)
│       ├── Analyst Ratings Card
│       │   ├── Stacked Horizontal Bar
│       │   ├── Buy/Hold/Sell Counts
│       │   └── Price Target Range
│       ├── News Card (4 headlines with category badges)
│       └── Also in Portfolio Card (3 related tickers)
└── Footer Bar
    ├── Back to Dashboard
    └── "Meridian Terminal v4.0" branding
```

### Grouping Logic
Content is grouped by decision-making relevance:
1. **Identification + Action** (top bar): What stock, can I trade?
2. **Price Assessment** (hero): How is it doing right now?
3. **Historical Analysis** (chart): What's the trend?
4. **Company Context** (about): What does the company do?
5. **Personal Stake** (position): What do I own and how has it performed?
6. **Fundamental Analysis** (financials + statistics): Is it valued well?
7. **Sentiment** (analyst ratings + news): What does the market think?
8. **Related Context** (portfolio): What else do I own?

### Navigation Model
- **Breadcrumb back-link**: `< Back to Dashboard` — stack-based navigation
- **Footer mirror**: Duplicate back link in footer for long-scroll convenience
- **In-page navigation**: None (single scrollable page, no tabs or anchors)
- **Cross-navigation**: "Also in Portfolio" links to other stock detail pages

---

## 4. User Flows

### Entry Points
1. Click a stock ticker from the portfolio dashboard
2. Click a stock from a watchlist
3. Direct URL navigation (`/stock/AAPL`)

### Primary Flow (Happy Path)
1. User arrives from dashboard, sees current price and daily change
2. Scans chart for recent price trend, toggles timeframe as needed
3. Reviews position: unrealized gain, portfolio weight
4. Checks analyst consensus and price targets
5. Scans news headlines for recent catalysts
6. Decides to hold, trade, or add to watchlist
7. Clicks TRADE → (modal/flow not shown)

### Alternate Paths
- **Research path**: User reads About section, checks financials, reviews statistics before any trade decision
- **Quick glance**: User checks price + change, scans chart, returns to dashboard
- **News-driven**: User comes specifically to check news after alert

### Error/Edge Paths
- Price feed stale (no real-time update)
- User has zero shares (position card changes)
- Market hours vs. after-hours display

### Exit Points
- "Back to Dashboard" (top or footer)
- Click TRADE → trade execution flow
- Click related ticker (MSFT, GOOGL, META) → different stock detail page
- External links (Website, SEC Filings)

---

## 5. Component Inventory

### Components
| Component | Variants | Count | Notes |
|-----------|----------|-------|-------|
| Back Link | Default | 2 | Header + footer |
| Ticker Badge | Default | 1 | AAPL with company name |
| Button | Primary (TRADE), Secondary (Watchlist), Ghost (SELL ALL) | 6 | Various contexts |
| Price Display | Hero | 1 | Large format price |
| Change Badge | Positive (green) | 1 | Needs negative (red) variant |
| Quick Stat | Inline | 5 | Open/High/Low/Vol/MktCap |
| Card | Content container | 8 | Consistent rounded container |
| Timeframe Button Group | Segmented toggle | 1 | 9 options (1D–ALL) |
| Chart Type Toggle | Pill toggle | 1 | Line/Area |
| Area Chart | SVG chart | 1 | Interactive with hover |
| Volume Bars | Mini bar chart | 1 | Below main chart |
| Key-Value Row | Statistics style | 12 | In Statistics card |
| Data Table | Financial table | 1 | 4 rows × 3 columns |
| Stacked Bar | Analyst consensus | 1 | Green/gold/red segments |
| Price Target Range | Range indicator | 1 | Min/avg/max with current marker |
| News Item | Card list item | 4 | Badge + headline + timestamp |
| Category Badge | Colored pill | 4 | TECH, EARNINGS, MACRO |
| Portfolio Ticker Row | List item | 3 | Ticker + price + change |
| Portfolio Weight Bar | Progress bar | 1 | Percentage fill |
| Period Toggle | Pill toggle | 1 | Annual/Quarterly |
| Mini Bar Chart | Revenue chart | 1 | 4 bars in financials |
| External Link | Text link with arrow | 2 | Website, SEC Filings |

### Component Details

#### Button — Primary (TRADE)
- **Type**: Custom
- **Variants**: Header TRADE (outlined coral), Position TRADE (outlined neutral), SELL ALL (outlined coral)
- **States**: Default, Hover (implied), Pressed (implied), Focused (not shown), Disabled (not shown)
- **Sizing**:
  - Header: ~90px × ~36px
  - Position: ~330px × ~44px (full-width within half-container)
- **Visual Properties**:
  - Background: transparent
  - Border: ~1.5px solid ~#C2724E (coral/terracotta)
  - Border Radius: ~6px
  - Text: ~13px / 600 (semi-bold) / #C2724E / ALL CAPS / letter-spacing ~1px
  - Padding: ~10px 24px
  - Shadow: none

#### Button — Secondary (Watchlist)
- **Type**: Custom
- **Visual Properties**:
  - Background: transparent
  - Border: ~1px solid ~#D6CFC7 (warm gray)
  - Border Radius: ~6px
  - Text: ~13px / 400 / ~#78716C / includes star icon prefix
  - Padding: ~10px 20px

#### Card Container
- **Type**: Standard content wrapper
- **Visual Properties**:
  - Background: #FFFFFF
  - Border: ~1px solid ~#EBE5DE (very subtle warm border)
  - Border Radius: ~12px
  - Shadow: 0px 1px 3px rgba(0,0,0,~0.04) — extremely subtle
  - Padding: ~24px
  - Min/Max dimensions: Left column cards span ~73% viewport width; right column cards span ~27%

#### Section Header (inside cards)
- **Type**: Typographic element
- **Visual Properties**:
  - Text: ~11px / 600 / ~#A8A29E (warm gray) / ALL CAPS
  - Letter Spacing: ~1.5px
  - Margin Bottom: ~16px

#### Price Hero
- **Type**: Custom display element
- **Visual Properties**:
  - Price text: ~52px / 300 (light) / ~#1C1917
  - Change badge background: ~#E8F5E9 (light green)
  - Change badge text: ~13px / 500 / ~#2D6A4F (dark green)
  - Change badge border-radius: ~20px (full pill)
  - Change badge padding: ~6px 12px
  - "today" label: ~13px / 400 / ~#A8A29E

#### Quick Stat (Inline)
- **Type**: Inline text element
- **Visual Properties**:
  - Label: ~13px / 400 / ~#A8A29E
  - Value: ~13px / 600 / ~#1C1917
  - Separator: `·` in ~#D6CFC7 with ~12px horizontal margin
  - Margin Top from price: ~8px

#### Timeframe Button (in Chart)
- **Type**: Segmented toggle item
- **States**: Default (text only), Active (underlined)
- **Visual Properties**:
  - Default: ~12px / 400 / ~#A8A29E / no background
  - Active (3M): ~12px / 600 / ~#1C1917 / bottom border ~2px solid ~#C2724E (coral underline)
  - Padding: ~8px 6px
  - Gap between items: ~4px

#### Chart Type Toggle (Line/Area)
- **Type**: Pill toggle
- **Visual Properties**:
  - Container: ~#F5F0EB background, border-radius ~6px
  - Active item (Area): #FFFFFF background, border-radius ~4px, shadow subtle
  - Text: ~12px / 500 / ~#1C1917 (active), ~#A8A29E (inactive)
  - Padding: ~6px 12px per item

#### Statistics Key-Value Row
- **Type**: List item
- **Visual Properties**:
  - Container height: ~36px
  - Border bottom: ~1px solid ~#F0EBE5 (subtle divider)
  - Label (left): ~13px / 400 / ~#78716C
  - Value (right): ~13px / 600 / ~#1C1917
  - Padding: ~8px 0

#### Analyst Stacked Bar
- **Type**: Custom visualization
- **Visual Properties**:
  - Height: ~8px
  - Border Radius: ~4px
  - Segments: Buy ~#2D6A4F (green, ~74%), Hold ~#C9A94E (gold, ~21%), Sell ~#C2724E (red, ~5%)
  - Numbers below: Buy "28" in green, Hold "8" in gold, Sell "2" in red
  - Font: ~20px / 700 for numbers, ~11px / 400 / ALL CAPS for labels

#### News Item
- **Type**: List card item
- **Visual Properties**:
  - Category badge: ~10px / 600 / ALL CAPS / letter-spacing ~1px
    - TECH: ~#2D6A4F text on ~#E8F5E9 background
    - EARNINGS: ~#8B6914 text on ~#FEF3C7 background
    - MACRO: ~#78542B text on ~#F5E6D3 background
  - Badge border-radius: ~4px, padding ~3px 8px
  - Headline: ~13px / 500 / ~#1C1917 / line-height ~1.4
  - Timestamp: ~11px / 400 / ~#A8A29E
  - Item padding: ~12px 0
  - Divider: implied spacing between items

#### Portfolio Ticker Row
- **Type**: Interactive list item
- **Visual Properties**:
  - Container: cursor pointer, ~padding 12px 0
  - Ticker symbol: ~14px / 700 / ~#1C1917
  - Company name: ~12px / 400 / ~#A8A29E
  - Price: ~14px / 600 / ~#1C1917 (right-aligned)
  - Change %: ~12px / 500 / ~#2D6A4F (green, right-aligned)
  - Divider between rows: ~1px solid ~#F0EBE5

#### Position Metric Box
- **Type**: Stat card (inline)
- **Visual Properties**:
  - Container: ~1px solid ~#EBE5DE, border-radius ~8px
  - Label: ~10px / 600 / ALL CAPS / ~#A8A29E / letter-spacing ~1px
  - Value: ~24px / 600 / ~#1C1917 (Market Value), ~#2D6A4F (Unrealized, Return — green for positive)
  - Padding: ~16px
  - Width: ~33% each (3-column equal split)

#### Portfolio Weight Bar
- **Type**: Progress indicator
- **Visual Properties**:
  - Track: ~4px height, ~#EBE5DE background, full width, border-radius ~2px
  - Fill: ~4px height, ~#2D6A4F (green), width proportional to 12.8%
  - Label (left): ~12px / 400 / ~#78716C ("Weight in portfolio")
  - Value (right): ~12px / 600 / ~#1C1917 ("12.8%")

#### Financial Table
- **Type**: Data table
- **Visual Properties**:
  - Header row: ~10px / 600 / ALL CAPS / ~#A8A29E / letter-spacing ~1px
  - Year column (left-aligned): ~14px / 500 / ~#1C1917
  - Data cells (right-aligned): ~14px / 400 / ~#1C1917
  - Row height: ~36px
  - Row divider: ~1px solid ~#F0EBE5
  - Left-aligned text, right-aligned numbers ✓ (follows table rules)

#### Price Target Range
- **Type**: Custom range indicator
- **Visual Properties**:
  - Track: ~3px height, gradient from ~#2D6A4F (left/low) through ~#C9A94E (mid) to ~#C2724E (right/high)
  - Current marker: ~8px diameter circle, ~#2D6A4F, positioned proportionally
  - Labels: "Low"/"Average"/"High" at ~10px / 400 / ~#A8A29E
  - Values: ~16px / 700 / ~#1C1917 ($210, $265, $310)
  - "Current: $247.63": ~12px / 500 / ~#1C1917, centered below marker

---

## 6. Layout + Spacing

### Grid System
- **Columns**: Implied 12-column grid; left content area ~9 columns, right sidebar ~3 columns
- **Gutter**: ~20px between left and right columns
- **Margins**: ~48px left/right page margins (within the iframe content area)
- **Max width**: ~1280px content area, centered on page

### Spacing Scale
Identified spacing tokens (4-point baseline system):

| Token | Value | Usage |
|-------|-------|-------|
| `xs` | 4px | Icon-text micro gap |
| `sm` | 8px | Inline stat separators, tight label gaps |
| `md` | 12px | News item internal gaps, badge padding |
| `base` | 16px | Card internal element gaps, section header to content |
| `lg` | 20px | Column gutter, inter-card vertical gap |
| `xl` | 24px | Card padding (all sides) |
| `2xl` | 32px | Hero section top/bottom padding |
| `3xl` | 48px | Page horizontal margins |

### Baseline Unit Compliance
The design uses a **4-point baseline system** with generally good compliance. The spacing values observed (4, 8, 12, 16, 20, 24, 32, 48) are all multiples of 4.

### Key Measurements
| Element | Property | Value | Baseline Compliant? |
|---------|----------|-------|---------------------|
| Page margin (horizontal) | padding | ~48px | Yes (12×4) |
| Card padding | padding | ~24px | Yes (6×4) |
| Card border-radius | border-radius | ~12px | Yes (3×4) |
| Card-to-card vertical gap | margin-bottom | ~20px | Yes (5×4) |
| Left-right column gap | gap | ~20px | Yes (5×4) |
| Section header to content | margin-bottom | ~16px | Yes (4×4) |
| Hero price to quick stats | margin-top | ~8px | Yes (2×4) |
| Quick stats to chart card | margin-top | ~24px | Yes (6×4) |
| Stat row height | height | ~36px | Yes (9×4) |
| Position metric box padding | padding | ~16px | Yes (4×4) |
| News item vertical padding | padding | ~12px | Yes (3×4) |
| Button vertical padding | padding | ~10px | No — ~should be 8 or 12 |
| Footer height | height | ~48px | Yes (12×4) |

### Spacing Relationships
| Relationship | Spacing A | Spacing B | Ratio | Consistent? |
|-------------|-----------|-----------|-------|-------------|
| Page margin vs Card padding | 48px | 24px | 2:1 | Yes |
| Card padding vs Element gap | 24px | 16px | 1.5:1 | Yes |
| Section gap vs Card-to-card gap | 24px | 20px | ~1.2:1 | Acceptable |
| Card padding vs Stat row gap | 24px | 8px | 3:1 | Yes |
| Column gap vs Card padding | 20px | 24px | ~0.83:1 | Slightly tight — column gap should match or exceed card padding |

### Layout Proportions
| Region A | Region B | Width Ratio | Closest Standard Ratio |
|----------|----------|-------------|----------------------|
| Left column | Right column | ~73:27 | ~3:1 (close to golden section ~72:28) |
| Chart card | Sidebar stats | ~73:27 | Same as above |
| Header back-link area | Action buttons area | ~70:30 | Standard nav pattern |

**Content-to-Whitespace Ratio**: ~65% content / ~35% whitespace — well within the ideal 60-70% content range. The warm cream background provides breathing room without feeling empty.

**Above-the-fold Content**: ~60% of critical content is above the fold (price hero, chart, beginning of statistics). Price, change, chart, and statistics are all visible without scrolling. Position card and financials require scroll.

**Aspect Ratios Detected**:
- Chart card: ~16:9 (including controls)
- Chart area only: ~2.5:1 (wide landscape)
- Position metric boxes: ~3:1 (landscape rectangles)
- News card: ~1:2.5 (tall portrait)
- Statistics card: ~1:3.5 (very tall portrait)
- Mini bar chart bars: ~1:2 each

### ASCII Layout Map

```
┌══════════════════════════════════════════════════════════════════════════════════════════════┐
│ ← ~48px →┌─────────────────────────────────────────────────────────────────────┐← ~48px → │
│           │ TOP NAV BAR                                                ~48px h │           │
│           │ (←) Back to Dashboard   AAPL · Apple Inc.    [TRADE] [☆ Watchlist]│           │
│           └─────────────────────────────────────────────────────────────────────┘           │
│                                          ↕ ~24px                                           │
│           ┌─────────────────────────────────────────────────────────────────────┐           │
│           │ PRICE HERO                                                 ~80px h │           │
│           │ $247.63          ┌──────────────────┐  today                       │           │
│           │   ~52px light    │▲ +$3.42 (+1.40%) │                              │           │
│           │                  └──────────────────┘                              │           │
│           │ Open $244.88 · High $251.2 · Low $244.18 · Vol 62.1M · MktCap 3.82T          │
│           └─────────────────────────────────────────────────────────────────────┘           │
│                                          ↕ ~24px                                           │
│           ┌──────────────────────────────────────────────┐ ←20px→ ┌────────────────────┐   │
│           │ CHART CARD                            ~380px │        │ STATISTICS         │   │
│           │ ┌──┬──┬──┬━━┬──┬───┬──┬──┬───┐  [Line|Area] │        │ ┌────────────────┐ │   │
│           │ │1D│1W│1M│3M│6M│YTD│1Y│5Y│ALL│               │        │ │Prev Close  $244│ │   │
│           │ └──┴──┴──┴━━┴──┴───┴──┴──┴───┘               │        │ │Open        $244│ │   │
│           │  $280─┐                                       │        │ │Day Range   $244│ │   │
│           │  $260─┤         ╱──╲    ╱──────╲              │        │ │52-Wk Range $189│ │   │
│           │  $240─┤    ╱──╲╱    ╲──╱        ╲──────       │        │ │Volume      62.1│ │   │
│           │  $220─┤╱──╱                          ▓▓▓     │        │ │Avg Volume  58.4│ │   │
│           │  $200─┤                                       │        │ │Market Cap  3.82│ │   │
│           │       Jan1  Jan16  Jan31  Feb15  Mar2  Mar17  │        │ │P/E (TTM)  31.2│ │   │
│           │  ▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐▐  Vol bars│        │ │EPS (TTM)  $7.94│ │   │
│           └──────────────────────────────────────────────┘        │ │Div Yield  0.44%│ │   │
│                          ↕ ~20px                                  │ │Beta        1.24│ │   │
│           ┌──────────────────────────────────────────────┐        │ │52-Wk Chg +28.4%│ │   │
│           │ ABOUT APPLE INC.                             │        │ └────────────────┘ │   │
│           │ Apple Inc. designs, manufactures, and        │        └────────────────────┘   │
│           │ markets smartphones, personal computers...   │                ↕ ~20px          │
│           │ Read more                                    │        ┌────────────────────┐   │
│           │ ──────────────────────────────────────        │        │ ANALYST RATINGS    │   │
│           │ Sector      Technology                       │        │ ┌─────────────────┐│   │
│           │ Industry    Consumer Electronics             │        │ │▓▓▓▓▓▓▓▓▓▓░░░░▒▒││   │
│           │ Founded     1976 · Cupertino, CA             │        │ └─────────────────┘│   │
│           │ CEO         Tim Cook                         │        │ 28 Buy  8 Hold  2 S│   │
│           │ Employees   164,000                          │        │                    │   │
│           │ Website ↗  SEC Filings ↗                    │        │ PRICE TARGET       │   │
│           └──────────────────────────────────────────────┘        │ Low    Avg    High │   │
│                          ↕ ~20px                                  │ $210   $265   $310 │   │
│           ┌──────────────────────────────────────────────┐        │ ─●────────────────│   │
│           │ YOUR POSITION                                │        │   Current: $247.63 │   │
│           │ 85 shares · Avg $178.40                      │        └────────────────────┘   │
│           │ ┌──────────┬───────────┬──────────┐          │                ↕ ~20px          │
│           │ │MKT VALUE │UNREALIZED │ RETURN   │          │        ┌────────────────────┐   │
│           │ │ $21,049  │+$5,884.55 │ +38.8%   │          │        │ NEWS               │   │
│           │ └──────────┴───────────┴──────────┘          │        │ ┌TECH┐ Apple AI..  │   │
│           │ Weight in portfolio ▓▓▓░░░░░░░░░░░░░  12.8%  │        │        1h ago      │   │
│           │ [        TRADE        ] [      SELL ALL     ] │        │ ┌EARN┐ Apple Q1.. │   │
│           └──────────────────────────────────────────────┘        │        3h ago      │   │
│                          ↕ ~20px                                  │ ┌MACRO┐ Fed sig.. │   │
│           ┌──────────────────────────────────────────────┐        │        5h ago      │   │
│           │ FINANCIALS                  [Annual|Quarter] │        │ ┌TECH┐ iPhone 17..│   │
│           │           REVENUE  NET INCOME  EPS           │        │        Yesterday   │   │
│           │ 2024      $394.3B   $101.2B   $6.42          │        └────────────────────┘   │
│           │ 2023      $383.3B    $97.0B   $6.16          │                ↕ ~20px          │
│           │ 2022      $394.3B    $99.8B   $6.11          │        ┌────────────────────┐   │
│           │ 2021      $365.8B    $94.7B   $5.61          │        │ ALSO IN PORTFOLIO  │   │
│           │  ┌─┐  ┌─┐  ┌─┐  ┌─┐                         │        │ MSFT    $468.21    │   │
│           │  │█│  │█│  │█│  │█│  Revenue mini bars       │        │ Microsoft  +1.12%  │   │
│           │  └─┘  └─┘  └─┘  └─┘                         │        │ GOOGL   $186.34    │   │
│           │ 2021 2022 2023 2024                           │        │ Alphabet  +1.15%   │   │
│           └──────────────────────────────────────────────┘        │ META    $612.50    │   │
│                          ↕ ~24px                                  │ Meta Plat  +1.45%  │   │
│           ┌─────────────────────────────────────────────────────────────────────────────┐  │
│           │ FOOTER                                                            ~48px h  │  │
│           │ (←) Back to Dashboard                         Meridian Terminal v4.0       │  │
│           └─────────────────────────────────────────────────────────────────────────────┘  │
└══════════════════════════════════════════════════════════════════════════════════════════════┘
```

### Responsiveness
No responsive breakpoints shown. Recommendations based on design rules:
- **< 1024px**: Stack right sidebar below left column (single column)
- **< 768px**:
  - Price hero: reduce from ~52px to ~36px
  - Body text: reduce by 25% (~14px → ~11px)
  - Financial table: transform to card layout
  - Position metric boxes: stack vertically
  - Page margins: reduce to ~16px
- **< 480px**:
  - Chart timeframe buttons: horizontal scroll
  - Headers reduce 50%
  - Quick stats: wrap to 2 lines

---

## 7. Typography System

### Font Families
| Role | Family | Type | Fallback |
|------|--------|------|----------|
| Headers/Display | ~Georgia or serif system | Serif | Times New Roman, serif |
| Body/UI | ~Inter or system sans-serif | Sans-serif | -apple-system, Segoe UI, sans-serif |
| Numbers/Data | Tabular lining figures (same as body) | Sans-serif | Monospace fallback for alignment |

> **Note**: The price display ($247.63) appears to use a light-weight serif or transitional typeface, giving the dashboard a financial/editorial tone. Body text and labels use a clean sans-serif.

### Type Scale
| Style Name | Size | Weight | Line Height | Letter Spacing | Usage | Modular Scale? |
|------------|------|--------|-------------|----------------|-------|----------------|
| Display Price | ~52px | 300 | ~1.1 | -0.02em | Hero price | Yes — H1 equivalent |
| Metric Large | ~24px | 600 | ~1.2 | 0 | Position values ($21,049) | Yes — H3 equivalent |
| Heading Card | ~20px | 700 | ~1.25 | 0 | Analyst numbers (28, 8, 2) | Yes — H4 equivalent |
| Ticker Symbol | ~18px | 700 | ~1.3 | 0 | AAPL in header | Yes — H5 equivalent |
| Price Target | ~16px | 700 | ~1.3 | 0 | $210, $265, $310 | Yes — H5/Body transition |
| Body | ~14px | 400 | ~1.5 | 0 | Descriptions, stat values | Yes — Base |
| Body Strong | ~14px | 600 | ~1.5 | 0 | Stat values, prices | Yes — Base bold |
| Label/Caption | ~13px | 400-500 | ~1.4 | 0 | Stat labels, quick stats | Yes |
| Section Header | ~11px | 600 | ~1.3 | ~1.5px | Card titles (ALL CAPS) | Yes — Small |
| Micro | ~10px | 600 | ~1.3 | ~1.5px | Column headers, badge labels (ALL CAPS) | Yes — Micro |

### Typography Rules Audit
- [x] Sans-serif used for body/labels/small text
- [x] Serif-like treatment used for hero price (large display) — appropriate
- [x] Bold used sparingly — only metric values and tickers
- [x] No italic on buttons/labels
- [x] ALL CAPS used correctly for section headers, labels, badges, buttons — never for sentences
- [ ] Letter spacing reduced for large text (>64px) — N/A, no text exceeds 64px
- [x] Line height: headers ~1.1-1.25, body ~1.5 ✓
- [x] Font sizes follow a clear system (~10, 11, 13, 14, 16, 18, 20, 24, 52)
- [x] Weight increases for small text (600 for 10-11px labels), decreases for large (300 for 52px price)
- [x] Clear hierarchy: size + weight + color + position all reinforce importance

---

## 8. Color + Theming

### Color Palette
| Token Name | Hex | Weight | Usage |
|------------|-----|--------|-------|
| Base-900 | ~#1C1917 | 900 | Primary text, prices, headings |
| Base-700 | ~#44403C | 700 | Strong secondary text |
| Base-500 | ~#78716C | 500 | Tertiary text, stat labels |
| Base-400 | ~#A8A29E | 400 | Section headers, timestamps, captions |
| Base-300 | ~#D6CFC7 | 300 | Borders, separators, inactive elements |
| Base-200 | ~#EBE5DE | 200 | Card borders, divider lines, input borders |
| Base-100 | ~#F5F0EB | 100 | Page background, toggle track |
| Base-50 | ~#FAFAF8 | 50 | Subtle background variations |
| White | #FFFFFF | — | Card backgrounds |
| Green-700 | ~#2D6A4F | 700 | Chart line, positive values, buy rating, portfolio weight bar |
| Green-100 | ~#E8F5E9 | 100 | Change badge background, TECH badge background |
| Coral-600 | ~#C2724E | 600 | TRADE button, sell rating, active timeframe underline |
| Coral-100 | ~#FDF2ED | 100 | SELL ALL hover background (implied) |
| Gold-600 | ~#C9A94E | 600 | Hold rating bar |
| Gold-700 | ~#8B6914 | 700 | EARNINGS badge text |
| Gold-100 | ~#FEF3C7 | 100 | EARNINGS badge background |
| Brown-600 | ~#78542B | 600 | MACRO badge text |
| Brown-100 | ~#F5E6D3 | 100 | MACRO badge background |
| Red-600 | ~#C2724E | 600 | Sell rating number (same as coral) |

### Opacity Map
| Element | Opacity | Purpose | Matches Rule? |
|---------|---------|---------|---------------|
| Primary text (prices, headings) | 100% | Full emphasis | Yes (87-100%) |
| Secondary text (stat labels) | ~65% effective | Medium emphasis | Yes (~54-60% via color) |
| Tertiary text (timestamps, "today") | ~45% effective | Low emphasis | Slightly below 54% minimum |
| Section headers (ALL CAPS) | ~55% effective | Label de-emphasis | Yes (54-60%) |
| Card borders | ~100% at low-contrast color | Subtle separation | Achieved via color not opacity |
| Table row dividers | ~100% at ~#F0EBE5 | Row separation | Achieved via color (~12-20% contrast) |
| Chart area fill | ~15-20% | Area chart fill under line | Yes (subtle fill) |
| Volume bars | ~80% | Secondary chart data | Reasonable |
| Placeholder text (implied) | ~50% | Hint content | Assumed — not visible in current state |

### Gradients
| Element | Type | Direction/Angle | Color Stops | Notes |
|---------|------|-----------------|-------------|-------|
| Price Target track | Linear | 0deg (left→right) | ~#2D6A4F 0%, ~#C9A94E 50%, ~#C2724E 100% | Semantic gradient: low(green) → avg(gold) → high(red) |
| Chart area fill | Linear | 180deg (top→bottom) | ~#2D6A4F at ~15% opacity 0%, transparent 100% | Subtle fade from line to baseline |

**Gradient Consistency**: Minimal use of gradients. The price target bar gradient is semantic (maps to buy/hold/sell scale). The chart area fill is a standard area-chart pattern. Both are subtle and purposeful.

### Color Harmony Analysis
The palette is **warm monochromatic** with strategic accent colors:
- **Base palette**: Warm stone/sand tones (brown-gray undertone) — creates an elegant, editorial feel
- **Accent green**: Desaturated forest green — calm, trustworthy for financial data
- **Accent coral**: Terracotta — warm, attention-grabbing without being alarming (softer than pure red)
- **Accent gold**: Muted amber — neutral/cautionary without yellow's harshness

The color harmony is **analogous** (warm browns → golds → corals on the warm side of the wheel) with **complementary** green as the primary accent. This is sophisticated and intentional.

### Color Psychology Check
| Color | Used For | Emotional Match | Correct? |
|-------|----------|-----------------|----------|
| Green (#2D6A4F) | Positive gains, Buy rating, chart | Success, growth, money | Yes ✓ |
| Coral (#C2724E) | Trade action, Sell rating | Urgency, action (softer than red) | Mostly ✓ — TRADE button uses "danger" color which could imply risk |
| Gold (#C9A94E) | Hold rating | Caution, neutral | Yes ✓ |
| Warm grays | UI chrome, labels | Sophisticated, calm, trustworthy | Yes ✓ |
| White | Cards | Clean, focused | Yes ✓ |
| Cream (#F5F0EB) | Page background | Warm, premium, editorial | Yes ✓ — differentiates from cold fintech blue |

**Potential issue**: The TRADE (buy more) button uses the same coral as the Sell rating. This creates a semantic conflict — buying and selling share the same action color. The primary TRADE CTA could use the green to align with "buy = positive."

### Semantic Mapping
| Semantic Role | Color | Correct Usage? |
|---------------|-------|----------------|
| Positive/Gain | Green ~#2D6A4F | Yes — used for price increase, return, buy |
| Negative/Loss | Coral ~#C2724E (implied) | Partially — not visible in current state (stock is up) |
| Warning/Neutral | Gold ~#C9A94E | Yes — hold rating |
| Action/CTA | Coral ~#C2724E | Questionable — see note above |
| Info | Not explicitly present | Missing — no blue info color |
| Disabled | Not shown | Missing — need disabled state colors |

### Color Weight Scale
The base palette follows a clear weight scale (50–900) with warm undertones. Accent colors (green, coral, gold) have implied 100/600-700 pairs for background/foreground usage. A full 100–800 scale for each accent would strengthen the system.

### Dark/Light Mode
Only **light mode** is shown. The warm cream (#F5F0EB) background suggests this is the intended primary mode. For dark mode:
- Base-900 → Background
- Base-100 → Primary text
- White → Dark card surface (~#2A2523)
- Green/coral/gold accents should maintain contrast ratios

### Contrast Audit
| Element | FG | BG | Ratio (est.) | WCAG AA (4.5:1) | Pass/Fail |
|---------|----|----|-------------|-----------------|-----------|
| Primary text on white card | #1C1917 | #FFFFFF | ~18:1 | AA | Pass ✓ |
| Primary text on cream bg | #1C1917 | #F5F0EB | ~14:1 | AA | Pass ✓ |
| Section header on white | #A8A29E | #FFFFFF | ~3.2:1 | AA | **Fail** ✗ |
| Stat label on white | #78716C | #FFFFFF | ~4.1:1 | AA | **Borderline** ⚠ |
| Timestamp text | #A8A29E | #FFFFFF | ~3.2:1 | AA | **Fail** ✗ |
| Green change badge text | #2D6A4F | #E8F5E9 | ~5.5:1 | AA | Pass ✓ |
| Coral button text on white | #C2724E | #FFFFFF | ~3.8:1 | AA | **Fail** ✗ |
| Gold hold number | #C9A94E | #FFFFFF | ~2.5:1 | AA | **Fail** ✗ |
| Chart line on white | #2D6A4F | #FFFFFF | ~7.5:1 | AA | Pass ✓ |
| Quick stat value on cream | #1C1917 | #F5F0EB | ~14:1 | AA | Pass ✓ |

**Critical contrast issues**:
1. Section headers (#A8A29E) fail WCAG AA at ~3.2:1
2. TRADE button coral text (#C2724E) fails at ~3.8:1
3. Gold "Hold" number (#C9A94E) severely fails at ~2.5:1
4. Timestamp/caption text fails at ~3.2:1

---

## 9. Interaction Design

### State Map
| Element | Default | Hover | Pressed | Focused | Disabled |
|---------|---------|-------|---------|---------|----------|
| TRADE button | Coral outline | Not shown | Not shown | Not shown | Not shown |
| Watchlist button | Gray outline | Not shown | Not shown | Not shown | Not shown |
| SELL ALL button | Coral outline | Not shown | Not shown | Not shown | Not shown |
| Timeframe button | Gray text | Not shown | Not shown | Not shown | N/A |
| Timeframe active | Dark text + underline | — | — | — | N/A |
| Chart type toggle | Active: white bg | Not shown | Not shown | Not shown | N/A |
| Annual/Quarterly toggle | Active: white bg/border | Not shown | Not shown | Not shown | N/A |
| News item | Default | Cursor pointer (shown) | Not shown | Not shown | N/A |
| Portfolio ticker | Default | Cursor pointer (shown) | Not shown | Not shown | N/A |
| Read more link | Default | Not shown | Not shown | Not shown | N/A |
| External links | Default | Not shown | Not shown | Not shown | N/A |

### Detailed Interaction State Specifications

#### TRADE Button (Primary CTA) — State Detail
| Property | Default | Hover (suggested) | Pressed (suggested) | Focused (suggested) | Disabled (suggested) |
|----------|---------|---------|---------|---------|----------|
| Background | transparent | ~#C2724E at 8% | ~#C2724E at 14% | transparent | transparent |
| Border | 1.5px #C2724E | 1.5px #A85A38 | 1.5px #A85A38 | 2px #C2724E | 1.5px #C2724E at 38% |
| Text Color | #C2724E | #A85A38 | #8B4530 | #C2724E | #C2724E at 38% |
| Shadow | none | 0 2px 4px rgba(194,114,78,0.15) | none | 0 0 0 3px rgba(194,114,78,0.2) | none |
| Opacity | 100% | 100% | 100% | 100% | 38% |
| Transform | none | none | scale(0.98) | none | none |
| Cursor | pointer | pointer | pointer | — | not-allowed |
| Transition | — | 150ms ease | 50ms ease | 150ms ease | none |

#### Timeframe Button — State Detail
| Property | Default | Hover | Active |
|----------|---------|-------|--------|
| Background | transparent | ~#F5F0EB at 50% | transparent |
| Border Bottom | none | none | 2px solid #C2724E |
| Text Color | #A8A29E | #78716C | #1C1917 |
| Font Weight | 400 | 400 | 600 |
| Cursor | pointer | pointer | default |

#### News Item — State Detail
| Property | Default | Hover |
|----------|---------|-------|
| Background | transparent | ~#F5F0EB at 30% |
| Cursor | default | pointer |
| Headline color | #1C1917 | #1C1917 (underline implied) |

#### Portfolio Ticker Row — State Detail
| Property | Default | Hover |
|----------|---------|-------|
| Background | transparent | ~#F5F0EB at 30% |
| Cursor | default | pointer |

### Interaction Rules Audit
- [ ] Input hover: only 5-15% shade/tint change — **No inputs present; N/A**
- [x] Inputs visually distinct from buttons — N/A (no form inputs)
- [ ] Submit button shows loading spinner on click — **Not shown; needed for TRADE/SELL ALL**
- [ ] Toggle takes immediate effect — **Implied for Annual/Quarterly and chart toggles**
- [ ] Focus states use box-shadow — **Not shown; needs implementation**
- [ ] Valid = green, Invalid = red + helper text — N/A (no form validation)
- [ ] Disabled states use 38% opacity — **Not shown; needs definition**
- [ ] Hover backgrounds use 4-8% opacity fill — **Not shown; needs definition**
- [ ] Pressed backgrounds use 12-16% opacity fill — **Not shown; needs definition**
- [ ] All interactive elements have visible focus indicators — **Not shown; critical a11y gap**

### Transitions + Animation
| Trigger | Element | Animation (suggested) | Duration | Easing | Delay |
|---------|---------|-----------|----------|--------|-------|
| Page load | Cards | Fade in + slide up 8px | 300ms | ease-out | Staggered 50ms per card |
| Page load | Price | Counter animation (0 → $247.63) | 600ms | ease-out | 100ms |
| Page load | Chart | Draw line left to right | 800ms | ease-in-out | 200ms |
| Timeframe change | Chart | Crossfade + redraw | 400ms | ease-in-out | 0ms |
| Chart type change | Chart area | Morph line ↔ area fill | 300ms | ease | 0ms |
| Toggle switch | Active pill | Slide + background transition | 200ms | ease | 0ms |
| Hover | Card | Subtle shadow increase | 150ms | ease | 0ms |
| Read more click | Description | Expand height | 300ms | ease-in-out | 0ms |

### Gestures
| Gesture | Element | Action | Notes |
|---------|---------|--------|-------|
| Hover + move | Chart | Tooltip with price at cursor position | Implied by interactive chart |
| Click + drag | Chart | Range selection / zoom (implied) | Common in financial charts |

### Micro-interactions
| Element | Trigger | Visual Change | Duration | Notes |
|---------|---------|---------------|----------|-------|
| Change badge | Price update | Flash/pulse green on positive, red on negative | 300ms | For real-time updates |
| Watchlist star | Click | Fill animation (outline → solid) | 200ms | Toggle state |
| Portfolio weight bar | Page load | Animate fill from 0 → 12.8% | 500ms | Draw attention |
| Volume bars | Chart timeframe change | Bars grow from baseline | 300ms staggered | Per-bar delay |

---

## 10. Content + Copy

### All Visible Text
| Location | Text | Type | Capitalization | Notes |
|----------|------|------|----------------|-------|
| Header | "Back to Dashboard" | Navigation link | Sentence case | With left chevron icon |
| Header | "AAPL" | Ticker badge | ALL CAPS | Inline with company name |
| Header | "Apple Inc." | Company name | Title case | Secondary to ticker |
| Header | "TRADE" | Button CTA | ALL CAPS | Primary action |
| Header | "☆ Watchlist" | Button label | Title case | With star icon prefix |
| Hero | "$247.63" | Price display | N/A | Currency format |
| Hero | "+$3.42 (+1.40%)" | Change badge | N/A | With up arrow icon |
| Hero | "today" | Time label | lowercase | Relative time reference |
| Quick stats | "Open $244.88" | Stat | Title case label | Dot-separated |
| Quick stats | "High $251.2" | Stat | Title case label | — |
| Quick stats | "Low $244.18" | Stat | Title case label | — |
| Quick stats | "Vol 62.1M" | Stat | Title case label | Abbreviated |
| Quick stats | "Mkt Cap 3.82T" | Stat | Title case label | Abbreviated |
| Chart | "1D", "1W", "1M", "3M", etc. | Toggle labels | ALL CAPS | Timeframe abbreviations |
| Chart | "Line", "Area" | Toggle labels | Title case | Chart type |
| Chart | "$200"–"$280" | Y-axis labels | N/A | Currency |
| Chart | "Jan 1"–"Mar 17" | X-axis labels | Title case | Date format |
| About | "ABOUT APPLE INC." | Section header | ALL CAPS | — |
| About | Description paragraph | Body text | Sentence case | Truncated with "Read more" |
| About | "Sector", "Industry", etc. | Key-value labels | Title case | — |
| About | "Technology", "Consumer Electronics", etc. | Key-value values | Title case | — |
| About | "Website ↗", "SEC Filings ↗" | External links | Title case | With arrow icon |
| Position | "YOUR POSITION" | Section header | ALL CAPS | — |
| Position | "85 shares" | Stat | lowercase "shares" | Bold number |
| Position | "Avg $178.40" | Stat | Title case "Avg" | — |
| Position | "MARKET VALUE", "UNREALIZED", "RETURN" | Metric labels | ALL CAPS | In metric boxes |
| Position | "$21,049", "+$5,884.55", "+38.8%" | Metric values | N/A | Currency/percent |
| Position | "Weight in portfolio" | Label | Sentence case | Progress bar label |
| Position | "12.8%" | Value | N/A | — |
| Position | "TRADE", "SELL ALL" | Button CTAs | ALL CAPS | Dual action buttons |
| Financials | "FINANCIALS" | Section header | ALL CAPS | — |
| Financials | "Annual", "Quarterly" | Toggle labels | Title case | — |
| Financials | "REVENUE", "NET INCOME", "EPS" | Column headers | ALL CAPS | — |
| Financials | "2024"–"2021" | Row labels | N/A | Year values |
| Financials | "$394.3B", "$101.2B", "$6.42" etc. | Data values | N/A | Currency abbreviated |
| Statistics | "STATISTICS" | Section header | ALL CAPS | — |
| Statistics | "Prev Close", "Open", etc. | Row labels | Title case | 12 key-value pairs |
| Statistics | "$244.21", "62.1M", "31.2" etc. | Row values | N/A | Mixed formats |
| Analyst | "ANALYST RATINGS" | Section header | ALL CAPS | — |
| Analyst | "28", "8", "2" | Rating counts | N/A | Large bold numbers |
| Analyst | "Buy", "Hold", "Sell" | Rating labels | Title case | — |
| Analyst | "PRICE TARGET" | Sub-header | ALL CAPS | — |
| Analyst | "Low", "Average", "High" | Range labels | Title case | — |
| Analyst | "$210", "$265", "$310" | Range values | N/A | — |
| Analyst | "Current: $247.63" | Marker label | Sentence case | — |
| News | "NEWS" | Section header | ALL CAPS | — |
| News | "TECH", "EARNINGS", "MACRO" | Category badges | ALL CAPS | Colored badges |
| News | 4 headline strings | Headlines | Sentence case | Truncatable |
| News | "1h ago", "3h ago", "5h ago", "Yesterday" | Timestamps | lowercase | Relative time |
| Portfolio | "ALSO IN PORTFOLIO" | Section header | ALL CAPS | — |
| Portfolio | "MSFT", "GOOGL", "META" | Ticker symbols | ALL CAPS | — |
| Portfolio | "Microsoft Corp.", "Alphabet Inc.", "Meta Platforms" | Company names | Title case | — |
| Portfolio | "$468.21", "$186.34", "$612.50" | Prices | N/A | — |
| Portfolio | "+1.12%", "+1.15%", "+1.45%" | Change percentages | N/A | All positive/green |
| Footer | "Back to Dashboard" | Navigation link | Sentence case | With left chevron |
| Footer | "Meridian Terminal v4.0" | Branding | Title case | Muted/light text |

### Tone
**Professional-editorial with warmth.** The copy is concise and data-focused, using financial abbreviations (B, T, M) comfortably. Section headers are institutional/terminal-style (ALL CAPS, tracked). The warm color palette and serif price display soften the typical cold fintech tone into something closer to a premium financial publication (think Bloomberg Terminal meets Barron's editorial design).

### Copy Quality Audit
- [x] Button labels are descriptive actions ("TRADE", "SELL ALL", not "Submit")
- [x] Labels use clear naming ("Weight in portfolio", not jargon)
- [ ] Placeholder text at reduced opacity — not applicable (no inputs shown)
- [x] ALL CAPS not used for full sentences — only labels, headers, badges
- [ ] Error messages near the relevant element — no errors shown

### Placeholder/Dynamic Content
All financial data is dynamic:
- Price, change, percentages: real-time feed
- Quick stats (Open/High/Low/Vol/MktCap): daily market data
- Chart: historical price data
- Position (shares, avg cost, values): user portfolio data
- Financials: quarterly/annual reported data
- Statistics: market data feed
- Analyst ratings: aggregated analyst data
- News: RSS/API feed
- Related portfolio: user's other holdings

### Truncation Rules
- **About description**: Truncated with ellipsis ("...") and "Read more" link after ~2 lines
- **News headlines**: Should truncate at ~2 lines with ellipsis (some headlines are long)
- **Company names in Portfolio**: Truncated if too long for available width
- **Financial values**: Abbreviated with B/M/T suffixes

---

## 11. Data + Logic

### Data Fields
| Field | Type | Source | Editable | Validation | Format |
|-------|------|--------|----------|------------|--------|
| Ticker Symbol | string | API | No | — | ALL CAPS, 1-5 chars |
| Company Name | string | API | No | — | Title case |
| Current Price | decimal | Real-time feed | No | — | $X,XXX.XX |
| Price Change | decimal | Computed | No | — | ±$X.XX |
| Price Change % | decimal | Computed | No | — | ±X.XX% |
| Open/High/Low | decimal | Market data | No | — | $X,XXX.XX |
| Volume | integer | Market data | No | — | XX.XM (abbreviated) |
| Market Cap | decimal | Computed | No | — | X.XXT (abbreviated) |
| Chart Data | time series | Historical API | No | — | Array of {date, price, volume} |
| About Description | string | API | No | — | Plain text, truncated |
| Company Facts | object | API | No | — | Key-value pairs |
| Shares Held | integer | User portfolio | No | — | Whole number |
| Avg Cost | decimal | Computed | No | — | $XXX.XX |
| Market Value | decimal | Computed | No | — | shares × current price |
| Unrealized P/L | decimal | Computed | No | — | market value − (shares × avg cost) |
| Return % | decimal | Computed | No | — | (unrealized / total cost) × 100 |
| Portfolio Weight | decimal | Computed | No | — | market value / total portfolio value |
| Revenue/NI/EPS | decimal[] | Financials API | No | — | $XXXB, $X.XX |
| Statistics | object | Market data | No | — | Various formats |
| Analyst Counts | integer[] | Analyst API | No | — | Buy/Hold/Sell integers |
| Price Targets | decimal[] | Analyst API | No | — | Low/Avg/High |
| News Items | object[] | News API | No | — | {category, headline, time} |
| Related Holdings | object[] | User portfolio | No | — | {ticker, name, price, change%} |

### Computed/Derived Values
| Value | Formula | Display |
|-------|---------|---------|
| Market Value | `shares × currentPrice` | $21,049 |
| Unrealized P/L | `marketValue - (shares × avgCost)` | +$5,884.55 |
| Return % | `(unrealizedPL / (shares × avgCost)) × 100` | +38.8% |
| Portfolio Weight | `marketValue / totalPortfolioValue × 100` | 12.8% |
| Analyst bar widths | `count / totalAnalysts × 100%` | Proportional bar segments |
| Price Target marker | `(current - low) / (high - low) × trackWidth` | Positioned dot |

### Pagination / Infinite Scroll
- **Financial table**: Fixed at 4 years (Annual) — no pagination needed
- **News**: Shows 4 items — likely needs "Show More" or pagination for full feed
- **Also in Portfolio**: Shows 3 items — likely needs "View All" link if portfolio is large

---

## 12. Accessibility

### Keyboard Navigation
Expected tab order:
1. Back to Dashboard link
2. TRADE button (header)
3. Watchlist button
4. Timeframe buttons (1D → ALL)
5. Chart type toggle (Line/Area)
6. Read more link
7. Website link, SEC Filings link
8. TRADE button (position)
9. SELL ALL button
10. Annual/Quarterly toggle
11. News items (4)
12. Portfolio ticker rows (3)
13. Back to Dashboard link (footer)

### Screen Reader
| Element | ARIA Requirement |
|---------|-----------------|
| Price change badge | `aria-label="Price increased by $3.42, up 1.40 percent today"` |
| Chart | `role="img"` with `aria-label` describing trend, or data table alternative |
| Stacked bar (analyst) | `aria-label="28 buy, 8 hold, 2 sell ratings"` |
| Price target range | `aria-label="Price target: low $210, average $265, high $310. Current price $247.63"` |
| Portfolio weight bar | `role="progressbar"` with `aria-valuenow="12.8"` |
| News category badges | Decorative only — category already visible in context |
| Volume bars | `aria-hidden="true"` (supplementary to chart data) |

### Touch Targets
| Element | Current Size (est.) | Min Required | Pass/Fail |
|---------|-------------|-------------|-----------|
| TRADE button (header) | ~90 × 36px | 40px desktop | Pass ✓ (width), Borderline (height) |
| Watchlist button | ~110 × 36px | 40px desktop | Pass ✓ (width), Borderline (height) |
| Timeframe button (each) | ~28 × 28px | 40px desktop | **Fail** ✗ |
| Chart type toggle item | ~50 × 28px | 40px desktop | **Fail** ✗ (height) |
| Read more link | Text-sized | 40px desktop | **Fail** ✗ |
| News item | Full row ~260 × 60px | 40px desktop | Pass ✓ |
| Portfolio ticker row | Full row ~260 × 48px | 40px desktop | Pass ✓ |
| External links | Text-sized | 40px desktop | **Fail** ✗ |

### Contrast + Sizing
See Section 8 Contrast Audit. Key failures:
- Section headers: ~3.2:1 (needs 4.5:1)
- TRADE button text: ~3.8:1 (needs 4.5:1)
- Gold "Hold" number: ~2.5:1 (needs 4.5:1)
- Timestamps: ~3.2:1 (needs 4.5:1)

---

## 13. Implementation Notes

### Recommended Patterns
- **Layout**: CSS Grid with `grid-template-columns: 1fr 320px` for the two-column layout. Switch to single column at `< 1024px`
- **Cards**: Shared `Card` component with consistent padding, radius, and border
- **Charts**: Use a library like Recharts, Lightweight Charts (TradingView), or D3.js for the interactive area chart
- **Tables**: HTML `<table>` with right-aligned numeric columns. Financial data table is small enough not to need virtualization
- **Number formatting**: Use `Intl.NumberFormat` for currency, percentages, and abbreviated values (62.1M, 3.82T)
- **Real-time**: WebSocket connection for price updates; debounce UI updates to prevent jank
- **Stacked bar**: Simple CSS flexbox with proportional `flex-grow` values
- **Price target range**: CSS positioned marker on a gradient track

### Design Tokens to Extract
```
// ═══════════════════════════
// COLORS — Base (Warm Stone)
// ═══════════════════════════
--color-base-900: #1C1917;
--color-base-700: #44403C;
--color-base-500: #78716C;
--color-base-400: #A8A29E;
--color-base-300: #D6CFC7;
--color-base-200: #EBE5DE;
--color-base-100: #F5F0EB;
--color-base-50:  #FAFAF8;
--color-white:    #FFFFFF;

// COLORS — Accent Green
--color-green-700: #2D6A4F;
--color-green-100: #E8F5E9;

// COLORS — Accent Coral
--color-coral-600: #C2724E;
--color-coral-100: #FDF2ED;

// COLORS — Accent Gold
--color-gold-700: #8B6914;
--color-gold-600: #C9A94E;
--color-gold-100: #FEF3C7;

// COLORS — Accent Brown
--color-brown-600: #78542B;
--color-brown-100: #F5E6D3;

// ═══════════════════════════
// SPACING (4px baseline)
// ═══════════════════════════
--spacing-unit: 4px;
--spacing-xs:   4px;   /* 1 unit */
--spacing-sm:   8px;   /* 2 units */
--spacing-md:   12px;  /* 3 units */
--spacing-base: 16px;  /* 4 units */
--spacing-lg:   20px;  /* 5 units */
--spacing-xl:   24px;  /* 6 units */
--spacing-2xl:  32px;  /* 8 units */
--spacing-3xl:  48px;  /* 12 units */

// ═══════════════════════════
// TYPOGRAPHY
// ═══════════════════════════
--font-family-display: Georgia, "Times New Roman", serif;
--font-family-body:    Inter, -apple-system, "Segoe UI", sans-serif;

--font-size-display:  52px;
--font-size-metric:   24px;
--font-size-heading:  20px;
--font-size-ticker:   18px;
--font-size-target:   16px;
--font-size-body:     14px;
--font-size-label:    13px;
--font-size-section:  11px;
--font-size-micro:    10px;

--font-weight-light:     300;
--font-weight-regular:   400;
--font-weight-medium:    500;
--font-weight-semibold:  600;
--font-weight-bold:      700;

--line-height-display: 1.1;
--line-height-heading: 1.25;
--line-height-body:    1.5;
--line-height-label:   1.4;

--letter-spacing-caps: 1.5px;

// ═══════════════════════════
// OPACITY
// ═══════════════════════════
--opacity-primary-text:   1.0;
--opacity-secondary-text: 0.60;
--opacity-disabled:       0.38;
--opacity-placeholder:    0.50;
--opacity-divider:        0.15;
--opacity-hover-fill:     0.06;
--opacity-pressed-fill:   0.14;
--opacity-scrim:          0.40;
--opacity-chart-fill:     0.15;

// ═══════════════════════════
// SHADOWS
// ═══════════════════════════
--shadow-card:    0px 1px 3px rgba(28, 25, 23, 0.04);
--shadow-hover:   0px 2px 8px rgba(28, 25, 23, 0.08);
--shadow-dropdown: 0px 4px 16px rgba(28, 25, 23, 0.12);

// ═══════════════════════════
// GRADIENTS
// ═══════════════════════════
--gradient-price-target: linear-gradient(90deg, #2D6A4F 0%, #C9A94E 50%, #C2724E 100%);
--gradient-chart-fill:   linear-gradient(180deg, rgba(45,106,79,0.15) 0%, transparent 100%);

// ═══════════════════════════
// RADII
// ═══════════════════════════
--radius-xs:   4px;
--radius-sm:   6px;
--radius-md:   8px;
--radius-lg:   12px;
--radius-full: 9999px;  /* pill shapes */

// ═══════════════════════════
// COMPONENT SIZES
// ═══════════════════════════
--button-height-sm:  36px;
--button-height-md:  44px;
--button-padding-v:  10px;
--button-padding-h:  24px;
--card-padding:      24px;
--stat-row-height:   36px;
--nav-height:        48px;
--footer-height:     48px;
--sidebar-width:     320px;
```

### Edge Cases
| Scenario | Handling |
|----------|---------|
| Price negative (stock down) | Change badge: red background, down arrow, negative values. Return/unrealized: red text |
| Extremely long company name | Truncate with ellipsis in header; full name in About section |
| No analyst ratings | Show "No analyst coverage" message in ratings card |
| No news | Show "No recent news" empty state |
| Zero shares (no position) | Position card: "You don't own this stock" with single TRADE button |
| Market closed | Add "Market Closed" badge near price; show last close time |
| After-hours pricing | Show after-hours price with "After Hours" label |
| Large unrealized loss | Red text, potentially with warning icon |
| >99% portfolio weight | Progress bar full, potential risk warning |
| Quarterly financials toggle | Table expands to 4-8 quarters; may need horizontal scroll |
| Data loading | Skeleton shimmer for each card independently |
| API failure | Error state per card, not full page — allow partial data display |

### Dev Gotchas
- **Number formatting consistency**: All prices use 2 decimal places except abbreviated values (62.1M, 3.82T). EPS uses 2 decimals. Ensure consistent formatting utility.
- **Chart responsiveness**: SVG chart must resize smoothly. Use `viewBox` and `preserveAspectRatio`.
- **Tabular numbers**: Use `font-variant-numeric: tabular-nums` for all numeric columns to prevent layout shift.
- **Real-time price updates**: Debounce to ~1s minimum to prevent excessive re-renders. Use CSS transitions on price changes for smooth visual feedback.
- **Z-index**: Chart tooltip needs to float above card borders. Modal (trade flow) needs to be above everything.
- **Scroll position**: "Also in Portfolio" clicking a ticker should navigate without losing scroll context (use route-level navigation, not full page reload).

---

## 14. Design Quality Audit

### Scorecard
| Category | Rating | Key Findings |
|----------|--------|-------------|
| Color Harmony | **Good** | Warm analogous palette with complementary green accent; sophisticated and intentional |
| Color Contrast (WCAG) | **Poor** | 4 critical failures: section headers, coral button text, gold hold number, timestamps |
| Color Weight System | **Good** | Clear base 50–900 scale with warm undertones; accent colors have implied pairs |
| Typography Scale | **Good** | Clear system from 10px–52px; weights inversely proportional to size; readable |
| Typography Hierarchy | **Good** | Size, weight, color, caps, and position all reinforce hierarchy effectively |
| Spacing Consistency | **Good** | 4-point baseline system consistently applied; only button padding (10px) deviates |
| Component Standards | **Good** | Cards, buttons, tables follow component rules well; table alignment correct |
| Visual Hierarchy | **Good** | Clear F-pattern: price → chart → position → stats. Size = importance applied well |
| Button Hierarchy | **Fair** | TRADE is visually primary but uses outline style (typically secondary). No filled primary variant |
| Icon Consistency | **Good** | Minimal icons used (chevron, star, arrow); consistent outline style |
| Negative Space | **Good** | Generous card padding, breathing room between sections; cream bg adds spaciousness |
| Alignment | **Good** | Everything aligns to grid; numbers right-aligned; labels left-aligned |
| Opacity Usage | **Fair** | Hierarchy achieved via color rather than opacity; explicit opacity tokens not visible |
| Gradient Quality | **Good** | Only 2 gradients, both subtle and semantically meaningful |
| Spacing Relationships | **Good** | Proportional system: page > section > card > element at ~1.5-2x ratios |
| Layout Proportions | **Good** | ~73:27 split close to golden ratio; content-whitespace ratio ~65:35 ideal |
| Interaction State Coverage | **Poor** | Only default states shown; hover, focus, pressed, disabled all missing |
| Accessibility | **Poor** | Contrast failures, small touch targets on timeframe buttons, no focus indicators |

### Violations Found
| # | Rule Violated | Element | Severity | Recommendation |
|---|---------------|---------|----------|----------------|
| 1 | Contrast minimum (4.5:1) | Section headers (#A8A29E on #FFF) at ~3.2:1 | **Critical** | Darken to #78716C (~4.5:1) or use Base-500 |
| 2 | Contrast minimum (4.5:1) | TRADE button text (#C2724E on #FFF) at ~3.8:1 | **Critical** | Darken to #A85A38 or use filled button with white text |
| 3 | Contrast minimum (4.5:1) | Gold "Hold" number (#C9A94E on #FFF) at ~2.5:1 | **Critical** | Darken to #8B6914 (Gold-700) at ~5.2:1 |
| 4 | Contrast minimum (4.5:1) | Timestamps (#A8A29E on #FFF) at ~3.2:1 | **Major** | Darken to #78716C |
| 5 | Button hierarchy | TRADE button uses outline (secondary style) for primary CTA | **Major** | Use filled coral background with white text for primary action |
| 6 | Touch targets (40px desktop) | Timeframe buttons at ~28×28px | **Major** | Increase to min 40×40px tap targets (add invisible padding if needed) |
| 7 | Touch targets | Chart type toggle items at ~50×28px height | **Major** | Increase height to 36–40px |
| 8 | Interaction states | No hover, focus, pressed, disabled states shown | **Major** | Define all states for every interactive element |
| 9 | Button color semantics | TRADE (buy) and Sell rating share same coral color | **Minor** | Use green for buy action, coral for sell action |
| 10 | Spacing baseline | Button padding 10px breaks 4-point system (not multiple of 4) | **Minor** | Use 8px or 12px vertical padding |
| 11 | Missing states | No empty, loading, error, disabled states designed | **Major** | Design skeleton states, error cards, empty positions |
| 12 | Focus indicators | No keyboard focus states visible anywhere | **Critical** | Add visible focus rings (box-shadow) per WCAG 2.4.7 |
| 13 | Table accessibility | Financial year labels ("2024") are in quotes in snapshot | **Minor** | Ensure proper table markup with `<th>` scope attributes |
| 14 | Icon + text spacing | Watchlist star icon may be too close to text | **Minor** | Ensure ~1em (14px) gap between icon and label |

### What's Working Well
1. **Warm color palette** — The cream/stone base with editorial serif pricing creates a premium, distinctive feel that stands apart from typical cold blue fintech dashboards
2. **Information density without clutter** — Packs significant data into one view while maintaining readable hierarchy through generous whitespace and clear card boundaries
3. **Typography hierarchy** — Excellent use of weight inversion (light at large sizes, bold at small) and ALL CAPS for structural labels
4. **Two-column layout proportions** — Near-golden ratio split (73:27) feels balanced; sidebar complements rather than competes with main content
5. **Consistent spacing system** — 4-point baseline consistently applied; proportional relationships between page/section/card/element spacing
6. **Number formatting** — Right-aligned numerics, consistent abbreviation conventions (B, T, M), tabular alignment
7. **Semantic color coding** — Green/gold/red maps intuitively to buy/hold/sell and positive/neutral/negative
8. **Card-based architecture** — Each card is an independent data module that could be rearranged, hidden, or lazy-loaded independently
9. **Progressive disclosure** — "Read more" for descriptions, Annual/Quarterly toggle, timeframe selector reduce initial cognitive load
10. **Footer navigation mirror** — Repeating "Back to Dashboard" at bottom is a thoughtful touch for long-scroll pages

---

## 15. Open Questions

| # | Question | Area | Priority | Default Assumption |
|---|----------|------|----------|--------------------|
| 1 | What font families are used? The price display appears serif, body appears sans-serif | Typography | High | Georgia for display, Inter for body |
| 2 | What happens when the stock is negative for the day? Red variant of change badge needed | States | High | Red badge with down arrow, swap green for red |
| 3 | What does the TRADE button do? Modal, drawer, or new page? | Interaction | High | Opens a trade modal/dialog |
| 4 | Is the chart interactive (tooltip on hover, zoom, pan)? | Interaction | High | Yes — hover tooltip showing price at cursor |
| 5 | What are the hover/pressed states for all interactive elements? | Interaction | High | Define per the suggested states in Section 9 |
| 6 | Is there a dark mode variant? | Theming | Medium | Not yet, but the warm palette could invert well |
| 7 | How does the "Read more" expand — inline or modal? | Interaction | Medium | Inline expand with animation |
| 8 | Are news items clickable to full articles? Internal or external? | Navigation | Medium | Yes — open in new tab (external source) |
| 9 | What is the responsive behavior below 1024px? | Layout | Medium | Single column, sidebar stacks below content |
| 10 | How often does price data refresh? WebSocket or polling? | Data | Medium | WebSocket with 1s debounce |
| 11 | What does "Quarterly" financials view show? How many quarters? | Content | Medium | 4 most recent quarters in same table format |
| 12 | Is the volume mini bar chart in Financials interactive? | Interaction | Low | No — static decorative visualization |
| 13 | Can users customize which cards are visible or their order? | Personalization | Low | No — fixed layout for v4.0 |
| 14 | What's the max news items? Is there pagination/load more? | Content | Low | Show 4, "View All" link for full feed |
| 15 | Should the Watchlist star animate on toggle? | Micro-interaction | Low | Yes — fill animation, 200ms |

---

## Appendix A: Pixel-Level Measurement Reference

### Element Dimensions
| Element | Width | Height | Aspect Ratio |
|---------|-------|--------|-------------|
| Full viewport | ~1456px | ~1580px (scrollable) | ~0.92:1 |
| Content max-width | ~1280px | — | — |
| Top navigation bar | 100% | ~48px | — |
| Left column | ~920px (~73%) | Auto | — |
| Right column (sidebar) | ~320px (~27%) | Auto | — |
| Chart card | ~920px | ~380px | ~2.4:1 |
| Chart area (SVG only) | ~870px | ~260px | ~3.3:1 |
| About card | ~920px | ~240px | ~3.8:1 |
| Position card | ~920px | ~260px | ~3.5:1 |
| Financials card | ~920px | ~320px | ~2.9:1 |
| Statistics card | ~320px | ~460px | ~0.7:1 |
| Analyst Ratings card | ~320px | ~220px | ~1.5:1 |
| News card | ~320px | ~280px | ~1.1:1 |
| Portfolio card | ~320px | ~200px | ~1.6:1 |
| TRADE button (header) | ~90px | ~36px | ~2.5:1 |
| Watchlist button | ~110px | ~36px | ~3:1 |
| TRADE button (position) | ~420px (50% card) | ~44px | ~9.5:1 |
| SELL ALL button | ~420px (50% card) | ~44px | ~9.5:1 |
| Position metric box | ~280px (~33%) | ~72px | ~3.9:1 |
| Timeframe button | ~28px | ~28px | 1:1 |
| Chart type toggle | ~100px | ~28px | ~3.6:1 |
| Change badge (pill) | ~150px | ~28px | ~5.4:1 |
| Category badge (news) | ~50-70px | ~20px | ~3:1 |
| Mini revenue bar | ~40px | ~50px | ~0.8:1 |
| Analyst stacked bar | ~260px | ~8px | 32.5:1 |
| Price target track | ~260px | ~3px | ~87:1 |
| Price target marker | ~8px | ~8px | 1:1 |
| Portfolio weight bar | ~600px | ~4px | 150:1 |
| Volume bars (set) | ~660px | ~30px | ~22:1 |
| Footer | 100% | ~48px | — |

### Padding & Margin Map
| Element | Padding (T/R/B/L) | Margin (T/R/B/L) | Border Width | Border Radius |
|---------|-------------------|-------------------|-------------|---------------|
| Page body | 0/48/0/48 | — | — | — |
| Top nav bar | 12/0/12/0 | 0/0/0/0 | 0 (bottom border implied) | 0 |
| Price hero section | 24/0/16/0 | 0/0/24/0 | — | — |
| Card (generic) | 24/24/24/24 | 0/0/20/0 | ~1px | 12px |
| Chart card | 20/24/20/24 | 0/0/20/0 | ~1px | 12px |
| Position metric box | 16/16/16/16 | 0/8/0/0 (gap) | ~1px | 8px |
| TRADE button (header) | 10/24/10/24 | 0/0/0/0 | ~1.5px | 6px |
| TRADE button (position) | 12/0/12/0 | 0/8/0/0 | ~1.5px | 6px |
| Watchlist button | 10/20/10/20 | 0/0/0/12 | ~1px | 6px |
| Change badge | 6/12/6/12 | 0/12/0/0 | 0 | 20px (pill) |
| Category badge (news) | 3/8/3/8 | 0/8/0/0 | 0 | 4px |
| Statistics card row | 8/0/8/0 | 0/0/0/0 | 0 (bottom border ~1px) | 0 |
| Annual/Quarterly toggle | 4/12/4/12 per item | 0/0/0/0 | ~1px | 6px |
| News item | 12/0/12/0 | 0/0/0/0 | 0 | 0 |
| Portfolio ticker row | 12/0/12/0 | 0/0/0/0 | 0 (bottom border ~1px) | 0 |
| Footer | 12/48/12/48 | 0/0/0/0 | top border ~1px | 0 |

### Gap & Spacing Map
| Between Elements | Gap (px) | Direction | Base Unit Multiple |
|-----------------|----------|-----------|-------------------|
| Top nav → Price hero | ~24px | Vertical | 6× |
| Price value → Change badge | ~12px | Horizontal | 3× |
| Change badge → "today" | ~8px | Horizontal | 2× |
| Price → Quick stats | ~8px | Vertical | 2× |
| Quick stats → Chart card | ~24px | Vertical | 6× |
| Left column cards → cards | ~20px | Vertical | 5× |
| Left column → Right column | ~20px | Horizontal | 5× |
| Right column cards → cards | ~20px | Vertical | 5× |
| Section header → content | ~16px | Vertical | 4× |
| About facts label → value | ~48px | Horizontal | 12× (fixed key-value indent) |
| About facts row → row | ~4px | Vertical | 1× |
| Position metric box → box | ~8px | Horizontal | 2× |
| Position metric label → value | ~8px | Vertical | 2× |
| Weight bar label → bar | ~8px | Vertical | 2× |
| TRADE → SELL ALL button | ~16px | Horizontal | 4× |
| Table header → first row | ~8px | Vertical | 2× |
| Table row → row | ~0px (border only) | Vertical | — |
| Stat row → row | ~0px (border only) | Vertical | — |
| News badge → headline | ~8px | Horizontal | 2× |
| Headline → timestamp | ~4px | Vertical | 1× |
| News item → item | ~0px (padding creates gap) | Vertical | — |
| Footer back → branding | Justified ends | Horizontal | — |
| Analyst count → label | ~4px | Vertical | 1× |
| Price target labels → values | ~4px | Vertical | 1× |

### Shadow Specifications
| Element | X | Y | Blur | Spread | Color | Opacity |
|---------|---|---|------|--------|-------|---------|
| Card (default) | 0px | 1px | 3px | 0px | #1C1917 | ~4% |
| Card (hover — suggested) | 0px | 2px | 8px | 0px | #1C1917 | ~8% |
| Chart type toggle (active item) | 0px | 1px | 2px | 0px | #1C1917 | ~6% |
| Annual/Quarterly toggle (active) | 0px | 1px | 2px | 0px | #1C1917 | ~6% |
| Dropdown (suggested) | 0px | 4px | 16px | 0px | #1C1917 | ~12% |

---

*End of specification. All measurements prefixed with `~` are visual estimates from screenshots at 1x resolution. Verify against source code or design tool for exact values.*

That's the full design specification. The document is saved to `meridian-dashboard-design-spec.md`.

**Key takeaways:**

- **Strongest aspects**: Warm editorial color palette, excellent typography hierarchy, clean information density, near-golden-ratio layout proportions
- **Critical issues to fix**: 4 WCAG contrast failures (section headers, button text, gold numbers, timestamps), missing interaction states entirely, small touch targets on chart controls, no focus indicators
- **Biggest gap**: Zero hover/pressed/focused/disabled states are defined — this needs a full interaction design pass before implementation