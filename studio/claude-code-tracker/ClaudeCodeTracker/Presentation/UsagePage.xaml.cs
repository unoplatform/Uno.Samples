namespace ClaudeCodeTracker.Presentation;

public sealed partial class UsagePage : Page
{
    public UsagePage()
    {
        this.InitializeComponent();

        // Hot Design fallback; Navigation injects the UsageModel from the ViewMap at runtime.
        this.DataContext = new UsageModel();
    }
}
