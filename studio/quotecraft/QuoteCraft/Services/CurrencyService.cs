namespace QuoteCraft.Services;

/// <summary>
/// The business's currency for amounts on screen (the profile's currency code). ShellModel reads it from the profile at
/// startup and Settings sets it on Save; the price fields of the line-item and catalog-item dialogs and
/// <see cref="CurrencyFormatConverter"/> read it. USD until the profile has been read.
/// </summary>
public sealed class CurrencyService
{
    private const string Default = "USD";
    private volatile string _code = Default;

    /// <summary>An ISO code: USD, CAD, EUR, GBP, AUD or MXN.</summary>
    public string Code
    {
        get => _code;
        set => _code = string.IsNullOrWhiteSpace(value) ? Default : value;
    }
}
