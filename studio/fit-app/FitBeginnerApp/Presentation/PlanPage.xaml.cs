using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class PlanPage : Page
{
    public PlanPage()
    {
        this.InitializeComponent();

        // Set the DataContext so Hot Design Previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions.Navigation resolves the model from the ViewMap<TPage, TModel>
        // and assigns its own instance; replacing this one is expected and harmless.
        this.DataContext = new PlanModel();

        Loaded += (_, _) =>
        {
            Motion.Entrance(HeaderSection, 0);
            Motion.Entrance(SummarySection, 70);
            Motion.Entrance(WeekListSection, 140);
            Motion.Entrance(SuggestedSection, 210);
        };
    }
}