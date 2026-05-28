namespace Nexus.Presentation;

public sealed partial class OverviewPage : Page
{
    private bool _attached;

    public OverviewPage()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Visual-tree behavior only. The MVUX ViewModel is wired by the framework via the
        // ViewMap<OverviewPage, OverviewModel> registration in App.xaml.cs; DataContext is
        // the generated bindable proxy by the time this fires. Run once per page lifetime.
        if (_attached)
        {
            return;
        }

        LineItemHoverBehavior.AttachToTree(this);
        _attached = true;
    }
}
