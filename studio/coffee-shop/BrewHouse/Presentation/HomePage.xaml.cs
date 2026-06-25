namespace BrewHouse.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Hot Design renders this page without running Navigation, so seed a representative
        // DataContext for the preview. At runtime Uno.Extensions Navigation injects the
        // DI-resolved HomePageData (with INavigator) and overrides this — set it on the *page*,
        // never on a child element, so the injected model wins.
        this.DataContext = new HomePageData(AppState.Current);
    }
}
