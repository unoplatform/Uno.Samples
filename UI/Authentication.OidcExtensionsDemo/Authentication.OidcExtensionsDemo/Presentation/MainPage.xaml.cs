using Authentication.OidcExtensionsDemo.Authentication;

namespace Authentication.OidcExtensionsDemo.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        PlatformChip.Text = PlatformSupport.PlatformName;
    }
}
