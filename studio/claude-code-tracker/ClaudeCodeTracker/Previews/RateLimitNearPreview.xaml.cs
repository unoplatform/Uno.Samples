using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A limit at the top of its window, which is the state this card exists to warn about.
[Preview("Rate Limit Card — Near Limit", typeof(ContentControl), dataTemplateKey: "RateLimitCardTemplate")]
public sealed partial class RateLimitNearPreview : Preview
{
    public RateLimitNearPreview() => this.InitializeComponent();
}
