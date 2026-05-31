# 01 — Architecture Brief

**Project:** EnterpriseDashboard (codename: Observatory)
**Stack:** Uno Platform · .NET 9+ · C# · XAML · WinUI 3 Surface
**Targets:** Windows, WebAssembly, iOS, Android, macOS, Linux

---

## 1. Solution Structure

```
EnterpriseDashboard/
├── EnterpriseDashboard.sln
├── EnterpriseDashboard/
│   ├── App.xaml / App.xaml.cs
│   ├── GlobalUsings.cs
│   ├── EnterpriseDashboard.csproj
│   │
│   ├── Models/
│   │   ├── DataPoint.cs                   # record DataPoint(double X, double Y)
│   │   ├── ChartSeries.cs                 # record ChartSeries(string Label, SolidColorBrush Color, DataPoint[] Points)
│   │   ├── ChartConfig.cs                 # record ChartConfig(string Title, string Subtitle, string Stat, string Unit)
│   │   ├── BoxData.cs                     # record BoxData(string Label, double Min, double Q1, double Median, double Q3, double Max, double[] Outliers)
│   │   ├── SlopeItem.cs                   # record SlopeItem(string Label, double Before, double After)
│   │   ├── HeatmapCell.cs                 # record HeatmapCell(int Row, int Col, double Value)
│   │   └── RingMetric.cs                  # record RingMetric(string Label, double Value, double Radius)
│   │
│   ├── Services/
│   │   ├── IChartDataService.cs           # Interface: provides deterministic synthetic data for all 12 charts
│   │   ├── ChartDataService.cs            # Implementation: hardcoded arrays + formula generators
│   │   └── IScrollTriggerService.cs       # Interface: manages viewport intersection callbacks
│   │
│   ├── Presentation/
│   │   ├── DashboardModel.cs              # MVUX Model: master state, replay command, chart visibility flags
│   │   ├── DashboardPage.xaml / .cs       # Main scrollable page with chart grid
│   │   └── Shell.xaml / .cs               # App shell with fixed replay button overlay
│   │
│   ├── Controls/
│   │   ├── ChartCard.xaml / .cs           # Shared card wrapper: header + content presenter + annotation slot
│   │   ├── ChartAnnotation.xaml / .cs     # Algebrica-style italic explanation text
│   │   ├── SectionDivider.xaml / .cs      # Numbered section header with rule line
│   │   │
│   │   ├── Charts/
│   │   │   ├── LineChart.xaml / .cs       # 01 — Trace
│   │   │   ├── BarChart.xaml / .cs        # 02 — Emerge
│   │   │   ├── AreaChart.xaml / .cs       # 03 — Reveal
│   │   │   ├── ScatterChart.xaml / .cs    # 04 — Rainfall
│   │   │   ├── HBarChart.xaml / .cs       # 05 — Race
│   │   │   ├── HistogramChart.xaml / .cs  # 06 — Build (full-width)
│   │   │   ├── HeatmapChart.xaml / .cs    # 07 — Cascade
│   │   │   ├── ArcChart.xaml / .cs        # 08 — Breathe
│   │   │   ├── BoxPlotChart.xaml / .cs    # 09 — Unfold
│   │   │   ├── GaugeChart.xaml / .cs      # 10 — Tension & Release
│   │   │   ├── SlopeChart.xaml / .cs      # 11 — Bridge (full-width)
│   │   │   └── DotStripChart.xaml / .cs   # 12 — Scatter-Settle (full-width)
│   │   │
│   │   └── Behaviors/
│   │       └── ScrollTriggerBehavior.cs   # Attached behavior: viewport intersection → triggers animation
│   │
│   ├── Animation/
│   │   ├── IAnimatableChart.cs            # interface { void Play(); void Reset(); bool IsPlaying { get; } }
│   │   ├── AnimationOrchestrator.cs       # Coordinates replay: Reset all → scroll to top → re-arm triggers
│   │   ├── EasingCurves.cs               # Static factory for custom easing functions
│   │   ├── StrokeDashAnimator.cs          # Reusable stroke-dash-offset line-draw animation helper
│   │   └── StaggeredReveal.cs            # Reusable stagger-delay utility for element sequences
│   │
│   ├── Helpers/
│   │   ├── SplineBuilder.cs              # Catmull-Rom → PathGeometry with BezierSegments
│   │   ├── PathLengthCalculator.cs       # Walks PathGeometry and sums arc lengths
│   │   ├── PolarCoordinates.cs           # Angle/Radius → Point conversion, arc path generation
│   │   ├── BrightnessMapper.cs           # Value (0–100) → greyscale SolidColorBrush
│   │   └── SyntheticDataGenerator.cs     # Deterministic formula generators (heatmap, scatter, dot strip)
│   │
│   ├── Themes/
│   │   ├── Colors.xaml                    # Monochrome palette: 10 grey tokens as SolidColorBrush
│   │   ├── Typography.xaml                # TextBlock styles: Display, Stat, MonoLabel, MonoBody, Annotation, AxisTick
│   │   ├── CardStyles.xaml                # ChartCard container style, SectionDivider style
│   │   └── ButtonStyles.xaml              # ReplayButton style with hover states
│   │
│   └── Assets/
│       └── Fonts/
│           └── PlayfairDisplay-Regular.ttf  # Bundled serif display font
```

---

## 2. MVUX Model Layer

The app uses MVUX for state management. There is one top-level model (`DashboardModel`) that owns the data and replay command. Individual chart controls are **stateless** — they receive data via dependency properties and own only their animation state machine internally.

### 2.1 DashboardModel

```csharp
public partial record DashboardModel
{
    private readonly IChartDataService _data;

    public DashboardModel(IChartDataService data)
    {
        _data = data;
    }

    // Static chart data — these are IFeed because data is loaded once
    public IFeed<DataPoint[]> LineSeriesA => Feed.Async(async ct => _data.GetLinePrimary());
    public IFeed<DataPoint[]> LineSeriesB => Feed.Async(async ct => _data.GetLineReference());
    public IFeed<double[]> BarValues => Feed.Async(async ct => _data.GetBarData());
    public IFeed<DataPoint[]> AreaLayerA => Feed.Async(async ct => _data.GetAreaLayerA());
    public IFeed<DataPoint[]> AreaLayerB => Feed.Async(async ct => _data.GetAreaLayerB());
    public IFeed<DataPoint[]> ScatterPoints => Feed.Async(async ct => _data.GetScatterPoints());
    public IFeed<(string Label, double Value)[]> HBarItems => Feed.Async(async ct => _data.GetHBarItems());
    public IFeed<double[]> HistogramBins => Feed.Async(async ct => _data.GetHistogramBins());
    public IFeed<HeatmapCell[]> HeatmapData => Feed.Async(async ct => _data.GetHeatmapData());
    public IFeed<RingMetric[]> ArcMetrics => Feed.Async(async ct => _data.GetArcMetrics());
    public IFeed<BoxData[]> BoxPlotGroups => Feed.Async(async ct => _data.GetBoxPlotData());
    public IFeed<SlopeItem[]> SlopeItems => Feed.Async(async ct => _data.GetSlopeItems());
    public IFeed<double[]> DotStripPoints => Feed.Async(async ct => _data.GetDotStripData());

    // Replay trigger — IState increments to re-arm all scroll triggers
    public IState<int> ResetKey => State.Value(this, () => 0);

    // Replay command — bound to the floating replay button
    public async ValueTask Replay(CancellationToken ct)
    {
        var current = await ResetKey;
        await ResetKey.UpdateAsync(_ => current + 1, ct);
    }
}
```

### 2.2 Why MVUX Over MVVM

MVUX `IFeed<T>` provides:
- Automatic loading/error/data states (though all data is synchronous here, the pattern keeps the door open for real data sources)
- Source-generated bindable proxies — no manual `INotifyPropertyChanged`
- The `IState<int> ResetKey` propagates the replay trigger reactively through bindings

### 2.3 Chart Controls Are Dumb Views

Each chart `UserControl`:
- Declares `DependencyProperty` for its data (e.g., `public double[] Values { get; set; }`)
- Declares `DependencyProperty` for `ResetKey` (int) — when this changes, the chart calls `Reset()` then re-arms its scroll trigger
- Implements `IAnimatableChart` for orchestrated replay
- All rendering and animation logic is in code-behind (not the model) because animation is a view concern

---

## 3. Rendering Strategy

### 3.1 Decision Matrix

| Chart | Element Count | Geometry Complexity | Recommended Renderer | Rationale |
|---|---|---|---|---|
| Line | ~30 | Catmull-Rom paths | **XAML Shapes** | `Path` + `BezierSegment` maps directly; `StrokeDashOffset` animatable |
| Bar | ~12 | Rectangles | **XAML Shapes** | `Rectangle` elements with `Height`/`Y` animation |
| Area | ~30 | Spline + clip rect | **XAML Shapes** | `Path` with `RectangleGeometry.Clip` animation |
| Scatter | ~48 | Circles | **XAML Shapes** | `Ellipse` elements with transform animation |
| HBar | ~6 | Rectangles | **XAML Shapes** | `Rectangle.Width` animation, simplest chart |
| Histogram | ~22 | Rectangles + spline | **XAML Shapes** | Rectangles + one `Path` for density curve |
| Heatmap | 168 | Rectangles | **SKXamlCanvas** | 168 elements is too many for XAML shape tree; SkiaSharp batches efficiently |
| Arc | ~8 | Arc paths | **XAML Shapes** | 4 arcs with `StrokeDashOffset`, rotation via `RotateTransform` |
| Box Plot | ~40 | Lines + rects | **XAML Shapes** | Simple geometry, phased animation |
| Gauge | ~15 | Arc + line | **XAML Shapes** | Needle position animated via `DoubleAnimation` on translate/rotate |
| Slope | ~30 | Lines + circles | **XAML Shapes** | Endpoint interpolation via `DoubleAnimation` |
| Dot Strip | ~60 | Circles | **SKXamlCanvas** | 60 circles with staggered drops; SkiaSharp frame-based animation preferred |

### 3.2 XAML Shape Rendering

For the 10 charts using XAML shapes, each chart's `UserControl` contains a `Canvas` (or `Grid`) with shape children (`Path`, `Rectangle`, `Ellipse`, `Line`). Shapes are created in code-behind during the `Loaded` event based on bound data, then animated via `Storyboard` objects.

```xml
<!-- Example: LineChart.xaml structure -->
<UserControl x:Class="EnterpriseDashboard.Observatory.Charts.LineChart">
    <local:ChartCard Title="{x:Bind Config.Title}" Subtitle="{x:Bind Config.Subtitle}"
                     Stat="{x:Bind Config.Stat}" Unit="{x:Bind Config.Unit}">
        <Canvas x:Name="ChartCanvas" />
    </local:ChartCard>
</UserControl>
```

### 3.3 SkiaSharp Rendering (Heatmap, Dot Strip)

For complex or high-element-count charts:

```xml
<SkiaSharp:SKXamlCanvas x:Name="SkiaCanvas" PaintSurface="OnPaintSurface" />
```

Animation is driven by a `DispatcherTimer` or `CompositionTarget.Rendering` callback that increments an elapsed-time value and calls `SkiaCanvas.Invalidate()`. The `OnPaintSurface` handler reads the elapsed time and computes interpolated positions.

```csharp
private double _elapsed = 0;
private DateTime _startTime;

private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
{
    var canvas = e.Surface.Canvas;
    canvas.Clear(SKColor.Parse("#131313"));

    _elapsed = (DateTime.Now - _startTime).TotalSeconds;

    foreach (var cell in _cells)
    {
        var delay = 0.12 + (cell.Row + cell.Col) * 0.032;
        var t = Math.Clamp((_elapsed - delay) / 0.35, 0, 1);
        var scaleY = EasingCurves.Spring(t);
        // Draw cell with computed scaleY...
    }
}
```

---

## 4. Service Layer

### 4.1 IChartDataService

```csharp
public interface IChartDataService
{
    DataPoint[] GetLinePrimary();
    DataPoint[] GetLineReference();
    double[] GetBarData();
    DataPoint[] GetAreaLayerA();
    DataPoint[] GetAreaLayerB();
    DataPoint[] GetScatterPoints();
    (string Label, double Value)[] GetHBarItems();
    double[] GetHistogramBins();
    HeatmapCell[] GetHeatmapData();
    RingMetric[] GetArcMetrics();
    BoxData[] GetBoxPlotData();
    SlopeItem[] GetSlopeItems();
    double[] GetDotStripData();
}
```

All methods return hardcoded or formula-generated deterministic data. No randomness at runtime — every replay produces identical visuals.

### 4.2 Dependency Injection Registration

```csharp
// App.xaml.cs — ConfigureServices
private static void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IChartDataService, ChartDataService>();
}
```

MVUX source generation handles the model registration automatically when using `Feed.Async` and `State.Value`.

---

## 5. Scroll Trigger System

### 5.1 ScrollTriggerBehavior

An attached behavior that monitors the parent `ScrollViewer` and fires when the element enters the viewport.

```csharp
public class ScrollTriggerBehavior
{
    // Attached DPs
    public static readonly DependencyProperty IsEnabledProperty = ...;
    public static readonly DependencyProperty ThresholdProperty = ...; // 0.0 – 1.0, default 0.25
    public static readonly DependencyProperty IsInViewProperty = ...;  // read-only output
    public static readonly DependencyProperty ResetKeyProperty = ...;  // when changed, re-arms

    // On ResetKey changed → set IsInView = false, re-subscribe
    // On parent ScrollViewer.ViewChanged:
    //   1. Get element bounds relative to ScrollViewer viewport
    //   2. Compute visible fraction
    //   3. If fraction >= Threshold → set IsInView = true, unsubscribe (one-shot)
}
```

**Binding in XAML:**
```xml
<local:LineChart
    local:ScrollTriggerBehavior.IsEnabled="True"
    local:ScrollTriggerBehavior.Threshold="0.25"
    local:ScrollTriggerBehavior.ResetKey="{Binding ResetKey}" />
```

### 5.2 Chart Integration

Each chart watches `IsInView` via a `PropertyChangedCallback`:

```csharp
private static void OnIsInViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    if (e.NewValue is true && d is LineChart chart)
    {
        chart.Play();
    }
}
```

On `Reset()`:
- All `Storyboard` objects are stopped
- All shapes are set to their pre-animation state (opacity 0, scale 0, offset = totalLength, etc.)
- `IsInView` is reset to `false` by the behavior

---

## 6. Replay Flow

```
User taps [Replay] button
    │
    ├─ DashboardModel.Replay() increments ResetKey
    │
    ├─ Shell.xaml.cs scrolls ScrollViewer to top (ChangeView(0, 0, null))
    │
    ├─ ResetKey binding propagates to all 12 charts
    │     └─ Each chart's ScrollTriggerBehavior.ResetKey changes
    │           └─ Behavior sets IsInView = false, re-subscribes to ViewChanged
    │
    ├─ Each chart's Reset() is called:
    │     ├─ Stop all active Storyboards
    │     ├─ Snap all elements to pre-animation state
    │     └─ Clear SkiaSharp elapsed timers
    │
    └─ As user scrolls down, each chart re-enters viewport
          └─ IsInView fires → Play() begins the animation sequence
```

---

## 7. Platform Considerations

| Concern | Approach |
|---|---|
| **Font loading** | Bundle PlayfairDisplay-Regular.ttf as embedded resource. Reference via `ms-appx:///Assets/Fonts/PlayfairDisplay-Regular.ttf#Playfair Display` |
| **StrokeDashOffset animation** | Fully supported on all Uno targets (Skia renderer handles this natively) |
| **RenderTransform animations** | `ScaleTransform`, `RotateTransform`, `TranslateTransform` — all supported cross-platform |
| **Composition API easing** | `CubicBezierEasingFunction` supports control points outside 0–1 range (needed for spring/bounce). Fallback: use multi-keyframe `DoubleAnimationUsingKeyFrames` with `SplineDoubleKeyFrame` |
| **SkiaSharp** | Add `UnoFeatures` `Skia` in `.csproj`. `SKXamlCanvas` available on all targets |
| **Responsive layout** | Use `ResponsiveView` from Uno Toolkit for 1-column (< 768px) vs 2-column (≥ 768px) grid |
| **Noise overlay** | Full-viewport `Canvas` with `ImageBrush` using tiled noise SVG, `IsHitTestVisible="False"`, `Opacity="0.02"` |

---

## 8. Build Order

| Phase | Deliverable | Validates |
|---|---|---|
| 1 | `Colors.xaml` + `Typography.xaml` + `CardStyles.xaml` | Theme system renders correctly |
| 2 | `ChartCard` + `ChartAnnotation` + `SectionDivider` | Shared component shells |
| 3 | `ScrollTriggerBehavior` + `AnimationOrchestrator` | Viewport detection + replay |
| 4 | `SplineBuilder` + `PathLengthCalculator` | Catmull-Rom math |
| 5 | `BarChart` (simplest) | Full animation pipeline end-to-end |
| 6 | `HBarChart` | Elastic easing validation |
| 7 | `LineChart` | Spline + stroke-dash + chase timing |
| 8 | `AreaChart` | Clip-rect animation |
| 9 | `GaugeChart` | Multi-phase animation state machine |
| 10 | `ScatterChart` | Staggered random-delay drops |
| 11 | `BoxPlotChart` | Four-phase sequential unfold |
| 12 | `HistogramChart` | Riemann-sum sweep + delayed density curve |
| 13 | `ArcChart` | Rotation + draw combination |
| 14 | `SlopeChart` | Three-phase bridge animation |
| 15 | `HeatmapChart` (SkiaSharp) | SKXamlCanvas + timer-driven animation |
| 16 | `DotStripChart` (SkiaSharp) | Drop physics + stacking |
| 17 | `DashboardPage` + `Shell` | Full composition + replay button |
| 18 | Noise overlay + polish | Final texture pass |
