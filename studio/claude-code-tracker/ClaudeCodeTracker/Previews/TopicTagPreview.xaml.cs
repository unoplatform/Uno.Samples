using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace ClaudeCodeTracker.Previews;

// A topic tag chip on the session detail page.
[Preview("Topic Tag", typeof(ContentControl), dataTemplateKey: "TopicTagTemplate")]
public sealed partial class TopicTagPreview : Preview
{
    public TopicTagPreview() => this.InitializeComponent();
}
