namespace ClaudeCodeTracker.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional): Hot Design renders the page WITHOUT running Navigation,
        // so it needs a DataContext to preview; at runtime Uno.Extensions Navigation injects its own
        // DashboardModel after construction and overrides this (harmless — same type, same data).
        // NB: DesignMode.DesignModeEnabled is false in Hot Design (it flags only the classic XAML
        // designer), so gating on it would blank the preview.
        this.DataContext = new DashboardModel();
    }
}
