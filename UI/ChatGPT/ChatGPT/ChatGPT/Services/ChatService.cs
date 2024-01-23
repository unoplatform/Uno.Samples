using ChatGPT.Business;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChatGPT.Services;
public class ChatService(IChatCompletionService client) : IChatService
{
	private readonly IChatCompletionService _client = client;

	private const string systemPrompt = "You are Uno ChatGPT Sample, a helpful assistant helping users to learn more about how to develop using Uno Platform.";

	public async ValueTask<ChatResponse> AskAsync(ChatRequest chatRequest, CancellationToken ct = default)
	{
		try
		{
			var request = ToCompletionRequest(chatRequest);
			var result = await _client.CreateCompletion(request, cancellationToken: ct);

			if (result.Successful)
			{
				var response = result.Choices.Select(choice => choice.Message.Content);

				return new ChatResponse(string.Join("", response));
			}
			else
			{
				return new ChatResponse(result.Error?.Message, IsError: true);
			}
		}
		catch (Exception)
		{
			return new ChatResponse(IsError: true);
		}
	}

	public async IAsyncEnumerable<ChatResponse> AskAsStream(ChatRequest chatRequest, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var request = ToCompletionRequest(chatRequest);
		var response = new ChatResponse();
		var content = new StringBuilder();

		IAsyncEnumerator<ChatCompletionCreateResponse>? responseStream = default;
		while (!response.IsError)
		{
			try
			{
				responseStream ??= _client.CreateCompletionAsStream(request).GetAsyncEnumerator(ct);
				if (await responseStream.MoveNextAsync())
				{
					if (responseStream.Current.Successful)
					{
						foreach (var choice in responseStream.Current.Choices)
						{
							content.Append(choice.Message.Content);
						}
						response = response with { Message = content.ToString() };
					}
					else
					{
						response = response with { Message = responseStream.Current.Error?.Message, IsError = true };
					}
				}
				else
				{
					yield break;
				}
			}
			catch (Exception)
			{
				response = response with { IsError = true };
			}

			yield return response;
		}
	}

	private ChatCompletionCreateRequest ToCompletionRequest(ChatRequest request)
	{
		var history = request.History;
		var requestMessages = new List<ChatMessage>(history.Count + 1)
		{
			ChatMessage.FromSystem(systemPrompt)
		};
		requestMessages.AddRange(history.Select(entry => entry.IsUser 
			? ChatMessage.FromUser(entry.Message) 
			: ChatMessage.FromAssistant(entry.Message)));

		return new ChatCompletionCreateRequest()
		{
			Messages = requestMessages,
			Model = Models.Gpt_3_5_Turbo
		};
	}
}
