using FitBeginnerApp.Presentation.MockData;
using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class WorkoutSessionPage : Page
{
    public WorkoutSessionPage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime the DataViewMap injects the tapped workout's generated VM as the page's
        // DataContext, and a child carrying its own would shadow it.
        //
        // Seed the PLAIN mock, not the live Model: WorkoutSessionModel surfaces IsStarted as an
        // IState<bool>, and a design surface has no context to pump it — the converter then sees an
        // object rather than a bool and falls through to false, so the Begin/in-progress branch is
        // only right by accident and the started state is unreachable in any preview.
        this.DataContext = WorkoutSessionPageMockData.Data;

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
