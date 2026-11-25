namespace QuoteCraft.Presentation;

/// <summary>
/// The Settings page: the business profile and quote defaults with the preview beside them, the logo, the catalog
/// count, and Save. Its account and billing (<see cref="AccountSettingsModel"/>) and its theme row
/// (<see cref="ThemeSettingsModel"/>) are models of their own, bound through <c>Account</c> and <c>Appearance</c>
/// (SettingsViewModel.cs).
/// </summary>
public partial record SettingsModel
{
    private readonly INavigator _navigator;
    private readonly IBusinessProfileRepository _profileRepo;
    private readonly ICatalogItemRepository _catalogRepo;
    private readonly CurrencyService _currency;
    private readonly LogoStore _logos;
    private readonly IDispatcher _dispatcher;

    public SettingsModel(
        INavigator navigator,
        IBusinessProfileRepository profileRepo,
        ICatalogItemRepository catalogRepo,
        CurrencyService currency,
        LogoStore logos,
        IDispatcher dispatcher,
        AccountSettingsModel account,
        ThemeSettingsModel appearance)
    {
        _navigator = navigator;
        _profileRepo = profileRepo;
        _catalogRepo = catalogRepo;
        _currency = currency;
        _logos = logos;
        _dispatcher = dispatcher;
        AccountSettings = account;
        ThemeSettings = appearance;
        _ = InitializeAsync();
    }

    /// <summary>The account and billing sections and the upgrade banner.</summary>
    internal AccountSettingsModel AccountSettings { get; }

    /// <summary>The Appearance row.</summary>
    internal ThemeSettingsModel ThemeSettings { get; }

    private async Task InitializeAsync()
    {
        try
        {
            await LoadProfileAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Failed to initialize settings");
            await LoadError.UpdateAsync(_ => "Your settings could not be loaded.", CancellationToken.None);
        }
    }

    /// <summary>Set when the profile could not be read; the page shows it with Retry.</summary>
    public IState<string> LoadError => State<string>.Value(this, () => string.Empty);

    /// <summary>What a Save found wrong, or empty.</summary>
    public IState<string> ValidationError => State<string>.Value(this, () => string.Empty);

    /// <summary>"Settings saved." after a successful Save, until the next one.</summary>
    public IState<string> SavedMessage => State<string>.Value(this, () => string.Empty);

    public IState<bool> IsSaving => State<bool>.Value(this, () => false);

    /// <summary>Retry after a failed read: the profile again.</summary>
    public async ValueTask Reload(CancellationToken ct)
    {
        await LoadError.UpdateAsync(_ => string.Empty, ct);
        await InitializeAsync();
    }

    public async ValueTask OpenCatalog(CancellationToken ct)
    {
        await _navigator.NavigateRouteAsync(this, "Catalog", cancellation: ct);
    }

    /// <summary>
    /// The quote preview beside the form (from 1400 px): sample items priced with the markup as the catalog adds them,
    /// tax at the default rate, the dates from today and the validity, and amounts in the currency, all as typed.
    /// </summary>
    public IFeed<SettingsPreview> Preview => Feed.Combine(DefaultTaxRate, DefaultMarkup, CurrencyCode, QuoteValidDays, QuoteNumberPrefix)
        .Select(inputs =>
        {
            var (tax, markup, currency, validDays, prefix) = inputs;
            return SettingsPreview.From(prefix, tax, markup, validDays, currency, DateTimeOffset.Now);
        });

    public IFeed<int> CatalogItemCount => Feed.Async(async ct =>
        await _catalogRepo.GetItemCountAsync());

    public IState<string> BusinessName => State<string>.Value(this, () => string.Empty);
    public IState<string> Phone => State<string>.Value(this, () => string.Empty);
    public IState<string> Email => State<string>.Value(this, () => string.Empty);
    public IState<string> Address => State<string>.Value(this, () => string.Empty);
    public IState<string> Website => State<string>.Value(this, () => string.Empty);
    public IState<string> BusinessNumber => State<string>.Value(this, () => string.Empty);
    public IState<string> DefaultTaxRate => State<string>.Value(this, () => Money.FormatRate(13m));
    public IState<string> DefaultMarkup => State<string>.Value(this, () => "0");
    public IState<string> CurrencyCode => State<string>.Value(this, () => "USD");
    public IState<string> QuoteValidDays => State<string>.Value(this, () => "14");
    public IState<string> QuoteNumberPrefix => State<string>.Value(this, () => "QC-");
    public IState<string> CustomFooter => State<string>.Value(this, () => string.Empty);
    public IState<string> LogoPath => State<string>.Value(this, () => string.Empty);

    /// <summary>The logo's file name, beside its thumbnail.</summary>
    public IFeed<string> LogoFileName => LogoPath.Select(path => string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileName(path));

    /// <summary>What Upload or Remove could not do; shown under the logo row.</summary>
    public IState<string> LogoError => State<string>.Value(this, () => string.Empty);

    private async ValueTask LoadProfileAsync(CancellationToken ct)
    {
        var profile = await _profileRepo.GetAsync();
        await BusinessName.UpdateAsync(_ => profile.BusinessName ?? string.Empty, ct);
        await Phone.UpdateAsync(_ => profile.Phone ?? string.Empty, ct);
        await Email.UpdateAsync(_ => profile.Email ?? string.Empty, ct);
        await Address.UpdateAsync(_ => profile.Address ?? string.Empty, ct);
        await Website.UpdateAsync(_ => profile.Website ?? string.Empty, ct);
        await BusinessNumber.UpdateAsync(_ => profile.BusinessNumber ?? string.Empty, ct);
        await DefaultTaxRate.UpdateAsync(_ => Money.FormatRate(profile.DefaultTaxRate), ct);
        await DefaultMarkup.UpdateAsync(_ => Money.FormatRate(profile.DefaultMarkup), ct);
        await CurrencyCode.UpdateAsync(_ => profile.CurrencyCode, ct);
        await QuoteValidDays.UpdateAsync(_ => profile.QuoteValidDays.ToString(), ct);
        await QuoteNumberPrefix.UpdateAsync(_ => profile.QuoteNumberPrefix, ct);
        await CustomFooter.UpdateAsync(_ => profile.CustomFooter ?? string.Empty, ct);
        await LogoPath.UpdateAsync(_ => profile.LogoPath ?? string.Empty, ct);
    }

    public async ValueTask Save(CancellationToken ct)
    {
        await SavedMessage.UpdateAsync(_ => string.Empty, ct);
        var error = SettingsValidation.Validate(await DefaultTaxRate, await DefaultMarkup, await QuoteValidDays);
        await ValidationError.UpdateAsync(_ => error ?? string.Empty, ct);
        if (error is not null)
        {
            return;
        }

        await IsSaving.UpdateAsync(_ => true, ct);
        try
        {
            await SaveProfileAsync();
            await SavedMessage.UpdateAsync(_ => "Settings saved.", ct);
        }
        catch (Exception ex)
        {
            ErrorReporter.Log(ex, "Failed to save settings");
            await ValidationError.UpdateAsync(_ => "Your settings could not be saved. Try again.", ct);
        }
        finally
        {
            await IsSaving.UpdateAsync(_ => false, ct);
        }
    }

    private async Task SaveProfileAsync()
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

        if (Money.TryParseRate(await DefaultTaxRate, out var tax))
            profile.DefaultTaxRate = tax;

        var markupText = await DefaultMarkup;
        profile.DefaultMarkup = string.IsNullOrWhiteSpace(markupText) ? 0m
            : Money.TryParseRate(markupText, out var markup) ? markup : profile.DefaultMarkup;

        var daysStr = await QuoteValidDays ?? "14";
        if (int.TryParse(daysStr, out var days))
            profile.QuoteValidDays = days;

        await _profileRepo.SaveAsync(profile);

        // Amounts everywhere follow the saved currency.
        _currency.Code = profile.CurrencyCode;
    }

    /// <summary>
    /// Upload: a picked image becomes the logo (<see cref="LogoStore"/>); Save keeps it. Read through the StorageFile:
    /// a picked file's Path is not readable on every head (WebAssembly, Android, iOS).
    /// </summary>
    public ValueTask UploadLogo(CancellationToken ct) => LogoError.ReportFailureAsync("The logo could not be added.", async () =>
    {
        if (await ImagePicker.PickAsync(_dispatcher, ct) is not { } file)
        {
            return;
        }

        string path;
        await using (var image = await file.OpenStreamForReadAsync())
        {
            path = await _logos.SaveAsync(image, Path.GetExtension(file.Name));
        }
        await LogoPath.UpdateAsync(_ => path, ct);
    }, ct);

    /// <summary>Remove: the logo's copy goes, and Save stores the profile without one.</summary>
    public ValueTask RemoveLogo(CancellationToken ct) => LogoError.ReportFailureAsync("The logo could not be removed.", async () =>
    {
        _logos.Delete(await LogoPath);
        await LogoPath.UpdateAsync(_ => string.Empty, ct);
    }, ct);
}
