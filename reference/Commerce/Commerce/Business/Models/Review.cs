namespace Commerce.Business.Models;
using Commerce.Data.Models;

public class Review
{
	public Review(ReviewData data)
	{
		Photo = data.Photo;
		Name = data.Name;
		Message = data.Message;
	}

	public string? Photo { get; init; }
	public string? Name { get; init; }
	public string? Message { get; init; }
}
