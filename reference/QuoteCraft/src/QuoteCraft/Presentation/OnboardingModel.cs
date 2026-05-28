namespace QuoteCraft.Presentation;

public partial record OnboardingModel
{
    private readonly INavigator _navigator;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly IOnboardingService _onboardingService;
    private readonly IQuoteRepository _quoteRepo;
    private readonly QuoteNumberGenerator _quoteNumberGen;

    public OnboardingModel(
        INavigator navigator,
        IBusinessProfileRepository profileRepo,
        IOnboardingService onboardingService,
        IQuoteRepository quoteRepo,
        QuoteNumberGenerator quoteNumberGen)
    {
        _navigator = navigator;
        _profileRepo = profileRepo;
        _onboardingService = onboardingService;
        _quoteRepo = quoteRepo;
        _quoteNumberGen = quoteNumberGen;
    }

    public IState<int> CurrentStep => State<int>.Value(this, () => 1);

    // Step 2: Business Info
    public IState<string> BusinessName => State<string>.Value(this, () => string.Empty);
    public IState<string> Phone => State<string>.Value(this, () => string.Empty);
    public IState<string> Email => State<string>.Value(this, () => string.Empty);
    public IState<string> Address => State<string>.Value(this, () => string.Empty);

    // Step 3: Quote Settings
    public IState<string> TaxRate => State<string>.Value(this, () => "8.5");
    public IState<string> ValidDays => State<string>.Value(this, () => "14");
    public IState<string> QuotePrefix => State<string>.Value(this, () => "QC-");

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask NextStep(CancellationToken ct)
    {
        var step = await CurrentStep;

        if (step == 1)
        {
            // Welcome -> Business Info (no validation needed)
            await CurrentStep.UpdateAsync(_ => 2, ct);
        }
        else if (step == 2)
        {
            // Validate business info before advancing
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
            await CurrentStep.UpdateAsync(_ => 3, ct);
        }
    }

    public async ValueTask PreviousStep(CancellationToken ct)
    {
        var step = await CurrentStep;
        if (step > 1)
        {
            await ValidationError.UpdateAsync(_ => string.Empty, ct);
            await CurrentStep.UpdateAsync(s => s - 1, ct);
        }
    }

    public async ValueTask Skip(CancellationToken ct)
    {
        await CompleteOnboarding(createQuote: false, ct);
    }

    public async ValueTask Complete(CancellationToken ct)
    {
        // Validate quote settings before completing
        var taxStr = await TaxRate ?? "0";
        if (!decimal.TryParse(taxStr, out var tax) || tax < 0 || tax > 100)
        {
            await ValidationError.UpdateAsync(_ => "Tax rate must be between 0 and 100.", ct);
            return;
        }

        var daysStr = await ValidDays ?? "14";
        if (!int.TryParse(daysStr, out var days) || days < 1)
        {
            await ValidationError.UpdateAsync(_ => "Valid days must be at least 1.", ct);
            return;
        }

        await ValidationError.UpdateAsync(_ => string.Empty, ct);
        await CompleteOnboarding(createQuote: true, ct);
    }

    private async Task CompleteOnboarding(bool createQuote, CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();

        // Save business info if provided
        var name = (await BusinessName)?.Trim();
        if (!string.IsNullOrEmpty(name))
            profile.BusinessName = name;

        var phone = (await Phone)?.Trim();
        if (!string.IsNullOrEmpty(phone))
            profile.Phone = phone;

        var email = (await Email)?.Trim();
        if (!string.IsNullOrEmpty(email))
            profile.Email = email;

        var address = (await Address)?.Trim();
        if (!string.IsNullOrEmpty(address))
            profile.Address = address;

        // Save quote settings
        var taxStr = await TaxRate ?? "0";
        if (decimal.TryParse(taxStr, out var taxRate))
            profile.DefaultTaxRate = taxRate;

        var daysStr = await ValidDays ?? "14";
        if (int.TryParse(daysStr, out var validDays) && validDays > 0)
            profile.QuoteValidDays = validDays;

        var prefix = (await QuotePrefix)?.Trim();
        if (!string.IsNullOrEmpty(prefix))
            profile.QuoteNumberPrefix = prefix;

        await _profileRepo.SaveAsync(profile);
        await _onboardingService.MarkCompleteAsync();

        if (createQuote)
        {
            // Create a blank quote and navigate directly to the editor
            var quoteNumber = await _quoteNumberGen.GenerateAsync();
            var quote = new QuoteEntity
            {
                Title = "New Quote",
                QuoteNumber = quoteNumber,
                Status = QuoteStatus.Draft,
                TaxRate = profile.DefaultTaxRate,
                ValidUntil = DateTimeOffset.UtcNow.AddDays(profile.QuoteValidDays),
            };
            await _quoteRepo.SaveAsync(quote);
            await _navigator.NavigateRouteAsync(this, "QuoteEditor", data: quote);
        }
        else
        {
            await _navigator.NavigateBackAsync(this);
        }
    }
}
