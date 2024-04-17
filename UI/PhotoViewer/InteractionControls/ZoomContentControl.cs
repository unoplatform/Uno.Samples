using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace InteractionControls;

[TemplatePart(Name = "PART_Grid", Type = typeof(Grid))]
[TemplatePart(Name = "PART_Presenter", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_scrollV", Type = typeof(ScrollBar))]
[TemplatePart(Name = "PART_scrollH", Type = typeof(ScrollBar))]
public partial class ZoomContentControl : ContentControl
{
    private Grid? _grid;
    private ContentPresenter? _presenter;
    private ScrollBar? _scrollV;
    private ScrollBar? _scrollH;
    private Canvas _canvas = new Canvas();

    private bool IsAllowedToWork => (IsEnabled && IsActive && _presenter is not null);

    public bool ResetWhenNotActive { get; set; } = true;

    #region Dependency Properties
    public static readonly DependencyProperty IsActiveProperty =
    DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty IsZoomAllowedProperty =
    DependencyProperty.Register(nameof(IsZoomAllowed), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty ZoomLevelProperty =
    DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(1d));

    public static readonly DependencyProperty MinZoomLevelProperty =
    DependencyProperty.Register(nameof(MinZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.25d));

    public static readonly DependencyProperty MaxZoomLevelProperty =
    DependencyProperty.Register(nameof(MaxZoomLevel), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(200d));

    public static readonly DependencyProperty HorizontalZoomCenterProperty =
    DependencyProperty.Register(nameof(HorizontalZoomCenter), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalZoomCenterProperty =
    DependencyProperty.Register(nameof(VerticalZoomCenter), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty ScaleWheelRatioProperty =
    DependencyProperty.Register(nameof(ScaleWheelRatio), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0006d));

    public static readonly DependencyProperty PanWheelRatioProperty =
    DependencyProperty.Register(nameof(PanWheelRatio), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.25d));

    public static readonly DependencyProperty IsPanAllowedProperty =
    DependencyProperty.Register(nameof(IsPanAllowed), typeof(bool), typeof(ZoomContentControl), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty HorizontalOffsetProperty =
    DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalOffsetProperty =
    DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalMaxScrollProperty =
    DependencyProperty.Register(nameof(VerticalMaxScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalMinScrollProperty =
    DependencyProperty.Register(nameof(VerticalMinScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty HorizontalMaxScrollProperty =
    DependencyProperty.Register(nameof(HorizontalMaxScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty HorizontalMinScrollProperty =
    DependencyProperty.Register(nameof(HorizontalMinScroll), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty HorizontalScrollValueProperty =
    DependencyProperty.Register(nameof(HorizontalScrollValue), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalScrollValueProperty =
    DependencyProperty.Register(nameof(VerticalScrollValue), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty ViewPortWidthProperty =
    DependencyProperty.Register(nameof(ViewPortWidth), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty ViewPortHeightProperty =
    DependencyProperty.Register(nameof(ViewPortHeight), typeof(double), typeof(ZoomContentControl), new PropertyMetadata(0.0d));

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsZoomAllowed
    {
        get => (bool)GetValue(IsZoomAllowedProperty);
        set => SetValue(IsZoomAllowedProperty, value);
    }

    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    public double MinZoomLevel
    {
        get => (double)GetValue(MinZoomLevelProperty);
        set => SetValue(MinZoomLevelProperty, value);
    }

    public double MaxZoomLevel
    {
        get => (double)GetValue(MaxZoomLevelProperty);
        set => SetValue(MaxZoomLevelProperty, value);
    }

    public double HorizontalZoomCenter
    {
        get => (double)GetValue(HorizontalZoomCenterProperty);
        set => SetValue(HorizontalZoomCenterProperty, value);
    }

    public double VerticalZoomCenter
    {
        get => (double)GetValue(VerticalZoomCenterProperty);
        set => SetValue(VerticalZoomCenterProperty, value);
    }

    public double ScaleWheelRatio
    {
        get => (double)GetValue(ScaleWheelRatioProperty);
        set => SetValue(ScaleWheelRatioProperty, value);
    }

    public double PanWheelRatio
    {
        get => (double)GetValue(PanWheelRatioProperty);
        set => SetValue(PanWheelRatioProperty, value);
    }

    public bool IsPanAllowed
    {
        get => (bool)GetValue(IsPanAllowedProperty);
        set => SetValue(IsPanAllowedProperty, value);
    }

    public double HorizontalOffset
    {
        get => (double)GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    public double VerticalOffset
    {
        get => (double)GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    public double VerticalMaxScroll
    {
        get => (double)GetValue(VerticalMaxScrollProperty);
        set => SetValue(VerticalMaxScrollProperty, value);
    }

    public double VerticalMinScroll
    {
        get => (double)GetValue(VerticalMinScrollProperty);
        set => SetValue(VerticalMinScrollProperty, value);
    }

    public double HorizontalMaxScroll
    {
        get => (double)GetValue(HorizontalMaxScrollProperty);
        set => SetValue(HorizontalMaxScrollProperty, value);
    }

    public double HorizontalMinScroll
    {
        get => (double)GetValue(HorizontalMinScrollProperty);
        set => SetValue(HorizontalMinScrollProperty, value);
    }

    public double HorizontalScrollValue
    {
        get => (double)GetValue(HorizontalScrollValueProperty);
        set => SetValue(HorizontalScrollValueProperty, value);
    }

    public double VerticalScrollValue
    {
        get => (double)GetValue(VerticalScrollValueProperty);
        set => SetValue(VerticalScrollValueProperty, value);
    }

    public double ViewPortHeight
    {
        get => (double)GetValue(ViewPortHeightProperty);
        set => SetValue(ViewPortHeightProperty, value);
    }

    public double ViewPortWidth
    {
        get => (double)GetValue(ViewPortWidthProperty);
        set => SetValue(ViewPortWidthProperty, value);
    }

    #endregion

    private void RegisterPropertyHandlers()
    {
        // Register for property changed events.
        RegisterPropertyChangedCallback(ZoomLevelProperty, CoerceZoomLevel);
        RegisterPropertyChangedCallback(MinZoomLevelProperty, CoerceZoomLevel);
        RegisterPropertyChangedCallback(MaxZoomLevelProperty, CoerceZoomLevel);

        RegisterPropertyChangedCallback(ZoomLevelProperty, (s, e) => UpdateScrollLimits());

        RegisterPropertyChangedCallback(HorizontalOffsetProperty, UpdateVerticalScrollBarValue);
        RegisterPropertyChangedCallback(VerticalOffsetProperty, UpdateHorizontalScrollBarValue);

        RegisterPropertyChangedCallback(IsActiveProperty, IsActiveChanged);
    }

    //Slide move is always on the opposite direction of the drag
    private void UpdateVerticalScrollBarValue(DependencyObject sender, DependencyProperty dp) => VerticalScrollValue = -1 * VerticalOffset;

    //Slide move is always on the opposite direction of the drag
    private void UpdateHorizontalScrollBarValue(DependencyObject sender, DependencyProperty dp) => HorizontalScrollValue = -1 * HorizontalOffset;

    private void IsActiveChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (ResetWhenNotActive && !IsActive)
        {
            ResetOffset();
            ResetZoom();
        }
        if (_scrollH is not null)
        {
            _scrollH.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
        }
        if (_scrollV is not null)
        {
            _scrollV.Visibility = IsActive ? Visibility.Visible : Visibility.Collapsed;
        }

        if (_grid is not null)
        {
            if (IsActive)
            {
                _grid.Children.Remove(_presenter);
                _grid.Children.Add(_canvas);
                _canvas.Children.Add(_presenter);
                Grid.SetRow(_canvas, 0);
                Grid.SetColumn(_canvas, 0);
            }
            else
            {
                _canvas.Children.Remove(_presenter);
                _grid.Children.Remove(_canvas);
                _grid.Children.Add(_presenter);
                Grid.SetRow(_presenter, 0);
                Grid.SetColumn(_presenter, 0);
            }
        }
    }

    private void UpdateScrollLimits()
    {
        HorizontalMaxScroll = this.ActualWidth * ZoomLevel;
        VerticalMaxScroll = this.ActualHeight * ZoomLevel;

        HorizontalMinScroll = -1 * HorizontalMaxScroll;
        VerticalMinScroll = -1 * VerticalMaxScroll;
    }

    private void CoerceZoomLevel(DependencyObject sender, DependencyProperty dp)
    {
        var control = (ZoomContentControl)sender;
        control.ZoomLevel = Math.Clamp(control.ZoomLevel, control.MinZoomLevel, control.MaxZoomLevel);
    }

    public ZoomContentControl()
    {
        DefaultStyleKey = typeof(ZoomContentControl);

        this.Loaded += (s, e) =>
        {
            if (_presenter?.Content is FrameworkElement { } fe)
            {
                fe.LayoutUpdated += (s, e) =>
                {
                    ViewPortWidth = fe.ActualWidth;
                    ViewPortHeight = fe.ActualHeight;

                    UpdateScrollLimits();
                };
            }
        };
        RegisterPropertyHandlers();
    }

    protected override void OnApplyTemplate()
    {
        _grid = GetTemplateChild("PART_Grid") as Grid;
        _presenter = GetTemplateChild("PART_Presenter") as ContentPresenter;
        _scrollV = GetTemplateChild("PART_scrollV") as ScrollBar;
        _scrollH = GetTemplateChild("PART_scrollH") as ScrollBar;

        RegisterToControlEvents();

        ResetOffset();
        ResetZoom();
        RegisterPointerHandlers();
    }
    #region ScrollBars Events

    private void RegisterToControlEvents()
    {
        //due to templatebinding there's no TwoWay mode. We need to manually update the values
        if (_scrollV is not null)
        {
            _scrollV.Scroll += ScrollV_Scroll;
        }

        if (_scrollH is not null)
        {
            _scrollH.Scroll += ScrollH_Scroll;
        }
    }

    //TemplateBinding doesn't support TwoWay mode. We need to manually update the values
    private void ScrollV_Scroll(object sender, ScrollEventArgs e) => VerticalOffset = -1 * e.NewValue;

    private void ScrollH_Scroll(object sender, ScrollEventArgs e) => HorizontalOffset = -1 * e.NewValue;

    #endregion

    private uint _capturedPointerId;
    private Point _referencePosition;

    private void RegisterPointerHandlers()
    {
        // Register for pointer events.
        PointerPressed -= OnPointerPressed;
        PointerReleased -= OnPointerReleased;
        PointerMoved -= OnPointerMoved;
        PointerWheelChanged -= OnPointerWheelChanged;

        PointerPressed += OnPointerPressed;
        PointerReleased += OnPointerReleased;
        PointerMoved += OnPointerMoved;
        PointerWheelChanged += OnPointerWheelChanged;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!IsAllowedToWork)
        {
            return; // Don't handle the event when the control is disabled.
        }

        var pointerPoint = e.GetCurrentPoint(this);
        var pointerProperties = pointerPoint.Properties;

        // If the middle button of a mouse is pressed, then we want to handle the event.
        if (pointerProperties.IsMiddleButtonPressed
            && pointerPoint.PointerDeviceType == PointerDeviceType.Mouse
            && IsPanAllowed)
        {
            e.Handled = true;

            // Capture the pointer so that we can track its movement even if it leaves the bounds of the control.
            var pointer = e.Pointer;
            var captured = CapturePointer(pointer);
            if (captured)
            {
                _capturedPointerId = pointer.PointerId;
                _referencePosition = pointerPoint.Position;
            }
        }
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        ReleasePointerCaptures();
        _capturedPointerId = default;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (IsAllowedToWork && _capturedPointerId > 0)
        {
            // If the pointer is captured, then we want to handle the event.
            e.Handled = true;

            var pointerPoint = e.GetCurrentPoint(this);

            // Adjust the offsets based on the pointer's movement.
            var position = pointerPoint.Position;
            var deltaX = position.X - _referencePosition.X;
            var deltaY = position.Y - _referencePosition.Y;
            HorizontalOffset += deltaX;
            VerticalOffset += deltaY;

            // Update the starting position for next time.
            _referencePosition = position;
        }
    }

    private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (!IsAllowedToWork)
        {
            return; // Don't handle the event when the control is disabled.
        }

        var pointerPoint = e.GetCurrentPoint(this);
        var pointerProperties = pointerPoint.Properties;
        var pointerPosition = pointerPoint.Position;

        var changeRatio = GetZoomDelta(pointerProperties);

        //Horizontal Scroll
        if (e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Shift))
        {
            HorizontalOffset += GetPanDelta(pointerProperties);
            return;
        }

        //Zoom In/Out
        if (pointerPoint.PointerDeviceType == PointerDeviceType.Mouse &&
            e.KeyModifiers.HasFlag(Windows.System.VirtualKeyModifiers.Control) &&
            IsZoomAllowed)
        {
            e.Handled = true;

            ZoomLevel *= changeRatio;
            HorizontalZoomCenter = pointerPosition.X;
            VerticalZoomCenter = pointerPosition.Y;
            return;
        }

        //Vertical Scroll
        VerticalOffset += GetPanDelta(pointerProperties);
    }

    private double GetZoomDelta(PointerPointProperties pointerProperties)
    {
        var delta = pointerProperties.MouseWheelDelta * ScaleWheelRatio;
        return 1 + delta;
    }

    private double GetPanDelta(PointerPointProperties pointerProperties)
    {
        var delta = pointerProperties.MouseWheelDelta * PanWheelRatio;
        return delta;
    }

    public void ResetZoom()
    {
        ZoomLevel = 1;
        HorizontalZoomCenter = 0;
        VerticalZoomCenter = 0;
    }

    public void ResetOffset()
    {
        HorizontalOffset = 0;
        VerticalOffset = 0;
    }

    public void Centralize()
    {
        HorizontalOffset = (ActualWidth / 2) - (ViewPortWidth / 2);
        VerticalOffset = (ActualHeight / 2) - (ViewPortHeight / 2);
    }
}
