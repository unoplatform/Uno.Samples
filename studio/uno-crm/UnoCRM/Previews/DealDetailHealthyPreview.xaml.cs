using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// DealDetailPage for an open, healthy deal (DealDetailPageMockData.Healthy), bound in XAML via {x:Bind}.
[Preview("Deal — Healthy", typeof(DealDetailPage))]
public sealed partial class DealDetailHealthyPreview : Preview
{
    public DealDetailHealthyPreview() => this.InitializeComponent();
}
