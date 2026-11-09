namespace BrewHouse.Presentation.MockData;

/// <summary>
/// A drink or food item in the catalogue — the app's central entity. Routed on by name
/// (<c>DataViewMap&lt;ProductDetailPage, ProductDetailModel, ProductItem&gt;</c>), rendered by the
/// shared <c>ProductCardTemplate</c>, and the item type of the Home and Menu list feeds.
/// </summary>
/// <remarks>
/// The convention every entity in this folder follows: they are immutable <c>partial record</c>s,
/// which MVUX requires — a Model's feeds and states hand out snapshots, so an entity that could be
/// mutated in place would let a view change data behind the Model's back. Records that flow through a
/// list also declare <c>[property: Key]</c> on their identifier, so add / update / remove and
/// selection match the right item by identity rather than by reference; key equality is generated for
/// a partial record, so declaring the attribute is all it takes. <see cref="CartItem"/> and
/// <see cref="OrderRecord"/> are the two that most need it — both live in an <c>IListState&lt;T&gt;</c>
/// on the shared CartService, where items really are added and removed one at a time.
/// </remarks>
public partial record ProductItem(
    [property: global::Uno.Extensions.Equality.Key] string Id,
    string Name,
    string Description,
    string Category,
    string CategoryId,
    string Price,
    double PriceValue,
    string ImageUrl,
    bool IsFeatured,
    bool IsSpecial);
