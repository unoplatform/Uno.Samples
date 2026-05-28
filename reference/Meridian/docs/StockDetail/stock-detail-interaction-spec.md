# Meridian Stock Detail — Interaction & State Spec

> **Page:** Stock Detail (Page 2)  
> **Source Prototype:** `stock-detail-v1.jsx`  
> **Pattern:** MVUX Reactive  
> **Relationship to dashboard:** Shares the Trade Drawer, design system, and several interaction patterns. This document covers only behaviors specific to or modified for the Stock Detail page.

---

## 1. State Model

### 1.1 Page-Level State

| State Property | Type | Default | Description |
|---------------|------|---------|-------------|
| `Stock` | `Stock` (injected) | *from nav* | The stock being viewed — set at navigation time, immutable for the page's lifetime |
| `SelectedTimeframe` | `IState<string>` | `"3M"` | Active timeframe for chart (1D/1W/1M/3M/6M/YTD/1Y/5Y/ALL) |
| `ChartType` | `IState<string>` | `"Area"` | Chart rendering mode ("Line" or "Area") |
| `FinancialsPeriod` | `IState<string>` | `"Annual"` | Toggle for financials table ("Annual" or "Quarterly") |
| `IsInWatchlist` | `IState<bool>` | `false` | Whether the user is watching this stock |
| `AboutExpanded` | `IState<bool>` | `false` | Whether the About description is fully expanded |
| `TradeStock` | `IState<Stock?>` | `null` | `null` = drawer closed, `Stock` = drawer open |

### 1.2 Trade Drawer State (shared model)

| State Property | Type | Default | Description |
|---------------|------|---------|-------------|
| `Side` | `IState<string>` | `"buy"` | Buy or sell |
| `Quantity` | `IState<int>` | `0` | Number of shares |
| `OrderType` | `IState<string>` | `"market"` | market / limit / stop |
| `LimitPrice` | `IState<decimal?>` | `null` | Only for limit/stop orders |
| `IsConfirmed` | `IState<bool>` | `false` | Toggles form → confirmation view |

### 1.3 Computed / Derived Values

| Computed | Source | Description |
|----------|--------|-------------|
| `ChartData` | `SelectedTimeframe` + `Stock.Ticker` | 90-point history re-fetched when timeframe changes |
| `ChartGradientOpacity` | `ChartType` | 0.12 for Area, 0 for Line (gradient fill toggle) |
| `PositionExists` | `Position` feed | `true` if user holds this stock, drives conditional card |
| `MarketValue` | `Position.Shares × Position.CurrentPrice` | Displayed in mini-stat card |
| `UnrealizedGain` | `(Current - Avg) × Shares` | Colored green/red |
| `ReturnPct` | `(Current - Avg) / Avg × 100` | Colored green/red |
| `PortfolioWeight` | `MarketValue / TotalPortfolioValue × 100` | Drives weight bar width |
| `PriceDotPosition` | `(Price - TargetLow) / (TargetHigh - TargetLow) × 100%` | Analyst range dot |
| `IsPositive` | `Stock.Pct >= 0` | Drives arrow direction, pill color, chart color |

---

## 2. User Flows

### Flow 1: Navigate to Stock Detail

```
1. User is on Dashboard
2. User clicks "Details ⟶" button in:
   a. Expanded watchlist row (3rd action button)
   b. Chart header in stock-detail mode (DETAILS button, left of TRADE)
3. Navigation fires: Request="StockDetail", Data=Stock
4. Page transition: slide left, 0.35s ease-smooth
5. StockDetailPage loads with Stock parameter
6. All feeds fire simultaneously (position, profile, analysts, etc.)
7. Page entrance animation begins (see §4)
8. User sees header with back nav, price, chart, and cards loading
```

### Flow 2: Research the Stock

```
1. User scans price hero (current price, day change, OHLC strip)
2. User checks chart — default 3M view in Area mode
3. User may toggle timeframes: click "1Y" → chart data swaps, re-animates
4. User may toggle chart type: click "Line" → gradient fill disappears
5. User scrolls right sidebar: stats (12 metrics), analyst consensus (buy/hold/sell bar + price target)
6. User scrolls left column: reads About (may expand for full description), checks position, reviews financials (may toggle Annual/Quarterly)
7. User checks Related News for recent headlines about this stock
8. User checks Similar Holdings to see comparable stocks in their portfolio
```

### Flow 3: Trade from Stock Detail

```
1. User clicks "TRADE" button (header bar OR position card)
2. TradeStock state set → drawer opens with spring slide
3. Drawer pre-filled with stock data (ticker, name, price)
4. (Same trade flow as dashboard — see dashboard interaction spec)
5. After confirmation + auto-close, TradeStock resets to null
```

### Flow 4: Sell All from Position Card

```
1. User clicks "SELL ALL" button in Position card
2. tradeSide set to "sell"
3. tradeQty pre-filled with String(position.shares) — e.g., "85"
4. TradeStock state set → drawer opens
5. Drawer shows pre-filled sell order for full position
6. User can adjust quantity or proceed
7. (Same trade flow from step 4 onward)
```

### Flow 5: Navigate Back to Dashboard

```
1. User clicks "← Back to Dashboard" (header or footer)
2. NavigateBackAsync fires
3. Page transition: slide right, 0.35s ease-smooth
4. Dashboard state is preserved (scroll, expanded row, chart-swap)
```

### Flow 6: Navigate to Similar Holding

```
1. User clicks a row in Similar Holdings card (e.g., MSFT)
2. Navigation fires: Request="StockDetail", Data=SimilarStock
3. Current page is replaced (or pushed) with new Stock Detail for MSFT
4. Back navigation returns to previous stock detail (or dashboard)
```

---

## 3. Component State Matrix

### 3.1 Watchlist Toggle Button

| State | Visual | Trigger |
|-------|--------|---------|
| **Not watching** | "☆ Watchlist", border=border, color=muted, bg=transparent | Default |
| **Watching** | "★ Watching", border=accent, color=accent, bg=accent@6% | Click toggles `IsInWatchlist` |
| **Transition** | all 0.25s ease | — |

### 3.2 Timeframe Buttons (×9)

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | transparent bg, subtle text, no underline | — |
| **Active** | gain@8% bg, gain text, 2px underline bar (gain color, centered) | Click sets `SelectedTimeframe` |
| **Transition** | all 0.25s ease; underline width 0→100% 0.25s ease | — |

### 3.3 Chart Type Buttons (×2)

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | transparent bg, subtle text | — |
| **Active** | gain@8% bg, gain text | Click sets `ChartType` |

### 3.4 Chart Area

| State | Visual | Trigger |
|-------|--------|---------|
| **Area mode** | Line stroke + gradient fill (gain@12%→0%) + data dots | `ChartType == "Area"` |
| **Line mode** | Line stroke only, no gradient fill | `ChartType == "Line"` |
| **Timeframe change** | Chart data swaps, re-animates line draw (1200ms) | `SelectedTimeframe` changes |
| **Tooltip active** | White card with date + serif price, dashed cursor line | PointerMoved in chart area |

### 3.5 About Description

| State | Visual | Trigger |
|-------|--------|---------|
| **Collapsed** | First 180 characters + "..." + "[Read more]" link | Default (`AboutExpanded == false`) |
| **Expanded** | Full description text + "[Show less]" link | Click "Read more" toggles `AboutExpanded` |
| **Link text** | accent color, 12px, weight 600 | — |

### 3.6 Financials Period Toggle

| State | Visual | Trigger |
|-------|--------|---------|
| **Annual selected** | "Annual" has white bg + shadow, "Quarterly" transparent + subtle | Default |
| **Quarterly selected** | Inverse of above | Click toggles `FinancialsPeriod` |
| **Container** | bg: #F6F4F0, r=8, p=3 | — |
| **Active button** | bg: card (white), shadow `0 1px 4px rgba(0,0,0,0.06)`, text primary | — |
| **Inactive button** | bg: transparent, text subtle | — |
| **Data change** | Annual: 4 rows (2021-2024). Quarterly: 8 rows (Q1-Q4 × 2 years). Bar chart updates. | — |

### 3.7 Revenue Bar Chart

| State | Visual | Trigger |
|-------|--------|---------|
| **Entrance** | Bars grow from 0 with spring overshoot, staggered 0.1s per bar | Page mount |
| **Latest year** | opacity 0.85 | Always the rightmost bar |
| **Older years** | opacity 0.3 | All other bars |
| **Period change** | Bars re-animate from 0 | `FinancialsPeriod` toggles |

### 3.8 Position Card

| State | Visual | Trigger |
|-------|--------|---------|
| **Has position** | Full card: shares, avg cost, 3 mini-stats, weight bar, TRADE + SELL ALL buttons | `Position != null` |
| **No position** | Simplified: "You don't hold {ticker}" + single [BUY {ticker}] gold button | `Position == null` |
| **Weight bar fill** | Animated width 0→target, 1s ease, 0.5s delay | Page entrance |

### 3.9 TRADE Button (Header & Position Card)

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | border=accent, bg=accent@6%, color=accent | — |
| **Hovered** | bg=accent@14% | PointerEntered |
| **Clicked** | Sets `TradeStock = Stock`, opens drawer with side="buy" | Click |

### 3.10 SELL ALL Button

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | border=loss, bg=transparent, color=loss | — |
| **Hovered** | bg=loss@6% | PointerEntered |
| **Clicked** | Sets `tradeSide = "sell"`, `tradeQty = position.shares`, opens drawer | Click |

### 3.11 Similar Holdings Row

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | Transparent bg, ticker+name left, price+delta right | — |
| **Hovered** | bg=#FAF8F5, translateX(3px) | PointerEntered |
| **Clicked** | Navigates to StockDetail for that ticker | Click |
| **Transition** | all 0.3s cubic-bezier(0.4,0,0.2,1) | — |

### 3.12 News Row

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | Static position | — |
| **Hovered** | translateX(4px) | PointerEntered |
| **Transition** | transform 0.2s ease | — |

### 3.13 External Links (Website ↗, SEC Filings ↗)

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | accent color, 12px, weight 600, no underline | — |
| **Hovered** | underline appears | PointerEntered |

### 3.14 Back Button (Header & Footer)

| State | Visual | Trigger |
|-------|--------|---------|
| **Default** | icon + "Back to Dashboard", muted color | — |
| **Hovered** | color shifts to text primary | PointerEntered |
| **Clicked** | NavigateBackAsync | Click |

### 3.15 Trade Drawer

Identical to dashboard. See dashboard interaction spec for the full 5-state matrix (Closed → Opening → Form → Confirmed → Closing) with all sub-component states.

---

## 4. Animation Inventory

### 4.1 Page Entrance Sequence

| Step | Element | Animation | Delay | Duration | Easing |
|------|---------|-----------|-------|----------|--------|
| 1 | Page transition (from dashboard) | slideLeft | — | 0.35s | ease-smooth |
| 2 | Header bar | fadeIn | 0s | 0.5s | ease |
| 3 | Price hero | fadeUp | 0.1s | 0.6s | ease |
| 4 | Chart card | cardEntrance | 0.15s | 0.7s | spring overshoot |
| 5 | About card | cardEntrance | 0.25s | 0.7s | spring overshoot |
| 6 | Position card | cardEntrance | 0.3s | 0.7s | spring overshoot |
| 7 | Financials card | cardEntrance | 0.35s | 0.7s | spring overshoot |
| 8 | Key Statistics card | cardEntrance | 0.2s | 0.7s | spring overshoot |
| 9 | Analyst Consensus card | cardEntrance | 0.3s | 0.7s | spring overshoot |
| 10 | News card | cardEntrance | 0.35s | 0.7s | spring overshoot |
| 11 | Similar Holdings card | cardEntrance | 0.4s | 0.7s | spring overshoot |
| 12 | Chart line draw | strokeDashoffset | 0.5s (after card) | 1.2s | ease-out |
| 13 | Revenue bars grow | scaleY 0→1 | 0.5s+stagger | 0.6s | spring overshoot |
| 14 | Weight bar fill | width 0→target | 0.5s | 1s | ease |
| 15 | Footer | fadeIn | 0.6s | 0.8s | ease |

**cardEntrance keyframes** (same as dashboard):
```
0%   → opacity: 0, translateY(32px), scale(0.94), rotateX(4deg)
60%  → translateY(-4px), scale(1.01), rotateX(-1deg)
100% → opacity: 1, translateY(0), scale(1), rotateX(0)
```

### 4.2 Interactive Animations

| Animation | Duration | Easing | Trigger |
|-----------|----------|--------|---------|
| Timeframe button active | 0.25s | ease | Click |
| Chart type toggle | 0.25s | ease | Click |
| Chart re-draw on timeframe change | 1.2s | ease-out | `SelectedTimeframe` changes |
| Gradient fill appear/disappear | 0.3s | ease | `ChartType` changes |
| About expand/collapse | ~0.3s | ease | `AboutExpanded` toggles |
| Financials period toggle | 0.2s | ease | `FinancialsPeriod` changes |
| Revenue bars re-grow | 0.6s + stagger | spring | Period change |
| Watchlist toggle | 0.25s | ease | `IsInWatchlist` toggles |
| News row hover slide | 0.2s | ease | PointerEntered |
| Similar row hover slide | 0.3s | smooth | PointerEntered |
| TRADE button hover | 0.2s | ease | PointerEntered (bg @6%→@14%) |
| SELL ALL button hover | 0.2s | ease | PointerEntered (transparent→loss@6%) |
| Back button hover | 0.2s | ease | PointerEntered (muted→text primary) |
| External link hover | instant | — | Underline appears/disappears |
| Price target dot position | 0.6s | spring | On mount / data change |

### 4.3 Continuous Animations

| Animation | Duration/Rate | Trigger |
|-----------|-------------|---------|
| Gain pill glow | 3s loop, 2s start delay | Always running |
| Card hover shadow | 0.3s ease | PointerEntered on any .sd-card |

### 4.4 Easing Functions Reference

| Name | Value | Usage |
|------|-------|-------|
| ease | `ease` | Simple transitions, fades |
| ease-out | `ease-out` | Chart line draw, exit animations |
| spring | `cubic-bezier(0.34, 1.56, 0.64, 1)` | Card entrance, bar grow, dot position |
| smooth | `cubic-bezier(0.4, 0, 0.2, 1)` | Row hover, expand/collapse |
| ease-smooth | `cubic-bezier(0.4, 0, 0.2, 1)` | Page transitions |

---

## 5. Chart Tooltip

Same as dashboard. Light tooltip card:

| Property | Value |
|----------|-------|
| Background | `#FFFFFF` |
| Border | `1px solid #E8E4DE` |
| Corner radius | 10px |
| Shadow | `0 8px 32px rgba(0,0,0,0.08)` |
| Padding | 10px 14px |
| Date label | Outfit 10px, muted, marginBottom 3px |
| Value | Instrument Serif 16px, weight 600, text primary |
| Value format | `$247.63` (2 decimal places for stock) |
| Cursor line | Full chart height, dashed `#E8E4DE`, strokeDasharray "4 4" |
| Tracking | Horizontal snap to nearest data point |
| Entrance | Instant (no animation) |

---

## 6. How All Parts Connect

### The Research Flow

The page is designed as a top-to-bottom reading experience, but the 2-column layout allows lateral scanning:

```
USER ENTERS (from dashboard "Details" button)
        │
        ▼
HEADER BAR — establishes identity (AAPL · Apple Inc.)
        │     provides escape hatch (← back) and primary action (TRADE)
        │
        ▼
PRICE HERO — answers "what's it worth right now?"
        │     day change + OHLC give immediate context
        │
        ├───────────── left column ─────────────┬──── right column ────────┐
        │                                        │                          │
   CHART CARD                              KEY STATISTICS              │
   answers "where has it been?"            answers "is it cheap?"      │
   9 timeframes let user zoom              12 metrics at a glance      │
   chart type toggle for preference        alternating rows for scan   │
        │                                        │                          │
   ABOUT CARD                              ANALYST CONSENSUS          │
   answers "what does this company do?"    answers "what do pros think?"│
   expandable for depth seekers            buy/hold/sell bar is instant │
   metadata for sector context             price target gives range    │
        │                                        │                          │
   POSITION CARD                           RELATED NEWS                │
   answers "what's my exposure?"           answers "what's happening?" │
   conditional: shows position or empty    filtered to this ticker     │
   TRADE + SELL ALL are action endpoints        │                          │
        │                                        │                          │
   FINANCIALS CARD                         SIMILAR HOLDINGS            │
   answers "is the business growing?"      answers "what else should   │
   annual vs quarterly toggle              I look at?"                 │
   bar chart gives visual comparison       navigates to other details  │
        │                                        │                          │
        └────────────────────────────────────────┴──────────────────────────┘
                                    │
                                    ▼
                              FOOTER — secondary back nav + branding
                                    │
                                    ▼
                           USER DECIDES: Trade, go back, or explore similar
```

### Cross-Component Connections

| Source | Target | Connection |
|--------|--------|------------|
| Header TRADE button | Trade Drawer | Sets `TradeStock` → drawer opens |
| Position TRADE button | Trade Drawer | Sets `TradeStock` → drawer opens, side="buy" |
| Position SELL ALL button | Trade Drawer | Sets `TradeStock`, side="sell", qty=position.shares |
| Timeframe buttons | Chart card | `SelectedTimeframe` → `ChartData` feed re-fires → chart re-renders |
| Chart type buttons | Chart card | `ChartType` → gradient opacity toggles |
| Period toggle | Financials table + bar chart | `FinancialsPeriod` → data rows switch + bars re-animate |
| "Read more" link | About card | `AboutExpanded` → text truncation toggles |
| Watchlist toggle | Header button state | `IsInWatchlist` → icon/text/color swaps |
| Similar Holdings row click | Navigation | Navigates to new StockDetail instance |
| Back button (header/footer) | Navigation | NavigateBackAsync to dashboard |

---

## 7. Edge Cases

| Scenario | Behavior | Recommendation |
|----------|----------|----------------|
| Stock not found / null nav param | Page would render empty | Guard in model constructor: if Stock is null, navigate back |
| User has no position in this stock | Position card shows empty state | "You don't hold {ticker}" + BUY button. MiniStats and weight bar hidden. |
| Analyst data unavailable | Rating bar would be empty | Show "No analyst data available" placeholder |
| Very long company description | Truncated at 180 chars | "Read more" expands; collapse re-truncates |
| News has 0 items for this ticker | News card would be empty | Show "No recent news for {ticker}" placeholder |
| Similar Holdings has 0 same-sector stocks | Card would be empty | Hide the card entirely (Visibility.Collapsed) |
| Navigating A → B → C via Similar Holdings | Deep forward stack | Consider limiting stack depth or using NavigateRouteAsync with replacement |
| Trade drawer open + back button pressed | Conflict: close drawer or navigate? | Close drawer first (if open), then navigate back on next click |
| Timeframe change while tooltip visible | Tooltip would reference stale data | Dismiss tooltip on any data change |
| Very narrow price target range (low ≈ high) | Dot position unstable | Clamp dot to 5%-95% range to avoid edge overflow |
| Period toggle during bar animation | Animation may conflict | Cancel existing animation before starting new one |

---

## 8. Keyboard & Focus

| Context | Recommended Behavior |
|---------|---------------------|
| Page entry | Focus moves to header area (not auto-focus on any input) |
| Timeframe buttons | ←/→ arrow keys cycle through options |
| Chart type buttons | ←/→ arrow keys toggle between Line/Area |
| Trade drawer | Focus trap when open, Tab cycles form fields, Escape closes |
| "Read more" / "Show less" | Enter/Space toggles expansion |
| Watchlist toggle | Enter/Space toggles watching state |
| Back button | Enter activates back navigation |
| Similar Holdings rows | Enter navigates to that stock's detail |
| TRADE / SELL ALL buttons | Enter activates the action |
| Tab order | Header → Price (passive) → Chart controls → About → Position buttons → Financials toggle → News (passive) → Similar Holdings |
