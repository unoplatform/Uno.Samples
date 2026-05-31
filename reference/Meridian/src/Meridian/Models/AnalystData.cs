namespace Meridian.Models;

public record AnalystData(
	int BuyCount,
	int HoldCount,
	int SellCount,
	decimal PriceTargetLow,
	decimal PriceTargetAvg,
	decimal PriceTargetHigh);
