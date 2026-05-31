namespace Meridian.Models;

public record KeyStat(
	string Label,
	string Value,
	bool IsColored = false,
	bool IsPositive = false);
