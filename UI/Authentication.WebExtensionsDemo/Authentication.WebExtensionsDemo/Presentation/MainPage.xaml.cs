using Authentication.WebExtensionsDemo.Authentication;

namespace Authentication.WebExtensionsDemo.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        PlatformChip.Text = PlatformSupport.PlatformName;
    }
}
