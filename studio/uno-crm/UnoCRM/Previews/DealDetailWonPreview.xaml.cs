using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// DealDetailPage for a closed-won deal (DealDetailPageMockData.Won), bound in XAML via {x:Bind}.
[Preview("Deal — Won", typeof(DealDetailPage))]
public sealed partial class DealDetailWonPreview : Preview
{
    public DealDetailWonPreview() => this.InitializeComponent();
}
