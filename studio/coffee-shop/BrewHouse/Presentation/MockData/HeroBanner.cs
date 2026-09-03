namespace BrewHouse.Presentation.MockData;

/// <summary>
/// A slide in the Home page's hero carousel. Carries no identifier: the banners are presentation-only
/// and are never added, removed or selected individually, so there is nothing for a key to match —
/// see <see cref="ProductItem"/> for the convention.
/// </summary>
public partial record HeroBanner(
    string ImageUrl,
    string Title,
    string Subtitle);
