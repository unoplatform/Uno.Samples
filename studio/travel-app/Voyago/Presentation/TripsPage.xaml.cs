namespace Voyago.Presentation;

public sealed partial class TripsPage : Page
{
    public TripsPage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from ITripsService and is
        // rendered through FeedViews, so its design-time data must be the feed-shaped mock that
        // returns the generated ViewModel (see Presentation/MockData) — and a hand-built generated VM
        // must never be seeded from a page constructor: it has no live SourceContext, so its feeds
        // never pump, and it would shadow the VM Navigation injects at runtime. The named previews
        // supply it in XAML instead.
        // (It also used to construct a THROWAWAY TripsService here — a second instance of a DI
        //  singleton, whose trip book is not the one the running app shares.)

    }
}