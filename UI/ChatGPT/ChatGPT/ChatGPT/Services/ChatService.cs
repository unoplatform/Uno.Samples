using ChatGPT.Business;
using System.Runtime.CompilerServices;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System.Text;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.Interfaces;

namespace ChatGPT.Services;
public class ChatService(IChatCompletionService client) : IChatService
{
	private readonly IChatCompletionService _client = client;

	public async ValueTask<ChatResponse> AskAsync(IImmutableList<ChatEntry> history, CancellationToken ct = default)
	{
		try
		{
			var request = CreateRequest(history);
			var result = await _client.CreateCompletion(request, cancellationToken: ct);

			if (result.Successful)
			{
				var response = result.Choices.Select(choice => choice.Message.Content);

				var responseContent = string.Join("", response);

				return new ChatResponse(string.Join("", responseContent));
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

	public async IAsyncEnumerable<ChatResponse> AskAsStream(IImmutableList<ChatEntry> history, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var request = CreateRequest(history);
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

	private ChatCompletionCreateRequest CreateRequest(IImmutableList<ChatEntry> history)
	{
		var requestMessages = new List<ChatMessage>(history.Count + 1)
		{
			ChatMessage.FromSystem("You are Uno ChatGPT Sample, a helpful assistant helping users to learn more about how to develop using Uno Platform.")
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
