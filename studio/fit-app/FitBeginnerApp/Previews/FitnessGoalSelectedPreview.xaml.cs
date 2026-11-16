using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The chosen fitness goal, showing the selected mark.
[Preview("Fitness Goal — Selected", typeof(ContentControl), dataTemplateKey: "FitnessGoalTemplate")]
public sealed partial class FitnessGoalSelectedPreview : Preview
{
    public FitnessGoalSelectedPreview() => this.InitializeComponent();
}
