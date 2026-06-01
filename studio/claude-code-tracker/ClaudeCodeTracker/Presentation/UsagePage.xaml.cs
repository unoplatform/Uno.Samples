using ClaudeCodeTracker.Presentation.MockData;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class UsagePage : Page
{
    public UsagePage()
    {
        this.InitializeComponent();
        Root.DataContext = UsagePageMockData.Data;
    }
}
