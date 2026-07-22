using BrewHouse.Presentation.MockData;
using Microsoft.UI.Xaml.Media;

namespace BrewHouse.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the generated HomeModel VM as the page's DataContext, and a
        // child carrying its own explicit DataContext would shadow it — leaving every binding (and
        // every ElementName=Root command binding) stuck on the inert mock.
        this.DataContext = HomePageMockData.Data;

        // Drive the (display-only) PipsPager from the FlipView so it always shows the current page.
        // A TwoWay XAML binding between PipsPager.SelectedPageIndex and FlipView.SelectedIndex is
        // unreliable on Uno (the pager could show a middle pip selected on the last slide), so mirror
        // the FlipView's index onto the pager explicitly whenever the slide changes.
        HeroFlip.SelectionChanged += (_, _) =>
        {
            if (HeroPips.SelectedPageIndex != HeroFlip.SelectedIndex)
            {
                HeroPips.SelectedPageIndex = HeroFlip.SelectedIndex;
            }
        };

    }

    // Cached once found: the FlipView's template ScrollViewer ("ScrollingHost") doesn't change at
    // runtime, so paging shouldn't re-walk the visual tree on every click.
    private ScrollViewer? _heroScroll;

    // Custom flipper buttons page the carousel, wrapping at the ends. Setting FlipView.SelectedIndex
    // programmatically does NOT navigate on the Skia/WASM head, so we scroll the FlipView's own
    // horizontal ScrollViewer by one viewport — the same mechanism a swipe uses — which snaps to the
    // next slide and updates SelectedIndex (so the pager stays in sync via SelectionChanged).
    private void HeroPrev_Click(object sender, RoutedEventArgs e) => StepHero(-1);

    private void HeroNext_Click(object sender, RoutedEventArgs e) => StepHero(+1);

    private void StepHero(int delta)
    {
        var count = HeroFlip.Items.Count;
        if (count <= 0)
        {
            return;
        }

        _heroScroll ??= FindDescendant<ScrollViewer>(HeroFlip);
        // Guard a zero ViewportWidth (before first layout) so we don't ChangeView to a no-op 0.
        if (_heroScroll is not { ViewportWidth: > 0 } scroll)
        {
            return;
        }

        // SelectedIndex can be -1 before the FlipView has committed a selection; treat that as 0 so
        // "previous" wraps to the last slide rather than the second-to-last.
        var current = Math.Max(0, HeroFlip.SelectedIndex);
        var target = (current + delta + count) % count;
        scroll.ChangeView(target * scroll.ViewportWidth, null, null);
    }

    private static T? FindDescendant<T>(DependencyObject root) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(root);
        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(root, i);
            if (child is T match)
            {
                return match;
            }

            if (FindDescendant<T>(child) is { } nested)
            {
                return nested;
            }
        }

        return default;
    }
}
