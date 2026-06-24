using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

namespace BrewHouse.Presentation;

public sealed partial class MenuPage : Page
{
    public MenuPage()
    {
        this.InitializeComponent();

        // Hot Design fallback (unconditional); Navigation injects the DI-resolved MenuPageData at
        // runtime and overrides this. Set on the page, not a child element.
        this.DataContext = new MenuPageData(AppState.Current);
    }

    // Tapping a product card opens its detail page (the page VM owns navigation).
    private void OnProductTapped(object sender, TappedRoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is ProductItem product)
            (DataContext as MenuPageData)?.ViewProductCommand.Execute(product);
    }

    // The inner "Add to Cart" handles its own tap so it doesn't also open the detail page.
    private void OnAddTapped(object sender, TappedRoutedEventArgs e) => e.Handled = true;
}
