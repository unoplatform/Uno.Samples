# Meridian Terminal — Uno Platform XAML Scaffold & Component Hierarchy

> Companion to the Implementation Briefs document.  
> This file provides the concrete XAML structure, control tree, data models, and code scaffolds that a developer uses as a starting skeleton.

---

## 1. Solution & Project Structure

```
Meridian.sln
└── Meridian/
    ├── Meridian.csproj
    ├── App.xaml / App.xaml.cs
    ├── Assets/
    │   └── Fonts/
    │       ├── InstrumentSerif-Regular.ttf
    │       ├── InstrumentSerif-Italic.ttf
    │       ├── IBMPlexMono-Regular.ttf
    │       ├── IBMPlexMono-Medium.ttf
    │       ├── Outfit-Variable.ttf
    │       └── (or individual weights: 300,400,500,600,700)
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
    ├── Services/
    │   ├── IMarketDataService.cs
    │   └── MockMarketDataService.cs
    ├── Presentation/
    │   ├── DashboardModel.cs          (MVUX)
    │   └── TradeDrawerModel.cs        (MVUX)
    ├── Controls/
    │   ├── OdometerControl.xaml/.cs
    │   ├── BrailleTickerControl.xaml/.cs
    │   ├── BrailleSpinnerControl.xaml/.cs
    │   ├── BraillePulseControl.xaml/.cs
    │   ├── BrailleActivityControl.xaml/.cs
    │   ├── SparklineControl.cs        (SKXamlCanvas subclass)
    │   ├── SectorRingControl.cs       (SKXamlCanvas subclass)
    │   ├── VolumeChartControl.cs      (SKXamlCanvas subclass)
    │   └── PerformanceChartControl.cs (SKXamlCanvas or LiveCharts2)
    ├── Views/
    │   ├── DashboardPage.xaml/.cs
    │   └── TradeDrawer.xaml/.cs
    └── Themes/
        ├── ColorPaletteOverride.xaml
        ├── FontResources.xaml
        ├── CardStyles.xaml
        └── TextBlockStyles.xaml
```

---

## 2. Data Models

```csharp
// Models/Stock.cs
public record Stock(
    string Ticker,
    string Name,
    decimal Price,
    decimal Change,
    decimal Pct,
    decimal High,
    decimal Low,
    decimal Open,
    string Volume
);

// Models/Holding.cs
public record Holding(
    string Ticker,
    int Shares,
    decimal AvgCost,
    decimal CurrentPrice
)
{
    public decimal MarketValue => Shares * CurrentPrice;
    public decimal GainLoss => (CurrentPrice - AvgCost) * Shares;
    public decimal GainPct => (CurrentPrice - AvgCost) / AvgCost * 100;
    public bool IsPositive => GainPct >= 0;
}

// Models/Sector.cs
public record Sector(string Name, double Pct, string ColorHex);

// Models/VolumeBar.cs
public record VolumeBar(string Hour, int Volume);

// Models/NewsItem.cs
public record NewsItem(string Time, string Text, string Tag);

// Models/ChartPoint.cs
public record ChartPoint(string Date, decimal Value);

// Models/IndexTicker.cs
public record IndexTicker(string Name, string Value, string Change, bool IsUp);

// Models/StreamTicker.cs
public record StreamTicker(string Ticker, string Price, string Delta, bool IsUp);

// Models/TradeOrder.cs
public record TradeOrder(
    string Ticker,
    string Side,         // "buy" | "sell"
    int Quantity,
    string OrderType,    // "market" | "limit" | "stop"
    decimal? LimitPrice,
    decimal EstimatedTotal
);
```

---

## 3. MVUX Presentation Models

```csharp
// Presentation/DashboardModel.cs
public partial record DashboardModel(IMarketDataService MarketData)
{
    // ── Read-only feeds ──
    public IListFeed<Stock> Watchlist => ListFeed.Async(MarketData.GetWatchlistAsync);
    public IListFeed<Holding> Holdings => ListFeed.Async(MarketData.GetHoldingsAsync);
    public IListFeed<Sector> Sectors => ListFeed.Async(MarketData.GetSectorsAsync);
    public IListFeed<VolumeBar> Volume => ListFeed.Async(MarketData.GetVolumeAsync);
    public IListFeed<NewsItem> News => ListFeed.Async(MarketData.GetNewsAsync);
    public IFeed<IImmutableList<ChartPoint>> PortfolioHistory
        => Feed.Async(MarketData.GetPortfolioHistoryAsync);

    // ── Editable states ──
    public IState<string> SelectedTimeframe => State.Value(this, () => "3M");
    public IState<string?> ChartTicker => State.Value(this, () => (string?)null);
    public IState<string?> ExpandedTicker => State.Value(this, () => (string?)null);
    public IState<string> SearchQuery => State.Value(this, () => "");
    public IState<Stock?> TradeStock => State.Value(this, () => (Stock?)null);
    public IState<string?> HoveredHolding => State.Value(this, () => (string?)null);

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
        await ChartTicker.Set(current == ticker ? null : ticker, CancellationToken.None);
    }

    public async ValueTask ToggleExpanded(string ticker)
    {
        var current = await ExpandedTicker;
        await ExpandedTicker.Set(current == ticker ? null : ticker, CancellationToken.None);
    }

    public async ValueTask OpenTrade(Stock stock)
        => await TradeStock.Set(stock, CancellationToken.None);

    public async ValueTask CloseTrade()
        => await TradeStock.Set(null, CancellationToken.None);

    public async ValueTask BackToPortfolio()
        => await ChartTicker.Set(null, CancellationToken.None);
}
```

```csharp
// Presentation/TradeDrawerModel.cs
public partial record TradeDrawerModel
{
    public IState<string> Side => State.Value(this, () => "buy");
    public IState<int> Quantity => State.Value(this, () => 0);
    public IState<string> OrderType => State.Value(this, () => "market");
    public IState<decimal?> LimitPrice => State.Value(this, () => (decimal?)null);
    public IState<bool> IsConfirmed => State.Value(this, () => false);

    public async ValueTask SetQuantity(int qty)
        => await Quantity.Set(qty, CancellationToken.None);

    public async ValueTask SubmitOrder()
        => await IsConfirmed.Set(true, CancellationToken.None);

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

---

## 4. Theme Resources

### ColorPaletteOverride.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Meridian Editorial Palette -->
    <Color x:Key="MeridianBackground">#F6F4F0</Color>
    <Color x:Key="MeridianCard">#FFFFFF</Color>
    <Color x:Key="MeridianTextPrimary">#1A1A2E</Color>
    <Color x:Key="MeridianTextMuted">#8A8A8A</Color>
    <Color x:Key="MeridianTextSubtle">#C4C0B8</Color>
    <Color x:Key="MeridianBorder">#E8E4DE</Color>
    <Color x:Key="MeridianGain">#2D6A4F</Color>
    <Color x:Key="MeridianLoss">#B5342B</Color>
    <Color x:Key="MeridianAccent">#C9A96E</Color>
    <Color x:Key="MeridianCardHover">#FAF8F5</Color>
    <Color x:Key="MeridianExpandedBg">#F8F6F2</Color>
    <Color x:Key="MeridianGainBgTint">#0D2D6A4F</Color>
    <Color x:Key="MeridianLossBgTint">#0DB5342B</Color>
    <Color x:Key="MeridianAccentBgTint">#0DC9A96E</Color>

    <SolidColorBrush x:Key="MeridianBackgroundBrush" Color="{StaticResource MeridianBackground}" />
    <SolidColorBrush x:Key="MeridianCardBrush" Color="{StaticResource MeridianCard}" />
    <SolidColorBrush x:Key="MeridianTextPrimaryBrush" Color="{StaticResource MeridianTextPrimary}" />
    <SolidColorBrush x:Key="MeridianTextMutedBrush" Color="{StaticResource MeridianTextMuted}" />
    <SolidColorBrush x:Key="MeridianTextSubtleBrush" Color="{StaticResource MeridianTextSubtle}" />
    <SolidColorBrush x:Key="MeridianBorderBrush" Color="{StaticResource MeridianBorder}" />
    <SolidColorBrush x:Key="MeridianGainBrush" Color="{StaticResource MeridianGain}" />
    <SolidColorBrush x:Key="MeridianLossBrush" Color="{StaticResource MeridianLoss}" />
    <SolidColorBrush x:Key="MeridianAccentBrush" Color="{StaticResource MeridianAccent}" />

    <!-- Uno Material Overrides -->
    <Color x:Key="PrimaryColor">#2D6A4F</Color>
    <Color x:Key="OnPrimaryColor">#FFFFFF</Color>
    <Color x:Key="SecondaryColor">#C9A96E</Color>
    <Color x:Key="BackgroundColor">#F6F4F0</Color>
    <Color x:Key="SurfaceColor">#FFFFFF</Color>
    <Color x:Key="OnSurfaceColor">#1A1A2E</Color>
    <Color x:Key="OnSurfaceVariantColor">#8A8A8A</Color>
    <Color x:Key="OutlineColor">#C4C0B8</Color>
    <Color x:Key="OutlineVariantColor">#E8E4DE</Color>
    <Color x:Key="ErrorColor">#B5342B</Color>
    <Color x:Key="TertiaryColor">#C9A96E</Color>

</ResourceDictionary>
```

### FontResources.xaml
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <FontFamily x:Key="InstrumentSerifFont">ms-appx:///Assets/Fonts/InstrumentSerif-Regular.ttf#Instrument Serif</FontFamily>
    <FontFamily x:Key="InstrumentSerifItalicFont">ms-appx:///Assets/Fonts/InstrumentSerif-Italic.ttf#Instrument Serif</FontFamily>
    <FontFamily x:Key="IBMPlexMonoFont">ms-appx:///Assets/Fonts/IBMPlexMono-Regular.ttf#IBM Plex Mono</FontFamily>
    <FontFamily x:Key="IBMPlexMonoMediumFont">ms-appx:///Assets/Fonts/IBMPlexMono-Medium.ttf#IBM Plex Mono</FontFamily>
    <FontFamily x:Key="OutfitFont">ms-appx:///Assets/Fonts/Outfit-Variable.ttf#Outfit</FontFamily>

</ResourceDictionary>
```

---

## 5. DashboardPage XAML Scaffold — Complete Component Tree

```xml
<!-- Views/DashboardPage.xaml -->
<Page x:Class="Meridian.Views.DashboardPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:utu="using:Uno.Toolkit.UI"
      xmlns:um="using:Uno.Material"
      xmlns:controls="using:Meridian.Controls"
      xmlns:skia="using:SkiaSharp.Views.Windows"
      Background="{StaticResource MeridianBackgroundBrush}">

    <Grid MaxWidth="1400" HorizontalAlignment="Center" Padding="32,20">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />   <!-- Header -->
            <RowDefinition Height="Auto" />   <!-- Ticker Tape -->
            <RowDefinition Height="Auto" />   <!-- Portfolio Hero -->
            <RowDefinition Height="*" />      <!-- Main Content -->
            <RowDefinition Height="Auto" />   <!-- Footer -->
        </Grid.RowDefinitions>

        <!-- ═══════════ ROW 0: HEADER ═══════════ -->
        <Grid Grid.Row="0" Margin="0,0,0,0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <!-- Logo + Title -->
            <StackPanel Grid.Column="0" Orientation="Horizontal"
                        Spacing="12" VerticalAlignment="Center">
                <TextBlock Text="Meridian"
                           FontFamily="{StaticResource InstrumentSerifItalicFont}"
                           FontSize="26"
                           Foreground="{StaticResource MeridianTextPrimaryBrush}" />
                <TextBlock Text="CAPITAL TERMINAL"
                           FontFamily="{StaticResource OutfitFont}"
                           FontSize="10" FontWeight="Medium"
                           Foreground="{StaticResource MeridianTextSubtleBrush}"
                           CharacterSpacing="120"
                           VerticalAlignment="Baseline" />
            </StackPanel>

            <!-- Market Status + Clock -->
            <StackPanel Grid.Column="2" Orientation="Horizontal"
                        Spacing="20" VerticalAlignment="Center">
                <StackPanel Orientation="Horizontal" Spacing="6">
                    <controls:BrailleSpinnerControl />
                    <TextBlock Text="NYSE Open"
                               FontSize="11" FontWeight="Medium"
                               Foreground="{StaticResource MeridianTextMutedBrush}" />
                </StackPanel>
                <Border Width="1" Height="14"
                        Background="{StaticResource MeridianBorderBrush}" />
                <TextBlock x:Name="ClockText"
                           FontFamily="{StaticResource IBMPlexMonoFont}"
                           FontSize="12"
                           Foreground="{StaticResource MeridianTextMutedBrush}" />
            </StackPanel>
        </Grid>

        <!-- ═══════════ ROW 1: BRAILLE TICKER TAPE ═══════════ -->
        <Border Grid.Row="1" Margin="0,14,0,22"
                Padding="0,8"
                BorderThickness="0,1"
                BorderBrush="{StaticResource MeridianBorderBrush}">
            <controls:BrailleTickerControl />
        </Border>

        <!-- ═══════════ ROW 2: PORTFOLIO HERO ═══════════ -->
        <Grid Grid.Row="2" Margin="0,0,0,24">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <!-- Left: Portfolio Value -->
            <StackPanel Grid.Column="0" Spacing="8">
                <TextBlock Text="TOTAL PORTFOLIO VALUE"
                           FontSize="11" FontWeight="SemiBold"
                           Foreground="{StaticResource MeridianTextMutedBrush}"
                           CharacterSpacing="140" />

                <!-- Odometer animated value -->
                <controls:OdometerControl Value="{Binding TotalPortfolioValue}"
                                          FontFamily="{StaticResource InstrumentSerifFont}"
                                          FontSize="62"
                                          Prefix="$" />

                <!-- Gain Pill -->
                <StackPanel Orientation="Horizontal" Spacing="14" Margin="0,4,0,0">
                    <Border CornerRadius="100"
                            Background="{StaticResource MeridianGainBgTint}"
                            Padding="14,6">
                        <StackPanel Orientation="Horizontal" Spacing="6">
                            <!-- Up arrow -->
                            <Viewbox Width="10" Height="10">
                                <Polygon Points="5,0 10,8 0,8"
                                         Fill="{StaticResource MeridianGainBrush}" />
                            </Viewbox>
                            <TextBlock FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                       FontSize="14"
                                       Foreground="{StaticResource MeridianGainBrush}">
                                <Run Text="+$" /><Run Text="{Binding TotalGain}" />
                            </TextBlock>
                            <TextBlock FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                       FontSize="13" Opacity="0.7"
                                       Foreground="{StaticResource MeridianGainBrush}">
                                <Run Text="(" /><Run Text="{Binding TotalGainPct}" /><Run Text="%)" />
                            </TextBlock>
                        </StackPanel>
                    </Border>
                    <TextBlock Text="all time unrealized"
                               FontSize="11" FontWeight="Medium"
                               Foreground="{StaticResource MeridianTextSubtleBrush}"
                               VerticalAlignment="Center" />
                </StackPanel>
            </StackPanel>

            <!-- Right: Index Cards -->
            <ItemsRepeater Grid.Column="1"
                           ItemsSource="{Binding IndexTickers}">
                <ItemsRepeater.Layout>
                    <StackLayout Orientation="Horizontal" Spacing="6" />
                </ItemsRepeater.Layout>
                <ItemsRepeater.ItemTemplate>
                    <DataTemplate>
                        <Border CornerRadius="12"
                                BorderThickness="1"
                                BorderBrush="{StaticResource MeridianBorderBrush}"
                                Background="{StaticResource MeridianCardBrush}"
                                Padding="16,10" MinWidth="120">
                            <StackPanel Spacing="4">
                                <TextBlock Text="{Binding Name}"
                                           FontSize="10" FontWeight="SemiBold"
                                           Foreground="{StaticResource MeridianTextSubtleBrush}"
                                           CharacterSpacing="60" />
                                <TextBlock Text="{Binding Value}"
                                           FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                           FontSize="14" />
                                <TextBlock Text="{Binding Change}"
                                           FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                           FontSize="11"
                                           Foreground="{StaticResource MeridianGainBrush}" />
                                <!-- NOTE: Foreground should be bound to IsUp for green/red -->
                            </StackPanel>
                        </Border>
                    </DataTemplate>
                </ItemsRepeater.ItemTemplate>
            </ItemsRepeater>
        </Grid>

        <!-- ═══════════ ROW 3: MAIN CONTENT GRID ═══════════ -->
        <Grid Grid.Row="3">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*" />       <!-- Left: Charts + Holdings -->
                <ColumnDefinition Width="20" />      <!-- Gap -->
                <ColumnDefinition Width="360" />     <!-- Right: Sidebar -->
            </Grid.ColumnDefinitions>

            <!-- ─── LEFT COLUMN ─── -->
            <StackPanel Grid.Column="0" Spacing="20">

                <!-- PERFORMANCE CHART CARD -->
                <Border CornerRadius="16"
                        BorderThickness="1"
                        BorderBrush="{StaticResource MeridianBorderBrush}"
                        Background="{StaticResource MeridianCardBrush}"
                        Padding="28,24,28,16"
                        x:Name="ChartCard">

                    <StackPanel Spacing="16">
                        <!-- Chart Header: dynamic based on ChartTicker -->
                        <Grid>
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="*" />
                                <ColumnDefinition Width="Auto" />
                            </Grid.ColumnDefinitions>

                            <StackPanel Orientation="Horizontal" Spacing="10">
                                <!-- Back button (visible when stock selected) -->
                                <Button Visibility="{Binding ChartTicker, Converter={StaticResource NullToCollapsedConverter}}"
                                        Command="{Binding BackToPortfolio}"
                                        Width="28" Height="28" Padding="0"
                                        CornerRadius="8"
                                        BorderThickness="1"
                                        BorderBrush="{StaticResource MeridianBorderBrush}"
                                        Background="Transparent">
                                    <FontIcon Glyph="&#xE72B;" FontSize="12" />
                                </Button>

                                <StackPanel>
                                    <!-- Label: "Performance" or Stock Name -->
                                    <TextBlock Text="{Binding ChartLabel}"
                                               FontSize="11" FontWeight="SemiBold"
                                               Foreground="{StaticResource MeridianTextMutedBrush}"
                                               CharacterSpacing="120" />

                                    <!-- Stock detail row (visible when stock selected) -->
                                    <StackPanel Orientation="Horizontal" Spacing="8"
                                                Visibility="{Binding ChartTicker, Converter={StaticResource NullToCollapsedConverter}}"
                                                Margin="0,3,0,0">
                                        <TextBlock FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                                   FontSize="18"
                                                   Text="{Binding SelectedStockPrice}" />
                                        <TextBlock FontFamily="{StaticResource IBMPlexMonoMediumFont}"
                                                   FontSize="12"
                                                   Text="{Binding SelectedStockPct}" />
                                        <Button Content="TRADE"
                                                Command="{Binding OpenTradeFromChart}"
                                                FontSize="10" FontWeight="Bold"
                                                Padding="12,3"
                                                CornerRadius="8"
                                                BorderThickness="1"
                                                BorderBrush="{StaticResource MeridianAccentBrush}"
                                                Foreground="{StaticResource MeridianAccentBrush}"
                                                Background="{StaticResource MeridianAccentBgTint}"
                                                CharacterSpacing="60" />
                                    </StackPanel>
                                </StackPanel>
                            </StackPanel>

                            <!-- Timeframe selector -->
                            <ItemsRepeater Grid.Column="1"
                                           ItemsSource="{Binding Timeframes}">
                                <ItemsRepeater.Layout>
                                    <StackLayout Orientation="Horizontal" Spacing="2" />
                                </ItemsRepeater.Layout>
                                <!-- Each button: Command=SelectTimeframe, parameter=TF string -->
                            </ItemsRepeater>
                        </Grid>

                        <!-- Chart area -->
                        <!-- Option A: SkiaSharp -->
                        <controls:PerformanceChartControl
                            Height="240"
                            ChartData="{Binding ChartData}"
                            StrokeColor="{Binding ChartColor}" />
                        <!-- Option B: LiveCharts2 CartesianChart -->
                    </StackPanel>
                </Border>

                <!-- HOLDINGS + ALLOCATION ROW -->
                <Grid>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="*" />
                        <ColumnDefinition Width="20" />
                        <ColumnDefinition Width="*" />
                    </Grid.ColumnDefinitions>

                    <!-- Holdings Card -->
                    <Border Grid.Column="0" CornerRadius="16"
                            BorderThickness="1"
                            BorderBrush="{StaticResource MeridianBorderBrush}"
                            Background="{StaticResource MeridianCardBrush}"
                            Padding="24,22">
                        <StackPanel Spacing="16">
                            <TextBlock Text="HOLDINGS"
                                       FontSize="11" FontWeight="SemiBold"
                                       Foreground="{StaticResource MeridianTextMutedBrush}"
                                       CharacterSpacing="120" />

                            <ItemsRepeater ItemsSource="{Binding Holdings}">
                                <ItemsRepeater.Layout>
                                    <StackLayout Spacing="6" />
                                </ItemsRepeater.Layout>
                                <ItemsRepeater.ItemTemplate>
                                    <DataTemplate>
                                        <!-- HoldingCard template -->
                                        <!-- Clickable: Command=SelectHolding, Param=Ticker -->
                                        <!-- Shows: ticker, shares×avg, mktVal, gain%, weight bar -->
                                        <!-- Selected state: green dot + tinted border -->
                                    </DataTemplate>
                                </ItemsRepeater.ItemTemplate>
                            </ItemsRepeater>
                        </StackPanel>
                    </Border>

                    <!-- Allocation + Volume Stack -->
                    <StackPanel Grid.Column="2" Spacing="20">

                        <!-- Sector Allocation Card -->
                        <Border CornerRadius="16"
                                BorderThickness="1"
                                BorderBrush="{StaticResource MeridianBorderBrush}"
                                Background="{StaticResource MeridianCardBrush}"
                                Padding="24,22">
                            <StackPanel Spacing="14">
                                <TextBlock Text="SECTOR ALLOCATION"
                                           FontSize="11" FontWeight="SemiBold"
                                           Foreground="{StaticResource MeridianTextMutedBrush}"
                                           CharacterSpacing="120" />
                                <controls:SectorRingControl
                                    Sectors="{Binding Sectors}" />
                            </StackPanel>
                        </Border>

                        <!-- Volume Profile Card -->
                        <Border CornerRadius="16"
                                BorderThickness="1"
                                BorderBrush="{StaticResource MeridianBorderBrush}"
                                Background="{StaticResource MeridianCardBrush}"
                                Padding="24,22">
                            <StackPanel Spacing="8">
                                <TextBlock Text="VOLUME PROFILE"
                                           FontSize="11" FontWeight="SemiBold"
                                           Foreground="{StaticResource MeridianTextMutedBrush}"
                                           CharacterSpacing="120" />
                                <controls:VolumeChartControl
                                    VolumeData="{Binding Volume}" Height="140" />
                            </StackPanel>
                        </Border>
                    </StackPanel>
                </Grid>
            </StackPanel>

            <!-- ─── RIGHT SIDEBAR ─── -->
            <StackPanel Grid.Column="2" Spacing="20">

                <!-- Search Bar -->
                <TextBox PlaceholderText="Search ticker or name…"
                         Text="{Binding SearchQuery, Mode=TwoWay}"
                         FontFamily="{StaticResource OutfitFont}"
                         FontSize="13"
                         CornerRadius="12"
                         BorderThickness="1"
                         BorderBrush="{StaticResource MeridianBorderBrush}"
                         Background="{StaticResource MeridianCardBrush}"
                         Padding="16,10" />

                <!-- Watchlist Card -->
                <Border CornerRadius="16"
                        BorderThickness="1"
                        BorderBrush="{StaticResource MeridianBorderBrush}"
                        Background="{StaticResource MeridianCardBrush}">
                    <StackPanel>
                        <TextBlock Text="WATCHLIST" Margin="22,20,22,12"
                                   FontSize="11" FontWeight="SemiBold"
                                   Foreground="{StaticResource MeridianTextMutedBrush}"
                                   CharacterSpacing="120" />

                        <ListView ItemsSource="{Binding FilteredWatchlist}"
                                  MaxHeight="440"
                                  SelectionMode="None">
                            <ListView.ItemTemplate>
                                <DataTemplate>
                                    <!-- WatchlistRow: ticker + braille activity + chevron -->
                                    <!-- + sparkline + price + delta -->
                                    <!-- Click: ToggleExpanded command -->
                                    <!-- Expanded panel: OHLC + range bar + action buttons -->
                                </DataTemplate>
                            </ListView.ItemTemplate>
                        </ListView>
                    </StackPanel>
                </Border>

                <!-- Market Pulse (News) Card -->
                <Border CornerRadius="16"
                        BorderThickness="1"
                        BorderBrush="{StaticResource MeridianBorderBrush}"
                        Background="{StaticResource MeridianCardBrush}"
                        Padding="22,20,22,16">
                    <StackPanel Spacing="14">
                        <Grid>
                            <TextBlock Text="MARKET PULSE"
                                       FontSize="11" FontWeight="SemiBold"
                                       Foreground="{StaticResource MeridianTextMutedBrush}"
                                       CharacterSpacing="120" />
                            <controls:BraillePulseControl
                                       HorizontalAlignment="Right" />
                        </Grid>

                        <ItemsRepeater ItemsSource="{Binding News}">
                            <ItemsRepeater.Layout>
                                <StackLayout Spacing="14" />
                            </ItemsRepeater.Layout>
                            <ItemsRepeater.ItemTemplate>
                                <DataTemplate>
                                    <StackPanel Orientation="Horizontal" Spacing="12">
                                        <Border CornerRadius="6" Padding="8,3"
                                                Background="{Binding TagBackground}">
                                            <TextBlock Text="{Binding Tag}"
                                                       FontSize="9" FontWeight="SemiBold"
                                                       CharacterSpacing="40" />
                                        </Border>
                                        <StackPanel Spacing="3">
                                            <TextBlock Text="{Binding Text}"
                                                       FontSize="12" TextWrapping="Wrap"
                                                       LineHeight="17" />
                                            <TextBlock FontFamily="{StaticResource IBMPlexMonoFont}"
                                                       FontSize="10"
                                                       Foreground="{StaticResource MeridianTextSubtleBrush}">
                                                <Run Text="{Binding Time}" /><Run Text=" ago" />
                                            </TextBlock>
                                        </StackPanel>
                                    </StackPanel>
                                </DataTemplate>
                            </ItemsRepeater.ItemTemplate>
                        </ItemsRepeater>
                    </StackPanel>
                </Border>
            </StackPanel>
        </Grid>

        <!-- ═══════════ ROW 4: FOOTER ═══════════ -->
        <Grid Grid.Row="4" Margin="0,24,0,0"
              Padding="0,16,0,0"
              BorderThickness="0,1,0,0"
              BorderBrush="{StaticResource MeridianBorderBrush}">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto" />
                <ColumnDefinition Width="*" />
                <ColumnDefinition Width="Auto" />
            </Grid.ColumnDefinitions>

            <TextBlock Grid.Column="0" FontSize="10"
                       Foreground="{StaticResource MeridianTextSubtleBrush}"
                       Text="Simulated data for demonstration. Not financial advice." />
            <controls:BrailleTickerControl Grid.Column="1"
                                           IsCompact="True" Opacity="0.4"
                                           Margin="24,0"
                                           HorizontalAlignment="Center"
                                           MaxWidth="300" />
            <TextBlock Grid.Column="2"
                       FontFamily="{StaticResource IBMPlexMonoFont}"
                       FontSize="10"
                       Foreground="{StaticResource MeridianTextSubtleBrush}"
                       Text="Meridian Terminal v4.0" />
        </Grid>

        <!-- ═══════════ AMBIENT ORB LAYER (behind everything) ═══════════ -->
        <!-- 4 Ellipses with Translation animations, placed in Row 0-4 spanning -->
        <!-- Canvas.ZIndex="-1" to sit behind content -->

    </Grid>

    <!-- ═══════════ TRADE DRAWER OVERLAY ═══════════ -->
    <!-- Conditionally shown when TradeStock != null -->
    <!-- Uses Flyout with DrawerFlyoutPresenter or custom overlay -->

</Page>
```

---

## 6. SkiaSharp Custom Control Scaffold

```csharp
// Controls/SparklineControl.cs
using SkiaSharp;
using SkiaSharp.Views.Windows;

public class SparklineControl : SKXamlCanvas
{
    public static readonly DependencyProperty IsPositiveProperty =
        DependencyProperty.Register(nameof(IsPositive), typeof(bool),
            typeof(SparklineControl), new PropertyMetadata(true, OnDataChanged));

    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register(nameof(Points), typeof(IList<double>),
            typeof(SparklineControl), new PropertyMetadata(null, OnDataChanged));

    public bool IsPositive { get => (bool)GetValue(IsPositiveProperty); set => SetValue(IsPositiveProperty, value); }
    public IList<double> Points { get => (IList<double>)GetValue(PointsProperty); set => SetValue(PointsProperty, value); }

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((SparklineControl)d).Invalidate();

    public SparklineControl()
    {
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var points = Points;
        if (points is null || points.Count < 2) return;

        var w = e.Info.Width;
        var h = e.Info.Height;
        var color = IsPositive
            ? SKColor.Parse("#2D6A4F")
            : SKColor.Parse("#B5342B");

        var min = points.Min();
        var max = points.Max();
        var range = max - min;
        if (range == 0) range = 1;

        var path = new SKPath();
        for (int i = 0; i < points.Count; i++)
        {
            var x = (float)i / (points.Count - 1) * w;
            var y = (float)(h - (points[i] - min) / range * h);
            if (i == 0) path.MoveTo(x, y);
            else path.LineTo(x, y);
        }

        // Area fill
        var areaPath = new SKPath(path);
        areaPath.LineTo(w, h);
        areaPath.LineTo(0, h);
        areaPath.Close();

        using var fillPaint = new SKPaint
        {
            IsAntialias = true,
            Shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0), new SKPoint(0, h),
                new[] { color.WithAlpha(64), color.WithAlpha(0) },
                SKShaderTileMode.Clamp)
        };
        canvas.DrawPath(areaPath, fillPaint);

        // Stroke
        using var strokePaint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = color,
            StrokeWidth = 1.8f,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };
        canvas.DrawPath(path, strokePaint);
    }
}
```

---

## 7. Implementation Priority Order

| Phase | Components | Effort | Dependencies |
|-------|-----------|--------|-------------|
| **P0: Shell** | DashboardPage layout grid, theme resources, font loading | 1 day | None |
| **P1: Static Data** | Models, MockMarketDataService, DashboardModel feeds | 1 day | P0 |
| **P2: Core Cards** | Holdings list, Watchlist list (no expand), News feed | 2 days | P1 |
| **P3: Chart** | PerformanceChartControl (SkiaSharp or LiveCharts2), timeframe selector | 2 days | P1 |
| **P4: Chart Linking** | ChartTicker state, holdings click → chart swap, back button | 1 day | P2, P3 |
| **P5: Watchlist Expand** | Expandable detail panel, OHLC grid, day range bar, action buttons | 1 day | P2 |
| **P6: Custom Viz** | SectorRingControl, VolumeChartControl, SparklineControl | 3 days | P1 |
| **P7: Trade Drawer** | TradeDrawer overlay, TradeDrawerModel, full order form + confirm | 2 days | P2 |
| **P8: Animations** | Card entrances, odometer, braille ticker/spinner/pulse/activity, orbs | 3 days | P0-P7 |
| **P9: Polish** | Hover states, 3D tilt, ripple, search focus ring, responsive breakpoints | 2 days | P0-P8 |

**Total estimated effort: ~18 days for a single developer**

---

## 8. Key Decision Points for Team

Before starting implementation, resolve these:

1. **Charting library**: SkiaSharp custom vs LiveCharts2 for the main area chart
2. **Trade drawer**: DrawerFlyoutPresenter vs custom Popup overlay
3. **Font licensing**: Confirm Instrument Serif can be bundled (it's Google Fonts, so likely OK)
4. **Braille fallback**: Test unicode braille rendering on all target platforms early in P0
5. **Responsive scope**: Is mobile layout in v1 scope or deferred?
6. **Real data API**: Any interface contract for IMarketDataService beyond mocks?
