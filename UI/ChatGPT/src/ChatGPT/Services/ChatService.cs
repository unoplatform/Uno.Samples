using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatGPT.Services;
public class ChatService : IChatService
{
	private const string systemPrompt = "You are Uno ChatGPT Sample, a helpful assistant helping users to learn more about how to develop using Uno Platform.";

	private readonly ChatClient _client;

	public ChatService(ChatClient client)
	{
		_client = client;
	}

	public async ValueTask<ChatResponse> AskAsync(ChatRequest chatRequest, CancellationToken ct = default)
	{
		try
		{
			var request = ToCompletionRequest(chatRequest);
			ChatCompletion result = await _client.CompleteChatAsync(request);

			return result.FinishReason switch
			{
				ChatFinishReason.Stop => new ChatResponse(result.ToString()),
				ChatFinishReason.Length => new ChatResponse("Incomplete model output due to MaxTokens parameter or token limit exceeded.", IsError: true),
				ChatFinishReason.ContentFilter => new ChatResponse("Omitted content due to a content filter flag.", IsError: true),
				_ => new ChatResponse(result.FinishReason.ToString())
			};
		}
		catch (Exception ex)
		{
			return new ChatResponse($"Something went wrong: {ex.Message}", IsError: true);
		}
	}

	public async IAsyncEnumerable<ChatResponse> AskAsStream(ChatRequest chatRequest, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var request = ToCompletionRequest(chatRequest);
		var response = new ChatResponse();
		var content = new StringBuilder();

		IAsyncEnumerator<StreamingChatCompletionUpdate>? responseStream = default;

		while (!response.IsError)
		{
			try
			{
				responseStream ??= _client.CompleteChatStreamingAsync(request).GetAsyncEnumerator(ct);

				if (await responseStream.MoveNextAsync())
				{
					foreach (var updatePart in responseStream.Current.ContentUpdate)
					{
						content.Append(updatePart.Text);
					}

					response = response with { Message = content.ToString() };
				}
				else
				{
					yield break;
				}
			}
			catch (Exception ex)
			{
				response = response with { Message = $"Something went wrong: {ex.Message}", IsError = true };
			}

			yield return response;
		}
	}

	private ChatMessage[] ToCompletionRequest(ChatRequest request)
		=> request.History
			.Select(ConvertMessage)
			.Prepend(ChatMessage.CreateSystemMessage(systemPrompt))
			.ToArray();
	

	private ChatMessage ConvertMessage(ChatEntry entry)
		=> entry.IsUser
			? ChatMessage.CreateUserMessage(entry.Message)
			: ChatMessage.CreateAssistantMessage(entry.Message);
}