namespace BrewHouse.Presentation.MockData;

// Design-time DataContext for MainPage (the shell). Supplies plain values the cart badge binds to so
// it renders in Hot Design / Studio. At runtime Navigation injects the generated MainModel, which
// surfaces these as feeds off the shared cart.
public partial record MainPageMockData
{
    // Default design-time state: a stocked cart, so the badge shows on the Cart tab / pane item.
    public static MainPageMockData Data { get; } = new();

    // A second design-time state: an empty cart, so the badge is hidden and the Cart tab renders
    // like every other one. The "Shell — No Badge" preview uses this.
    public static MainPageMockData EmptyCart { get; } = new() { CartItemCount = 0 };

    // Init-settable so a variant (see EmptyCart) can zero the count; defaults to a stocked cart.
    public int CartItemCount { get; init; } = 3;
    public bool CartHasItems => CartItemCount > 0;
}
