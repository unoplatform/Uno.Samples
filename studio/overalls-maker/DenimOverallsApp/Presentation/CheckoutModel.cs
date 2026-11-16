namespace DenimOverallsApp.Presentation;

[Uno.Extensions.Reactive.ReactiveBindable(true)]
public partial record CheckoutModel
{
    public CheckoutModel(OverallConfiguration configuration)
    {
        Configuration = configuration;
    }

    public OverallConfiguration Configuration { get; }

    // ── Form pre-fill hints (labels/placeholders only) ────────────────────
    public string FirstName    { get; } = "";
    public string LastName     { get; } = "";
    public string Email        { get; } = "";
    public string Phone        { get; } = "";
    public string AddressLine1 { get; } = "";
    public string AddressLine2 { get; } = "";
    public string City         { get; } = "";
    public string PostalCode   { get; } = "";
    public string Country      { get; } = "United States";

    // ── Payment method options ────────────────────────────────────────────
    public IReadOnlyList<PaymentMethod> PaymentMethods { get; } = new[]
    {
        new PaymentMethod("card",   "Credit / Debit Card", "\uE8C5"),
        new PaymentMethod("paypal", "PayPal",              "\uE8C5"),
        new PaymentMethod("klarna", "Pay in 3 (Klarna)",   "\uE8C5"),
    };

    public string SelectedPayment { get; } = "card";

    // ── Order summary strip ───────────────────────────────────────────────
    public string OrderTotal      => OverallCatalog.Format(Configuration.TotalPrice + OverallCatalog.ShippingFee);
    public string LeadTimeSummary { get; } = "Ships in 14–21 business days";

    // ── Security assurance chips ──────────────────────────────────────────
    public IReadOnlyList<string> TrustBadges { get; } = new[]
    {
        "SSL Encrypted",
        "Free Returns",
        "Handcrafted Quality",
        "Lifetime Guarantee",
    };
}

public partial record PaymentMethod(
    string Id,
    string Label,
    string Glyph);
