using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// A preview group holding the DealDetailPage states (Healthy / Watch / At Risk / Won) as named
// children, in escalation order.
public sealed partial class DealDetailPreviews : PreviewGroup
{
    public DealDetailPreviews() => this.InitializeComponent();
}
