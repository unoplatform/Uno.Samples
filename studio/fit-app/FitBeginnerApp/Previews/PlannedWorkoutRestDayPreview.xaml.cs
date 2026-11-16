using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// A rest day — the edge case in the week list: moon glyph, zero minutes, "Rest" pill.
[Preview("Planned Workout — Rest Day", typeof(ContentControl), dataTemplateKey: "PlannedWorkoutTemplate")]
public sealed partial class PlannedWorkoutRestDayPreview : Preview
{
    public PlannedWorkoutRestDayPreview() => this.InitializeComponent();
}
