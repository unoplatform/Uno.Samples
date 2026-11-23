namespace QuoteCraft.Presentation;

public partial record ShellModel
{
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly CurrencyService _currency;
    private readonly ILogger<ShellModel> _logger;

    public ShellModel(
        IBusinessProfileRepository profileRepo,
        CurrencyService currency,
        ILogger<ShellModel> logger)
    {
        _profileRepo = profileRepo;
        _currency = currency;
        _logger = logger;

        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        try
        {
            // The business's currency, for every amount on screen.
            _currency.Code = (await _profileRepo.GetAsync()).CurrencyCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize shell");
        }
    }
}
