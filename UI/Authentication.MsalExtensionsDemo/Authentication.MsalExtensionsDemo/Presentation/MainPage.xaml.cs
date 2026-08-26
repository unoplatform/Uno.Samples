using Authentication.MsalExtensionsDemo.Authentication;

namespace Authentication.MsalExtensionsDemo.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        PlatformChip.Text = PlatformSupport.PlatformName;
    }

    private void OnSectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var tag = (args.SelectedItem as NavigationViewItem)?.Tag as string;

        SignInSection.Visibility = tag == "signin" ? Visibility.Visible : Visibility.Collapsed;
        GraphSection.Visibility = tag == "graph" ? Visibility.Visible : Visibility.Collapsed;
        SetupSection.Visibility = tag == "setup" ? Visibility.Visible : Visibility.Collapsed;
    }
}
