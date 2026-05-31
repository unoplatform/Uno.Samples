# Meridian Capital Terminal — Fukasawa Interaction Brief

## Uno Platform · Design, Interaction & Implementation Spec

**Target stack:** C# · WinUI 3 · XAML · Uno Platform (Skia renderer)
**Design philosophy:** Naoto Fukasawa — "without thought" design. Every motion is purposeful but invisible. The user never thinks "that's a nice animation" because the interface simply feels alive and attentive.

**Shared platform primitives used across all interactions:**

| Primitive | Namespace / Package | Role |
|---|---|---|
| `Storyboard` / `DoubleAnimation` | `Microsoft.UI.Xaml.Media.Animation` | All GPU-bound property animations (Opacity, RenderTransform) |
| `CompositeTransform` | `Microsoft.UI.Xaml.Media` | TranslateY, ScaleX/ScaleY — must be set on `RenderTransform`, not in a `TransformGroup` |
| `VisualStateManager` | `Microsoft.UI.Xaml` | Pointer states (PointerOver, Normal, Pressed), data-conditional visual changes |
| `ShadowContainer` | `Uno.Toolkit.UI` | Inner/outer shadows with animatable BlurRadius, Opacity, Spread |
| `SKXamlCanvas` | `SkiaSharp.Views.Windows` | Custom-drawn elements (braille, charts, sparklines) via `OnPaintSurface` |
| `DispatcherTimer` | `Microsoft.UI.Xaml` | Periodic ticks for frame-based animations (braille, session warmth) |
| `ScrollViewer.ViewChanged` | `Microsoft.UI.Xaml.Controls` | Scroll position detection |
| `PointerMoved` / `PointerExited` | `UIElement` events | Mouse/touch tracking for tilt, magnetism, silence |
| Composition `HueRotationEffect` | `Windows.UI.Composition` (Skia backends) | Session warmth hue shift |
| `EasingFunctionBase` subclasses | `Microsoft.UI.Xaml.Media.Animation` | `BackEase`, `BounceEase`, `CircleEase`, `CubicEase`, `ElasticEase`, `PowerEase` |

**GPU-bound animation constraint (critical):** Uno only GPU-accelerates `Opacity` and `RenderTransform` (Translate, Rotate, Scale, Composite). All other property animations run on the UI thread. Every interaction below is designed within this constraint.

---

## 01 · Market Breathing

### Design intent

Card borders carry an imperceptible glow pulse during market hours (9:30 AM – 4:00 PM ET). After close, everything goes still. You never notice it running — you feel something is off when it stops.

### Visual spec

- **Glow:** Outer shadow on the chart card. Color `AccentGreen` at 3–4% opacity. BlurRadius oscillates between 0 and 6 on a 5-second sinusoidal cycle.
- **Market closed:** Shadow snaps to BlurRadius 0, Opacity 0 over 1.2s with `CubicEase` EaseOut. The card goes dead flat.
- **Scope:** Only the main chart card and the portfolio hero card. Not every card — restraint is key.

### States

| State | Trigger | Shadow |
|---|---|---|
| `MarketOpen` | `DateTime.Now` between 9:30–16:00 ET (weekdays) | Pulsing: BlurRadius 0↔6, Opacity 0↔0.04, RepeatBehavior Forever, Duration 5s |
| `MarketClosed` | Outside market hours or weekend | Static: BlurRadius 0, Opacity 0 |

### Uno implementation

```
Approach: ShadowContainer + Storyboard with RepeatBehavior="Forever"
```

- Wrap the chart card `Border` in a `utu:ShadowContainer`.
- Define a `utu:Shadow` with `IsInner="False"`, `Color="{StaticResource AccentGreenBrush}"`, `BlurRadius="0"`, `Opacity="0"`.
- In code-behind, use a `DispatcherTimer` (1-minute interval) that checks market hours. When market is open, start a `Storyboard` targeting the `Shadow.BlurRadius` (0→6→0) and `Shadow.Opacity` (0→0.04→0) with `AutoReverse="True"`, `RepeatBehavior="Forever"`, `Duration="0:0:5"`.
- On market close, `Storyboard.Stop()` and run a one-shot `DoubleAnimation` on both properties back to 0 over 1.2s with `CubicEase` EaseOut.

**Note:** `ShadowContainer.Shadow` properties are `DependencyProperty` and support `Storyboard` targeting. Since shadow is not a GPU-bound property, this runs on UI thread — acceptable at 5s period (one repaint per 2.5s).

**Fallback:** If `ShadowContainer` shadow animation performance is insufficient, use a `Border` with `BorderBrush` set to a `SolidColorBrush` whose `Opacity` is animated (GPU-bound via the Brush's `Opacity` property).

---

## 02 · Chart Data Settle

### Design intent

When the chart finishes its draw-in animation, the line overshoots by ~2–4% on Y then springs back to rest. Like an analog gauge needle finding its rest point.

### Visual spec

- **Phase 1 — Overshoot:** ScaleY jumps to 1.04, duration 100ms, no easing (snap).
- **Phase 2 — Settle:** ScaleY eases from 1.04 through 0.99 to 1.0, duration 600ms, `BackEase` EaseOut (natural overshoot on the way back).
- **Transform origin:** Bottom-center of the chart area (the X-axis baseline), so the chart stretches upward.

### States

| Phase | ScaleY | Duration | Easing |
|---|---|---|---|
| Drawing (recharts equivalent) | 0 → 1.0 | 800ms | Linear (handled by chart lib) |
| Overshoot | 1.0 → 1.04 | 100ms | None |
| Settle | 1.04 → 1.0 | 600ms | `BackEase` EaseOut, Amplitude 0.3 |
| Idle | 1.0 | — | — |

### Uno implementation

```
Approach: CompositeTransform.ScaleY + sequenced Storyboard
```

- The chart lives in an `SKXamlCanvas` (SkiaSharp-drawn area chart). Wrap it in a `Border` with `RenderTransformOrigin="0.5,1.0"` (bottom-center) and a `CompositeTransform` on `RenderTransform`.
- After the chart's data-draw animation completes (use a flag or timed callback from the chart rendering), fire a `Storyboard` with two sequential `DoubleAnimationUsingKeyFrames` on `CompositeTransform.ScaleY`:
  - KeyFrame at 0ms: `ScaleY = 1.0`
  - KeyFrame at 100ms: `ScaleY = 1.04` (LinearKeyFrame — snap)
  - KeyFrame at 400ms: `ScaleY = 0.99` (SplineKeyFrame with `BackEase` feel)
  - KeyFrame at 700ms: `ScaleY = 1.0` (SplineKeyFrame — settle)
- This is GPU-bound (ScaleTransform on RenderTransform) so it's silky.

**Trigger:** Fire on every data source change (timeframe switch, stock selection). The `SKXamlCanvas` calls `InvalidateVisual()` for the draw, and the settle storyboard begins 800ms later (matching the chart draw duration).

---

## 03 · Ink Spread

### Design intent

The active timeframe button's fill radiates outward from the exact click point — like pressing a thumb into wet paper.

### Visual spec

- A circular `Ellipse` spawns at the pointer coordinates relative to the button.
- Starts at `Scale(0)`, animates to `Scale(3)` over 300ms with `CubicEase` EaseOut.
- Fill: `AccentGreen` at 10% opacity.
- Simultaneously, the ellipse's `Opacity` fades from 1 → 0 over 400ms.
- The button's background color transitions to its active state behind the ink.

### States

| Phase | Visual |
|---|---|
| Tap/Click | Ellipse spawns at `(e.GetPosition(button).X, e.GetPosition(button).Y)` |
| Spreading (0–300ms) | Ellipse scales 0→3, opacity 1→0 |
| Active | Button background is `AccentGreen` at 8%, underline bar width 100% |

### Uno implementation

```
Approach: PointerPressed handler + Canvas overlay + ScaleTransform Storyboard
```

- Each timeframe `Button` has a `Canvas` overlay child (`IsHitTestVisible="False"`, `ClipToBounds="True"`).
- In the `PointerPressed` handler:
  1. Read `e.GetCurrentPoint(button).Position` → the click point.
  2. Create an `Ellipse` (Width/Height = button width × 3), set `RenderTransformOrigin="0.5,0.5"`, position it centered on the click point via `Canvas.Left/Top`.
  3. Set `RenderTransform` to a `CompositeTransform` with `ScaleX=0, ScaleY=0`.
  4. Add to Canvas, fire Storyboard:
     - `DoubleAnimation` on `ScaleX`: 0→1, 300ms, `CubicEase` EaseOut
     - `DoubleAnimation` on `ScaleY`: 0→1, 300ms, `CubicEase` EaseOut
     - `DoubleAnimation` on `Opacity`: 1→0, 400ms, `CubicEase` EaseIn
  5. On `Storyboard.Completed`, remove the `Ellipse` from Canvas.
- This is fully GPU-bound (Scale + Opacity on the Ellipse's RenderTransform).

---

## 04 · Weight Whisper

### Design intent

Hovering a holding card subtly shifts the main chart's gradient fill opacity proportional to that holding's portfolio weight. A subliminal connection — no explicit UI linking them.

### Visual spec

- **Default gradient opacity:** 12% at top stop, 0% at bottom.
- **On holding hover:** Top stop shifts to `(weight / maxWeight) * 0.25 + 0.05`. AAPL at 24% → ~0.30. JPM at 8% → ~0.13.
- **Transition:** Opacity change over 500ms with `CubicEase` EaseInOut.
- **Line stroke opacity:** Also shifts — base 0.8, on hover 0.6 + (weight/maxWeight) × 0.4.

### States

| State | Gradient Top Opacity | Line Opacity |
|---|---|---|
| No holding hovered | 0.12 | 0.80 |
| AAPL hovered (24%) | 0.30 | 1.00 |
| JPM hovered (8%) | 0.13 | 0.73 |
| Transition | 500ms CubicEase | 500ms CubicEase |

### Uno implementation

```
Approach: PointerEntered/Exited on holding items → code-behind updates chart gradient
```

- The chart area fill is drawn in `SKXamlCanvas.OnPaintSurface` using an `SKPaint` with a `SKShader.CreateLinearGradient`. The gradient's top stop alpha is stored as a `double` property on the chart control or its ViewModel.
- Each holding item's `PointerEntered` sets a `HoveredWeight` property on the ViewModel. `PointerExited` clears it.
- The chart control observes `HoveredWeight` and animates a local `_gradientAlpha` field using a manual spring (a `DispatcherTimer` at 16ms ticking a spring physics calculation — same `stiffness=0.08, damping=0.82` pattern from the React prototype).
- On each spring tick, call `canvas.InvalidateVisual()` to repaint with the new alpha.
- **Why not Storyboard?** The gradient is inside SkiaSharp, not a XAML `DependencyProperty`. A code-driven spring on a `DispatcherTimer` gives the same feel and stays inside the Skia draw loop.

**Alternative if chart is XAML-native:** If using a XAML `Path` + `LinearGradientBrush`, you can `Storyboard`-animate the `GradientStop.Opacity` directly (not GPU-bound, but at 500ms duration, one repaint is acceptable).

---

## 05 · Silence on Leave

### Design intent

When the cursor leaves the chart card, the card's content dips to 92% opacity for 600ms then recovers. The chart "exhales" — it knows you stopped looking.

### Visual spec

- **On PointerEntered:** Opacity snaps to 1.0 (150ms, ease).
- **On PointerExited:** After a 200ms delay, opacity dips to 0.92 then recovers to 1.0 over 600ms.
- **Target:** A wrapping element around the card's *content* (not the card itself — the border and shadow stay at full opacity).

### States

| State | Content Opacity | Duration |
|---|---|---|
| Hovering | 1.0 | 150ms ease-in |
| Exhale start (200ms after leave) | 0.92 | 200ms ease-out |
| Recovery | 1.0 | 400ms `CubicEase` EaseInOut |
| Idle (never hovered) | 1.0 | — |

### Uno implementation

```
Approach: PointerEntered/Exited + Opacity Storyboard on inner Grid
```

- The chart card's content sits in an inner `Grid` named `ChartContentLayer`.
- `PointerEntered` on the card: Start `Storyboard` → `ChartContentLayer.Opacity` to 1.0, 150ms.
- `PointerExited` on the card: Start a sequenced `Storyboard`:
  - `DoubleAnimationUsingKeyFrames` targeting `ChartContentLayer.Opacity`:
    - KeyFrame 0ms: Value 1.0
    - KeyFrame 200ms: Value 1.0 (the delay — holds current value)
    - KeyFrame 400ms: Value 0.92 (`CubicEase` EaseOut)
    - KeyFrame 800ms: Value 1.0 (`CubicEase` EaseInOut)
- This is GPU-bound (`Opacity` is always GPU-accelerated). Zero performance concern.

---

## 06 · Empty State Gravity

### Design intent

When the watchlist search returns zero results, the container gains an inward shadow and the border desaturates. It physically feels hollow and heavy. Results returning lifts the weight.

### Visual spec

- **Empty state:** Inner shadow appears — `IsInner="True"`, `BlurRadius="16"`, `OffsetY="6"`, `Opacity="0.06"`, Color black. Border color transitions from `--border` to `--surface-active` (more muted).
- **Populated state:** Inner shadow `Opacity="0"`, `BlurRadius="0"`. Border color reverts. Transition: 500ms `CubicEase` EaseInOut.
- **Braille decoration:** A dim braille pattern `⣿⣶⣤⣀⠀⣀⣤⣶⣿` centered in the empty space at 15% opacity, grayscale.

### States

| State | Inner Shadow | Border | Transition |
|---|---|---|---|
| Has results | Opacity 0, Blur 0 | `BorderBrush` | 500ms CubicEase |
| Empty | Opacity 0.06, Blur 16, OffsetY 6 | `SurfaceActiveBrush` | 500ms CubicEase |

### Uno implementation

```
Approach: ShadowContainer with IsInner="True" + VisualStateManager
```

- Wrap the watchlist `ListView`/`ItemsRepeater` in a `utu:ShadowContainer`.
- Define two `VisualState`s in the parent control — `HasResults` and `Empty`.
- `Empty` state `Setter`s:
  - `ShadowContainer.Shadow.BlurRadius = 16`
  - `ShadowContainer.Shadow.Opacity = 0.06`
  - `ShadowContainer.Shadow.OffsetY = 6`
  - `ShadowContainer.Shadow.IsInner = True`
  - `ShadowContainer.BorderBrush = {StaticResource SurfaceActiveBrush}`
- Transition between states uses `VisualTransition` with `GeneratedDuration="0:0:0.5"` — the VSM auto-generates `DoubleAnimation`s for numeric properties.
- The braille empty-state `TextBlock` fades in with a separate Opacity storyboard (GPU-bound).

**Trigger logic:** Bind to the `ICollectionView.Count` or `ItemsSource.Count` — use a `StateTrigger` or drive state from code-behind/ViewModel.

---

## 07 · Data-Driven Braille

### Design intent

Braille activity indicators are tethered to actual percentage change, not decorative sine waves. TSLA at -1.84% is agitated. JPM at +0.78% is calm. Decoration becomes information.

### Visual spec

- **Intensity mapping:** `intensity = min(1.0, |pctChange| / 2.0 + 0.15)`
- **Oscillation speed:** `DispatcherTimer` interval scales with intensity: `180ms / intensity` (clamped 100ms–300ms).
- **Braille density:** Each character maps to `B_LEVELS[round(v × 6)]` where `v = (sin(frame × 0.2 × intensity + i × 0.9 + tickerSeed) × 0.5 + 0.5) × intensity`.
- **Color:** `--gain` for positive, `--loss` for negative.
- **Character opacity:** `0.25 + v × 0.65` — calm stocks are faint, volatile stocks are dense.

### Uno implementation

```
Approach: SKXamlCanvas + DispatcherTimer per row (or one shared timer)
```

- Each watchlist row includes a small `SKXamlCanvas` (e.g., 60×14 px) next to the ticker symbol.
- A shared `DispatcherTimer` at 60fps (16ms) increments a global `_frame` counter.
- In each canvas's `OnPaintSurface`, read the stock's `pctChange` from the bound data, compute intensity, and draw 6–12 braille characters using `SKPaint` with `SKTypeface` set to a monospace font supporting braille Unicode block (IBM Plex Mono or Segoe UI Symbol).
- Each character's alpha is computed from the intensity formula.
- `InvalidateVisual()` is called on each timer tick only for visible rows (use `ListView` virtualization — only live rows have active canvases).

**Performance:** SkiaSharp text draw is very fast. 8 visible rows × 12 characters = 96 glyphs per frame at 60fps is trivial for the Skia backend.

**Alternative for fewer custom draws:** Use a `TextBlock` per braille indicator, update `Text` and per-character `Foreground` opacity via code-behind on each tick. Less smooth but zero SkiaSharp dependency.

---

## 08 · Tooltip Magnetism

### Design intent

Within snap radius, the chart crosshair accelerates to the nearest data point. Between points it eases with a slight lag — like a ratcheted dial. Your hand knows where the notches are.

### Visual spec

- **Snap radius:** 18 logical pixels from any data point's X coordinate.
- **Free tracking:** Crosshair follows mouse with 1-frame ease (`lerp(current, target, 0.4)` per frame).
- **Snapped:** Crosshair jumps to data point X with `lerp(current, target, 0.7)` — faster, more decisive.
- **Tooltip pill:** Dark rounded rect, travels with the crosshair, displays `$value`. Springs vertically to sit above the data point.
- **Data point highlight:** Active point grows from r=1.5 to r=4 with a concentric glow ring at r=10, opacity 8%.

### Uno implementation

```
Approach: PointerMoved on SKXamlCanvas + manual interpolation in paint loop
```

- The chart is drawn in `SKXamlCanvas`. `PointerMoved` feeds raw mouse X into a `_targetX` field.
- A render loop (tied to `DispatcherTimer` at 16ms or `CompositionTarget.Rendering`) runs `_currentX = lerp(_currentX, snapTarget, factor)` where:
  - `snapTarget` = nearest data point X if within 18px, else `_targetX`
  - `factor` = 0.7 if snapped, 0.4 if free
- On each tick, `InvalidateVisual()` → `OnPaintSurface` draws:
  1. The vertical crosshair line at `_currentX`
  2. The nearest data point highlight (circle + glow)
  3. The tooltip pill positioned above the point
- `PointerExited` fades the crosshair opacity to 0 over 150ms (track a `_crosshairAlpha` that lerps to 0).

**Why not XAML overlay?** The chart is already SkiaSharp-drawn. Keeping the tooltip in the same paint call avoids z-index coordination between XAML and Skia layers, and gives sub-frame interpolation control that XAML Storyboards can't express (the lerp factor changes based on snap state).

---

## 09 · Scroll Anticipation

### Design intent

The watchlist has edges you can feel. Accent-colored bars thicken at boundaries. Fade masks hint at hidden content. The list has physical weight.

### Visual spec

- **Top edge (at scroll top):** 3px bar, `AccentBrush` at 20% opacity, with gradient fade downward.
- **Bottom edge (at scroll bottom):** Same, gradient upward.
- **Content masks:** When NOT at an edge, a 32px gradient mask (card background → transparent) overlays the content, signaling more content exists.
- **Transitions:** Edge bars animate height 0→3 and opacity 0→0.2 over 400ms ease.

### Uno implementation

```
Approach: ScrollViewer.ViewChanged + Opacity animations on overlay Borders
```

- The watchlist `ScrollViewer` fires `ViewChanged`. In the handler:
  ```csharp
  var sv = sender as ScrollViewer;
  bool atTop = sv.VerticalOffset < 4;
  bool atBottom = sv.VerticalOffset + sv.ViewportHeight >= sv.ScrollableHeight - 4;
  ```
- Four `Border` elements sit in a `Grid` overlaying the `ScrollViewer`:
  1. `TopEdgeBar` — Height 3, HorizontalAlignment Stretch, VerticalAlignment Top. `Background` = `LinearGradientBrush` from `AccentBrush` (opacity 0.2) to Transparent.
  2. `BottomEdgeBar` — Same, VerticalAlignment Bottom, gradient reversed.
  3. `TopFadeMask` — Height 32, `Background` = gradient from card color to Transparent. `IsHitTestVisible="False"`.
  4. `BottomFadeMask` — Same, bottom-aligned.
- Use `VisualStateManager` with states `AtTop`, `AtBottom`, `ScrollingMiddle`, `AtBothEdges`.
- Each state sets `Opacity` on the relevant overlays. `VisualTransition` with `GeneratedDuration="0:0:0.4"` auto-generates smooth opacity transitions.
- All transitions are GPU-bound (Opacity only).

---

## 10 · Weighted Paper

### Design intent

Every hover lift has mass. The card presses down slightly before rising — like a thick sheet on a soft surface. Unifies motion language under one material metaphor: heavy financial paper.

### Visual spec

- **Phase 1 — Press:** TranslateY moves from 0 to +3px over 80ms (linear — instant weight).
- **Phase 2 — Lift:** TranslateY springs from +3 to -6px using spring physics (overshoot, settle).
- **Phase 3 — Return:** On pointer exit, TranslateY springs back to 0.
- **Shadow:** Simultaneously, the outer shadow's `BlurRadius` goes 4→16 (lift) and `Opacity` goes 0.04→0.12.

### States

| Phase | TranslateY | Duration | Easing |
|---|---|---|---|
| Idle | 0 | — | — |
| Press | +3 | 80ms | Linear |
| Lift | -6 | 500ms | `ElasticEase` EaseOut, Oscillations 1, Springiness 4 |
| Return | 0 | 400ms | `CubicEase` EaseOut |

### Uno implementation

```
Approach: PointerEntered/Exited + CompositeTransform.TranslateY Storyboards
```

- Each holding card `Border` has `RenderTransform` set to a `CompositeTransform`.
- `PointerEntered` fires a `Storyboard`:
  - `DoubleAnimationUsingKeyFrames` on `CompositeTransform.TranslateY`:
    - 0ms → 0 (current)
    - 80ms → 3 (Linear — the press-down)
    - 580ms → -6 (`ElasticEase` EaseOut, Oscillations 1, Springiness 4 — overshoot lift)
- `PointerExited` fires a separate `Storyboard`:
  - `DoubleAnimation` on `CompositeTransform.TranslateY` to 0, Duration 400ms, `CubicEase` EaseOut.
- Shadow changes can either animate `ShadowContainer` properties (UI-thread, fine for 500ms transitions) or use `ThemeShadow` with `Translation` Z-axis changes (GPU-bound): `Translation="0,0,8"` → `Translation="0,0,32"` on hover.

**GPU note:** `TranslateY` on `CompositeTransform` is GPU-bound. This is the smoothest possible path in Uno.

---

## 11 · Session Warmth

### Design intent

The longer someone sits on the dashboard, the ambient orbs drift warmer in hue. Not enough to consciously perceive moment-to-moment, but a screenshot at minute 1 vs minute 20 shows the greens shifting toward teal. A patina of use.

### Visual spec

- **Hue shift:** 0° at session start, caps at 25° after ~20 minutes.
- **Rate:** `hueShift = min(elapsedSeconds × 0.02, 25.0)` — roughly 1.2° per minute.
- **Target:** Only the ambient orb layer (3 radial gradient ellipses), not the entire UI.
- **Reset:** On page navigation or app resume from background.

### Uno implementation

```
Approach: Composition HueRotationEffect on orb container Visual (Skia backends)
            OR DispatcherTimer + manual HSL shift on ellipse fills
```

**Option A — Composition API (preferred on Skia backends):**

- The orb container is a `Canvas` or `Grid`. Get its `Visual` via `ElementCompositionPreview.GetElementVisual(orbContainer)`.
- Create a `HueRotationEffect` via the Composition API (implemented on Skia backends per Uno docs):
  ```csharp
  var compositor = Window.Current.Compositor;
  var hueEffect = new HueRotationEffect { Source = new CompositionEffectSourceParameter("source") };
  var factory = compositor.CreateEffectFactory(hueEffect, new[] { "HueRotation.Angle" });
  var brush = factory.CreateBrush();
  // Apply to a SpriteVisual layered over the orbs
  ```
- A `DispatcherTimer` at 10-second intervals updates the `Angle` property: `effectBrush.Properties.InsertScalar("HueRotation.Angle", hueRadians)`.
- This runs on the compositor thread — zero UI thread cost.

**Option B — Manual HSL (universal fallback):**

- Each orb `Ellipse` has a `RadialGradientBrush` fill. Store the base HSL values.
- A `DispatcherTimer` (60-second interval — this changes glacially) recalculates the HSL with the hue offset, converts to `Color`, and sets `GradientStop.Color`.
- `ColorAnimation` in a Storyboard can also animate `GradientStop.Color` directly for smooth transitions between recalculated values.

**Option A is strongly preferred** because it operates below the XAML layer and has zero layout/render cost. Option B is the fallback for non-Skia backends (native Android/iOS).

---

## Integration Priority

Ranked by Fukasawa alignment (maximum subtlety, maximum purpose):

| Priority | Interaction | Effort | Impact |
|---|---|---|---|
| 1 | 07 · Data-Driven Braille | Medium | Transforms decoration into information — highest philosophical alignment |
| 2 | 01 · Market Breathing | Low | Makes the dashboard time-aware with zero UI additions |
| 3 | 05 · Silence on Leave | Low | One Storyboard, one event — maximum subtlety for minimum code |
| 4 | 10 · Weighted Paper | Medium | Unifies the motion vocabulary under one material metaphor |
| 5 | 02 · Chart Data Settle | Low | Single keyframe sequence, satisfying gauge-needle feel |
| 6 | 09 · Scroll Anticipation | Low | Physical affordance with no visual footprint when not needed |
| 7 | 04 · Weight Whisper | Medium | Subliminal data connection — invisible but felt |
| 8 | 06 · Empty Gravity | Low | Emotional weight to absence |
| 9 | 03 · Ink Spread | Medium | Tactile feedback — slightly more visible than others |
| 10 | 08 · Tooltip Magnetism | High | Requires custom SkiaSharp interpolation loop |
| 11 | 11 · Session Warmth | Medium | Beautiful but least functional — ship last |

---

## Shared Helper: Spring Physics Utility

Several interactions (04, 08, 10) benefit from a shared spring physics calculator. WinUI/Uno doesn't expose `SpringVector3NaturalMotionAnimation` universally, so implement a lightweight version:

```csharp
public class SpringAnimator
{
    private double _value, _velocity;
    private readonly double _stiffness, _damping;

    public SpringAnimator(double stiffness = 0.08, double damping = 0.82)
    {
        _stiffness = stiffness;
        _damping = damping;
    }

    public double Value => _value;
    public bool IsAtRest => Math.Abs(_velocity) < 0.01
                         && Math.Abs(_target - _value) < 0.1;

    private double _target;
    public double Target
    {
        get => _target;
        set => _target = value;
    }

    /// <summary>Call per frame (16ms tick). Returns current value.</summary>
    public double Step()
    {
        var force = (_target - _value) * _stiffness;
        _velocity = (_velocity + force) * _damping;
        _value += _velocity;
        if (IsAtRest) _value = _target;
        return _value;
    }
}
```

Wire to a `DispatcherTimer` at 16ms or `CompositionTarget.Rendering` for frame-synced updates. Use for gradient opacity whisper, tooltip magnetism lerp, and weighted paper lift if `ElasticEase` doesn't feel right.

---

## File Structure Suggestion

```
Controls/
  MarketBreathingShadow.cs         // 01 - Attached behavior for ShadowContainer pulse
  ChartSettleAnimator.cs           // 02 - Settle storyboard factory
  InkSpreadBehavior.cs             // 03 - Attached behavior for any Button
  BrailleActivityCanvas.cs         // 07 - SKXamlCanvas subclass
  MagneticTooltipCanvas.cs         // 08 - Chart overlay with snap logic
  ScrollEdgeIndicator.cs           // 09 - Attached behavior for ScrollViewer
Helpers/
  SpringAnimator.cs                // Shared spring physics
  MarketHoursService.cs            // Is market open? Weekday + time check
  SessionTimer.cs                  // Elapsed session time for warmth
Themes/
  Animations.xaml                  // Shared Storyboard resources
  Shadows.xaml                     // ShadowCollection resources for breathing/gravity
```
