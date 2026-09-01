namespace UnoCRM.Presentation;

public sealed partial class LeadsPage : Page
{
    public LeadsPage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from ICrmService and is rendered
        // through feeds (including the LiveCharts series), so its design-time data must be the
        // feed-shaped mock that returns the generated ViewModel — and a hand-built generated VM must
        // never be seeded from a page constructor. The named previews supply it in XAML instead.
    }
}
