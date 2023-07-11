using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Windows.System;
using Microsoft.UI.Xaml.Input;

namespace ChatUI.Behaviors;

public static class ReversedPointerWheelBehavior
{
	public static readonly DependencyProperty IsReversedProperty = DependencyProperty.RegisterAttached(
		"IsReversed",
		typeof(bool),
		typeof(FrameworkElement),
		new PropertyMetadata(default(bool), OnIsReversedChanged));

	public static bool GetIsReversed(FrameworkElement element)
		=> (bool)element.GetValue(IsReversedProperty);

	public static void SetIsReversed(FrameworkElement element, bool value)
		=> element.SetValue(IsReversedProperty, value);

	private static void OnIsReversedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
					SetIsReversed(scp, GetIsReversed(sv));
				}
			}
		}
	}

	private static void OnWheelChanged(object sender, PointerRoutedEventArgs e)
	{
		if (sender is not ScrollContentPresenter { ScrollOwner: ScrollViewer sv } scp || !GetIsReversed(scp))
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
