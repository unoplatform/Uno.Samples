using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

    }

    // The Cart count badges bind here via x:Bind (page-scoped), so they don't depend on the
    // nav/region DataContext of the NavigationViewItem / TabBarItem. AppState.Current is the same
    // instance the DI-registered AppState resolves to, so the badge stays in sync with every page.
    public AppState CartState => AppState.Current;

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
