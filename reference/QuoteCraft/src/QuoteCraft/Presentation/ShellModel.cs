namespace QuoteCraft.Presentation;

public partial record ShellModel
{
    private readonly INavigator _navigator;
    private readonly Services.IOnboardingService _onboarding;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly ILogger<ShellModel> _logger;

    public ShellModel(
        INavigator navigator,
        Services.IOnboardingService onboarding,
        IBusinessProfileRepository profileRepo,
        ILogger<ShellModel> logger)
    {
        _navigator = navigator;
        _onboarding = onboarding;
        _profileRepo = profileRepo;
        _logger = logger;

        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        try
        {
            // Load currency setting for the converter
            var profile = await _profileRepo.GetAsync();
            Helpers.CurrencyFormatConverter.CurrencyCode = profile.CurrencyCode;

            // Check onboarding
            var isComplete = await _onboarding.IsOnboardingCompleteAsync();
            if (!isComplete)
            {
                await _navigator.NavigateRouteAsync(this, "Onboarding");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize shell");
        }
    }
}
