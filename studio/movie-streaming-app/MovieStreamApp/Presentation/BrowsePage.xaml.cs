using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MovieStreamApp.Presentation;

public sealed partial class BrowsePage : Page
{
    public BrowsePage()
    {
        this.InitializeComponent();

        // Hot Design / the Previews gallery renders the page WITHOUT running Navigation, so no model
        // is injected and the preview shows empty lists + blank images. Seed a representative sample
        // DataContext (the MVUX bindable proxy over a real model) so the preview populates. Set it on
        // the PAGE, unconditionally — Navigation overrides this.DataContext at runtime, and it must not
        // be gated on DesignMode.DesignModeEnabled (which is false in Hot Design).
        this.DataContext = new BrowseViewModel(new WatchlistService());
    }

    private void TrendingScrollLeft(object sender, RoutedEventArgs e) => ScrollRail(TrendingScroller, back: true);
    private void TrendingScrollRight(object sender, RoutedEventArgs e) => ScrollRail(TrendingScroller, back: false);
    private void NewArrivalsScrollLeft(object sender, RoutedEventArgs e) => ScrollRail(NewArrivalsScroller, back: true);
    private void NewArrivalsScrollRight(object sender, RoutedEventArgs e) => ScrollRail(NewArrivalsScroller, back: false);

    // Page a horizontal rail by ~80% of its viewport width (desktop arrow clicks).
    private static void ScrollRail(ScrollViewer rail, bool back)
    {
        var delta = rail.ViewportWidth * 0.8;
        rail.ChangeView(rail.HorizontalOffset + (back ? -delta : delta), null, null);
    }
}
