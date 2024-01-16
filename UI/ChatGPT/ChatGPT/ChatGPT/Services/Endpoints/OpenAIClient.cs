// Ignore Spelling: Uno

using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using ChatGPT.Business;

namespace ChatGPT.Services.Endpoints;

internal class OpenAIClient
{
	private OpenAIService _client;
	public OpenAIClient(string apiKey)
	{
		_client = new OpenAIService(
			new OpenAiOptions()
			{
				ApiKey = apiKey
            });
	}

	public async Task<List<string>> CreateCompletions(string prompt)
	{
		var result = await _client.Completions.CreateCompletion(new CompletionCreateRequest() 
		{ 
			Prompt = prompt, 
			Model = Models.Gpt_3_5_Turbo_Instruct,
			Temperature = 0.7f,
			MaxTokens = 512
		});

		if (result.Successful)
		{
			return result.Choices.Select(choice => choice.Text).ToList();
		}
		else
		{
			if (result.Error == null)
			{
				throw new Exception("Unknown Error");
			}

			return new List<string>{ result.Error.Message ?? $"Unknown Error Message", result.Error.Code ?? $"Unknown Error Code." };
		}
	}

	public async Task<ChatResponse> ChatCompletions(string prompt)
	{
		var result = await _client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
		{
			Messages = new List<ChatMessage>
			{
				ChatMessage.FromSystem("You are a helpful assistant."),
				ChatMessage.FromUser(prompt)
			},
			Model = Models.Gpt_3_5_Turbo
        });

        if (result.Successful)
        {
            var _result = result.Choices.Select( choice => choice.Message.Content);
			    return new ChatResponse { Message = _result.ToList() };
        }
		else
		{
			if (result.Error == null)
			{
				throw new Exception("Unknown Error");
			}

			return new ChatResponse 
            { 
                Error = true,
                ErrorMessage = new List<string> { result.Error.Message ?? $"Unknown Error Message", result.Error.Code ?? $"Unknown Error Code." } 
            };
		}
	}
}
