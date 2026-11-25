namespace QuoteCraft.Presentation;

public partial record ClientsModel
{
    private readonly INavigator _navigator;
    private readonly IClientRepository _clientRepo;
    private readonly IQuoteRepository _quoteRepo;
    private readonly QuoteCreationService _creation;
    private readonly IFeatureGateService _featureGate;

    public ClientsModel(
        INavigator navigator,
        IClientRepository clientRepo,
        IQuoteRepository quoteRepo,
        QuoteCreationService creation,
        IFeatureGateService featureGate)
    {
        _navigator = navigator;
        _clientRepo = clientRepo;
        _quoteRepo = quoteRepo;
        _creation = creation;
        _featureGate = featureGate;
    }

    public IState<string> SearchText => State<string>.Value(this, () => string.Empty);
    public IState<ClientDisplayItem> SelectedClient => State<ClientDisplayItem>.Empty(this);
    public IState<int> Version => State<int>.Value(this, () => 0);

    public IListFeed<ClientDisplayItem> Clients =>
        Feed.Combine(SearchText, Version).SelectAsync(async (inputs, ct) =>
        {
            var (search, _) = inputs;
            var clients = await _clientRepo.GetAllAsync();
            var quotes = await _quoteRepo.GetAllAsync();

            // Pre-group quotes by ClientId: O(N+M) instead of O(N*M)
            var quotesByClient = quotes
                .GroupBy(q => q.ClientId ?? "")
                .ToDictionary(g => g.Key, g => g.ToList());

            var items = clients.Select(c =>
            {
                var clientQuotes = quotesByClient.GetValueOrDefault(c.Id, []);
                var initials = GetInitials(c.Name);
                var city = ParseCity(c.Address);
                var totalValue = clientQuotes.Sum(q => q.Total);

                return new ClientDisplayItem(c.Id, c.Name, initials, clientQuotes.Count, city, totalValue, c);
            });

            if (!string.IsNullOrWhiteSpace(search))
            {
                items = items.Where(i =>
                    i.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    i.City.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return (IImmutableList<ClientDisplayItem>)items.ToImmutableList();
        })
        .AsListFeed();

    public IFeed<int> ClientCount => Version
        .SelectAsync(async (_, ct) => (await _clientRepo.GetAllAsync()).Count);

    public IFeed<ClientsAnalytics> Analytics => Version
        .SelectAsync(async (_, ct) =>
        {
            var clients = await _clientRepo.GetAllAsync();
            var quotes = await _quoteRepo.GetAllAsync();

            var totalClients = clients.Count;
            var totalRevenue = quotes
                .Where(q => q.Status == QuoteStatus.Accepted)
                .Sum(q => q.Total);
            var avgPerClient = totalClients > 0 ? Money.RoundToCents(totalRevenue / totalClients) : 0m;

            return new ClientsAnalytics(totalClients, totalRevenue, avgPerClient);
        });

    // Combined detail feed: client + quotes (avoids nested FeedView with ElementName). Read again on every Version
    // bump, so an edit shows in the detail and Retry reloads it.
    public IFeed<ClientDetail> SelectedClientDetail => Feed.Combine(SelectedClient, Version)
        .SelectAsync(async (inputs, ct) =>
        {
            var (client, _) = inputs;
            var entity = await _clientRepo.GetByIdAsync(client.Id) ?? client.Entity;
            var all = await _quoteRepo.GetAllAsync();
            var quotes = all.Where(q => q.ClientId == client.Id).ToImmutableList();
            var item = new ClientDisplayItem(entity.Id, entity.Name, GetInitials(entity.Name), quotes.Count,
                ParseCity(entity.Address), quotes.Sum(q => q.Total), entity);
            return new ClientDetail(item, quotes);
        });

    /// <summary>One-pane layouts (below 1080): close the detail and show the list again.</summary>
    public async ValueTask BackToList(CancellationToken ct)
    {
        await SelectedClient.UpdateDataAsync(_ => Option.None<ClientDisplayItem>(), ct);
    }

    public async ValueTask SelectClient(ClientDisplayItem item, CancellationToken ct)
    {
        await SelectedClient.UpdateAsync(_ => item, ct);
    }

    public async ValueTask RefreshList(CancellationToken ct)
    {
        await Version.UpdateAsync(v => v + 1, ct);
    }

    public ValueTask DeleteClient(CancellationToken ct) => _navigator.RunAsync(this, "The client could not be deleted.", async () =>
    {
        if (await SelectedClient is not { } client
            || !await _navigator.ConfirmAsync(this, new ConfirmRequest("\uE74D", "Delete Client?",
                "This client will be removed from your contact list. Their quotes stay.", "Delete", "Cancel"), ct))
        {
            return;
        }

        await _clientRepo.DeleteAsync(client.Id);
        await SelectedClient.UpdateDataAsync(_ => Option.None<ClientDisplayItem>(), ct);
        await Version.UpdateAsync(v => v + 1, ct);
    }, ct);

    /// <summary>Add Client: the plan limit first (only where plans exist), then the client dialog.</summary>
    public ValueTask AddClient(CancellationToken ct) => _navigator.RunAsync(this, "The client could not be added.", async () =>
    {
        if (!await _featureGate.CanAddClientAsync())
        {
            if (await _navigator.ConfirmAsync(this, new ConfirmRequest("\uE77B", "Client Limit Reached",
                    _featureGate.GetUpgradeMessage("clients"), "Upgrade", "OK"), ct))
            {
                await OpenSettings(ct);
            }
            return;
        }

        await EditInDialogAsync(new ClientEditRequest(new ClientEntity(), IsNew: true), ct);
    }, ct);

    /// <summary>Edit: the client as stored now, in the client dialog.</summary>
    public ValueTask EditClient(CancellationToken ct) => _navigator.RunAsync(this, "The client could not be opened.", async () =>
    {
        if (await SelectedClient is { } selected && await _clientRepo.GetByIdAsync(selected.Id) is { } client)
        {
            await EditInDialogAsync(new ClientEditRequest(client, IsNew: false), ct);
        }
    }, ct);

    /// <summary>The dialog saves the client; the list and the detail read it again.</summary>
    private async Task EditInDialogAsync(ClientEditRequest request, CancellationToken ct)
    {
        if (await _navigator.ShowDialogAsync<ClientEntity>(this, "ClientEditor", request, ct) is not null)
        {
            await Version.UpdateAsync(v => v + 1, ct);
        }
    }

    /// <summary>A quote in the client's list: open it in the editor.</summary>
    public ValueTask OpenQuote(QuoteEntity quote, CancellationToken ct) => _navigator.RunAsync(this, "The quote could not be opened.",
        async () => await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote, cancellation: ct), ct);

    /// <summary>Plans and the upgrade live in Settings.</summary>
    public async ValueTask OpenSettings(CancellationToken ct)
    {
        await _navigator.NavigateRouteAsync(this, "Settings", cancellation: ct);
    }

    public ValueTask CreateQuoteForClient(CancellationToken ct) => _navigator.RunAsync(this, "The quote could not be created.", async () =>
    {
        var client = await SelectedClient;
        if (client is null) return;

        // A plan limit (only where plans exist) is reported like any failure.
        if (!await _featureGate.CanCreateQuoteAsync())
        {
            throw new InvalidOperationException(_featureGate.GetUpgradeMessage("quotes"));
        }

        var quote = await _creation.CreateDraftForClientAsync("New Quote", client.Entity);
        await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote);
    }, ct);

    private static string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[^1][0])}";
        return parts[0].Length >= 2
            ? $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[0][1])}"
            : $"{char.ToUpper(parts[0][0])}";
    }

    private static string ParseCity(string? address)
    {
        if (string.IsNullOrWhiteSpace(address)) return string.Empty;
        var lines = address.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length >= 2)
        {
            var cityLine = lines[^1].Trim();
            var parts = cityLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cityParts = parts.TakeWhile(p => !p.All(c => char.IsDigit(c) || c == '-')).ToArray();
            return cityParts.Length > 0 ? string.Join(" ", cityParts) : cityLine;
        }
        return address.Trim();
    }
}
