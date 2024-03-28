namespace ToDo.Business;

public class LocalizationService : ILocalizationService
{
    public CultureInfo[] SupportedCultures { get; set; }

    public CultureInfo CurrentCulture { get; set; }

    public Task SetCurrentCultureAsync(CultureInfo newCulture)
    {
        CurrentCulture = newCulture;
        return Task.CompletedTask;
    }

}
