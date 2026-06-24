namespace ClaudeCodeTracker.Presentation;

public sealed partial class UsagePage : Page
{
    public UsagePage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional): Hot Design renders the page without Navigation, so it
        // needs a DataContext; at runtime Navigation injects + overrides this. (DesignMode.DesignModeEnabled
        // is false in Hot Design, so gating on it would blank the preview.)
        this.DataContext = new UsageModel();
    }
}
