using Liveline;
using Liveline.Models;
using Meridian.Helpers;
using Meridian.Presentation;
using Meridian.Services;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace Meridian.Views;

public sealed partial class DashboardPage : Page
{
    private readonly DispatcherTimer _animationTimer;
    private IMarketDataService _marketData = null!;
    private FinnhubService _finnhub = null!;
    private string? _currentChartTicker;
    private Border? _currentExpandedPanel;
    private Border? _selectedHoldingBorder;
    private Border? _selectedHoldingDot;
    private int _animationFrame;
    private bool _isDrawerAnimating;

    // Cached braille TextBlock references (avoids visual tree walk every tick)
    private readonly List<TextBlock> _brailleActivityBlocks = new();
    private bool _brailleBlocksCached;

    // Hover brushes (lazy from resources)
    private static SolidColorBrush? _hoverBorderBrush;
    private static SolidColorBrush? _defaultBorderBrush;
    private static SolidColorBrush? _hoverBgBrush;
    private static SolidColorBrush? _transparentBg;

    // Market breathing
    private bool _isMarketOpen;

    // Weighted Paper — spring hover lift
    private Storyboard? _activeCardEnterSb;

    // Weight Whisper — chart gradient density on holding hover
    private double _chartFillTarget = 1.0;
    private double _chartFillCurrent = 1.0;

    // Silence on Leave — chart content exhale
    private Storyboard? _silenceEnterSb;
    private Storyboard? _silenceExitSb;

    // Chart Data Settle — gauge-needle overshoot
    private Storyboard? _chartSettleSb;

    // Tooltip Magnetism — crosshair tracking
    private double _crosshairTargetX = double.NaN;
    private double _crosshairCurrentX = double.NaN;
    private bool _crosshairVisible;

    // Session warmth — hue drifts warmer the longer the session runs
    private readonly DateTime _sessionStartUtc = DateTime.UtcNow;
    private static readonly (double H, double S, double L, byte A)[] OrbBaseHsl =
    [
        (153.5, 0.405, 0.296, 10),  // Green (#2D6A4F)
        (39.0,  0.458, 0.610,  8),  // Gold (#C9A96E)
        (160.0, 0.380, 0.300,  6),  // Teal-green
    ];

    private static SolidColorBrush HoverBorderBrush => _hoverBorderBrush ??= (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
    private static SolidColorBrush DefaultBorderBrush => _defaultBorderBrush ??= (SolidColorBrush)Application.Current.Resources["MeridianBorderBrush"];
    private static SolidColorBrush HoverBgBrush => _hoverBgBrush ??= (SolidColorBrush)Application.Current.Resources["MeridianCardHoverBrush"];
    private static SolidColorBrush TransparentBg => _transparentBg ??= new SolidColorBrush(Microsoft.UI.Colors.Transparent);

    // ── Braille animation constants ──
    private static readonly string[] SpinnerGlyphs = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
    private static readonly string BraillePulsePattern = "⠀⣀⣤⣴⣶⣷⣿⣷⣶⣴⣤⣀⠀⠀⠀⠀⠀⠀";
    private static readonly string[] BrailleActivityGlyphs = ["⠀", "⣀", "⣤", "⣴", "⣶", "⣷", "⣿"];

    public DashboardPage()
    {
        this.InitializeComponent();

        // _marketData / _finnhub / _animationTimer.Start are deferred to OnPageLoaded:
        // App.Services is assigned after `await builder.NavigateAsync<Shell>()` returns,
        // but this ctor runs *during* NavigateAsync — so App.Services is still null here
        // and GetRequiredService throws "Value cannot be null (provider)". DataContext is
        // wired by Uno.Extensions Navigation from the ViewMap; XAML uses {Binding}, and
        // the code-behind uses _marketData for imperative paths (sparklines, ticker tape).

        UpdateClock(); // Initial clock display (uses no DI)

        // Single animation timer — 16ms tick (~60fps), clock piggybacks every ~1s.
        // Started in OnPageLoaded so it can't tick before _finnhub/_marketData exist.
        _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _animationTimer.Tick += OnAnimationTick;

        TradeDrawerPanel.CloseRequested += (_, _) => CloseTradeDrawer();

        // Update clip rect when ticker container sizes
        TickerTapeContainer.SizeChanged += (_, _) =>
        {
            TickerClip.Rect = new Windows.Foundation.Rect(
                0, 0, TickerTapeContainer.ActualWidth, TickerTapeContainer.ActualHeight);
        };

        // Keyboard shortcuts (handledEventsToo catches Escape even from focused TextBoxes)
        AddHandler(KeyDownEvent, new KeyEventHandler(OnPageKeyDown), true);

        // Pause animations when window loses focus to save CPU
        var window = (Application.Current as App)?.MainWindow;
        if (window != null)
        {
            window.Activated += OnWindowActivated;
        }

        Loaded += OnPageLoaded;
        Unloaded += OnPageUnloaded;
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
    {
        // CoreWindowActivationState is used across Uno targets
        if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            _animationTimer.Stop();
        else
            _animationTimer.Start();
    }

    // ── Lifecycle ──────────────────────────────────────────────────────

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Services are safe to resolve here: App.Services is assigned right after
            // NavigateAsync<Shell>() returns, well before any page reaches Loaded.
            _marketData = App.Services.GetRequiredService<IMarketDataService>();
            _finnhub = App.Services.GetRequiredService<FinnhubService>();
            _finnhub.QuotesUpdated += OnLiveQuotesUpdated;
            _finnhub.NewsUpdated += OnLiveNewsUpdated;

            _animationTimer.Start();

            AnimateCardEntrance();
            PopulatePortfolioSummary();

            await LoadChartAsync(null);
            await LoadSkiaControlsAsync();
            _finnhub.Start();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[DashboardPage.OnPageLoaded] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async void PopulatePortfolioSummary()
    {
        try
        {
            var holdings = await _marketData.GetHoldingsAsync(CancellationToken.None);
            var totalValue = holdings.Sum(h => h.MarketValue);
            var totalCost = holdings.Sum(h => h.Shares * h.AvgCost);
            var totalGain = totalValue - totalCost;
            var totalPct = totalCost != 0 ? totalGain / totalCost * 100 : 0;
            var isPositive = totalGain >= 0;

            var sign = isPositive ? "+" : "";
            HeroPortfolioValue.Text = $"${totalValue:N2}";
            HeroPortfolioValue.SetValue(
                AutomationProperties.NameProperty,
                $"Portfolio value: ${totalValue:N2}");
            HeroGainValue.Text = $"{sign}${Math.Abs(totalGain):N2}";
            HeroGainPct.Text = $"({sign}{totalPct:N2}%)";

            var gainBrush = isPositive
                ? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
                : (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];
            var gainBg = isPositive
                ? (SolidColorBrush)Application.Current.Resources["MeridianGainBgTintBrush"]
                : (SolidColorBrush)Application.Current.Resources["MeridianLossBgTintBrush"];
            HeroGainValue.Foreground = gainBrush;
            HeroGainPct.Foreground = gainBrush;
            GainArrowPolygon.Fill = gainBrush;
            GainPill.Background = gainBg;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[DashboardPage.PopulatePortfolioSummary] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private void AnimateCardEntrance()
    {
        // Chart card entrance: opacity 0→1 + translateY 24→0, 700ms
        ChartCard.Opacity = 0;
        ChartCardTranslate.Y = 24;

        var fadeIn = new DoubleAnimation
        {
            From = 0, To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            BeginTime = TimeSpan.FromMilliseconds(150)
        };
        Storyboard.SetTarget(fadeIn, ChartCard);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        var slideUp = new DoubleAnimation
        {
            From = 24, To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            BeginTime = TimeSpan.FromMilliseconds(150)
        };
        Storyboard.SetTarget(slideUp, ChartCard);
        Storyboard.SetTargetProperty(slideUp, "(UIElement.RenderTransform).(TranslateTransform.Y)");

        var sb = new Storyboard();
        sb.Children.Add(fadeIn);
        sb.Children.Add(slideUp);
        sb.Begin();
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        _animationTimer.Stop();
        // _finnhub may still be null if Unloaded fires before Loaded completed.
        _finnhub?.Stop();
        _brailleActivityBlocks.Clear();
        _brailleBlocksCached = false;
        _cardLiftCache.Clear();

        var window = (Application.Current as App)?.MainWindow;
        if (window != null)
            window.Activated -= OnWindowActivated;
    }

    private async Task LoadSkiaControlsAsync()
    {
        var sectors = await _marketData.GetSectorsAsync(CancellationToken.None);
        SectorRing.Sectors = sectors.ToList();

        var volume = await _marketData.GetVolumeAsync(CancellationToken.None);
        VolumeChart.VolumeData = volume.ToList();

        // Load sparkline data once ItemsRepeater has materialized children.
        // MVUX Watchlist feed resolves asynchronously, so the first LayoutUpdated
        // tick can fire before any SparklineControl exists. Stay subscribed until
        // at least one sparkline is found, then run once.
        void OnLayoutReady(object? s, object e)
        {
            var found = new List<Controls.SparklineControl>();
            FindSparklines(this, found);
            if (found.Count == 0) return; // wait for items to materialize

            // LayoutUpdated's sender is always null in WinUI/Uno, so unsubscribe
            // via the captured instance — casting the null sender throws (and the
            // throw is fatal when raised synchronously from TextBox text input).
            this.LayoutUpdated -= OnLayoutReady;
            _ = PopulateSparklines();
        }
        this.LayoutUpdated += OnLayoutReady;
    }

    private async Task PopulateSparklines()
    {
        try
        {
            var sparklines = new List<Controls.SparklineControl>();
            FindSparklines(this, sparklines);

            foreach (var spark in sparklines)
            {
                var ticker = ExtractTicker(spark.DataContext);
                if (ticker == null) continue;

                var history = await _marketData.GetStockHistoryAsync(ticker, CancellationToken.None);
                if (history.Count == 0) continue;

                // Use last 24 data points for mini chart
                var points = history.TakeLast(24).Select(p => (double)p.Value).ToList();
                spark.Points = points;
                spark.IsPositive = points.Last() >= points.First();
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[DashboardPage.PopulateSparklines] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private static void FindSparklines(DependencyObject parent, List<Controls.SparklineControl> results)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is Controls.SparklineControl sc)
                results.Add(sc);
            else
                FindSparklines(child, results);
        }
    }

    // ── Trade Drawer (pre-created storyboards) ────────────────────────

    private Storyboard? _drawerOpenSb;
    private Storyboard? _drawerCloseSb;
    private TranslateTransform? _drawerTransform;

    private void EnsureDrawerStoryboards()
    {
        if (_drawerOpenSb != null) return;

        _drawerTransform = new TranslateTransform { X = 420 };
        TradeDrawerPanel.RenderTransform = _drawerTransform;

        // Open storyboard
        var openBackdrop = new DoubleAnimation
        {
            From = 0, To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(200)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
        };
        Storyboard.SetTarget(openBackdrop, DrawerBackdrop);
        Storyboard.SetTargetProperty(openBackdrop, "Opacity");

        var openSlide = new DoubleAnimation
        {
            From = 420, To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(350)),
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 },
        };
        Storyboard.SetTarget(openSlide, TradeDrawerPanel);
        Storyboard.SetTargetProperty(openSlide, "(UIElement.RenderTransform).(TranslateTransform.X)");

        _drawerOpenSb = new Storyboard();
        _drawerOpenSb.Children.Add(openBackdrop);
        _drawerOpenSb.Children.Add(openSlide);
        _drawerOpenSb.Completed += (_, _) =>
        {
            _isDrawerAnimating = false;
            TradeDrawerPanel.FocusQuantityInput();
        };

        // Close storyboard
        var closeSlide = new DoubleAnimation
        {
            From = 0, To = 420,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
        };
        Storyboard.SetTarget(closeSlide, TradeDrawerPanel);
        Storyboard.SetTargetProperty(closeSlide, "(UIElement.RenderTransform).(TranslateTransform.X)");

        var closeBackdrop = new DoubleAnimation
        {
            From = 1, To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
        };
        Storyboard.SetTarget(closeBackdrop, DrawerBackdrop);
        Storyboard.SetTargetProperty(closeBackdrop, "Opacity");

        _drawerCloseSb = new Storyboard();
        _drawerCloseSb.Children.Add(closeSlide);
        _drawerCloseSb.Children.Add(closeBackdrop);
        _drawerCloseSb.Completed += (_, _) =>
        {
            DrawerBackdrop.Visibility = Visibility.Collapsed;
            TradeDrawerPanel.Visibility = Visibility.Collapsed;
            _isDrawerAnimating = false;
        };
    }

    private async void OpenTradeDrawer(string ticker)
    {
        if (_isDrawerAnimating) return;

        try
        {
            var watchlist = await _marketData.GetWatchlistAsync(CancellationToken.None);
            var stock = watchlist.FirstOrDefault(s => s.Ticker == ticker);
            if (stock == null) return;

            TradeDrawerPanel.SetStock(stock);
            EnsureDrawerStoryboards();

            DrawerBackdrop.Opacity = 0;
            DrawerBackdrop.Visibility = Visibility.Visible;
            TradeDrawerPanel.Visibility = Visibility.Visible;
            _drawerTransform!.X = 420;

            _isDrawerAnimating = true;
            _drawerOpenSb!.Stop();
            _drawerOpenSb.Begin();
        }
        catch (Exception ex)
        {
            _isDrawerAnimating = false;
            System.Diagnostics.Debug.WriteLine($"Trade drawer error: {ex.Message}");
        }
    }

    private void CloseTradeDrawer()
    {
        if (_isDrawerAnimating) return;

        EnsureDrawerStoryboards();
        _isDrawerAnimating = true;
        _drawerCloseSb!.Stop();
        _drawerCloseSb.Begin();
    }

    private void OnDrawerBackdropTapped(object sender, TappedRoutedEventArgs e)
    {
        if (!_isDrawerAnimating) CloseTradeDrawer();
    }

    // ── Animation Timer ───────────────────────────────────────────────

    private void OnAnimationTick(object? sender, object e)
    {
        _animationFrame = (_animationFrame + 1) % 100_000;

        // Smooth ticker scroll every frame (must be 60fps)
        UpdateTickerScroll();

        // Tooltip Magnetism: smooth crosshair tracking (every frame)
        UpdateCrosshair();

        // Pulse animations every 2nd frame (30fps is smooth enough for breathing)
        if (_animationFrame % 2 == 0)
        {
            var phase = (_animationFrame % 188) / 188.0 * Math.PI * 2;
            var sinVal = Math.Sin(phase);
            GainPill.Opacity = 0.72 + 0.28 * sinVal;
            var scale = 1.0 + 0.02 * sinVal;
            GainPillScale.ScaleX = scale;
            GainPillScale.ScaleY = scale;

            if (_selectedHoldingDot != null)
                _selectedHoldingDot.Opacity = 0.4 + 0.6 * (0.5 + 0.5 * sinVal);

            // Market breathing: 5s sinusoidal glow on chart card
            UpdateMarketBreathing();

            // Weight Whisper: smoothly interpolate chart fill opacity
            if (Math.Abs(_chartFillCurrent - _chartFillTarget) > 0.001)
            {
                _chartFillCurrent += (_chartFillTarget - _chartFillCurrent) * 0.08;
                LivelineChart.FillOpacity = _chartFillCurrent;
            }
        }

        // Slower decorative animations
        if (_animationFrame % 5 == 0)
            BrailleSpinner.Text = SpinnerGlyphs[(_animationFrame / 5) % SpinnerGlyphs.Length];

        if (_animationFrame % 9 == 0)
            UpdateBraillePulse();

        if (_animationFrame % 13 == 0)
            UpdateBrailleActivity();

        // Clock update ~1s (62 * 16ms ≈ 992ms)
        if (_animationFrame % 62 == 0)
        {
            UpdateClock();
            _isMarketOpen = MarketHoursHelper.IsMarketOpen();
        }

        // Session warmth: update orb hue every ~10s
        if (_animationFrame % 625 == 0)
            UpdateSessionWarmth();

        // Empty state check every ~500ms
        if (_animationFrame % 30 == 0)
            CheckWatchlistEmpty();
    }

    // ── Ticker Tape ───────────────────────────────────────────────────

    private bool _tickerTapeInitialized;
    private bool _useLiveData;
    private double _tickerWidth;
    private bool _tickerPositioned;

    private void BuildTickerTapeText()
    {
        var tickers = _useLiveData
            ? _finnhub.GetLatestQuotes()
            : (IReadOnlyList<StreamTicker>)_marketData.GetStreamTickers();

        // Build ticker text for dual-TextBlock marquee (A + B leapfrog)
        var segment = new System.Text.StringBuilder();
        foreach (var t in tickers)
            segment.Append($"⣤⣴⣶⣷ {t.Ticker}  {t.Price}  {t.Delta}   │   ");

        var tickerText = segment.ToString();
        TickerTapeTextA.Text = tickerText;
        TickerTapeTextB.Text = tickerText;

        // Re-measure width after layout (deferred), but don't stop scrolling
        if (_tickerPositioned)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                var w = TickerTapeTextA.ActualWidth;
                if (w > 50) _tickerWidth = w;
            });
        }

        // Also populate footer ticker
        BuildFooterTickerText(tickers);
    }

    private void BuildFooterTickerText(IReadOnlyList<StreamTicker> tickers)
    {
        var sb = new System.Text.StringBuilder();
        for (int repeat = 0; repeat < 5; repeat++)
        {
            foreach (var t in tickers)
                sb.Append($"⣤⣴⣶⣷ {t.Ticker} {t.Price}  ·  ");
        }
        FooterTickerText.Text = sb.ToString();
    }

    private void InitTickerTape()
    {
        if (_tickerTapeInitialized) return;
        _tickerTapeInitialized = true;
        BuildTickerTapeText();
    }

    private void OnLiveQuotesUpdated()
    {
        // Called from Finnhub when new quotes arrive — update ticker tape on UI thread
        DispatcherQueue.TryEnqueue(() =>
        {
            _useLiveData = true;
            BuildTickerTapeText();
        });
    }

    private void OnLiveNewsUpdated(IReadOnlyList<NewsItem> news)
    {
        // Called from Finnhub when news arrives — update Market Pulse on UI thread
        DispatcherQueue.TryEnqueue(() =>
        {
            NewsRepeater.ItemsSource = news;
        });
    }

    private void UpdateTickerScroll()
    {
        InitTickerTape();

        // Measure once after layout
        if (_tickerWidth <= 0)
        {
            var w = TickerTapeTextA.ActualWidth;
            if (w > 50) _tickerWidth = w;
            else return;
        }

        // Position B right after A on first valid measurement
        if (!_tickerPositioned)
        {
            _tickerPositioned = true;
            TickerTranslateA.X = 0;
            TickerTranslateB.X = _tickerWidth;
        }

        // Scroll both TextBlocks left
        TickerTranslateA.X -= 0.95;
        TickerTranslateB.X -= 0.95;

        // Leapfrog: when fully off-screen left, jump behind the other
        if (TickerTranslateA.X < -_tickerWidth)
            TickerTranslateA.X = TickerTranslateB.X + _tickerWidth;
        if (TickerTranslateB.X < -_tickerWidth)
            TickerTranslateB.X = TickerTranslateA.X + _tickerWidth;

        // Footer ticker scroll with modular reset (prevents float overflow)
        FooterTickerTranslate.X -= 0.29;
        var fw = FooterTickerText.ActualWidth;
        if (fw > 50 && FooterTickerTranslate.X < -fw / 5.0)
            FooterTickerTranslate.X += fw / 5.0;
    }

    private int _pulseOffset;
    private void UpdateBraillePulse()
    {
        _pulseOffset = (_pulseOffset + 1) % BraillePulsePattern.Length;
        BraillePulse.Text = BraillePulsePattern[_pulseOffset..] + BraillePulsePattern[.._pulseOffset];
    }

    // ── Braille Activity (cached references) ──────────────────────────

    private int _brailleActivityFrame;
    private readonly System.Text.StringBuilder _brailleActivitySb = new(6);

    private void CacheBrailleBlocks()
    {
        if (_brailleBlocksCached) return;
        _brailleBlocksCached = true;
        _brailleActivityBlocks.Clear();
        FindTaggedBlocks(this, "BrailleActivity", _brailleActivityBlocks);
    }

    private static void FindTaggedBlocks(DependencyObject parent, string tag, List<TextBlock> results)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is TextBlock tb && tb.Tag as string == tag)
                results.Add(tb);
            else
                FindTaggedBlocks(child, tag, results);
        }
    }

    private void UpdateBrailleActivity()
    {
        CacheBrailleBlocks();
        if (_brailleActivityBlocks.Count == 0) return;

        _brailleActivityFrame = (_brailleActivityFrame + 1) % 10_000;

        foreach (var tb in _brailleActivityBlocks)
        {
            var pct = ExtractPct(tb.DataContext);
            var intensity = Math.Min(1.0, Math.Abs(pct) / 2.0 + 0.15);
            var seed = Math.Abs(tb.DataContext?.GetHashCode() ?? 0) * 0.01;

            _brailleActivitySb.Clear();
            for (int i = 0; i < 6; i++)
            {
                var v = (Math.Sin(_brailleActivityFrame * 0.3 * intensity + i * 0.9 + seed) * 0.5 + 0.5) * intensity;
                var level = (int)Math.Round(v * (BrailleActivityGlyphs.Length - 1));
                _brailleActivitySb.Append(BrailleActivityGlyphs[Math.Clamp(level, 0, BrailleActivityGlyphs.Length - 1)]);
            }
            tb.Text = _brailleActivitySb.ToString();
            tb.Opacity = 0.25 + intensity * 0.65;
        }
    }

    // ── Chart (Liveline) ──────────────────────────────────────────────

    private async Task LoadChartAsync(string? ticker)
    {
        try
        {
            _currentChartTicker = ticker;

            var allPoints = string.IsNullOrEmpty(ticker)
                ? await _marketData.GetPortfolioHistoryAsync(CancellationToken.None)
                : await _marketData.GetStockHistoryAsync(ticker, CancellationToken.None);

            var points = FilterByTimeframe(allPoints, _activeTimeframe);
            if (points.Count == 0) return;

            // Convert to Liveline data points
            var livelineData = new List<LivelinePoint>(points.Count);
            foreach (var p in points)
            {
                var dt = DateTime.TryParse(p.Date, out var parsed)
                    ? new DateTimeOffset(parsed) : DateTimeOffset.Now;
                livelineData.Add(new LivelinePoint(dt, (double)p.Value));
            }

            var lastValue = (double)points[^1].Value;
            var isPositive = points.Count >= 2 && points[^1].Value >= points[0].Value;

            // Set theme color based on gain/loss
            LivelineChart.Theme = new LivelineTheme
            {
                Color = isPositive ? "#2D6A4F" : "#B5342B",
                IsDark = false
            };

            // Push data — Liveline handles smooth lerp animation
            LivelineChart.Data = livelineData;
            LivelineChart.Value = lastValue;
            LivelineChart.IsLoading = false;

            UpdateChartHeader(ticker);
            PlayChartSettleAnimation();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[DashboardPage.LoadChartAsync] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
        }
    }

    private async void UpdateChartHeader(string? ticker)
    {
        if (string.IsNullOrEmpty(ticker))
        {
            ChartLabel.Text = "PERFORMANCE";
            StockDetailPanel.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            return;
        }

        try
        {
            var watchlist = await _marketData.GetWatchlistAsync(CancellationToken.None);
            var stock = watchlist.FirstOrDefault(s => s.Ticker == ticker);
            if (stock == null) return;

            ChartLabel.Text = stock.Name.ToUpperInvariant();
            StockPrice.Text = $"${stock.Price:N2}";
            StockDelta.Text = $"{(stock.Pct >= 0 ? "+" : "")}{stock.Pct:N2}%";
            StockDelta.Foreground = stock.Pct >= 0
                ? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
                : (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];

            StockDetailPanel.Visibility = Visibility.Visible;
            BackButton.Visibility = Visibility.Visible;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Chart header error: {ex.Message}");
        }
    }

    // ── Holdings tap ──────────────────────────────────────────────────

    private static string? ExtractTicker(object? dataContext)
    {
        if (dataContext is null) return null;
        // Direct record match (non-MVUX paths)
        if (dataContext is Holding h) return h.Ticker;
        if (dataContext is Stock s) return s.Ticker;
        // MVUX proxies don't match the original record type — use reflection
        var prop = dataContext.GetType().GetProperty("Ticker");
        return prop?.GetValue(dataContext) as string;
    }

    private async void OnHoldingTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not Border tappedBorder) return;
        if (tappedBorder.DataContext is not { } dc) return;

        var ticker = ExtractTicker(dc);
        if (ticker == null) return;

        var isDeselecting = _currentChartTicker == ticker;

        // Clear previous selection
        if (_selectedHoldingBorder != null)
        {
            _selectedHoldingBorder.BorderBrush = DefaultBorderBrush;
            _selectedHoldingBorder.Background = TransparentBg;
        }
        if (_selectedHoldingDot != null)
            _selectedHoldingDot.Visibility = Visibility.Collapsed;

        if (isDeselecting)
        {
            _selectedHoldingBorder = null;
            _selectedHoldingDot = null;
            await LoadChartAsync(null);
        }
        else
        {
            // Highlight new selection
            _selectedHoldingBorder = tappedBorder;
            tappedBorder.BorderBrush = HoverBorderBrush;
            tappedBorder.Background = HoverBgBrush;

            // Find and show the ActiveDot
            _selectedHoldingDot = FindActiveDot(tappedBorder);
            if (_selectedHoldingDot != null)
                _selectedHoldingDot.Visibility = Visibility.Visible;

            await LoadChartAsync(ticker);
        }
    }

    private static Border? FindActiveDot(DependencyObject parent)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is Border b && b.Tag as string == "ActiveDot")
                return b;
            var found = FindActiveDot(child);
            if (found != null) return found;
        }
        return null;
    }

    private async void OnBackButtonClick(object sender, RoutedEventArgs e)
    {
        // Clear holding selection
        if (_selectedHoldingBorder != null)
        {
            _selectedHoldingBorder.BorderBrush = DefaultBorderBrush;
            _selectedHoldingBorder.Background = TransparentBg;
            _selectedHoldingBorder = null;
        }
        if (_selectedHoldingDot != null)
        {
            _selectedHoldingDot.Visibility = Visibility.Collapsed;
            _selectedHoldingDot = null;
        }
        await LoadChartAsync(null);
    }

    // ── Watchlist expand/collapse ─────────────────────────────────────

    private void OnWatchlistRowTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement tappedRow) return;
        if (tappedRow.Parent is not StackPanel parent) return;

        Border? expandedPanel = null;
        foreach (var child in parent.Children)
        {
            if (child is Border b && b.Tag as string == "ExpandedPanel")
            {
                expandedPanel = b;
                break;
            }
        }
        if (expandedPanel is null) return;

        var chevron = FindTaggedElement<TextBlock>(parent, "Chevron");

        // Collapse previous panel (instant if different row)
        if (_currentExpandedPanel != null && _currentExpandedPanel != expandedPanel)
        {
            var prevChevron = FindTaggedElement<TextBlock>(_currentExpandedPanel.Parent as DependencyObject ?? this, "Chevron");
            CollapseWatchlistPanel(_currentExpandedPanel, prevChevron);
        }

        var isExpanding = expandedPanel.Visibility != Visibility.Visible;

        if (isExpanding)
        {
            expandedPanel.Visibility = Visibility.Visible;
            ExpandWatchlistPanel(expandedPanel, chevron);
            _currentExpandedPanel = expandedPanel;
        }
        else
        {
            CollapseWatchlistPanel(expandedPanel, chevron);
            _currentExpandedPanel = null;
        }

        // Invalidate braille cache when watchlist layout changes
        _brailleBlocksCached = false;
    }

    private void ExpandWatchlistPanel(Border panel, TextBlock? chevron)
    {
        // Compute dynamic day-range bar from Stock data
        UpdateDayRangeBar(panel);

        // Panel: fade in + slide down
        var fadeIn = new DoubleAnimation
        {
            From = 0, To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(fadeIn, panel);
        Storyboard.SetTargetProperty(fadeIn, "Opacity");

        var slideDown = new DoubleAnimation
        {
            From = -8, To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(slideDown, panel);
        Storyboard.SetTargetProperty(slideDown, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

        var sb = new Storyboard();
        sb.Children.Add(fadeIn);
        sb.Children.Add(slideDown);
        sb.Begin();

        // Chevron: rotate 0→180°
        if (chevron?.RenderTransform is RotateTransform)
        {
            var rotate = new DoubleAnimation
            {
                To = 180,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(rotate, chevron);
            Storyboard.SetTargetProperty(rotate, "(UIElement.RenderTransform).(RotateTransform.Angle)");
            var chevSb = new Storyboard();
            chevSb.Children.Add(rotate);
            chevSb.Begin();
        }
    }

    private static void UpdateDayRangeBar(Border panel)
    {
        var ticker = ExtractTicker(panel.DataContext);
        if (ticker == null || panel.DataContext == null) return;

        var type = panel.DataContext.GetType();
        var ratioProp = type.GetProperty("DayRangeRatio");
        if (ratioProp?.GetValue(panel.DataContext) is not double ratio) return;

        // Find the Grid with 3 columns inside the panel (fill, dot, remainder)
        static Grid? FindRangeGrid(DependencyObject parent)
        {
            var count = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is Grid g && g.ColumnDefinitions.Count == 3
                    && g.Height is 6)
                    return g;
                var found = FindRangeGrid(child);
                if (found != null) return found;
            }
            return null;
        }

        var rangeGrid = FindRangeGrid(panel);
        if (rangeGrid == null) return;

        rangeGrid.ColumnDefinitions[0].Width = new GridLength(ratio, GridUnitType.Star);
        rangeGrid.ColumnDefinitions[2].Width = new GridLength(1.0 - ratio, GridUnitType.Star);
    }

    private void CollapseWatchlistPanel(Border panel, TextBlock? chevron)
    {
        // Panel: fade out + slide up
        var fadeOut = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(fadeOut, panel);
        Storyboard.SetTargetProperty(fadeOut, "Opacity");

        var slideUp = new DoubleAnimation
        {
            To = -8,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(slideUp, panel);
        Storyboard.SetTargetProperty(slideUp, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

        var sb = new Storyboard();
        sb.Children.Add(fadeOut);
        sb.Children.Add(slideUp);
        sb.Completed += (_, _) => panel.Visibility = Visibility.Collapsed;
        sb.Begin();

        // Chevron: rotate back to 0°
        if (chevron?.RenderTransform is RotateTransform)
        {
            var rotate = new DoubleAnimation
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(rotate, chevron);
            Storyboard.SetTargetProperty(rotate, "(UIElement.RenderTransform).(RotateTransform.Angle)");
            var chevSb = new Storyboard();
            chevSb.Children.Add(rotate);
            chevSb.Begin();
        }
    }

    private static T? FindTaggedElement<T>(DependencyObject parent, string tag) where T : FrameworkElement
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T fe && fe.Tag as string == tag)
                return fe;
            var found = FindTaggedElement<T>(child, tag);
            if (found != null) return found;
        }
        return null;
    }

    private async void OnViewChartFromWatchlist(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
        {
            var parent = fe;
            while (parent != null)
            {
                var ticker = ExtractTicker(parent.DataContext);
                if (ticker != null)
                {
                    await LoadChartAsync(ticker);
                    return;
                }
                parent = parent.Parent as FrameworkElement;
            }
        }
    }

    private void OnChartTradeButtonClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_currentChartTicker))
            OpenTradeDrawer(_currentChartTicker);
    }

    private void OnWatchlistTradeButtonTapped(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
        {
            var parent = fe;
            while (parent != null)
            {
                var ticker = ExtractTicker(parent.DataContext);
                if (ticker != null)
                {
                    OpenTradeDrawer(ticker);
                    return;
                }
                parent = parent.Parent as FrameworkElement;
            }
        }
    }

    // ── Stock Detail navigation ────────────────────────────────────────

    private async void OnWatchlistDetailsButtonTapped(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
        {
            // Walk up the tree to find the ticker from DataContext
            var parent = fe;
            string? ticker = null;
            while (parent != null && ticker == null)
            {
                ticker = ExtractTicker(parent.DataContext);
                parent = parent.Parent as FrameworkElement;
            }

            if (ticker == null) return;

            // Region nav: navigates to "StockDetail" route at the Shell-content region level
            // and hands the ticker as nav data — the framework injects it into StockDetailModel's
            // `string Ticker` primary constructor parameter via the DataViewMap registration.
            var nav = this.Navigator();
            if (nav is not null)
                await nav.NavigateRouteAsync(this, "StockDetail", data: ticker);
        }
    }

    private async void OnChartDetailsButtonClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentChartTicker)) return;

        var nav = this.Navigator();
        if (nav is not null)
            await nav.NavigateRouteAsync(this, "StockDetail", data: _currentChartTicker);
    }

    // ── Hover effects ─────────────────────────────────────────────────

    private void OnCardPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b)
        {
            b.BorderBrush = HoverBorderBrush;
            b.Background = HoverBgBrush;

            // Weighted Paper: press-down-then-lift spring animation
            AnimateWeightedPaperEnter(b);

            // Weight Whisper: shift chart gradient density proportional to weight
            var marketValue = ExtractMarketValue(b.DataContext);
            if (marketValue > 0)
            {
                var weightRatio = Math.Clamp(marketValue / 22000.0, 0.05, 1.0);
                _chartFillTarget = 1.0 + weightRatio * 0.5;
            }
        }
    }

    private void OnCardPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b)
        {
            // Don't clear highlight on the selected holding
            if (b == _selectedHoldingBorder) return;
            b.BorderBrush = DefaultBorderBrush;
            b.Background = TransparentBg;

            // Weighted Paper: return spring
            AnimateWeightedPaperExit(b);

            // Weight Whisper: reset gradient density
            _chartFillTarget = 1.0;
        }
    }

    private void OnRowPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b) b.Background = HoverBgBrush;
    }

    private void OnRowPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b) b.Background = TransparentBg;
    }

    // News item hover slide (translateX 4px per spec)
    private void OnNewsItemPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
            fe.RenderTransform = new TranslateTransform { X = 4 };
    }

    private void OnNewsItemPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
            fe.RenderTransform = null;
    }

    private void OnChartCardPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b)
        {
            b.BorderBrush = HoverBorderBrush;

            // Card shadow: subtle lift (skip ChartCard — has its own transforms)
            if (sender != ChartCard)
                AnimateCardLift(b, true);
        }

        // Silence on Leave: snap content to full opacity
        if (sender == ChartCard)
        {
            EnsureSilenceStoryboards();
            _silenceExitSb!.Stop();
            _silenceEnterSb!.Begin();
        }
    }

    private void OnChartCardPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (sender is Border b)
        {
            b.BorderBrush = DefaultBorderBrush;

            if (sender != ChartCard)
                AnimateCardLift(b, false);
        }

        // Silence on Leave: exhale — dip to 92% then recover
        if (sender == ChartCard)
        {
            EnsureSilenceStoryboards();
            _silenceEnterSb!.Stop();
            _silenceExitSb!.Begin();
        }
    }

    // Cached lift storyboards per card to avoid GC pressure on hover
    private readonly Dictionary<Border, (Storyboard Sb, DoubleAnimation TransY, DoubleAnimation ScaleX, DoubleAnimation ScaleY)> _cardLiftCache = new();

    private void AnimateCardLift(Border card, bool lifting)
    {
        if (card.RenderTransform is not CompositeTransform)
        {
            card.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            card.RenderTransform = new CompositeTransform();
        }

        if (!_cardLiftCache.TryGetValue(card, out var cached))
        {
            var ease = new CubicEase { EasingMode = EasingMode.EaseOut };
            var dur = new Duration(TimeSpan.FromMilliseconds(300));

            var translateAnim = new DoubleAnimation { Duration = dur, EasingFunction = ease };
            Storyboard.SetTarget(translateAnim, card);
            Storyboard.SetTargetProperty(translateAnim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            var scaleXAnim = new DoubleAnimation { Duration = dur, EasingFunction = ease };
            Storyboard.SetTarget(scaleXAnim, card);
            Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

            var scaleYAnim = new DoubleAnimation { Duration = dur, EasingFunction = ease };
            Storyboard.SetTarget(scaleYAnim, card);
            Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

            var sb = new Storyboard();
            sb.Children.Add(translateAnim);
            sb.Children.Add(scaleXAnim);
            sb.Children.Add(scaleYAnim);

            cached = (sb, translateAnim, scaleXAnim, scaleYAnim);
            _cardLiftCache[card] = cached;
        }

        cached.TransY.To = lifting ? -3 : 0;
        cached.ScaleX.To = lifting ? 1.008 : 1.0;
        cached.ScaleY.To = lifting ? 1.008 : 1.0;
        cached.Sb.Stop();
        cached.Sb.Begin();
    }

    private void UpdateClock()
    {
        ClockText.Text = DateTime.Now.ToString("ddd, MMM d · hh:mm:ss tt");
    }

    // ── Market Breathing ─────────────────────────────────────────────

    private void UpdateMarketBreathing()
    {
        if (_isMarketOpen)
        {
            // 5-second sinusoidal cycle (313 frames at 16ms ≈ 5s)
            var breathPhase = (_animationFrame % 313) / 313.0 * Math.PI * 2;
            var breathVal = (Math.Sin(breathPhase) + 1) / 2; // 0 to 1
            ChartBreathingGlow.Opacity = breathVal * 0.15;
        }
        else if (ChartBreathingGlow.Opacity > 0)
        {
            // Ease-out fade when market closes (~1.2s)
            ChartBreathingGlow.Opacity *= 0.95;
            if (ChartBreathingGlow.Opacity < 0.001) ChartBreathingGlow.Opacity = 0;
        }
    }

    // ── Session Warmth ───────────────────────────────────────────────

    private void UpdateSessionWarmth()
    {
        var elapsed = (DateTime.UtcNow - _sessionStartUtc).TotalSeconds;
        var hueShift = Math.Min(elapsed * 0.02, 25.0);
        if (hueShift < 0.1) return; // No visible change yet

        Orb1Center.Color = HslToColor(OrbBaseHsl[0].H + hueShift, OrbBaseHsl[0].S, OrbBaseHsl[0].L, OrbBaseHsl[0].A);
        Orb2Center.Color = HslToColor(OrbBaseHsl[1].H + hueShift, OrbBaseHsl[1].S, OrbBaseHsl[1].L, OrbBaseHsl[1].A);
        Orb3Center.Color = HslToColor(OrbBaseHsl[2].H + hueShift, OrbBaseHsl[2].S, OrbBaseHsl[2].L, OrbBaseHsl[2].A);
    }

    private static Windows.UI.Color HslToColor(double h, double s, double l, byte a)
    {
        h %= 360;
        if (h < 0) h += 360;
        var c = (1 - Math.Abs(2 * l - 1)) * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = l - c / 2;

        double r, g, b;
        if (h < 60) { r = c; g = x; b = 0; }
        else if (h < 120) { r = x; g = c; b = 0; }
        else if (h < 180) { r = 0; g = c; b = x; }
        else if (h < 240) { r = 0; g = x; b = c; }
        else if (h < 300) { r = x; g = 0; b = c; }
        else { r = c; g = 0; b = x; }

        return Windows.UI.Color.FromArgb(a,
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255));
    }

    // ── Data-Driven Braille Helper ───────────────────────────────────

    private static double ExtractPct(object? dataContext)
    {
        if (dataContext is Stock s) return (double)s.Pct;
        try { return (double)((dynamic)dataContext!).Pct; }
        catch { return 0; }
    }

    // ── Weighted Paper ───────────────────────────────────────────────

    private void AnimateWeightedPaperEnter(Border card)
    {
        if (card.RenderTransform is not CompositeTransform) return;

        _activeCardEnterSb?.Stop();

        // TranslateY: press → lift → settle back to origin
        var translateAnim = new DoubleAnimationUsingKeyFrames();
        translateAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        translateAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.5, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(80)) });
        translateAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = -2,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 4 }
        });
        translateAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
        Storyboard.SetTarget(translateAnim, card);
        Storyboard.SetTargetProperty(translateAnim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

        // Scale: subtle elevation once settled — card feels like it's floating
        var scaleXAnim = new DoubleAnimationUsingKeyFrames();
        scaleXAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)) });
        scaleXAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.015,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
        Storyboard.SetTarget(scaleXAnim, card);
        Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

        var scaleYAnim = new DoubleAnimationUsingKeyFrames();
        scaleYAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)) });
        scaleYAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.015,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
        Storyboard.SetTarget(scaleYAnim, card);
        Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

        _activeCardEnterSb = new Storyboard();
        _activeCardEnterSb.Children.Add(translateAnim);
        _activeCardEnterSb.Children.Add(scaleXAnim);
        _activeCardEnterSb.Children.Add(scaleYAnim);
        _activeCardEnterSb.Begin();
    }

    private void AnimateWeightedPaperExit(Border card)
    {
        if (card.RenderTransform is not CompositeTransform) return;

        _activeCardEnterSb?.Stop();

        var translateAnim = new DoubleAnimation
        {
            To = 0,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(translateAnim, card);
        Storyboard.SetTargetProperty(translateAnim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

        var scaleXAnim = new DoubleAnimation
        {
            To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleXAnim, card);
        Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

        var scaleYAnim = new DoubleAnimation
        {
            To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleYAnim, card);
        Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

        var sb = new Storyboard();
        sb.Children.Add(translateAnim);
        sb.Children.Add(scaleXAnim);
        sb.Children.Add(scaleYAnim);
        sb.Begin();
    }

    // ── Silence on Leave ─────────────────────────────────────────────

    private void EnsureSilenceStoryboards()
    {
        if (_silenceEnterSb != null) return;

        // Enter: snap to 1.0 (150ms)
        var enterAnim = new DoubleAnimation
        {
            To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(150)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        Storyboard.SetTarget(enterAnim, ChartContentLayer);
        Storyboard.SetTargetProperty(enterAnim, "Opacity");

        _silenceEnterSb = new Storyboard();
        _silenceEnterSb.Children.Add(enterAnim);

        // Exit: hold → dip → recover (200ms delay, 200ms dip, 400ms recovery)
        var exitAnim = new DoubleAnimationUsingKeyFrames();
        exitAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        exitAnim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(200)) });
        exitAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 0.92,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
        exitAnim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(800)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        });
        Storyboard.SetTarget(exitAnim, ChartContentLayer);
        Storyboard.SetTargetProperty(exitAnim, "Opacity");

        _silenceExitSb = new Storyboard();
        _silenceExitSb.Children.Add(exitAnim);
    }

    // ── Scroll Anticipation ─────────────────────────────────────────

    private void OnWatchlistScrollChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        if (sender is not ScrollViewer sv) return;
        bool atTop = sv.VerticalOffset < 4;
        bool atBottom = sv.ScrollableHeight < 1 || sv.VerticalOffset + sv.ViewportHeight >= sv.ScrollableHeight - 4;

        WatchlistTopMask.Opacity = atTop ? 0 : 1;
        WatchlistBottomMask.Opacity = atBottom ? 0 : 1;
    }

    // ── Empty State Gravity ──────────────────────────────────────────

    private bool _wasEmpty;

    private void CheckWatchlistEmpty()
    {
        var hasSearch = !string.IsNullOrEmpty(SearchBox.Text);
        var childCount = VisualTreeHelper.GetChildrenCount(WatchlistScrollViewer);
        // ItemsRepeater inside ScrollViewer — check if repeater has materialized items
        bool isEmpty = hasSearch && GetRepeaterItemCount() == 0;

        if (isEmpty == _wasEmpty) return;
        _wasEmpty = isEmpty;

        WatchlistEmptyState.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        WatchlistCard.BorderBrush = isEmpty
            ? (SolidColorBrush)Application.Current.Resources["MeridianTextMutedBrush"]
            : DefaultBorderBrush;
    }

    private int GetRepeaterItemCount()
    {
        // Walk into the ScrollViewer to find the ItemsRepeater and count children
        var count = VisualTreeHelper.GetChildrenCount(WatchlistScrollViewer);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(WatchlistScrollViewer, i);
            if (child is ItemsRepeater repeater)
                return VisualTreeHelper.GetChildrenCount(repeater);
            // ScrollViewer wraps content in a ScrollContentPresenter
            var innerCount = VisualTreeHelper.GetChildrenCount(child);
            for (int j = 0; j < innerCount; j++)
            {
                var inner = VisualTreeHelper.GetChild(child, j);
                if (inner is ItemsRepeater rep)
                    return VisualTreeHelper.GetChildrenCount(rep);
            }
        }
        return -1; // unknown — don't trigger empty state
    }

    // ── Weight Whisper Helper ────────────────────────────────────────

    private static double ExtractMarketValue(object? dataContext)
    {
        if (dataContext is Holding h) return (double)h.MarketValue;
        try { return (double)((dynamic)dataContext!).MarketValue; }
        catch { return 0; }
    }

    // ── Chart Data Settle ────────────────────────────────────────────

    private void PlayChartSettleAnimation()
    {
        _chartSettleSb?.Stop();

        // Gauge-needle overshoot: fires 800ms after data push (lerp mostly converged)
        var anim = new DoubleAnimationUsingKeyFrames
        {
            BeginTime = TimeSpan.FromMilliseconds(800)
        };
        anim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        anim.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 1.04, KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(100)) });
        // Uno.UI requires KeySpline control-point Y in [0,1] (stricter than WinUI).
        // The original 0.66,-0.56 overshoot is replaced with a BackEase EaseIn, which
        // mathematically produces the same "pull below the target, then settle" curve.
        anim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 0.99,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new BackEase { Amplitude = 0.7, EasingMode = EasingMode.EaseIn }
        });
        anim.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(700)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });

        Storyboard.SetTarget(anim, ChartSettleWrapper);
        Storyboard.SetTargetProperty(anim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

        _chartSettleSb = new Storyboard();
        _chartSettleSb.Children.Add(anim);
        _chartSettleSb.Begin();
    }

    // ── Tooltip Magnetism ────────────────────────────────────────────

    private void OnChartPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(LivelineChart).Position;
        _crosshairTargetX = pos.X;
        _crosshairVisible = true;
    }

    private void OnChartPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _crosshairVisible = false;
    }

    private void UpdateCrosshair()
    {
        if (!_crosshairVisible)
        {
            if (!double.IsNaN(_crosshairCurrentX))
            {
                _crosshairCurrentX = double.NaN;
                LivelineChart.CrosshairX = double.NaN;
            }
            return;
        }

        if (double.IsNaN(_crosshairTargetX)) return;

        if (double.IsNaN(_crosshairCurrentX))
            _crosshairCurrentX = _crosshairTargetX;
        else
            _crosshairCurrentX += (_crosshairTargetX - _crosshairCurrentX) * 0.45;

        LivelineChart.CrosshairX = _crosshairCurrentX;
    }

    // ── Ink Spread (Timeframe Buttons) ───────────────────────────────

    private string _activeTimeframe = "3M";

    private void OnTimeframeClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not string tf) return;
        if (tf == _activeTimeframe) return;

        _activeTimeframe = tf;
        UpdateTimeframeStyles();
        AnimateTimeframeInk(btn);

        // Reload chart with new timeframe filter
        _ = LoadChartAsync(_currentChartTicker);
    }

    private void UpdateTimeframeStyles()
    {
        var gainBg = (SolidColorBrush)Application.Current.Resources["MeridianGainBgTintBrush"];
        var gainFg = (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"];
        var subtleFg = (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"];

        foreach (var child in TimeframeSelector.Children)
        {
            if (child is Button b && b.Tag is string tf)
            {
                bool active = tf == _activeTimeframe;
                b.Background = active ? gainBg : TransparentBg;
                b.Foreground = active ? gainFg : subtleFg;
                b.BorderThickness = active ? new Thickness(0, 0, 0, 2) : new Thickness(0);
                b.BorderBrush = active ? gainFg : null;
            }
        }
    }

    private void AnimateTimeframeInk(Button btn)
    {
        btn.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
        btn.RenderTransform = new CompositeTransform();

        // Press-down then overshoot-spring — visible at small button scale
        var animX = new DoubleAnimationUsingKeyFrames();
        animX.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 0.75, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        animX.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 5 }
        });
        Storyboard.SetTarget(animX, btn);
        Storyboard.SetTargetProperty(animX, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

        var animY = new DoubleAnimationUsingKeyFrames();
        animY.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 0.75, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        animY.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(400)),
            EasingFunction = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 1, Springiness = 5 }
        });
        Storyboard.SetTarget(animY, btn);
        Storyboard.SetTargetProperty(animY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

        // Brief opacity flash to sell the "ink" effect
        var flash = new DoubleAnimationUsingKeyFrames();
        flash.KeyFrames.Add(new LinearDoubleKeyFrame
            { Value = 0.5, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
        flash.KeyFrames.Add(new EasingDoubleKeyFrame
        {
            Value = 1.0,
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        });
        Storyboard.SetTarget(flash, btn);
        Storyboard.SetTargetProperty(flash, "Opacity");

        var sb = new Storyboard();
        sb.Children.Add(animX);
        sb.Children.Add(animY);
        sb.Children.Add(flash);
        sb.Begin();
    }

    // ── Search Focus Ring ────────────────────────────────────────────

    private void OnSearchFocused(object sender, RoutedEventArgs e)
    {
        // WinUI TextBox overrides BorderBrush in focused VisualState,
        // so we animate the wrapper Border instead
        SearchFocusRing.BorderBrush = (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
        SearchFocusRing.BorderThickness = new Thickness(2);
    }

    private void OnSearchUnfocused(object sender, RoutedEventArgs e)
    {
        SearchFocusRing.BorderBrush = DefaultBorderBrush;
        SearchFocusRing.BorderThickness = new Thickness(1);
    }

    // ── Timeframe Filter ─────────────────────────────────────────────

    private static IImmutableList<ChartPoint> FilterByTimeframe(IImmutableList<ChartPoint> points, string timeframe)
        => ChartHelper.FilterByTimeframe(points, timeframe);

    // ── Keyboard Shortcuts ───────────────────────────────────────────

    private void OnPageKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        var ctrl = Microsoft.UI.Input.InputKeyboardSource
            .GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control)
            .HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

        // Ctrl+K → focus search
        if (ctrl && e.Key == Windows.System.VirtualKey.K)
        {
            SearchBox.Focus(FocusState.Programmatic);
            e.Handled = true;
            return;
        }

        // Escape → contextual dismiss (drawer → search → holding selection)
        if (e.Key == Windows.System.VirtualKey.Escape)
        {
            if (TradeDrawerPanel.Visibility == Visibility.Visible)
            {
                CloseTradeDrawer();
            }
            else if (!string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchBox.Text = "";
                Focus(FocusState.Programmatic); // unfocus search box
            }
            else if (_currentChartTicker != null)
            {
                OnBackButtonClick(this, new RoutedEventArgs());
            }
            e.Handled = true;
        }
    }
}
