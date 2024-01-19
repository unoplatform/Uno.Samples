using ChatGPT.Business;
using ChatGPT.Extensions;
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

		await Messages.AddAsync(new Message(prompt), ct);
		await Prompt.Set(string.Empty, ct);

		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		var history = (await Messages)
			.Where(msg => msg.Status is Status.Value)
			.Select(msg => new ChatEntry(msg.Content!, msg.Source is Source.User))
			.ToImmutableList();
		var response = await _chatService.AskAsync(history);

		await Update(message, response, ct);
	}

	private async ValueTask AskAsStream(string prompt, CancellationToken ct)
	{
		if (prompt is null or { Length: 0 })
		{
			return;
		}

		await Messages.AddAsync(new Message(prompt), ct);
		await Prompt.Set(string.Empty, ct);

		var message = Message.CreateLoading();
		await Messages.AddAsync(message, ct);

		var history = (await Messages)
			.Where(msg => msg.Status is Status.Value)
			.Select(msg => new ChatEntry(msg.Content!, msg.Source is Source.User))
			.ToImmutableList();
		await foreach (var response in _chatService.AskAsStream(history).WithCancellation(ct))
		{
			await Update(message, response, ct);
		}
	}

	private async ValueTask Update (Message message, ChatResponse response, CancellationToken ct)
	{
		message = message with
		{
			Content = response.Message,
			Status = response.IsError ? Status.Error : Status.Value
		};

		await Messages.UpdateAsync(message, ct);
	}
}
