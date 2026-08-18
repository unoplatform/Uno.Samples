using UnoCRM.Controls;
using Uno.UI.HotDesign;

namespace UnoCRM.Previews;

// The pipeline deal card, open + healthy (green dot). Values are set on the control's dependency
// properties in the XAML, so there is no code-behind data.
[Preview("Pipeline Card — Healthy", typeof(PipelineCard))]
public sealed partial class PipelineCardHealthyPreview : Preview
{
    public PipelineCardHealthyPreview() => this.InitializeComponent();
}
