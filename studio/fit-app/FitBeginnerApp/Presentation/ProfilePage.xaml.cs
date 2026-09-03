using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class ProfilePage : Page
{
    public ProfilePage()
    {
        this.InitializeComponent();

        // No design-time DataContext here. This page's data comes from IFitnessService and is
        // rendered through FeedViews, so its design-time data must be the feed-shaped mock that
        // returns the generated ViewModel (see Presentation/MockData) — and a hand-built generated
        // VM must never be seeded from a page constructor: it has no live SourceContext, so its
        // feeds never pump, and it would shadow the VM Navigation injects at runtime.
        //
        // The named previews supply it in XAML instead, which is the only safe place for it. The
        // page's automatic "Default" preview therefore shows the FeedViews' empty state.
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
