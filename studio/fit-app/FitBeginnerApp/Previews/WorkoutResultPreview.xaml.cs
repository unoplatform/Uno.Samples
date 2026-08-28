using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// One completed workout as a result card. This template is shared verbatim by HomePage and
// ProgressPage, so previewing it once covers both.
[Preview("Workout Result", typeof(ContentControl), dataTemplateKey: "WorkoutResultTemplate")]
public sealed partial class WorkoutResultPreview : Preview
{
    public WorkoutResultPreview() => this.InitializeComponent();
}
