namespace QuoteCraft.Presentation;

/// <summary>The quote that catalog items are added to.</summary>
public record CatalogBrowserRequest(string QuoteId);

/// <summary>
/// Add from Catalog (route <c>!CatalogBrowser</c>): search and filter the catalog and add items to the quote, one tap
/// each, with the business's default markup. The dialog stays open; the caller reloads the quote when it closes.
/// </summary>
public partial record CatalogBrowserModel
{
    private const string AllCategories = "All";

    private readonly string _quoteId;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly IQuoteRepository _quoteRepo;
    private readonly IBusinessProfileRepository _profileRepo;

    public CatalogBrowserModel(
        CatalogBrowserRequest request,
        ICatalogItemRepository catalogRepo,
        IQuoteRepository quoteRepo,
        IBusinessProfileRepository profileRepo)
    {
        _quoteId = request.QuoteId;
        _catalogRepo = catalogRepo;
        _quoteRepo = quoteRepo;
        _profileRepo = profileRepo;
    }

    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);

    public IState<string> SelectedCategory => State<string>.Value(this, () => AllCategories);

    /// <summary>What the last tap added, or why it could not.</summary>
    public IState<string> Status => State<string>.Value(this, () => string.Empty);

    private IFeed<List<CatalogItemEntity>> Catalog => Feed.Async(async ct => await _catalogRepo.GetAllAsync());

    public IListFeed<string> Categories => Catalog
        .Select(items => (IImmutableList<string>)new[] { AllCategories }
            .Concat(items.Select(i => i.Category).Distinct().Order())
            .ToImmutableList())
        .AsListFeed();

    public IListFeed<CatalogItemEntity> Items => Feed.Combine(Catalog, SearchText, SelectedCategory)
        .Select(inputs =>
        {
            var (items, search, category) = inputs;
            IEnumerable<CatalogItemEntity> filtered = items;
            if (!string.IsNullOrEmpty(category) && category != AllCategories)
            {
                filtered = filtered.Where(i => i.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(i =>
                    i.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    i.Category.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            return (IImmutableList<CatalogItemEntity>)filtered.ToImmutableList();
        })
        .AsListFeed();

    public async ValueTask AddItem(CatalogItemEntity item, CancellationToken ct)
    {
        try
        {
            var profile = await _profileRepo.GetAsync();
            await _quoteRepo.SaveLineItemAsync(new LineItemEntity
            {
                QuoteId = _quoteId,
                Description = item.Description,
                UnitPrice = Money.WithMarkup(item.UnitPrice, profile.DefaultMarkup),
                Quantity = 1,
            });
            await Status.UpdateAsync(_ => $"Added \"{item.Description}\" to the quote.", ct);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Add a catalog item");
            await Status.UpdateAsync(_ => ErrorReporter.Describe($"\"{item.Description}\" could not be added.", ex), ct);
        }
    }
}
