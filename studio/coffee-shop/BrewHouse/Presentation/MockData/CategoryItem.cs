namespace BrewHouse.Presentation.MockData;

/// <summary>
/// A filter chip in the Menu's category bar (and the Home page's category strip). Built to the entity
/// convention on <see cref="ProductItem"/>.
/// </summary>
public partial record CategoryItem(
    string Id,
    string Name,
    // Drives the filter chip's selected look in XAML (theme brushes), not a hardcoded colour.
    bool IsSelected = false);
