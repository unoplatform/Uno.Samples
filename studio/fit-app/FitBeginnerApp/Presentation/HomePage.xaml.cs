using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the HomeModel as the page's DataContext, and a child carrying its
        // own explicit DataContext would shadow it, leaving every binding stuck on the inert seed.
        this.DataContext = new HomeModel();
        // (A plain [ReactiveBindable(false)] Model IS the design-time data here — it projects fixed
        //  values, so no separate mock is needed and the auto-Default preview renders populated.)

        // One orchestrated load: sections fade + rise in sequence (skipped under reduced motion).
        Loaded += (_, _) =>
        {
            Motion.Entrance(HeroSection, 0);
            Motion.Entrance(WeekSection, 70);
            Motion.Entrance(TodaySection, 140);
            Motion.Entrance(ResultsSection, 210);
            Motion.Entrance(TipsSection, 280);
        };
    }
}
