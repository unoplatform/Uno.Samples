# Meridian Stock Detail — Design Brief

> **Page:** Stock Detail (Page 2)  
> **Purpose:** The research surface a user visits before deciding to trade — a deep dive on a single ticker.  
> **Entry:** Explicit "View Details" button only (from dashboard watchlist or chart header). The dashboard's chart-swap is preserved for quick glances; this page is the deliberate escalation to full research.  
> **Design System:** Meridian (editorial luxury — warm cream, Instrument Serif, forest green + gold accents)  
> **Source Prototype:** `stock-detail-v1.jsx`

---

## 1. Page Purpose & UX Model

The Stock Detail page answers the question the dashboard can't: **"Should I trade this stock?"** The dashboard shows *what you own*. This page shows *whether a specific stock deserves your attention*.

The information hierarchy follows a real investor's evaluation flow: identity → price → trend → context → position → fundamentals → signal. Every card on the page serves one of these layers. The user enters with a ticker in mind, scans the price and chart, checks the fundamentals and analyst sentiment, evaluates their existing position, reads the news, and either trades or navigates back.

---

## 2. Full Page Layout (≥1200px Desktop)

```
┌────────────────────────────── 1400px max-width ───────────────────────────────────┐
│ 32px horizontal padding                                           20px top padding │
│                                                                                    │
│  ┌─ HEADER BAR ──────────────────────────────────────────────────────────────────┐ │
│  │ ← Back to Dashboard    AAPL · Apple Inc.               [TRADE]  [★ Watchlist]│ │
│  │ border-bottom: 1px solid #E8E4DE                         gold     toggle      │ │
│  │ paddingBottom: 20px, marginBottom: 24px                                       │ │
│  └───────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                    │
│  ┌─ PRICE HERO ──────────────────────────────────────────────────────────────────┐ │
│  │ $247.63    [▲ +$3.42 (+1.40%)]  today                                         │ │
│  │ serif 48px   gain pill (glow)   subtle                                         │ │
│  │                                                                                │ │
│  │ Open $244.88  ·  High $251.20  ·  Low $244.18  ·  Vol 62.1M  ·  Mkt Cap 3.82T│ │
│  │ marginBottom: 28px                                                             │ │
│  └────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                    │
│  ┌──────────────── LEFT (flex) ──────────────┐  20  ┌── RIGHT (340px) ──────────┐ │
│  │                                            │  gap │                            │ │
│  │  ┌─ ADVANCED CHART CARD ────────────────┐ │      │  ┌─ KEY STATISTICS ──────┐ │ │
│  │  │ [1D 1W 1M 3M 6M YTD 1Y 5Y ALL]      │ │      │  │ 12 stat rows          │ │ │
│  │  │          flex:1                       │ │      │  │ alternating bg         │ │ │
│  │  │ ╱╲  ╱╲  area chart (320px)           │ │      │  └────────────────────────┘ │ │
│  │  │ ░░░░░░░░ gradient fill               │ │      │                            │ │
│  │  │ ▐▐▐▐▐▐▐▐ volume sub-chart (50px)    │ │      │  ┌─ ANALYST CONSENSUS ──┐ │ │
│  │  │ [Line] [Area] chart type selector    │ │      │  │ rating bar + counts   │ │ │
│  │  └──────────────────────────────────────┘ │      │  │ price target range    │ │ │
│  │                                            │      │  └────────────────────────┘ │ │
│  │  ┌─ ABOUT CARD ────────────────────────┐ │      │                            │ │
│  │  │ description (truncated / expandable) │ │      │  ┌─ RELATED NEWS ────────┐ │ │
│  │  │ metadata grid + external links       │ │      │  │ 4 news items           │ │ │
│  │  └──────────────────────────────────────┘ │      │  │ filtered to ticker     │ │ │
│  │                                            │      │  └────────────────────────┘ │ │
│  │  ┌─ YOUR POSITION ─────────────────────┐ │      │                            │ │
│  │  │ shares + avg cost                    │ │      │  ┌─ SIMILAR HOLDINGS ────┐ │ │
│  │  │ 3 mini-stat cards + weight bar       │ │      │  │ same-sector tickers   │ │ │
│  │  │ [TRADE] [SELL ALL] buttons           │ │      │  │ from user portfolio   │ │ │
│  │  └──────────────────────────────────────┘ │      │  └────────────────────────┘ │ │
│  │                                            │      │                            │ │
│  │  ┌─ FINANCIALS ────────────────────────┐ │      │                            │ │
│  │  │ [Annual|Quarterly] toggle            │ │      │                            │ │
│  │  │ 4-year data table + revenue bar chart│ │      │                            │ │
│  │  └──────────────────────────────────────┘ │      │                            │ │
│  └────────────────────────────────────────────┘      └────────────────────────────┘ │
│                                                                                    │
│  ┌─ FOOTER ──────────────────────────────────────────────────────────────────────┐ │
│  │ ← Back to Dashboard                                  Meridian Terminal v4.0   │ │
│  │ border-top: 1px solid #E8E4DE, marginTop: 28px, padding: 16px 0              │ │
│  └───────────────────────────────────────────────────────────────────────────────┘ │
└────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Color System

Inherits the full Meridian palette. No new colors introduced.

| Token | Hex | Usage on this page |
|-------|-----|-------------------|
| Background | `#F6F4F0` | Page background |
| Card/Surface | `#FFFFFF` | All card backgrounds |
| Surface Hover | `#FAF8F5` | Stat row alternating bg, similar-holding row hover |
| Surface Muted | `#F8F6F2` | Close button hover bg |
| Surface Tinted | `#F0ECE6` | Weight bar track, price target track, quick-select selected bg |
| Text Primary | `#1A1A2E` | Headlines, ticker symbol, stat values |
| Text Muted | `#8A8A8A` | Labels, metadata keys, section headers, timestamps |
| Text Subtle | `#C4C0B8` | OHLC label prefix, dot separators, "today" label, axis labels |
| Border | `#E8E4DE` | Card borders, input borders, dividers, stat row borders |
| Gain | `#2D6A4F` | Positive values, chart stroke, active timeframe, analyst buy, weight bar fill |
| Loss | `#B5342B` | Negative values, sell button, analyst sell count |
| Accent Gold | `#C9A96E` | Trade button, accent links, watchlist toggle active, gold volume bars, focus ring |

---

## 4. Typography

| Token | Family | Size | Weight | Usage |
|-------|--------|------|--------|-------|
| Price Hero | Instrument Serif | 48px | 400 | Current stock price |
| Drawer Ticker | Instrument Serif | 24px | 400 | Trade drawer stock symbol |
| Confirmation Title | Instrument Serif | 20px | 400 | "Order Placed" |
| Header Ticker | Instrument Serif | 20px | 400 | Page header "AAPL" |
| Serif Italic Footer | Instrument Serif Italic | 18px | 400 | "Meridian" wordmark (at 14px in page footer) |
| Preview Total | Instrument Serif | 16px | 400 | Order preview estimated total |
| Header Name | Outfit | 14px | 400 | "Apple Inc." in header |
| Body | Outfit | 13px | 400 | Description text, metadata values, stat labels |
| Sub-body | Outfit | 12px | 400-600 | News headlines, about metadata labels, button text |
| Label | Outfit | 11px | 500-600 | "today" label, back button text, footer back |
| Section Label | Mono | 8–9.5px | 600 | Card headers: "PERFORMANCE", "STATISTICS", etc. |
| Stat Value | Mono | 13px | 500 | Stat values, OHLC values |
| Shares Display | Mono | 16px | 600 | "85 shares" |
| Mini-Stat Value | Mono | 18px | 600 | Market value, unrealized gain, return % |
| Change Value | Mono | 14px | 500 | Gain pill value (+$3.42) |
| Price Axis | Mono | 10px | 400 | Chart Y-axis labels, X-axis dates |
| Timestamp | Mono | 10px | 400 | News item timestamps |
| Tag | Mono | 9px | 600 | News tag badges |

**Section label pattern (every card):** `font-family: var(--mono)`, `font-size: 8–9.5px`, `font-weight: 600`, `color: var(--muted)`, `letter-spacing: 0.12em`, `text-transform: uppercase`.

---

## 5. Spacing & Grid

| Property | Value |
|----------|-------|
| Page max-width | 1400px, centered |
| Page padding | 20px top/bottom, 32px left/right |
| Main grid | `grid-template-columns: 1fr 340px`, `gap: 20px` |
| Left column | `flex-direction: column`, `gap: 20px` |
| Right column | `flex-direction: column`, `gap: 20px` |
| Card `border-radius` | 12px (cards), 10px (inputs, mini-stats), 8px (buttons, toggles) |
| Card padding | 22-24px (standard), 14-16px (compact sidebar cards), 12-14px (mini-stats) |
| Card border | `1px solid #E8E4DE` |
| Card grain texture | SVG feTurbulence `baseFrequency: 0.85`, `numOctaves: 4`, rect opacity 2% |
| Card hover shadow | `0 12px 40px rgba(0,0,0,0.06)`, border shifts to `#DDD8D0` |

---

## 6. Component Inventory

### 6.1 Header Bar

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                      │
│  ← Back to Dashboard  │  AAPL · Apple Inc.                    [TRADE]  [★ Watchlist] │
│  ↑ ghost button        │  ↑ serif 20px  ↑ Outfit 14px          ↑ gold     ↑ toggle   │
│    icon 14×14          │    weight 400    muted                  accent     state btn  │
│    Outfit 12px         │    · = mono 14px, subtle                primary               │
│    muted color         │                                                              │
│    hover→text color    │                                                              │
│                        ↑                                                              │
│                   divider: 1px × 20px, var(--border)                                  │
│                                                                                      │
│  Bottom border: 1px solid #E8E4DE                                                    │
│  Padding bottom: 20px, Margin bottom: 24px                                           │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

| Element | Font | Size | Weight | Color | Notes |
|---------|------|------|--------|-------|-------|
| Back arrow | SVG | 14×14 | — | `currentColor` | Chevron: `M9 2L4 7L9 12`, strokeWidth 1.5 |
| "Back to Dashboard" | Outfit | 12px | 500 | `var(--muted)` | Hover: color shifts to `var(--text)` |
| Divider | — | 1×20px | — | `var(--border)` | Vertical separator |
| Ticker | Inst. Serif | 20px | 400 | `var(--text)` | — |
| Dot separator | Plex Mono | 14px | — | `var(--subtle)` | "·" character |
| Company name | Outfit | 14px | 400 | `var(--muted)` | — |
| TRADE button | Outfit | 12px | 700 | `var(--accent)` | Border: 1px accent, bg: accent@6%, r=8, p: 8px 20px |
| Watchlist toggle | Outfit | 12px | 600 | dynamic | Off: "☆ Watchlist", border=border, muted. On: "★ Watching", border=accent, color=accent, bg=accent@6% |

### 6.2 Price Hero

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                      │
│  $247.63            [▲ +$3.42 (+1.40%)]    today                                     │
│  ↑ Inst.Serif 48px   ↑ gain pill             ↑ Outfit 11px, subtle                   │
│    weight 400          Plex Mono 14px 500     marginBottom: 6px                       │
│    lineHeight: 1       green color                                                    │
│    letterSpacing:      bg: gain@8%                                                    │
│    -0.03em             borderRadius: 100                                              │
│    alignItems:         padding: 6px 14px                                              │
│    flex-end            glowPulse: 3s ease 2s loop                                     │
│                        arrow: SVG 10×10                                               │
│                                                                                      │
│  Open $244.88  ·  High $251.20  ·  Low $244.18  ·  Vol 62.1M  ·  Mkt Cap 3.82T     │
│  ↑ label Outfit 13px, muted, weight 500                                              │
│  ↑ value Plex Mono 13px, weight 500, text primary                                    │
│  ↑ separator: " · " Plex Mono, subtle, margin 0 10px                                │
│  ↑ wraps on narrow screens (flexWrap: wrap)                                           │
│                                                                                      │
│  Section margin bottom: 28px                                                         │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

**Gain pill glow animation:** Same as dashboard — `box-shadow: 0 0 0 0→6px→0 rgba(45,106,79,0.06)`, 3s loop, 2s start delay.

**Arrow direction:** `▲` (path `M5 1L9 6H1L5 1Z`) when positive, `▼` (path `M5 9L1 4H9L5 9Z`) when negative.

### 6.3 Advanced Chart Card

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│  padding: 24px 28px 20px                                                             │
│                                                                                      │
│  [1D] [1W] [1M] [3M] [6M] [YTD] [1Y] [5Y] [ALL]        [Line] [Area]              │
│  ↑ timeframe buttons (9)                                  ↑ chart type (2)           │
│    left-aligned                                             right-aligned              │
│                                                                                      │
│  ┌────────────────────────────────────── height: 320px ──────────────────────────┐   │
│  │          ╱╲         ╱╲     ╱╲                                                 │   │
│  │        ╱    ╲     ╱    ╲ ╱    ╲   ╱╲                                          │   │
│  │      ╱        ╲ ╱        ╲      ╲╱    ╲                                       │   │
│  │    ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░                                     │   │
│  │  $210 ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─                                    │   │
│  │  Jan         Feb         Mar         Apr                                      │   │
│  └───────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                      │
│  ┌──────────────────────────── volume sub-chart, height: 50px, opacity: 0.6 ────┐   │
│  │  ▐ ▐ ▐▐▐ ▐▐▐▐ ▐▐▐ ▐▐ ▐▐▐ ▐▐▐▐▐▐ ▐▐ ▐▐ ▐▐▐ ▐▐▐ ▐▐ ▐▐ ▐ ▐ ▐              │   │
│  │  ↑ bars: radius [2,2,0,0], recent bars gold (#C9A96E), older #E0DCD5         │   │
│  └───────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                      │
│  Card: sd-card class (grain, border, hover shadow)                                   │
│  animationDelay: 0.15s                                                               │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

**Timeframe button spec:**

| Property | Default | Active |
|----------|---------|--------|
| Padding | 4px 10px | same |
| Border | none | none |
| Border-radius | 6px | same |
| Font | Outfit 11px, weight 600 | same |
| Background | transparent | `rgba(45,106,79,0.08)` |
| Color | `var(--subtle)` | `var(--gain)` |
| Underline (::after) | width 0 | width 100%, 2px, border-radius 1px, gain color |

**Chart type button spec:** Same pattern but `font-size: 10px`, `padding: 4px 12px`, `letter-spacing: 0.04em`.

**Area chart:**
- Stroke: `#2D6A4F`, 2.5px, monotone interpolation
- Gradient: linear top→bottom, gain@12% → gain@0% (Area mode); 0%→0% (Line mode — no fill)
- Tooltip: same ChartTip component as dashboard (white card, serif value, dashed cursor)
- Animation: 1200ms ease-out on entry
- Axes: Plex Mono 10px, fill `#B0ADA6`, no axis lines, no tick lines
- Y formatter: `$${value.toFixed(0)}`
- X interval: 14 (show every 14th date label)

**Volume sub-chart:**
- Height: 50px, marginTop: 4px, opacity: 0.6
- Bars: radius `[2,2,0,0]` (rounded top only)
- Recent bars (index > 75): `var(--accent)` / gold
- Older bars: `#E0DCD5`
- No axes, no labels

### 6.4 Key Statistics Card (right sidebar)

```
┌───────────────────────────────────────┐
│  padding: 20px 10px 10px 10px         │
│                                        │
│  STATISTICS   ← section label          │
│    padding: 0 14px                     │
│    marginBottom: 10px                  │
│                                        │
│  ┌─ stat rows ──────────────────────┐ │
│  │ Prev Close ............ $244.21  │ │  ← row, transparent bg
│  │ Open .................. $244.88  │ │  ← row, #FAF8F5 bg
│  │ Day Range ...... $244.18–$251.20 │ │  ← alternating
│  │ 52-Week Range .. $189.50–$258.30 │ │
│  │ Volume .................. 62.1M  │ │
│  │ Avg Volume .............. 58.4M  │ │
│  │ Market Cap .............. 3.82T  │ │
│  │ P/E (TTM) ............... 31.2   │ │
│  │ EPS (TTM) .............. $7.94   │ │
│  │ Dividend Yield .......... 0.44%  │ │
│  │ Beta .................... 1.24   │ │
│  │ 52-Week Change ........ +28.4%   │ │  ← green colored
│  └──────────────────────────────────┘ │
│                                        │
│  Row: padding 9px 14px, radius 6px    │
│  Label: Outfit 12px, muted, 500       │
│  Value: Plex Mono 13px, 500           │
│  animationDelay: 0.2s                 │
└───────────────────────────────────────┘
```

### 6.5 Analyst Consensus Card

```
┌───────────────────────────────────────┐
│  padding: 22px 24px                    │
│                                        │
│  ANALYST RATINGS                       │
│  marginBottom: 16px                    │
│                                        │
│  ██████████████████████░░░░░░░░░░░░░  │
│  ↑ stacked bar: height 10px, r=5      │
│    gap: 2px between segments           │
│    buy (flex:28) → gain                │
│    hold (flex:8) → accent/gold         │
│    sell (flex:2) → loss                │
│    first segment: borderRadius 5 0 0 5 │
│    last segment:  borderRadius 0 5 5 0 │
│  marginBottom: 14px                    │
│                                        │
│  Buy            Hold           Sell    │
│  28             8              2       │
│  ↑ Plex Mono    ↑ Plex Mono   ↑       │
│    18px, 600      18px, 600     18px   │
│    gain color     accent color  loss   │
│  ↑ Outfit 10px, muted, 500            │
│  marginBottom: 20px                    │
│                                        │
│  ─────────── border-top ──────────     │
│  paddingTop: 16px                      │
│                                        │
│  PRICE TARGET                          │
│  marginBottom: 12px                    │
│                                        │
│  Low          Average         High     │
│  $210         $265            $310     │
│  ↑ label: 10px, muted, 500            │
│  ↑ value: Plex Mono 14px, 600         │
│  text-align: left/center/right         │
│  marginBottom: 8px                     │
│                                        │
│  ░░░░░░░░░░░░░●░░░░░░░░░░░░░░░░░░░  │
│               ↑ price indicator dot    │
│  Track: 6px height, r=3, bg #F0ECE6   │
│  Dot: 12×12, r=50%, bg gain           │
│    border: 2px solid white             │
│    shadow: 0 1px 4px rgba(0,0,0,0.15)  │
│            0 0 0 3px rgba(45,106,79,   │
│            0.15) — green halo          │
│    left: ((price-low)/(high-low))×100% │
│    transform: translateX(-50%)         │
│    transition: left 0.6s spring        │
│                                        │
│  Current: $247.63                      │
│  ↑ center-aligned, 10px, muted         │
│    value: Plex Mono 500, text primary  │
│  marginTop: 8px                        │
│                                        │
│  animationDelay: 0.3s                  │
└───────────────────────────────────────┘
```

### 6.6 About Card

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│  padding: 22px 24px                                                                  │
│                                                                                      │
│  ABOUT APPLE INC.    ← section label, marginBottom: 14px                             │
│                                                                                      │
│  Apple Inc. designs, manufactures, and markets smartphones,                          │
│  personal computers, tablets, wearables, and accessories...  [Read more]             │
│  ↑ Outfit 13px, lineHeight 1.65, text primary                  ↑ accent, 12px, 600  │
│  Collapsed: first 180 chars + "..."                              Toggles aboutOpen    │
│  Expanded: full description                                     → "Show less"        │
│  marginBottom: 12px                                                                  │
│                                                                                      │
│  ─────────── border-top: 1px border ────────────                                     │
│  paddingTop: 14px                                                                    │
│                                                                                      │
│  Sector          Technology                                                          │
│  Industry        Consumer Electronics                                                │
│  Founded         1976 · Cupertino, CA                                                │
│  CEO             Tim Cook                                                            │
│  Employees       164,000                                                             │
│  ↑ label: 12px, muted, 500, width 80px, flexShrink 0                                │
│  ↑ value: 13px, text primary, 500                                                    │
│  gap: 8px between rows, 16px between label/value                                     │
│                                                                                      │
│  [Website ↗]     [SEC Filings ↗]                                                    │
│  ↑ 12px, accent, weight 600, no underline                                            │
│    hover: underline                                                                  │
│  marginTop: 8px, gap: 16px                                                           │
│                                                                                      │
│  animationDelay: 0.25s                                                               │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

### 6.7 Your Position Card

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│  padding: 22px 24px                                                                  │
│                                                                                      │
│  YOUR POSITION    ← section label, marginBottom: 16px                                │
│                                                                                      │
│  85 shares  ·  Avg $178.40                                                           │
│  ↑ Plex Mono 16px, 600   ↑ Plex Mono 14px, muted                                    │
│  · separator: subtle, margin 0 8px                                                   │
│  marginBottom: 16px                                                                  │
│                                                                                      │
│  ┌────────────────┐  ┌────────────────┐  ┌────────────────┐                         │
│  │ MARKET VALUE   │  │ UNREALIZED     │  │ RETURN         │                         │
│  │ $21,049        │  │ +$5,884.15     │  │ +38.8%         │                         │
│  └────────────────┘  └────────────────┘  └────────────────┘                         │
│  ↑ MiniStat cards: flex:1, minWidth 120px, gap 12px                                  │
│    Container: padding 16px 18px, r=12, border 1px border, bg card                    │
│    Label: 9px, mono, 600, muted, uppercase, letterSpacing 0.1em, mb 6px              │
│    Value: 18px, mono, 600, text primary (or gain colored if positive)                 │
│  marginBottom: 16px                                                                  │
│                                                                                      │
│  Weight in portfolio .......................... 12.8%                                 │
│  ↑ label: 11px, muted     ↑ value: mono                                              │
│  ░░░░░░░░░▓▓▓░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░                                       │
│  ↑ track: 4px height, r=2, bg #F0ECE6                                                │
│  ↑ fill: r=2, bg gain, opacity 0.6, width 12.8%                                     │
│    animation: slideWidth 1s ease 0.5s both                                            │
│  marginBottom: 16px                                                                  │
│                                                                                      │
│  ┌───── TRADE ──────┐   ┌──── SELL ALL ────┐                                        │
│  │ gold accent btn  │   │ red outline btn  │                                         │
│  │ flex:1, r=10     │   │ flex:1, r=10     │                                         │
│  │ p: 10px 0        │   │ p: 10px 0        │                                         │
│  │ border: accent    │   │ border: loss     │                                         │
│  │ bg: accent@6%    │   │ bg: transparent   │                                         │
│  │ Outfit 12px 700  │   │ Outfit 12px 700   │                                         │
│  │ color: accent     │   │ color: loss       │                                         │
│  │ hover: bg@14%    │   │ hover: loss@6%    │                                         │
│  └──────────────────┘   └──────────────────┘                                         │
│  gap: 8px                                                                            │
│                                                                                      │
│  NO POSITION STATE: "You don't hold AAPL" + single [BUY AAPL] gold button           │
│                                                                                      │
│  animationDelay: 0.3s                                                                │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

### 6.8 Financials Card

```
┌──────────────────────────────────────────────────────────────────────────────────────┐
│  padding: 22px 24px                                                                  │
│                                                                                      │
│  FINANCIALS                                    [Annual] [Quarterly]                   │
│  ↑ section label                                ↑ toggle pill (compact)               │
│                                                   bg: #F6F4F0, r=8, p=3              │
│                                                   each btn: p=5px 12px, r=6           │
│                                                   active: bg card, shadow             │
│                                                           text: text primary           │
│                                                   default: bg transparent              │
│                                                            text: subtle                │
│  marginBottom: 16px                                                                   │
│                                                                                      │
│  ┌── HEADER ROW ──────────────────────────────────────────────────────────────────┐  │
│  │        Revenue      Net Income     EPS                                         │  │
│  │  ↑ grid: 60px 1fr 1fr 1fr, gap 8                                              │  │
│  │  ↑ header: 9px, 600, subtle, uppercase, letterSpacing 0.08em, text-align right │  │
│  │  marginBottom: 6px                                                             │  │
│  └────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                      │
│  ┌── DATA ROWS (×4) ─────────────────────────────────────────────────────────────┐  │
│  │  2024    $394.3B      $101.2B       $6.42    ← bg #FAF8F5 (even rows)         │  │
│  │  2023    $383.3B       $97.0B       $6.16    ← bg transparent (odd rows)      │  │
│  │  2022    $394.3B       $99.8B       $6.11                                      │  │
│  │  2021    $365.8B       $94.7B       $5.61                                      │  │
│  │  ↑ year: Outfit 12px, 600                                                      │  │
│  │  ↑ values: Plex Mono 13px, 500, text-align right                              │  │
│  │  Row: grid same columns, padding 10px 14px, r=6                                │  │
│  └────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                      │
│  ┌── REVENUE BAR CHART ──────────────────────────────────────────────────────────┐  │
│  │  marginTop: 16px, height 80px, padding 0 14px                                  │  │
│  │  flex row, alignItems flex-end, gap 8px                                         │  │
│  │                                                                                 │  │
│  │  ▐▐▐▐   ▐▐▐▐   ▐▐▐▐   ▐▐▐▐                                                  │  │
│  │  2021    2022    2023    2024                                                    │  │
│  │  ↑ bars: flex 1, maxWidth 48px, r=6                                             │  │
│  │    height: (revNum / maxRevNum) × 64px                                          │  │
│  │    color: var(--gain) for all                                                    │  │
│  │    latest year (last bar): opacity 0.85                                          │  │
│  │    older years: opacity 0.3                                                      │  │
│  │    animation: barGrow 0.6s spring stagger 0.1s                                   │  │
│  │    transformOrigin: bottom center                                                │  │
│  │  ↑ year labels: Plex Mono 9px, subtle                                           │  │
│  └────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                      │
│  animationDelay: 0.35s                                                               │
└──────────────────────────────────────────────────────────────────────────────────────┘
```

### 6.9 Related News Card

Same component pattern as dashboard Market Pulse, filtered to the current ticker.

| Element | Spec |
|---------|------|
| Card padding | 22px 22px 16px |
| Section label | "NEWS" |
| Item gap | 14px between items |
| Item layout | flex row, gap 12px, alignItems flex-start |
| Tag badge | 9px mono 600, uppercase, 3px 8px padding, r=6, letterSpacing 0.04em |
| Tag "Earnings" | bg: gain@8%, color: gain |
| Tag "Tech" | bg: accent@10%, color: accent |
| Tag "Macro" | bg: accent@10%, color: accent |
| Headline | 12px, lineHeight 1.45, text primary |
| Timestamp | Plex Mono 10px, subtle, marginTop 3px |
| Hover | `.news-row` → `translateX(4px)`, 0.2s ease |

### 6.10 Similar Holdings Card

| Element | Spec |
|---------|------|
| Card padding | 20px 0 (no horizontal — content uses its own) |
| Section label | "ALSO IN PORTFOLIO", padding 0 22px, marginBottom 10px |
| Row padding | 12px 22px |
| Row separator | bottom border: `1px solid rgba(0,0,0,0.04)` (not on last row) |
| Ticker | 13px, weight 600, text primary |
| Company name | 11px, subtle |
| Price | Plex Mono 13px, 500, text primary, text-align right |
| Delta | Plex Mono 11px, 500, gain/loss color, text-align right |
| Hover | `.sim-row` → bg #FAF8F5, translateX(3px), 0.3s smooth easing |
| Click action | Navigates to that stock's detail page |

### 6.11 Footer

| Element | Spec |
|---------|------|
| Container | marginTop 28px, padding 16px 0, border-top 1px border |
| Layout | flex space-between, alignItems center |
| Back button | ghost, Outfit 11px, muted, icon 12×12, gap 6px |
| Version label | Plex Mono 10px, subtle |
| Entrance | fadeIn 0.8s ease 0.6s both |

### 6.12 Trade Drawer

Identical to the dashboard Trade Drawer — same 420px sliding panel, same spring-eased entrance, same Buy/Sell toggle, quantity input, order types, preview card, submit button, and confirmation state. Shared component between both pages. See dashboard design brief for full pixel-level spec.

**One difference on this page:** The SELL ALL button in Your Position pre-fills `tradeSide = "sell"` and `tradeQty = String(position.shares)` before opening the drawer.

---

## 7. Ambient & Texture

| Element | Spec |
|---------|------|
| Ambient orbs (3) | Green 340×340 at 2%/18%, Gold 260×260 at 12%/-6%, Green 320×320 at 55%/65% |
| Page grain | Fixed, inset 0, opacity 0.14, SVG feTurbulence 0.8/4/0.07 |
| Card grain | ::before on each .sd-card, feTurbulence 0.85/4/0.02 |

---

## 8. Responsive Breakpoints

```
≥1200px — as prototyped, 2-column grid (1fr + 340px)

900–1199px
┌─────────────────────────────────────────┐
│  Header + Price Hero (full width)        │
│  ┌─ Chart ────────────────────────────┐ │
│  └────────────────────────────────────┘ │
│  ┌─ Stats ──────┐  ┌─ Analysts ──────┐ │
│  └──────────────┘  └────────────────┘ │
│  ┌─ About ────────────────────────────┐ │
│  ┌─ Position ─────────────────────────┐ │
│  ┌─ Financials ───────────────────────┐ │
│  ┌─ News ───────┐  ┌─ Similar ───────┐ │
│  └──────────────┘  └────────────────┘ │
└─────────────────────────────────────────┘

<600px (Mobile)
  Single column stack
  Chart type selector → scrollable horizontal
  Stat trio → wraps to 2+1
  Similar holdings → hidden
  Trade drawer → full-screen overlay
```
