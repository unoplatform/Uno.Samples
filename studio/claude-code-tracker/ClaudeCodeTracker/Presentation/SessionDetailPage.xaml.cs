namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionDetailPage : Page
{
    public SessionDetailPage()
    {
        this.InitializeComponent();

        // DataContext is provided by navigation: the DataViewMap injects the tapped SessionEntry
        // as a SessionDetailModel, so each row opens its own detail. A design-time-only fallback
        // keeps Hot Design previews populated without overriding the navigated data at runtime.
        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        {
            this.DataContext = new SessionDetailModel(SampleData.Sessions[0]);
        }
    }
}
