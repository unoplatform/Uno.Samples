# 02 — Design Brief

**Project:** EnterpriseDashboard (codename: Observatory)
**Aesthetic:** Editorial data visualization — monochrome, film-grain texture, mathematical illustration precision
**Influences:** Algebrica.org illustration library (annotation-first, pedagogical diagrams), Dieter Rams (reduce without distorting), Edward Tufte (data-ink ratio)

---

## 1. Color System

Every color is achromatic. Zero hue, zero saturation. The palette is ordered by luminance and every token has a defined role.

### 1.1 Token Table

| Token | Hex | Luminance | Role | XAML Key |
|---|---|---|---|---|
| `Black` | `#0A0A0A` | 4% | Page background | `ObsBlackBrush` |
| `Dark` | `#111111` | 7% | Elevated surface (button bg) | `ObsDarkBrush` |
| `Card` | `#131313` | 8% | Chart card fill | `ObsCardBrush` |
| `Border` | `#1C1C1C` | 11% | Card border, dividers | `ObsBorderBrush` |
| `Grid` | `#1F1F1F` | 12% | Chart gridlines, unfilled shapes | `ObsGridBrush` |
| `Faint` | `#2A2A2A` | 16% | Ultra-subtle axis annotations | `ObsFaintBrush` |
| `Grey` | `#555555` | 33% | Secondary data, axis labels | `ObsGreyBrush` |
| `Mid` | `#888888` | 53% | Tertiary data, descriptions | `ObsMidBrush` |
| `Light` | `#BBBBBB` | 73% | Primary data lines | `ObsLightBrush` |
| `OffWhite` | `#DDDDDD` | 87% | Emphasized text | `ObsOffWhiteBrush` |
| `White` | `#EEEEEE` | 93% | Titles, peak-value encoding | `ObsWhiteBrush` |

### 1.2 Data Encoding Rule

Brightness is the sole encoding channel for magnitude. Higher value = brighter fill. This mapping is universal across all 12 charts:

```
value > 80%  →  White (#EEEEEE)
value > 60%  →  Light (#BBBBBB)
value > 40%  →  Mid   (#888888)
value ≤ 40%  →  Grey  (#555555)
```

Implement as `BrightnessMapper.cs`:
```csharp
public static SolidColorBrush FromValue(double value)
{
    if (value > 80) return (SolidColorBrush)App.Current.Resources["ObsWhiteBrush"];
    if (value > 60) return (SolidColorBrush)App.Current.Resources["ObsLightBrush"];
    if (value > 40) return (SolidColorBrush)App.Current.Resources["ObsMidBrush"];
    return (SolidColorBrush)App.Current.Resources["ObsGreyBrush"];
}
```

### 1.3 Colors.xaml

```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <Color x:Key="ObsBlack">#0A0A0A</Color>
    <Color x:Key="ObsDark">#111111</Color>
    <Color x:Key="ObsCard">#131313</Color>
    <Color x:Key="ObsBorder">#1C1C1C</Color>
    <Color x:Key="ObsGrid">#1F1F1F</Color>
    <Color x:Key="ObsFaint">#2A2A2A</Color>
    <Color x:Key="ObsGrey">#555555</Color>
    <Color x:Key="ObsMid">#888888</Color>
    <Color x:Key="ObsLight">#BBBBBB</Color>
    <Color x:Key="ObsOffWhite">#DDDDDD</Color>
    <Color x:Key="ObsWhite">#EEEEEE</Color>

    <SolidColorBrush x:Key="ObsBlackBrush" Color="{StaticResource ObsBlack}" />
    <SolidColorBrush x:Key="ObsDarkBrush" Color="{StaticResource ObsDark}" />
    <SolidColorBrush x:Key="ObsCardBrush" Color="{StaticResource ObsCard}" />
    <SolidColorBrush x:Key="ObsBorderBrush" Color="{StaticResource ObsBorder}" />
    <SolidColorBrush x:Key="ObsGridBrush" Color="{StaticResource ObsGrid}" />
    <SolidColorBrush x:Key="ObsFaintBrush" Color="{StaticResource ObsFaint}" />
    <SolidColorBrush x:Key="ObsGreyBrush" Color="{StaticResource ObsGrey}" />
    <SolidColorBrush x:Key="ObsMidBrush" Color="{StaticResource ObsMid}" />
    <SolidColorBrush x:Key="ObsLightBrush" Color="{StaticResource ObsLight}" />
    <SolidColorBrush x:Key="ObsOffWhiteBrush" Color="{StaticResource ObsOffWhite}" />
    <SolidColorBrush x:Key="ObsWhiteBrush" Color="{StaticResource ObsWhite}" />

</ResourceDictionary>
```

---

## 2. Typography System

Two type families provide editorial contrast: a serif display face for titles and stat values, and a monospace face for data labels and body text.

### 2.1 Type Scale

| Style Key | Family | Weight | Size (px) | Line Height | Letter Spacing | Color | Usage |
|---|---|---|---|---|---|---|---|
| `ObsDisplay` | Playfair Display | 400 | 42 | 48 | -1.0 | White | Page title h1 |
| `ObsSectionTitle` | Playfair Display | 400 | 21 | 28 | -0.3 | White | Section headers |
| `ObsCardTitle` | Playfair Display | 400 | 19 | 24 | -0.3 | White | Chart card titles |
| `ObsStatValue` | Playfair Display | 400 | 30 | 36 | 0 | White | Large stat numbers |
| `ObsHeaderStat` | Playfair Display | 400 | 22 | 28 | 0 | White | Header counter values |
| `ObsMonoBody` | Courier New | 400 | 12 | 20 | 0 | Mid | Intro paragraph |
| `ObsMonoDesc` | Courier New | 400 | 11 | 16 | 0 | Grey | Section descriptions |
| `ObsMonoSmall` | Courier New | 400 | 10 | 16 | 1.0 | Mid | Stat units, legend text |
| `ObsMonoTag` | Courier New | 400 | 9 | 12 | 2.5 | Grey | Uppercase card subtitles |
| `ObsAxisLabel` | Courier New | 400 | 8 | 12 | 0 | Grey | Chart axis values |
| `ObsAxisTick` | Courier New | 400 | 7 | 12 | 1.0 | Grey | Heatmap axis, small ticks |
| `ObsAnnotation` | Courier New | 400 | 8.5 | 12 | 0 | Mid | Italic chart annotations |

### 2.2 Font Registration

Playfair Display is bundled as a TTF asset. Courier New is system-available on all targets.

```xml
<!-- App.xaml or font registration -->
<FontFamily x:Key="PlayfairDisplay">ms-appx:///Assets/Fonts/PlayfairDisplay-Regular.ttf#Playfair Display</FontFamily>
<FontFamily x:Key="CourierNew">Courier New</FontFamily>
```

### 2.3 Typography.xaml (Key Styles)

```xml
<Style x:Key="ObsDisplay" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{StaticResource PlayfairDisplay}" />
    <Setter Property="FontSize" Value="42" />
    <Setter Property="FontWeight" Value="Normal" />
    <Setter Property="Foreground" Value="{StaticResource ObsWhiteBrush}" />
    <Setter Property="CharacterSpacing" Value="-40" /> <!-- -1px at 42px ≈ -40 1/1000em -->
    <Setter Property="LineHeight" Value="48" />
</Style>

<Style x:Key="ObsCardTitle" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{StaticResource PlayfairDisplay}" />
    <Setter Property="FontSize" Value="19" />
    <Setter Property="FontWeight" Value="Normal" />
    <Setter Property="Foreground" Value="{StaticResource ObsWhiteBrush}" />
    <Setter Property="CharacterSpacing" Value="-16" />
</Style>

<Style x:Key="ObsMonoTag" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{StaticResource CourierNew}" />
    <Setter Property="FontSize" Value="9" />
    <Setter Property="Foreground" Value="{StaticResource ObsGreyBrush}" />
    <Setter Property="CharacterSpacing" Value="278" /> <!-- 2.5px at 9px -->
    <Setter Property="TextTransform" Value="Uppercase" />
</Style>

<Style x:Key="ObsAnnotation" TargetType="TextBlock">
    <Setter Property="FontFamily" Value="{StaticResource CourierNew}" />
    <Setter Property="FontSize" Value="8.5" />
    <Setter Property="FontStyle" Value="Italic" />
    <Setter Property="Foreground" Value="{StaticResource ObsMidBrush}" />
    <Setter Property="Opacity" Value="0.85" />
    <Setter Property="TextWrapping" Value="Wrap" />
    <Setter Property="MaxWidth" Value="140" />
    <Setter Property="LineHeight" Value="12" />
</Style>
```

---

## 3. Layout System

### 3.1 Page Structure

```
┌─────────────────────────────────────┐
│         Fixed noise overlay          │  IsHitTestVisible="False", Opacity=0.02
│  ┌─────────────────────────────────┐ │
│  │          ScrollViewer           │ │
│  │  ┌───────────────────────────┐  │ │
│  │  │     Header (max 820px)    │  │ │  Padding: 56 top, 28 horizontal
│  │  │     ─ Title               │  │ │
│  │  │     ─ Subtitle            │  │ │
│  │  │     ─ Stats row           │  │ │
│  │  │     ─ Rule line           │  │ │
│  │  ├───────────────────────────┤  │ │
│  │  │     Chart Grid (820px)    │  │ │  2 columns, 18px gap
│  │  │  ┌─────────┬─────────────┐│  │ │
│  │  │  │ Section Divider (span) ││  │ │
│  │  │  ├────┬────┤             ││  │ │
│  │  │  │ C1 │ C2 │             ││  │ │  Individual chart cards
│  │  │  ├────┼────┤             ││  │ │
│  │  │  │ C3 │ C4 │             ││  │ │
│  │  │  ├────┴────┤             ││  │ │
│  │  │  │ Full-width chart      ││  │ │  Histogram, Slope, DotStrip span both
│  │  │  └─────────┴─────────────┘│  │ │
│  │  ├───────────────────────────┤  │ │
│  │  │     Footer                │  │ │
│  │  └───────────────────────────┘  │ │
│  └─────────────────────────────────┘ │
│                                       │
│  ┌──────────┐                         │  Fixed position: bottom-right
│  │ [↺ REPLAY]│                         │  z-index above scroll content
│  └──────────┘                         │
└─────────────────────────────────────┘
```

### 3.2 Spacing Constants

| Element | Value | XAML |
|---|---|---|
| Page max width | 820px | `MaxWidth="820"` on inner `StackPanel` |
| Page top padding | 56px | `Padding="28,56,28,0"` |
| Page horizontal padding | 28px | (see above) |
| Page bottom padding | 80px | `Padding="0,0,0,80"` on grid |
| Chart grid columns | 2 | `ColumnDefinitions="*,*"` |
| Chart grid gap | 18px | `ColumnSpacing="18" RowSpacing="18"` |
| Card padding | 24px horizontal, 24px top, 18px bottom | `Padding="24,24,24,18"` |
| Card corner radius | 5px | `CornerRadius="5"` |
| Card border | 1px `Border` color | `BorderThickness="1"` |
| Section top padding | 36px | `Margin="0,36,0,10"` |
| Header stat gap | 28px between stat items | `Spacing="28"` |
| Legend gap | 16px between legend items | `Spacing="16"` |
| Chart header → content | 18px | `Margin="0,0,0,18"` on header |
| Annotation max-width | 110–160px depending on chart | Per-chart constant |

### 3.3 Responsive Breakpoints

Using Uno Toolkit `Responsive` markup extension:

| Breakpoint | Layout | Card Padding | Grid Columns |
|---|---|---|---|
| ≥ 768px (Normal+) | 2 columns | 24px | `*,*` |
| 480–767px (Narrow) | 1 column | 20px | `*` |
| < 480px (Narrowest) | 1 column | 16px | `*` |

Full-width charts (Histogram, Slope, DotStrip) always span the full width regardless of breakpoint.

Side-by-side charts (Radar-style layouts with text + visualization) stack vertically below 600px.

```xml
<Grid ColumnDefinitions="{Responsive Normal='*,*', Narrow='*'}"
      ColumnSpacing="{Responsive Normal=18, Narrow=0}"
      RowSpacing="18" />
```

---

## 4. Component Specifications

### 4.1 ChartCard

The shared container wrapping every chart.

**Visual Anatomy:**
```
┌─────────────────────────────────────┐
│ [Title]                    [TAG]    │  Title: ObsCardTitle, Tag: ObsMonoTag
│ [30]  peak value                    │  Stat: ObsStatValue, Unit: ObsMonoSmall
│                                     │
│  ┌─────────────────────────────┐    │
│  │                             │    │  Chart content area (ContentPresenter)
│  │     [Chart Visualization]   │    │
│  │                             │    │
│  │  ╭ Annotation text here ╮   │    │  ChartAnnotation: positioned absolutely
│  │  ╰──────────────────────╯   │    │
│  └─────────────────────────────┘    │
│                                     │
│  ● Primary  ○ Reference            │  Legend row (optional)
└─────────────────────────────────────┘
```

**States:**
- **Default:** Card background `Card`, border `Border`, content area holds chart
- **Loading (unused):** Data is synchronous so no loading state needed
- **Animated:** Chart content plays its animation; annotation fades in late

### 4.2 ChartAnnotation

Algebrica-style pedagogical note embedded in the chart area.

| Property | Value |
|---|---|
| Position | Absolute within the chart Canvas, coordinates per-chart |
| Font | ObsAnnotation style (Courier New, 8.5px, italic) |
| Color | `Mid` at 85% opacity |
| Max width | 110–160px (set per chart) |
| Line height | 12px |
| Animation | Fades in near the end of the chart's animation sequence |

### 4.3 SectionDivider

Numbered section header separating chart groups.

```
01   Cartesian Foundations
Six charts on orthogonal axes...
──────────────────────────────────────
```

| Element | Style |
|---|---|
| Number | ObsMonoTag, letter-spacing 3px |
| Title | ObsSectionTitle (Playfair 21px) |
| Description | ObsMonoDesc (Courier 11px, Grey) |
| Rule | 1px `Border` color, margin-top 14px |
| Full span | `Grid.ColumnSpan="2"` |

### 4.4 Replay Button

Fixed-position floating button, bottom-right corner.

**Default state:**
- Background: `Dark` (#111)
- Border: 1px `Border` (#1C1C1C)
- Border radius: 40px (pill shape)
- Padding: 9px 18px
- Icon: Refresh arrow, 13×13, stroke `Mid`
- Label: "REPLAY", ObsMonoTag style at `Mid` color

**Hover state:**
- Background: `Border` (#1C1C1C)
- Border color: `Grey` (#555)
- Transition: 200ms ease-out

**Pressed state:**
- Background: `Grid` (#1F1F1F)
- Scale: 0.97

---

## 5. Chart Viewport (SVG → XAML Mapping)

Each chart has a defined coordinate space. The React prototype uses SVG `viewBox`. In XAML, the equivalent is a `Canvas` with explicit `Width`/`Height` inside a `Viewbox` for responsive scaling.

### 5.1 Standard Chart Viewport

| Property | Single-column chart | Full-width chart |
|---|---|---|
| Canvas width | 360 | 740 (spans both columns) |
| Canvas height | 200 | 220 |
| Plot area insets | Top: 12, Right: 12, Bottom: 28, Left: 34 | Varies per chart |
| Axis label font | ObsAxisLabel (8px) | ObsAxisLabel (8px) |
| Grid line stroke | `Grid`, 0.5px | `Grid`, 0.5px |

### 5.2 Viewbox Scaling

```xml
<Viewbox Stretch="Uniform" HorizontalAlignment="Stretch">
    <Canvas Width="360" Height="200" x:Name="ChartCanvas" />
</Viewbox>
```

The `Viewbox` ensures the chart scales proportionally within its card regardless of actual pixel dimensions.

---

## 6. Glow Ring Specification

Several charts (Line, Scatter, Radar) use a "glow ring" behind data dots — a larger, semi-transparent circle that provides visual weight.

| Property | Value |
|---|---|
| Glow radius | 2.5× – 3× the dot radius |
| Fill | Same color as the dot |
| Opacity | 4–6% |
| Animation | Scales from 0 to 1 simultaneously with the dot (Spring easing) |
| Exception (Scatter) | Glow starts at 2.5× scale and shrinks TO 1× (inverse) |

---

## 7. Film Grain Overlay

A fixed full-viewport texture layer providing analog film atmosphere.

| Property | Value |
|---|---|
| Position | Fixed, covering entire viewport |
| Content | SVG `feTurbulence` fractal noise, base frequency 0.85, 4 octaves |
| Tiling | 256×256px tile repeated |
| Opacity | 2% (0.02) |
| Hit testing | Disabled (`IsHitTestVisible="False"`) |
| Z-index | Above all chart content, below the replay button |

**XAML implementation:** Create the noise as a PNG asset (256×256 grayscale noise) and tile it via `ImageBrush` with `TileMode="Tile"` on a full-screen `Rectangle`.

```xml
<Rectangle IsHitTestVisible="False" Opacity="0.02"
           HorizontalAlignment="Stretch" VerticalAlignment="Stretch">
    <Rectangle.Fill>
        <ImageBrush ImageSource="ms-appx:///Assets/noise-256.png"
                    Stretch="None" AlignmentX="Left" AlignmentY="Top" />
    </Rectangle.Fill>
</Rectangle>
```

---

## 8. Data-Ink Principles

Every visual element must earn its place. Apply Tufte's data-ink ratio across all charts:

**Include:**
- Data lines, bars, dots, areas (primary ink)
- Grid lines at meaningful intervals only (never more than 4–5 horizontal)
- Axis labels at readable intervals (skip crowded ticks)
- One annotation per chart explaining the most important insight

**Exclude:**
- Decorative borders or drop shadows on cards (border is structural, not decorative)
- Chart backgrounds (transparent — card background suffices)
- Redundant legends (if the chart only has one series, no legend)
- Y-axis titles (the card title and stat value already establish context)
- Tick marks on axes (the grid lines serve this purpose)

---

## 9. Accessibility

| Requirement | Implementation |
|---|---|
| Contrast ratio | All data elements ≥ 3:1 against `Card` (#131313). `Grey` (#555) = 3.5:1. `Mid` (#888) = 5.2:1. `Light` (#BBB) = 8.4:1. |
| Touch targets | Replay button: 44×44px minimum (pill shape exceeds this) |
| Screen reader | Each `ChartCard` has `AutomationProperties.Name` set to "{Title}: {Stat} {Unit}" |
| Reduced motion | Check `UISettings.AnimationsEnabled`. If false, skip all animations — render charts in their final state immediately |
| Color independence | All encoding uses brightness (luminance), not hue — accessible to all forms of color vision deficiency |
