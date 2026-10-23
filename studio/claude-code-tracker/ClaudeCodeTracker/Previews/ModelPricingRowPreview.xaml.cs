using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A row of the model price list.
[Preview("Model Pricing Row", typeof(ContentControl), dataTemplateKey: "ModelPricingRowTemplate")]
public sealed partial class ModelPricingRowPreview : Preview
{
    public ModelPricingRowPreview() => this.InitializeComponent();
}
