namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MainPage (the shell). Supplies the cart-count feed the badge binds
// to so it renders in Hot Design / Studio. At runtime Navigation injects the generated MainModel.
public partial record MainPageMockData
{
    public static MainPageMockData Data { get; } = new();

    public IFeed<int> CartItemCount => Feed.Async(async _ => 3);
}
