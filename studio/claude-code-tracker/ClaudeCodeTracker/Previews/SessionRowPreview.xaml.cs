using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A session row on the Sessions page — the tappable one that navigates to the detail page.
[Preview("Session Row", typeof(ContentControl), dataTemplateKey: "SessionRowTemplate")]
public sealed partial class SessionRowPreview : Preview
{
    public SessionRowPreview() => this.InitializeComponent();
}
