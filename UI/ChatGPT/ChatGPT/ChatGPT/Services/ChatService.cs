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

	private List<ChatMessage> _messages =
	[
		ChatMessage.FromSystem("You are Uno ChatGPT Sample, a helpful assistant helping users to learn more about how to develop using Uno Platform.")
	];

	public async ValueTask<ChatResponse> AskAsync(string prompt, CancellationToken ct = default)
	{
		try
		{
			AddMessage(prompt, Source.User);

			var request = new ChatCompletionCreateRequest()
			{
				Messages = _messages,
				Model = Models.Gpt_3_5_Turbo
			};
			var result = await _client.CreateCompletion(request, cancellationToken: ct);

			if (result.Successful)
			{
				var response = result.Choices.Select(choice => choice.Message.Content);

				var responseContent = string.Join("", response);

				AddMessage(responseContent, Source.AI);

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

	public async IAsyncEnumerable<ChatResponse> AskAsStream(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
	{
		AddMessage(prompt, Source.User);

		var response = new ChatResponse();
		var content = new StringBuilder();

		var request = new ChatCompletionCreateRequest()
		{
			Messages = _messages,
			Model = Models.Gpt_3_5_Turbo
		};

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
					if (response.Message is { })
					{
						AddMessage(response.Message, Source.AI);
					}
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

	private void AddMessage (string prompt, Source source)
	{
		_messages.Add(source == Source.User ? ChatMessage.FromUser(prompt) : ChatMessage.FromAssistant(prompt));
	}
}
