using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// An earned achievement badge, showing the unlocked treatment.
[Preview("Milestone — Unlocked", typeof(ContentControl), dataTemplateKey: "MilestoneBadgeTemplate")]
public sealed partial class MilestoneUnlockedPreview : Preview
{
    public MilestoneUnlockedPreview() => this.InitializeComponent();
}
