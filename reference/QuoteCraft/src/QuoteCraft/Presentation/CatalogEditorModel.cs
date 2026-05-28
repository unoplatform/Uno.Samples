namespace QuoteCraft.Presentation;

public partial record CatalogEditorModel(
    INavigator Navigator,
    ICatalogItemRepository CatalogRepo,
    CatalogItemEntity Entity)
{
    public IState<string> Description => State<string>.Value(this, () => Entity.Description);
    public IState<string> UnitPrice => State<string>.Value(this, () => Entity.UnitPrice.ToString("F2"));
    public IState<string> Category => State<string>.Value(this, () => Entity.Category);
    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask Save(CancellationToken ct)
    {
        var desc = (await Description)?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(desc))
        {
            await ValidationError.UpdateAsync(_ => "Description is required.", ct);
            return;
        }

        Entity.Description = desc;
        Entity.Category = await Category ?? string.Empty;

        var priceStr = await UnitPrice ?? "0";
        if (!decimal.TryParse(priceStr, out var price) || price < 0)
        {
            await ValidationError.UpdateAsync(_ => "Price must be a non-negative number.", ct);
            return;
        }
        Entity.UnitPrice = price;

        await ValidationError.UpdateAsync(_ => string.Empty, ct);
        await CatalogRepo.SaveAsync(Entity);
        await Navigator.GoBack(this);
    }

    public async ValueTask Delete(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(Entity.Id))
        {
            await CatalogRepo.DeleteAsync(Entity.Id);
        }
        await Navigator.GoBack(this);
    }
}
