using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// The same row at the smallest share, where the bar is a sliver and the label still has to read.
[Preview("Model Usage Row — Small Share", typeof(ContentControl), dataTemplateKey: "ModelUsageRowTemplate")]
public sealed partial class ModelUsageRowMinorPreview : Preview
{
    public ModelUsageRowMinorPreview() => this.InitializeComponent();
}
