namespace QuoteCraft.Helpers;

/// <summary>
/// A quote's valid-until date between the editor's date picker and the database (PRD F1.8). Stored dates are UTC: the
/// expiry query compares them as text, and the pages, the Send email and the PDF show their UTC calendar day. A picked
/// day is stored as the end of that day, so the quote is valid through it.
/// </summary>
public static class ValidUntilDate
{
    /// <summary>
    /// The stored date's calendar day for the picker, at noon local time: no time zone moves noon to another day.
    /// </summary>
    public static DateTimeOffset? ToPicker(DateTimeOffset? stored) =>
        stored is { } date ? new DateTimeOffset(date.UtcDateTime.Date.AddHours(12)) : null;

    /// <summary>The picked day as stored: its last second, in UTC.</summary>
    public static DateTimeOffset FromPicker(DateTimeOffset picked) =>
        new(picked.Year, picked.Month, picked.Day, 23, 59, 59, TimeSpan.Zero);

    /// <summary>True when the two dates fall on the same calendar day (both empty counts as the same).</summary>
    public static bool SameDay(DateTimeOffset? a, DateTimeOffset? b) => a?.Date == b?.Date;

    /// <summary>
    /// What is wrong with a picked day, or null: a day before today (in UTC, as the date is stored) would leave the quote
    /// expired at once.
    /// </summary>
    public static string? Check(DateTimeOffset picked, DateTimeOffset now) =>
        picked.Date < now.UtcDateTime.Date ? "The valid-until date cannot be in the past." : null;
}
