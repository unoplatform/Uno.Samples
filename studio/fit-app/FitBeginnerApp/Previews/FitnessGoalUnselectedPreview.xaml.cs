using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// An unchosen goal — the other half of the IsSelected branch.
[Preview("Fitness Goal — Unselected", typeof(ContentControl), dataTemplateKey: "FitnessGoalTemplate")]
public sealed partial class FitnessGoalUnselectedPreview : Preview
{
    public FitnessGoalUnselectedPreview() => this.InitializeComponent();
}
