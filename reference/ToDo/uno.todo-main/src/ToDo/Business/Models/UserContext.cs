namespace ToDo.Business.Models;

public partial record UserContext
{
	public string? Name { get; init; }

	public string? Email { get; init; }

	public string? AccessToken { get; init; }
}
