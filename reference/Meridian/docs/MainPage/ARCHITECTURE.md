# Meridian Capital Terminal — Architecture & Technical Constraints

> **Source Prototype:** Meridian v4 (React/JSX)  
> **Target Platform:** Uno Platform (C# / WinUI 3 / XAML)  
> **Architecture:** MVUX Reactive Pattern  
> **Date:** March 18, 2026

---

## 3.1 Project Structure

```
Meridian.sln
└── Meridian/
    ├── Meridian.csproj
    ├── App.xaml / App.xaml.cs
    │
    ├── Assets/
    │   └── Fonts/
    │       ├── InstrumentSerif-Regular.ttf
    │       ├── InstrumentSerif-Italic.ttf
    │       ├── IBMPlexMono-Regular.ttf
    │       ├── IBMPlexMono-Medium.ttf
    │       └── Outfit-Variable.ttf
    │
    ├── Models/
    │   ├── Stock.cs
    │   ├── Holding.cs
    │   ├── Sector.cs
    │   ├── VolumeBar.cs
    │   ├── NewsItem.cs
    │   ├── ChartPoint.cs
    │   ├── IndexTicker.cs
    │   ├── StreamTicker.cs
    │   └── TradeOrder.cs
    │
    ├── Services/
    │   ├── IMarketDataService.cs
    │   └── MockMarketDataService.cs
    │
    ├── Presentation/
    │   ├── DashboardModel.cs          ← Primary MVUX model
    │   └── TradeDrawerModel.cs        ← Trade drawer MVUX model
    │
    ├── Controls/
    │   ├── OdometerControl.xaml/.cs
    │   ├── BrailleTickerControl.xaml/.cs
    │   ├── BrailleSpinnerControl.xaml/.cs
    │   ├── BraillePulseControl.xaml/.cs
    │   ├── BrailleActivityControl.xaml/.cs
    │   ├── SparklineControl.cs        ← SKXamlCanvas subclass
    │   ├── SectorRingControl.cs       ← SKXamlCanvas subclass
    │   ├── VolumeChartControl.cs      ← SKXamlCanvas subclass
    │   └── PerformanceChartControl.cs ← SKXamlCanvas or LiveCharts2
    │
    ├── Views/
    │   ├── DashboardPage.xaml/.cs
    │   └── TradeDrawer.xaml/.cs
    │
    └── Themes/
        ├── ColorPaletteOverride.xaml
        ├── FontResources.xaml
        ├── CardStyles.xaml
        └── TextBlockStyles.xaml
```

### Required UnoFeatures

```xml
<UnoFeatures>
    Material;
    Toolkit;
    MVUX;
    Skia;
    Hosting;
    ExtensionsCore;
    Logging;
</UnoFeatures>
```

Additional NuGet:
- `SkiaSharp.Views.Uno.WinUI` — for `SKXamlCanvas`
- `LiveChartsCore.SkiaSharpView.Uno.WinUI` — if using LiveCharts2 for main area chart (optional)

---

## 3.2 MVUX Model Design

### DashboardModel (Primary)

```csharp
public partial record DashboardModel(IMarketDataService MarketData)
{
    // ── Read-only feeds (data from service) ──
    public IListFeed<Stock> Watchlist =>
        ListFeed.Async(MarketData.GetWatchlistAsync);

    public IListFeed<Holding> Holdings =>
        ListFeed.Async(MarketData.GetHoldingsAsync);

    public IListFeed<Sector> Sectors =>
        ListFeed.Async(MarketData.GetSectorsAsync);

    public IListFeed<VolumeBar> Volume =>
        ListFeed.Async(MarketData.GetVolumeAsync);

    public IListFeed<NewsItem> News =>
        ListFeed.Async(MarketData.GetNewsAsync);

    public IFeed<IImmutableList<ChartPoint>> PortfolioHistory =>
        Feed.Async(MarketData.GetPortfolioHistoryAsync);

    // ── Editable states (user interaction) ──
    public IState<string> SelectedTimeframe =>
        State.Value(this, () => "3M");

    public IState<string?> ChartTicker =>
        State.Value(this, () => (string?)null);

    public IState<string?> ExpandedTicker =>
        State.Value(this, () => (string?)null);

    public IState<string> SearchQuery =>
        State.Value(this, () => "");

    public IState<Stock?> TradeStock =>
        State.Value(this, () => (Stock?)null);

    public IState<string?> HoveredHolding =>
        State.Value(this, () => (string?)null);

    // ── Computed feed: chart data swaps based on ChartTicker ──
    public IFeed<IImmutableList<ChartPoint>> ChartData =>
        ChartTicker.SelectAsync(async (ticker, ct) =>
            ticker is null
                ? await MarketData.GetPortfolioHistoryAsync(ct)
                : await MarketData.GetStockHistoryAsync(ticker, ct));

    // ── Commands ──
    public async ValueTask SelectHolding(string ticker)
    {
        var current = await ChartTicker;
        await ChartTicker.Set(
            current == ticker ? null : ticker,
            CancellationToken.None);
    }

    public async ValueTask ToggleExpanded(string ticker)
    {
        var current = await ExpandedTicker;
        await ExpandedTicker.Set(
            current == ticker ? null : ticker,
            CancellationToken.None);
    }

    public async ValueTask OpenTrade(Stock stock) =>
        await TradeStock.Set(stock, CancellationToken.None);

    public async ValueTask CloseTrade() =>
        await TradeStock.Set(null, CancellationToken.None);

    public async ValueTask BackToPortfolio() =>
        await ChartTicker.Set(null, CancellationToken.None);
}
```

### TradeDrawerModel (Separate)

```csharp
public partial record TradeDrawerModel
{
    public IState<string> Side => State.Value(this, () => "buy");
    public IState<int> Quantity => State.Value(this, () => 0);
    public IState<string> OrderType => State.Value(this, () => "market");
    public IState<decimal?> LimitPrice => State.Value(this, () => (decimal?)null);
    public IState<bool> IsConfirmed => State.Value(this, () => false);

    public async ValueTask SetQuantity(int qty) =>
        await Quantity.Set(qty, CancellationToken.None);

    public async ValueTask SelectOrderType(string type)
    {
        await OrderType.Set(type, CancellationToken.None);
        if (type == "market")
            await LimitPrice.Set(null, CancellationToken.None);
    }

    public async ValueTask SubmitOrder() =>
        await IsConfirmed.Set(true, CancellationToken.None);

    public async ValueTask Reset()
    {
        await Side.Set("buy", CancellationToken.None);
        await Quantity.Set(0, CancellationToken.None);
        await OrderType.Set("market", CancellationToken.None);
        await LimitPrice.Set(null, CancellationToken.None);
        await IsConfirmed.Set(false, CancellationToken.None);
    }
}
```

### IMarketDataService Interface

```csharp
public interface IMarketDataService
{
    ValueTask<IImmutableList<Stock>> GetWatchlistAsync(CancellationToken ct);
    ValueTask<IImmutableList<Holding>> GetHoldingsAsync(CancellationToken ct);
    ValueTask<IImmutableList<Sector>> GetSectorsAsync(CancellationToken ct);
    ValueTask<IImmutableList<VolumeBar>> GetVolumeAsync(CancellationToken ct);
    ValueTask<IImmutableList<NewsItem>> GetNewsAsync(CancellationToken ct);
    ValueTask<IImmutableList<ChartPoint>> GetPortfolioHistoryAsync(CancellationToken ct);
    ValueTask<IImmutableList<ChartPoint>> GetStockHistoryAsync(string ticker, CancellationToken ct);
    IImmutableList<IndexTicker> GetIndexTickers();
    IImmutableList<StreamTicker> GetStreamTickers();
}
```

---

## 3.3 Component Decomposition

| Component | Type | Reusable? | Uno Control Base | DependencyProperties |
|-----------|------|-----------|-----------------|---------------------|
| **OdometerControl** | `UserControl` | Yes — any animated number | Custom (`ItemsControl` + per-digit `ScrollViewer`) | `Value (decimal)`, `Prefix (string)`, `FontSize (double)` |
| **BrailleTickerControl** | `UserControl` | Yes — with compact mode | Custom (`TextBlock` with `Inlines`) | `IsCompact (bool)`, `StreamTickers (IList)` |
| **BrailleSpinnerControl** | `UserControl` | Yes — any live indicator | Custom (`TextBlock`) | `Color (Brush)`, `IntervalMs (int)` |
| **BraillePulseControl** | `UserControl` | Yes — any heartbeat display | Custom (`TextBlock`) | `Color (Brush)`, `Width (int)` |
| **BrailleActivityControl** | `UserControl` | Yes — per-item activity | Custom (`TextBlock`) | `Intensity (double 0-1)` |
| **SparklineControl** | `SKXamlCanvas` | Yes — any inline mini chart | SkiaSharp custom drawing | `Points (IList<double>)`, `IsPositive (bool)` |
| **SectorRingControl** | `SKXamlCanvas` | Yes — any donut/ring | SkiaSharp custom drawing | `Sectors (IList<Sector>)`, `HoveredIndex (int?)` |
| **VolumeChartControl** | `SKXamlCanvas` | Moderate — volume-specific | SkiaSharp custom drawing | `VolumeData (IList<VolumeBar>)`, `HoveredIndex (int?)` |
| **PerformanceChartControl** | `SKXamlCanvas` or LiveCharts2 | No — complex header logic | SkiaSharp or `CartesianChart` | `ChartData (IList<ChartPoint>)`, `StrokeColor (Color)` |
| **HoldingCard** | `DataTemplate` | Yes — `ItemTemplate` | `Border` / `Card` | Binds to `Holding` record |
| **WatchlistRow** | `DataTemplate` | Yes — `ItemTemplate` | Custom with expandable section | Binds to `Stock` record |
| **NewsItemTemplate** | `DataTemplate` | Yes — `ItemTemplate` | `AutoLayout` row | Binds to `NewsItem` record |
| **TradeDrawer** | `UserControl` | Yes — reusable overlay | `DrawerFlyoutPresenter` or `Popup` | `Stock (Stock)`, `OnClose (event)` |
| **IndexCardTemplate** | `DataTemplate` | Yes — used ×3 | `Card` (`ElevatedCardStyle`) | Binds to `IndexTicker` record |

---

## 3.4 Charting Strategy

The prototype uses Recharts (React library). For Uno Platform, three approaches ranked by recommendation:

### Option A: SkiaSharp Custom Drawing (Recommended for custom viz)

**Use for:** Sector ring, volume profile, sparklines.

- Full control over every pixel
- Consistent rendering across all Uno targets
- Matches the prototype's custom SVG approach directly
- Supports all hover/animation behaviors natively
- `SKXamlCanvas.PaintSurface` event → draw with `SKCanvas`
- Pointer hit-testing via manual coordinate math in `PointerMoved`

### Option B: LiveCharts2 (Recommended for area chart)

**Use for:** Main performance area chart.

- Mature .NET charting library with Uno/SkiaSharp backend
- Built-in tooltips, axis formatting, and animations
- Supports multiple series (future: benchmark overlay)
- `CartesianChart` control with `LineSeries<ChartPoint>`
- Good trade-off: less code than full SkiaSharp custom, more features out of box

### Option C: Full SkiaSharp Custom (Fallback for area chart)

**Use if:** LiveCharts2 doesn't meet aesthetic requirements or introduces dependency issues.

- More code but total control over tooltip styling, gradient fills, and animation curves
- Can exactly match the prototype's chart appearance

**Decision needed:** Team should prototype the main area chart in both LiveCharts2 and SkiaSharp custom to evaluate visual fidelity and effort.

---

## 3.5 Trade Drawer Implementation

### Option A: DrawerFlyoutPresenter (Recommended)

```xml
<Button Content="Trade">
    <Button.Flyout>
        <Flyout Placement="Full"
                FlyoutPresenterStyle="{StaticResource RightDrawerFlyoutPresenterStyle}">
            <local:TradeDrawer Stock="{Binding TradeStock}" />
        </Flyout>
    </Button.Flyout>
</Button>
```

**Pros:**
- Built-in light-dismiss behavior
- Gesture support on touch platforms
- Correct z-ordering and backdrop handling
- Platform-consistent behavior

**Cons:**
- Spring overshoot animation may need custom `Storyboard` override
- Backdrop blur requires additional styling

### Option B: Custom Popup Overlay

```csharp
var popup = new Popup
{
    Child = new TradeDrawer { Stock = stock },
    IsLightDismissEnabled = true,
};
popup.IsOpen = true;
```

**Pros:**
- Full control over entrance/exit animation timing
- Easy backdrop blur via `AcrylicBrush` on background grid

**Cons:**
- Manual z-ordering and dismissal logic
- More code to maintain

**Recommendation:** Start with `DrawerFlyoutPresenter` for correctness. If spring overshoot or backdrop blur prove difficult, fall back to Option B.

---

## 3.6 Data / Content Structure

### Record Types (all immutable)

```csharp
public record Stock(string Ticker, string Name, decimal Price,
    decimal Change, decimal Pct, decimal High, decimal Low,
    decimal Open, string Volume);

public record Holding(string Ticker, int Shares, decimal AvgCost,
    decimal CurrentPrice)
{
    public decimal MarketValue => Shares * CurrentPrice;
    public decimal GainLoss => (CurrentPrice - AvgCost) * Shares;
    public decimal GainPct => (CurrentPrice - AvgCost) / AvgCost * 100;
    public bool IsPositive => GainPct >= 0;
}

public record Sector(string Name, double Pct, string ColorHex);
public record VolumeBar(string Hour, int Volume);
public record NewsItem(string Time, string Text, string Tag);
public record ChartPoint(string Date, decimal Value);
public record IndexTicker(string Name, string Value, string Change, bool IsUp);
public record StreamTicker(string Ticker, string Price, string Delta, bool IsUp);
public record TradeOrder(string Ticker, string Side, int Quantity,
    string OrderType, decimal? LimitPrice, decimal EstimatedTotal);
```

### Data Flow

```
IMarketDataService (mock/live)
    ↓
DashboardModel (MVUX feeds + states)
    ↓ data binding
DashboardPage.xaml (XAML controls)
    ↓ user interaction
DashboardModel commands (update states)
    ↓ reactive propagation
UI updates automatically via MVUX binding
```

---

## 3.7 Responsive / Adaptive Strategy

Use Uno Toolkit `Responsive` markup extension for breakpoint-driven layout changes.

### Breakpoint Definitions

| Name | Min Width | Layout Changes |
|------|-----------|---------------|
| `Narrow` | 0px | Single column, ticker tape hidden, drawer = full screen |
| `Medium` | 600px | Single column, ticker tape visible, stacked cards |
| `Wide` | 900px | 2-column with narrower sidebar (300px) |
| `ExtraWide` | 1200px | Full layout as prototyped (360px sidebar) |

### Implementation Pattern

```xml
<Grid Visibility="{Responsive Wide=Visible, Narrow=Collapsed}" />

<Grid.ColumnDefinitions>
    <ColumnDefinition Width="*" />
    <ColumnDefinition Width="{Responsive ExtraWide=360, Wide=300, Medium=0, Narrow=0}" />
</Grid.ColumnDefinitions>
```

For the right sidebar on narrow screens, it should collapse below the main content as a full-width section.

---

## 3.8 Accessibility Requirements

| Element | Requirement |
|---------|-------------|
| Portfolio value | `AutomationProperties.Name="Portfolio value: $163,842.56"` (updated dynamically) |
| Chart | `AutomationProperties.Name="Portfolio performance chart, 3 month view"` / `"Apple stock chart, 3 month view"` |
| Holding card | `AutomationProperties.Name="Apple, 85 shares, value $21,048, up 38.8 percent"` per card |
| Watchlist row | `AutomationProperties.Name="Apple, $247.63, up 1.40 percent"` per row |
| Trade drawer | Focus trap when open; `Tab` cycles form fields; `Escape` closes |
| Color independence | Gain/loss communicated via `▲`/`▼` arrow icons AND color — never color alone |
| Touch targets | All buttons: `MinWidth="44"` `MinHeight="44"` |
| Braille animations | Purely decorative — `AutomationProperties.AccessibilityView="Raw"` to exclude from screen readers |
| News tags | `AutomationProperties.Name` includes tag text + headline for screen readers |
| Form labels | All trade drawer inputs: `AutomationProperties.LabeledBy` pointing to label `TextBlock` |
| Focus order | Logical tab order via `TabIndex` on interactive elements |

---

## 3.9 Performance Considerations

| Area | Concern | Mitigation |
|------|---------|------------|
| Braille ticker | `DispatcherTimer` at 70ms (~14fps) updating `TextBlock.Inlines` | Batch updates, avoid triggering full layout passes. Consider using a single `TextBlock.Text` string replacement instead of `Inlines` manipulation. |
| SkiaSharp canvases | Unnecessary redraws on every timer tick | Only call `Invalidate()` on data change or hover state change, never on timer. Sparklines are static after load — draw once. |
| Ambient orbs | Continuous translation animation on 4 elements | Use `CompositionAnimation` (GPU-accelerated), not `Storyboard` for continuous motion. |
| Odometer | 10+ digit slots each with internal scroll strip | Limit to max 12 digit slots. Recycle vertical strips. Only animate digits that actually changed on data update. |
| Watchlist expand | Hidden expanded panels for all 8 items always in tree | Use `x:DeferLoadStrategy="Lazy"` or `x:Load` binding on expanded panels. Only instantiate when `ExpandedTicker == Ticker`. |
| ListView virtualization | Watchlist has 8 items — virtualization optional but good hygiene | Use `ItemsRepeater` with default `StackLayout` (virtualizing). |
| Chart data swap | Switching between portfolio and stock history | Pre-compute all 8 stock histories at service initialization. Swap by reference, not regeneration. |
| Spring physics | `DispatcherTimer` loop for spring number animation | Use native `SpringVector3NaturalMotionAnimation` via Composition API where available. Fall back to timer only if needed. |
| Font loading | 3 custom font families, variable weight for Outfit | Load via `ms-appx:///Assets/Fonts/` URIs. Ensure fonts are set to `Content` build action, not `Embedded Resource`. |

---

## 3.10 Implementation Risks

| Risk | Severity | Description | Mitigation |
|------|----------|-------------|------------|
| Braille unicode rendering | **HIGH** | Some braille characters (⣷, ⣿) may render inconsistently across platforms, particularly WASM and older Android | Test on all target platforms in P0. Fallback: custom SkiaSharp-drawn dot matrix that mimics braille grid. |
| Instrument Serif font | **MEDIUM** | Must be bundled as `.ttf`. If licensing prohibits bundling for commercial use, need substitute. | Instrument Serif is Google Fonts (OFL license) — should be fine. Fallback: Playfair Display or Cormorant Garamond. |
| 3D card tilt | **MEDIUM** | `CompositionAnimation` on `RotationAngle` requires per-platform testing. May not work identically on all Skia backends. | Implement as desktop-only enhancement. Graceful fallback: no tilt, just hover shadow on other platforms. |
| Backdrop blur | **MEDIUM** | `AcrylicBrush` rendering varies across Uno targets. WASM may not support backdrop blur. | Test early. Fallback: semi-transparent solid color overlay without blur. |
| Spring animations | **LOW** | WinUI/Uno support `SpringVector3NaturalMotionAnimation` natively. Should work on Skia renderer. | Test on Skia + native backends. Fallback: cubic-bezier approximation. |
| LiveCharts2 compatibility | **LOW** | If using LiveCharts2, verify latest version works with current Uno Platform version. | Pin to known working version. Alternative: full SkiaSharp custom chart. |
| DispatcherTimer frequency | **LOW** | 5 timers running simultaneously (ticker 70ms, spinner 80ms, pulse 120ms, activity 150ms, clock 1000ms) | Consolidate into fewer timers where possible. E.g., one 70ms timer driving ticker + spinner + pulse via frame counting. |

---

## 3.11 Open Questions

1. **Charting library:** Should the team use SkiaSharp custom drawing or LiveCharts2 for the main performance area chart? (Recommendation: prototype both, compare visual fidelity.)

2. **Trade drawer host:** `DrawerFlyoutPresenter` (Uno Toolkit) vs custom `Popup` overlay? (Recommendation: start with DrawerFlyoutPresenter.)

3. **Font licensing:** Confirm Instrument Serif (Google Fonts OFL) is acceptable for the project's license requirements.

4. **Responsive scope:** Is mobile/tablet layout in v1 scope, or desktop-only with responsive as v2?

5. **Braille fallback:** If braille unicode proves unreliable cross-platform, should we invest in a SkiaSharp dot-matrix renderer, or cut the feature?

6. **Real-time data:** Is there an API contract for `IMarketDataService` beyond mocks? Does the team plan to integrate a real market data provider (e.g., Alpha Vantage, Polygon.io)?

7. **3D tilt scope:** Should the chart card tilt effect be included on touch platforms, or desktop/mouse only?

8. **Timer consolidation:** Should the 5 concurrent `DispatcherTimer` instances be merged into a single animation tick, or kept separate for clarity?

---

## 3.12 Implementation Priority

| Phase | Scope | Est. Effort | Dependencies |
|-------|-------|-------------|--------------|
| **P0: Shell** | `DashboardPage` layout grid, theme resources, font loading, project setup | 1 day | None |
| **P1: Static Data** | All models, `MockMarketDataService`, `DashboardModel` feeds wired | 1 day | P0 |
| **P2: Core Cards** | Holdings list, Watchlist (no expand), News feed — basic data display | 2 days | P1 |
| **P3: Chart** | `PerformanceChartControl` (SkiaSharp or LiveCharts2), timeframe selector | 2 days | P1 |
| **P4: Chart Linking** | `ChartTicker` state, holdings click → chart swap, back button, dynamic header | 1 day | P2, P3 |
| **P5: Watchlist Expand** | Expandable detail panel, OHLC grid, day range bar, View Chart + Trade buttons | 1 day | P2 |
| **P6: Custom Viz** | `SectorRingControl`, `VolumeChartControl`, `SparklineControl` | 3 days | P1 |
| **P7: Trade Drawer** | `TradeDrawer` overlay, `TradeDrawerModel`, full order form + confirmation | 2 days | P2 |
| **P8: Animations** | Card entrances, odometer, braille ticker/spinner/pulse/activity, ambient orbs | 3 days | P0–P7 |
| **P9: Polish** | Hover states, 3D tilt, ripple, search focus ring, responsive breakpoints, a11y | 2 days | P0–P8 |

**Total estimated effort: ~18 developer-days**

---

## Assumptions

- Mock data only — no live API integration in initial implementation
- Desktop-first target — responsive/mobile is a stretch goal
- MVUX is the architecture pattern (not MVVM)
- Uno Toolkit and Material theme are available (`UnoFeatures` includes `Toolkit`, `Material`, `MVUX`, `Skia`)
- SkiaSharp is available for custom drawing (`UnoFeatures` includes `Skia`)
- Font assets (Instrument Serif, IBM Plex Mono, Outfit) can be legally bundled
- The warm cream light theme is the only target for v1 — no dark mode toggle
- All data pre-loaded at startup — no incremental loading or pagination needed for 8-item lists

## Missing Details

- Exact breakpoint behavior for responsive layout — prototype only shows desktop
- Keyboard navigation flow and shortcut keys — prototype is mouse-only
- Error states for data loading failures — no loading/error states in prototype
- Pagination or infinite scroll strategy if watchlist grows beyond 8 items
- Real chart data fetching and caching strategy for live API integration
- Animation behavior on subsequent visits — should odometer always roll, or only on first load?
- Empty state designs — search with no results, no holdings, no news
