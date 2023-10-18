using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace ChatUI.Behaviors;

public sealed partial class InputPaneExtensions
{
	private static FrameworkElement _element;

	/// <summary>
	/// Get value of IsPanIntoView
	/// </summary>
	/// <param name="obj">FrameworkElement</param>
	/// <returns>Value of IsPanIntoView</returns>
	public static bool GetIsPanIntoView(DependencyObject obj)
	{
		return (bool)obj.GetValue(IsPanIntoViewProperty);
	}

	/// <summary>
	/// Set value of IsPanIntoView
	/// </summary>
	/// <param name="obj">FrameworkElement</param>
	/// <param name="value">New value of IsPanIntoView</param>
	public static void SetIsPanIntoView(DependencyObject obj, bool value)
	{
		obj.SetValue(IsPanIntoViewProperty, value);
	}

	/// <summary>
	/// Property for IsPanIntoView, which only triggers the first time the FrameworkElement is displayed 
	/// </summary>
	public static readonly DependencyProperty IsPanIntoViewProperty =
		DependencyProperty.RegisterAttached("IsPanIntoView", typeof(bool), typeof(InputPaneExtensions), new PropertyMetadata(false, IsPanIntoViewChanged));


	/// <summary>
	/// Event raised when IsPanIntoViewChanged is changed. It only triggers once
	/// </summary>
	/// <param name="d">FrameworkElement</param>
	/// <param name="e">Event arguments</param>
	private static void IsPanIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		_element = d as FrameworkElement;

		InputPane.GetForCurrentView().Showing -= OnKeyboardShowing;
		InputPane.GetForCurrentView().Showing += OnKeyboardShowing;
		InputPane.GetForCurrentView().Hiding -= OnKeyboardHidding;
		InputPane.GetForCurrentView().Hiding += OnKeyboardHidding;
	}

	/// <summary>
	/// Event raised when the keyboard is showing
	/// </summary>
	/// <param name="sender">InputPane</param>
	/// <param name="args">InputPane Event Arguments</param>
	private static void OnKeyboardShowing(InputPane sender, InputPaneVisibilityEventArgs args)
	{
#if __ANDROID__
		var navigationBarHeightId = Android.Content.Res.Resources.System.GetIdentifier("navigation_bar_height", "dimen", "android");
		var navigationBarHeight = Android.Content.Res.Resources.System.GetDimensionPixelSize(navigationBarHeightId) / Android.Content.Res.Resources.System.DisplayMetrics.Density;

		var statusBarOffset = Windows.UI.ViewManagement.StatusBar.GetForCurrentView()?.OccludedRect.Height ?? 0d;

		var translateTo = -args.OccludedRect.Height + statusBarOffset;
#else
		var translateTo = -args.OccludedRect.Height;
#endif

		_element.Margin = new Thickness(_element.Margin.Left, _element.Margin.Top, _element.Margin.Right, (int)-translateTo);
	}

	/// <summary>
	/// Event raised when the keyboard is hidding
	/// </summary>
	/// <param name="sender">InputPane</param>
	/// <param name="args">InputPane Event Arguments</param>
	private static void OnKeyboardHidding(InputPane sender, InputPaneVisibilityEventArgs args)
	{
		var translateTo = args.OccludedRect.Height;

		_element.Margin = new Thickness(_element.Margin.Left, _element.Margin.Top, _element.Margin.Right, (int)translateTo);
	}
}
