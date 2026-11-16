using Microsoft.UI.Xaml.Input;
using Windows.System;

namespace ChatUI.Behaviors;

public static class ReversedPointerWheel
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled",
        typeof(bool),
        typeof(ReversedPointerWheel),
        new PropertyMetadata(default(bool), OnIsEnabledChanged));

    public static bool GetIsEnabled(FrameworkElement element)
        => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(FrameworkElement element, bool value)
        => element.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ScrollContentPresenter scp)
        {
#if HAS_UNO
                // We do have native support for reversed PointerWheel in Uno.
                Uno.UI.Xaml.Controls.ScrollContentPresenter.SetIsPointerWheelReversed(scp, (bool)e.NewValue);
#else
            scp.PointerWheelChanged -= OnWheelChanged;
            if (e.NewValue is true)
            {
                scp.PointerWheelChanged += OnWheelChanged;
            }
#endif
        }
        else if (d is FrameworkElement sv)
        {
            sv.Loaded -= PropagateValueToScp;
            if (e.NewValue != DependencyProperty.UnsetValue)
            {
                sv.Loaded += PropagateValueToScp;
                PropagateValueToScp(sv, null);
            }

            static void PropagateValueToScp(object sender, RoutedEventArgs? _)
            {
                if (sender is FrameworkElement sv && TryFindFirstChild(sv, 10, out ScrollContentPresenter scp))
                {
                    SetIsEnabled(scp, GetIsEnabled(sv));
                }
            }
        }
    }

    private static void OnWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        if (sender is not ScrollContentPresenter { ScrollOwner: ScrollViewer sv } scp || !GetIsEnabled(scp))
        {
            if (sender is FrameworkElement fe)
            {
                fe.PointerWheelChanged -= OnWheelChanged;
            }

            return;
        }

        var properties = e.GetCurrentPoint(null).Properties;
        if (e.KeyModifiers == VirtualKeyModifiers.Control)
        {
            // Zoom, do nothing.
        }
        else if (!scp.CanVerticallyScroll || properties.IsHorizontalMouseWheel || e.KeyModifiers == VirtualKeyModifiers.Shift)
        {
            if (scp.CanHorizontallyScroll)
            {
                sv.ChangeView(
                    horizontalOffset: sv.HorizontalOffset + properties.MouseWheelDelta,
                    verticalOffset: null,
                    zoomFactor: null,
                    disableAnimation: false);

                e.Handled = true;
            }
        }
        else
        {
            sv.ChangeView(
                horizontalOffset: null,
                verticalOffset: sv.VerticalOffset + properties.MouseWheelDelta,
                zoomFactor: null,
                disableAnimation: false);

            e.Handled = true;
        }
    }

    private static bool TryFindFirstChild<T>(DependencyObject element, uint limit, /*[NotNullWhen(true)]*/ out T result)
    {
        // Finds the first child of type T in the visual tree of element.
        // This is a workaround for the fact that ScrollViewer doesn't expose its ScrollContentPresenter.

        if (element is T t)
        {
            result = t;
            return true;
        }

        if (limit is not 0)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                if (TryFindFirstChild(VisualTreeHelper.GetChild(element, i), limit - 1, out result))
                {
                    return true;
                }
            }
        }

        result = default;
        return false;
    }
}
