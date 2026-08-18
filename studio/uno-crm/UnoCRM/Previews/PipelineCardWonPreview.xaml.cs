using UnoCRM.Controls;
using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// The pipeline deal card, closed-won (green check + "Won" meta).
[Preview("Pipeline Card — Won", typeof(PipelineCard))]
public sealed partial class PipelineCardWonPreview : Preview
{
    public PipelineCardWonPreview() => this.InitializeComponent();
}
