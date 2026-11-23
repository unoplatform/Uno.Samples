namespace QuoteCraft.Presentation;

/// <summary>The catalog item to edit (a new one to add), and the categories the field offers.</summary>
public record CatalogItemEditRequest(CatalogItemEntity Item, bool IsNew, IReadOnlyList<string> Categories);

/// <summary>Saved, or Delete pressed (the caller asks once more before removing the item).</summary>
public record CatalogItemEditResult(bool DeleteRequested);

/// <summary>
/// Add or edit a catalog item (route <c>!CatalogItemEditor</c>). Prices are whole cents in the business's currency,
/// typed in the user's number format; an empty price is zero. Save writes the item; it stays open on a problem.
/// </summary>
public partial record CatalogItemEditorModel
{
    private readonly INavigator _navigator;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly CatalogItemEntity _item;
    private readonly CurrencyService _currency;

    public CatalogItemEditorModel(INavigator navigator, ICatalogItemRepository catalogRepo, CurrencyService currency, CatalogItemEditRequest request)
    {
        _navigator = navigator;
        _currency = currency;
        _catalogRepo = catalogRepo;
        _item = request.Item;
        IsEditing = !request.IsNew;
        Heading = request.IsNew ? "Add Catalog Item" : "Edit Catalog Item";
        Subheading = request.IsNew ? "Add a new item to your pricing catalog" : "Update item details and pricing";
        CategoryNames = request.Categories;
        Description = State<string>.Value(this, () => _item.Description ?? string.Empty);
        UnitPrice = State<string>.Value(this, () => request.IsNew ? string.Empty : Money.FormatAmountInput(_item.UnitPrice));
        Category = State<string>.Value(this, () => _item.Category ?? string.Empty);
    }

    public bool IsEditing { get; }
    public string Heading { get; }
    public string Subheading { get; }
    public IReadOnlyList<string> CategoryNames { get; }

    /// <summary>The item's category when the list has it: the field starts on that entry.</summary>
    public string? InitialCategory => CategoryNames.Contains(_item.Category ?? string.Empty) ? _item.Category : null;

    public string UnitPriceHeader => $"Unit Price ({Money.Symbol(_currency.Code)})";
    public string UnitPricePlaceholder => Money.FormatAmountInput(0m);

    public IState<string> Description { get; }
    public IState<string> UnitPrice { get; }
    public IState<string> Category { get; }

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask Save(CancellationToken ct)
    {
        var description = (await Description)?.Trim() ?? string.Empty;
        if (description.Length == 0)
        {
            await ValidationError.UpdateAsync(_ => "Enter a description.", ct);
            return;
        }

        var priceText = await UnitPrice;
        var price = 0m;
        if (!string.IsNullOrWhiteSpace(priceText) && !Money.TryParseAmount(priceText, out price))
        {
            await ValidationError.UpdateAsync(_ => $"Enter the unit price as an amount, for example {Money.FormatAmountInput(125m)}.", ct);
            return;
        }

        _item.Description = description;
        _item.Category = (await Category)?.Trim() ?? string.Empty;
        _item.UnitPrice = price;
        try
        {
            await _catalogRepo.SaveAsync(_item);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Save catalog item");
            await ValidationError.UpdateAsync(_ => "The item could not be saved. Try again.", ct);
            return;
        }
        await _navigator.NavigateBackWithResultAsync(this, data: new CatalogItemEditResult(DeleteRequested: false), cancellation: ct);
    }

    public async ValueTask Delete(CancellationToken ct) =>
        await _navigator.NavigateBackWithResultAsync(this, data: new CatalogItemEditResult(DeleteRequested: true), cancellation: ct);
}
