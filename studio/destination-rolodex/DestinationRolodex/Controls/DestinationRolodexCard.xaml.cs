using System.Diagnostics;
using DestinationRolodex.Input;
using DestinationRolodex.Models;
using DestinationRolodex.Rendering;
using DestinationRolodex.Services;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;
using Windows.Foundation;
using Windows.System;

namespace DestinationRolodex.Controls;

// xaml-lint: allow codebehind - reusable custom control: pointer capture, wheel normalization, keyboard navigation, the
// frame loop and Skia invalidation are the control's implementation and have no XAML surface. Hosts bind its
// dependency properties and events; they never touch this file.
/// <summary>
/// Living Postcard: one photograph behind an infinite country list, integrated caption and overlaid controls.
/// A single continuous picker position drives rows, the photo effect between the adjacent ordered pair, the
/// geographic globe, photo-in-type, caption treatment, material colors and the arrival camera (brief §3).
/// Two instances share no state.
/// </summary>
public sealed partial class DestinationRolodexCard : UserControl
{
    private const double MinAutoPlayIntervalMs = 2500;
    private const double RowPoolHalf = 4;

    private readonly CountryPickerController _picker = new();
    private readonly CameraController _camera = new();
    private readonly TransitionCoordinator _coordinator = new();
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(8) };
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly Tween _pickerOpacity = new(1);
    private readonly Tween _pickerY = new(0);
    private readonly Tween _lookOpacity = new(0);
    private readonly Tween _lookY = new(6);
    private readonly Tween _scrim = new(1);
    private readonly Tween _entranceY = new(0);
    private readonly Tween _entranceScale = new(1);
    private readonly Tween _hoverLift = new(0);
    private readonly Tween _pointerLight = new(0);
    private readonly List<Button> _dots = new();
    private double _dotWidth = 30;
    private readonly List<Border> _dotBars = new();
    private readonly List<double> _frameIntervals = new();
    private readonly SolidColorBrush _accentBrush = new(Windows.UI.Color.FromArgb(255, 0xFF, 0xF7, 0xE0));
    private readonly HashSet<string> _failedIds = new();
    private readonly Dictionary<(string, float, float), float> _fitCache = new();

    private IReadOnlyList<Destination> _data = Array.Empty<Destination>();
    private SKColor[] _pigments = Array.Empty<SKColor>();
    private (double X, double Y, double Z)?[] _geo = Array.Empty<(double, double, double)?>();
    private DestinationMediaCache? _cache;
    private PhotoTransitionSurface? _surface;
    private GlobeElement? _globe;
    private CardShadowElement? _shadow;
    private Image? _fallbackImage;
    private int _index = -1;
    private int _settledIndex = -1;
    private int _mediaToken;
    private int _previewTicket;
    private bool _loaded;
    private bool _entered;
    private bool _visible = true;
    private bool _hostVisible = true;
    private bool _auto;
    private bool _autoExplicit;
    private bool _hover;
    private bool _focused;
    private bool _previewing;
    private bool _syncingSelection;
    private double _autoProgress;
    private double? _lastTick;
    private Pointer? _capturedPointer;
    private Point _dragOrigin;
    private double _dragPanX, _dragPanY;
    private bool _dragIsLook;
    private (Destination From, Destination To)? _lastPair;
    private DateTime? _statusDue;
    private Size _lastViewport;
    private double _pickerCenterY = 260;
    private double _rowPitch = 76;
    private double _countryFontSize = 58;
    private double _pickerWindow = 300;
    private SKPoint? _pointerScenePoint;
    private FontFamily? _fontSemiBold, _fontMedium, _fontRegular;
    private Brush? _sceneInk, _regionInk;

    public DestinationRolodexCard()
    {
        InitializeComponent();
        _timer.Tick += (_, _) => Tick();
        _picker.SelectionChanged += OnPickerSelectionChanged;
        _coordinator.Started += t => TransitionStarted?.Invoke(this, new TransitionEventArgs(t.FromDestination, t.ToDestination, t.Settings.Kind, t.Settings.Duration, t.IsPreview, t.Direction));
        _coordinator.Completed += OnTransitionCompleted;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += (_, _) => UpdateLayoutMetrics();
        Caption.SizeChanged += (_, _) => UpdateLayoutMetrics();
        EffectiveViewportChanged += OnEffectiveViewportChanged;

        Scene.PointerWheelChanged += OnSceneWheel;
        Scene.PointerPressed += OnScenePointerPressed;
        Scene.PointerMoved += OnScenePointerMoved;
        Scene.PointerReleased += OnScenePointerReleased;
        Scene.PointerCanceled += (s, e) => EndDrag(e, cancelled: true);
        Scene.PointerCaptureLost += (s, e) => EndDrag(e, cancelled: true);
        Scene.PointerEntered += OnScenePointerEntered;
        Scene.PointerExited += OnScenePointerExited;
        CardFrame.PointerEntered += (_, e) => { if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && !IsReducedMotionEnabled) { _hoverLift.Go(-2, .42); Wake(); } };
        CardFrame.PointerExited += (_, _) => { _hoverLift.Go(0, .42); Wake(); };
        Root.KeyDown += OnKeyDown;
        Root.GotFocus += (_, _) => { _focused = true; Wake(); };
        Root.LostFocus += (_, _) => { _focused = XamlRoot is { } xr && FocusManager.GetFocusedElement(xr) is DependencyObject d && IsDescendant(d); Wake(); };
        CreditLink.Click += (_, _) => { if (SelectedDestination?.PhotoSource is { } uri) { _ = Windows.System.Launcher.LaunchUriAsync(uri); } };
        PlayRing.Stroke = _accentBrush;
    }

    #region Dependency properties

    public static readonly DependencyProperty DestinationsProperty = DependencyProperty.Register(
        nameof(Destinations), typeof(IReadOnlyList<Destination>), typeof(DestinationRolodexCard), new PropertyMetadata(null, (d, _) => ((DestinationRolodexCard)d).OnDestinationsChanged()));

    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(
        nameof(SelectedIndex), typeof(int), typeof(DestinationRolodexCard), new PropertyMetadata(0, (d, e) => ((DestinationRolodexCard)d).OnSelectedIndexChanged((int)e.NewValue)));

    public static readonly DependencyProperty SelectedDestinationIdProperty = DependencyProperty.Register(
        nameof(SelectedDestinationId), typeof(string), typeof(DestinationRolodexCard), new PropertyMetadata(null, (d, e) => ((DestinationRolodexCard)d).OnSelectedIdChanged(e.NewValue as string)));

    public static readonly DependencyProperty TransitionKindProperty = DependencyProperty.Register(
        nameof(TransitionKind), typeof(PhotoTransitionKind), typeof(DestinationRolodexCard), new PropertyMetadata(PhotoTransitionKind.MorphBlend, (d, _) => ((DestinationRolodexCard)d).Wake()));

    public static readonly DependencyProperty TransitionDurationProperty = DependencyProperty.Register(
        nameof(TransitionDuration), typeof(TimeSpan), typeof(DestinationRolodexCard), new PropertyMetadata(TimeSpan.FromMilliseconds(1200)));

    public static readonly DependencyProperty TransitionStrengthProperty = DependencyProperty.Register(
        nameof(TransitionStrength), typeof(double), typeof(DestinationRolodexCard), new PropertyMetadata(.6, (d, _) => ((DestinationRolodexCard)d).Wake()));

    public static readonly DependencyProperty IsAutoPlayEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoPlayEnabled), typeof(bool), typeof(DestinationRolodexCard), new PropertyMetadata(false, (d, e) => ((DestinationRolodexCard)d).OnAutoPlayEnabledChanged((bool)e.NewValue)));

    public static readonly DependencyProperty AutoPlayIntervalProperty = DependencyProperty.Register(
        nameof(AutoPlayInterval), typeof(TimeSpan), typeof(DestinationRolodexCard), new PropertyMetadata(TimeSpan.FromMilliseconds(5500)));

    public static readonly DependencyProperty IsReducedMotionEnabledProperty = DependencyProperty.Register(
        nameof(IsReducedMotionEnabled), typeof(bool), typeof(DestinationRolodexCard), new PropertyMetadata(false, (d, _) => ((DestinationRolodexCard)d).OnReducedMotionChanged()));

    public static readonly DependencyProperty AllowRuntimeEffectProperty = DependencyProperty.Register(
        nameof(AllowRuntimeEffect), typeof(bool), typeof(DestinationRolodexCard), new PropertyMetadata(true));

    public static readonly DependencyProperty DiagnosticCrossfadeProperty = DependencyProperty.Register(
        nameof(DiagnosticCrossfade), typeof(bool), typeof(DestinationRolodexCard), new PropertyMetadata(false, (d, _) => ((DestinationRolodexCard)d).Wake()));

    public static readonly DependencyProperty ViewModeProperty = DependencyProperty.Register(
        nameof(ViewMode), typeof(CardViewMode), typeof(DestinationRolodexCard), new PropertyMetadata(CardViewMode.Browse, (d, e) => ((DestinationRolodexCard)d).ApplyViewMode((CardViewMode)e.NewValue)));

    public static readonly DependencyProperty RendererStatusProperty = DependencyProperty.Register(
        nameof(RendererStatus), typeof(PhotoRendererStatus), typeof(DestinationRolodexCard), new PropertyMetadata(PhotoRendererStatus.Unavailable));

    public static readonly DependencyProperty MediaStatusProperty = DependencyProperty.Register(
        nameof(MediaStatus), typeof(DestinationMediaStatus), typeof(DestinationRolodexCard), new PropertyMetadata(DestinationMediaStatus.Empty));

    public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register(
        nameof(IsPlaying), typeof(bool), typeof(DestinationRolodexCard), new PropertyMetadata(false));

    public static readonly DependencyProperty SceneHeightProperty = DependencyProperty.Register(
        nameof(SceneHeight), typeof(double), typeof(DestinationRolodexCard), new PropertyMetadata(double.NaN, (d, _) => ((DestinationRolodexCard)d).UpdateLayoutMetrics()));

    /// <summary>The ordered catalog (brief: ItemsSource). Replacing it invalidates loads and keeps the selection by id when possible.</summary>
    public IReadOnlyList<Destination>? Destinations { get => (IReadOnlyList<Destination>?)GetValue(DestinationsProperty); set => SetValue(DestinationsProperty, value); }
    public int SelectedIndex { get => (int)GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    public string? SelectedDestinationId { get => (string?)GetValue(SelectedDestinationIdProperty); set => SetValue(SelectedDestinationIdProperty, value); }
    public PhotoTransitionKind TransitionKind { get => (PhotoTransitionKind)GetValue(TransitionKindProperty); set => SetValue(TransitionKindProperty, value); }
    /// <summary>Timed replay duration (lab). Browsing is position-driven and ignores it.</summary>
    public TimeSpan TransitionDuration { get => (TimeSpan)GetValue(TransitionDurationProperty); set => SetValue(TransitionDurationProperty, value); }
    public double TransitionStrength { get => (double)GetValue(TransitionStrengthProperty); set => SetValue(TransitionStrengthProperty, value); }
    /// <summary>Host setting. The tour starts paused unless the host turns this on; reduced motion keeps it off.</summary>
    public bool IsAutoPlayEnabled { get => (bool)GetValue(IsAutoPlayEnabledProperty); set => SetValue(IsAutoPlayEnabledProperty, value); }
    public TimeSpan AutoPlayInterval { get => (TimeSpan)GetValue(AutoPlayIntervalProperty); set => SetValue(AutoPlayIntervalProperty, value); }
    /// <summary>OS policy plus application setting: no entrance, lift, parallax, photo displacement, moving photo fill or animated globe travel.</summary>
    public bool IsReducedMotionEnabled { get => (bool)GetValue(IsReducedMotionEnabledProperty); set => SetValue(IsReducedMotionEnabledProperty, value); }
    /// <summary>Diagnostics: false forces the plain-image capability path even where the runtime effect compiles.</summary>
    public bool AllowRuntimeEffect { get => (bool)GetValue(AllowRuntimeEffectProperty); set => SetValue(AllowRuntimeEffectProperty, value); }
    /// <summary>Lab-only: renders the diagnostic Original Crossfade instead of the selected effect. Never an advertised mode.</summary>
    public bool DiagnosticCrossfade { get => (bool)GetValue(DiagnosticCrossfadeProperty); set => SetValue(DiagnosticCrossfadeProperty, value); }
    public CardViewMode ViewMode { get => (CardViewMode)GetValue(ViewModeProperty); set => SetValue(ViewModeProperty, value); }
    public PhotoRendererStatus RendererStatus { get => (PhotoRendererStatus)GetValue(RendererStatusProperty); private set => SetValue(RendererStatusProperty, value); }
    public DestinationMediaStatus MediaStatus { get => (DestinationMediaStatus)GetValue(MediaStatusProperty); private set => SetValue(MediaStatusProperty, value); }
    public bool IsPlaying { get => (bool)GetValue(IsPlayingProperty); private set => SetValue(IsPlayingProperty, value); }
    /// <summary>Explicit scene height; NaN lets the card clamp from the viewport height (brief §2), never below the caption height + 400.</summary>
    public double SceneHeight { get => (double)GetValue(SceneHeightProperty); set => SetValue(SceneHeightProperty, value); }

    public Destination? SelectedDestination => _index >= 0 && _index < _data.Count ? _data[_index] : null;
    public TransitionSettings Settings => new(TransitionKind, TransitionDuration, TransitionStrength);
    public string? RendererDetail { get; private set; }

    #endregion

    #region Events

    /// <summary>Selection candidate changed (the nearest row, at the half step).</summary>
    public event EventHandler<DestinationChangedEventArgs>? SelectionChanged;
    /// <summary>Fired once the list has arrived on a destination (q settled on an integer).</summary>
    public event EventHandler<DestinationChangedEventArgs>? SelectionSettled;
    public event EventHandler<TransitionEventArgs>? TransitionStarted;
    public event EventHandler<TransitionProgressEventArgs>? TransitionProgress;
    public event EventHandler<TransitionEventArgs>? TransitionCompleted;
    public event EventHandler<string>? RendererFallback;
    public event EventHandler<string>? MediaFailed;
    public event EventHandler<CardViewMode>? ViewModeChanged;

    public sealed record DestinationChangedEventArgs(int Index, Destination Destination, bool UserInitiated);
    public sealed record TransitionEventArgs(Destination From, Destination To, PhotoTransitionKind Kind, TimeSpan Duration, bool IsPreview, int Direction);
    public sealed record TransitionProgressEventArgs(double Progress, bool IsPreview, Destination From, Destination To);

    #endregion

    #region Lifecycle

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        _fontSemiBold ??= (FontFamily)Application.Current.Resources["RolodexFontSemiBold"];
        _fontMedium ??= (FontFamily)Application.Current.Resources["RolodexFontMedium"];
        _fontRegular ??= (FontFamily)Application.Current.Resources["RolodexFontRegular"];
        _sceneInk ??= (Brush)Application.Current.Resources["RolodexSceneTextBrushFixed"];
        _regionInk ??= (Brush)Application.Current.Resources["PostcardRegionBrushFixed"];
        GlyphLayout.Apply(Eyebrow, "DESTINATIONS", _fontSemiBold, 10, 2.2, _sceneInk);
#if DEBUG
        if (App.HarnessOptions.NoShader)
        {
            AllowRuntimeEffect = false;
        }
#endif
        EnsureSurface();
        _cache ??= new DestinationMediaCache();
        ApplyHarnessHooks();
        if (XamlRoot is { } root)
        {
            root.Changed += OnXamlRootChanged;
            _hostVisible = root.IsHostVisible;
        }

        UpdateLayoutMetrics();
        InitializeData();
        if (!_entered)
        {
            _entered = true;
            if (!IsReducedMotionEnabled)
            {
                _entranceY.Restart(8, 0, .56, new CubicBezier(.16, 1, .3, 1));
                _entranceScale.Restart(.996, 1, .56, new CubicBezier(.16, 1, .3, 1));
            }
        }

        Wake();
        await LoadSurfaceAssetsAsync();
    }

    private async Task LoadSurfaceAssetsAsync()
    {
        try
        {
            if (_surface is not null && !_surface.Renderer.HasTypeface)
            {
                var fontFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Fonts/Inter_SemiBold.ttf"));
                SKTypeface? typeface;
                using (var stream = await fontFile.OpenStreamForReadAsync())
                {
                    typeface = SKTypeface.FromStream(stream);
                }

                if (!_loaded || _surface is null)
                {
                    typeface?.Dispose();
                    return;
                }

                if (typeface is not null)
                {
                    _surface.Renderer.SetTypeface(typeface);
                }
            }

            if (_globe is null && SKCanvasElement.IsSupportedOnCurrentPlatform())
            {
                byte[]? mask = null;
                try
                {
                    var maskFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Globe/landmask.bin"));
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(maskFile);
                    mask = System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeBufferExtensions.ToArray(buffer);
                }
                catch
                {
                    mask = null; // neutral maskless globe
                }

                if (_loaded && _globe is null)
                {
                    _globe = new GlobeElement(new GlobeRenderer(mask));
                    GlobeHost.Child = _globe;
                }
            }

            PresentFrame();
        }
        catch (Exception ex)
        {
            RendererDetail = $"Scene assets: {ex.Message}";
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        _timer.Stop();
        _lastTick = null;
        _mediaToken++;
        _previewTicket++;
        _previewing = false;
        if (XamlRoot is { } root)
        {
            root.Changed -= OnXamlRootChanged;
        }

        _coordinator.Clear();
        if (_surface is not null)
        {
            PhotoHost.Child = null;
            _surface.Release();
            _surface = null;
        }

        if (_globe is not null)
        {
            GlobeHost.Child = null;
            _globe.Release();
            _globe = null;
        }

        if (_shadow is not null)
        {
            ShadowHost.Child = null;
            _shadow.Release();
            _shadow = null;
        }

        _fallbackImage = null;
        PhotoHost.Child = null;
        _cache?.Dispose();
        _cache = null;
    }

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        _hostVisible = sender.IsHostVisible;
        UpdateLayoutMetrics();
        Wake();
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        var viewport = args.EffectiveViewport;
        var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
        bounds.Intersect(viewport);
        _visible = !bounds.IsEmpty && bounds.Width > 0 && bounds.Height > 0;
        Wake();
    }

    private void EnsureSurface()
    {
        if (_surface is not null || _fallbackImage is not null)
        {
            return;
        }

        if (SKCanvasElement.IsSupportedOnCurrentPlatform())
        {
            _surface = new PhotoTransitionSurface(AllowRuntimeEffect);
            PhotoHost.Child = _surface;
            _shadow = new CardShadowElement();
            ShadowHost.Child = _shadow;
            if (_surface.Renderer.HasRuntimeEffect)
            {
                RendererStatus = PhotoRendererStatus.Shader;
                RendererDetail = "Runtime SkSL effect";
            }
            else
            {
                RendererStatus = PhotoRendererStatus.PlainImage;
                RendererDetail = _surface.Renderer.CompileError ?? "Runtime effect unavailable";
                RendererFallback?.Invoke(this, $"Runtime effect unavailable. Showing plain photos with a crossfade. {RendererDetail}");
            }
        }
        else
        {
            _fallbackImage = new Image { Stretch = Stretch.UniformToFill, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            PhotoHost.Child = _fallbackImage;
            RendererStatus = PhotoRendererStatus.Unavailable;
            RendererDetail = "SKCanvasElement is not supported on this target";
            RendererFallback?.Invoke(this, "Skia surface unavailable on this target. Showing plain photos.");
        }
    }

    private void ApplyHarnessHooks()
    {
#if DEBUG
        var h = App.HarnessOptions;
        if (_cache is not null)
        {
            if (h.MediaDelayMs > 0) _cache.ArtificialDelay = TimeSpan.FromMilliseconds(h.MediaDelayMs);
            if (h.MediaFail is { } fail) foreach (var id in fail.Split(',', StringSplitOptions.RemoveEmptyEntries)) _cache.FailingIds.Add(id);
        }
#endif
    }

    #endregion

    #region Data

    private void OnDestinationsChanged()
    {
        if (_loaded)
        {
            InitializeData();
        }
    }

    private void InitializeData()
    {
        var previousId = SelectedDestination?.Id ?? SelectedDestinationId;
        var incoming = Destinations ?? Array.Empty<Destination>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var d in incoming)
        {
            if (!ids.Add(d.Id))
            {
                throw new ArgumentException($"Duplicate destination id: {d.Id}", nameof(Destinations));
            }
        }

        _mediaToken++;
        _previewTicket++;
        _previewing = false;
        _lastPair = null;
        _failedIds.Clear();
        _coordinator.Clear();
        _cache?.Clear();
        _cache?.Pinned.Clear();
        _data = incoming;
        _pigments = _data.Select(d => ParseSkColor(d.FallbackColor)).ToArray();
        _geo = _data.Select(d => d.Geo is { IsValid: true } g ? GlobeRenderer.ToUnitVector(g) : ((double, double, double)?)null).ToArray();
        _index = -1;
        _settledIndex = -1;

        var start = previousId is not null ? Math.Max(0, IndexOf(previousId)) : Math.Clamp(SelectedIndex, 0, Math.Max(0, _data.Count - 1));
        var hasData = _data.Count > 0;
        EmptyState.Visibility = hasData ? Visibility.Collapsed : Visibility.Visible;
        CardFrame.Visibility = hasData ? Visibility.Visible : Visibility.Collapsed;
        ShadowHost.Visibility = hasData ? Visibility.Visible : Visibility.Collapsed;
        var canNavigate = _data.Count > 1;
        PrevButton.IsEnabled = NextButton.IsEnabled = PlayButton.IsEnabled = canNavigate;
        RebuildIndicators();
        _picker.ReducedMotion = IsReducedMotionEnabled;
        _camera.ReducedMotion = IsReducedMotionEnabled;
        _autoExplicit = false;
        _auto = IsAutoPlayEnabled && !IsReducedMotionEnabled && canNavigate;
        _autoProgress = 0;
        SyncPlayButton();
        _picker.Reset(_data.Count, start);
        if (!hasData)
        {
            MediaStatus = DestinationMediaStatus.Empty;
        }

        UpdateLayoutMetrics();
        PresentFrame();
        Wake();
    }

    private int IndexOf(string id)
    {
        for (var i = 0; i < _data.Count; i++)
        {
            if (_data[i].Id == id)
            {
                return i;
            }
        }

        return -1;
    }

    private void RebuildIndicators()
    {
        Pagination.Children.Clear();
        _dots.Clear();
        _dotBars.Clear();
        var inactive = (Brush)Application.Current.Resources["PostcardDotBrushFixed"];
        for (var i = 0; i < _data.Count; i++)
        {
            var index = i;
            var bar = new Border { Width = 4, Height = 4, CornerRadius = new CornerRadius(2), Background = inactive, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            var button = new Button { Style = (Style)Application.Current.Resources["RolodexBareButton"], Width = _dotWidth, Height = 44, MinWidth = 0, Content = bar };
            AutomationProperties.SetName(button, $"Show {_data[i].Country}");
            ToolTipService.SetToolTip(button, _data[i].Country);
            button.Click += (_, _) => GoTo(index);
            _dots.Add(button);
            _dotBars.Add(bar);
            Pagination.Children.Add(button);
        }
    }

    private void UpdateIndicators()
    {
        var inactive = (Brush)Application.Current.Resources["PostcardDotBrushFixed"];
        for (var i = 0; i < _dotBars.Count; i++)
        {
            var selected = i == _index;
            var bar = _dotBars[i];
            bar.Width = selected ? 21 : 4;
            bar.Height = selected ? 3 : 4;
            bar.CornerRadius = new CornerRadius(selected ? 1.5 : 2);
            bar.Background = selected ? _accentBrush : inactive;
            AutomationProperties.SetItemStatus(_dots[i], selected ? "selected" : string.Empty);
        }
    }

    #endregion

    #region Layout metrics (content-aware; brief §2)

    // xaml-lint: allow responsive - these thresholds are container queries on the card's own width (two cards can sit
    // side by side at different widths) and viewport-height clamps; {utu:Responsive} only switches on window width.
    private void UpdateLayoutMetrics()
    {
        if (!_loaded)
        {
            return;
        }

        var cardWidth = Root.ActualWidth > 0 ? Root.ActualWidth : 480;
        var viewport = XamlRoot?.Size ?? new Size(1440, 1040);
        _lastViewport = viewport;
        var narrowHost = viewport.Width <= 560;
        var compact = cardWidth <= 360;
        var rim = 6d;
        var outerRadius = narrowHost ? 27d : 30d;
        Rim.CornerRadius = new CornerRadius(outerRadius);
        Rim.Padding = new Thickness(rim);
        Inner.CornerRadius = new CornerRadius(outerRadius - rim);
        Scene.CornerRadius = new CornerRadius(outerRadius - rim);
        InnerHighlight.CornerRadius = new CornerRadius(outerRadius - rim);
        var sceneWidth = Math.Max(0, cardWidth - 2 * rim);

        var captionHeight = Caption.ActualHeight > 0 ? Caption.ActualHeight : 150;
        var footerHeight = SceneFooter.ActualHeight > 0 ? SceneFooter.ActualHeight : 44;
        var contentMinimum = captionHeight + 400;
        double sceneHeight;
        if (!double.IsNaN(SceneHeight) && SceneHeight > 0)
        {
            sceneHeight = SceneHeight;
        }
        else if (narrowHost)
        {
            sceneHeight = Math.Clamp(viewport.Height * .73, 553, 620);
        }
        else
        {
            sceneHeight = Math.Clamp(viewport.Height * .74, 580, 670);
        }

        sceneHeight = Math.Max(sceneHeight, contentMinimum);
        Scene.Height = sceneHeight;
        SceneClip.Rect = new Rect(0, 0, sceneWidth, sceneHeight);
        Caption.Padding = new Thickness(compact ? 20 : 26, 0, compact ? 20 : 26, 19);
        LocationText.FontSize = compact ? 20.5 : 23;
        SceneFooter.Margin = new Thickness(compact ? 19 : 24, 0, compact ? 16 : 18, captionHeight + 14);
        TopBar.Margin = compact ? new Thickness(18, 16, 12, 0) : new Thickness(22, 18, 16, 0);
        GlobeHost.Width = GlobeHost.Height = compact ? 25 : 27;
        MediaStatusPill.Margin = new Thickness(compact ? 18 : 22, compact ? 60 : 66, 22, 0);
        GlyphLayout.Apply(Eyebrow, "DESTINATIONS", _fontSemiBold!, compact ? 9 : 10, compact ? 1.5 : 2.2, _sceneInk!);

        // Pagination must show every destination: below 340 the six 30 px targets no longer fit beside the arrows.
        var dotWidth = cardWidth > 0 && cardWidth < 300 ? 21 : 30;
        if (Math.Abs(dotWidth - _dotWidth) > .5)
        {
            _dotWidth = dotWidth;
            Pagination.Spacing = dotWidth < 30 ? 1 : 2;
            foreach (var dot in _dots)
            {
                dot.Width = dotWidth;
            }
        }

        // Picker centered in the browsing area above the caption and footer.
        var browsingBottom = sceneHeight - captionHeight - footerHeight - 16;
        _pickerCenterY = Math.Max(120, browsingBottom / 2 + 12);
        _rowPitch = Math.Clamp(sceneWidth * .17, 58, 80);
        _pickerWindow = compact ? 266 : 300;
        _countryFontSize = Math.Clamp(sceneWidth * .1235, 34, 61);
        Picker.Configure(_pickerWindow, compact);
        Picker.Margin = new Thickness(0, _pickerCenterY - _pickerWindow / 2, 0, 0);

        if (_fallbackImage is not null)
        {
            var ox = PhotoRenderer.OverscanPerSide * sceneWidth;
            var oy = PhotoRenderer.OverscanPerSide * sceneHeight;
            PhotoHost.Margin = new Thickness(-ox, -oy, -ox, -oy);
        }

        UpdateHint();
        PresentFrame();
    }

    private void UpdateHint()
    {
        var coarse = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
        var text = coarse ? "SWIPE TO EXPLORE" : "SCROLL TO EXPLORE";
        var compact = Root.ActualWidth > 0 && Root.ActualWidth <= 360;
        GlyphLayout.Apply(HintText, text, _fontMedium!, compact ? 8 : 9, compact ? .8 : 1.3, _sceneInk!);
    }

    #endregion

    #region Selection

    private void OnPickerSelectionChanged(int index, bool user)
    {
        _index = index;
        var d = _data[index];
        LocationText.Text = d.Location;
        GlyphLayout.Apply(RegionText, d.Region.ToUpperInvariant(), _fontMedium!, Root.ActualWidth <= 360 ? 8 : 9, Root.ActualWidth <= 360 ? 1 : 1.45, _regionInk!);
        DescriptionText.Text = d.Description;
        LookCountry.Text = d.Country;
        CreditText.Text = string.IsNullOrEmpty(d.PhotoCredit) ? "View photo source" : $"Photo: {d.PhotoCredit}";
        CreditLink.IsEnabled = d.PhotoSource is not null;
        UpdateIndicators();
        if (!_syncingSelection)
        {
            _syncingSelection = true;
            SelectedIndex = index;
            SelectedDestinationId = d.Id;
            _syncingSelection = false;
        }

        RequestPhotos(index);
        SelectionChanged?.Invoke(this, new DestinationChangedEventArgs(index, d, user));
        Wake();
    }

    private void OnSettled(int index, bool user)
    {
        if (index == _settledIndex)
        {
            return;
        }

        var previous = _settledIndex;
        _settledIndex = index;
        var d = _data[index];
        if (previous >= 0 && previous != index)
        {
            _lastPair = (_data[previous], d);
        }

        if (user)
        {
            Announcer.Text = $"{d.Country}. {d.Description}";
        }

        SelectionSettled?.Invoke(this, new DestinationChangedEventArgs(index, d, user));
    }

    private void OnSelectedIndexChanged(int value)
    {
        if (_syncingSelection || !_loaded || _data.Count == 0 || value < 0 || value >= _data.Count)
        {
            return;
        }

        if (value != _index)
        {
            GoTo(value);
        }
    }

    private void OnSelectedIdChanged(string? id)
    {
        if (_syncingSelection || !_loaded || id is null)
        {
            return;
        }

        var i = IndexOf(id);
        if (i >= 0 && i != _index)
        {
            GoTo(i);
        }
    }

    public void Next() => Step(1, true);
    public void Previous() => Step(-1, true);

    private void Step(int step, bool user)
    {
        if (_data.Count == 0)
        {
            return;
        }

        if (user)
        {
            ExitPreview();
            Pause();
        }

        _autoProgress = 0;
        _picker.Step(step, user);
        Wake();
    }

    /// <summary>Shortest path to the destination at <paramref name="index"/>; ties resolve forward.</summary>
    public void GoTo(int index)
    {
        if (_data.Count == 0)
        {
            return;
        }

        ExitPreview();
        Pause();
        _autoProgress = 0;
        _picker.GoTo(index);
        Wake();
    }

    #endregion

    #region Media (generation-safe; brief §3 and §9)

    /// <summary>Requests the pair around the selection plus its neighbors; a late completion never installs anything, the next frame simply finds it ready.</summary>
    private void RequestPhotos(int index)
    {
        if (_cache is null || _data.Count == 0)
        {
            return;
        }

        var token = _mediaToken;
        foreach (var offset in new[] { 0, 1, -1, 2 })
        {
            var d = _data[CircularMath.PositiveModulo(index + offset, _data.Count)];
            if (_cache.Peek(d.Id) is not null || _failedIds.Contains(d.Id))
            {
                continue;
            }

            var destination = d;
            _ = _cache.GetAsync(destination).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _ = DispatcherQueue.TryEnqueue(() =>
                    {
                        if (token != _mediaToken || !_loaded) return;
                        _failedIds.Add(destination.Id);
                        if (_index >= 0 && _data[_index].Id == destination.Id)
                        {
                            MediaStatus = DestinationMediaStatus.Failed;
                            SetStatus("Photo unavailable · check connection", retry: true);
                            MediaFailed?.Invoke(this, t.Exception?.GetBaseException().Message ?? "decode failed");
                        }

                        PresentFrame();
                    });
                }
                else
                {
                    _ = DispatcherQueue.TryEnqueue(() => { if (_loaded) { PinWorkingSet(); Wake(); } });
                }
            }, TaskScheduler.Default);
        }

        PinWorkingSet();
    }

    private void PinWorkingSet()
    {
        if (_cache is null)
        {
            return;
        }

        _cache.Pinned.Clear();
        if (_data.Count > 0)
        {
            var frame = SceneFrame.Sample(_picker.Position, _data.Count);
            _cache.Pinned.Add(_data[frame.FromIndex].Id);
            _cache.Pinned.Add(_data[frame.ToIndex].Id);
            _cache.Pinned.Add(_data[frame.Selected].Id);
            _cache.Pinned.Add(_data[CircularMath.PositiveModulo(frame.Selected - 1, _data.Count)].Id);
            _cache.Pinned.Add(_data[CircularMath.PositiveModulo(frame.Selected + 1, _data.Count)].Id);
        }

        if (_coordinator.Active is { } t) { _cache.Pinned.Add(t.FromDestination.Id); _cache.Pinned.Add(t.ToDestination.Id); }
    }

    private void SetStatus(string? message, bool retry)
    {
        MediaStatusPill.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
        StatusText.Text = message ?? string.Empty;
        RetryButton.Visibility = retry ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>Retries the selected destination's photo after a failure.</summary>
    public void Retry()
    {
        if (_index < 0 || _cache is null)
        {
            return;
        }

        var d = _data[_index];
        _failedIds.Remove(d.Id);
        _cache.Invalidate(d.Id);
        SetStatus(null, retry: false);
        MediaStatus = DestinationMediaStatus.Loading;
        _statusDue = DateTime.UtcNow.AddMilliseconds(1100);
        RequestPhotos(_index);
        Wake();
    }

    #endregion

    #region Autoplay (starts paused; brief §8)

    public void Play()
    {
        ExitPreview();
        if (_data.Count < 2)
        {
            return;
        }

        if (ViewMode == CardViewMode.Look)
        {
            ViewMode = CardViewMode.Browse;
        }

        _auto = true;
        _autoExplicit = true;
        _autoProgress = 0;
        SyncPlayButton();
        Wake();
    }

    public void Pause()
    {
        _auto = false;
        _autoExplicit = false;
        _autoProgress = 0;
        SyncPlayButton();
    }

    private void OnAutoPlayEnabledChanged(bool enabled)
    {
        if (!_loaded)
        {
            return;
        }

        if (enabled && !IsReducedMotionEnabled && _data.Count > 1)
        {
            _auto = true;
            _autoExplicit = false;
            _autoProgress = 0;
            SyncPlayButton();
            Wake();
        }
        else if (!enabled)
        {
            Pause();
        }
    }

    private void SyncPlayButton()
    {
        IsPlaying = _auto;
        var label = _auto ? "Pause destination tour" : "Play destination tour";
        AutomationProperties.SetName(PlayButton, label);
        ToolTipService.SetToolTip(PlayButton, label);
        PlayGlyph.Visibility = _auto ? Visibility.Collapsed : Visibility.Visible;
        PauseGlyph.Visibility = _auto ? Visibility.Visible : Visibility.Collapsed;
        PlayRing.StrokeDashOffset = 138.2;
    }

    #endregion

    #region View mode and camera

    private void ApplyViewMode(CardViewMode mode)
    {
        if (!_loaded)
        {
            return;
        }

        ExitPreview();
        Pause();
        var look = mode == CardViewMode.Look;
        _camera.IsLooking = look;
        Picker.IsInert = look;
        Scene.IsTabStop = look;
        AutomationProperties.SetName(Scene, look ? "Explore destination photo. Arrow keys pan; plus and minus zoom; Escape returns to countries." : string.Empty);
        CameraControls.Visibility = look ? Visibility.Visible : Visibility.Collapsed;
        GestureHint.Visibility = look ? Visibility.Collapsed : Visibility.Visible;
        CreditLink.Visibility = look ? Visibility.Visible : Visibility.Collapsed;
        PlayButton.Visibility = look ? Visibility.Collapsed : Visibility.Visible;
        var reduced = IsReducedMotionEnabled;
        _pickerOpacity.Go(look ? 0 : 1, reduced ? 0 : .36);
        _pickerY.Go(look ? -_pickerWindow * .04 : 0, reduced ? 0 : .6);
        _lookOpacity.Go(look ? 1 : 0, reduced ? 0 : .35);
        _lookY.Go(look ? 0 : 6, reduced ? 0 : .4);
        _scrim.Go(look ? .55 : 1, reduced ? 0 : .4);
        ResetCamera();
        if (look)
        {
            Scene.Focus(FocusState.Programmatic);
        }

        ViewModeChanged?.Invoke(this, mode);
        Wake();
    }

    public void ZoomIn() => SetZoom(_camera.TargetZoom + .15);
    public void ZoomOut() => SetZoom(_camera.TargetZoom - .15);

    private void SetZoom(double zoom)
    {
        _camera.SetZoom(zoom);
        ZoomValue.Text = $"{_camera.TargetZoom:0.0}×";
        Wake();
    }

    public void ResetCamera()
    {
        _camera.Reset();
        ZoomValue.Text = "1.0×";
        Wake();
    }

    #endregion

    #region Reduced motion

    private void OnReducedMotionChanged()
    {
        var reduced = IsReducedMotionEnabled;
        _picker.ReducedMotion = reduced;
        _camera.ReducedMotion = reduced;
        if (!_loaded)
        {
            return;
        }

        if (reduced)
        {
            ExitPreview();
            _coordinator.CancelAll();
            Pause();
            ResetCamera();
            _picker.SnapToNearest();
            _entranceY.Snap(0);
            _entranceScale.Snap(1);
            _hoverLift.Snap(0);
            _pointerLight.Snap(0);
        }

        PresentFrame();
        Wake();
    }

    #endregion

    #region Input

    private double Now => _clock.Elapsed.TotalSeconds;

    private void OnSceneWheel(object sender, PointerRoutedEventArgs e)
    {
        if (_data.Count == 0 || e.KeyModifiers.HasFlag(VirtualKeyModifiers.Control))
        {
            return;
        }

        var point = e.GetCurrentPoint(Scene);
        if (point.Properties.IsHorizontalMouseWheel)
        {
            return;
        }

        e.Handled = true;
        ExitPreview();
        // Uno reports ±120 per notch; the prototype's math expects the browser's 100 per notch, positive = down.
        var cssDelta = -point.Properties.MouseWheelDelta * (100d / 120d);
        if (ViewMode == CardViewMode.Look)
        {
            SetZoom(_camera.TargetZoom - cssDelta * .0012);
            return;
        }

        Pause();
        _picker.Wheel(cssDelta, Now);
        Wake();
    }

    private void OnScenePointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_data.Count == 0 || _capturedPointer is not null)
        {
            return;
        }

        var point = e.GetCurrentPoint(Scene);
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && !point.Properties.IsLeftButtonPressed)
        {
            return;
        }

        ExitPreview();
        Pause();
        _hoverLift.Go(0, .2);
        _capturedPointer = e.Pointer;
        _dragOrigin = point.Position;
        _dragIsLook = ViewMode == CardViewMode.Look;
        if (_dragIsLook)
        {
            _dragPanX = _camera.TargetX;
            _dragPanY = _camera.TargetY;
        }
        else
        {
            _picker.DragStart(point.Position.X, point.Position.Y, Now, HitTestRow(point.Position.Y));
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                Picker.Focus(FocusState.Pointer);
            }
        }

        Scene.CapturePointer(e.Pointer);
        e.Handled = true;
        Wake();
    }

    /// <summary>Row step under a scene y coordinate (rows are drawn by the renderer; solve the pose geometry analytically).</summary>
    private long? HitTestRow(double sceneY)
    {
        var y = sceneY - _pickerCenterY - _pickerY.Value;
        if (Math.Abs(y) > _pickerWindow / 2)
        {
            return null;
        }

        var d = y / _rowPitch;
        for (var i = 0; i < 3; i++)
        {
            d = y / (_rowPitch * (1 - Math.Min(Math.Abs(d), 3) * .035));
        }

        var step = CircularMath.RoundLikeJavaScript(_picker.Position + d);
        return Math.Abs(step - _picker.Position) < 2.6 ? step : null;
    }

    private void OnScenePointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(Scene);
        if (_capturedPointer is not null && e.Pointer.PointerId == _capturedPointer.PointerId)
        {
            var dx = point.Position.X - _dragOrigin.X;
            var dy = point.Position.Y - _dragOrigin.Y;
            if (_dragIsLook)
            {
                _camera.PanBy(_dragPanX, _dragPanY, dx, dy);
            }
            else
            {
                _picker.DragMove(point.Position.X, point.Position.Y, Now);
            }

            e.Handled = true;
        }
        else if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse && ViewMode == CardViewMode.Browse && !IsReducedMotionEnabled)
        {
            var w = Scene.ActualWidth;
            var h = Scene.ActualHeight;
            if (w > 0 && h > 0)
            {
                _camera.SetPointer((point.Position.X / w - .5) * 2, (point.Position.Y / h - .5) * 2);
            }
        }

        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
        {
            _pointerScenePoint = new SKPoint((float)point.Position.X, (float)point.Position.Y);
        }

        Wake();
    }

    private void OnScenePointerReleased(object sender, PointerRoutedEventArgs e) => EndDrag(e, cancelled: false);

    private void EndDrag(PointerRoutedEventArgs e, bool cancelled)
    {
        if (_capturedPointer is null || e.Pointer.PointerId != _capturedPointer.PointerId)
        {
            return;
        }

        _capturedPointer = null;
        Scene.ReleasePointerCapture(e.Pointer);
        if (!_dragIsLook)
        {
            _picker.DragEnd(Now, cancelled);
        }

        Wake();
    }

    private void OnScenePointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
        {
            _hover = true;
            if (!IsReducedMotionEnabled)
            {
                _pointerLight.Go(1, .32);
            }

            Wake();
        }
    }

    private void OnScenePointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hover = false;
        _pointerLight.Go(0, .32);
        if (_capturedPointer is null && ViewMode == CardViewMode.Browse)
        {
            _camera.ClearPointer();
        }

        Wake();
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Escape && ViewMode == CardViewMode.Look)
        {
            e.Handled = true;
            ViewMode = CardViewMode.Browse;
            Picker.Focus(FocusState.Keyboard);
            return;
        }

        if (!ReferenceEquals(e.OriginalSource, Picker) && !ReferenceEquals(e.OriginalSource, Scene))
        {
            return;
        }

        if (ViewMode == CardViewMode.Look)
        {
            switch (e.Key)
            {
                case VirtualKey.Left: _camera.Nudge(-.18, 0); break;
                case VirtualKey.Right: _camera.Nudge(.18, 0); break;
                case VirtualKey.Up: _camera.Nudge(0, -.18); break;
                case VirtualKey.Down: _camera.Nudge(0, .18); break;
                case VirtualKey.Add: case (VirtualKey)187: SetZoom(_camera.TargetZoom + .15); break;
                case VirtualKey.Subtract: case (VirtualKey)189: SetZoom(_camera.TargetZoom - .15); break;
                default: return;
            }

            e.Handled = true;
            Wake();
            return;
        }

        if (_data.Count == 0)
        {
            return;
        }

        switch (e.Key)
        {
            case VirtualKey.Down: Step(1, true); break;
            case VirtualKey.Up: Step(-1, true); break;
            case VirtualKey.PageDown: Step(3, true); break;
            case VirtualKey.PageUp: Step(-3, true); break;
            case VirtualKey.Home: GoTo(0); break;
            case VirtualKey.End: GoTo(_data.Count - 1); break;
            default:
                if (e.Key >= VirtualKey.A && e.Key <= VirtualKey.Z)
                {
                    var match = _picker.TypeAhead((char)('a' + (e.Key - VirtualKey.A)), _data.Select(d => d.Country).ToList(), Now);
                    if (match >= 0)
                    {
                        GoTo(match);
                        break;
                    }
                }

                return;
        }

        e.Handled = true;
    }

    private void OnPlayClick(object sender, RoutedEventArgs e) { if (_auto) Pause(); else Play(); }
    private void OnLookClick(object sender, RoutedEventArgs e) => ViewMode = ViewMode == CardViewMode.Browse ? CardViewMode.Look : CardViewMode.Browse;
    private void OnZoomInClick(object sender, RoutedEventArgs e) => ZoomIn();
    private void OnZoomOutClick(object sender, RoutedEventArgs e) => ZoomOut();
    private void OnResetClick(object sender, RoutedEventArgs e) => ResetCamera();
    private void OnRetryClick(object sender, RoutedEventArgs e) => Retry();
    private void OnPreviousClick(object sender, RoutedEventArgs e) => Previous();
    private void OnNextClick(object sender, RoutedEventArgs e) => Next();

    private bool IsDescendant(DependencyObject element)
    {
        var e = element;
        while (e is not null)
        {
            if (ReferenceEquals(e, this)) return true;
            e = VisualTreeHelper.GetParent(e);
        }

        return false;
    }

    #endregion

    #region Lab replay / scrub (preview; no app side effects)

    private async Task<(Destination From, Destination To, DecodedPhoto FromPhoto, DecodedPhoto ToPhoto)?> PreparePreviewAsync()
    {
        if (_data.Count < 2 || _cache is null)
        {
            return null;
        }

        Pause();
        if (ViewMode == CardViewMode.Look)
        {
            ViewMode = CardViewMode.Browse;
        }

        var current = Math.Max(0, _index);
        var pair = _lastPair ?? (_data[current], _data[CircularMath.PositiveModulo(current + 1, _data.Count)]);
        if (RendererStatus != PhotoRendererStatus.Shader || IsReducedMotionEnabled)
        {
            RendererFallback?.Invoke(this, IsReducedMotionEnabled ? "Reduced motion is enabled; replay is unavailable." : "Runtime effect unavailable. Using the plain-image path.");
            return null;
        }

        var ticket = ++_previewTicket;
        _statusDue = null;
        SetStatus(null, retry: false);
        _previewing = true;
        _picker.IsFrozen = true;
        try
        {
            var from = await _cache.GetAsync(pair.From);
            var to = await _cache.GetAsync(pair.To);
            if (ticket != _previewTicket)
            {
                return null;
            }

            return (pair.From, pair.To, from, to);
        }
        catch
        {
            if (ticket == _previewTicket)
            {
                _previewing = false;
                _picker.IsFrozen = false;
                RendererFallback?.Invoke(this, "Photos need to finish loading before replay.");
            }

            return null;
        }
    }

    /// <summary>Lab: replays the last settled pair (or current → next) over <see cref="TransitionDuration"/>, as a preview.</summary>
    public async Task<bool> ReplayTransitionAsync()
    {
        _coordinator.CancelPreview();
        var pair = await PreparePreviewAsync();
        if (pair is null)
        {
            return false;
        }

        _camera.Reset();
        var direction = PreviewDirection(pair.Value.From, pair.Value.To);
        _coordinator.Replay(pair.Value.From, pair.Value.FromPhoto, pair.Value.To, pair.Value.ToPhoto, direction, Settings);
        _picker.SyncPreview(IndexOf(pair.Value.From.Id), IndexOf(pair.Value.To.Id), 0);
        Wake();
        return true;
    }

    /// <summary>Lab: holds the pair at a fixed progress; rows follow smoothstep(progress).</summary>
    public async Task<bool> SeekTransitionAsync(double progress)
    {
        var pair = await PreparePreviewAsync();
        if (pair is null)
        {
            return false;
        }

        var direction = PreviewDirection(pair.Value.From, pair.Value.To);
        _coordinator.Seek(pair.Value.From, pair.Value.FromPhoto, pair.Value.To, pair.Value.ToPhoto, direction, Settings, progress);
        _picker.SyncPreview(IndexOf(pair.Value.From.Id), IndexOf(pair.Value.To.Id), progress);
        PresentFrame();
        TransitionProgress?.Invoke(this, new TransitionProgressEventArgs(Math.Clamp(progress, 0, 1), true, pair.Value.From, pair.Value.To));
        Wake();
        return true;
    }

    private int PreviewDirection(Destination from, Destination to)
    {
        var delta = CircularMath.ShortestDelta(IndexOf(from.Id), IndexOf(to.Id), _data.Count);
        return delta == 0 ? 1 : Math.Sign(delta);
    }

    private void ExitPreview()
    {
        if (!_previewing)
        {
            return;
        }

        _previewTicket++;
        _coordinator.CancelPreview();
        _previewing = false;
        _picker.IsFrozen = false;
        _picker.SnapToNearest();
        PresentFrame();
    }

    private void OnTransitionCompleted(TransitionCoordinator.Transition t)
    {
        if (t.IsPreview && _previewing)
        {
            _picker.SyncPreview(IndexOf(t.FromDestination.Id), IndexOf(t.ToDestination.Id), 1);
            _coordinator.CancelPreview();
            _previewing = false;
            _picker.IsFrozen = false;
            _picker.SnapToNearest();
        }

        TransitionCompleted?.Invoke(this, new TransitionEventArgs(t.FromDestination, t.ToDestination, t.Settings.Kind, t.Settings.Duration, t.IsPreview, t.Direction));
    }

    #endregion

    #region Frame loop

    private void Wake()
    {
        if (!_timer.IsEnabled && _loaded && _visible && _hostVisible && _data.Count > 0)
        {
            _lastTick = null;
            _timer.Start();
        }
    }

    private void Tick()
    {
        if (!_loaded || !_visible || !_hostVisible || _data.Count == 0)
        {
            _timer.Stop();
            _lastTick = null;
            return;
        }

        if (XamlRoot is { } root && root.Size != _lastViewport)
        {
            UpdateLayoutMetrics();
        }

        var now = Now;
        var dt = _lastTick is { } last ? Math.Min(now - last, .5) : 1d / 60;
        if (_lastTick is { } prev)
        {
            _frameIntervals.Add((now - prev) * 1000);
            if (_frameIntervals.Count > 2000) _frameIntervals.RemoveAt(0);
        }

        _lastTick = now;

        var moving = !_previewing && _picker.Tick(dt, now);

        var canAuto = _auto && ViewMode == CardViewMode.Browse && _capturedPointer is null && (_autoExplicit || (!_hover && !_focused));
        if (canAuto)
        {
            var interval = Math.Max(MinAutoPlayIntervalMs, AutoPlayInterval.TotalMilliseconds);
            _autoProgress += Math.Min(dt, .032) * 1000 / interval;
            if (_autoProgress >= 1)
            {
                _autoProgress = 0;
                Step(1, user: false);
            }
        }

        PlayRing.StrokeDashOffset = 138.2 * (1 - _autoProgress);

        _camera.IsDrifting = false;
        var cameraMoving = _camera.Tick(Math.Min(dt, .032));

        var transitioning = _coordinator.Tick(Math.Min(dt, .032) * 1000);
        if (_coordinator.Active is { } active)
        {
            if (active.IsPreview && _previewing)
            {
                _picker.SyncPreview(IndexOf(active.FromDestination.Id), IndexOf(active.ToDestination.Id), active.Progress);
            }

            TransitionProgress?.Invoke(this, new TransitionProgressEventArgs(active.Progress, active.IsPreview, active.FromDestination, active.ToDestination));
        }

        if (!_previewing && _picker.IsSettled && _index >= 0)
        {
            OnSettled(_index, true);
        }

        if (_statusDue is { } due && DateTime.UtcNow >= due)
        {
            _statusDue = null;
            if (MediaStatus == DestinationMediaStatus.Loading && _index >= 0)
            {
                SetStatus($"Loading {_data[_index].Country} photo…", retry: false);
            }
        }

        var tdt = Math.Min(dt, .032);
        var tweens = _pickerOpacity.Tick(tdt) | _pickerY.Tick(tdt) | _lookOpacity.Tick(tdt) | _lookY.Tick(tdt) | _scrim.Tick(tdt)
                     | _entranceY.Tick(tdt) | _entranceScale.Tick(tdt) | _hoverLift.Tick(tdt) | _pointerLight.Tick(tdt);
        Picker.Opacity = _pickerOpacity.Value;
        LookTitle.Opacity = _lookOpacity.Value;
        LookTitleTranslate.Y = _lookY.Value;
        FrameTransform.TranslateY = _entranceY.Value + _hoverLift.Value;
        FrameTransform.ScaleX = FrameTransform.ScaleY = _entranceScale.Value;

        PresentFrame();
        if (_cache is not null && _surface is not null)
        {
            _cache.ObserveFrames(_surface.Renderer.FramesRendered);
            _cache.Collect(_surface.Renderer.FramesRendered);
        }

        var pendingWork = _statusDue is not null;
        if (!(moving || cameraMoving || canAuto || transitioning || tweens || pendingWork))
        {
            _timer.Stop();
            _lastTick = null;
        }
    }

    /// <summary>Builds one coherent frame from the picker position (or the lab preview) and presents it to every Skia element.</summary>
    private void PresentFrame()
    {
        if (!_loaded || _data.Count == 0)
        {
            return;
        }

        var n = _data.Count;
        var scene = SceneFrame.Sample(_picker.Position, n);
        var sceneWidth = (float)Math.Max(1, Scene.ActualWidth > 0 ? Scene.ActualWidth : 468);
        var material = MaterialColors.From(_pigments[scene.FromIndex], _pigments[scene.ToIndex], scene.P);
        var accent = Windows.UI.Color.FromArgb(255, material.Accent.Red, material.Accent.Green, material.Accent.Blue);
        if (_accentBrush.Color != accent)
        {
            _accentBrush.Color = accent;
        }

        Picker.SetAccent(accent);

        // Caption treatment follows travel; zero at arrival.
        Caption.Opacity = IsReducedMotionEnabled ? 1 : scene.CaptionOpacity;
        CaptionTranslate.Y = IsReducedMotionEnabled ? 0 : scene.CaptionOffsetY;

        // Media identity: in flight the adjacent ordered pair at fraction p; at rest the settled photo.
        DecodedPhoto? from, to;
        FocalPoint focalA, focalB;
        float progress;
        int direction;
        var kind = TransitionKind;
        if (_previewing && _coordinator.Active is { } preview)
        {
            from = preview.FromPhoto;
            to = preview.ToPhoto;
            focalA = preview.FromDestination.FocalPoint;
            focalB = preview.ToDestination.FocalPoint;
            progress = preview.Progress;
            direction = preview.Direction;
            kind = preview.Settings.Kind;
        }
        else
        {
            var dFrom = _data[scene.FromIndex];
            var dTo = _data[scene.ToIndex];
            from = _cache?.Peek(dFrom.Id);
            to = _cache?.Peek(dTo.Id);
            focalA = dFrom.FocalPoint;
            focalB = dTo.FocalPoint;
            progress = (float)scene.P;
            direction = 1;
            if (from is null || to is null)
            {
                // One endpoint not ready: hold the selected destination's own photo, static, or the neutral backdrop when that
                // photo is the missing one. A neighbor's photograph never stands in under this caption (brief §9).
                var selectedIsFrom = scene.Selected == scene.FromIndex;
                var ready = selectedIsFrom ? from : to;
                from = to = ready;
                focalA = focalB = selectedIsFrom ? dFrom.FocalPoint : dTo.FocalPoint;
                progress = 0;
                var selectedId = _data[scene.Selected].Id;
                if (ready is null && _failedIds.Contains(selectedId))
                {
                    // Arrived on a destination whose photo already failed while it was a neighbor: say so, offer Retry.
                    if (MediaStatus != DestinationMediaStatus.Failed)
                    {
                        MediaStatus = DestinationMediaStatus.Failed;
                        _statusDue = null;
                        SetStatus("Photo unavailable · check connection", retry: true);
                    }
                }
                else if (ready is null)
                {
                    if (MediaStatus != DestinationMediaStatus.Loading)
                    {
                        MediaStatus = DestinationMediaStatus.Loading;
                        _statusDue ??= DateTime.UtcNow.AddMilliseconds(1100);
                    }
                }
                else if (MediaStatus != DestinationMediaStatus.Ready)
                {
                    // The selected photo is on screen; a still-loading or failed neighbor is not this card's status.
                    MediaStatus = DestinationMediaStatus.Ready;
                    _statusDue = null;
                    SetStatus(null, retry: false);
                }
            }
            else if (MediaStatus != DestinationMediaStatus.Ready && !_failedIds.Contains(_data[scene.Selected].Id))
            {
                MediaStatus = DestinationMediaStatus.Ready;
                _statusDue = null;
                SetStatus(null, retry: false);
            }
        }

        var camera = _camera.Pose;
        if (!IsReducedMotionEnabled)
        {
            camera = camera with { Zoom = (float)(camera.Zoom * scene.ArrivalScale), TranslateY = (float)(camera.TranslateY + scene.ArrivalOffsetY) };
        }

        var frame = new PhotoFrame(from, to, focalA, focalB, progress, (float)TransitionStrength, direction, kind, camera, material.Pigment)
        {
            DiagnosticCrossfade = DiagnosticCrossfade,
            Rows = BuildRows(scene, sceneWidth),
            RowsOpacity = (float)_pickerOpacity.Value,
            RowsOffsetY = (float)_pickerY.Value,
            RowInk = new SKColor(0xFF, 0xFD, 0xF6),
            Scrim = (float)_scrim.Value,
            PointerLight = _pointerScenePoint,
            PointerLightOpacity = (float)_pointerLight.Value,
        };
        _surface?.Present(frame);

        if (_fallbackImage is not null && !_previewing)
        {
            var shown = _data[scene.Selected];
            if (_fallbackImage.Tag as string != shown.Id)
            {
                _fallbackImage.Tag = shown.Id;
                _fallbackImage.Source = new BitmapImage(shown.ImageUri);
            }
        }

        // Globe follows the same pair and fraction; a destination without coordinates shows a neutral, unpinned globe.
        if (_globe is not null)
        {
            (double X, double Y, double Z)? orientation = null;
            if (_geo[scene.Selected] is { } gSel)
            {
                orientation = _geo[scene.FromIndex] is { } a && _geo[scene.ToIndex] is { } b && !IsReducedMotionEnabled ? GlobeRenderer.Slerp(a, b, scene.P) : gSel;
            }

            _globe.Present(new GlobeElement.GlobeFrame(orientation, material.Accent));
        }

        _shadow?.Present(new CardShadowElement.ShadowFrame(material.Shadow, (float)Rim.CornerRadius.TopLeft, 1f));
    }

    private IReadOnlyList<RowGlyphs> BuildRows(SceneFrame scene, float sceneWidth)
    {
        if (_surface is null || !_surface.Renderer.HasTypeface || _pickerOpacity.Value <= 0.001)
        {
            return Array.Empty<RowGlyphs>();
        }

        var n = _data.Count;
        var q = _picker.Position;
        var baseIndex = (long)Math.Floor(q);
        var list = new List<RowGlyphs>(CountryPicker.RowPool);
        var fontSize = (float)_countryFontSize;
        var available = sceneWidth - 48;
        for (var i = 0; i < CountryPicker.RowPool; i++)
        {
            var integer = baseIndex + i - (long)RowPoolHalf;
            var d = integer - q;
            var pose = CircularMath.Pose(d, _rowPitch);
            var a = Math.Abs(d);
            if (!pose.Visible || (n == 1 && a > .5))
            {
                continue;
            }

            var edge = _pickerWindow / 2 - Math.Abs(pose.Y);
            var mask = Math.Clamp(edge / (.1 * _pickerWindow), 0, 1);
            var opacity = pose.Opacity * mask;
            if (opacity <= .002)
            {
                continue;
            }

            var destination = _data[CircularMath.PositiveModulo(integer, n)];
            var size = FitFontSize(destination.Country, fontSize, available);
            var zFactor = 900d / (900d + 13d * a);
            var blur = IsReducedMotionEnabled ? 0f : (a > 1.3 ? (float)Math.Min((a - 1.3) * .45, 1) : 0f);
            var photoAlpha = IsReducedMotionEnabled ? 0f : (float)scene.PhotoInTypeOpacity(d);
            var photo = photoAlpha > .004f ? _cache?.Peek(destination.Id) : null;
            list.Add(new RowGlyphs(
                destination.Country,
                size,
                -.055f,
                (float)(_pickerCenterY + pose.Y),
                (float)(pose.Scale * zFactor),
                (float)(pose.Scale * zFactor * Math.Cos(pose.RotationX * Math.PI / 180d)),
                (float)opacity,
                blur,
                photo,
                destination.FocalPoint,
                photoAlpha));
        }

        return list;
    }

    /// <summary>Longer labels shrink to fit the row width instead of clipping (brief §2).</summary>
    private float FitFontSize(string text, float size, float available)
    {
        var key = (text, size, available);
        if (_fitCache.TryGetValue(key, out var fitted))
        {
            return fitted;
        }

        // Inter SemiBold averages ~0.58 em per glyph; tracking is −0.055 em.
        var estimate = text.Length * (.58f - .055f) * size;
        fitted = estimate <= available ? size : Math.Max(30f, size * available / estimate);
        if (_fitCache.Count > 64) _fitCache.Clear();
        _fitCache[key] = fitted;
        return fitted;
    }

    #endregion

    #region Diagnostics

    /// <summary>Frame-loop statistics for the evidence record: loop intervals and RenderOverride durations.</summary>
    public (int Frames, double P50Ms, double P95Ms, double MaxMs, double RenderP50Ms, double RenderP95Ms, double RenderMaxMs) GetFrameStatistics()
    {
        var intervals = _frameIntervals.OrderBy(v => v).ToArray();
        var renders = (_surface?.Renderer.RecentRenderMilliseconds ?? Array.Empty<double>()).OrderBy(v => v).ToArray();
        double P(double[] s, double p) => s.Length == 0 ? 0 : s[Math.Min(s.Length - 1, (int)Math.Floor(p * (s.Length - 1)))];
        return (intervals.Length, P(intervals, .5), P(intervals, .95), intervals.Length > 0 ? intervals[^1] : 0, P(renders, .5), P(renders, .95), renders.Length > 0 ? renders[^1] : 0);
    }

    public (int Count, long Bytes, int Retired) GetCacheStatistics() => _cache is null ? (0, 0, 0) : (_cache.Count, _cache.Bytes, _cache.RetiredCount);

    /// <summary>Current frame sample (q, pair, fraction, travel), for tests and the lab.</summary>
    public SceneFrame CurrentFrame => SceneFrame.Sample(_picker.Position, _data.Count);

#if DEBUG
    /// <summary>Harness only: freezes the picker at a fractional position so the position-driven scene can be captured mid-flight.</summary>
    public void HoldPositionForDiagnostics(double q)
    {
        ExitPreview();
        Pause();
        _picker.IsFrozen = true;
        _picker.SyncPreview(0, 0, 0);
        _picker.SetPositionForDiagnostics(q);
        PresentFrame();
        Wake();
    }
#endif

    #endregion

    private static SKColor ParseSkColor(string hex) => SKColor.TryParse(hex, out var c) ? c : SKColor.Parse("#244e51");
}
