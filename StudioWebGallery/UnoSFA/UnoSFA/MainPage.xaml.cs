namespace UnoSFA;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private void NavigateToPipeline_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(PipelinePage));
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
