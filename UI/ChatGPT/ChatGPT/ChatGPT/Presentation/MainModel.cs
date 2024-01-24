using ChatGPT.Business;
using ChatGPT.Services;

namespace ChatGPT.Presentation;

public partial record MainModel
{
	private readonly IChatService _chatService;
	public MainModel(IChatService chatService)
	{
		_chatService = chatService;
	}

	public IState<string> Prompt => State.Value(this, () => string.Empty);

	public bool CanStream { get; } = !OperatingSystem.IsBrowser();

	public IState<bool> UseStream => State.Value(this, () => CanStream);

	public IListState<Message> Messages => ListState<Message>.Empty(this);

	public async ValueTask AskMessage(string prompt, CancellationToken ct)
	{
		if (await UseStream)
		{
			await AskAsStream(prompt, ct);
		}
		else
		{
			await Ask(prompt, ct);
		}
	}

	private async ValueTask Ask(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		//Add user prompt to ListState with Message record
		await Messages.AddAsync(new Message(prompt), ct);

		await Prompt.Set(string.Empty, ct);

		//Create loading message with Message record
		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		var requestContent = await CreateRequest();

		//Create ChatRequest record with messages list
		var request = new ChatRequest(requestContent);

		var response = await _chatService.AskAsync(request);

		//Update loading message with AI response as Message record
		//Finds the message with same id and upadtes the instance
		await Messages.UpdateAsync(message.Update(response), ct);
	}

	private async ValueTask AskAsStream(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		//Add user prompt to ListState with Message record
		await Messages.AddAsync(new Message(prompt), ct);

		await Prompt.Set(string.Empty, ct);

		//Create loading message with Message record
		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		var requestContent = await CreateRequest();

		//Create ChatRequest record with messages list
		var request = new ChatRequest(requestContent);

		await foreach (var response in _chatService.AskAsStream(request).WithCancellation(ct))
		{
			//Finds the message with same id and upadtes the instance
			await Messages.UpdateAsync(message.Update(response), ct);
		}
	}

	private async Task<ImmutableList<ChatEntry>> CreateRequest()
		=> (await Messages)
			.Where(msg => msg.Status is Status.Value)
			.Select(msg => new ChatEntry(msg.Content!, msg.Source is Source.User))
			.ToImmutableList();
}
