namespace Commerce.Data.Models;

public record ReviewData
{
	public string? Photo { get; set; }
	public string? Name { get; set; }
	public string? Message { get; set; }
}
