namespace BrewHouse.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();

        // Deliberately NO design-time DataContext here. The page renders its products through a
        // FeedView, whose design-time data has to be a feed-shaped mock returning the generated
        // ViewModel (see MenuPageMockData) — and a hand-built generated VM must never be seeded from
        // a page constructor: it has no live SourceContext, so its feeds never pump, and it would
        // shadow the VM Navigation injects at runtime.
        //
        // The explicit MenuPreviews units supply that DataContext in XAML instead, which is the only
        // safe place for it. The page's automatic "Default" preview therefore shows the FeedView's
        // empty state — use the named previews.
    }
}
