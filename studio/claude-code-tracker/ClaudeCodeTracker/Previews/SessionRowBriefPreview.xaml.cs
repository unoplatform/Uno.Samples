using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// The same row for the cheapest, shortest session: the other end of the range it has to lay out.
[Preview("Session Row — Brief", typeof(ContentControl), dataTemplateKey: "SessionRowTemplate")]
public sealed partial class SessionRowBriefPreview : Preview
{
    public SessionRowBriefPreview() => this.InitializeComponent();
}
