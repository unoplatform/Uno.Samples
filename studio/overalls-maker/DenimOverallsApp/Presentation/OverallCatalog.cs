using System.Globalization;

namespace DenimOverallsApp.Presentation;

/// <summary>
/// Single source of truth for the overall configurator: option catalogs, pricing rules
/// and label lookups. Shared by the configurator, summary and checkout models.
/// </summary>
public static class OverallCatalog
{
    public const decimal BasePrice = 129.00m;
    public const decimal ShippingFee = 12.00m;

    public static readonly IReadOnlyList<OverallOption> Lengths = new[]
    {
        new OverallOption("long",  "Long",  "Full-length bib overall, ankle cuff", ""),
        new OverallOption("short", "Short", "Shorts overall, mid-thigh cut",       ""),
    };

    public static readonly IReadOnlyList<OverallOption> Bibs = new[]
    {
        new OverallOption("classic",   "Classic Bib", "Straight bib with single chest pocket",      ""),
        new OverallOption("wide",      "Wide Bib",    "Broad bib, double chest pockets",            ""),
        new OverallOption("scoop",     "Scoop Bib",   "Low curved bib, relaxed silhouette",         ""),
        new OverallOption("crossback", "Cross-Back",  "Adjustable cross-back straps, no front bib", ""),
    };

    public static readonly IReadOnlyList<DenimColorOption> Colors = new[]
    {
        new DenimColorOption("indigo", "Deep Indigo",  "#1B3F7A"),
        new DenimColorOption("mid",    "Mid Wash",     "#3B6FA0"),
        new DenimColorOption("light",  "Light Wash",   "#8DB4D8"),
        new DenimColorOption("black",  "Jet Black",    "#1A1A2E"),
        new DenimColorOption("ecru",   "Raw Ecru",     "#D8C9A3"),
        new DenimColorOption("stone",  "Stone Bleach", "#C5BEA8"),
    };

    public static readonly IReadOnlyList<OverallOption> Pockets = new[]
    {
        new OverallOption("patch", "Patch Pockets",   "Classic large patch pockets on the thighs",  ""),
        new OverallOption("cargo", "Cargo Pockets",   "Side cargo pockets with brass button flaps", ""),
        new OverallOption("slim",  "Slim Pockets",    "Minimal hidden side-seam pockets",           ""),
        new OverallOption("none",  "No Side Pockets", "Clean lines, chest pocket only",             ""),
    };

    public static OverallConfiguration CreateDefault()
        => Reprice(new OverallConfiguration(
            LengthOption:  "long",
            BibType:       "classic",
            DenimColor:    "indigo",
            DenimColorHex: "#1B3F7A",
            CustomText:    "",
            PocketType:    "patch",
            BasePrice:     BasePrice,
            TotalPrice:    0m));

    /// <summary>Returns a copy of <paramref name="c"/> with <see cref="OverallConfiguration.TotalPrice"/> recomputed.</summary>
    public static OverallConfiguration Reprice(OverallConfiguration c)
        => c with
        {
            TotalPrice = c.BasePrice
                + LengthAdjustment(c.LengthOption)
                + BibAdjustment(c.BibType)
                + ColorAdjustment(c.DenimColor)
                + PocketAdjustment(c.PocketType),
        };

    public static decimal LengthAdjustment(string id) => id switch
    {
        "long"  => 20m,
        "short" => 0m,
        _       => 0m,
    };

    public static decimal BibAdjustment(string id) => id switch
    {
        "classic"   => 0m,
        "wide"      => 12m,
        "scoop"     => 8m,
        "crossback" => 15m,
        _           => 0m,
    };

    public static decimal ColorAdjustment(string id) => id switch
    {
        "indigo" => 0m,
        "mid"    => 0m,
        "light"  => 6m,
        "black"  => 10m,
        "ecru"   => 12m,
        "stone"  => 12m,
        _        => 0m,
    };

    public static decimal PocketAdjustment(string id) => id switch
    {
        "patch" => 10m,
        "cargo" => 15m,
        "slim"  => 6m,
        "none"  => 0m,
        _       => 0m,
    };

    public static string LengthLabel(string id) => LabelOf(Lengths, id);
    public static string BibLabel(string id) => LabelOf(Bibs, id);
    public static string PocketLabel(string id) => LabelOf(Pockets, id);
    public static string ColorLabel(string id) => Colors.FirstOrDefault(o => o.Id == id)?.Label ?? string.Empty;
    public static string ColorHex(string id) => Colors.FirstOrDefault(o => o.Id == id)?.HexColor ?? "#1B3F7A";

    public static string Format(decimal value) => value.ToString("C", PriceCulture);

    private static readonly CultureInfo PriceCulture = new("en-US");

    private static string LabelOf(IEnumerable<OverallOption> options, string id)
        => options.FirstOrDefault(o => o.Id == id)?.Label ?? string.Empty;
}
