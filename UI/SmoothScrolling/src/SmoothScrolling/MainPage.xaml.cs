using System.Diagnostics;
using System.Numerics;
using Microsoft.UI.Xaml.Media;

namespace SmoothScrolling;

public sealed partial class MainPage : Page
{
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private ScrollViewer? _listScroller;
    private long _windowStart;
    private int _frames;
    private int _fresh;
    private double _lastOffset = double.NaN;

    public MainPage()
    {
        this.InitializeComponent();

        var items = FeedItem.Generate(1000);
        Repeater.Layout = new StackLayout();
        Repeater.ItemsSource = items;
        FeedListView.ItemsSource = items;

        Loaded += (_, _) => CompositionTarget.Rendering += OnRendering;
        Unloaded += (_, _) => CompositionTarget.Rendering -= OnRendering;
    }

    private bool IsScrollViewMode => FeedScrollView.Visibility == Visibility.Visible;

    private double CurrentOffset => IsScrollViewMode ? FeedScrollView.VerticalOffset : (_listScroller?.VerticalOffset ?? 0);

    private void OnRendering(object? sender, object e)
    {
        var offset = CurrentOffset;
        _frames++;
        if (offset != _lastOffset)
        {
            _fresh++;
            _lastOffset = offset;
        }

        var elapsed = _clock.ElapsedMilliseconds - _windowStart;
        if (elapsed >= 500)
        {
            var scale = 1000.0 / elapsed;
            FpsText.Text = $"{_frames * scale:F0}";
            FreshText.Text = $"{_fresh * scale:F0}";
            OffsetText.Text = $"{offset:N0}";
            _frames = 0;
            _fresh = 0;
            _windowStart = _clock.ElapsedMilliseconds;
        }
    }

    private void OnModeChanged(SelectorBar sender, SelectorBarSelectionChangedEventArgs args)
    {
        var scrollViewMode = sender.SelectedItem == ScrollViewItem;
        FeedScrollView.Visibility = scrollViewMode ? Visibility.Visible : Visibility.Collapsed;
        FeedListView.Visibility = scrollViewMode ? Visibility.Collapsed : Visibility.Visible;
        if (!scrollViewMode)
        {
            FeedListView.UpdateLayout();
            _listScroller ??= FindScrollViewer(FeedListView);
        }

        _lastOffset = double.NaN;
    }

    private void OnFling(object sender, RoutedEventArgs e)
    {
        if (IsScrollViewMode)
        {
            FeedScrollView.AddScrollVelocity(new Vector2(0, 15000), null);
        }
        else if (_listScroller is { } scroller)
        {
            scroller.ChangeView(null, scroller.VerticalOffset + 12000, null, false);
        }
    }

    private void OnBackToTop(object sender, RoutedEventArgs e)
    {
        if (IsScrollViewMode)
        {
            FeedScrollView.ScrollTo(0, 0, new ScrollingScrollOptions(ScrollingAnimationMode.Enabled));
        }
        else
        {
            _listScroller?.ChangeView(null, 0, null, false);
        }
    }

    private static ScrollViewer? FindScrollViewer(DependencyObject root)
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is ScrollViewer sv)
            {
                return sv;
            }

            if (FindScrollViewer(child) is { } found)
            {
                return found;
            }
        }

        return null;
    }
}
