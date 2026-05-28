namespace QuoteCraft.Presentation;

public partial record ClientEditorModel
{
    private readonly INavigator _navigator;
    private readonly IClientRepository _clientRepo;

    public ClientEditorModel(ClientEntity client, INavigator navigator, IClientRepository clientRepo)
    {
        _navigator = navigator;
        _clientRepo = clientRepo;
        Name = State<string>.Value(this, () => client.Name);
        Email = State<string>.Value(this, () => client.Email ?? string.Empty);
        Phone = State<string>.Value(this, () => client.Phone ?? string.Empty);
        Address = State<string>.Value(this, () => client.Address ?? string.Empty);
        _clientId = client.Id;
    }

    private readonly string _clientId;

    public IState<string> Name { get; }
    public IState<string> Email { get; }
    public IState<string> Phone { get; }
    public IState<string> Address { get; }
    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask Save(CancellationToken ct)
    {
        var name = (await Name)?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            await ValidationError.UpdateAsync(_ => "Client name is required.", ct);
            return;
        }

        var email = (await Email)?.Trim();
        if (!string.IsNullOrEmpty(email) && (!email.Contains('@') || !email.Contains('.')))
        {
            await ValidationError.UpdateAsync(_ => "Please enter a valid email address.", ct);
            return;
        }

        var phone = (await Phone)?.Trim();
        if (!string.IsNullOrEmpty(phone))
        {
            var digits = phone.Where(char.IsDigit).Count();
            if (digits < 7)
            {
                await ValidationError.UpdateAsync(_ => "Phone number must have at least 7 digits.", ct);
                return;
            }
        }

        await ValidationError.UpdateAsync(_ => string.Empty, ct);
        var client = new ClientEntity
        {
            Id = _clientId,
            Name = name,
            Email = email,
            Phone = phone,
            Address = await Address,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
        await _clientRepo.SaveAsync(client);
        await _navigator.NavigateBackAsync(this);
    }
}
