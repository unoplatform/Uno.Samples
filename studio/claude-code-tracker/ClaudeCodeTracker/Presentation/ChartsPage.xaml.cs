namespace ClaudeCodeTracker.Presentation;

public sealed partial class ChartsPage : Page
{
    public ChartsPage()
    {
        this.InitializeComponent();

        // Hot Design fallback; Navigation injects the ChartsModel from the ViewMap at runtime.
        this.DataContext = new ChartsModel();
    }
}
