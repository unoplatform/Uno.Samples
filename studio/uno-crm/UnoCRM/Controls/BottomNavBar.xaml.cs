namespace UnoCRM.Controls;

/// <summary>
/// Floating bottom navigation bar shared by the mobile layouts of every page.
/// Set <see cref="ActiveTab"/> ("Home" | "Pipeline" | "Leads" | "Contacts") to highlight
/// the current page; the control navigates the hosting <see cref="Frame"/> on tap.
/// </summary>
public sealed partial class BottomNavBar : UserControl
{
    public BottomNavBar()
    {
        this.InitializeComponent();
        Loaded += (_, _) => UpdateActiveState();
    }

    public static readonly DependencyProperty ActiveTabProperty =
        DependencyProperty.Register(
            nameof(ActiveTab),
            typeof(string),
            typeof(BottomNavBar),
            new PropertyMetadata("Home", (d, _) => ((BottomNavBar)d).UpdateActiveState()));

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    private void UpdateActiveState() =>
        VisualStateManager.GoToState(this, string.IsNullOrEmpty(ActiveTab) ? "Home" : ActiveTab, false);

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
            "Home" => typeof(MainPage),
            "Pipeline" => typeof(PipelinePage),
            "Leads" => typeof(LeadsPage),
            "Contacts" => typeof(ContactsPage),
            _ => null,
        };

        if (target is not null)
        {
            FindFrame()?.Navigate(target);
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
