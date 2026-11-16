namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for CartPage. Mirrors CartModel's binding surface with a small non-empty
// cart so the items list and order summary render in Hot Design / Studio. At runtime the
// navigation-injected generated CartModel VM overrides this.
public partial record CartPageMockData
{
    private static readonly IImmutableList<CartItem> SampleCart =
    [
        new("p-001", "Classic Latte",
            "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            5.50, 2),
        new("p-003", "Butter Croissant",
            "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            3.25, 1),
    ];

    // Default design-time state: a small non-empty cart.
    public static CartPageMockData Data { get; } = new();

    // A second design-time state: an empty cart, so the "empty cart" hero shows instead of the
    // items list and order summary. The "Cart — Empty" preview uses this.
    public static CartPageMockData Empty { get; } = new() { Cart = [] };

    // Plain, materialized values (not feeds) so the items list, order summary and header subtitle
    // render directly in Hot Design; the live CartModel surfaces feeds at runtime. Init-settable so
    // a variant (see Empty) can supply no items; defaults to the sample cart above.
    public IImmutableList<CartItem> Cart { get; init; } = SampleCart;

    public IReadOnlyList<CartItem> CartItems => Cart;

    public CartSummary Summary => new(Cart);
    public string ItemCountText => Summary.ItemCountText;
    public bool CartHasItems => Summary.HasItems;
    public IReadOnlyList<string> PopularChoices { get; } = ["Latte", "Croissant", "Matcha"];

    public void Increment(CartItem item) { }
    public void Decrement(CartItem item) { }
    public void PlaceOrder() { }
    public void GoToMenu() { }
}
