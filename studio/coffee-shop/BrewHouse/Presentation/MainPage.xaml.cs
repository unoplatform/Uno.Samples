using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // The shell is view-only (no navigation-injected model), so bind its chrome (the Cart
        // count badge) to the shared cart/orders singleton. AppState.Current is the same instance
        // the DI-registered AppState resolves to, so the badge stays in sync with every page.
        this.DataContext = AppState.Current;
    }

    // Flip the whole app between light and dark by setting RequestedTheme on the visual-tree root,
    // which re-resolves every {ThemeResource} (the warm and dark-roast ThemeDictionaries).
    private void OnToggleTheme(object sender, RoutedEventArgs e)
    {
        if (this.XamlRoot?.Content is FrameworkElement root)
        {
            root.RequestedTheme = root.ActualTheme == ElementTheme.Dark
                ? ElementTheme.Light
                : ElementTheme.Dark;
        }
    }
}
