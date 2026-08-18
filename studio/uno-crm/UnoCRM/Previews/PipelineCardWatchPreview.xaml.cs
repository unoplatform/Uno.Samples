using UnoCRM.Controls;
using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// The pipeline deal card, open + watch (amber dot).
[Preview("Pipeline Card — Watch", typeof(PipelineCard))]
public sealed partial class PipelineCardWatchPreview : Preview
{
    public PipelineCardWatchPreview() => this.InitializeComponent();
}
