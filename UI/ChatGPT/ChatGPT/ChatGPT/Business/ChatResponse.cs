namespace ChatGPT.Business;
public partial record ChatResponse(Status Status, string? Message = null)
{
	public Guid Id { get; init; } = Guid.NewGuid();
}
