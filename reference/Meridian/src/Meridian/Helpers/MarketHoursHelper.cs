namespace Meridian.Helpers;

internal static class MarketHoursHelper
{
    private static readonly TimeZoneInfo EasternTime = GetEasternTimeZone();

    internal static bool IsMarketOpen()
    {
        var et = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, EasternTime);
        if (et.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return false;
        var time = et.TimeOfDay;
        return time >= TimeSpan.FromHours(9.5) && time < TimeSpan.FromHours(16);
    }

    private static TimeZoneInfo GetEasternTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("America/New_York"); }
    }
}
