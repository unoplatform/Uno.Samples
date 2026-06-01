using ClaudeCodeTracker.Presentation.MockData;

namespace ClaudeCodeTracker.Presentation;

public sealed partial class ChartsPage : Page
{
    public ChartsPage()
    {
        this.InitializeComponent();
        Root.DataContext = ChartsPageMockData.Data;
    }
}
