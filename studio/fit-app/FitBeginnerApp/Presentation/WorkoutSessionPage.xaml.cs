using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class WorkoutSessionPage : Page
{
    public WorkoutSessionPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (the preview bypasses Navigation, so no workout is injected). At
        // runtime the DataViewMap injects the tapped WorkoutEntry and overrides this. Set on the
        // *page* DataContext so Navigation can override it.
        this.DataContext = new WorkoutSessionModel(
            new WorkoutEntry("w-001", "Morning Energizer", "Full Body", new DateOnly(2026, 6, 1), 20, false, "Beginner"));

        Loaded += (_, _) =>
        {
            Motion.Entrance(TitleSection, 0);
            Motion.Entrance(SummarySection, 70);
            Motion.Entrance(MotivationSection, 140);
            Motion.Entrance(ExercisesSection, 210);
            Motion.Entrance(TipsSection, 280);
            Motion.Entrance(BeginSection, 350);
        };
    }
}
