namespace UnoCRM.Presentation;

public sealed partial class PipelinePage : Page
{
    public PipelinePage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from ICrmService and is rendered
        // through feeds, so its design-time data has to be the feed-shaped mock in
        // Presentation/MockData — and a mock must never be seeded from a page constructor, because
        // this page sits in a navigation region and the ViewModel Navigation injects REPLACES whatever
        // a constructor set — verified on the simulator, where a page seeded here kept rendering live
        // service data. So a ctor seed is not a way to preview a mock either. The named previews
        // supply it in XAML instead, which is also the only place a variant can be chosen.
    }
}
