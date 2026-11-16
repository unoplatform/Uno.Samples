namespace UnoCRM.Presentation;

public sealed partial class LeadsPage : Page
{
    public LeadsPage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from ICrmService and is rendered
        // through feeds, so its design-time data has to be the feed-shaped mock in
        // Presentation/MockData (which also supplies the LiveCharts series as plain arrays) — whose
        // statics return a GENERATED ViewModel, and a generated ViewModel must never be built by hand
        // outside a preview: its SourceContext is created with the instance and dies with the hosting
        // view, so one built here would have no live context and its feeds would never pump. Nor would
        // it survive: this page sits in a navigation region and the ViewModel Navigation injects
        // REPLACES whatever a constructor set — verified on the simulator, where a page seeded here
        // kept rendering live service data. The named previews supply it in XAML.
    }
}
