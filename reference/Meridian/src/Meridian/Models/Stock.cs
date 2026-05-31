namespace Meridian.Models;

public record Stock(
    string Ticker,
    string Name,
    decimal Price,
    decimal Change,
    decimal Pct,
    decimal High,
    decimal Low,
    decimal Open,
    string Volume
)
{
    /// <summary>Position of current price within day range (0.0–1.0), clamped for visibility.</summary>
    public double DayRangeRatio
    {
        get
        {
            var range = High - Low;
            if (range <= 0) return 0.5;
            return Math.Clamp((double)((Price - Low) / range), 0.05, 0.95);
        }
    }
};
