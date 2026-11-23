namespace QuoteCraft.Presentation;

public partial record OnboardingModel
{
    private readonly INavigator _navigator;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly IOnboardingService _onboardingService;
    private readonly QuoteCreationService _creation;

    public OnboardingModel(
        INavigator navigator,
        IBusinessProfileRepository profileRepo,
        IOnboardingService onboardingService,
        QuoteCreationService creation)
    {
        _navigator = navigator;
        _profileRepo = profileRepo;
        _onboardingService = onboardingService;
        _creation = creation;
    }

    public IState<int> CurrentStep => State<int>.Value(this, () => 1);

    // Step 2: Business Info
    public IState<string> BusinessName => State<string>.Value(this, () => string.Empty);
    public IState<string> Phone => State<string>.Value(this, () => string.Empty);
    public IState<string> Email => State<string>.Value(this, () => string.Empty);
    public IState<string> Address => State<string>.Value(this, () => string.Empty);

    // Step 3: Quote Settings
    public IState<string> TaxRate => State<string>.Value(this, () => Money.FormatRate(8.5m));
    public IState<string> ValidDays => State<string>.Value(this, () => "14");
    public IState<string> QuotePrefix => State<string>.Value(this, () => "QC-");

    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    public async ValueTask NextStep(CancellationToken ct)
    {
        var step = await CurrentStep;

        if (step == 1)
        {
            await CurrentStep.UpdateAsync(_ => 2, ct);
        }
        else if (step == 2)
        {
            var email = (await Email)?.Trim();
            if (!EmailAddress.IsValidOrEmpty(email))
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

    public ValueTask Skip(CancellationToken ct) => CompleteOnboarding(createQuote: false, ct);

    public async ValueTask Complete(CancellationToken ct)
    {
        if (!Money.TryParseRate(await TaxRate, out var tax) || tax > 100)
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

    /// <summary>
    /// Saves what was entered and leaves onboarding: for the editor on a new Draft (Create First Quote), or for the quotes
    /// list (Skip). A failure stays on the step and says so.
    /// </summary>
    private ValueTask CompleteOnboarding(bool createQuote, CancellationToken ct) => ValidationError.ReportFailureAsync(
        "Your setup could not be finished.", async () =>
    {
        var profile = await _profileRepo.GetAsync();

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

        if (Money.TryParseRate(await TaxRate, out var taxRate))
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
            // A blank Draft with the defaults just saved, opened in the editor.
            var quote = await _creation.CreateDraftAsync("New Quote");

            // Onboarding is the first route on a first run: open the editor inside Main, as the other pages do.
            await _navigator.NavigateRouteAsync(this, "Main/QuoteEditor", qualifier: Qualifiers.ClearBackStack, data: quote);
        }
        else
        {
            // Onboarding is the first route on a first run, so there is nothing to go back to.
            await _navigator.NavigateRouteAsync(this, "Main", qualifier: Qualifiers.ClearBackStack);
        }
    }, ct);
}
