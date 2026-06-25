namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for CartPage. Mirrors CartModel's binding surface with a small non-empty
// cart so the items list and order summary render in Hot Design / Studio. At runtime the
// navigation-injected generated CartModel VM overrides this.
public partial record CartPageMockData
{
    public static CartPageMockData Data { get; } = new();

    private static readonly IImmutableList<CartItem> SampleCart =
    [
        new("p-001", "Classic Latte",
            "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            5.50, 2),
        new("p-003", "Butter Croissant",
            "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            3.25, 1),
    ];

    public IListFeed<CartItem> CartItems => ListFeed.Async(async _ => SampleCart);

    public IFeed<CartSummary> Summary => Feed.Async(async _ => new CartSummary(SampleCart));

    public IFeed<string> ItemCountText =>
        Feed.Async(async _ => new CartSummary(SampleCart).ItemCountText);

    public void Increment(CartItem item) { }
    public void Decrement(CartItem item) { }
    public void RemoveItem(CartItem item) { }
    public void PlaceOrder() { }
    public void GoToMenu() { }
}
