using BrewHouse.Presentation.MockData;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BrewHouse.Presentation;

public sealed partial class CartPage : Page
{
    // The "order placed" confirmation is a transient toast — pure view behaviour, so it lives here
    // and not in the model. It shows on Place Order and auto-dismisses after 4 seconds.
    private readonly DispatcherTimer _confirmTimer = new() { Interval = TimeSpan.FromSeconds(4) };

    public CartPage()
    {
        this.InitializeComponent();

        // Hot Design fallback; Navigation injects the generated CartModel VM at runtime and
        // overrides this. Set on the page's root element, not a deeper child.
        Root.DataContext = CartPageMockData.Data;

        _confirmTimer.Tick += (_, _) =>
        {
            _confirmTimer.Stop();
            OrderConfirmedBanner.Visibility = Visibility.Collapsed;
        };
    }

    // Fires alongside the PlaceOrder command (the command empties the cart, which flips the body to
    // the empty-cart hero; this banner overlays it briefly to confirm the order).
    private void OnPlaceOrder(object sender, RoutedEventArgs e)
    {
        OrderConfirmedBanner.Visibility = Visibility.Visible;
        _confirmTimer.Stop();
        _confirmTimer.Start();
    }
}
