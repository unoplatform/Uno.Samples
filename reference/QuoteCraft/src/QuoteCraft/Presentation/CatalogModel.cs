namespace QuoteCraft.Presentation;

// Summary card for each category
public partial record CatalogCategoryCard(
    string Name,
    string IconGlyph,
    int ItemCount);

// Individual catalog item (flat, no headers needed)
public partial record CatalogDisplayItem(
    string Id,
    string Description,
    double UnitPrice,
    string Category,
    bool IsHeader,
    bool IsExpanded = true,
    string IconGlyph = "")
{
    public static CatalogDisplayItem FromEntity(CatalogItemEntity entity) =>
        new(entity.Id, entity.Description, (double)entity.UnitPrice, entity.Category, false);

    public static string GetCategoryIcon(string category) => category switch
    {
        "Plumbing" => "\uE81C",
        "Electrical" => "\uE945",
        "General Contracting" => "\uE80F",
        "Painting" => "\uE790",
        "HVAC" => "\uE9CA",
        "Materials" => "\uE82D",
        "Labor" => "\uE716",
        _ => "\uE82D",
    };
}

// Composite detail for the right pane (avoids nested FeedView with ElementName)
public partial record CategoryDetail(
    string Name,
    string IconGlyph,
    IImmutableList<CatalogDisplayItem> Items);

public partial record CatalogModel
{
    private readonly INavigator _navigator;
    private readonly ICatalogItemRepository _catalogRepo;

    public CatalogModel(INavigator navigator, ICatalogItemRepository catalogRepo)
    {
        _navigator = navigator;
        _catalogRepo = catalogRepo;
    }

    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);
    public IState<string> SelectedCategory => State<string>.Empty(this);
    public IState<int> Version => State<int>.Value(this, () => 0);

    public IFeed<int> CatalogItemCount => Feed.Async(async ct => await _catalogRepo.GetItemCountAsync());

    public IListFeed<string> Categories => Feed.Async(async ct =>
        (IImmutableList<string>)(await _catalogRepo.GetCategoriesAsync()).ToImmutableList()
    ).AsListFeed();

    // Category cards for the grid
    public IListFeed<CatalogCategoryCard> CategoryCards =>
        Feed.Combine(SearchText, Version).SelectAsync(async (inputs, ct) =>
        {
            var (search, _) = inputs;
            var all = await _catalogRepo.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                all = all.Where(i =>
                    i.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    i.Category.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return (IImmutableList<CatalogCategoryCard>)all
                .GroupBy(i => i.Category)
                .OrderBy(g => g.Key)
                .Select(g => new CatalogCategoryCard(
                    g.Key,
                    CatalogDisplayItem.GetCategoryIcon(g.Key),
                    g.Count()))
                .ToImmutableList();
        }).AsListFeed();

    // Detail feed for right pane - follows ClientsModel pattern (Empty state = NoneTemplate)
    public IFeed<CategoryDetail> SelectedCategoryDetail =>
        Feed.Combine(SelectedCategory, Version).SelectAsync(async (inputs, ct) =>
        {
            var (category, _) = inputs;
            var all = await _catalogRepo.GetAllAsync();
            var filtered = all
                .Where(i => i.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            var items = filtered.Select(CatalogDisplayItem.FromEntity).ToImmutableList();
            var icon = CatalogDisplayItem.GetCategoryIcon(category);

            return new CategoryDetail(category, icon, items);
        });

    public async ValueTask RefreshList(CancellationToken ct)
    {
        await Version.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask SelectCategoryCard(CatalogCategoryCard card, CancellationToken ct)
    {
        var current = await SelectedCategory;
        // Toggle: click again to deselect (set to null -> Empty -> NoneTemplate)
        if (current == card.Name)
            await SelectedCategory.UpdateAsync(_ => null!, ct);
        else
            await SelectedCategory.UpdateAsync(_ => card.Name, ct);
    }

    public async ValueTask OpenItem(CatalogDisplayItem item, CancellationToken ct)
    {
        if (item.IsHeader) return;
        var entity = await _catalogRepo.GetByIdAsync(item.Id);
        if (entity is not null)
            await _navigator.NavigateRouteAsync(this, "CatalogEditor", data: entity);
    }

    public async ValueTask AddItem(CancellationToken ct)
    {
        await _navigator.NavigateRouteAsync(this, "CatalogEditor", data: new CatalogItemEntity());
    }

    // ── Inline Dialog Support ──────────────────────────────────────────────
    // Exposed so dialog-driven code-behind can persist without service-locator lookups.

    public Task<List<string>> GetCategoriesAsync() => _catalogRepo.GetCategoriesAsync();

    public Task<CatalogItemEntity?> GetItemByIdAsync(string id) => _catalogRepo.GetByIdAsync(id);

    public async ValueTask SaveItemAsync(CatalogItemEntity entity, CancellationToken ct)
    {
        await _catalogRepo.SaveAsync(entity);
        await Version.UpdateAsync(v => v + 1, ct);
    }

    public async ValueTask DeleteItemAsync(string id, CancellationToken ct)
    {
        await _catalogRepo.DeleteAsync(id);
        await Version.UpdateAsync(v => v + 1, ct);
    }
}
