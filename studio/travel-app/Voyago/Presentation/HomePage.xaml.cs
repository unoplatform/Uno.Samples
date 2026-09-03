using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace Voyago.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from IDiscoveryService and is
        // rendered through FeedViews, so its design-time data must be the feed-shaped mock that
        // returns the generated ViewModel (see Presentation/MockData) — and a hand-built generated VM
        // must never be seeded from a page constructor: it has no live SourceContext, so its feeds
        // never pump, and it would shadow the VM Navigation injects at runtime. The named previews
        // supply it in XAML instead, which is the only safe place for it.

    }

    // Mirror the hero carousel onto the (display-only) pager on the one event that is reliable on
    // Uno — the TwoWay PipsPager<->FlipView binding is not (lesson 49).
    private void OnHeroSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HeroPager.SelectedPageIndex != Hero.SelectedIndex)
        {
            HeroPager.SelectedPageIndex = Hero.SelectedIndex;
        }
    }

    // Cached once found: the FlipView's template ScrollViewer ("ScrollingHost") doesn't change at
    // runtime, so paging shouldn't re-walk the visual tree on every click.
    private ScrollViewer? _heroScroll;

    private void HeroPrev_Click(object sender, RoutedEventArgs e) => StepHero(-1);

    private void HeroNext_Click(object sender, RoutedEventArgs e) => StepHero(+1);

    // Custom flipper buttons page the carousel, wrapping at the ends. Setting FlipView.SelectedIndex
    // programmatically does NOT navigate on the Skia/WASM head, so scroll the FlipView's own
    // horizontal ScrollViewer by one viewport — the mechanism a swipe uses — which snaps to the next
    // slide and updates SelectedIndex (so the pager stays in sync via SelectionChanged).
    private void StepHero(int delta)
    {
        var count = Hero.Items.Count;
        if (count <= 0)
        {
            return;
        }

        _heroScroll ??= FindDescendant<ScrollViewer>(Hero);
        // Guard a zero ViewportWidth (before first layout) so we don't ChangeView to a no-op 0.
        if (_heroScroll is not { ViewportWidth: > 0 } scroll)
        {
            return;
        }

        // SelectedIndex can be -1 before the FlipView has committed a selection; treat that as 0 so
        // "previous" wraps to the last slide rather than the second-to-last.
        var current = Math.Max(0, Hero.SelectedIndex);
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
