using Microsoft.UI.Xaml;

namespace FitBeginnerApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (the preview bypasses Navigation). Set on the *page* DataContext so
        // Navigation can override it with the injected MainModel at runtime.
        this.DataContext = new MainModel();
    }

    // Flip the whole app between light and dark by setting RequestedTheme on the visual-tree root,
    // which re-resolves every {ThemeResource} (the light and dark ThemeDictionaries). No
    // IThemeService needed for a basic toggle.
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
