# Meridian Capital Terminal — Interaction & State Spec

> **Source Prototype:** Meridian v4 (React/JSX)  
> **Target Platform:** Uno Platform (C# / WinUI 3 / XAML)  
> **Date:** March 18, 2026

---

## 2.1 Application State Model

The prototype manages 7 pieces of mutable state. In MVUX, these map to `IState<T>` properties on the presentation model.

| State Property | Type | Default | Description |
|---------------|------|---------|-------------|
| `SelectedTimeframe` | `IState<string>` | `"3M"` | Active timeframe filter for chart (1D / 1W / 1M / 3M / YTD / 1Y) |
| `ChartTicker` | `IState<string?>` | `null` | `null` = portfolio view; `"AAPL"` = stock detail view |
| `ExpandedTicker` | `IState<string?>` | `null` | Which watchlist row is expanded (`null` = all collapsed) |
| `SearchQuery` | `IState<string>` | `""` | Current search text, filters watchlist in real-time |
| `TradeStock` | `IState<Stock?>` | `null` | `null` = drawer closed; `Stock` object = drawer open for that stock |
| `HoveredHolding` | `IState<string?>` | `null` | Which holding card is being hovered (visual feedback only) |
| `CurrentTime` | `IState<DateTime>` | `DateTime.Now` | Updated every 1s via `DispatcherTimer` for clock display |

### Derived / Computed Values

| Computed | Source | Description |
|----------|--------|-------------|
| `ChartData` | `ChartTicker` + service | Portfolio history when `null`, stock-specific history when set |
| `ChartColor` | `ChartTicker` → `Stock.Pct` | `#2D6A4F` (green) for positive stocks, `#B5342B` (red) for negative |
| `ChartLabel` | `ChartTicker` | `"Performance"` or the stock's company name |
| `ChartYFormat` | `ChartTicker` | `$124k` (portfolio scale) or `$892` (stock scale) |
| `FilteredWatchlist` | `SearchQuery` + `Watchlist` | Case-insensitive filter on ticker or name |
| `TotalPortfolioValue` | `Holdings` | Sum of `Shares × CurrentPrice` |
| `TotalGain` | `Holdings` | Sum of `(Current - Avg) × Shares` |
| `TotalGainPct` | `Holdings` | `TotalGain / TotalCost × 100` |

### Trade Drawer State (Separate Model)

| State Property | Type | Default | Description |
|---------------|------|---------|-------------|
| `Side` | `IState<string>` | `"buy"` | Buy or sell |
| `Quantity` | `IState<int>` | `0` | Number of shares |
| `OrderType` | `IState<string>` | `"market"` | market / limit / stop |
| `LimitPrice` | `IState<decimal?>` | `null` | Only used for limit/stop orders |
| `IsConfirmed` | `IState<bool>` | `false` | Toggles form → confirmation view |

---

## 2.2 Primary User Flows

### Flow 1: View Stock Detail (Chart ↔ Holdings Linking)

```
1. User clicks a holding card in the Holdings panel
2. ChartTicker state updates to that ticker (e.g., "AAPL")
3. Chart card header transitions:
   - "Performance" label → stock full name
   - Stock price + delta% appear
   - "TRADE" button appears (gold outline)
   - ← back button appears
4. Chart data swaps from portfolio history to stock-specific 90-day history
5. Chart re-animates with 800ms entrance
6. Chart stroke color changes: green for positive stocks, red for negative
7. Y-axis formatter switches from "$124k" to "$892"
8. Holding card shows:
   - Green 6px dot with ring shadow (selected indicator)
   - Green border + green-tinted background
9. Click same holding again → ChartTicker = null → returns to portfolio
10. Click ← back arrow → same as step 9
```

**Alternative entry point:** Click "View Chart" button in expanded watchlist row → same result as step 1.

### Flow 2: Execute Trade

```
1. User clicks "Trade" button (in chart header OR expanded watchlist row)
2. TradeStock state updates → TradeDrawer begins entrance
3. Drawer slides in from right (420px, spring easing 0.35s)
4. Backdrop fades in (25% black + 4px blur, 0.2s)
5. Drawer header shows: ticker (serif 24px) + delta badge + company name + price
6. User selects Buy/Sell via toggle pill
   - Selected side gets card shadow + colored text (green=buy, red=sell)
7. User enters quantity:
   - Type directly in NumberBox
   - OR click quick-select button (1, 5, 10, 25, 100) → fills input
8. User selects order type (Market / Limit / Stop)
   - Market: default, no additional input
   - Limit/Stop: additional price input appears with fadeUp animation
9. Order preview card appears as soon as quantity > 0:
   - Shows: action, quantity, price type, estimated total
10. Submit button changes from disabled gray → colored with dynamic text:
    - "Buy 10 AAPL · $2,476.30" (green)
    - "Sell 5 NVDA · $4,460.70" (red)
11. User clicks submit → form replaced by confirmation state:
    - Centered checkmark with spring entrance animation
    - "Order Placed" in serif
    - Summary line below
12. After 1.8s → drawer auto-closes:
    - Drawer slides out right (0.3s ease)
    - Backdrop fades out
    - TradeStock resets to null
```

**Dismiss paths:**
- Click backdrop → triggers closing animation
- Click × close button → triggers closing animation
- Both cancel the 1.8s auto-close timer if in confirmed state

### Flow 3: Search and Explore Watchlist

```
1. User clicks search bar → focus ring appears (gold border + 3px gold ring)
2. User types → SearchQuery state updates on each keystroke
3. Watchlist filters in real-time:
   - Case-insensitive match on ticker OR company name
   - No debounce needed (small dataset)
4. Clear button (×) appears when text non-empty
   - Hover: rotates 90°
   - Click: clears search, removes clear button
5. User clicks watchlist row → ExpandedTicker toggles:
   - If already expanded: collapses (ExpandedTicker = null)
   - If different row: old collapses, new expands (ExpandedTicker = new ticker)
   - Only one row open at a time
6. Expansion animation:
   - maxHeight: 0 → 180px (0.4s cubic-bezier(0.4, 0, 0.2, 1))
   - opacity: 0 → 1 (0.3s ease)
   - Chevron rotates: 0° → 180°
7. Expanded content visible:
   - 4-column OHLC grid (Open, High, Low, Volume)
   - Day range bar with positioned dot
   - Two action buttons: "View Chart" + "Trade"
8. Collapse animation: reverse of step 6
```

---

## 2.3 Component State Matrix

### Holding Card

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | Transparent bg, `#E8E4DE` border, no dot | — |
| **Hovered** | Gold border, `#FDFCFA` bg, `translateY(-2px)`, elevated shadow | `PointerEntered` |
| **Selected** | Green border, `rgba(45,106,79,0.04)` bg, green dot indicator with ring shadow | Click (toggles `ChartTicker`) |
| **Hovered + Selected** | Green border + gold shadow, slight additional lift | Both conditions met |

### Watchlist Row

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | Transparent bg, 1px bottom border `rgba(0,0,0,0.04)` | — |
| **Hovered** | `#FAF8F5` bg, `translateX(3px)` | `PointerEntered` |
| **Expanded** | `#FAF8F5` bg, detail panel visible (180px), chevron rotated 180° | Click (toggles `ExpandedTicker`) |
| **Hovered + Expanded** | Same as expanded + hover bg | Both conditions |

### Timeframe Button

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | Transparent bg, subtle color text | — |
| **Active** | Chart-colored bg tint, colored text, underline indicator (2px bar) | Click (sets `SelectedTimeframe`) |

### Chart Card

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Portfolio Mode** | "Performance" header, green chart, no back/trade buttons | `ChartTicker == null` |
| **Stock Detail Mode** | Stock name header, price + delta + Trade button, back arrow visible, chart color matches stock direction | `ChartTicker != null` |
| **Mouse Tracking** | 3D perspective tilt (max ±5°) following cursor position | `PointerMoved` on card |
| **Click** | Ripple effect: ellipse expanding from click point, 700ms | `PointerPressed` |

### Trade Drawer

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Closed** | Not in visual tree / `Visibility.Collapsed` | `TradeStock == null` |
| **Opening** | Slides from right (spring 0.35s), backdrop fades in (0.2s) | `TradeStock` set to a stock |
| **Form** | Full order form visible, submit button reflects current input | Default drawer state |
| **Confirmed** | Form replaced by checkmark animation + "Order Placed" + summary | Submit button clicked |
| **Closing** | Slides out right (0.3s ease), backdrop fades out | Close btn / backdrop click / 1.8s auto-close |

### Submit Button (Trade Drawer)

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Disabled** | Gray (`#E0DCD5`), muted text, `cursor: default` | `Quantity == 0` |
| **Enabled Buy** | Green bg (`#2D6A4F`), white text, dynamic label | `Quantity > 0`, `Side == "buy"` |
| **Enabled Sell** | Red bg (`#B5342B`), white text, dynamic label | `Quantity > 0`, `Side == "sell"` |
| **Hover (enabled)** | `Scale(1.02)` | `PointerEntered` when enabled |

### Buy/Sell Toggle

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Buy Selected** | Buy side: white bg, card shadow, green text. Sell side: transparent, subtle text | `Side == "buy"` |
| **Sell Selected** | Sell side: white bg, card shadow, red text. Buy side: transparent, subtle text | `Side == "sell"` |

### Order Type Button (×3)

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | `#E8E4DE` border, transparent bg, muted text | — |
| **Selected** | Gold border (`#C9A96E`), gold bg tint, gold text | Click (sets `OrderType`) |

### Search Bar

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | `#E8E4DE` border, white bg, placeholder text | — |
| **Focused** | Gold border + 3px gold ring shadow | Focus |
| **Has Text** | Clear button (×) visible | `SearchQuery.Length > 0` |

### Sector Ring Arc

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | All arcs at base radius, full opacity | — |
| **Hovered** | Hovered arc: expands +4px radius, +6px stroke, glow shadow. Others: fade to 35% | `PointerEntered` on arc or legend row |

### News Item

| State | Visual Change | Trigger |
|-------|--------------|---------|
| **Default** | Static position | — |
| **Hovered** | `translateX(4px)` | `PointerEntered` |

---

## 2.4 Animation Inventory

Every animation in the prototype, with duration, easing, and Uno implementation approach.

### Entrance Animations (one-shot on load)

| Animation | Duration | Easing | Stagger | Uno Approach |
|-----------|----------|--------|---------|--------------|
| Card Entrance | 0.7s | `cubic-bezier(0.34, 1.56, 0.64, 1)` | Per card via `animationDelay` | `Storyboard`: `TranslateY` 32→0, `Opacity` 0→1, `ScaleX/Y` 0.94→1 |
| Odometer Digit Roll | 1.4s per digit | `cubic-bezier(0.16, 1, 0.3, 1)` | 60ms per digit L→R | Custom control: per-digit `ScrollViewer` offset animation |
| Spring Number Count | ~1.5s settle | Custom spring (stiffness 0.04, damping 0.85) | 200ms after mount | `CompositionAnimation` with spring easing or `DispatcherTimer` manual loop |
| Sector Arc Reveal | 1.2s per arc | ease-in-out | 120ms per arc | SkiaSharp: animate `StrokeDash` progress per arc |
| Volume Bar Grow | 0.5s per bar | `cubic-bezier(0.34, 1.56, 0.64, 1)` | 22ms per bar L→R | SkiaSharp: animate bar height 0→target |
| Sparkline Draw | 1.2s | ease | 500ms delay after mount | SkiaSharp: `StrokeDash` offset animation |
| News Item FadeUp | 0.4s | ease | 60ms per item | `Storyboard`: `TranslateY` + `Opacity` |
| Weight Bar Slide | 1s | ease | 80ms per holding | `Storyboard` on `Width` or `ScaleX` |

### Interactive Animations (triggered by user action)

| Animation | Duration | Easing | Uno Approach |
|-----------|----------|--------|--------------|
| Watchlist Row Expand | 0.4s | `cubic-bezier(0.4, 0, 0.2, 1)` | `Storyboard` animating `MaxHeight` + `Opacity` |
| Watchlist Row Collapse | 0.4s | `cubic-bezier(0.4, 0, 0.2, 1)` | Reverse of expand storyboard |
| Chevron Rotate | 0.3s | ease | `Storyboard` on `RotateTransform.Angle` 0↔180 |
| Trade Drawer Slide In | 0.35s | `cubic-bezier(0.34, 1.56, 0.64, 1)` | `Storyboard`: `TranslateX` from `420` to `0` |
| Trade Drawer Slide Out | 0.3s | ease | `Storyboard`: `TranslateX` from `0` to `420`, then `Collapse` |
| Backdrop Fade In | 0.2s | ease | `Storyboard` on `Opacity` 0→1 |
| Backdrop Fade Out | 0.3s | ease | `Storyboard` on `Opacity` 1→0 |
| Limit Price Input FadeUp | 0.3s | ease | `Storyboard`: `TranslateY` 12→0 + `Opacity` 0→1 |
| Ripple Click | 0.7s | ease-out | `Storyboard`: `Ellipse` `ScaleTransform` 0→1 + `Opacity` 1→0 |
| Card 3D Tilt | continuous | 0.1s linear tracking | `CompositionAnimation`: `RotationAngleInDegrees` on `PointerMoved` |
| Submit Button Hover Scale | 0.2s | ease | `PointerEntered` → `ScaleTransform` 1→1.02 |
| Close Button Hover Rotate | 0.2s | ease | `PointerEntered` → `RotateTransform` 0→90° |
| News Item Hover Slide | 0.2s | ease | `PointerEntered` → `TranslateTransform.X` 0→4 |
| Holdings Hover Lift | 0.35s | `cubic-bezier(0.4, 0, 0.2, 1)` | `VisualState` → border, background, `TranslateY` -2, shadow |

### Continuous / Looping Animations

| Animation | Interval / Duration | Uno Approach |
|-----------|-------------------|--------------|
| Braille Ticker Scroll | 70ms step (compact: 100ms) | `DispatcherTimer`, update text `Inlines` |
| Braille Spinner Cycle | 80ms step | `DispatcherTimer`, cycle `TextBlock.Text` through 10 glyphs |
| Braille Pulse Scroll | 120ms step | `DispatcherTimer`, shift 18-char window through heartbeat pattern |
| Braille Activity Oscillation | 150ms step | `DispatcherTimer`, per-glyph `sin()` with phase offsets |
| Gain Pill Glow Pulse | 3s loop, starts at 2s | `Storyboard` `RepeatBehavior="Forever"` on shadow opacity |
| NOW Needle Breathe | 2.5s loop | `Storyboard` `RepeatBehavior="Forever"` on circle opacity + scale |
| Ambient Orb Float | 24-40s loop per orb | `Storyboard` `RepeatBehavior="Forever"` on `TranslateTransform` keyframes |
| Live Clock | 1s interval | `DispatcherTimer` updating `CurrentTime` state |

---

## 2.5 Edge Cases & Error States

| Scenario | Current Behavior | Recommendation |
|----------|-----------------|----------------|
| Search yields no results | Watchlist renders empty (no items) | Add "No matches found" empty state placeholder |
| Very long stock name | Truncated via `TextTrimming="CharacterEllipsis"` in watchlist rows | Already handled |
| Trade drawer while chart in portfolio mode | Trade button only visible in stock-detail mode — path blocked | Correct by design |
| Rapid click toggling on holdings | Simple toggle on `ChartTicker` — no race condition possible with MVUX | No issue |
| Drawer submit with quantity 0 | Button visually disabled, click handler checks `numQty > 0` | Already handled |
| Drawer close during confirmation | Closing state triggers immediately, but 1.8s timer should be cancelled | Implement `CancellationToken` for auto-close timer |
| User types in limit price then switches to Market | `LimitPrice` input hides, but value persists in state | Should reset `LimitPrice` to `null` when switching to Market |
| Window resize during 3D tilt | Tilt calculation uses relative coords (0-1 range) | Already resilient |
| ChartTicker set to ticker not in WATCHLIST | `chartStock` would be `null` | Add null guard — fall back to portfolio view |
| Multiple rapid drawer open/close | Spring animation could overlap | Gate on `isClosing` flag to prevent re-open during close animation |

---

## 2.6 Keyboard & Focus Considerations

> **Note:** The prototype only implements mouse/touch interactions. The following are recommended additions for the Uno Platform implementation.

| Context | Recommended Behavior |
|---------|---------------------|
| Search bar | `Ctrl+K` or `/` focuses search from anywhere |
| Watchlist | `↑`/`↓` arrow keys move focus, `Enter` toggles expand, `Escape` collapses |
| Trade drawer | Focus trap when open, `Tab` cycles through form fields, `Escape` closes |
| Holdings | `Enter` toggles chart link, `Tab` moves between cards |
| Timeframe buttons | `←`/`→` arrow keys cycle through options |
| Submit button | `Enter` submits when focused and enabled |
