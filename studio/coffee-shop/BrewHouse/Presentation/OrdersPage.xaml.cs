using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class OrdersPage : Page
{
    public OrdersPage()
    {
        this.InitializeComponent();

        // No design-time DataContext here: this page renders through a FeedView, whose
        // design-time data must be the feed-shaped mock returning the generated ViewModel
        // (see the MockData folder) — and a hand-built generated VM must never be seeded from
        // a page constructor, because it has no live SourceContext and would shadow the VM
        // Navigation injects. The named previews supply it in XAML instead.
    }
}
