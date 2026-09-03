using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The "Easy" feeling pill — also the selector's fallback for any unrecognised value.
[Preview("Feeling — Easy", typeof(ContentControl), dataTemplateKey: "FeelingPillEasyTemplate")]
public sealed partial class FeelingPillEasyPreview : Preview
{
    public FeelingPillEasyPreview() => this.InitializeComponent();
}
