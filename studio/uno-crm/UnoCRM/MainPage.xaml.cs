namespace UnoCRM;

/// <summary>
/// The navigation shell: hosts the NavigationView (wide) / TabBar (narrow) chrome and the
/// content region that Uno.Extensions Navigation injects the active page into. The content
/// pages (Dashboard, Pipeline, Leads, Contacts) carry no navigation chrome of their own.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }
}
