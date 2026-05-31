namespace Meridian.Models;

public record Holding(
    string Ticker,
    int Shares,
    decimal AvgCost,
    decimal CurrentPrice
)
{
    public decimal MarketValue => Shares * CurrentPrice;
    public decimal GainLoss => (CurrentPrice - AvgCost) * Shares;
    public decimal GainPct => AvgCost != 0 ? (CurrentPrice - AvgCost) / AvgCost * 100 : 0;
    public bool IsPositive => GainPct >= 0;
}
