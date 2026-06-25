using System.Globalization;

namespace ClaudeCodeTracker.Presentation.Data;

/// <summary>
/// Centralized display formatting so every page renders numbers, currency and dates the
/// same way. Costs are always shown in USD (Claude pricing is USD) regardless of the UI
/// culture; token/count values and dates follow the current UI culture.
/// </summary>
public static class Fmt
{
    private static readonly CultureInfo Usd = CultureInfo.GetCultureInfo("en-US");

    public static string Money(decimal value) => value.ToString("C2", Usd);

    public static string Tokens(long value) => value.ToString("N0", CultureInfo.CurrentCulture);

    public static string Count(int value) => value.ToString("N0", CultureInfo.CurrentCulture);

    public static string Percent(double value) => value.ToString("0.#", CultureInfo.CurrentCulture);

    public static string DateTimeShort(DateTimeOffset value) =>
        value.ToLocalTime().ToString("MMM d, h:mm tt", CultureInfo.CurrentCulture);
}
