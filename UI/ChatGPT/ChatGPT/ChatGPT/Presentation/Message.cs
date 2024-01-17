
namespace ChatGPT.Business.Models;
public partial record Message(Guid Id, Source Source, Status Status, string Content)
{
	public Message(string userMessage)
		: this(Guid.NewGuid(), Source.User, Status.Value, userMessage)
	{
	}

	public Message(ChatResponse response)
		: this(response.Id, Source.AI, response.Status, response.Message ?? "...")
	{
	}
}
