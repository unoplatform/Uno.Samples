namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionsPage : Page
{
    public SessionsPage()
    {
        this.InitializeComponent();

        // Nothing else to do here. Search and the model chip are two-way bound to states on
        // SessionsModel, which asks ITrackerService for the matching sessions; the FeedView renders
        // the result, the empty message and the failure state. This page used to filter the list
        // itself in code-behind — setting ItemsSource and toggling an empty-state Visibility — which
        // put view state and query logic in the view.
    }
}
