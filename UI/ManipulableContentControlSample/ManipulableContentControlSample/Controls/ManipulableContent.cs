using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace ManipulableContentControlSample.Controls;

[TemplatePart(Name = "PART_Presenter", Type = typeof(ContentPresenter))]
public partial class ManipulableContent : ContentControl
{
    private ContentPresenter? _presenter;

    #region Dependency Properties
    public static readonly DependencyProperty IsActiveProperty =
    DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ManipulableContent), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty IsZoomAllowedProperty =
    DependencyProperty.Register(nameof(IsZoomAllowed), typeof(bool), typeof(ManipulableContent), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty ZoomLevelProperty =
    DependencyProperty.Register(nameof(ZoomLevel), typeof(double), typeof(ManipulableContent), new PropertyMetadata(1d));

    public static readonly DependencyProperty MinZoomLevelProperty =
    DependencyProperty.Register(nameof(MinZoomLevel), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.25d));

    public static readonly DependencyProperty MaxZoomLevelProperty =
    DependencyProperty.Register(nameof(MaxZoomLevel), typeof(double), typeof(ManipulableContent), new PropertyMetadata(200d));

    public static readonly DependencyProperty HorizontalZoomCenterProperty =
    DependencyProperty.Register(nameof(HorizontalZoomCenter), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalZoomCenterProperty =
    DependencyProperty.Register(nameof(VerticalZoomCenter), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty ScaleWheelRatioProperty =
    DependencyProperty.Register(nameof(ScaleWheelRatio), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.0006d));

    public static readonly DependencyProperty PanWheelRatioProperty =
    DependencyProperty.Register(nameof(PanWheelRatio), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.25d));

    public static readonly DependencyProperty IsPanAllowedProperty =
    DependencyProperty.Register(nameof(IsPanAllowed), typeof(bool), typeof(ManipulableContent), new PropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty HorizontalOffsetProperty =
    DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.0d));

    public static readonly DependencyProperty VerticalOffsetProperty =
    DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(ManipulableContent), new PropertyMetadata(0.0d));

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
    #endregion

    private void RegisterPropertyHandlers()
    {
        // Register for property changed events.
        RegisterPropertyChangedCallback(ZoomLevelProperty, CoerceZoomLevel);
        RegisterPropertyChangedCallback(MinZoomLevelProperty, CoerceZoomLevel);
        RegisterPropertyChangedCallback(MaxZoomLevelProperty, CoerceZoomLevel);
    }

    private void CoerceZoomLevel(DependencyObject sender, DependencyProperty dp)
    {
        var control = (ManipulableContent)sender;
        control.ZoomLevel = Math.Clamp(control.ZoomLevel, control.MinZoomLevel, control.MaxZoomLevel);
    }

    public ManipulableContent()
    {
        DefaultStyleKey = typeof(ManipulableContent);

        RegisterPropertyHandlers();
    }

    protected override void OnApplyTemplate()
    {
        _presenter = GetTemplateChild("PART_Presenter") as ContentPresenter;
        RegisterPointerHandlers();
    }
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

    private bool IsAllowedToWork => (IsEnabled && IsActive && _presenter is not null);

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

        var pointerPoint = e.GetCurrentPoint(_presenter);
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

            var hzc = HorizontalZoomCenter;
            var vzc = VerticalZoomCenter;
            var newPointerPosX = ((pointerPosition.X - hzc) * changeRatio) + hzc;
            var newPointerPosY = ((pointerPosition.Y - vzc) * changeRatio) + vzc;

            ZoomLevel *= changeRatio;
            HorizontalOffset += newPointerPosX - pointerPosition.X;
            VerticalOffset += newPointerPosY - pointerPosition.Y;
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
    }

    public void ResetOffset()
    {
        HorizontalOffset = 0;
        VerticalOffset = 0;
    }
}
