namespace ToDo.Business;

public class LocalizationService : ILocalizationService
{
    //TODO: Adjust as needed
    public CultureInfo[] SupportedCultures { get; private set; }

    public CultureInfo CurrentCulture { get; private set; }

    public Task SetCurrentCultureAsync(CultureInfo newCulture)
    {
        if (SupportedCultures.Contains(newCulture))
        {
            CurrentCulture = newCulture;
            return Task.CompletedTask;
        }
        else
        {
            throw new ArgumentException("The specified culture is not supported.", nameof(newCulture));
        }
    }

}
