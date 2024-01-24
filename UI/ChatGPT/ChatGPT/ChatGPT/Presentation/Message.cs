using ChatGPT.Business;

namespace ChatGPT.Presentation;

public partial record Message(Guid Id, Source Source, Status Status, string? Content)
{
	public static Message CreateLoading()
		=> new Message(Guid.NewGuid(), Source.AI, Status.Loading, null);

	public Message(string userMessage)
		: this(Guid.NewGuid(), Source.User, Status.Value, userMessage)
	{
	}

	public Message Update(ChatResponse response)
	{
		return this with
		{
			Content = response.Message,
			Status = response.IsError ? Status.Error : Status.Value
		};
	}
}
