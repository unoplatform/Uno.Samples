using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The "Good" feeling pill (tertiary container role).
[Preview("Feeling — Good", typeof(ContentControl), dataTemplateKey: "FeelingPillGoodTemplate")]
public sealed partial class FeelingPillGoodPreview : Preview
{
    public FeelingPillGoodPreview() => this.InitializeComponent();
}
