using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// DealDetailPage for an open, at-risk deal (DealDetailPageMockData.AtRisk). DataContext is bound in
// XAML via {x:Bind}, so there is no LoadDataContext override.
[Preview("Deal — At Risk", typeof(DealDetailPage))]
public sealed partial class DealDetailAtRiskPreview : Preview
{
    public DealDetailAtRiskPreview() => this.InitializeComponent();
}
