using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class ProgressPage : Page
{
    public ProgressPage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the ProgressModel as the page's DataContext, and a child carrying its
        // own explicit DataContext would shadow it, leaving every binding stuck on the inert seed.
        this.DataContext = new ProgressModel();
        // (A plain [ReactiveBindable(false)] Model IS the design-time data here — it projects fixed
        //  values, so no separate mock is needed and the auto-Default preview renders populated.)

        Loaded += (_, _) =>
        {
            Motion.Entrance(HeaderSection, 0);
            Motion.Entrance(StatsSection, 70);
            Motion.Entrance(StreakSection, 140);
            Motion.Entrance(MilestonesSection, 210);
            Motion.Entrance(HistorySection, 280);
        };
    }
}
