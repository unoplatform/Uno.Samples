using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A part-consumed plan rate limit.
[Preview("Rate Limit Card", typeof(ContentControl), dataTemplateKey: "RateLimitCardTemplate")]
public sealed partial class RateLimitCardPreview : Preview
{
    public RateLimitCardPreview() => this.InitializeComponent();
}
