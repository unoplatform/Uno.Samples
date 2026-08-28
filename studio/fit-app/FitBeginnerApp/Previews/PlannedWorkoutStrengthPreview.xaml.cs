using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// A strength session in the week list — the WorkoutIcon converter resolves "Full Body" to the
// dumbbell glyph. The card is a Button that navigates; with no region above it in an isolated
// preview the navigation is simply inert, which is expected.
[Preview("Planned Workout — Strength", typeof(ContentControl), dataTemplateKey: "PlannedWorkoutTemplate")]
public sealed partial class PlannedWorkoutStrengthPreview : Preview
{
    public PlannedWorkoutStrengthPreview() => this.InitializeComponent();
}
