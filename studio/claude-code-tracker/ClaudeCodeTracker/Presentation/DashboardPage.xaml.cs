using ClaudeCodeTracker.Presentation.MockData;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();
        Root.DataContext = DashboardPageMockData.Data;
    }
}
