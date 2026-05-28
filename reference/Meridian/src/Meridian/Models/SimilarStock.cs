namespace Meridian.Models;

public record SimilarStock(
	string Ticker,
	string Name,
	decimal Price,
	decimal Pct);
