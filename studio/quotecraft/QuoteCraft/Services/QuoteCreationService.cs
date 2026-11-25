namespace QuoteCraft.Services;

/// <summary>
/// Makes new quotes, the one place that does: New Quote, a client's New Quote, onboarding's first quote and Duplicate.
/// A new quote is a Draft with the next number (<see cref="QuoteNumberGenerator"/>, D10), the business's default tax
/// rate, and a valid-until date the business's number of days from now. Plan limits are the callers' (each asks in its
/// own way, and only where plans exist: D3).
/// </summary>
public class QuoteCreationService
{
    private readonly IQuoteRepository _quoteRepo;
    private readonly IClientRepository _clientRepo;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly QuoteNumberGenerator _numbers;

    public QuoteCreationService(
        IQuoteRepository quoteRepo,
        IClientRepository clientRepo,
        IBusinessProfileRepository profileRepo,
        QuoteNumberGenerator numbers)
    {
        _quoteRepo = quoteRepo;
        _clientRepo = clientRepo;
        _profileRepo = profileRepo;
        _numbers = numbers;
    }

    /// <summary>
    /// Saves a new Draft and returns it. A client name links the quote to the client with that name (ignoring case);
    /// a name no client has stays on the quote unlinked (<see cref="ClientLink"/>).
    /// </summary>
    public async Task<QuoteEntity> CreateDraftAsync(string title, string? clientName = null)
    {
        var name = clientName?.Trim();
        var clientId = string.IsNullOrEmpty(name) ? null : ClientLink.Resolve(name, null, null, await _clientRepo.GetAllAsync());
        return await SaveDraftAsync(title, name, clientId);
    }

    /// <summary>Saves a new Draft for <paramref name="client"/> and returns it.</summary>
    public Task<QuoteEntity> CreateDraftForClientAsync(string title, ClientEntity client) =>
        SaveDraftAsync(title, client.Name, client.Id);

    /// <summary>
    /// Saves a copy of the quote as a new Draft ("… (Copy)") with the same client, notes, tax rate and line items, and
    /// returns it; null when the quote is gone.
    /// </summary>
    public async Task<QuoteEntity?> DuplicateAsync(string quoteId)
    {
        if (await _quoteRepo.GetByIdAsync(quoteId) is not { } source)
        {
            return null;
        }

        var copy = await NewDraftAsync(source.Title + " (Copy)", source.ClientName, source.ClientId);
        copy.Notes = source.Notes;
        copy.TaxRate = source.TaxRate;
        await _quoteRepo.SaveAsync(copy);

        foreach (var item in source.LineItems)
        {
            await _quoteRepo.SaveLineItemAsync(new LineItemEntity
            {
                QuoteId = copy.Id,
                Description = item.Description,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
            });
        }
        return copy;
    }

    private async Task<QuoteEntity> SaveDraftAsync(string title, string? clientName, string? clientId)
    {
        var quote = await NewDraftAsync(title, clientName, clientId);
        await _quoteRepo.SaveAsync(quote);
        return quote;
    }

    private async Task<QuoteEntity> NewDraftAsync(string title, string? clientName, string? clientId)
    {
        var profile = await _profileRepo.GetAsync();
        return new QuoteEntity
        {
            Title = title,
            QuoteNumber = await _numbers.GenerateAsync(),
            Status = QuoteStatus.Draft,
            ClientId = clientId,
            ClientName = string.IsNullOrEmpty(clientName) ? null : clientName,
            TaxRate = profile.DefaultTaxRate,
            ValidUntil = DateTimeOffset.UtcNow.AddDays(profile.QuoteValidDays),
        };
    }
}
