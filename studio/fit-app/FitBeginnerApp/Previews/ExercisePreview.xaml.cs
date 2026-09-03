using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// One exercise in the guided session — the app's richest item card.
[Preview("Exercise", typeof(ContentControl), dataTemplateKey: "ExerciseTemplate")]
public sealed partial class ExercisePreview : Preview
{
    public ExercisePreview() => this.InitializeComponent();
}
