using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// A preview group holding the PipelineCard states (Healthy / Watch / At Risk / Won) as named
// children — each classifies under its content type (PipelineCard) in the Previews tree.
public sealed partial class PipelineCardPreviews : PreviewGroup
{
    public PipelineCardPreviews() => this.InitializeComponent();
}
