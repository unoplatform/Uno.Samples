using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The "Great" feeling pill. The selector picks one keyed template per value, so each value is
// its own preview surface; this one uses the primary container role.
[Preview("Feeling — Great", typeof(ContentControl), dataTemplateKey: "FeelingPillGreatTemplate")]
public sealed partial class FeelingPillGreatPreview : Preview
{
    public FeelingPillGreatPreview() => this.InitializeComponent();
}
