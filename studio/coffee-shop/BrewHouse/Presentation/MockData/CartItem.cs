namespace BrewHouse.Presentation.MockData;

/// <summary>
/// One line in the basket: a product, the quantity of it, and the money that follows from the two.
/// Carries <c>[property: Key]</c> on <see cref="ProductId"/> because it lives in the shared
/// CartService's <c>IListState&lt;CartItem&gt;</c>, where lines are added, re-quantified and removed
/// individually — see <see cref="ProductItem"/> for the convention.
/// </summary>
public partial record CartItem(
    [property: global::Uno.Extensions.Equality.Key] string ProductId,
    string Name,
    string ImageUrl,
    double Price,
    int Quantity)
{
    public double LineTotal => Price * Quantity;
    public string LineTotalFormatted => LineTotal.ToString("F2");
    public string PriceFormatted => Price.ToString("F2");
}
