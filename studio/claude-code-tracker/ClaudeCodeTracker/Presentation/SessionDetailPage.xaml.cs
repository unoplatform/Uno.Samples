namespace ClaudeCodeTracker.Presentation;

public sealed partial class SessionDetailPage : Page
{
    public SessionDetailPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional): preview a representative session when Navigation isn't
        // running. At runtime the DataViewMap injects the tapped SessionEntry's model and overrides this
        // *page* DataContext (each row opens its own detail). Setting a *child* element's DataContext
        // instead would shadow the injected model — that was the original "every row opens s-001" bug.
        // DesignMode.DesignModeEnabled is false in Hot Design, so don't gate on it (lesson 21).
        this.DataContext = new SessionDetailModel(SampleData.Sessions[0]);
    }
}
