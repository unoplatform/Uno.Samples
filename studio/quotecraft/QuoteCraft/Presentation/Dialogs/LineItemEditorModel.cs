using System.Globalization;

namespace QuoteCraft.Presentation;

/// <summary>The line item to edit, or none to add one.</summary>
public record LineItemEditRequest(LineItemEntity? Item);

/// <summary>What the line-item dialog did: the item to save (its id kept when editing), or the id to delete.</summary>
public record LineItemEditResult(LineItemEntity? Saved, string? DeletedId);

/// <summary>
/// Add or edit a line item (route <c>!LineItemEditor</c>). Prices are in the business's currency and typed in the
/// user's number format; nothing is saved until the three fields read cleanly (<see cref="LineItemInput"/>).
/// </summary>
public partial record LineItemEditorModel
{
    private readonly INavigator _navigator;
    private readonly string? _itemId;
    private readonly CurrencyService _currency;

    public LineItemEditorModel(INavigator navigator, CurrencyService currency, LineItemEditRequest request)
    {
        _navigator = navigator;
        _currency = currency;
        var item = request.Item;
        _itemId = item?.Id;
        IsEditing = item is not null;
        Description = State<string>.Value(this, () => item?.Description ?? string.Empty).ForEach(RevalidateAsync);
        UnitPrice = State<string>.Value(this, () => item is null ? string.Empty : Money.FormatAmountInput(item.UnitPrice)).ForEach(RevalidateAsync);
        Quantity = State<string>.Value(this, () => (item?.Quantity ?? 1).ToString(CultureInfo.CurrentCulture)).ForEach(RevalidateAsync);
    }

    /// <summary>
    /// While a message shows, each edit reads the form again: the message goes when the field is fixed, or names what is
    /// still wrong. Before the first Save, typing shows nothing.
    /// </summary>
    private async ValueTask RevalidateAsync(string? changed, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(await ValidationError))
        {
            return;
        }
        var error = LineItemInput.TryRead(await Description, await UnitPrice, await Quantity, out _, out _, out _);
        await ValidationError.UpdateAsync(_ => error ?? string.Empty, ct);
    }

    /// <summary>Editing shows Delete; adding does not.</summary>
    public bool IsEditing { get; }

    public string UnitPriceHeader => $"Unit Price ({Money.Symbol(_currency.Code)})";

    public string UnitPricePlaceholder => Money.FormatAmountInput(0m);

    public IState<string> Description { get; }
    public IState<string> UnitPrice { get; }
    public IState<string> Quantity { get; }

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    /// <summary>The running line total, or a dash while the price or quantity does not read.</summary>
    public IFeed<string> LineTotal => Feed.Combine(UnitPrice, Quantity).Select(inputs =>
    {
        var (priceText, quantityText) = inputs;
        var price = 0m;
        var priceReads = string.IsNullOrWhiteSpace(priceText) || Money.TryParseAmount(priceText, out price);
        var quantityReads = int.TryParse(quantityText, NumberStyles.None, CultureInfo.CurrentCulture, out var quantity) && quantity >= 1;
        return priceReads && quantityReads
            ? Money.Format(Money.LineTotal(price, quantity), _currency.Code)
            : "—";
    });

    public async ValueTask IncreaseQuantity(CancellationToken ct) =>
        await Quantity.UpdateAsync(q => int.TryParse(q, out var n) ? (n + 1).ToString(CultureInfo.CurrentCulture) : "1", ct);

    public async ValueTask DecreaseQuantity(CancellationToken ct) =>
        await Quantity.UpdateAsync(q => int.TryParse(q, out var n) && n > 1 ? (n - 1).ToString(CultureInfo.CurrentCulture) : q, ct);

    /// <summary>Says what is wrong and keeps the dialog open, rather than saving a guess.</summary>
    public async ValueTask Save(CancellationToken ct)
    {
        var error = LineItemInput.TryRead(await Description, await UnitPrice, await Quantity,
            out var description, out var unitPrice, out var quantity);
        if (error is not null)
        {
            await ValidationError.UpdateAsync(_ => error, ct);
            return;
        }

        var saved = new LineItemEntity { Description = description, UnitPrice = unitPrice, Quantity = quantity };
        if (_itemId is { Length: > 0 } id)
        {
            saved.Id = id;
        }
        await _navigator.NavigateBackWithResultAsync(this, data: new LineItemEditResult(saved, null), cancellation: ct);
    }

    public async ValueTask Delete(CancellationToken ct)
    {
        if (_itemId is { Length: > 0 } id)
        {
            await _navigator.NavigateBackWithResultAsync(this, data: new LineItemEditResult(null, id), cancellation: ct);
        }
    }
}
