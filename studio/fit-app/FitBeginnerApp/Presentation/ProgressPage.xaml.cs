using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class ProgressPage : Page
{
    public ProgressPage()
    {
        this.InitializeComponent();

        // Set the DataContext so Hot Design Previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions.Navigation resolves the model from the ViewMap<TPage, TModel>
        // and assigns its own instance; replacing this one is expected and harmless.
        this.DataContext = new ProgressModel();

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
