namespace UnoCRM;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();
        DataContext = CrmData.Dashboard;

        // Uno.Extensions Navigation reassigns DataContext when it activates a view that has
        // no mapped view model. Re-apply the shared dataset so the static bindings resolve.
        DataContextChanged += (_, _) =>
        {
            if (DataContext is not DashboardData)
            {
                DataContext = CrmData.Dashboard;
            }
        };
    }
}
