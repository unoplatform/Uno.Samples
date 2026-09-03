namespace Voyago.Presentation.MockData;

/// <summary>
/// Design-time DataContext for the destination detail surfaces in Hot Design / Studio, built to the
/// recipe documented on <see cref="HomePageMockData"/>. The page (mobile) and the ContentDialog
/// (desktop) share one Model shape, so one mock covers both. <see cref="IsBooked"/> is a plain bool
/// here: the live Model derives it from the shared trip book, which a design surface has no context
/// to pump. This mock has no feeds — it exists for that one state — so rule 4 does not apply.
/// </summary>
[ReactiveBindable]
public partial record DestinationDetailMockData
{
    /// <summary>Not yet booked: the "Book this trip" CTA in its default, actionable state.</summary>
    public static DestinationDetailMockDataViewModel Data => new();

    /// <summary>Already booked — the only other state either surface can be in.</summary>
    public static DestinationDetailMockDataViewModel Booked =>
        DestinationDetailMockDataViewModel.ForModel(new() { IsBooked = true });

    public string Name { get; init; } = Catalog.Santorini.Name;
    public string Country { get; init; } = Catalog.Santorini.Country;
    public string Tagline { get; init; } = Catalog.Santorini.Tagline;
    public string ImageUrl { get; init; } = Catalog.Santorini.ImageUrl;
    public string PriceFrom { get; init; } = Catalog.Santorini.PriceFrom;
    public double Rating { get; init; } = Catalog.Santorini.Rating;
    public string ReviewsText { get; init; } = $"{Catalog.Santorini.ReviewCount:N0} reviews";

    public bool IsBooked { get; init; }

    // Mirrors the Model's signature, not a void stub: MVUX generates a command from an
    // `async ValueTask` method, so a `void Book()` here would leave the preview's "Book this trip"
    // CTA bound to nothing and rendered disabled.
    public async ValueTask Book(CancellationToken ct) => await ValueTask.CompletedTask;
}

// Reaches the generated ViewModel's protected model-taking constructor, so Booked can wrap a
// customized model. See HomePageMockDataViewModel for the full explanation.
public partial class DestinationDetailMockDataViewModel
{
    internal static DestinationDetailMockDataViewModel ForModel(DestinationDetailMockData model) => new(model);
}
