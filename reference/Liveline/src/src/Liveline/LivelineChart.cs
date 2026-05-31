using Liveline.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Liveline;

/// <summary>
/// Real-time animated line chart driven by <see cref="CompositionTarget.Rendering"/>.
/// </summary>
public partial class LivelineChart : UserControl
{
    private readonly LivelineChartCanvas _canvas;
    private bool _animationAttached;

    public LivelineChart()
    {
        _canvas = new LivelineChartCanvas
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        HorizontalAlignment = HorizontalAlignment.Stretch;
        VerticalAlignment = VerticalAlignment.Stretch;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;
        Content = _canvas;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(IList<LivelinePoint>), typeof(LivelineChart),
            new PropertyMetadata(null, OnRenderPropertyChanged));

    public IList<LivelinePoint>? Data
    {
        get => (IList<LivelinePoint>?)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(LivelineChart),
            new PropertyMetadata(0.0, OnRenderPropertyChanged));

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.Register(nameof(Theme), typeof(LivelineTheme), typeof(LivelineChart),
            new PropertyMetadata(null, OnRenderPropertyChanged));

    public LivelineTheme? Theme
    {
        get => (LivelineTheme?)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly DependencyProperty ShowGridProperty =
        DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(LivelineChart),
            new PropertyMetadata(true, OnRenderPropertyChanged));

    public bool ShowGrid
    {
        get => (bool)GetValue(ShowGridProperty);
        set => SetValue(ShowGridProperty, value);
    }

    public static readonly DependencyProperty ShowBadgeProperty =
        DependencyProperty.Register(nameof(ShowBadge), typeof(bool), typeof(LivelineChart),
            new PropertyMetadata(true, OnRenderPropertyChanged));

    public bool ShowBadge
    {
        get => (bool)GetValue(ShowBadgeProperty);
        set => SetValue(ShowBadgeProperty, value);
    }

    public static readonly DependencyProperty ShowFillProperty =
        DependencyProperty.Register(nameof(ShowFill), typeof(bool), typeof(LivelineChart),
            new PropertyMetadata(true, OnRenderPropertyChanged));

    public bool ShowFill
    {
        get => (bool)GetValue(ShowFillProperty);
        set => SetValue(ShowFillProperty, value);
    }

    /// <summary>
    /// Controls the momentum indicator on the live dot. Defaults to
    /// <see cref="MomentumDirection.Auto"/> (direction derived from value changes).
    /// </summary>
    public static readonly DependencyProperty MomentumProperty =
        DependencyProperty.Register(nameof(Momentum), typeof(MomentumDirection), typeof(LivelineChart),
            new PropertyMetadata(MomentumDirection.Auto, OnRenderPropertyChanged));

    public MomentumDirection Momentum
    {
        get => (MomentumDirection)GetValue(MomentumProperty);
        set => SetValue(MomentumProperty, value);
    }

    public static readonly DependencyProperty LerpSpeedProperty =
        DependencyProperty.Register(nameof(LerpSpeed), typeof(double), typeof(LivelineChart),
            new PropertyMetadata(0.04, OnRenderPropertyChanged));

    public double LerpSpeed
    {
        get => (double)GetValue(LerpSpeedProperty);
        set => SetValue(LerpSpeedProperty, value);
    }

    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LivelineChart),
            new PropertyMetadata(false, OnRenderPropertyChanged));

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public static readonly DependencyProperty IsPausedProperty =
        DependencyProperty.Register(nameof(IsPaused), typeof(bool), typeof(LivelineChart),
            new PropertyMetadata(false, OnRenderPropertyChanged));

    public bool IsPaused
    {
        get => (bool)GetValue(IsPausedProperty);
        set => SetValue(IsPausedProperty, value);
    }

    private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LivelineChart chart)
            chart.PushState();
    }

    private void PushState()
    {
        _canvas.UpdateState(
            Data, Value, Theme,
            ShowGrid, ShowBadge, ShowFill,
            Momentum,
            LerpSpeed, IsPaused, IsLoading);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _canvas.EnsureResources();
        if (!_animationAttached)
        {
            CompositionTarget.Rendering += OnCompositionRendering;
            _animationAttached = true;
        }
        PushState();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_animationAttached)
        {
            CompositionTarget.Rendering -= OnCompositionRendering;
            _animationAttached = false;
        }
        _canvas.ReleaseResources();
    }

    private void OnCompositionRendering(object? sender, object e)
    {
        if (_canvas.TickAnimation())
            _canvas.Invalidate();
    }
}
