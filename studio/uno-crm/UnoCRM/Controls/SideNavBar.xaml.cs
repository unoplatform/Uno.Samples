namespace UnoCRM.Controls;

/// <summary>
/// Desktop navigation rail shared by the wide layout of every page.
/// Set <see cref="ActiveTab"/> ("Dashboard" | "Pipeline" | "Leads" | "Contacts") to
/// highlight the current page; the control navigates the hosting <see cref="Frame"/> on tap.
/// Desktop counterpart of the mobile <see cref="BottomNavBar"/>.
/// </summary>
public sealed partial class SideNavBar : UserControl
{
    public SideNavBar()
    {
        this.InitializeComponent();
        Loaded += (_, _) => UpdateActiveState();
    }

    public static readonly DependencyProperty ActiveTabProperty =
        DependencyProperty.Register(
            nameof(ActiveTab),
            typeof(string),
            typeof(SideNavBar),
            new PropertyMetadata("Dashboard", (d, _) => ((SideNavBar)d).UpdateActiveState()));

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    private void UpdateActiveState() =>
        VisualStateManager.GoToState(this, string.IsNullOrEmpty(ActiveTab) ? "Dashboard" : ActiveTab, false);

    private void OnTabClick(object sender, RoutedEventArgs e)
    {
        var tab = ((FrameworkElement)sender).Tag as string;
        if (tab is null || tab == ActiveTab)
        {
            // Tapping the current tab is a no-op (avoids pushing a duplicate onto the back stack).
            return;
        }

        var target = tab switch
        {
            "Dashboard" => typeof(MainPage),
            "Pipeline" => typeof(PipelinePage),
            "Leads" => typeof(LeadsPage),
            "Contacts" => typeof(ContactsPage),
            _ => null,
        };

        if (target is null)
        {
            return;
        }

        // Tab navigation shouldn't grow a back stack — otherwise Android's Back button
        // replays the whole tab history. Clear it after a successful navigation.
        var frame = FindFrame();
        if (frame is not null && frame.Navigate(target))
        {
            frame.BackStack.Clear();
        }
    }

    private Frame? FindFrame()
    {
        DependencyObject? current = this;
        while (current is not null)
        {
            if (current is Frame frame)
            {
                return frame;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
