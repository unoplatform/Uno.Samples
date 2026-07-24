namespace UnoCRM.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();

        // Design-time DataContext for Hot Design / Studio (seed on the page itself, never
        // a child). At runtime Uno.Extensions Navigation injects the mapped DashboardModel, which
        // overrides this. DashboardModel is a pure projection, so it is safe to construct directly.
        this.DataContext = new DashboardModel();
    }
}
