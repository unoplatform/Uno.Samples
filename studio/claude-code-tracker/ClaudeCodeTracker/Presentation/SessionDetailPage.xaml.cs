using ClaudeCodeTracker.Presentation.MockData;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionDetailPage : Page
{
    public SessionDetailPage()
    {
        this.InitializeComponent();
        Root.DataContext = SessionDetailPageMockData.Data;
    }
}
