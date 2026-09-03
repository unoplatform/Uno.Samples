namespace BrewHouse.Presentation.MockData;

/// <summary>
/// One line of a placed <see cref="OrderRecord"/> — a frozen copy of what a <see cref="CartItem"/>
/// was at checkout, so a later price change cannot rewrite history. Needs no key: an order's lines are
/// fixed once placed and are only ever read as a set (see <see cref="ProductItem"/> for the
/// convention).
/// </summary>
public partial record OrderLineItem(
    string Name,
    int Quantity,
    double Price,
    string ImageUrl = "");
