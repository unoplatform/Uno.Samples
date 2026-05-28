# 09 — Implementation Notes

This file translates the visual and behavioral spec into implementation guidance. The target platform is **Uno Platform** (C# / XAML / WinUI), Matt's primary stack. Where guidance is framework-agnostic it is marked. Where Uno-specific, it leans on current Uno conventions and patterns.

If implementing in another framework (React, Vue, Flutter), the framework-agnostic guidance still applies; map the Uno specifics to equivalents.

## 9.1 Decision overview

Three foundational decisions shape everything downstream.

### Decision 1: state management — MVUX

**Decision**: Use MVUX (Model-View-Update-Extensions) as the state pattern.
**Reason**: The dashboard has durable async state (data from APIs / sources, refresh, sync), few user-driven mutations, and clear unit-test boundaries per section. MVUX fits this profile better than MVVM.
**Tradeoff**: MVUX is newer than MVVM in Uno's lineage; less reference material, but more aligned with reactive data flow.

### Decision 2: theming — ThemeResource dictionaries

**Decision**: Express semantic tokens as `ThemeResource` entries in two `ResourceDictionary` files (dark, light) merged at root.
**Reason**: Native Uno theming primitive, switches via `Application.RequestedTheme` or a per-element `RequestedTheme` property.
**Tradeoff**: Forces re-evaluation of dynamically-bound visual brushes when theme changes; minimal cost on this dashboard size.

### Decision 3: charts — composition over library

**Decision**: Render each chart as composed XAML primitives (Lines, Polylines, Rectangles, Ellipses, custom Paths) inside a Canvas or Grid container. No third-party chart library.
**Reason**: Chart styling discipline matters more than chart breadth. A library would default to conventional chart aesthetics; composed primitives keep the calibrated-instrument look. Each chart is small enough that bespoke composition is tractable.
**Tradeoff**: Each chart variant requires its own implementation. Estimated ~80–150 lines of XAML + supporting code per chart variant.

## 9.2 Project structure

```
EnterpriseDashboard/
├── EnterpriseDashboard.csproj
├── App.xaml                       # ResourceDictionary merge for both themes
├── App.xaml.cs
├── Themes/
│   ├── Dark.xaml                  # Dark semantic tokens
│   ├── Light.xaml                 # Light semantic tokens
│   └── Generic.xaml               # Component styles, theme-independent
├── Views/
│   ├── ShellPage.xaml             # Three-column layout
│   ├── AcquisitionView.xaml       # § 01
│   ├── EngagementView.xaml        # § 02
│   ├── CohortsView.xaml           # § 03
│   └── Header.xaml                # Page header
├── Components/
│   ├── Cards/
│   │   ├── Card.xaml              # Container
│   │   ├── CardHead.xaml
│   │   ├── MetricBlock.xaml
│   │   ├── Delta.xaml
│   │   ├── Annotation.xaml
│   │   ├── CardFoot.xaml
│   │   └── ChartFrame.xaml
│   ├── Charts/
│   │   ├── LineChart.xaml
│   │   ├── BarChart.xaml
│   │   ├── AreaChart.xaml
│   │   ├── ScatterPlot.xaml
│   │   ├── Histogram.xaml
│   │   ├── HorizontalBarList.xaml
│   │   ├── Heatmap.xaml
│   │   ├── ConcentricArcs.xaml
│   │   ├── BoxPlot.xaml
│   │   ├── Gauge.xaml
│   │   ├── Slopegraph.xaml
│   │   ├── StripPlot.xaml
│   │   └── RetentionGrid.xaml
│   ├── Navigation/
│   │   ├── NavRail.xaml
│   │   └── RailButton.xaml
│   ├── ContextRail/
│   │   ├── ContextRail.xaml
│   │   ├── RailSection.xaml
│   │   ├── RailKV.xaml
│   │   ├── RailList.xaml
│   │   ├── SignalItem.xaml
│   │   └── RailNote.xaml
│   ├── Controls/
│   │   ├── RangeSelector.xaml
│   │   ├── IconButton.xaml
│   │   ├── ThemeToggle.xaml
│   │   └── Toast.xaml
│   ├── SectionBar.xaml
│   ├── SectionHead.xaml
│   └── Skeleton.xaml
├── Models/                        # MVUX models (one per section)
│   ├── AcquisitionModel.cs
│   ├── EngagementModel.cs
│   └── CohortsModel.cs
├── Services/
│   ├── IDataService.cs
│   ├── DataService.cs             # Production: aggregates Stripe, Mixpanel, etc.
│   └── FixtureDataService.cs      # Development: deterministic fixtures
└── Assets/
    └── Icons/                     # Inline SVG paths as resources
```

## 9.3 Theme implementation

### Resource dictionary structure

```xml
<!-- Themes/Dark.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Dark">

      <!-- Surfaces -->
      <SolidColorBrush x:Key="BgBrush"          Color="#08090C"/>
      <SolidColorBrush x:Key="SurfaceBrush"     Color="#0E1015"/>
      <SolidColorBrush x:Key="Surface2Brush"    Color="#14171F"/>
      <SolidColorBrush x:Key="Surface3Brush"    Color="#1A1E27"/>

      <!-- Borders -->
      <SolidColorBrush x:Key="BorderBrush"      Color="#232831"/>
      <SolidColorBrush x:Key="BorderSoftBrush"  Color="#181C24"/>

      <!-- Ink -->
      <SolidColorBrush x:Key="InkBrush"         Color="#E6E9EE"/>
      <SolidColorBrush x:Key="Ink2Brush"        Color="#A8ADB6"/>
      <SolidColorBrush x:Key="Ink3Brush"        Color="#737880"/>
      <SolidColorBrush x:Key="Ink4Brush"        Color="#525762"/> <!-- a11y-adjusted -->
      <SolidColorBrush x:Key="Ink5Brush"        Color="#2A2E37"/>

      <!-- Semantic -->
      <SolidColorBrush x:Key="AccentBrush"      Color="#7DB8FF"/>
      <SolidColorBrush x:Key="PositiveBrush"    Color="#4FC9A7"/>
      <SolidColorBrush x:Key="NegativeBrush"    Color="#E8806F"/>

      <!-- Charts -->
      <SolidColorBrush x:Key="GridBrush"        Color="#1E222B"/>

    </ResourceDictionary>
  </ResourceDictionary.ThemeDictionaries>
</ResourceDictionary>
```

Light theme has the same keys with light values.

In `App.xaml`:

```xml
<Application.Resources>
  <ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
      <ResourceDictionary Source="Themes/Dark.xaml"/>
      <ResourceDictionary Source="Themes/Light.xaml"/>
      <ResourceDictionary Source="Themes/Generic.xaml"/>
    </ResourceDictionary.MergedDictionaries>
  </ResourceDictionary>
</Application.Resources>
```

### Theme switching

Theme switching is a single property set:

```csharp
public void ToggleTheme()
{
    var current = ((FrameworkElement)Window.Current.Content).RequestedTheme;
    ((FrameworkElement)Window.Current.Content).RequestedTheme =
        current == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
}
```

The 350ms cross-fade specified in `05-interactions-and-motion.md` is implemented via `Storyboard` on the Shell, fading opacity 1 → 0.6 → 1 during the theme swap. Alternatively, accept the instant switch — the visual difference is small at well-balanced themes.

### Typography styles

```xml
<!-- Themes/Generic.xaml -->
<Style x:Key="DisplayXLStyle" TargetType="TextBlock">
  <Setter Property="FontFamily" Value="ms-appx:///Assets/Fonts/BricolageGrotesque.ttf#Bricolage Grotesque"/>
  <Setter Property="FontWeight" Value="Light"/>
  <Setter Property="FontSize" Value="48"/>
  <Setter Property="LineHeight" Value="46"/>
  <Setter Property="CharacterSpacing" Value="-25"/>
  <Setter Property="Foreground" Value="{ThemeResource InkBrush}"/>
</Style>

<Style x:Key="MetricValueStyle" TargetType="TextBlock">
  <Setter Property="FontFamily" Value="ms-appx:///Assets/Fonts/BricolageGrotesque.ttf#Bricolage Grotesque"/>
  <Setter Property="FontWeight" Value="ExtraLight"/>
  <Setter Property="FontSize" Value="56"/>
  <Setter Property="LineHeight" Value="50"/>
  <Setter Property="CharacterSpacing" Value="-35"/>
  <Setter Property="Foreground" Value="{ThemeResource InkBrush}"/>
</Style>

<Style x:Key="MonoSStyle" TargetType="TextBlock">
  <Setter Property="FontFamily" Value="ms-appx:///Assets/Fonts/JetBrainsMono.ttf#JetBrains Mono"/>
  <Setter Property="FontSize" Value="10"/>
  <Setter Property="CharacterSpacing" Value="160"/>
  <Setter Property="Foreground" Value="{ThemeResource Ink3Brush}"/>
  <!-- CharacterSpacing is in 1/1000 em, so 160 ≈ +0.16em tracking -->
</Style>
```

Fonts are bundled with the app, not loaded from Google Fonts at runtime. Download Bricolage Grotesque (variable) and JetBrains Mono (300, 400, 500 weights) and embed under `Assets/Fonts/`.

### Optical sizing in Uno

Variable font axes including `opsz` are supported in WinUI 3 via `FontFamily` with the family name. The `font-variation-settings: "opsz" 96` CSS equivalent is set in XAML via `FontVariations`:

```xml
<TextBlock Style="{StaticResource MetricValueStyle}"
           FontVariations="opsz 96"
           Text="$87.3K"/>
```

If the current Uno version does not support `FontVariations`, fall back to the static "Display" subfamily of Bricolage (`Bricolage Grotesque Display`) and embed that as a separate font file.

## 9.4 MVUX models

Each section has a model. The model exposes async data feeds and a refresh command.

```csharp
public partial record AcquisitionModel
{
    private readonly IDataService _data;

    public AcquisitionModel(IDataService data) => _data = data;

    public IFeed<MoneyValue> RecurringRevenue
        => _data.RecurringRevenue().AsFeed();

    public IFeed<int> NewSignups
        => _data.NewSignups().AsFeed();

    public IFeed<MoneyValue> CumulativeARR
        => _data.CumulativeARR().AsFeed();

    public IFeed<RevenueBreakdown> Breakdown
        => _data.RevenueBreakdown().AsFeed();

    public IFeed<MovementSummary> Movement
        => _data.Movement().AsFeed();

    public ICommand Refresh
        => Command.Create<Unit>(b => b.Given(_data.RefreshAll));
}
```

The XAML view binds to feed projections:

```xml
<TextBlock Text="{Binding RecurringRevenue.Value.Formatted, Mode=OneWay}"
           Style="{StaticResource MetricValueStyle}"/>
```

MVUX feeds handle async loading, error, and refresh states uniformly via `FeedView`:

```xml
<mvux:FeedView Source="{Binding RecurringRevenue}">
  <mvux:FeedView.ValueTemplate>
    <DataTemplate>
      <TextBlock Text="{Binding Formatted}" Style="{StaticResource MetricValueStyle}"/>
    </DataTemplate>
  </mvux:FeedView.ValueTemplate>
  <mvux:FeedView.ProgressTemplate>
    <DataTemplate>
      <local:Skeleton Width="180" Height="56"/>
    </DataTemplate>
  </mvux:FeedView.ProgressTemplate>
  <mvux:FeedView.NoneTemplate>
    <DataTemplate>
      <TextBlock Text="—" Style="{StaticResource MetricValueStyle}"
                 Foreground="{ThemeResource Ink4Brush}"/>
    </DataTemplate>
  </mvux:FeedView.NoneTemplate>
  <mvux:FeedView.ErrorTemplate>
    <DataTemplate>
      <TextBlock Text="—" Style="{StaticResource MetricValueStyle}"
                 Foreground="{ThemeResource Ink4Brush}"/>
    </DataTemplate>
  </mvux:FeedView.ErrorTemplate>
</mvux:FeedView>
```

This wires the four card states (default, loading, empty, error) directly to the feed lifecycle.

## 9.5 Card composition

The Card is a UserControl with named slots:

```xml
<!-- Components/Cards/Card.xaml -->
<UserControl x:Class="EnterpriseDashboard.Observatory.Controls.Card">
  <Grid Padding="28,28,28,24"
        Background="{ThemeResource SurfaceBrush}">
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto"/> <!-- CardHead -->
      <RowDefinition Height="Auto"/> <!-- MetricBlock -->
      <RowDefinition Height="*"/>    <!-- ChartFrame -->
    </Grid.RowDefinitions>

    <ContentPresenter Grid.Row="0" Content="{x:Bind Head}"/>
    <ContentPresenter Grid.Row="1" Content="{x:Bind Metric}"/>
    <ContentPresenter Grid.Row="2" Content="{x:Bind Chart}"/>

    <!-- CardFoot absolutely positioned -->
    <ContentPresenter VerticalAlignment="Bottom" HorizontalAlignment="Left"
                      Margin="0,0,0,-4" Content="{x:Bind Foot}"/>
  </Grid>
</UserControl>
```

Cards consumed in views like:

```xml
<components:Card Variant="Tall">
  <components:Card.Head>
    <components:CardHead Title="Recurring Revenue" Meta="12 MO"/>
  </components:Card.Head>
  <components:Card.Metric>
    <components:MetricBlock Value="$87.3K" Unit="MRR"
                            DeltaMagnitude="+12.4%" DeltaDirection="Up"/>
  </components:Card.Metric>
  <components:Card.Chart>
    <charts:LineChart Series="{Binding RecurringSeries}"
                      HighlightIndex="8"
                      Annotation="[ peak observed sep — primary channel diverges ]"/>
  </components:Card.Chart>
  <components:Card.Foot>
    <components:CardFoot Source="Stripe" Updated="2m ago"/>
  </components:Card.Foot>
</components:Card>
```

## 9.6 Card grid (hairline separation)

The "1px gap filled with border-soft" CSS technique translates to a `Grid` with explicit `BorderSoftBrush` cells, or more cleanly with a `UniformGrid` and a manual 1px `Border` inset:

```xml
<Border Background="{ThemeResource BorderSoftBrush}">
  <Grid ColumnSpacing="1" RowSpacing="1">
    <Grid.ColumnDefinitions>
      <ColumnDefinition Width="*"/>
      <ColumnDefinition Width="*"/>
      <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    <components:Card Grid.Column="0">...</components:Card>
    <components:Card Grid.Column="1">...</components:Card>
    <components:Card Grid.Column="2">...</components:Card>
  </Grid>
</Border>
```

The outer Border fills the gaps. The Cards' `SurfaceBrush` background shows through; the 1px gaps reveal the underlying BorderSoft.

## 9.7 Charts

### General pattern

Each chart is a `UserControl` with a single dependency property for data and a few for variants:

```csharp
public sealed partial class LineChart : UserControl
{
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(nameof(Series), typeof(IReadOnlyList<double>),
            typeof(LineChart), new PropertyMetadata(null, OnSeriesChanged));

    public IReadOnlyList<double> Series
    {
        get => (IReadOnlyList<double>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    public int? HighlightIndex { get; set; }
    public string Annotation { get; set; }

    private static void OnSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((LineChart)d).Render();

    private void Render()
    {
        // Compute scaled point coordinates
        // Create Polyline, Ellipse markers, Polyline baseline
        // Clear and re-add children of internal Canvas
    }
}
```

### Specific chart hints

**Heatmap** (Active Usage): use a `UniformGrid` of 7 rows × 24 columns, each cell a `Rectangle` with computed `Fill`. Update via `OnDataChanged`. Use `color-mix` equivalent via interpolating `Color` values in C#.

**Concentric arcs** (Goal Attainment): each arc is a `Path` with an `ArcSegment` geometry. Stroke + `StrokeDashArray` to control the partial draw. Calculate the dash from the percentage value.

**Slopegraph** (Retention by Segment): two columns of `Ellipse` markers + a list of `Line` connectors. Calculate y-positions from values.

**Strip plot** (Per-Account MRR): a `Canvas` with N `Ellipse` children placed at jittered (x, y) coordinates.

**Retention grid** (Cohort Retention): a `Grid` with `Border` cells. Each cell's `Background` computed from value via the dual-zone ramp specified in `03-pages-and-views.md` § 03.C.

**Box plot** (Session Length): one row per plan; each row is a horizontal `StackPanel` or absolutely-positioned `Canvas` with whisker line, box rectangle, median line, and outlier ellipses.

**Gauge** (Health Score): two `Path` arcs for the background and foreground, a `Line` for the needle, an `Ellipse` for the pivot dot.

**Line chart** (Recurring Revenue): a `Polyline` for the main line, a list of `Ellipse` for markers, a `Line` (dashed) for the baseline, a `TextBlock` for the annotation.

### Chart data contract

Charts accept simple data shapes. Avoid heavyweight ViewModels:

```csharp
public record TimeSeriesPoint(string Label, double Value);
public record ScatterPoint(double X, double Y);
public record CohortRow(string Label, double?[] Values);
public record BoxStats(string Label, double Min, double Q1, double Median, double Q3, double Max, IReadOnlyList<double> Outliers);
```

The model layer aggregates raw API data into these shapes before binding.

## 9.8 Navigation

Section switching is a single property on the Shell ViewModel:

```csharp
public IFeed<string> ActiveSection { get; }  // "acq", "eng", "coh"
```

ShellPage binds the visibility of each view to this feed:

```xml
<Grid>
  <views:AcquisitionView Visibility="{Binding ActiveSection.Value, Converter={StaticResource SectionEquals}, ConverterParameter=acq}"/>
  <views:EngagementView  Visibility="{Binding ActiveSection.Value, Converter={StaticResource SectionEquals}, ConverterParameter=eng}"/>
  <views:CohortsView     Visibility="{Binding ActiveSection.Value, Converter={StaticResource SectionEquals}, ConverterParameter=coh}"/>
</Grid>
```

The transition between views can be implemented with a `Storyboard` keyed to the property change, or accepted as instant for a v1.

## 9.9 Persistence

State that persists (theme, active range, active section):

```csharp
public class SettingsService
{
    private readonly ApplicationDataContainer _settings
        = ApplicationData.Current.LocalSettings;

    public string Theme
    {
        get => (string)(_settings.Values["theme"] ?? "dark");
        set => _settings.Values["theme"] = value;
    }

    public string Range
    {
        get => (string)(_settings.Values["range"] ?? "30D");
        set => _settings.Values["range"] = value;
    }

    public string ActiveSection
    {
        get => (string)(_settings.Values["section"] ?? "acq");
        set => _settings.Values["section"] = value;
    }
}
```

Restored on app startup via the Shell's constructor.

## 9.10 Performance

The dashboard is small enough that performance is not a primary concern. Two guidelines:

1. **Charts re-render only on data change**, not on every parent re-layout. Use `DependencyProperty` `OnChanged` callbacks, not size-changed events for data updates.
2. **The pulse and breath animations use Composition** rather than Storyboard if possible. Uno's `Microsoft.UI.Composition` API supports infinite animations more efficiently than `Storyboard`. If Composition is not yet supported on the target platforms (WebAssembly, GTK, etc.), Storyboard is acceptable.

## 9.11 Fixture data for development

The `FixtureDataService` returns deterministic, hand-crafted data for development. Each card's fixture matches the values documented in `03-pages-and-views.md`.

```csharp
public class FixtureDataService : IDataService
{
    public IObservable<MoneyValue> RecurringRevenue() =>
        Observable.Return(new MoneyValue(87_300, "USD", "K", "+12.4%", DeltaDirection.Up));

    public IObservable<IReadOnlyList<TimeSeriesPoint>> RecurringSeries() =>
        Observable.Return<IReadOnlyList<TimeSeriesPoint>>(new[]
        {
            new TimeSeriesPoint("JAN", 44),
            new TimeSeriesPoint("FEB", 48),
            new TimeSeriesPoint("MAR", 52),
            // ...
            new TimeSeriesPoint("SEP", 90), // peak
            // ...
            new TimeSeriesPoint("DEC", 87.3),
        });

    // ... one method per card series
}
```

The fixture service is registered in DI for `Debug` builds only.

## 9.12 Telemetry (forward planning)

Not in v1. When added:

- Track section switches (which section, how long viewed)
- Track theme switches
- Track refresh frequency
- Do not track hover or scroll (privacy concern, low signal)

Telemetry events follow the existing naming conventions in Matt's Uno Platform workflow.

## 9.13 Localization

Localization is handled via `.resw` resource files. Per locale: `Strings/en/Resources.resw`, `Strings/fr/Resources.resw`.

The dashboard contains few localized strings:
- Section titles (3)
- Section descriptions (3)
- Card titles (~14)
- Right rail headings (~6)
- Right rail notes (3)
- Toast messages (~4)
- Empty/error state messages (~10)

Numeric formatting uses `CultureInfo.CurrentCulture` for thousands separators and decimal points.

## 9.14 Testing

Test surfaces in priority order:

1. **Visual regression** of each section in both themes, all viewports. Tool: any pixel-diff tool that runs against rendered Uno output. Capture baseline screenshots once design is approved.
2. **Unit tests on models**: data shaping, formatting, delta computation.
3. **Component tests** for charts: given known input, the rendered XAML tree contains expected primitives at expected positions (no need to test pixel rendering — test the geometry).
4. **A11y tests** per the checklist in `08-accessibility.md` § 8.10.
5. **End-to-end tests** of theme toggle, section switch, refresh.

Tests are placed under `EnterpriseDashboard.Tests/`.

## 9.15 Build and deploy

The dashboard is delivered as an Uno Platform app targeting WinAppSDK desktop primarily. Secondary targets: WebAssembly (for in-browser embed) and macOS (via Skia).

Build commands:
```
dotnet build -c Release -p:UnoPlatform=Desktop
dotnet build -c Release -p:UnoPlatform=WebAssembly
```

Font assets must be bundled correctly in the WebAssembly build via the `<EmbeddedResource>` item group.

## 9.16 What this brief intentionally does not specify

- Data source SDK choices (Stripe vs Stripe.NET, etc.)
- Authentication flow
- CI/CD pipeline
- Telemetry vendor
- Error reporting service
- Multi-window support (if added later)

These are implementation choices the engineer makes when integrating the dashboard into a larger product.

## Unresolved Questions

- Is MVUX confirmed as the state pattern, or should we use ReactiveUI / MVVM as a fallback?
- Which font hosting approach: bundled with the app (recommended), or fetched at runtime from Google Fonts (smaller binary, network dependency)?
- What's the upstream API contract? The fixture service assumes a small set of methods; the production service shape depends on the data layer.
- Should the dashboard support multi-tenancy (per-account scoping) in v1 or punt to v1.1?
- Is the desktop-first / mobile-secondary commitment durable, or is mobile a near-term roadmap item that should influence v1 architecture?
- Does the production deployment need offline / cached state (read-only on no-connection)?
