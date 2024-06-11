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

		//Add the user prompt message to the conversation
		await Messages.AddAsync(new Message(prompt), ct);

		await Prompt.Set(string.Empty, ct);

		//Add a placeholder loading message while awaiting the AI response
		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		//Create the request with the conversation history
		var request = await CreateRequest();

		var response = await _chatService.AskAsync(request);

		//Update loading placeholder message with AI response
		//Finds the message with same id and updates the instance
		await Messages.UpdateAsync(message.With(response), ct);
	}

	private async ValueTask AskAsStream(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		//Add the User prompt message to the conversation
		await Messages.AddAsync(new Message(prompt), ct);

		await Prompt.Set(string.Empty, ct);

		//Add a Plaecholder loading message while awaiting the AI response
		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		//Create the request with the conversation history
		var request = await CreateRequest();

		await foreach (var response in _chatService.AskAsStream(request).WithCancellation(ct))
		{
			//Finds the message with same id and updates the instance with new part of response
			await Messages.UpdateAsync(message.With(response), ct);
		}
	}

	private async Task<ChatRequest> CreateRequest()
	{
		var requestContent = (await Messages)
								.Where(msg => msg.Status is Status.Value)
								.Select(msg => new ChatEntry(msg.Content!, msg.Source is Source.User))
								.ToImmutableList();

		return new ChatRequest(requestContent);
	}
}