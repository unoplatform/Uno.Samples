using Microsoft.UI.Xaml.Controls;
using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// The same badge not yet earned — the locked half of the IsUnlocked branch.
[Preview("Milestone — Locked", typeof(ContentControl), dataTemplateKey: "MilestoneBadgeTemplate")]
public sealed partial class MilestoneLockedPreview : Preview
{
    public MilestoneLockedPreview() => this.InitializeComponent();
}
