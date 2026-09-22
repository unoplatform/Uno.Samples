using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A session in the Dashboard's recent-activity strip — same data, not clickable.
[Preview("Recent Session Card", typeof(ContentControl), dataTemplateKey: "RecentSessionCardTemplate")]
public sealed partial class RecentSessionCardPreview : Preview
{
    public RecentSessionCardPreview() => this.InitializeComponent();
}
