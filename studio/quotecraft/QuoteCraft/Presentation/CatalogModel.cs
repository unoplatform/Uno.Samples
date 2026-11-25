namespace QuoteCraft.Presentation;

public partial record CatalogCategoryCard(
    string Name,
    string IconGlyph,
    int ItemCount);

// An item in an open category, as the item list shows it
public partial record CatalogDisplayItem(
    string Id,
    string Description,
    decimal UnitPrice,
    string Category)
{
    public static CatalogDisplayItem FromEntity(CatalogItemEntity entity) =>
        new(entity.Id, entity.Description, entity.UnitPrice, entity.Category);

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
    private readonly ICatalogItemRepository _catalogRepo;

    private readonly INavigator _navigator;

    public CatalogModel(INavigator navigator, ICatalogItemRepository catalogRepo)
    {
        _navigator = navigator;
        _catalogRepo = catalogRepo;
    }

    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);
    public IState<string> SelectedCategory => State<string>.Empty(this);
    public IState<int> Version => State<int>.Value(this, () => 0);

    public IFeed<int> CatalogItemCount => Version.SelectAsync(async (_, ct) => await _catalogRepo.GetItemCountAsync());

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

    /// <summary>One-pane layouts (below 1080): close the category and show the cards again.</summary>
    public async ValueTask BackToList(CancellationToken ct)
    {
        await SelectedCategory.UpdateDataAsync(_ => Option.None<string>(), ct);
    }

    public async ValueTask SelectCategoryCard(CatalogCategoryCard card, CancellationToken ct)
    {
        // Toggle: a second tap closes the category (no selection: the detail shows its NoneTemplate)
        if (await SelectedCategory == card.Name)
        {
            await SelectedCategory.UpdateDataAsync(_ => Option.None<string>(), ct);
        }
        else
        {
            await SelectedCategory.UpdateAsync(_ => card.Name, ct);
        }
    }

    /// <summary>A tap (or Enter) on an item: the item dialog, as stored now.</summary>
    public ValueTask EditItem(CatalogDisplayItem item, CancellationToken ct) => _navigator.RunAsync(this, "The catalog item could not be opened.", async () =>
    {
        if (await _catalogRepo.GetByIdAsync(item.Id) is { } entity)
        {
            await EditInDialogAsync(entity, isNew: false, ct);
        }
    }, ct);

    public ValueTask AddItem(CancellationToken ct) => _navigator.RunAsync(this, "The catalog item could not be added.",
        async () => await EditInDialogAsync(new CatalogItemEntity(), isNew: true, ct), ct);

    /// <summary>Add Item in an open category: the category is filled in.</summary>
    public ValueTask AddItemToCategory(CancellationToken ct) => _navigator.RunAsync(this, "The catalog item could not be added.", async () =>
    {
        var entity = new CatalogItemEntity();
        if (await SelectedCategory is { Length: > 0 } category)
        {
            entity.Category = category;
        }
        await EditInDialogAsync(entity, isNew: true, ct);
    }, ct);

    /// <summary>The item dialog saves the item; Delete there asks once more, then removes it.</summary>
    private async Task EditInDialogAsync(CatalogItemEntity entity, bool isNew, CancellationToken ct)
    {
        var categories = await _catalogRepo.GetCategoriesAsync();
        var result = await _navigator.ShowDialogAsync<CatalogItemEditResult>(this, "CatalogItemEditor",
            new CatalogItemEditRequest(entity, isNew, categories), ct);
        if (result is null)
        {
            return;
        }

        if (result.DeleteRequested)
        {
            if (!await _navigator.ConfirmAsync(this, new ConfirmRequest("\uE74D", "Delete Catalog Item?",
                    "This item will be removed from your catalog. Quotes that use it keep their line.", "Delete", "Cancel"), ct))
            {
                return;
            }
            await _catalogRepo.DeleteAsync(entity.Id);
        }
        await Version.UpdateAsync(v => v + 1, ct);
    }
}
