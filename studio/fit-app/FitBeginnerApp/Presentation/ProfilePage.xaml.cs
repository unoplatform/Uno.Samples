using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class ProfilePage : Page
{
    public ProfilePage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the ProfileModel as the page's DataContext, and a child carrying its
        // own explicit DataContext would shadow it, leaving every binding stuck on the inert seed.
        this.DataContext = new ProfileModel();
        // (A plain [ReactiveBindable(false)] Model IS the design-time data here — it projects fixed
        //  values, so no separate mock is needed and the auto-Default preview renders populated.)

        Loaded += (_, _) =>
        {
            Motion.Entrance(HeroSection, 0);
            Motion.Entrance(StatsSection, 70);
            Motion.Entrance(PreferencesSection, 140);
            Motion.Entrance(GoalsSection, 210);
            Motion.Entrance(SettingsSection, 280);
        };
    }
}
