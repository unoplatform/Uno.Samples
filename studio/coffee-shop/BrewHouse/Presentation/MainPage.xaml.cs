using BrewHouse.Presentation.MockData;
using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // Hot Design fallback so the cart badge has a source in the preview; at runtime Navigation
        // injects the generated MainModel VM onto the page and overrides this. Set on the *page*
        // DataContext (not RootShellGrid) so the badge's ElementName=RootShellGrid binding inherits
        // the live model — setting the child's DataContext would pin it to the mock.
        this.DataContext = MainPageMockData.Data;
    }
}
