# 03 — Animation & Interaction Brief

**Project:** EnterpriseDashboard (codename: Observatory)
**Principle:** Each chart's animation is a metaphor for what the chart *does*. The motion teaches you how to read the data.

---

## 1. Easing Curve Library

Four custom easing curves used across all 12 charts. Each has a psychological character.

### 1.1 Curve Definitions

| Name | Cubic Bezier | Character | Used By |
|---|---|---|---|
| **Spring** | `(0.34, 1.56, 0.64, 1.0)` | Overshoot & settle — energy then control | Dot pop-in, scale reveals |
| **Smooth** | `(0.22, 0.61, 0.36, 1.0)` | Gentle deceleration — confidence without aggression | Line draw, path reveals, arc draw |
| **Bounce** | `(0.68, -0.55, 0.27, 1.55)` | Double overshoot — playful elasticity | Bar growth, elastic width snap |
| **Snap** | `(0.85, 0.0, 0.15, 1.0)` | Sharp acceleration/deceleration — mechanical precision | Clip-path wipe |
| **Out** | `(0.16, 1.0, 0.30, 1.0)` | Fast exit to rest — gravity settling | Dot rainfall drops, line endpoint reach |

### 1.2 WinUI Implementation

**Storyboard approach (control points clamped 0–1):**
Use `SplineDoubleKeyFrame` for curves where control points are within 0–1 range (`Smooth`, `Snap`):
```xml
<DoubleAnimationUsingKeyFrames>
    <SplineDoubleKeyFrame KeyTime="0:0:2.2" Value="0"
        KeySpline="0.22,0.61 0.36,1.0" />
</DoubleAnimationUsingKeyFrames>
```

**Composition approach (control points outside 0–1):**
For `Spring` (y₁=1.56) and `Bounce` (y₁=-0.55, y₂=1.55), use the Composition API:
```csharp
var compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;
var easing = compositor.CreateCubicBezierEasingFunction(
    new Vector2(0.34f, 1.56f),
    new Vector2(0.64f, 1.0f)
);
var animation = compositor.CreateScalarKeyFrameAnimation();
animation.InsertKeyFrame(1.0f, targetValue, easing);
animation.Duration = TimeSpan.FromSeconds(0.45);
ElementCompositionPreview.GetElementVisual(element).StartAnimation("Scale.X", animation);
```

**Fallback approach (if Composition is unavailable):**
Approximate overshoot using multi-keyframe storyboards:
```xml
<!-- Spring approximation: overshoot to 1.12 at 60%, settle to 1.0 at 100% -->
<DoubleAnimationUsingKeyFrames>
    <SplineDoubleKeyFrame KeyTime="0:0:0.27" Value="1.12" KeySpline="0.34,0.6 0.64,1.0" />
    <SplineDoubleKeyFrame KeyTime="0:0:0.45" Value="1.0" KeySpline="0.2,0.0 0.3,1.0" />
</DoubleAnimationUsingKeyFrames>
```

### 1.3 EasingCurves.cs

```csharp
public static class EasingCurves
{
    // For SkiaSharp timer-driven animation: evaluate the cubic bezier at time t
    public static double Evaluate(double t, double x1, double y1, double x2, double y2)
    {
        // Newton-Raphson method to find t_bezier for given t_time
        // then evaluate y(t_bezier)
        // Standard implementation: ~5 iterations for convergence
    }

    public static double Spring(double t) => Evaluate(t, 0.34, 1.56, 0.64, 1.0);
    public static double Smooth(double t) => Evaluate(t, 0.22, 0.61, 0.36, 1.0);
    public static double Bounce(double t) => Evaluate(t, 0.68, -0.55, 0.27, 1.55);
    public static double Snap(double t) => Evaluate(t, 0.85, 0.0, 0.15, 1.0);
    public static double Out(double t) => Evaluate(t, 0.16, 1.0, 0.30, 1.0);
}
```

---

## 2. Scroll Trigger System

### 2.1 State Machine

```
                   ┌──────────┐
          reset    │          │
       ┌──────────▶│  ARMED   │
       │           │          │
       │           └────┬─────┘
       │                │ element enters viewport
       │                │ (visible fraction ≥ threshold)
       │                ▼
       │           ┌──────────┐
       │           │          │
       │           │ TRIGGERED │──▶ calls chart.Play()
       │           │          │
       │           └────┬─────┘
       │                │ (one-shot: unsubscribes from scroll events)
       │                ▼
       │           ┌──────────┐
       │           │          │
       └───────────│  PLAYED  │
     ResetKey++    │          │
                   └──────────┘
```

### 2.2 Threshold Calculation

```csharp
// In ScrollViewer.ViewChanged handler:
var transform = element.TransformToVisual(scrollViewer);
var position = transform.TransformPoint(new Point(0, 0));
var elementRect = new Rect(position, new Size(element.ActualWidth, element.ActualHeight));
var viewportRect = new Rect(0, 0, scrollViewer.ViewportWidth, scrollViewer.ViewportHeight);

// Intersection
var intersection = RectHelper.Intersect(elementRect, viewportRect);
if (intersection != Rect.Empty)
{
    var visibleFraction = (intersection.Width * intersection.Height) /
                          (elementRect.Width * elementRect.Height);
    if (visibleFraction >= Threshold)
    {
        IsInView = true;
        // Unsubscribe
    }
}
```

### 2.3 Reduced Motion Check

Before any animation starts:
```csharp
var settings = new UISettings();
if (!settings.AnimationsEnabled)
{
    // Skip animation — render final state immediately
    RenderFinalState();
    return;
}
```

---

## 3. Chart Animation Specifications

Each chart spec below defines:
- **Metaphor** — the psychological satisfaction the motion provides
- **State table** — every visual element's pre/post animation values
- **Sequence timeline** — ordered steps with exact delays, durations, and easing
- **Annotation timing** — when the explanatory text appears

---

### 3.1 Line Chart — "Trace"

**Metaphor:** Following a pen trace a journey. You watch the story unfold in real-time. The satisfaction is *witnessing the path emerge*.

**Pre-animation state (all elements):**
| Element | Property | Initial Value |
|---|---|---|
| Grid lines | Opacity | 0 |
| Axis labels | Opacity | 0 |
| Line A (primary path) | StrokeDashOffset | totalLength (hidden) |
| Line B (reference path) | StrokeDashOffset | totalLength (hidden) |
| Area gradient fill | Opacity | 0 |
| Dot A[i] | Scale | 0 |
| Dot A[i] | Opacity | 0 |
| Glow ring A[i] | Scale | 0 |
| Glow ring A[i] | Opacity | 0 |
| Dot B[i] | Scale | 0 |
| Dot B[i] | Opacity | 0 |
| Annotation | Opacity | 0 |

**Post-animation state (final resting values):**
| Element | Property | Final Value |
|---|---|---|
| Grid lines | Opacity | 1 |
| Axis labels | Opacity | 1 |
| Line A | StrokeDashOffset | 0 (fully drawn) |
| Line B | StrokeDashOffset | 0 (fully drawn) |
| Area gradient fill | Opacity | 1 |
| Dot A[i] | Scale | 1 |
| Dot A[i] | Opacity | 1 |
| Glow ring A[i] | Scale | 1 |
| Glow ring A[i] | Opacity | 0.06 |
| Dot B[i] | Scale | 1 |
| Dot B[i] | Opacity | 1 |
| Annotation | Opacity | 0.85 |

**Animation sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Grid + labels | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Line A path | StrokeDashOffset | totalLen | 0 | 300ms | 2400ms | Smooth |
| 2 | Line B path | StrokeDashOffset | totalLen | 0 | 500ms | 2400ms | Smooth |
| 3 | Dot A[i] | ScaleX, ScaleY | 0 | 1 | `300 + (i/11)×2200` ms | 450ms | Spring |
| 3 | Dot A[i] | Opacity | 0 | 1 | same | 450ms | Spring |
| 3 | Glow A[i] | ScaleX, ScaleY | 0 | 1 | same | 500ms | Spring |
| 3 | Glow A[i] | Opacity | 0 | 0.06 | same | 500ms | Spring |
| 4 | Dot B[i] | Scale + Opacity | 0→1 | | `500 + (i/11)×2200` ms | 400ms | Spring |
| 5 | Area fill | Opacity | 0 | 1 | 2000ms | 1200ms | EaseOut |
| 6 | Peak annotation | Opacity | 0 | 0.85 | 2600ms | 800ms | EaseOut |

**Key detail:** Dot A[i] delay is computed as `300ms + (i/11) × 2200ms`. This syncs with Line A's draw: when the line reaches month `i`, the dot materializes. The dots "chase" the pen.

**Peak annotation position:** 10px right and 28px above the peak data point. Appears 2.6s in, after the peak dot has landed.

---

### 3.2 Bar Chart — "Emerge"

**Metaphor:** Growth from nothing. Bars rise from the baseline like buildings under construction. The center-outward ripple suggests controlled expansion — order emerging from a seed.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Bar[i] | Height | 0 |
| Bar[i] | Y | baseline (plotTop + plotHeight) |
| Month label[i] | Opacity | 0 |
| Annotation | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Grid lines | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Bar[i] | Height | 0 | barHeight | `150 + |i-5.5|×80` ms | 900ms | **Bounce** |
| 1 | Bar[i] | Y | baseline | targetY | same | same | **Bounce** |
| 2 | Label[i] | Opacity | 0 | 1 | delay[i]+300ms | 400ms | EaseOut |
| 3 | Annotation | Opacity | 0 | 0.85 | 1200ms | 600ms | EaseOut |

**Center index:** 5.5 (between June and July).
- June (i=5): delay = 150 + 0.5×80 = 190ms
- Dec (i=11): delay = 150 + 5.5×80 = 590ms

**Bounce easing** makes bars overshoot their target height by ~12% then settle. This mimics elastic material.

---

### 3.3 Area Chart — "Reveal"

**Metaphor:** Unveiling hidden terrain. A curtain sweeps left-to-right, revealing a landscape that was always there but concealed. The satisfaction is *discovery* — seeing what was beneath the surface.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Clip rectangle | Width | 0 |
| All paths/fills | Rendered but clipped | Invisible behind clip |
| Dot[i] | Scale | 0 |
| Annotation | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Grid + labels | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Clip RectangleGeometry | Width | 0 | plotWidth | 200ms | 2400ms | **Snap** |
| 2 | Dot[i] | Scale + Opacity | 0→1 | | `200 + (i/11)×2400` ms | 400ms | Spring |
| 3 | Annotation | Opacity | 0 | 0.85 | 2800ms | 800ms | EaseOut |

**Clip implementation:**
```xml
<Canvas x:Name="AreaContent">
    <Canvas.Clip>
        <RectangleGeometry x:Name="ClipRect" Rect="0,0,0,200" />
    </Canvas.Clip>
</Canvas>
```
Animate `ClipRect.Rect` from `(plotLeft, 0, 0, canvasHeight)` to `(plotLeft, 0, plotWidth, canvasHeight)`.

**Snap easing** creates a sense of mechanical precision — like a stage curtain operated by a motor.

---

### 3.4 Scatter Chart — "Rainfall"

**Metaphor:** Order emerging from chaos. Dots fall from above like rain, each landing at its correct Y position. Once settled, the trend line reveals the hidden structure — the r² = 0.87 relationship that was invisible in the chaos of individual drops.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Point[i] | CenterY | plotTop - 10 (above chart) |
| Point[i] | Opacity | 0 |
| Trend line | StrokeDashOffset | totalLength |
| Annotation | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Grid lines | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Point[i] | TranslateY | (above chart) | targetY | `order[i]` ms | 600ms | **Out** |
| 1 | Point[i] | Opacity | 0 | 0.85 | same | 150ms | EaseOut |
| 1 | Glow[i] | Scale | 2.5 | 1.0 | same | 500ms | **Bounce** |
| 1 | Glow[i] | Opacity | 0 | 0.04 | same | 300ms | EaseOut |
| 2 | Trend line | StrokeDashOffset | totalLen | 0 | 2200ms | 1000ms | Smooth |
| 3 | Annotation | Opacity | 0 | 0.85 | 2500ms | 600ms | EaseOut |

**Delay formula:** `400 + ((x×7 + y×13 + i×37) % 1800)` ms — deterministic pseudo-random.

**Unique glow behavior:** The glow ring starts LARGE (2.5×) and shrinks inward. This creates a "shockwave settling" effect — each dot impacts with force then calms down.

**Trend line appears LAST:** After all dots settle (2200ms), the trend line draws through them, revealing the correlation structure. The sequence is: chaos → settle → pattern. This mirrors how statistical insight works.

---

### 3.5 Horizontal Bar Chart — "Race"

**Metaphor:** Competition and ranking. Bars slide in like racers crossing a finish line, with elastic overshoot (they briefly pass their target then snap back). The top-to-bottom stagger reinforces the ranking.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Bar[i] | Width | 0 |
| Label[i] | Opacity | 0 |
| Value label[i] | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Label[i] | Opacity | 0 | 1 | `150 + i×100` ms | 400ms | EaseOut |
| 1 | Bar[i] | Width | 0 | targetWidth | `150 + i×100` ms | 700ms | **Bounce** |
| 2 | Value[i] | Opacity | 0 | 1 | delay[i]+500ms | 300ms | EaseOut |
| 3 | Annotation | Opacity | 0 | 0.85 | 1200ms | 600ms | EaseOut |

**Bounce overshoot:** Each bar extends ~15% past its target width then snaps back to the correct position.

---

### 3.6 Histogram — "Build"

**Metaphor:** Construction → completion. Bins rise like Riemann sums being computed — a direct visual reference to Algebrica's integral illustrations. Once the discrete approximation is built, the smooth density curve crowns it, showing the continuous truth that the discrete samples approximate. The satisfaction is *convergence* — rough blocks becoming a smooth curve.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Bin[i] | Height | 0 |
| Bin[i] | Y | baseline |
| Mean line | Opacity | 0 |
| Density curve | StrokeDashOffset | totalLength |
| 1σ annotation | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Grid + labels | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Bin[i] | Height + Y | 0 → binHeight | `200 + i×60` ms | 600ms | **Out** |
| 2 | Mean dashed line | Opacity | 0 | 0.5 | 1400ms | 500ms | EaseOut |
| 2 | μ label | Opacity | 0 | 1 | 1500ms | 500ms | EaseOut |
| 3 | Density curve | StrokeDashOffset | totalLen | 0 | 1800ms | 1600ms | **Smooth** |
| 4 | 1σ annotation | Opacity | 0 | 0.85 | 3000ms | 800ms | EaseOut |

**Left-to-right sweep:** Bins 0→19 animate sequentially with 60ms stagger. Total sweep: 200 + 19×60 = 1340ms for the last bin to START. The last bin finishes at ~1940ms. The density curve begins at 1800ms, overlapping slightly with the final bins — this overlap is intentional, creating the feeling of the curve "emerging from" the bins.

**Full width:** This chart spans both grid columns. The bins are narrower (plotWidth/20 - 2px gap).

---

### 3.7 Heatmap — "Cascade"

**Metaphor:** Spreading activation. Like flipping tiles on a board or a signal propagating through a network. The diagonal wave creates a sense of information flowing from source to periphery.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Cell[d][h] | ScaleY | 0 |
| Cell[d][h] | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 1 | Cell[d][h] | ScaleY | 0 | 1 | `120 + (d+h)×32` ms | 300ms | **Spring** |
| 1 | Cell[d][h] | Opacity | 0 | 1 | same | 200ms | EaseOut |

**Transform origin:** Bottom-center of each cell (in XAML: `RenderTransformOrigin="0.5, 1.0"`). Cells grow upward from their base.

**Diagonal wave:** Delay = `row + column`. This creates a wave front that moves from top-left (Mon 00:00) to bottom-right (Sun 23:00). 

**Timing:** First cell fires at 120ms. Last cell (d=6, h=23) fires at `120 + 29×32 = 1048ms`. Last cell finishes at ~1348ms. Total animation: ~1.35s.

**Renderer:** SKXamlCanvas. The 168-cell animation is driven by a `DispatcherTimer` at ~60fps.

---

### 3.8 Arc Gauge — "Breathe"

**Metaphor:** Turbine startup. Rings spin from scattered starting angles into alignment while simultaneously drawing their arcs. The rotation + draw creates a sense of machinery powering up — energy charging into the system.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Ring group[i] | RotateTransform.Angle | `-(80 + i×25)°` |
| Filled arc[i] | StrokeDashOffset | totalLength |
| Center percentage | Opacity | 0 |
| Legend items | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 1 | Ring group[i] | Rotation | `-(80+i×25)°` | 0° | `300 + i×220` ms | 1300ms | **Smooth** |
| 1 | Filled arc[i] | StrokeDashOffset | totalLen | 0 | same | 1300ms | **Smooth** |
| 2 | Center text | Opacity | 0 | 1 | 1200ms | 500ms | EaseOut |
| 3 | Legend items | Opacity | 0 | 1 | 1400ms (stagger 100ms each) | 300ms | EaseOut |

**Ring rotation origins:** Each ring group's `RenderTransformOrigin` is the center of the gauge (`0.5, 0.5`). The staggered starting angles create a "scattered → aligned" trajectory.

---

### 3.9 Box Plot — "Unfold"

**Metaphor:** Structure revealing itself from the center outward. Like opening a sealed letter or a flower blooming — the most essential information (median) appears first, then progressively more context unfolds around it. The satisfaction is *understanding building layer by layer*.

This is a **four-phase** animation. Each phase adds one layer of statistical context.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Median line[i] | ScaleY | 0 |
| Median line[i] | Opacity | 0 |
| Box[i] | Width | 0 (centered on median X) |
| Box[i] | X | medianX |
| Whisker lines[i] | X endpoints | both at medianX |
| Whisker caps[i] | Opacity | 0 |
| Outlier dots[i] | Scale | 0 |
| Outlier dots[i] | Opacity | 0 |
| Annotation | Opacity | 0 |

**Phase sequence:**

| Phase | Delay | Target | Property | From | To | Duration | Easing |
|---|---|---|---|---|---|---|---|
| **1: Medians** | 300ms + i×80ms | Median line[i] | ScaleY + Opacity | 0→1 | | 400ms | **Spring** |
| **2: Boxes** | 900ms | Box[i] X | medianX | q1X | 600ms | **Out** |
| | | Box[i] Width | 0 | (q3X - q1X) | 600ms | **Out** |
| | | Box fill | Opacity 0 | 0.06 | 600ms | EaseOut |
| **3: Whiskers** | 1500ms | Left whisker X1 | medianX | minX | 500ms | **Out** |
| | | Right whisker X2 | medianX | maxX | 500ms | **Out** |
| | | Whisker caps | Opacity 0 | 0.7 | 300ms | EaseOut |
| **4: Outliers** | 2000ms | Outlier[j] | Scale + Opacity | 0→1 | 350ms + j×100ms | **Spring** |
| **5: Annotation** | 2200ms | Annotation | Opacity | 0 | 0.85 | 600ms | EaseOut |

**Phase 1** is the hook — five median lines appear in rapid succession (80ms stagger), establishing the comparison baseline.

**Phase 2** expands the boxes outward FROM the medians. The box literally grows out of the median line, teaching the viewer that the IQR surrounds the center.

**Phase 3** extends whiskers, showing the full range. The whiskers animate as line endpoints moving outward.

**Phase 4** (outliers) uses open circles (no fill, 1px stroke) to visually separate them from the main data body.

---

### 3.10 Gauge — "Tension & Release"

**Metaphor:** Physical instrument settling under force. The needle swings past the target (tension), then bounces back to rest (release). This mimics a real analog gauge needle's momentum. The satisfaction is *physical correctness* — it feels heavy and real.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Background arc | Rendered | Static (no animation) |
| Filled arc | StrokeDashOffset | totalLength |
| Tick marks[i] | Opacity | 0 |
| Tick labels[i] | Opacity | 0 |
| Needle | Endpoint (x2, y2) | At 0° (fully left) |
| Hub circle | Opacity | 0 |
| Status label | Opacity | 0 |

**Three-phase needle animation:**

```
Phase 0 (idle)          Phase 1 (overshoot)      Phase 2 (settle)
    ╱                       │                         ╲
   ╱                        │                          ╲
  ╱                         │                           ╲ 74%
 ╱ 0%                       │ 84%                        ╲
─────────                ──────────                 ──────────
```

| Phase | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| Arc | Filled arc | StrokeDashOffset | totalLen | 0 | 300ms | 1500ms | Smooth |
| Ticks | Tick[i] | Opacity | 0 | 1 | `200 + i×40` ms | 300ms | EaseOut |
| Hub | Center circle | Opacity | 0 | 1 | 400ms | 400ms | EaseOut |
| **1** | Needle endpoint | (x2, y2) | pos(0%) | pos(**84%**) | 500ms | 800ms | **Smooth** |
| **2** | Needle endpoint | (x2, y2) | pos(84%) | pos(**74%**) | 1400ms | 500ms | **Bounce** |
| Label | Status text | Opacity | 0 | 1 | 1800ms | 500ms | EaseOut |

**Needle position calculation:**
```csharp
double NeedleAngle(double fraction)
{
    double startAngle = Math.PI;   // 180° (left)
    double endAngle = 0;           // 0° (right)
    return startAngle + (endAngle - startAngle) * fraction;
}
```

The overshoot fraction (84%) and target (74%) are hardcoded for this synthetic data. The difference (10%) creates enough visual swing to feel real without being comedic.

---

### 3.11 Slope Chart — "Bridge"

**Metaphor:** Connection across a gap. Left-side dots land (establishing the "before" state), lines reach across the void to the right side, then right-side dots materialize on arrival. The satisfaction is *bridging* — two separate moments in time becoming connected and comparable.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Left dot[i] | Scale + Opacity | 0 |
| Left value label[i] | Opacity | 0 |
| Left item label[i] | Opacity | 0 |
| Connecting line[i] | X2, Y2 | (x1, y1) — collapsed to left point |
| Connecting line[i] | Opacity | 0 |
| Right dot[i] | Scale + Opacity | 0 |
| Right value label[i] | Opacity | 0 |
| Period labels ("BEFORE"/"AFTER") | Opacity | 0 |
| Annotation | Opacity | 0 |

**Three-phase sequence:**

| Phase | Target | Property | From | To | Delay | Duration | Easing | Per-item stagger |
|---|---|---|---|---|---|---|---|---|
| Labels | "BEFORE"/"AFTER" | Opacity | 0 | 1 | 100ms | 400ms | EaseOut | — |
| **1: Left dots** | Left dot[i] | Scale + Opacity | 0→1 | | `300 + i×80` ms | 400ms | **Spring** | 80ms |
| 1 | Left value[i] | Opacity | 0 | 1 | delay+200ms | 300ms | EaseOut | — |
| 1 | Item label[i] | Opacity | 0 | 0.9 (rising) / 0.5 (declining) | delay+200ms | 300ms | EaseOut | — |
| **2: Lines reach** | Line[i] X2 | x1 | x2 (right column) | `900 + i×80` ms | 800ms | **Out** | 80ms |
| 2 | Line[i] Y2 | y1 | y2 (right position) | same | same | same | — |
| 2 | Line[i] Opacity | 0 | 0.7 (rising) / 0.35 (declining) | same | same | same | — |
| **3: Right dots** | Right dot[i] | Scale + Opacity | 0→1 | | `1600 + i×80` ms | 400ms | **Spring** | 80ms |
| 3 | Right value[i] | Opacity | 0 | 1 | delay+200ms | 300ms | EaseOut | — |
| 4 | Annotation | Opacity | 0 | 0.85 | 2600ms | 600ms | EaseOut | — |

**Visual encoding of direction:**
- Rising slopes (after > before): Line color = `Light`, opacity = 0.7. Labels and dots brighter.
- Declining slopes (after < before): Line color = `Grey`, opacity = 0.35. Labels dimmer.

This brightness encoding lets you instantly distinguish winners from losers without reading numbers.

---

### 3.12 Dot Strip — "Scatter-Settle"

**Metaphor:** Individual voices forming a collective pattern. Each dot is one observation — it falls from above and stacks at its position. As dots accumulate, the distribution shape emerges from the sum of individuals. The satisfaction is *emergence* — from atoms to structure.

**Pre-animation state:**
| Element | Property | Initial |
|---|---|---|
| Dot[i] | CenterY | above chart (plotTop - 5) |
| Dot[i] | Opacity | 0 |
| Mean line | Opacity | 0 |
| Mean label | Opacity | 0 |
| Annotation | Opacity | 0 |
| Axis | Opacity | 0 |

**Sequence:**

| Step | Target | Property | From | To | Delay | Duration | Easing |
|---|---|---|---|---|---|---|---|
| 0 | Axis + labels | Opacity | 0 | 1 | 0ms | 400ms | EaseOut |
| 1 | Dot[i] | CenterY | aboveChart | stackedY | `order[i]` ms | 500ms | **Out** |
| 1 | Dot[i] | Opacity | 0 | 0.85 | same | 150ms | EaseOut |
| 2 | Mean line | Opacity | 0 | 0.4 | 2500ms | 500ms | EaseOut |
| 2 | Mean label | Opacity | 0 | 1 | 2600ms | 500ms | EaseOut |
| 3 | Annotation | Opacity | 0 | 0.85 | 2800ms | 600ms | EaseOut |

**Stacking calculation:**
```csharp
// Bin dots by value (bin size = 5 units)
var bins = new Dictionary<int, int>();
foreach (var point in points)
{
    int bin = (int)Math.Round(point / 5.0);
    bins[bin] = bins.GetValueOrDefault(bin) + 1;
    int stackRow = bins[bin] - 1;
    double y = baselineY - stackRow * (dotRadius * 2.2);
    // This dot renders at (scaleX(point), y)
}
```

**Delay formula:** `300 + ((Math.Round(value × 7) + i × 29) % 2200)` ms — deterministic but appears random.

**Brightness encoding:** Dots within 1σ of the mean (|value - 50| < 16) are `Light`. Others are `Grey`. This subtly highlights the central tendency without an explicit annotation.

**Renderer:** SKXamlCanvas. The 60-dot drop animation is driven by a timer.

---

## 4. Annotation System

### 4.1 Content Per Chart

| Chart | Annotation Text | Position | Appears At |
|---|---|---|---|
| Line | "Peak observed in Sep. Primary channel diverges from reference baseline." | Right of peak dot | 2600ms |
| Bar | "Highest throughput recorded in Aug at 91 ops." | Above peak bar | 1200ms |
| Area | "Combined layer sum. Upper boundary shows total load across both channels." | Near right edge, above top line | 2800ms |
| Scatter | "Positive linear trend. Most observations cluster tightly around the regression line." | Above trend line, right quadrant | 2500ms |
| HBar | "Alpha leads by a clear margin over the next rank." | Above first bar | 1200ms |
| Histogram | "≈68% of observations fall within one standard deviation of the mean." | Above the ±1σ range | 3000ms |
| Heatmap | *(no annotation — the visual is self-explanatory)* | — | — |
| Arc | *(no annotation — legend provides context)* | — | — |
| Box Plot | "Box spans Q1–Q3 (interquartile range). Whiskers extend to min/max within 1.5×IQR." | Above the box row, right side | 2200ms |
| Gauge | *(status label "NOMINAL" serves as the annotation)* | Below center hub | 1800ms |
| Slope | "Lines rising left-to-right indicate improvement. Declining slopes show regression between periods." | Bottom center | 2600ms |
| Dot Strip | "Each dot is one observation. Stacking reveals the density — taller columns indicate higher frequency." | Right side, above the distribution peak | 2800ms |

### 4.2 Annotation Animation

All annotations use the same animation:
- Property: `Opacity`
- From: `0`
- To: `0.85`
- Duration: `600–800ms`
- Easing: `EaseOut`
- Delay: Late in the chart's sequence (after data has settled)

---

## 5. Replay Animation

When the user taps [Replay]:

| Step | Action | Duration |
|---|---|---|
| 1 | `ScrollViewer.ChangeView(null, 0, null, false)` | ~400ms (smooth scroll) |
| 2 | All charts receive `ResetKey++` via binding | Immediate |
| 3 | Each chart's `Reset()` fires: stops Storyboards, snaps to pre-animation state, re-arms scroll trigger | Immediate |
| 4 | Replay button itself does a brief pulse: scale 0.95 → 1.0 | 200ms, Spring |
| 5 | User scrolls down; charts re-trigger as they enter viewport | Per-chart timing |

**Replay button hover interaction:**
```
Default → Hover: Background Dark→Border, BorderColor Border→Grey, 200ms ease-out
Hover → Default: Reverse, 200ms ease-out
Default → Pressed: Scale 0.97, Background Grid, instant
Pressed → Default: Scale 1.0, 150ms Spring
```

---

## 6. Total Animation Durations

| Chart | First element animates at | Last element finishes at | Total duration |
|---|---|---|---|
| Line | 0ms (grid) | ~3400ms (annotation) | 3.4s |
| Bar | 0ms | ~2100ms | 2.1s |
| Area | 0ms | ~3600ms | 3.6s |
| Scatter | 0ms | ~3100ms | 3.1s |
| HBar | 0ms | ~1800ms | 1.8s |
| Histogram | 0ms | ~3800ms | 3.8s |
| Heatmap | 120ms | ~1350ms | 1.35s |
| Arc | 300ms | ~1900ms | 1.9s |
| Box Plot | 300ms | ~2800ms | 2.8s |
| Gauge | 200ms | ~2300ms | 2.3s |
| Slope | 100ms | ~3200ms | 3.2s |
| Dot Strip | 0ms | ~3400ms | 3.4s |

**Design intent:** No chart exceeds 4 seconds. The fastest (Heatmap, 1.35s) provides an energetic counterpoint to the slowest (Histogram, 3.8s). Users scrolling at a moderate pace will see each chart complete before the next one begins.
