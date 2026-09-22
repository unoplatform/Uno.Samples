using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A swatch-and-label legend row, shared by both Charts legends.
[Preview("Chart Legend Row", typeof(ContentControl), dataTemplateKey: "ChartLegendRowTemplate")]
public sealed partial class ChartLegendRowPreview : Preview
{
    public ChartLegendRowPreview() => this.InitializeComponent();
}
