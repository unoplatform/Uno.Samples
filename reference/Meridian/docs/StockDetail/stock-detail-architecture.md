# Meridian Stock Detail — Architecture & Technical Constraints

> **Page:** Stock Detail (Page 2)  
> **Pattern:** MVUX Reactive  
> **Target:** Uno Platform (C# / WinUI 3 / XAML)  
> **Shared infrastructure:** Same Meridian solution, same `IMarketDataService`, same theme resources, same Trade Drawer component  
> **Source Prototype:** `stock-detail-v1.jsx`

---

## 1. Navigation Architecture

### Route Registration

```csharp
// In App host configuration
new RouteMap("Dashboard", View: views.FindByViewModel<DashboardModel>(), IsDefault: true),
new RouteMap("StockDetail", View: views.FindByViewModel<StockDetailModel>()),
```

### Navigation Parameter

The page receives a stock ticker as a navigation parameter. Uno Navigation Extensions passes data via the route:

```csharp
// From dashboard — explicit "Details" button triggers navigation
uen:Navigation.Request="StockDetail"
uen:Navigation.Data="{Binding SelectedStock}"
```

The `StockDetailModel` receives the `Stock` object (or ticker string) as a constructor-injected navigation parameter.

### Back Navigation

```csharp
// XAML approach (preferred)
uen:Navigation.Request="-"  // goes back

// Dashboard state must be preserved:
// - scroll position
// - expanded watchlist row
// - chart-swap ticker
// Uno Navigation handles frame-based state preservation by default
```

---

## 2. Data Models

### Existing Models (reused from dashboard)

```csharp
public record Stock(string Ticker, string Name, decimal Price,
    decimal Change, decimal Pct, decimal High, decimal Low,
    decimal Open, string Volume);

public record Holding(string Ticker, int Shares, decimal AvgCost,
    decimal CurrentPrice);

public record ChartPoint(string Date, decimal Value);
public record NewsItem(string Time, string Text, string Tag);
```

### New Models (Stock Detail only)

```csharp
public record StockProfile(
    string Description,
    string Sector,
    string Industry,
    int Founded,
    string Headquarters,
    string CEO,
    string Employees,
    string WebsiteUrl);

public record AnalystData(
    int BuyCount,
    int HoldCount,
    int SellCount,
    decimal PriceTargetLow,
    decimal PriceTargetAvg,
    decimal PriceTargetHigh);

public record Financial(
    string Period,       // "2024" or "Q1 2024"
    string Revenue,      // formatted: "$394.3B"
    string NetIncome,    // formatted: "$101.2B"
    string EPS,          // formatted: "$6.42"
    decimal RevenueNum); // raw number for bar chart height

public record KeyStat(
    string Label,
    string Value,
    bool IsColored = false,
    bool IsPositive = false);

public record SimilarStock(
    string Ticker,
    string Name,
    decimal Price,
    decimal Pct);
```

---

## 3. MVUX Presentation Model

### StockDetailModel

```csharp
public partial record StockDetailModel(
    IMarketDataService MarketData,
    Stock Stock)           // ← injected via navigation parameter
{
    // ── Read-only feeds ──

    public IFeed<Holding?> Position =>
        Feed.Async(ct => MarketData.GetHoldingAsync(Stock.Ticker, ct));

    public IFeed<StockProfile> Profile =>
        Feed.Async(ct => MarketData.GetStockProfileAsync(Stock.Ticker, ct));

    public IFeed<AnalystData> Analysts =>
        Feed.Async(ct => MarketData.GetAnalystDataAsync(Stock.Ticker, ct));

    public IListFeed<Financial> Financials =>
        ListFeed.Async(ct => MarketData.GetFinancialsAsync(Stock.Ticker, ct));

    public IListFeed<KeyStat> KeyStats =>
        ListFeed.Async(ct => MarketData.GetKeyStatsAsync(Stock.Ticker, ct));

    public IListFeed<NewsItem> RelatedNews =>
        ListFeed.Async(ct => MarketData.GetNewsForTickerAsync(Stock.Ticker, ct));

    public IListFeed<SimilarStock> SimilarHoldings =>
        ListFeed.Async(ct => MarketData.GetSimilarHoldingsAsync(Stock.Ticker, ct));

    // ── Computed feed: chart data reacts to timeframe ──

    public IFeed<IImmutableList<ChartPoint>> ChartData =>
        SelectedTimeframe.SelectAsync(async (tf, ct) =>
            await MarketData.GetStockHistoryAsync(Stock.Ticker, tf, ct));

    // ── Editable states ──

    public IState<string> SelectedTimeframe =>
        State.Value(this, () => "3M");

    public IState<string> ChartType =>
        State.Value(this, () => "Area");      // "Line" | "Area"

    public IState<string> FinancialsPeriod =>
        State.Value(this, () => "Annual");    // "Annual" | "Quarterly"

    public IState<bool> IsInWatchlist =>
        State.Value(this, () => false);

    public IState<bool> AboutExpanded =>
        State.Value(this, () => false);

    public IState<Stock?> TradeStock =>
        State.Value(this, () => (Stock?)null);

    // ── Commands ──

    public async ValueTask ToggleWatchlist()
    {
        var current = await IsInWatchlist;
        await IsInWatchlist.Set(!current, CancellationToken.None);
    }

    public async ValueTask OpenTrade() =>
        await TradeStock.Set(Stock, CancellationToken.None);

    public async ValueTask OpenSellAll()
    {
        // TradeDrawerModel will be initialized with side="sell" and qty=position.shares
        await TradeStock.Set(Stock, CancellationToken.None);
    }

    public async ValueTask CloseTrade() =>
        await TradeStock.Set(null, CancellationToken.None);

    public async ValueTask ToggleAbout()
    {
        var current = await AboutExpanded;
        await AboutExpanded.Set(!current, CancellationToken.None);
    }
}
```

### TradeDrawerModel (reused from dashboard)

Same `TradeDrawerModel` as the dashboard. The drawer is a shared component hosted on this page via the same `TradeStock` state pattern.

---

## 4. Service Interface Extensions

```csharp
// New methods added to IMarketDataService
ValueTask<Stock> GetStockAsync(string ticker, CancellationToken ct);
ValueTask<Holding?> GetHoldingAsync(string ticker, CancellationToken ct);
ValueTask<StockProfile> GetStockProfileAsync(string ticker, CancellationToken ct);
ValueTask<AnalystData> GetAnalystDataAsync(string ticker, CancellationToken ct);
ValueTask<IImmutableList<Financial>> GetFinancialsAsync(string ticker, CancellationToken ct);
ValueTask<IImmutableList<KeyStat>> GetKeyStatsAsync(string ticker, CancellationToken ct);
ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(
    string ticker, string timeframe, CancellationToken ct);
ValueTask<IImmutableList<NewsItem>> GetNewsForTickerAsync(string ticker, CancellationToken ct);
ValueTask<IImmutableList<SimilarStock>> GetSimilarHoldingsAsync(string ticker, CancellationToken ct);
```

`MockMarketDataService` implements all of these with hardcoded data matching the prototype's constants.

---

## 5. Component Decomposition

| Component | Type | Reusable? | Base Control | Notes |
|-----------|------|-----------|-------------|-------|
| **StockDetailPage** | Page | No | `Page` | Top-level container, hosts all cards + trade drawer |
| **PageHeaderBar** | UserControl | Yes | `Grid` | Back nav + ticker/name + action buttons |
| **PriceHero** | UserControl | Yes | `StackPanel` | Price, change pill, OHLC strip |
| **AdvancedChartCard** | UserControl | Moderate | `Border` + `SKXamlCanvas`/LiveCharts2 | Extends dashboard chart with type selector + volume sub-chart |
| **KeyStatisticsCard** | UserControl | Yes | `Border` + `ItemsRepeater` | 12-row stat grid, alternating backgrounds |
| **AnalystConsensusCard** | UserControl | Yes | `Border` | Rating bar + counts + price target range |
| **AboutCard** | UserControl | Yes | `Border` | Expandable description + metadata grid + links |
| **PositionCard** | UserControl | Yes | `Border` | Conditional: has-position vs empty-state |
| **MiniStatCard** | UserControl | Yes | `Border` | Reusable stat card (label + value + optional color) |
| **FinancialsCard** | UserControl | Moderate | `Border` + `SKXamlCanvas` | Period toggle + data table + revenue bar chart |
| **RelatedNewsCard** | UserControl | Yes (shared) | Same as dashboard MarketPulse | Filtered to current ticker |
| **SimilarHoldingsCard** | UserControl | Yes | `Border` + `ItemsRepeater` | Clickable rows navigating to other detail pages |
| **TradeDrawer** | UserControl | Yes (shared) | `DrawerFlyoutPresenter` / `Popup` | Identical to dashboard — same component |
| **PriceTargetRange** | UserControl | Yes | `Grid` | Low/Avg/High labels + track + positioned dot |
| **WeightBar** | UserControl | Yes (shared) | `Border` | Animated fill bar from dashboard |

---

## 6. Project File Additions

```
Meridian/
├── Models/
│   ├── (existing: Stock, Holding, ChartPoint, NewsItem, etc.)
│   ├── StockProfile.cs          ← NEW
│   ├── AnalystData.cs           ← NEW
│   ├── Financial.cs             ← NEW
│   ├── KeyStat.cs               ← NEW
│   └── SimilarStock.cs          ← NEW
│
├── Presentation/
│   ├── DashboardModel.cs        (existing)
│   ├── TradeDrawerModel.cs      (existing, shared)
│   └── StockDetailModel.cs      ← NEW
│
├── Views/
│   ├── DashboardPage.xaml/.cs   (existing, modified: add Details buttons)
│   ├── StockDetailPage.xaml/.cs ← NEW
│   └── TradeDrawer.xaml/.cs     (existing, shared)
│
├── Controls/
│   ├── (existing SkiaSharp controls)
│   ├── PageHeaderBar.xaml/.cs   ← NEW
│   ├── PriceHero.xaml/.cs       ← NEW
│   ├── KeyStatisticsCard.xaml/.cs ← NEW
│   ├── AnalystConsensusCard.xaml/.cs ← NEW
│   ├── AboutCard.xaml/.cs       ← NEW
│   ├── PositionCard.xaml/.cs    ← NEW
│   ├── MiniStatCard.xaml/.cs    ← NEW
│   ├── FinancialsCard.xaml/.cs  ← NEW
│   ├── SimilarHoldingsCard.xaml/.cs ← NEW
│   └── PriceTargetRange.xaml/.cs ← NEW
│
└── Services/
    ├── IMarketDataService.cs    (modified: 9 new methods)
    └── MockMarketDataService.cs (modified: implement new methods)
```

---

## 7. Data Flow

```
Dashboard (Stock selected)
    │
    ▼  Navigation.Request = "StockDetail", Data = Stock
    │
StockDetailModel constructor receives Stock
    │
    ├── Stock.Ticker → drives all Feed.Async calls
    │   ├── Position feed → PositionCard (conditional render)
    │   ├── Profile feed → AboutCard
    │   ├── Analysts feed → AnalystConsensusCard
    │   ├── Financials feed → FinancialsCard
    │   ├── KeyStats feed → KeyStatisticsCard
    │   ├── RelatedNews feed → RelatedNewsCard
    │   └── SimilarHoldings feed → SimilarHoldingsCard
    │
    ├── SelectedTimeframe state → ChartData computed feed → AdvancedChartCard
    │
    ├── ChartType state → controls gradient fill visibility
    │
    ├── FinancialsPeriod state → controls which data rows display
    │
    ├── IsInWatchlist state → Watchlist toggle button
    │
    ├── AboutExpanded state → description truncation
    │
    └── TradeStock state → TradeDrawer visibility
        │
        └── TradeDrawerModel (same as dashboard)
```

---

## 8. Dashboard Modifications

### Watchlist Expanded Row — Add "Details" Button

```xml
<!-- Current: 2 buttons -->
<Button Content="View Chart" Command="{Binding SelectHolding}" />
<Button Content="Trade" Command="{Binding OpenTrade}" />

<!-- New: 3 buttons -->
<Button Content="View Chart" Command="{Binding SelectHolding}" />
<Button Content="Trade" Command="{Binding OpenTrade}" />
<Button Content="Details ⟶"
        uen:Navigation.Request="StockDetail"
        uen:Navigation.Data="{Binding}" />
```

### Chart Header (Stock Detail Mode) — Add "DETAILS" Button

```xml
<!-- Between stock delta and TRADE button -->
<Button Content="DETAILS"
        uen:Navigation.Request="StockDetail"
        uen:Navigation.Data="{Binding ChartStock}"
        Style="{StaticResource MeridianGhostButtonStyle}" />
<Button Content="TRADE"
        Command="{Binding OpenTrade}"
        Style="{StaticResource MeridianAccentButtonStyle}" />
```

---

## 9. Performance Considerations

| Area | Concern | Mitigation |
|------|---------|------------|
| Navigation transition | Page creation + all feeds fire simultaneously | Pre-compute key data (Stock, OHLC) from dashboard data, pass via nav param. Only fetch supplemental data (profile, analysts, financials) on page load. |
| Chart re-render on timeframe change | Swapping chart data triggers full redraw | Cache all timeframe histories at service level. Swap by reference, not regeneration. |
| Financial period toggle | Switches between 4 rows (annual) and 8 rows (quarterly) | Both datasets pre-loaded in mock service. No async call on toggle — just filter. |
| About expansion | Text reflow on expand/collapse | Use `MaxLines` + `TextTrimming` for collapsed state. No layout measurement needed. |
| Multiple ItemsRepeater instances | 5 list-based components on one page | All lists are small (4-12 items). No virtualization needed. |
| Trade drawer animation | Same spring animation as dashboard | Same mitigation: use `CompositionAnimation` for spring, gate on `isClosing` flag. |
| Similar Holdings navigation | Clicking navigates to the same page type with different data | Uno Navigation handles parameter-based re-creation. Verify state cleanup on re-entry. |

---

## 10. Risk Matrix

| Risk | Severity | Mitigation |
|------|----------|------------|
| Navigation parameter loss | MEDIUM | Validate `Stock` param in model constructor. If null, navigate back to dashboard. |
| Position card conditional render | LOW | Bind `Visibility` to `Position != null`. Test both states (has position, no position). |
| Chart type selector scope | LOW | v1 ships with Line + Area only. Candlestick is stretch goal — leave the selector extensible. |
| Financial data formatting | LOW | Format numbers in the service layer, not the view. Pass pre-formatted strings. |
| Similar Holdings circular nav | LOW | Navigating A→B→A creates forward stack. May want to implement `NavigateRouteAsync` with replacement instead of push. |

---

## 11. Implementation Priority

| Phase | Scope | Est. |
|-------|-------|------|
| P0 | Page shell + navigation wiring + route registration | 0.5 day |
| P1 | Header + Price Hero + back navigation | 0.5 day |
| P2 | Key Statistics card | 0.5 day |
| P3 | Advanced chart (reuse PerformanceChartControl + add type selector + volume sub) | 1.5 days |
| P4 | About card with expand/collapse | 0.5 day |
| P5 | Your Position card (conditional + mini-stats + weight bar + buttons) | 1 day |
| P6 | Analyst Consensus card (rating bar + price target range) | 1 day |
| P7 | Financials card (table + toggle + revenue bar chart) | 1 day |
| P8 | Related News + Similar Holdings (reuse dashboard components) | 0.5 day |
| P9 | Dashboard modifications (add Details buttons) + polish + responsive | 0.5 day |

**Total: ~7.5 developer-days**

---

## 12. Assumptions

- All data is mock — same as dashboard
- `StockProfile` descriptions are hardcoded strings in `MockMarketDataService`
- Analyst data is fabricated (realistic numbers, not from any API)
- Financials are 4 years annual / 8 quarters — pre-formatted in the service
- Candlestick chart type is a stretch goal — v1 ships with Line + Area
- "Similar Holdings" only shows same-sector stocks the user already holds
- The `TradeDrawer` is a shared component between dashboard and stock detail
- Navigation preserves dashboard state (Uno Navigation frame-based caching)
- The price on this page is static (no odometer) — this is a research page, not a live ticker
