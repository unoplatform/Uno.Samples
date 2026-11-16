namespace DenimOverallsApp.Presentation;

/// <summary>Represents a single denim overall configuration option (e.g., a color swatch, pocket style, bib type).</summary>
public partial record OverallOption(
    string Id,
    string Label,
    string Description,
    string Glyph);

/// <summary>The user's complete customization selection — shared between configurator, summary and checkout.</summary>
public partial record OverallConfiguration(
    string LengthOption,
    string BibType,
    string DenimColor,
    string DenimColorHex,
    string CustomText,
    string PocketType,
    bool HasLogo,
    decimal BasePrice,
    decimal TotalPrice);

/// <summary>Color swatch option — carries a hex value for the swatch dot.</summary>
public partial record DenimColorOption(
    string Id,
    string Label,
    string HexColor);

/// <summary>An <see cref="OverallOption"/> paired with whether it is the current selection.</summary>
public partial record SelectableOption(
    OverallOption Option,
    bool IsSelected);

/// <summary>A <see cref="DenimColorOption"/> paired with whether it is the current selection.</summary>
public partial record SelectableColor(
    DenimColorOption Option,
    bool IsSelected);
