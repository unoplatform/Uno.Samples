
namespace ChatGPT.Business.Models;
public partial record Message
{
    public Guid Id { get; init; }
    public Source Source { get; init; }
    public Status Status { get; init; }
    public string Content { get; init; }
}
