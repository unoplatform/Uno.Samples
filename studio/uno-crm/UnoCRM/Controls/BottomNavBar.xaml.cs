using Microsoft.UI.Xaml.Media.Animation;

namespace UnoCRM.Controls;

/// <summary>
/// Floating bottom navigation bar shared by the mobile layouts of every page.
/// Set <see cref="ActiveTab"/> ("Home" | "Pipeline" | "Leads" | "Contacts") to highlight
/// the current page; the control navigates the hosting <see cref="Frame"/> on tap.
/// <para>
/// The bar slides out of view while the page scrolls down and slides back in on scroll up.
/// It watches the <see cref="ScrollViewer"/> assigned to <see cref="ScrollHost"/>, or — when
/// that is unset — the first <see cref="ScrollViewer"/> sibling of the bar's parent. If a page
/// nests its scroller inside another element, set <see cref="ScrollHost"/> to keep auto-hide working.
/// </para>
/// </summary>
public sealed partial class BottomNavBar : UserControl
{
    private ScrollViewer? _scrollViewer;
    private double _lastOffset;
    private bool _hidden;

    public BottomNavBar()
    {
        this.InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public static readonly DependencyProperty ActiveTabProperty =
        DependencyProperty.Register(
            nameof(ActiveTab),
            typeof(string),
            typeof(BottomNavBar),
            new PropertyMetadata("Home", (d, _) => ((BottomNavBar)d).UpdateActiveState()));

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    /// <summary>
    /// Optional explicit scroll host driving the auto-hide. When unset, the bar falls back to
    /// the first <see cref="ScrollViewer"/> sibling of its parent. ElementName-bindable.
    /// </summary>
    public static readonly DependencyProperty ScrollHostProperty =
        DependencyProperty.Register(
            nameof(ScrollHost),
            typeof(ScrollViewer),
            typeof(BottomNavBar),
            new PropertyMetadata(null));

    public ScrollViewer? ScrollHost
    {
        get => (ScrollViewer?)GetValue(ScrollHostProperty);
        set => SetValue(ScrollHostProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateActiveState();
        HookHostScrollViewer();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged -= OnHostViewChanged;
            _scrollViewer = null;
        }
    }

    private void UpdateActiveState() =>
        VisualStateManager.GoToState(this, string.IsNullOrEmpty(ActiveTab) ? "Home" : ActiveTab, false);

    /// <summary>
    /// The bar is a direct child of the page's mobile root grid, alongside the page's
    /// main vertical <see cref="ScrollViewer"/>. Find that sibling and watch it scroll
    /// so the auto-hide works without every page having to wire it up.
    /// </summary>
    private void HookHostScrollViewer()
    {
        if (_scrollViewer is not null)
        {
            return;
        }

        // Explicit host wins; otherwise fall back to the first ScrollViewer sibling.
        var scrollViewer = ScrollHost ?? FindSiblingScrollViewer();
        if (scrollViewer is null)
        {
            return;
        }

        _scrollViewer = scrollViewer;
        _lastOffset = scrollViewer.VerticalOffset;
        scrollViewer.ViewChanged += OnHostViewChanged;
    }

    private ScrollViewer? FindSiblingScrollViewer()
    {
        if (VisualTreeHelper.GetParent(this) is not DependencyObject parent)
        {
            return null;
        }

        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < count; i++)
        {
            if (VisualTreeHelper.GetChild(parent, i) is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }
        }

        return null;
    }

    private void OnHostViewChanged(object? sender, ScrollViewerViewChangedEventArgs e)
    {
        if (_scrollViewer is null)
        {
            return;
        }

        var offset = _scrollViewer.VerticalOffset;
        var delta = offset - _lastOffset;

        if (offset <= 8)
        {
            // Always reveal the bar near the top of the page.
            ShowBar();
        }
        else if (delta > 6)
        {
            // Scrolling down — get the bar out of the way.
            HideBar();
        }
        else if (delta < -6)
        {
            // Scrolling up — bring it back.
            ShowBar();
        }

        _lastOffset = offset;
    }

    private void ShowBar()
    {
        if (!_hidden)
        {
            return;
        }

        _hidden = false;
        AnimateBarTo(0);
    }

    private void HideBar()
    {
        if (_hidden)
        {
            return;
        }

        _hidden = true;
        var height = BarRoot.ActualHeight > 0 ? BarRoot.ActualHeight : 120;
        AnimateBarTo(height + BarRoot.Margin.Bottom + 40);
    }

    private void AnimateBarTo(double y)
    {
        var animation = new DoubleAnimation
        {
            To = y,
            Duration = new Duration(TimeSpan.FromMilliseconds(280)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            EnableDependentAnimation = true,
        };

        Storyboard.SetTarget(animation, BarTransform);
        Storyboard.SetTargetProperty(animation, "Y");

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Begin();
    }

    private void OnTabClick(object sender, RoutedEventArgs e)
    {
        var tab = ((FrameworkElement)sender).Tag as string;
        if (tab is null || tab == ActiveTab)
        {
            // Tapping the current tab is a no-op (avoids pushing a duplicate onto the back stack).
            return;
        }

        var target = tab switch
        {
            "Home" => typeof(MainPage),
            "Pipeline" => typeof(PipelinePage),
            "Leads" => typeof(LeadsPage),
            "Contacts" => typeof(ContactsPage),
            _ => null,
        };

        if (target is null)
        {
            return;
        }

        // Tab navigation shouldn't grow a back stack — otherwise Android's Back button
        // replays the whole tab history. Clear it after a successful navigation.
        var frame = FindFrame();
        if (frame is not null && frame.Navigate(target))
        {
            frame.BackStack.Clear();
        }
    }

    private Frame? FindFrame()
    {
        DependencyObject? current = this;
        while (current is not null)
        {
            if (current is Frame frame)
            {
                return frame;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
