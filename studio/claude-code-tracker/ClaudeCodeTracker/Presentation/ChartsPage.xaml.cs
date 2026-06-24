namespace ClaudeCodeTracker.Presentation;

public sealed partial class ChartsPage : Page
{
    public ChartsPage()
    {
        this.InitializeComponent();

        // Hot Design fallback only; Navigation injects the ChartsModel from the ViewMap at runtime.
        // Gate on design mode so the injected instance wins and ChartsModel's paint-building ctor
        // doesn't run twice per navigation (lesson 21).
        if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
        {
            this.DataContext = new ChartsModel();
        }
    }
}
