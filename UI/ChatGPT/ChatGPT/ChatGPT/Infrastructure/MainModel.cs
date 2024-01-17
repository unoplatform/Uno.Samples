using ChatGPT.Services;
using ChatGPT.Business;
using Microsoft.VisualBasic;

namespace ChatGPT.Infrastructure;
public partial record MainModel
{
	private readonly IChatService _chatService;
	public MainModel(IChatService chatService)
	{
		_chatService = chatService;
	}

	public IState<string> Prompt => State.Value(this, () => string.Empty);

	public IListState<Message> Messages => ListState<Message>.Empty(this);

	// For ref when using Toggle to use message stream completion or not
	public async ValueTask SendMessageSimple(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		await Messages.AddAsync(new Message(prompt), ct);
		await Prompt.Set(string.Empty, ct);

		var id = Guid.NewGuid();

		await Messages.AddAsync(new Message(id, Source.AI, Status.Loading, "..."), ct);

		var response = await _chatService.AskAsync(prompt);

		var comparer = KeyEqualityComparer.Find<Message>();

		await Messages.Update(messages => messages.AddOrUpdate(new Message(id, Source.AI, Status.Value, response.Message), comparer), ct);
	}

	public async ValueTask SendMessage(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		await Messages.AddAsync(new Message(prompt), ct);
		await Prompt.Set(string.Empty, ct);

		var comparer = KeyEqualityComparer.Find<Message>();
		await foreach (var response in _chatService.AskAsStream(prompt).WithCancellation(ct))
		{
			await Messages.Update(messages => messages.AddOrUpdate(new Message(response), comparer), ct);
		}
	}
}
