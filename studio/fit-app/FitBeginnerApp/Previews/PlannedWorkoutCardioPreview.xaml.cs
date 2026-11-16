using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The same card for a cardio session, to show the per-type glyph mapping.
[Preview("Planned Workout — Cardio", typeof(ContentControl), dataTemplateKey: "PlannedWorkoutTemplate")]
public sealed partial class PlannedWorkoutCardioPreview : Preview
{
    public PlannedWorkoutCardioPreview() => this.InitializeComponent();
}
