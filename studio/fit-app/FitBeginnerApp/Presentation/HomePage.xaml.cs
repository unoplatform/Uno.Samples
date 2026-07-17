using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design fallback (the preview bypasses Navigation). Set on the *page* DataContext so
        // Navigation can override it with the injected HomeModel at runtime.
        this.DataContext = new HomeModel();

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
