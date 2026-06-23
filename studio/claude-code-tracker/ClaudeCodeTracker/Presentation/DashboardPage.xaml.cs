namespace ClaudeCodeTracker.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();

        // Set the DataContext so Hot Design previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions Navigation resolves DashboardModel from the ViewMap<TPage, TModel>
        // and assigns its own instance; replacing this one is expected and harmless.
        this.DataContext = new DashboardModel();
    }
}
