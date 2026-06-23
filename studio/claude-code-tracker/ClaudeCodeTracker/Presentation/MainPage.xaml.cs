namespace ClaudeCodeTracker.Presentation;

public sealed partial class MainPage : Page
{
    // Width (px) at/above which the NavigationView pane is expanded with labels; below it the
    // pane is hidden and the bottom TabBar takes over.
    private const double WideThreshold = 640;

    public MainPage()
    {
        this.InitializeComponent();

        // Drive the shell layout from the page's actual rendered width rather than
        // {utu:Responsive} / NavigationView's native Auto. Those read the ResponsiveHelper /
        // window metrics, which report "narrow" in Studio Web and Hot Design — so the pane stays
        // collapsed (no labels) there even though the page lays out at the full width. Reading the
        // real layout width keeps the labeled pane visible in those design surfaces while still
        // collapsing to the TabBar on actual phones. (Deliberate, documented exception to the
        // "{utu:Responsive} over code-behind" rule for this design-time-visibility case.)
        Loaded += (_, _) => ApplyShellLayout(ActualWidth);
        SizeChanged += (_, e) => ApplyShellLayout(e.NewSize.Width);
    }

    private void ApplyShellLayout(double width)
    {
        var isWide = width >= WideThreshold;
        NavView.IsPaneVisible = isWide;
        BottomTabs.Visibility = isWide ? Visibility.Collapsed : Visibility.Visible;
    }
}
