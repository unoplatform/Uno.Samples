using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A model's spend and token share, with its share bar near full.
[Preview("Model Usage Row", typeof(ContentControl), dataTemplateKey: "ModelUsageRowTemplate")]
public sealed partial class ModelUsageRowPreview : Preview
{
    public ModelUsageRowPreview() => this.InitializeComponent();
}
