using BrewHouse.Presentation.MockData;

namespace BrewHouse.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a design-time DataContext
        // for the preview. Set it on the *page* (this.DataContext), never on a child element: at
        // runtime Navigation injects the generated HomeModel VM as the page's DataContext, and a
        // child carrying its own explicit DataContext would shadow it — leaving every binding (and
        // every ElementName=Root command binding) stuck on the inert mock.
        this.DataContext = HomePageMockData.Data;

        // Drive the (display-only) PipsPager from the FlipView so it always shows the current page.
        // A TwoWay XAML binding between PipsPager.SelectedPageIndex and FlipView.SelectedIndex is
        // unreliable on Uno (the pager could show a middle pip selected on the last slide), so mirror
        // the FlipView's index onto the pager explicitly whenever the slide changes.
        HeroFlip.SelectionChanged += (_, _) =>
        {
            if (HeroPips.SelectedPageIndex != HeroFlip.SelectedIndex)
            {
                HeroPips.SelectedPageIndex = HeroFlip.SelectedIndex;
            }
        };
    }
}
