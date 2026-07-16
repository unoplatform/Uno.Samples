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
    }
}
