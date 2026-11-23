namespace QuoteCraft.Presentation;

/// <summary>The client to edit; a new <see cref="ClientEntity"/> to add one.</summary>
public record ClientEditRequest(ClientEntity Client, bool IsNew);

/// <summary>
/// Add or edit a client (route <c>!ClientEditor</c>). Save checks the name and email, writes the client and returns
/// it; it stays open with a message when something is wrong or the save fails.
/// </summary>
public partial record ClientEditorModel
{
    private readonly INavigator _navigator;
    private readonly IClientRepository _clientRepo;
    private readonly ClientEntity _client;

    public ClientEditorModel(INavigator navigator, IClientRepository clientRepo, ClientEditRequest request)
    {
        _navigator = navigator;
        _clientRepo = clientRepo;
        _client = request.Client;
        Heading = request.IsNew ? "Add Client" : "Edit Client";
        Subheading = request.IsNew ? "Add a new client to your contact list" : "Update client details";
        Name = State<string>.Value(this, () => _client.Name ?? string.Empty);
        Email = State<string>.Value(this, () => _client.Email ?? string.Empty);
        Phone = State<string>.Value(this, () => _client.Phone ?? string.Empty);
        Address = State<string>.Value(this, () => _client.Address ?? string.Empty);
    }

    public string Heading { get; }
    public string Subheading { get; }

    public IState<string> Name { get; }
    public IState<string> Email { get; }
    public IState<string> Phone { get; }
    public IState<string> Address { get; }

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask Save(CancellationToken ct)
    {
        var name = (await Name)?.Trim() ?? string.Empty;
        if (name.Length == 0)
        {
            await ValidationError.UpdateAsync(_ => "Enter the client's name.", ct);
            return;
        }

        var email = (await Email)?.Trim() ?? string.Empty;
        if (!EmailAddress.IsValidOrEmpty(email))
        {
            await ValidationError.UpdateAsync(_ => "Enter a valid email address, or leave it empty.", ct);
            return;
        }

        _client.Name = name;
        _client.Email = email;
        _client.Phone = await Phone;
        _client.Address = await Address;
        _client.UpdatedAt = DateTimeOffset.UtcNow;
        try
        {
            await _clientRepo.SaveAsync(_client);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Save client");
            await ValidationError.UpdateAsync(_ => "The client could not be saved. Try again.", ct);
            return;
        }
        await _navigator.NavigateBackWithResultAsync(this, data: _client, cancellation: ct);
    }
}
