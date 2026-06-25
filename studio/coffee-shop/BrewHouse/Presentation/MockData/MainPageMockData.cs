namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MainPage (the shell). Supplies plain values the cart badge binds to so
// it renders in Hot Design / Studio. At runtime Navigation injects the generated MainModel, which
// surfaces these as feeds off the shared cart.
public partial record MainPageMockData
{
    public static MainPageMockData Data { get; } = new();

    public int CartItemCount => 3;
    public bool CartHasItems => CartItemCount > 0;
}
