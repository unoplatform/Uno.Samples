using Uno.UI.HotDesign;

namespace FitBeginnerApp.Previews;

// A preview group holding the WeeklyRing states (Not Started / Partway / Goal Met) as named
// children, each setting the control's dependency properties in XAML.
public sealed partial class WeeklyRingPreviews : PreviewGroup
{
    public WeeklyRingPreviews() => this.InitializeComponent();
}
