using Liveline;
using Liveline.Models;
using Meridian.Models;
using Meridian.Presentation;
using Meridian.Services;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Text;

namespace Meridian.Views;

public sealed partial class StockDetailPage : Page
{
	private IMarketDataService _marketData = null!;
	private Stock _stock = null!;
	private string _currentTimeframe = "3M";
	private string _currentChartType = "Area";
	private string _currentPeriod = "Annual";
	private bool _aboutExpanded;

	// ── Consolidated Animation Timer ──
	private DispatcherTimer? _animationTimer;
	private int _animationFrame;

	// Magnetic tooltip — crosshair tracking
	private double _crosshairTargetX = double.NaN;
	private double _crosshairCurrentX = double.NaN;
	private bool _crosshairVisible;

	// Braille liveness
	private static readonly string[] SpinnerGlyphs = ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];
	private static readonly string[] BrailleActivityGlyphs = ["⠀", "⣀", "⣤", "⣴", "⣶", "⣷", "⣿"];

	// Price Spring Count (#1)
	private double _priceTarget;
	private double _priceCurrent;
	private double _priceVelocity;
	private bool _priceAnimating;

	// Position Value Springs (#5)
	private readonly double[] _positionTargets = new double[3];
	private readonly double[] _positionCurrents = new double[3];
	private readonly double[] _positionVelocities = new double[3];
	private bool _positionAnimating;
	private int _positionAnimStartFrame;

	// Analyst Count Springs (#4)
	private readonly double[] _analystTargets = new double[3];
	private readonly double[] _analystCurrents = new double[3];
	private readonly double[] _analystVelocities = new double[3];
	private bool _analystAnimating;
	private int _analystAnimStartFrame;

	// News recency dot (#11)
	private Border? _newsRecencyDot;

	private bool _initialized;

	public StockDetailPage()
	{
		this.InitializeComponent();
		// Uno.Extensions Navigation can assign DataContext asynchronously, *after*
		// the Loaded event fires (observed in this project: DataContext is null at
		// Loaded). Hook both Loaded and DataContextChanged and run init from
		// whichever fires *after* DataContext has a non-null Ticker.
		Loaded += (_, _) => TryInitialize();
		DataContextChanged += (_, _) => TryInitialize();
	}

	private void TryInitialize()
	{
		if (_initialized) return;

		var ticker = DataContext?.GetType().GetProperty("Ticker")?.GetValue(DataContext) as string;
		if (string.IsNullOrEmpty(ticker))
			return; // DataContext not ready yet — wait for the next event

		_initialized = true;
		InitializePage(ticker);
	}

	private async void InitializePage(string ticker)
	{
		try
		{
			_marketData = App.Services.GetRequiredService<IMarketDataService>();

			// Fetch full Stock record for code-behind UI population
			_stock = await _marketData.GetStockAsync(ticker, CancellationToken.None);

			// Populate static UI (header bound via XAML {Binding})
			PopulatePriceHero();
			PopulateOhlcStrip();
			UpdateTimeframeButtons();
			UpdateChartTypeButtons();

			// Load async data
			await Task.WhenAll(
				LoadChartAsync(),
				LoadStatsAsync(),
				LoadAnalystAsync(),
				LoadAboutAsync(),
				LoadPositionAsync(),
				LoadFinancialsAsync(),
				LoadNewsAsync(),
				LoadSimilarAsync()
			);

			// Animate entrance
			AnimatePageEntrance();

			// Start consolidated animation timer
			_animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
			_animationTimer.Tick += OnAnimationTick;
			_animationTimer.Start();
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"[StockDetailPage.InitializePage ticker={ticker}] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
		}
	}

	protected override void OnNavigatedFrom(NavigationEventArgs e)
	{
		base.OnNavigatedFrom(e);
		_animationTimer?.Stop();
		_animationTimer = null;
	}

	// ═══════════════════════════════════════════════════
	// PRICE HERO
	// ═══════════════════════════════════════════════════

	private void PopulatePriceHero()
	{
		// Initialize price spring count (#1)
		_priceTarget = (double)_stock.Price;
		_priceCurrent = _priceTarget * 0.97;
		_priceVelocity = 0;
		_priceAnimating = true;
		HeroPrice.Text = $"${_priceCurrent:N2}";

		var isPositive = _stock.Pct >= 0;
		var sign = isPositive ? "+" : "";

		// Polygon arrow: flip for loss
		GainArrowViewbox.RenderTransform = isPositive
			? null
			: new ScaleTransform { ScaleY = -1 };

		var gainBrush = isPositive
			? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
			: (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];
		var gainBgBrush = isPositive
			? (SolidColorBrush)Application.Current.Resources["MeridianGainBgTintBrush"]
			: (SolidColorBrush)Application.Current.Resources["MeridianLossBgTintBrush"];

		GainArrowPolygon.Fill = gainBrush;
		GainValue.Text = $"{sign}${Math.Abs(_stock.Change):N2}";
		GainValue.Foreground = gainBrush;
		GainPct.Text = $"({sign}{_stock.Pct:N2}%)";
		GainPct.Foreground = gainBrush;
		GainPill.Background = gainBgBrush;
	}

	private void PopulateOhlcStrip()
	{
		OhlcStrip.Children.Clear();
		var stats = new (string Label, string Value)[]
		{
			("Open", $"${_stock.Open:N2}"),
			("High", $"${_stock.High:N2}"),
			("Low", $"${_stock.Low:N2}"),
			("Vol", _stock.Volume),
			("Mkt Cap", "—"), // Placeholder — mock data doesn't have market cap on Stock
		};

		for (var i = 0; i < stats.Length; i++)
		{
			if (i > 0)
			{
				OhlcStrip.Children.Add(new TextBlock
				{
					Text = " · ",
					FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
					FontSize = 13,
					Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"],
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(10, 0, 10, 0),
				});
			}

			var sp = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
			sp.Children.Add(new TextBlock
			{
				Text = stats[i].Label,
				FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
				FontSize = 13,
				FontWeight = new FontWeight(500),
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextMutedBrush"],
				VerticalAlignment = VerticalAlignment.Center,
			});
			sp.Children.Add(new TextBlock
			{
				Text = stats[i].Value,
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 13,
				FontWeight = new FontWeight(500),
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"],
				VerticalAlignment = VerticalAlignment.Center,
			});
			OhlcStrip.Children.Add(sp);
		}

		// OHLC cascade (#9): stagger fade-in left-to-right
		var ohlcIndex = 0;
		foreach (var child in OhlcStrip.Children)
		{
			if (child is StackPanel ohlcSp)
			{
				AnimateEntrance(ohlcSp, 300, 400 + ohlcIndex * 60, translateY: 6);
				ohlcIndex++;
			}
		}
	}

	// ═══════════════════════════════════════════════════
	// CHART
	// ═══════════════════════════════════════════════════

	private async Task LoadChartAsync()
	{
		var points = await _marketData.GetStockHistoryAsync(_stock.Ticker, _currentTimeframe, CancellationToken.None);
		ApplyChartData(points);
	}

	private void ApplyChartData(IImmutableList<ChartPoint> points)
	{
		if (points.Count == 0) return;

		var isPositive = _stock.Pct >= 0;
		StockChart.Theme = new LivelineTheme
		{
			Color = isPositive ? "#2D6A4F" : "#B5342B",
			IsDark = false,
		};
		StockChart.ShowFill = _currentChartType == "Area";

		var data = points.Select(p =>
		{
			var dt = DateTime.TryParse(p.Date, out var parsed)
				? new DateTimeOffset(parsed) : DateTimeOffset.Now;
			return new LivelinePoint(dt, (double)p.Value);
		}).ToList();
		StockChart.Data = data;
		StockChart.Value = (double)points[^1].Value;

		// Volume sub-chart: use the values as a simple proxy
		var values = points.Select(p => p.Value).ToList();
		VolumeSubChart.Values = values;
	}

	private void OnTimeframeClick(object sender, RoutedEventArgs e)
	{
		if (sender is Button btn && btn.Tag is string tf)
		{
			_currentTimeframe = tf;
			UpdateTimeframeButtons();
			_ = LoadChartAsync();
		}
	}

	private void UpdateTimeframeButtons()
	{
		foreach (var child in TimeframeButtons.Children)
		{
			if (child is Button btn)
			{
				var isActive = (string)btn.Tag == _currentTimeframe;
				btn.Foreground = isActive
					? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
					: (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"];
				btn.FontWeight = isActive
					? new FontWeight(600)
					: new FontWeight(400);
			}
		}
	}

	private void OnChartTypeClick(object sender, RoutedEventArgs e)
	{
		if (sender is Button btn && btn.Tag is string type)
		{
			_currentChartType = type;
			StockChart.ShowFill = type == "Area";
			UpdateChartTypeButtons();
		}
	}

	private void UpdateChartTypeButtons()
	{
		foreach (var child in ChartTypeButtons.Children)
		{
			if (child is Button btn)
			{
				var isActive = (string)btn.Tag == _currentChartType;
				btn.Background = isActive
					? (SolidColorBrush)Application.Current.Resources["MeridianCardBrush"]
					: new SolidColorBrush(Microsoft.UI.Colors.Transparent);
				btn.Foreground = isActive
					? (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"]
					: (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"];
			}
		}
	}

	// ── Tooltip Magnetism ──────────────────────────────

	private void OnStockChartPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		var pos = e.GetCurrentPoint(StockChart).Position;
		_crosshairTargetX = pos.X;
		_crosshairVisible = true;
	}

	private void OnStockChartPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		_crosshairVisible = false;
	}

	// ── Consolidated Animation Tick ────────────────────

	private void OnAnimationTick(object? sender, object e)
	{
		_animationFrame = (_animationFrame + 1) % 100_000;

		// Crosshair tracking (every frame)
		UpdateCrosshair();

		// 30fps cadence (every 2nd frame): springs, gain pill glow, news dot
		if (_animationFrame % 2 == 0)
		{
			UpdatePriceSpring();
			UpdatePositionSprings();
			UpdateAnalystSprings();
			UpdateGainPillGlow();
			UpdateNewsRecencyDot();
		}

		// Braille spinner (~12fps, every 5 frames)
		if (_animationFrame % 5 == 0)
			UpdateBrailleSpinner();

		// Braille activity (~5fps, every 13 frames)
		if (_animationFrame % 13 == 0)
			UpdateBrailleActivity();
	}

	private void UpdateCrosshair()
	{
		if (!_crosshairVisible)
		{
			if (!double.IsNaN(_crosshairCurrentX))
			{
				_crosshairCurrentX = double.NaN;
				StockChart.CrosshairX = double.NaN;
			}
			return;
		}

		if (double.IsNaN(_crosshairTargetX)) return;

		if (double.IsNaN(_crosshairCurrentX))
			_crosshairCurrentX = _crosshairTargetX;
		else
			_crosshairCurrentX += (_crosshairTargetX - _crosshairCurrentX) * 0.35;

		StockChart.CrosshairX = _crosshairCurrentX;
	}

	private void UpdatePriceSpring()
	{
		if (!_priceAnimating) return;
		_priceVelocity += (_priceTarget - _priceCurrent) * 0.08;
		_priceVelocity *= 0.82;
		_priceCurrent += _priceVelocity;
		HeroPrice.Text = $"${_priceCurrent:N2}";
		if (Math.Abs(_priceCurrent - _priceTarget) < 0.005 && Math.Abs(_priceVelocity) < 0.001)
		{
			_priceCurrent = _priceTarget;
			HeroPrice.Text = $"${_priceTarget:N2}";
			_priceAnimating = false;
		}
	}

	private void UpdatePositionSprings()
	{
		if (!_positionAnimating) return;
		var elapsed = _animationFrame - _positionAnimStartFrame;
		var allSettled = true;
		for (var i = 0; i < 3; i++)
		{
			if (elapsed < i * 8) { allSettled = false; continue; } // 120ms stagger
			_positionVelocities[i] += (_positionTargets[i] - _positionCurrents[i]) * 0.08;
			_positionVelocities[i] *= 0.82;
			_positionCurrents[i] += _positionVelocities[i];
			if (Math.Abs(_positionCurrents[i] - _positionTargets[i]) > 0.5 ||
				Math.Abs(_positionVelocities[i]) > 0.1) allSettled = false;
		}
		// Update text
		var sign0 = _positionCurrents[0] >= 0 ? "" : "-";
		MiniMarketValue.Text = $"${Math.Abs(_positionCurrents[0]):N0}";
		var sign1 = _positionCurrents[1] >= 0 ? "+" : "";
		MiniUnrealized.Text = $"{sign1}${_positionCurrents[1]:N2}";
		var sign2 = _positionCurrents[2] >= 0 ? "+" : "";
		MiniReturn.Text = $"{sign2}{_positionCurrents[2]:N1}%";
		if (allSettled) _positionAnimating = false;
	}

	private void UpdateAnalystSprings()
	{
		if (!_analystAnimating) return;
		var elapsed = _animationFrame - _analystAnimStartFrame;
		var allSettled = true;
		for (var i = 0; i < 3; i++)
		{
			if (elapsed < i * 6) { allSettled = false; continue; } // 100ms stagger
			_analystVelocities[i] += (_analystTargets[i] - _analystCurrents[i]) * 0.08;
			_analystVelocities[i] *= 0.82;
			_analystCurrents[i] += _analystVelocities[i];
			if (Math.Abs(_analystCurrents[i] - _analystTargets[i]) > 0.3 ||
				Math.Abs(_analystVelocities[i]) > 0.05) allSettled = false;
		}
		BuyCount.Text = $"{Math.Max(0, (int)Math.Round(_analystCurrents[0]))}";
		HoldCount.Text = $"{Math.Max(0, (int)Math.Round(_analystCurrents[1]))}";
		SellCount.Text = $"{Math.Max(0, (int)Math.Round(_analystCurrents[2]))}";
		if (allSettled) _analystAnimating = false;
	}

	private void UpdateGainPillGlow()
	{
		var phase = (_animationFrame % 188) / 188.0 * Math.PI * 2;
		var sinVal = Math.Sin(phase);
		GainPill.Opacity = 0.72 + 0.28 * sinVal;
		var scale = 1.0 + 0.02 * sinVal;
		if (GainPill.RenderTransform is ScaleTransform st)
		{
			st.ScaleX = scale;
			st.ScaleY = scale;
		}
	}

	private void UpdateBrailleSpinner()
	{
		if (FindName("DetailBrailleSpinner") is TextBlock spinner)
			spinner.Text = SpinnerGlyphs[(_animationFrame / 5) % SpinnerGlyphs.Length];
	}

	private void UpdateBrailleActivity()
	{
		if (FindName("DetailBrailleActivity") is not TextBlock tb) return;
		var intensity = Math.Min(1.0, Math.Abs((double)_stock.Pct) / 2.0 + 0.15);
		var sb = new System.Text.StringBuilder(4);
		for (var i = 0; i < 4; i++)
		{
			var v = (Math.Sin(_animationFrame * 0.3 * intensity + i * 0.9) * 0.5 + 0.5) * intensity;
			var level = (int)Math.Round(v * (BrailleActivityGlyphs.Length - 1));
			sb.Append(BrailleActivityGlyphs[Math.Clamp(level, 0, BrailleActivityGlyphs.Length - 1)]);
		}
		tb.Text = sb.ToString();
		tb.Opacity = 0.25 + intensity * 0.65;
	}

	private void UpdateNewsRecencyDot()
	{
		if (_newsRecencyDot is null) return;
		var dotPhase = (_animationFrame % 156) / 156.0 * Math.PI * 2;
		var sinVal = Math.Sin(dotPhase);
		_newsRecencyDot.Opacity = 0.4 + 0.6 * (0.5 + 0.5 * sinVal);
		var scale = 0.8 + 0.3 * (0.5 + 0.5 * sinVal);
		if (_newsRecencyDot.RenderTransform is CompositeTransform ct)
		{
			ct.ScaleX = scale;
			ct.ScaleY = scale;
		}
	}

	// ── Stagger Animation Helper ───────────────────────

	private static void AnimateEntrance(FrameworkElement element, int durationMs, int delayMs,
		double translateX = 0, double translateY = 0)
	{
		element.Opacity = 0;
		element.RenderTransform = new CompositeTransform { TranslateX = translateX, TranslateY = translateY };

		var sb = new Storyboard();

		var fadeIn = new DoubleAnimation
		{
			From = 0, To = 1,
			Duration = new Duration(TimeSpan.FromMilliseconds(durationMs)),
			BeginTime = TimeSpan.FromMilliseconds(delayMs),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(fadeIn, element);
		Storyboard.SetTargetProperty(fadeIn, "Opacity");
		sb.Children.Add(fadeIn);

		if (translateX != 0)
		{
			var slideX = new DoubleAnimation
			{
				From = translateX, To = 0,
				Duration = new Duration(TimeSpan.FromMilliseconds(durationMs)),
				BeginTime = TimeSpan.FromMilliseconds(delayMs),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(slideX, element);
			Storyboard.SetTargetProperty(slideX, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
			sb.Children.Add(slideX);
		}

		if (translateY != 0)
		{
			var slideY = new DoubleAnimation
			{
				From = translateY, To = 0,
				Duration = new Duration(TimeSpan.FromMilliseconds(durationMs)),
				BeginTime = TimeSpan.FromMilliseconds(delayMs),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(slideY, element);
			Storyboard.SetTargetProperty(slideY, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");
			sb.Children.Add(slideY);
		}

		sb.Begin();
	}

	// ═══════════════════════════════════════════════════
	// KEY STATISTICS
	// ═══════════════════════════════════════════════════

	private async Task LoadStatsAsync()
	{
		var stats = await _marketData.GetKeyStatsAsync(_stock.Ticker, CancellationToken.None);
		StatsRows.Children.Clear();

		for (var i = 0; i < stats.Count; i++)
		{
			var stat = stats[i];
			var row = new Grid
			{
				Padding = new Thickness(14, 9, 14, 9),
				CornerRadius = new CornerRadius(6),
				Background = i % 2 == 1
					? (SolidColorBrush)Application.Current.Resources["MeridianSurfaceHoverBrush"]
					: new SolidColorBrush(Microsoft.UI.Colors.Transparent),
			};
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			var label = new TextBlock
			{
				Text = stat.Label,
				FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
				FontSize = 12,
				FontWeight = new FontWeight(500),
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextMutedBrush"],
				VerticalAlignment = VerticalAlignment.Center,
			};
			Grid.SetColumn(label, 0);

			var valueBrush = stat.IsColored
				? (stat.IsPositive
					? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
					: (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"])
				: (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"];

			var value = new TextBlock
			{
				Text = stat.Value,
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 13,
				FontWeight = new FontWeight(500),
				Foreground = valueBrush,
				VerticalAlignment = VerticalAlignment.Center,
			};
			Grid.SetColumn(value, 1);

			row.Children.Add(label);
			row.Children.Add(value);
			StatsRows.Children.Add(row);
		}

		// Stat row stagger (#7): cascade left-to-right
		for (var j = 0; j < StatsRows.Children.Count; j++)
			AnimateEntrance((FrameworkElement)StatsRows.Children[j], 300, j * 35, translateX: 6);
	}

	// ═══════════════════════════════════════════════════
	// ANALYST CONSENSUS
	// ═══════════════════════════════════════════════════

	private async Task LoadAnalystAsync()
	{
		var data = await _marketData.GetAnalystDataAsync(_stock.Ticker, CancellationToken.None);

		// Rating bar proportions
		BuySegment.Width = new GridLength(data.BuyCount, GridUnitType.Star);
		HoldSegment.Width = new GridLength(data.HoldCount, GridUnitType.Star);
		SellSegment.Width = new GridLength(data.SellCount, GridUnitType.Star);

		// Analyst count springs (#4): spring from 0
		_analystTargets[0] = data.BuyCount;
		_analystTargets[1] = data.HoldCount;
		_analystTargets[2] = data.SellCount;
		Array.Clear(_analystCurrents);
		Array.Clear(_analystVelocities);
		_analystAnimStartFrame = _animationFrame;
		_analystAnimating = true;
		BuyCount.Text = "0";
		HoldCount.Text = "0";
		SellCount.Text = "0";

		TargetLow.Text = $"${data.PriceTargetLow:N0}";
		TargetAvg.Text = $"${data.PriceTargetAvg:N0}";
		TargetHigh.Text = $"${data.PriceTargetHigh:N0}";
		TargetCurrent.Text = $"${_stock.Price:N2}";

		// Position the price target dot
		var range = data.PriceTargetHigh - data.PriceTargetLow;
		if (range > 0)
		{
			var pct = (double)((_stock.Price - data.PriceTargetLow) / range);
			pct = Math.Clamp(pct, 0.05, 0.95); // Keep dot in bounds
			PriceTargetDot.RenderTransform = new TranslateTransform();

			// Price target dot spring (#8): spring from left to position
			PriceTargetDot.Loaded += (s, e) =>
			{
				var parent = PriceTargetDot.Parent as FrameworkElement;
				if (parent is null) return;
				var trackWidth = parent.ActualWidth - PriceTargetDot.Width;
				if (trackWidth <= 0) return;
				var targetX = pct * trackWidth;

				var springAnim = new DoubleAnimation
				{
					From = 0, To = targetX,
					Duration = new Duration(TimeSpan.FromMilliseconds(900)),
					BeginTime = TimeSpan.FromMilliseconds(600),
					EasingFunction = new ElasticEase
					{
						Oscillations = 1, Springiness = 8,
						EasingMode = EasingMode.EaseOut,
					},
				};
				Storyboard.SetTarget(springAnim, PriceTargetDot);
				Storyboard.SetTargetProperty(springAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
				var sb = new Storyboard();
				sb.Children.Add(springAnim);
				sb.Begin();
			};
		}
	}

	// ═══════════════════════════════════════════════════
	// ABOUT CARD
	// ═══════════════════════════════════════════════════

	private async Task LoadAboutAsync()
	{
		var profile = await _marketData.GetStockProfileAsync(_stock.Ticker, CancellationToken.None);

		AboutLabel.Text = $"ABOUT {_stock.Name.ToUpperInvariant()}";
		AboutDescription.Text = profile.Description;
		AboutDescription.MaxLines = 3;

		MetaSector.Text = profile.Sector;
		MetaIndustry.Text = profile.Industry;
		MetaFounded.Text = $"{profile.Founded} · {profile.Headquarters}";
		MetaCeo.Text = profile.CEO;
		MetaEmployees.Text = profile.Employees;
	}

	private void OnReadMoreClick(object sender, RoutedEventArgs e)
	{
		_aboutExpanded = !_aboutExpanded;

		// About smooth expand (#12): fade transition
		var fadeOut = new DoubleAnimation
		{
			To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(150)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
		};
		Storyboard.SetTarget(fadeOut, AboutDescription);
		Storyboard.SetTargetProperty(fadeOut, "Opacity");
		var sb = new Storyboard();
		sb.Children.Add(fadeOut);
		sb.Completed += (_, _) =>
		{
			if (_aboutExpanded)
			{
				AboutDescription.MaxLines = 0;
				AboutDescription.TextTrimming = TextTrimming.None;
				ReadMoreButton.Content = "Show less";
			}
			else
			{
				AboutDescription.MaxLines = 3;
				AboutDescription.TextTrimming = TextTrimming.CharacterEllipsis;
				ReadMoreButton.Content = "Read more";
			}

			var fadeIn = new DoubleAnimation
			{
				From = 0, To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(_aboutExpanded ? 300 : 200)),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(fadeIn, AboutDescription);
			Storyboard.SetTargetProperty(fadeIn, "Opacity");
			var sbIn = new Storyboard();
			sbIn.Children.Add(fadeIn);
			sbIn.Begin();
		};
		sb.Begin();
	}

	// ═══════════════════════════════════════════════════
	// POSITION CARD
	// ═══════════════════════════════════════════════════

	private async Task LoadPositionAsync()
	{
		var position = await _marketData.GetHoldingAsync(_stock.Ticker, CancellationToken.None);

		if (position is null)
		{
			HasPositionPanel.Visibility = Visibility.Collapsed;
			NoPositionPanel.Visibility = Visibility.Visible;
			NoPositionText.Text = $"You don't hold {_stock.Ticker}";
			BuyButton.Content = $"BUY {_stock.Ticker}";
			StockChart.BaselineY = double.NaN; // No cost basis line
			return;
		}

		HasPositionPanel.Visibility = Visibility.Visible;
		NoPositionPanel.Visibility = Visibility.Collapsed;

		PositionShares.Text = position.Shares.ToString();
		PositionAvgCost.Text = $"${position.AvgCost:N2}";

		// Position value springs (#5)
		var gainLoss = position.GainLoss;
		var gainPct = position.GainPct;
		var isPositive = gainLoss >= 0;
		var colorBrush = isPositive
			? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
			: (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];
		MiniUnrealized.Foreground = colorBrush;
		MiniReturn.Foreground = colorBrush;

		_positionTargets[0] = (double)position.MarketValue;
		_positionTargets[1] = (double)gainLoss;
		_positionTargets[2] = (double)gainPct;
		Array.Clear(_positionCurrents);
		Array.Clear(_positionVelocities);
		_positionAnimStartFrame = _animationFrame;
		_positionAnimating = true;
		MiniMarketValue.Text = "$0";
		MiniUnrealized.Text = "$0.00";
		MiniReturn.Text = "0.0%";

		// Average cost baseline (#3): show on chart if holding
		StockChart.BaselineY = (double)position.AvgCost;

		// Weight bar (approximate: position value / total portfolio)
		// Using a hardcoded total for mock data
		var totalPortfolio = 163842.56m;
		var weight = (double)(position.MarketValue / totalPortfolio * 100);
		WeightPctLabel.Text = $"{weight:N1}%";

		// Animate weight bar fill after a delay
		DispatcherQueue.TryEnqueue(async () =>
		{
			await Task.Delay(500);
			var maxWidth = WeightBarFill.Parent is FrameworkElement parent ? parent.ActualWidth : 300;
			var targetWidth = maxWidth * weight / 100.0;
			WeightBarFill.Width = targetWidth;
		});
	}

	// ═══════════════════════════════════════════════════
	// FINANCIALS CARD
	// ═══════════════════════════════════════════════════

	private async Task LoadFinancialsAsync()
	{
		var data = await _marketData.GetFinancialsAsync(_stock.Ticker, _currentPeriod, CancellationToken.None);
		PopulateFinancialsTable(data);
		RevenueBarChart.Financials = data.ToList();
	}

	private void PopulateFinancialsTable(IImmutableList<Financial> data)
	{
		// Remove old data rows (keep header row at index 0)
		while (FinancialsTable.Children.Count > 1)
			FinancialsTable.Children.RemoveAt(FinancialsTable.Children.Count - 1);

		for (var i = 0; i < data.Count; i++)
		{
			var item = data[i];
			var row = new Grid
			{
				Padding = new Thickness(14, 10, 14, 10),
				CornerRadius = new CornerRadius(6),
				Background = i % 2 == 0
					? (SolidColorBrush)Application.Current.Resources["MeridianSurfaceHoverBrush"]
					: new SolidColorBrush(Microsoft.UI.Colors.Transparent),
			};
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			AddFinancialCell(row, 0, item.Period, HorizontalAlignment.Left,
				(FontFamily)Application.Current.Resources["OutfitFont"], 12,
				new FontWeight(600));
			AddFinancialCell(row, 1, item.Revenue, HorizontalAlignment.Right);
			AddFinancialCell(row, 2, item.NetIncome, HorizontalAlignment.Right);
			AddFinancialCell(row, 3, item.EPS, HorizontalAlignment.Right);

			FinancialsTable.Children.Add(row);

			// Financials row stagger (#13)
			AnimateEntrance(row, 350, i * 80, translateY: 8);
		}
	}

	private static void AddFinancialCell(Grid row, int col, string text,
		HorizontalAlignment hAlign,
		FontFamily? font = null, double fontSize = 13,
		FontWeight? weight = null)
	{
		var tb = new TextBlock
		{
			Text = text,
			FontFamily = font ?? (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
			FontSize = fontSize,
			FontWeight = weight ?? new FontWeight(500),
			Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"],
			HorizontalAlignment = hAlign,
			VerticalAlignment = VerticalAlignment.Center,
		};
		Grid.SetColumn(tb, col);
		row.Children.Add(tb);
	}

	private void OnPeriodToggleClick(object sender, RoutedEventArgs e)
	{
		if (sender is Button btn && btn.Tag is string period)
		{
			_currentPeriod = period;
			UpdatePeriodToggle();
			_ = LoadFinancialsAsync();
		}
	}

	private void UpdatePeriodToggle()
	{
		foreach (var child in PeriodToggle.Children)
		{
			if (child is Button btn)
			{
				var isActive = (string)btn.Tag == _currentPeriod;
				btn.Background = isActive
					? (SolidColorBrush)Application.Current.Resources["MeridianCardBrush"]
					: new SolidColorBrush(Microsoft.UI.Colors.Transparent);
				btn.Foreground = isActive
					? (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"]
					: (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"];
			}
		}
	}

	// ═══════════════════════════════════════════════════
	// NEWS
	// ═══════════════════════════════════════════════════

	private async Task LoadNewsAsync()
	{
		var news = await _marketData.GetNewsForTickerAsync(_stock.Ticker, CancellationToken.None);
		NewsRows.Children.Clear();

		var newsIndex = 0;
		foreach (var item in news)
		{
			var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12 };

			// Tag badge
			var tagBorder = new Border
			{
				CornerRadius = new CornerRadius(6),
				Padding = new Thickness(8, 3, 8, 3),
				VerticalAlignment = VerticalAlignment.Top,
			};
			var tagText = new TextBlock
			{
				Text = item.Tag.ToUpperInvariant(),
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 9,
				FontWeight = new FontWeight(600),
				CharacterSpacing = 40,
			};

			// Color based on tag
			switch (item.Tag.ToLowerInvariant())
			{
				case "earnings":
					tagBorder.Background = (SolidColorBrush)Application.Current.Resources["MeridianGainBgTintBrush"];
					tagText.Foreground = (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"];
					break;
				case "tech":
					tagBorder.Background = (SolidColorBrush)Application.Current.Resources["MeridianAccentBgTintBrush"];
					tagText.Foreground = (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
					break;
				default:
					tagBorder.Background = (SolidColorBrush)Application.Current.Resources["MeridianAccentBgTintBrush"];
					tagText.Foreground = (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
					break;
			}
			tagBorder.Child = tagText;

			// Content
			var content = new StackPanel { Spacing = 3 };
			content.Children.Add(new TextBlock
			{
				Text = item.Text,
				FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
				FontSize = 12,
				LineHeight = 17,
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"],
				TextWrapping = TextWrapping.Wrap,
			});
			content.Children.Add(new TextBlock
			{
				Text = item.Time,
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 10,
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"],
			});

			// News recency dot (#11): add breathing dot to first item
			if (newsIndex == 0)
			{
				_newsRecencyDot = new Border
				{
					Width = 6, Height = 6, CornerRadius = new CornerRadius(3),
					Background = (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"],
					VerticalAlignment = VerticalAlignment.Top, Margin = new Thickness(0, 6, 0, 0),
					RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5),
					RenderTransform = new CompositeTransform(),
				};
				row.Children.Insert(0, _newsRecencyDot);
			}

			row.Children.Add(tagBorder);
			row.Children.Add(content);
			NewsRows.Children.Add(row);
			newsIndex++;
		}
	}

	// ═══════════════════════════════════════════════════
	// SIMILAR HOLDINGS
	// ═══════════════════════════════════════════════════

	private async Task LoadSimilarAsync()
	{
		var similar = await _marketData.GetSimilarHoldingsAsync(_stock.Ticker, CancellationToken.None);

		if (similar.Count == 0)
		{
			SimilarCard.Visibility = Visibility.Collapsed;
			return;
		}

		SimilarRows.Children.Clear();
		for (var i = 0; i < similar.Count; i++)
		{
			var item = similar[i];
			var row = new Grid
			{
				Padding = new Thickness(22, 12, 22, 12),
				Tag = item.Ticker,
			};
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(72) }); // Sparkline
			row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			// Left: Ticker + Name
			var left = new StackPanel { Spacing = 2 };
			left.Children.Add(new TextBlock
			{
				Text = item.Ticker,
				FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
				FontSize = 13,
				FontWeight = new FontWeight(600),
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"],
			});
			left.Children.Add(new TextBlock
			{
				Text = item.Name,
				FontFamily = (FontFamily)Application.Current.Resources["OutfitFont"],
				FontSize = 11,
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextSubtleBrush"],
			});
			Grid.SetColumn(left, 0);

			// Right: Price + Delta
			var right = new StackPanel { Spacing = 2, HorizontalAlignment = HorizontalAlignment.Right };
			right.Children.Add(new TextBlock
			{
				Text = $"${item.Price:N2}",
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 13,
				FontWeight = new FontWeight(500),
				Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextPrimaryBrush"],
				HorizontalAlignment = HorizontalAlignment.Right,
			});
			var deltaSign = item.Pct >= 0 ? "+" : "";
			var deltaBrush = item.Pct >= 0
				? (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"]
				: (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];
			right.Children.Add(new TextBlock
			{
				Text = $"{deltaSign}{item.Pct:N2}%",
				FontFamily = (FontFamily)Application.Current.Resources["IBMPlexMonoFont"],
				FontSize = 11,
				FontWeight = new FontWeight(500),
				Foreground = deltaBrush,
				HorizontalAlignment = HorizontalAlignment.Right,
			});
			Grid.SetColumn(right, 2);

			// Middle: Sparkline (#6)
			var sparkline = new Controls.SparklineControl
			{
				Width = 60, Height = 24,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				IsPositive = item.Pct >= 0,
				Opacity = 0,
			};
			Grid.SetColumn(sparkline, 1);

			row.Children.Add(left);
			row.Children.Add(sparkline);
			row.Children.Add(right);

			// Bottom border (not on last row)
			if (i < similar.Count - 1)
			{
				row.BorderBrush = (SolidColorBrush)Application.Current.Resources["MeridianRowSeparatorBrush"];
				row.BorderThickness = new Thickness(0, 0, 0, 1);
			}

			// Click handler for navigation
			row.Tapped += OnSimilarRowTapped;
			row.PointerEntered += OnSimilarRowPointerEntered;
			row.PointerExited += OnSimilarRowPointerExited;

			SimilarRows.Children.Add(row);
		}

		// Load sparkline data in parallel (#6)
		_ = LoadSimilarSparklinesAsync(similar);
	}

	private async Task LoadSimilarSparklinesAsync(IImmutableList<SimilarStock> similar)
	{
		// Fetch all histories in parallel
		var tasks = similar.Select(s => _marketData.GetStockHistoryAsync(s.Ticker, "3M", CancellationToken.None).AsTask()).ToArray();
		var histories = await Task.WhenAll(tasks);

		for (var i = 0; i < similar.Count && i < SimilarRows.Children.Count; i++)
		{
			if (SimilarRows.Children[i] is not Grid row) continue;
			var sparkline = row.Children.OfType<Controls.SparklineControl>().FirstOrDefault();
			if (sparkline is null || histories[i].Count == 0) continue;

			var points = histories[i].TakeLast(24).Select(p => (double)p.Value).ToList();
			sparkline.Points = points;
			sparkline.IsPositive = points[^1] >= points[0];
			AnimateEntrance(sparkline, 300, i * 100);
		}
	}

	private async void OnSimilarRowTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
	{
		if (sender is Grid row && row.Tag is string ticker)
		{
			// Region nav to the same route with new data — framework re-resolves
			// StockDetailModel with the new ticker via the DataViewMap.
			var nav = this.Navigator();
			if (nav is not null)
				await nav.NavigateRouteAsync(this, "StockDetail", data: ticker);
		}
	}

	private void OnSimilarRowPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		if (sender is Grid row)
		{
			row.Background = (SolidColorBrush)Application.Current.Resources["MeridianSurfaceHoverBrush"];
		}
	}

	private void OnSimilarRowPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		if (sender is Grid row)
		{
			row.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
		}
	}

	// ═══════════════════════════════════════════════════
	// NAVIGATION
	// ═══════════════════════════════════════════════════

	private async void OnBackButtonClick(object sender, RoutedEventArgs e)
	{
		var nav = this.Navigator();
		if (nav is not null)
			await nav.NavigateBackAsync(this);
	}

	// Back button trail (#14)
	private void OnBackButtonPointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		var sb = new Storyboard();
		var arrowSlide = new DoubleAnimation
		{
			To = -3, Duration = new Duration(TimeSpan.FromMilliseconds(200)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(arrowSlide, BackArrowTranslate);
		Storyboard.SetTargetProperty(arrowSlide, "X");
		var trailExtend = new DoubleAnimation
		{
			To = 40, Duration = new Duration(TimeSpan.FromMilliseconds(250)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(trailExtend, BackTrailLine);
		Storyboard.SetTargetProperty(trailExtend, "Width");
		sb.Children.Add(arrowSlide);
		sb.Children.Add(trailExtend);
		sb.Begin();
	}

	private void OnBackButtonPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
	{
		var sb = new Storyboard();
		var arrowReturn = new DoubleAnimation
		{
			To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(arrowReturn, BackArrowTranslate);
		Storyboard.SetTargetProperty(arrowReturn, "X");
		var trailRetract = new DoubleAnimation
		{
			To = 0, Duration = new Duration(TimeSpan.FromMilliseconds(200)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(trailRetract, BackTrailLine);
		Storyboard.SetTargetProperty(trailRetract, "Width");
		sb.Children.Add(arrowReturn);
		sb.Children.Add(trailRetract);
		sb.Begin();
	}

	// ═══════════════════════════════════════════════════
	// TRADE
	// ═══════════════════════════════════════════════════

	private TranslateTransform? _drawerTransform;
	private Storyboard? _drawerOpenSb;
	private Storyboard? _drawerCloseSb;
	private bool _isDrawerAnimating;

	private void OnTradeButtonClick(object sender, RoutedEventArgs e)
	{
		OpenTradeDrawer(_stock);
	}

	private async void OnSellAllClick(object sender, RoutedEventArgs e)
	{
		var position = await _marketData.GetHoldingAsync(_stock.Ticker, CancellationToken.None);
		OpenTradeDrawer(_stock, isSell: true, prefillQty: position?.Shares ?? 0);
	}

	private void OpenTradeDrawer(Stock stock, bool isSell = false, int prefillQty = 0)
	{
		if (_isDrawerAnimating) return;

		TradeDrawerPanel.SetStock(stock, isSell, prefillQty);
		EnsureDrawerStoryboards();

		DrawerBackdrop.Opacity = 0;
		DrawerBackdrop.Visibility = Visibility.Visible;
		TradeDrawerPanel.Visibility = Visibility.Visible;
		_drawerTransform!.X = 420;

		_isDrawerAnimating = true;
		_drawerOpenSb!.Stop();
		_drawerOpenSb.Begin();
	}

	private void CloseTradeDrawer()
	{
		if (_isDrawerAnimating) return;
		_isDrawerAnimating = true;
		_drawerCloseSb?.Stop();
		_drawerCloseSb?.Begin();
	}

	private void OnDrawerBackdropTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
	{
		CloseTradeDrawer();
	}

	private void EnsureDrawerStoryboards()
	{
		if (_drawerOpenSb != null) return;

		_drawerTransform = new TranslateTransform();
		TradeDrawerPanel.RenderTransform = _drawerTransform;

		// Open animation
		_drawerOpenSb = new Storyboard();
		var openSlide = new DoubleAnimation
		{
			From = 420, To = 0,
			Duration = new Duration(TimeSpan.FromMilliseconds(350)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		};
		Storyboard.SetTarget(openSlide, _drawerTransform);
		Storyboard.SetTargetProperty(openSlide, "X");
		var openBackdrop = new DoubleAnimation
		{
			From = 0, To = 1,
			Duration = new Duration(TimeSpan.FromMilliseconds(250)),
		};
		Storyboard.SetTarget(openBackdrop, DrawerBackdrop);
		Storyboard.SetTargetProperty(openBackdrop, "Opacity");
		_drawerOpenSb.Children.Add(openSlide);
		_drawerOpenSb.Children.Add(openBackdrop);
		_drawerOpenSb.Completed += (_, _) =>
		{
			_isDrawerAnimating = false;
			TradeDrawerPanel.FocusQuantityInput();
		};

		// Close animation
		_drawerCloseSb = new Storyboard();
		var closeSlide = new DoubleAnimation
		{
			From = 0, To = 420,
			Duration = new Duration(TimeSpan.FromMilliseconds(300)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn },
		};
		Storyboard.SetTarget(closeSlide, _drawerTransform);
		Storyboard.SetTargetProperty(closeSlide, "X");
		var closeBackdrop = new DoubleAnimation
		{
			From = 1, To = 0,
			Duration = new Duration(TimeSpan.FromMilliseconds(200)),
		};
		Storyboard.SetTarget(closeBackdrop, DrawerBackdrop);
		Storyboard.SetTargetProperty(closeBackdrop, "Opacity");
		_drawerCloseSb.Children.Add(closeSlide);
		_drawerCloseSb.Children.Add(closeBackdrop);
		_drawerCloseSb.Completed += (_, _) =>
		{
			DrawerBackdrop.Visibility = Visibility.Collapsed;
			TradeDrawerPanel.Visibility = Visibility.Collapsed;
			_isDrawerAnimating = false;
		};

		TradeDrawerPanel.CloseRequested += (_, _) => CloseTradeDrawer();
	}

	// ═══════════════════════════════════════════════════
	// WATCHLIST
	// ═══════════════════════════════════════════════════

	private bool _isWatching;

	private void OnWatchlistToggleClick(object sender, RoutedEventArgs e)
	{
		_isWatching = !_isWatching;
		UpdateWatchlistButton();

		// Watchlist star bounce (#10)
		WatchlistButton.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
		WatchlistButton.RenderTransform ??= new CompositeTransform();
		var scaleTo = _isWatching ? 1.15 : 0.95;
		var dur = _isWatching ? 350 : 200;
		var scaleAnim = new DoubleAnimationUsingKeyFrames();
		scaleAnim.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
		scaleAnim.KeyFrames.Add(new EasingDoubleKeyFrame
		{
			Value = scaleTo,
			KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(dur / 3)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		});
		scaleAnim.KeyFrames.Add(new EasingDoubleKeyFrame
		{
			Value = 1.0,
			KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(dur)),
			EasingFunction = _isWatching
				? new ElasticEase { Oscillations = 1, Springiness = 5, EasingMode = EasingMode.EaseOut }
				: new CubicEase { EasingMode = EasingMode.EaseOut },
		});
		Storyboard.SetTarget(scaleAnim, WatchlistButton);
		Storyboard.SetTargetProperty(scaleAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
		var scaleAnimY = new DoubleAnimationUsingKeyFrames();
		scaleAnimY.KeyFrames.Add(new LinearDoubleKeyFrame { Value = 1.0, KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero) });
		scaleAnimY.KeyFrames.Add(new EasingDoubleKeyFrame
		{
			Value = scaleTo,
			KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(dur / 3)),
			EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
		});
		scaleAnimY.KeyFrames.Add(new EasingDoubleKeyFrame
		{
			Value = 1.0,
			KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(dur)),
			EasingFunction = _isWatching
				? new ElasticEase { Oscillations = 1, Springiness = 5, EasingMode = EasingMode.EaseOut }
				: new CubicEase { EasingMode = EasingMode.EaseOut },
		});
		Storyboard.SetTarget(scaleAnimY, WatchlistButton);
		Storyboard.SetTargetProperty(scaleAnimY, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
		var sb = new Storyboard();
		sb.Children.Add(scaleAnim);
		sb.Children.Add(scaleAnimY);
		sb.Begin();
	}

	private void UpdateWatchlistButton()
	{
		if (_isWatching)
		{
			WatchlistIcon.Text = "\u2605"; // Filled star
			WatchlistLabel.Text = "Watching";
			WatchlistButton.BorderBrush = (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
			WatchlistButton.Foreground = (SolidColorBrush)Application.Current.Resources["MeridianAccentBrush"];
			WatchlistButton.Background = (SolidColorBrush)Application.Current.Resources["MeridianAccentBgTintBrush"];
		}
		else
		{
			WatchlistIcon.Text = "\u2606"; // Empty star
			WatchlistLabel.Text = "Watchlist";
			WatchlistButton.BorderBrush = (SolidColorBrush)Application.Current.Resources["MeridianBorderBrush"];
			WatchlistButton.Foreground = (SolidColorBrush)Application.Current.Resources["MeridianTextMutedBrush"];
			WatchlistButton.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
		}
	}

	// ═══════════════════════════════════════════════════
	// EXTERNAL LINKS
	// ═══════════════════════════════════════════════════

	private async void OnWebsiteLinkClick(object sender, RoutedEventArgs e)
	{
		var profile = await _marketData.GetStockProfileAsync(_stock.Ticker, CancellationToken.None);
		if (!string.IsNullOrEmpty(profile.WebsiteUrl))
		{
			await Windows.System.Launcher.LaunchUriAsync(new Uri(profile.WebsiteUrl));
		}
	}

	// ═══════════════════════════════════════════════════
	// ANIMATIONS
	// ═══════════════════════════════════════════════════

	private void AnimatePageEntrance()
	{
		var elements = new (FrameworkElement Element, int DelayMs)[]
		{
			(HeaderBar, 0),
			(PriceHeroPanel, 100),
			(ChartCard, 150),
			(StatsCard, 200),
			(PositionCard, 250),
			(AnalystCard, 300),
			(AboutCard, 300),
			(NewsCard, 350),
			(FinancialsCard, 350),
			(SimilarCard, 400),
			(FooterBar, 600),
		};

		foreach (var (element, delay) in elements)
		{
			if (element.Visibility == Visibility.Collapsed) continue;

			element.RenderTransform = new CompositeTransform { TranslateY = 32, ScaleX = 0.94, ScaleY = 0.94 };
			element.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);

			var sb = new Storyboard();

			var opacityAnim = new DoubleAnimation
			{
				From = 0,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(700)),
				BeginTime = TimeSpan.FromMilliseconds(delay),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(opacityAnim, element);
			Storyboard.SetTargetProperty(opacityAnim, "Opacity");

			var translateAnim = new DoubleAnimation
			{
				From = 32,
				To = 0,
				Duration = new Duration(TimeSpan.FromMilliseconds(700)),
				BeginTime = TimeSpan.FromMilliseconds(delay),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(translateAnim, element);
			Storyboard.SetTargetProperty(translateAnim, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

			var scaleXAnim = new DoubleAnimation
			{
				From = 0.94,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(700)),
				BeginTime = TimeSpan.FromMilliseconds(delay),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(scaleXAnim, element);
			Storyboard.SetTargetProperty(scaleXAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");

			var scaleYAnim = new DoubleAnimation
			{
				From = 0.94,
				To = 1,
				Duration = new Duration(TimeSpan.FromMilliseconds(700)),
				BeginTime = TimeSpan.FromMilliseconds(delay),
				EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
			};
			Storyboard.SetTarget(scaleYAnim, element);
			Storyboard.SetTargetProperty(scaleYAnim, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");

			sb.Children.Add(opacityAnim);
			sb.Children.Add(translateAnim);
			sb.Children.Add(scaleXAnim);
			sb.Children.Add(scaleYAnim);
			sb.Begin();
		}
	}
}
