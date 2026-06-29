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

        // Hot Design fallback for the preview; at runtime Navigation injects the generated CartModel
        // VM as the page's DataContext. Set on the *page* (this.DataContext), never a child element:
        // a child with its own DataContext would shadow the injected VM, leaving the buttons' commands
        // bound to the inert mock.
        this.DataContext = CartPageMockData.Data;

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
