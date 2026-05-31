namespace QuoteCraft.Presentation;

public partial record SettingsModel
{
    private readonly INavigator _navigator;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly Services.IFeatureGateService _featureGate;
    private readonly Services.ISubscriptionService _subscriptionService;
    private readonly Services.IAuthService _authService;
    private readonly ILogger<SettingsModel> _logger;

    public SettingsModel(
        INavigator navigator,
        IBusinessProfileRepository profileRepo,
        ICatalogItemRepository catalogRepo,
        Services.IFeatureGateService featureGate,
        Services.ISubscriptionService subscriptionService,
        Services.IAuthService authService,
        ILogger<SettingsModel> logger)
    {
        _navigator = navigator;
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
        _featureGate = featureGate;
        _subscriptionService = subscriptionService;
        _authService = authService;
        _logger = logger;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await LoadProfile(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize settings");
        }
    }

    public IFeed<BusinessProfileEntity> Profile => Feed.Async(async ct =>
        await _profileRepo.GetAsync());

    public IFeed<int> CatalogItemCount => Feed.Async(async ct =>
        await _catalogRepo.GetItemCountAsync());

    // Subscription info feeds
    public IFeed<string> CurrentPlanName => Feed.Async(async ct =>
    {
        var tier = _featureGate.CurrentTier;
        var maxQuotes = _featureGate.MaxQuotesPerMonth;
        return tier switch
        {
            Services.SubscriptionTier.Free => $"Free ({maxQuotes} quotes/mo)",
            Services.SubscriptionTier.Pro => "Pro (Unlimited)",
            Services.SubscriptionTier.Business => "Business (Unlimited)",
            _ => "Free"
        };
    });

    public IFeed<string> QuotaUsage => Feed.Async(async ct =>
    {
        var used = await _featureGate.GetQuotesUsedThisMonthAsync();
        var max = _featureGate.MaxQuotesPerMonth;
        return max == int.MaxValue ? $"{used} this month" : $"{used} of {max}";
    });

    public IState<string> BusinessName => State<string>.Value(this, () => string.Empty);
    public IState<string> Phone => State<string>.Value(this, () => string.Empty);
    public IState<string> Email => State<string>.Value(this, () => string.Empty);
    public IState<string> Address => State<string>.Value(this, () => string.Empty);
    public IState<string> Website => State<string>.Value(this, () => string.Empty);
    public IState<string> BusinessNumber => State<string>.Value(this, () => string.Empty);
    public IState<string> DefaultTaxRate => State<string>.Value(this, () => "13.0");
    public IState<string> DefaultMarkup => State<string>.Value(this, () => "0");
    public IState<string> CurrencyCode => State<string>.Value(this, () => "USD");
    public IState<string> QuoteValidDays => State<string>.Value(this, () => "14");
    public IState<string> QuoteNumberPrefix => State<string>.Value(this, () => "QC-");
    public IState<string> CustomFooter => State<string>.Value(this, () => string.Empty);
    public IState<string> LogoPath => State<string>.Value(this, () => string.Empty);

    public async ValueTask LoadProfile(CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();
        await BusinessName.UpdateAsync(_ => profile.BusinessName ?? string.Empty, ct);
        await Phone.UpdateAsync(_ => profile.Phone ?? string.Empty, ct);
        await Email.UpdateAsync(_ => profile.Email ?? string.Empty, ct);
        await Address.UpdateAsync(_ => profile.Address ?? string.Empty, ct);
        await Website.UpdateAsync(_ => profile.Website ?? string.Empty, ct);
        await BusinessNumber.UpdateAsync(_ => profile.BusinessNumber ?? string.Empty, ct);
        await DefaultTaxRate.UpdateAsync(_ => profile.DefaultTaxRate.ToString("F1"), ct);
        await DefaultMarkup.UpdateAsync(_ => profile.DefaultMarkup.ToString("F1"), ct);
        await CurrencyCode.UpdateAsync(_ => profile.CurrencyCode, ct);
        await QuoteValidDays.UpdateAsync(_ => profile.QuoteValidDays.ToString(), ct);
        await QuoteNumberPrefix.UpdateAsync(_ => profile.QuoteNumberPrefix, ct);
        await CustomFooter.UpdateAsync(_ => profile.CustomFooter ?? string.Empty, ct);
        await LogoPath.UpdateAsync(_ => profile.LogoPath ?? string.Empty, ct);
    }

    public async ValueTask Save(CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();

        profile.BusinessName = await BusinessName;
        profile.Phone = await Phone;
        profile.Email = await Email;
        profile.Address = await Address;
        profile.Website = await Website;
        profile.BusinessNumber = await BusinessNumber;
        profile.CustomFooter = await CustomFooter;
        profile.LogoPath = await LogoPath;
        profile.QuoteNumberPrefix = await QuoteNumberPrefix ?? "QC-";
        profile.CurrencyCode = await CurrencyCode ?? "USD";
        profile.UpdatedAt = DateTimeOffset.UtcNow;

        var taxStr = await DefaultTaxRate ?? "0";
        if (decimal.TryParse(taxStr, out var tax))
            profile.DefaultTaxRate = tax;

        var markupStr = await DefaultMarkup ?? "0";
        if (decimal.TryParse(markupStr, out var markup))
            profile.DefaultMarkup = markup;

        var daysStr = await QuoteValidDays ?? "14";
        if (int.TryParse(daysStr, out var days))
            profile.QuoteValidDays = days;

        await _profileRepo.SaveAsync(profile);

        // Update the currency converter for the whole app
        Helpers.CurrencyFormatConverter.CurrencyCode = profile.CurrencyCode;
    }

    public async ValueTask SetLogoPath(string path, CancellationToken ct)
    {
        await LogoPath.UpdateAsync(_ => path, ct);
    }

    public async ValueTask RemoveLogo(CancellationToken ct)
    {
        var currentPath = await LogoPath;
        if (!string.IsNullOrEmpty(currentPath) && File.Exists(currentPath))
            File.Delete(currentPath);
        await LogoPath.UpdateAsync(_ => string.Empty, ct);
    }

    public async ValueTask UpgradeToPro(CancellationToken ct)
    {
        try
        {
            var url = await _subscriptionService.GetCheckoutUrlAsync("pro", "monthly");
            if (!string.IsNullOrEmpty(url))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open checkout");
        }
    }

    public async ValueTask UpgradeToProAnnual(CancellationToken ct)
    {
        try
        {
            var url = await _subscriptionService.GetCheckoutUrlAsync("pro", "annual");
            if (!string.IsNullOrEmpty(url))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open checkout");
        }
    }

    public async ValueTask ManageSubscription(CancellationToken ct)
    {
        try
        {
            var url = await _subscriptionService.GetCustomerPortalUrlAsync();
            if (!string.IsNullOrEmpty(url))
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open customer portal");
        }
    }

    public async ValueTask SignOut(CancellationToken ct)
    {
        await _authService.SignOutAsync();
        await _navigator.NavigateRouteAsync(this, "Auth", qualifier: Qualifiers.ClearBackStack);
    }

    public bool IsAuthenticated => _authService.CurrentState.IsAuthenticated;
}
