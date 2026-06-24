namespace ClaudeCodeTracker.Presentation;

public sealed partial class UsagePage : Page
{
    public UsagePage()
    {
        this.InitializeComponent();

        // Hot Design fallback only; Navigation injects the UsageModel from the ViewMap at runtime
        // (gate on design mode so the injected instance wins; lesson 21).
        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        {
            this.DataContext = new UsageModel();
        }
    }
}
