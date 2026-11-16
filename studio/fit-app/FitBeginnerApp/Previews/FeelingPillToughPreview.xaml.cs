using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The "Tough" feeling pill (secondary container role).
[Preview("Feeling — Tough", typeof(ContentControl), dataTemplateKey: "FeelingPillToughTemplate")]
public sealed partial class FeelingPillToughPreview : Preview
{
    public FeelingPillToughPreview() => this.InitializeComponent();
}
