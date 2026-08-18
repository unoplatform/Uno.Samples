using UnoCRM.Controls;
using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// The pipeline deal card, open + at risk (red dot).
[Preview("Pipeline Card — At Risk", typeof(PipelineCard))]
public sealed partial class PipelineCardAtRiskPreview : Preview
{
    public PipelineCardAtRiskPreview() => this.InitializeComponent();
}
