namespace UnoCRM;

public sealed partial class PipelinePage : Page
{
    public PipelinePage()
    {
        this.InitializeComponent();
    }

    private void NavigateToDashboard_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(MainPage));
    }

    private void NavigateToLeads_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(LeadsPage));
    }

    private void NavigateToContacts_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(ContactsPage));
    }
}