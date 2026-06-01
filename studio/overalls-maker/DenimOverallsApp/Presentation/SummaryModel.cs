using Uno.Extensions.Navigation;

namespace DenimOverallsApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(true)]
public partial record SummaryModel
{
    private readonly INavigator _navigator;

    public SummaryModel(OverallConfiguration configuration, INavigator navigator)
    {
        Configuration = configuration;
        _navigator = navigator;
        LineItems = BuildLineItems(configuration);
    }

    public OverallConfiguration Configuration { get; }

    // ── Configuration snapshot ────────────────────────────────────────────
    public string LengthLabel => OverallCatalog.LengthLabel(Configuration.LengthOption);
    public string BibLabel    => OverallCatalog.BibLabel(Configuration.BibType);
    public string ColorName   => OverallCatalog.ColorLabel(Configuration.DenimColor);
    public string PocketLabel => OverallCatalog.PocketLabel(Configuration.PocketType);
    public string ColorHex    => Configuration.DenimColorHex;
    public string CustomText  => Configuration.CustomText;
    public bool   HasCustomText => !string.IsNullOrWhiteSpace(Configuration.CustomText);

    // ── Price breakdown ───────────────────────────────────────────────────
    public IReadOnlyList<PriceLineItem> LineItems { get; }

    public string SubtotalLabel => OverallCatalog.Format(Configuration.TotalPrice);
    public string ShippingLabel => OverallCatalog.Format(OverallCatalog.ShippingFee);
    public string TotalLabel    => OverallCatalog.Format(Configuration.TotalPrice + OverallCatalog.ShippingFee);

    // ── Delivery info ─────────────────────────────────────────────────────
    public string LeadTime   => "14–21 business days";
    public string ShipMethod => "Standard tracked shipping";

    // ── Navigation ────────────────────────────────────────────────────────
    /// <summary>Carries the configuration forward to the checkout form.</summary>
    public ValueTask ProceedToCheckout(CancellationToken ct)
        => new(_navigator.NavigateRouteAsync(this, "Checkout", data: Configuration, cancellation: ct));

    private static IReadOnlyList<PriceLineItem> BuildLineItems(OverallConfiguration c)
    {
        var items = new List<PriceLineItem>
        {
            new("Base Overall", OverallCatalog.Format(c.BasePrice), false),
            new($"{OverallCatalog.LengthLabel(c.LengthOption)} Length", Plus(OverallCatalog.LengthAdjustment(c.LengthOption)), false),
            new($"{OverallCatalog.BibLabel(c.BibType)}",                Plus(OverallCatalog.BibAdjustment(c.BibType)),       false),
            new($"{OverallCatalog.ColorLabel(c.DenimColor)} Dye",      Plus(OverallCatalog.ColorAdjustment(c.DenimColor)),  false),
            new($"{OverallCatalog.PocketLabel(c.PocketType)}",         Plus(OverallCatalog.PocketAdjustment(c.PocketType)), false),
        };

        if (!string.IsNullOrWhiteSpace(c.CustomText))
        {
            items.Add(new PriceLineItem("Custom Embroidery", Plus(0m), true));
        }

        if (c.HasLogo)
        {
            items.Add(new PriceLineItem("Uno Logo Patch", "-" + OverallCatalog.Format(OverallCatalog.LogoDiscount), true));
        }

        return items;
    }

    private static string Plus(decimal value) => "+" + OverallCatalog.Format(value);
}

public partial record PriceLineItem(
    string Label,
    string Amount,
    bool   IsOptional);
