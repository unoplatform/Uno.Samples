using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace EnterpriseDashboard.Observatory.Animation;

// Attached behavior — fires when the target enters the parent ScrollViewer viewport.
// One-shot per ResetKey value; bump ResetKey to re-arm.
public static class ScrollTriggerBehavior
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(ScrollTriggerBehavior),
        new PropertyMetadata(false, OnIsEnabledChanged));

    public static void SetIsEnabled(DependencyObject d, bool value) => d.SetValue(IsEnabledProperty, value);
    public static bool GetIsEnabled(DependencyObject d) => (bool)d.GetValue(IsEnabledProperty);

    public static readonly DependencyProperty ThresholdProperty = DependencyProperty.RegisterAttached(
        "Threshold", typeof(double), typeof(ScrollTriggerBehavior),
        new PropertyMetadata(0.25));

    public static void SetThreshold(DependencyObject d, double value) => d.SetValue(ThresholdProperty, value);
    public static double GetThreshold(DependencyObject d) => (double)d.GetValue(ThresholdProperty);

    public static readonly DependencyProperty ResetKeyProperty = DependencyProperty.RegisterAttached(
        "ResetKey", typeof(int), typeof(ScrollTriggerBehavior),
        new PropertyMetadata(0, OnResetKeyChanged));

    public static void SetResetKey(DependencyObject d, int value) => d.SetValue(ResetKeyProperty, value);
    public static int GetResetKey(DependencyObject d) => (int)d.GetValue(ResetKeyProperty);

    private static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
        "State", typeof(TriggerState), typeof(ScrollTriggerBehavior), new PropertyMetadata(null));

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement fe) return;
        if ((bool)e.NewValue)
        {
            var state = new TriggerState(fe);
            fe.SetValue(StateProperty, state);
            fe.Loaded += state.OnLoaded;
            fe.Unloaded += state.OnUnloaded;
            if (fe.IsLoaded) state.Attach();
        }
        else if (fe.GetValue(StateProperty) is TriggerState s)
        {
            fe.Loaded -= s.OnLoaded;
            fe.Unloaded -= s.OnUnloaded;
            s.Detach();
            fe.ClearValue(StateProperty);
        }
    }

    private static void OnResetKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d.GetValue(StateProperty) is TriggerState s) s.Rearm();
    }

    private sealed class TriggerState
    {
        private readonly FrameworkElement _element;
        private ScrollViewer? _scroll;
        private bool _fired;

        public TriggerState(FrameworkElement e) { _element = e; }

        public void OnLoaded(object? s, RoutedEventArgs e) => Attach();
        public void OnUnloaded(object? s, RoutedEventArgs e) => Detach();

        public void Attach()
        {
            _scroll = FindAncestor<ScrollViewer>(_element);
            if (_scroll == null) return;
            _scroll.ViewChanged += OnViewChanged;
            // ViewChanged only fires on actual scrolling — when initial content fits the
            // viewport, no scroll happens. So a Check() that returned early because the
            // ScrollViewer hadn't been measured yet (ViewportWidth/Height = 0 at Loaded
            // time) would never re-fire. The scroller's SizeChanged covers its later
            // measure pass; the dispatcher re-queue covers the same pass for the
            // chart element without subscribing to element.SizeChanged (which can
            // re-enter Check during a Rearm→Reset→BuildScene rebuild and race Play).
            _scroll.SizeChanged += OnViewSizeChanged;
            Check();
            _element.DispatcherQueue?.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, Check);
        }

        public void Detach()
        {
            if (_scroll != null)
            {
                _scroll.ViewChanged -= OnViewChanged;
                _scroll.SizeChanged -= OnViewSizeChanged;
            }
            _scroll = null;
        }

        public void Rearm()
        {
            _fired = false;
            if (_element is IAnimatableChart chart) chart.Reset();
            Check();
        }

        private void OnViewChanged(object? s, ScrollViewerViewChangedEventArgs e) => Check();
        private void OnViewSizeChanged(object? s, SizeChangedEventArgs e) => Check();

        private void Check()
        {
            if (_fired || _scroll == null || _element.ActualHeight <= 0) return;

            try
            {
                var transform = _element.TransformToVisual(_scroll);
                var pos = transform.TransformPoint(new Point(0, 0));
                var elemRect = new Rect(pos, new Size(_element.ActualWidth, _element.ActualHeight));
                var vp = new Rect(0, 0, _scroll.ViewportWidth, _scroll.ViewportHeight);
                if (vp.Width <= 0 || vp.Height <= 0) return;

                var ix = Math.Max(0, Math.Min(elemRect.Right, vp.Right) - Math.Max(elemRect.Left, vp.Left));
                var iy = Math.Max(0, Math.Min(elemRect.Bottom, vp.Bottom) - Math.Max(elemRect.Top, vp.Top));
                var visible = (ix * iy) / Math.Max(1, elemRect.Width * elemRect.Height);

                if (visible >= GetThreshold(_element))
                {
                    _fired = true;
                    if (_element is IAnimatableChart chart) chart.Play();
                }
            }
            catch
            {
                // Layout not ready yet — try again on next view change.
            }
        }
    }

    private static T? FindAncestor<T>(DependencyObject? d) where T : class
    {
        while (d != null)
        {
            if (d is T t) return t;
            d = VisualTreeHelper.GetParent(d);
        }
        return null;
    }
}
