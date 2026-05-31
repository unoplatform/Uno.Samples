namespace Meridian.Models;

public record StockProfile(
	string Description,
	string Sector,
	string Industry,
	int Founded,
	string Headquarters,
	string CEO,
	string Employees,
	string WebsiteUrl);
