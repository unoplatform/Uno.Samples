using ChatGPT.Business;
using OpenAI.Managers;
using OpenAI;
using System.Runtime.CompilerServices;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using System.Text;
using OpenAI.ObjectModels.ResponseModels;

namespace ChatGPT.Services;
public class ChatService : IChatService
{
	private OpenAIService _client;

	public ChatService(IOptions<AppConfig> appConfig)
	{
		var apiKey = appConfig.Value.ApiKey;

		_client = new OpenAIService(
			new OpenAiOptions()
			{
				ApiKey = apiKey
			});
	}

	public async ValueTask<ChatResponse> AskAsync(string prompt)
	{

		var result = await _client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
		{
			Messages = new List<ChatMessage>
			{
				ChatMessage.FromUser(prompt)
			},
			Model = Models.Gpt_3_5_Turbo
		});

		if (result.Successful)
		{
			var _result = result.Choices.Select(choice => choice.Message.Content);
			return new ChatResponse(Status.Value, string.Join("", _result));
		}
		else
		{
			if (result.Error == null)
			{
				throw new Exception("Unknown Error");
			}
			return new ChatResponse(Status.Error, $"{result.Error.Message ?? "Unknown Error Message"} {result.Error.Code ?? "Unknown Error Code"}");
		}
	}

	public async IAsyncEnumerable<ChatResponse> AskAsStream(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var response = new ChatResponse(Status.Loading);
		var content = new StringBuilder();

		yield return response;

		var request = new ChatCompletionCreateRequest()
		{
			Messages = new List<ChatMessage> { ChatMessage.FromUser(prompt) },
			Model = Models.Gpt_3_5_Turbo
		};

		IAsyncEnumerator<ChatCompletionCreateResponse>? responseStream = default;
		while (response is { Status: Status.Loading })
		{
			try
			{
				responseStream ??= _client.ChatCompletion.CreateCompletionAsStream(request).GetAsyncEnumerator(ct);
				if (await responseStream.MoveNextAsync())
				{
					foreach (var choice in responseStream.Current.Choices)
					{
						content.Append(choice.Message.Content);
					}
					response = response with { Message = content.ToString() };
				}
				else
				{
					response = response with { Status = Status.Value };
				}
			}
			catch (Exception)
			{
				response = response with { Status = Status.Error };
			}

			yield return response;
		}
	}
}
