namespace Meridian.Models;

public record PortfolioSummary(
	decimal TotalValue,
	decimal TotalGainLoss,
	decimal TotalGainPct,
	bool IsPositive
);
