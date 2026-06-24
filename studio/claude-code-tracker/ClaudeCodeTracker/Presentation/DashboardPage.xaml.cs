namespace ClaudeCodeTracker.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();

        // Hot Design fallback only; at runtime Uno.Extensions Navigation resolves DashboardModel
        // from the ViewMap<TPage, TModel> and injects its own instance, so gate this on design mode
        // (matches SessionDetailPage and avoids constructing a throwaway model per navigation; lesson 21).
        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        {
            this.DataContext = new DashboardModel();
        }
    }
}
