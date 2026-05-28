# Meridian Capital Terminal — Design Brief

> **Source Prototype:** Meridian v4 (React/JSX)
> **Target Platform:** Uno Platform (C# / WinUI 3 / XAML)
> **Date:** March 18, 2026

---

## 1.1 Overall Layout Structure

The dashboard uses a single-page layout with a fixed max-width container (1400px) centered on the viewport. The background is a warm cream (`#F6F4F0`) with ambient floating gradient orbs behind the content layer. A subtle SVG noise grain texture overlays the entire viewport at low opacity (28%).

### Full Page Layout (≥1200px Desktop)

```
┌──────────────────────────────── 1400px max-width ────────────────────────────────┐
│ 32px padding                                                          32px padding│
│                                                                                   │
│  ┌─ HEADER BAR ─────────────────────────────────────────────────────────────────┐ │
│  │ Meridian                                              ⠋ NYSE Open │ Thu, Mar 18│
│  │ CAPITAL TERMINAL                                        ──── 02:34:17 PM     │ │
│  └──────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                   │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ │
│  ⣤⣴⣶⣷ AAPL 247.63 +1.40% │ ⣴⣶⣷ NVDA 892.14 −1.42% │ ⣤⣴ MSFT 468.21 +1.12%   │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ │
│                                                                                   │
│  TOTAL PORTFOLIO VALUE                               ┌─────────┬─────────┬──────┐ │
│  $163,842.56  ← odometer                             │ S&P 500 │ NASDAQ  │DOW 30│ │
│  ┌──────────────────────────────────┐                 │ 5,892   │ 18,742  │43,218│ │
│  │ ▲ +$40,274.28  (32.63%)        │  all time        │ +0.87%  │ +1.12%  │+0.34%│ │
│  └──────────────────────────────────┘  unrealized     └─────────┴─────────┴──────┘ │
│                                                                                   │
│  ┌───────────────────────────── LEFT ──────┐ 20px ┌──── RIGHT 360px ────────────┐ │
│  │                                         │  gap │                              │ │
│  │  ┌─ PERFORMANCE CHART CARD ──────────┐  │      │  ┌─ SEARCH ──────────────┐  │ │
│  │  │ Performance     1D 1W 1M [3M] YTD │  │      │  │ 🔍 Search ticker...   │  │ │
│  │  │                                    │  │      │  └───────────────────────┘  │ │
│  │  │     ╱╲    ╱╲  ╱╲   ╱╲             │  │      │                              │ │
│  │  │   ╱    ╲╱    ╲   ╲╱   ╲  ╱╲       │  │      │  ┌─ WATCHLIST ──────────┐  │ │
│  │  │ ╱                       ╲╱   ╲     │  │      │  │ AAPL ⣤⣶⣷ ▾   ~~~  $247│  │ │
│  │  │ ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░    │  │      │  │──────────────────────│  │ │
│  │  │ Jan    Feb    Mar    Apr    May     │  │      │  │ NVDA ⣴⣶⣷ ▾   ~~~  $892│  │ │
│  │  └────────────────────────────────────┘  │      │  │──────────────────────│  │ │
│  │                                         │      │  │ MSFT ⣤⣷⣿ ▾   ~~~  $468│  │ │
│  │  ┌─ HOLDINGS ──────┐ 20 ┌─ ALLOC ────┐  │      │  │──────────────────────│  │ │
│  │  │ AAPL  85×178    │ px │ SECTOR     │  │      │  │ ...more rows...      │  │ │
│  │  │ NVDA  22×480    │gap │  ╭───╮     │  │      │  └───────────────────────┘  │ │
│  │  │ MSFT  40×380    │    │  │5  │ legend│  │      │                              │ │
│  │  │ GOOGL 60×142    │    │  ╰───╯     │  │      │  ┌─ MARKET PULSE ────────┐  │ │
│  │  │ META  18×350    │    ├────────────┤  │      │  │ MARKET PULSE  ⣤⣶⣿⣶⣤⠀│  │ │
│  │  │ JPM   30×195    │    │ VOLUME     │  │      │  │ [Earn] NVDA beats...  │  │ │
│  │  │ (weight bars)   │    │ ▐▐▐▐▐▐▐▐  │  │      │  │ [Bond] Treasury...   │  │ │
│  │  └─────────────────┘    │ NOW↓       │  │      │  │ [Tech] Apple AI...   │  │ │
│  │                         └────────────┘  │      │  └───────────────────────┘  │ │
│  └─────────────────────────────────────────┘      └──────────────────────────────┘ │
│                                                                                   │
│  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ │
│  Simulated data...            ⣤⣴⣶⣷⣿ (40% opacity)        Meridian Terminal v4.0 │
│                                                                                   │
└───────────────────────────────────────────────────────────────────────────────────┘
```

### Layout Zones (top to bottom)

| Zone | Contents | Width |
|------|----------|-------|
| **Header Bar** | Logo (Instrument Serif italic) + braille spinner + "NYSE Open" label + live clock | Full width |
| **Braille Ticker Tape** | Scrolling unicode waveform with interleaved ticker symbols + prices, bordered top/bottom | Full width |
| **Portfolio Hero** | Large serif portfolio value (odometer animation) + gain pill (pulsing glow) + 3 index ticker cards | Full width, flex row |
| **Main Content Grid** | 2-column: flexible left + 360px fixed right sidebar | Full width |
| **Footer** | Disclaimer text + compact braille stream (40% opacity) + version label | Full width |

### Header Bar Detail

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│                                                                                  │
│  Meridian  CAPITAL TERMINAL              ⠋ NYSE Open  │  Thu, Mar 18 · 02:34:17 │
│  ↑serif    ↑outfit 10px                  ↑braille     │  ↑plex mono 12px         │
│  italic    120 char-spacing              spinner      │                          │
│  26px      uppercase                     80ms cycle   │  1s DispatcherTimer      │
│                                          green        │                          │
│                                                       ↑ 1px × 14px divider       │
└──────────────────────────────────────────────────────────────────────────────────┘
```

### Portfolio Hero Detail

```
┌────────────────────────────────────────────────────────────────────────────────┐
│ TOTAL PORTFOLIO VALUE  ← 11px, SemiBold, #8A8A8A, uppercase, 140 char-spacing │
│                                                                                │
│ $163,842.56  ← Instrument Serif 62px, OdometerControl (per-digit roll)        │
│                                                                                │
│ ┌─ gain pill ─────────────────────────────┐                                    │
│ │  ▲  +$40,274.28  (32.63%)              │  all time unrealized               │
│ │  ↑  ↑ Plex Mono 14px, green            │  ↑ Outfit 11px, subtle              │
│ │ icon │ Plex Mono 13px, 70% opacity     │                                    │
│ └───────────────── CornerRadius="100" ────┘                                    │
│ Background: rgba(45,106,79,0.08)                                               │
│ Glow: pulsing box-shadow, 3s loop, 2s delay                                   │
│                                                                                │
│                                   ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│                                   │ S&P 500  │ │ NASDAQ   │ │ DOW 30   │      │
│                                   │ 5,892    │ │ 18,742   │ │ 43,218   │      │
│                                   │ +0.87%   │ │ +1.12%   │ │ +0.34%   │      │
│                                   └──────────┘ └──────────┘ └──────────┘      │
│                                   ↑ Card/Border, CornerRadius=12, 120px min    │
│                                     Padding=16,10  gap=6                       │
└────────────────────────────────────────────────────────────────────────────────┘
```

### Performance Chart Card — Portfolio Mode vs Stock Detail Mode

```
PORTFOLIO MODE (ChartTicker == null)
┌──────────────────────────────────────────────────────────────────────────┐
│  Performance                                    1D  1W  1M [3M] YTD  1Y │
│  ↑ section label                                ↑ toggle buttons         │
│                                                    active = green bg tint │
│  ┌────────────────────────────────────────────────────────────────────┐  │
│  │         ╱╲       ╱╲     ╱╲                                        │  │
│  │       ╱    ╲   ╱    ╲ ╱    ╲   ╱╲                                 │  │
│  │     ╱        ╲╱        ╲      ╱    ╲                              │  │
│  │   ╱            ░░░░░░░░░░╲  ╱                                     │  │
│  │  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░                                    │  │
│  │  ↑ area gradient fill under line                                   │  │
│  │  $124k ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─                                  │  │
│  │  Jan 1        Feb 1       Mar 1      Apr 1                        │  │
│  └────────────────────────────────────────────────────────────────────┘  │
│  ↑ stroke=#2D6A4F (green), 2.5px                                        │
│  Height: 240px                                                           │
└──────────────────────────────────────────────────────────────────────────┘

STOCK DETAIL MODE (ChartTicker == "AAPL")
┌──────────────────────────────────────────────────────────────────────────┐
│  ← Apple Inc.                                   1D  1W  1M [3M] YTD  1Y │
│  ↑  ↑ company name (section label)                                       │
│ back $247.63  +1.40%  [TRADE]                                            │
│ btn  ↑mono18  ↑green  ↑ gold outline button                             │
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────────┐  │
│  │       ╱╲         ╱╲                                               │  │
│  │     ╱    ╲     ╱    ╲     ╱╲                                      │  │
│  │   ╱        ╲ ╱        ╲ ╱    ╲                                    │  │
│  │  ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░                                   │  │
│  │  $210 ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─                                   │  │
│  └────────────────────────────────────────────────────────────────────┘  │
│  ↑ stroke=green for +pct, red for -pct                                   │
│  Height: 210px (shorter to accommodate header row)                       │
└──────────────────────────────────────────────────────────────────────────┘
```

### Holding Card Row (inside Holdings Card)

```
DEFAULT STATE
┌──────────────────────────────────────────────────────────────────┐
│  AAPL        85 × $178.40                          $21,048      │
│  ↑ 14px bold ↑ 10px subtle                     ↑ Plex Mono 13px │
│                                                    +38.8%       │
│                                                    ↑ green 11px  │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░  ← weight bar 3px, green fill    │
│  ↑ animated width = portfolio weight %                           │
│                                                                  │
│  Border: 1px #E8E4DE  │  CornerRadius: 12  │  Padding: 12,14    │
└──────────────────────────────────────────────────────────────────┘

SELECTED STATE (driving chart)
┌──────────────────────────────────────────────────────────────────┐
│  ● AAPL      85 × $178.40                          $21,048      │
│  ↑ 6px green dot with 3px ring shadow                +38.8%     │
│                                                                  │
│  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓░░░░░░░░░                                   │
│                                                                  │
│  Border: 1px #2D6A4F  │  Background: rgba(45,106,79,0.04)       │
└──────────────────────────────────────────────────────────────────┘
```

### Sector Allocation Ring

```
┌─────────────────────────────────────────────────────────────────┐
│  SECTOR ALLOCATION                                               │
│                                                                  │
│       ╭━━━━━━━━━━━━━╮                                            │
│      ╱    ╭━━━━━╮    ╲        ■ Technology ........... 68.2%     │
│     ╱     │  5  │     ╲       ■ Consumer Disc. ....... 14.5%     │
│    ╱      │SECT.│      ╲      ■ Financials ............ 9.8%     │
│     ╲     ╰━━━━━╯     ╱      ■ Healthcare ............ 4.8%     │
│      ╲               ╱       ■ Energy ................. 2.7%     │
│       ╰━━━━━━━━━━━━━╯         ↑ swatches rotate 45° on hover    │
│                                                                  │
│  ↑ 180×180 SVG                ↑ Legend: synced hover with arcs   │
│    StrokeWidth=22                                                │
│    Arcs reveal staggered 120ms                                   │
│    Hover: expand +4px, glow, others fade 35%                     │
│    Center: morphs to hovered sector name + %                     │
└─────────────────────────────────────────────────────────────────┘
```

### Volume Profile

```
┌─────────────────────────────────────────────────────────────────┐
│  VOLUME PROFILE                                                  │
│                                                                  │
│  ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ← 75% gridline │
│              ┌───── market hours zone ─────┐                     │
│  ─ ─ ─ ─ ─ ─│─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─│─ ─ ← 50% gridline │
│        ╱╲   │        ╱ ╲       ╱╲          │                     │
│       ╱  ╲  │      ╱     ╲   ╱    ╲  ┊     │                     │
│     ╱     ╲ │    ╱        ╲╱       ╲ ┊     │← envelope line      │
│  ─ ── ─ ─ ─│─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ┊─ ─ ─│← 25% gridline      │
│   ▐ ▐ ▐ ▐  │▐▐ ▐▐▐ ▐▐▐ ▐▐▐▐ ▐▐ ▐▐  ┊▐ ▐  │                     │
│   ▐ ▐ ▐ ▐  │▐▐ ▐▐▐ ▐▐▐ ▐▐▐▐ ▐▐ ▐▐  ┊▐ ▐  │← pill bars (rx=50%)│
│   ↑ gray    │↑ gold gradient (>55M)  ┊     │                     │
│             └────────────────────────┊─────┘                     │
│                               NOW →  ┊ ← red dashed + breathing  │
│  0:00    6:00   9:30AM─────4:00PM  18:00  23:00                  │
│                                                                  │
│  Hover: crosshair + dot on envelope + [14:00 · 72M] tooltip     │
│  Non-hovered bars → 30% opacity                                  │
└─────────────────────────────────────────────────────────────────┘
```

### Watchlist Row — Collapsed & Expanded

```
COLLAPSED
┌──────────────────────────────────────────────────────────────────┐
│  AAPL ⣤⣶⣷⣿⣶⣤ ▾    Apple Inc.       ~~~sparkline~~~    $247.63  │
│  ↑14px ↑braille  ↑8px   ↑11px subtle    ↑72×30 SVG      +1.40%  │
│   bold  activity chevron                  green           ↑green  │
│         intensity=|pct|                                           │
│                                                                  │
│  Row: padding 14px 22px, bottom border 1px rgba(0,0,0,0.04)     │
│  Hover: translateX(3px), bg=#FAF8F5                              │
└──────────────────────────────────────────────────────────────────┘

EXPANDED (maxHeight 0 → 180px, 0.4s cubic-bezier)
┌──────────────────────────────────────────────────────────────────┐
│  AAPL ⣤⣶⣷⣿⣶⣤ ▴    Apple Inc.       ~~~sparkline~~~    $247.63  │
│                                                           +1.40%  │
│  ┌────────────────────────────────────────────────────────────┐  │
│  │  bg: #F8F6F2                                               │  │
│  │                                                            │  │
│  │   OPEN        HIGH        LOW         VOLUME               │  │
│  │   $244.88     $251.20     $244.18     62.1M                │  │
│  │   ↑9px label  ↑13px mono  ↑13px mono  ↑13px mono           │  │
│  │                                                            │  │
│  │   Day Range                            $244.18 — $251.20   │  │
│  │   ░░░░░░░░░░░░░░░●░░░░░░░░░░░░░  ← 4px track, dot=price  │  │
│  │                   ↑ positioned dot (green/red)              │  │
│  │                                                            │  │
│  │   ┌─ View Chart ──────┐  ┌─── Trade ─────────┐            │  │
│  │   │ 📈 View Chart     │  │     Trade          │            │  │
│  │   │ border, subtle     │  │  gold border/bg    │            │  │
│  │   └────────────────────┘  └────────────────────┘            │  │
│  └────────────────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────────────────┘
```

### Trade Drawer (Overlay)

```
┌─────────── full viewport ──────────────────────────┬──── 420px ────┐
│                                                     │               │
│              backdrop                               │  AAPL  +1.40% │
│              25% black + 4px blur                   │  Apple Inc.   │
│              click to dismiss                       │  $247.63   [×]│
│                                                     │───────────────│
│                                                     │               │
│                                                     │ [  BUY  |sell]│
│                                                     │               │
│                                                     │ SHARES        │
│                                                     │ ┌───────────┐ │
│                                                     │ │    10     │ │
│                                                     │ └───────────┘ │
│                                                     │ [1][5][10][25]│
│                                                     │         [100] │
│                                                     │               │
│                                                     │ ORDER TYPE    │
│                                                     │ [Market][Limit│
│                                                     │        ][Stop]│
│                                                     │               │
│                                                     │ ┌─ PREVIEW ─┐ │
│                                                     │ │Buy AAPL   │ │
│                                                     │ │10 shares  │ │
│                                                     │ │~$247.63   │ │
│                                                     │ │───────────│ │
│                                                     │ │Est. Total │ │
│                                                     │ │ $2,476.30 │ │
│                                                     │ └───────────┘ │
│                                                     │               │
│                                                     │ ┌───────────┐ │
│                                                     │ │Buy 10 AAPL│ │
│                                                     │ │ $2,476.30 │ │
│                                                     │ └───────────┘ │
│                                                     │ ↑ green/red   │
│                                                     │   full-width  │
└─────────────────────────────────────────────────────┴───────────────┘

CONFIRMATION STATE (replaces form, auto-closes 1.8s)
                                                     ┌───────────────┐
                                                     │               │
                                                     │       ✓       │
                                                     │  Order Placed │
                                                     │               │
                                                     │  Buy 10 AAPL  │
                                                     │  @ Market     │
                                                     │               │
                                                     └───────────────┘
```

### Footer

```
┌──────────────────────────────────────────────────────────────────────────────────┐
│ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ │
│  Simulated data for          ⣤⣴⣶⣷⣿⣶⣴⣤ AAPL ⣤⣴⣶      Meridian Terminal v4.0  │
│  demonstration...            ↑ compact braille 40%              ↑ Plex Mono 10px │
│  ↑ Outfit 10px, subtle         max-width 300px                    subtle         │
│                                                                                  │
│  16px top padding, border-top: 1px #E8E4DE                                       │
└──────────────────────────────────────────────────────────────────────────────────┘
```

### Responsive Breakpoint Layouts

```
≥1200px (Wide Desktop) — as prototyped above

900–1199px (Narrow Desktop)
┌─────────────────────────────────────────────┐
│  HEADER                                      │
│  TICKER TAPE                                 │
│  PORTFOLIO HERO (value + index cards wrap)   │
│  ┌─ CHART CARD ───────────────────────────┐  │
│  └────────────────────────────────────────┘  │
│  ┌─ HOLDINGS ──────┐  ┌─ ALLOC + VOL ────┐  │
│  └─────────────────┘  └──────────────────┘  │
│  ┌─ SEARCH ──────────────────────────────┐  │
│  ┌─ WATCHLIST ───────────────────────────┐  │
│  └───────────────────────────────────────┘  │
│  ┌─ MARKET PULSE ────────────────────────┐  │
│  └───────────────────────────────────────┘  │
│  FOOTER                                      │
└─────────────────────────────────────────────┘

600–899px (Tablet)
┌───────────────────────────────┐
│  HEADER                        │
│  TICKER TAPE                   │
│  PORTFOLIO HERO (stacked)     │
│  ┌─ CHART ──────────────────┐ │
│  └──────────────────────────┘ │
│  ┌─ HOLDINGS ───────────────┐ │
│  └──────────────────────────┘ │
│  ┌─ ALLOCATION ─────────────┐ │
│  └──────────────────────────┘ │
│  ┌─ VOLUME ─────────────────┐ │
│  └──────────────────────────┘ │
│  ┌─ SEARCH + WATCHLIST ─────┐ │
│  └──────────────────────────┘ │
│  ┌─ NEWS ───────────────────┐ │
│  └──────────────────────────┘ │
│  FOOTER                        │
└───────────────────────────────┘

<600px (Mobile)
┌───────────────────┐
│  HEADER (compact)  │
│  (no ticker tape)  │
│  PORTFOLIO VALUE   │
│  (no index cards)  │
│  ┌─ CHART ──────┐ │
│  └──────────────┘ │
│  ┌─ HOLDINGS ───┐ │
│  └──────────────┘ │
│  ┌─ WATCHLIST ──┐ │
│  └──────────────┘ │
│  ┌─ NEWS ───────┐ │
│  └──────────────┘ │
│  FOOTER (compact)  │
└───────────────────┘
Trade drawer → full screen
```

### Main Content Grid Breakdown

**Left Column:**
- Performance Chart Card (full width of left column)
- Holdings + Allocation/Volume Row (2-column sub-grid, equal width)

**Right Sidebar (360px fixed):**
- Search Bar
- Watchlist Card (scrollable, max-height 440px)
- Market Pulse (News) Card

---

## 1.2 Visual Hierarchy

The visual hierarchy follows a clear 4-tier information architecture:

| Tier | Elements | Typography | Treatment |
|------|----------|------------|-----------|
| **1 (Hero)** | Portfolio value, stock price in chart header | Instrument Serif, 62px / 20px | Odometer roll animation, largest element on page |
| **2 (Key Data)** | Chart area, gain pill, watchlist prices | IBM Plex Mono 14px / Outfit 14px | Mono for numbers, green/red semantic color |
| **3 (Supporting)** | Holdings, sectors, volume, news, OHLC data | Outfit 12-13px, Plex Mono 11-13px | Cards with grain texture, hover interactions |
| **4 (Ambient)** | Labels, timestamps, braille streams, footer | Outfit 10-11px, Plex Mono 8-10px | Muted/subtle color, uppercase tracking |

---

## 1.3 Color System

The prototype uses a bespoke editorial palette. These must be mapped to Uno Material color overrides via `ColorPaletteOverride.xaml`.

| Token Name | Hex | Usage | Uno Material Mapping |
|------------|-----|-------|---------------------|
| Background | `#F6F4F0` | Page background | Background / Surface |
| Card | `#FFFFFF` | Card surfaces | SurfaceVariant / Surface |
| Text Primary | `#1A1A2E` | Headlines, body text | OnSurface |
| Text Muted | `#8A8A8A` | Labels, metadata | OnSurfaceVariant |
| Text Subtle | `#C4C0B8` | Timestamps, axis labels, dividers | Outline |
| Border | `#E8E4DE` | Card borders, row separators | OutlineVariant |
| Gain (semantic) | `#2D6A4F` | Positive values, primary accent, chart stroke | Primary / Custom semantic brush |
| Loss (semantic) | `#B5342B` | Negative values, red chart stroke | Error / Custom semantic brush |
| Accent Gold | `#C9A96E` | CTAs, trade buttons, volume bar peaks, highlights | Tertiary / Custom semantic brush |

### Derived Colors

| Token | Value | Usage |
|-------|-------|-------|
| Card Hover | `#FAF8F5` | Watchlist row hover, expanded row bg |
| Expanded Bg | `#F8F6F2` | Watchlist expanded panel background |
| Gain Bg Tint | `#0D2D6A4F` | Gain pill background (8% opacity green) |
| Loss Bg Tint | `#0DB5342B` | Loss badge backgrounds |
| Accent Bg Tint | `#0DC9A96E` | Trade button background, order type selected |
| Selected Holding Bg | `rgba(45,106,79,0.04)` | Holding card when driving the chart |

---

## 1.4 Typography System

Three font families are used with distinct roles. In Uno, these map to custom `FontFamily` resources or Material type scale overrides.

| Font Family | Role | Sizes Used | Uno Approach |
|-------------|------|-----------|--------------|
| **Instrument Serif** | Hero display values, center text in sector ring, trade drawer stock label | 62px, 24px, 22px, 20px, 16px | Custom `FontFamily` resource; `DisplayLarge` / `HeadlineLarge` override |
| **Outfit** | UI labels, body text, buttons, section headers | 16px, 14px, 13px, 12px, 11px, 10px, 9px | Default app font; maps to Material Body/Label styles |
| **IBM Plex Mono** | All financial numbers, timestamps, ticker symbols, code-like elements | 20px, 18px, 14px, 13px, 12px, 11px, 10px, 9px, 8px, 7px | Custom `FontFamily` resource; explicit `TextBlock` style |

**Section label pattern (used in every card):** `FontSize="11"`, `FontWeight="SemiBold"`, `Foreground=MeridianTextMuted`, `CharacterSpacing="120"`, `TextTransform="Uppercase"`.

> **Assumption:** Instrument Serif and IBM Plex Mono are bundled as `.ttf` assets. Outfit is the default app font set via Material theme customization.

---

## 1.5 Component Inventory

Complete inventory of every UI component in the prototype, with recommended Uno control mapping.

### Header Zone

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| App Logo | "Meridian" in Instrument Serif italic 26px | `TextBlock` with `FontFamily=InstrumentSerifItalic` |
| Subtitle | "CAPITAL TERMINAL" Outfit 10px, 120 char spacing | `TextBlock` |
| Braille Spinner | Cycling braille character at 80ms interval (⠋⠙⠹⠸⠼⠴⠦⠧⠇⠏) in green | Custom `UserControl` with `DispatcherTimer` |
| Market Status | "NYSE Open" text label | `TextBlock` |
| Divider | 1px × 14px vertical line | `Border` |
| Live Clock | HH:MM:SS updated every 1s in Plex Mono | `TextBlock` with `DispatcherTimer` |

### Braille Ticker Tape

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Tape Container | Full-width, `Border` top/bottom, 8px vertical padding | `Border` with `BorderThickness="0,1"` |
| Braille Stream | Pre-built tape array: braille height glyphs (⠀⣀⣤⣴⣶⣷⣿) interspersed with ticker blocks | Custom `UserControl` |
| Ticker Block | Bold ticker symbol + mono price + colored delta% + pipe separator | `TextBlock.Inlines` with `Run` elements |
| Edge Fade | Gradient transparency on left/right edges | `OpacityMask` with `LinearGradientBrush` |
| Compact variant | Smaller fonts (8-9px), shorter braille chunks, used in footer | Same control with `IsCompact="True"` |

### Portfolio Hero Section

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Section Label | "TOTAL PORTFOLIO VALUE" uppercase tracking | `TextBlock` (section label pattern) |
| Portfolio Value | $163,842.56 with per-digit rolling odometer animation | Custom `OdometerControl` |
| Gain Pill | Green rounded capsule with ↑ arrow + gain amount + percentage | `Border` `CornerRadius="100"` with inner `StackPanel` |
| Gain Pill Glow | Pulsing box-shadow ring on 3s loop, starts after 2s | `Storyboard` animating `ThemeShadow` or opacity on outer border |
| "all time unrealized" label | Static text beside gain pill | `TextBlock` |
| Index Cards (×3) | S&P 500, NASDAQ, DOW 30 with name + value + colored delta | `Card` (`ElevatedCardStyle`) or `Border` in `ItemsRepeater` |

### Performance Chart Card

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Card Container | `CornerRadius="16"`, white bg, grain texture, 3D tilt on hover | `Border` with `CompositionAnimation` for tilt |
| Click Ripple | Animated ellipse expanding from click point, 700ms | Custom `Storyboard` on `Ellipse` at pointer coords |
| Back Button | ← chevron, 28×28, visible only when stock selected | `Button` with `Visibility` bound to `ChartTicker != null` |
| Chart Label | "Performance" (portfolio) or stock name (stock detail) | `TextBlock` bound to computed `ChartLabel` |
| Stock Price | Mono 18px, visible only in stock-detail mode | `TextBlock` |
| Stock Delta | Colored percentage, visible only in stock-detail mode | `TextBlock` |
| Trade Button (inline) | "TRADE" gold outline button, visible in stock-detail mode | `Button` bound to `OpenTrade` command |
| Timeframe Selector | 6 toggle buttons (1D/1W/1M/3M/YTD/1Y), underline on active | `ItemsRepeater` with `RadioButton` styled or `ToggleButton` group |
| Area Chart | 90-day line + gradient fill, tooltip on hover | `SKXamlCanvas` custom or LiveCharts2 `CartesianChart` |

### Holdings Card

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Section Label | "HOLDINGS" uppercase tracking | `TextBlock` (section label pattern) |
| Holding Row (×6) | Clickable card: ticker + shares×avg + market value + gain% + weight bar | `DataTemplate` in `ItemsRepeater` |
| Selected Indicator | Green 6px dot with 3px ring shadow, visible when holding drives chart | `Ellipse` bound to `ChartTicker == Ticker` |
| Weight Bar | Thin bar (3px) showing portfolio weight as percentage width, colored | `ProgressBar` or `Border` with animated `Width` |
| States | Default (transparent) → Hovered (gold border, lift, shadow) → Selected (green border, tint, dot) | `VisualStateManager` |

### Sector Allocation Ring

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Donut Chart | 5 arc segments with rounded caps, staggered draw-in animation | `SKXamlCanvas` custom drawing |
| Arc Hover | Segment expands outward + glow, others fade to 35% | SkiaSharp pointer hit-testing |
| Center Text | "5 SECTORS" default, morphs to hovered sector name + percentage | `SKXamlCanvas` text drawing |
| Legend (×5) | Color swatch (rotates 45° on hover) + name + percentage | `ItemsRepeater` or inline `StackPanel` items |
| Synced Hover | Hovering legend highlights corresponding arc, and vice versa | Shared hover index state |

### Volume Profile

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Combined Visualization | 24 pill bars + Catmull-Rom envelope line + area gradient + grid lines | `SKXamlCanvas` custom drawing |
| Market Hours Zone | Shaded rectangle (9:00-16:00) with "9:30 AM — 4:00 PM" label | SkiaSharp `DrawRect` |
| NOW Needle | Dashed red vertical line + breathing dot at current time position | SkiaSharp line + circle with opacity animation |
| Crosshair Hover | Vertical hairline + dot on envelope + tooltip with hour + volume | SkiaSharp pointer tracking |
| Bar Animation | Staggered grow from 0 with spring overshoot, 22ms per bar | SkiaSharp timed animation loop |

### Search Bar

```
DEFAULT
┌──────────────────────────────────────────────────────────────────┐
│  🔍  Search ticker or name…                                      │
│  ↑   ↑ placeholder, Outfit 13px                                  │
│ icon                                                             │
│                                                                  │
│  Border: 1px #E8E4DE  │  CornerRadius: 12  │  Background: white  │
└──────────────────────────────────────────────────────────────────┘

FOCUSED
┌──────────────────────────────────────────────────────────────────┐
│  🔍  AAPL|                                                    ×  │
│       ↑ typed text, cursor                                   ↑   │
│                                                          clear   │
│                                                     rotates 90°  │
│  Border: 1px #C9A96E + 3px gold ring                  on hover   │
└──────────────────────────────────────────────────────────────────┘
```

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Input | Placeholder "Search ticker or name…", mono-spaced clear button | `TextBox` with `CornerRadius="12"` |
| Focus Ring | Gold border + 3px gold ring on focus | `FocusVisualSecondaryBrush` or `VisualState` |
| Clear Button | × that rotates 90° on hover | Button with `RenderTransform` rotation |
| Filter Logic | Case-insensitive match on ticker or company name in real-time | Computed filtered feed in MVUX model |

### Watchlist

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Container | Scrollable, max-height 440px | `ListView` or `ScrollViewer` + `ItemsRepeater` |
| Row (×8) | Ticker + BrailleActivity + chevron + name + sparkline + price + delta% | `DataTemplate` |
| Braille Activity | 6 oscillating braille chars, intensity = |price change| | Custom `UserControl` with `DispatcherTimer` |
| Chevron | 8×8 SVG arrow, rotates 180° when expanded | `FontIcon` with `RotateTransform` |
| Sparkline | 24-point mini line chart, colored green/red, stroke-draw animation | Custom `SparklineControl` (`SKXamlCanvas`) |
| Expanded Panel | Animated open (0→180px height), contains OHLC + range + buttons | `Border` with `Storyboard` on `MaxHeight` + `Opacity` |
| OHLC Grid | 4-column: Open, High, Low, Volume | `Grid` 4 columns |
| Day Range Bar | 4px track with positioned dot showing current price relative to range | `Border` track + `Ellipse` with computed `Margin` |
| View Chart Button | Opens stock in main chart | `Button` → `SelectHolding` command |
| Trade Button | Opens trade drawer | `Button` → `OpenTrade` command |
| Hover State | `translateX(3px)` + `#FAF8F5` background | `VisualState` on `PointerEntered` |

### Market Pulse (News Feed)

```
┌──────────────────────────────────────────────────────────────────┐
│  MARKET PULSE                            ⠀⣀⣠⣴⣶⣿⣶⣴⣠⣀⠀⠀⠀⠀  │
│  ↑ section label                         ↑ BraillePulse 50%     │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────────┐ │
│  │ [Macro]  Fed signals potential rate adjustment in Q2...     │ │
│  │ ↑ tag     2m ago                                            │ │
│  │ gold bg   ↑ Plex Mono 10px, subtle                         │ │
│  ├─────────────────────────────────────────────────────────────┤ │
│  │ [Earnings]  NVIDIA beats earnings expectations, raises...  │ │
│  │ ↑ green bg   18m ago                                        │ │
│  ├─────────────────────────────────────────────────────────────┤ │
│  │ [Bonds]  Treasury yields climb as inflation data...        │ │
│  │ ↑ red bg    34m ago                                         │ │
│  ├─────────────────────────────────────────────────────────────┤ │
│  │ [Tech]  Apple announces expanded AI features...            │ │
│  │ ↑ gold bg   1h ago                                          │ │
│  └─────────────────────────────────────────────────────────────┘ │
│                                                                  │
│  Tag: 9px SemiBold, CornerRadius=6, 3px 8px padding             │
│  Headline: 12px, lineHeight=17                                   │
│  Hover: translateX(4px) per row                                  │
└──────────────────────────────────────────────────────────────────┘
```

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Header | "MARKET PULSE" + BraillePulse animation (right-aligned) | `Grid` with `TextBlock` + `BraillePulseControl` |
| Braille Pulse | 18-char scrolling heartbeat pattern in gold at 50% opacity | Custom `UserControl` with `DispatcherTimer` |
| News Item (×4) | Colored tag badge + headline + timestamp | `DataTemplate` in `ItemsRepeater` |
| Tag Badge | Rounded rect, color varies: Earnings=green, Bonds=red, Macro/Tech=gold | `Border` with `CornerRadius="6"` |
| Hover | `translateX(4px)` slide | `VisualState` |

### Trade Drawer (Overlay)

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Backdrop | Full-screen 25% black + 4px blur, click to dismiss | `Grid` with `AcrylicBrush` or semi-transparent `Background` |
| Drawer Panel | 420px wide, slides from right edge | `DrawerFlyoutPresenter` (Right) or custom `Popup` |
| Header | Stock ticker (serif 24px) + delta badge + name + price + close button | `Grid` |
| Close Button | 32×32 rounded, × glyph | `Button` with `CornerRadius="8"` |
| Buy/Sell Toggle | 2-segment pill, selected side gets card shadow + colored text | Two `RadioButton` styled as toggle pills |
| Shares Input | Full-width number input with gold focus ring | `NumberBox` or `TextBox` with input filter |
| Quick Select (×5) | Buttons for 1, 5, 10, 25, 100 | `ItemsRepeater` horizontal with `Button` items |
| Order Type Selector | 3-button grid: Market / Limit / Stop | Three `RadioButton` styled as cards |
| Limit/Stop Price Input | Conditional, appears with fadeUp when non-market selected | `TextBox` with `Visibility` bound + entrance `Storyboard` |
| Order Preview | Summary card: action + quantity + price + estimated total | `Border` with inner `StackPanel` |
| Submit Button | Full-width, disabled/enabled states, dynamic text label | `Button` with computed `Content` and `IsEnabled` |
| Confirmation State | Centered checkmark animation + "Order Placed" + summary | `StackPanel` shown via `Visibility` toggle |

### Ambient Background Layer

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Orbs (×4) | Large radial gradient ellipses (200-350px), slow infinite drift paths | `Ellipse` with `RadialGradientBrush` + `Storyboard` `TranslateTransform` |
| Grain Overlay | SVG noise texture, fixed position, 28% opacity | `Image` or `Canvas` with generated noise, `IsHitTestVisible="False"` |

### Footer

| Component | Description | Uno Control |
|-----------|-------------|-------------|
| Disclaimer | "Simulated data..." 10px subtle text | `TextBlock` |
| Compact Ticker | Braille stream at 40% opacity, max-width 300px | `BrailleTickerControl IsCompact="True"` |
| Version | "Meridian Terminal v4.0" Plex Mono | `TextBlock` |

---

## 1.6 Spacing & Sizing

| Property | Value |
|----------|-------|
| Page padding | `32px` left/right, `20px` top/bottom |
| Card `CornerRadius` | `16` |
| Card inner padding | `20-28px` |
| Grid gap (between all cards) | `20px` |
| Main grid columns | `1fr` + `20px` gap + `360px` fixed |
| Holdings sub-grid | `1fr` + `20px` gap + `1fr` |
| Card `BorderThickness` | `1px` |
| Card border color | `#E8E4DE` |
| Card hover shadow | `0 12px 40px rgba(0,0,0,0.06)` → `ThemeShadow` with `Translation="0,0,32"` |
| Section label pattern | `11px`, `SemiBold`, `CharacterSpacing="120"`, uppercase, `#8A8A8A` |
| Row separator | `1px solid rgba(0,0,0,0.04)` |
| Spacing scale | `4, 6, 8, 10, 12, 14, 16, 20, 22, 24, 28, 32` (4px base grid) |

---

## 1.7 Responsive / Adaptive Guidance

The prototype is desktop-optimized. See ASCII layout diagrams in Section 1.1 for visual breakpoint layouts. For Uno Platform cross-platform deployment, apply `Responsive` markup extension breakpoints:

| Breakpoint | Layout | Key Changes |
|-----------|--------|-------------|
| **≥1200px** (Wide Desktop) | Full 2-column layout as prototyped | No changes |
| **900–1199px** (Narrow Desktop) | Right sidebar collapses below main content | Sidebar becomes full-width below chart/holdings row |
| **600–899px** (Tablet) | Full single column stack | Holdings, allocation, volume each get own full row |
| **<600px** (Mobile) | Compact single column | Ticker tape hidden, index cards hidden, trade drawer full-screen, allocation/volume may be hidden |

Use Uno Toolkit `ResponsiveView` or `Responsive` markup extension on `Visibility` properties to toggle layout regions.

---

## 1.8 Ambiguities & Missing Visual Details

- Exact grain texture density and blend mode not specified — implementation should test opacity 20-30% range
- No dark mode design exists — cream light theme is the only target for v1
- Font weight for Outfit variable font not pinned to specific named weights beyond what's listed
- Icon set not specified — prototype uses inline SVG; Uno implementation should use `FontIcon` (Fluent/Segoe) or bundled SVG assets
- No loading skeleton or shimmer state is designed for initial data fetch
- The 3D tilt effect magnitude (5° max) may need tuning per platform
