// Ignore Spelling: Uno

using OpenAI.Managers;
using OpenAI;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace UnoChatGPT.Services.Endpoints
{
	internal class OpenAIClient
	{
		private OpenAIService _client;
		public OpenAIClient()
		{
			_client = new OpenAIService(
				new OpenAiOptions()
				{
					ApiKey = "<insert api key here>"
				});
		}

		public async Task<List<string>> CreateCompletions(string prompt)
		{
			var result = await _client.Completions.CreateCompletion(new CompletionCreateRequest() 
			{ 
				Prompt = prompt, 
				Model = Models.TextDavinciV3,
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

		public async Task<List<string>> ChatCompletions(string prompt)
		{
			var result = await _client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
			{
				Messages = new List<ChatMessage>
				{
					ChatMessage.FromSystem("You are a helpful assistant."),
					ChatMessage.FromUser(prompt)
				},
				Model = Models.ChatGpt3_5Turbo
			});

            if (result.Successful)
            {
                var _result = result.Choices.Select( choice => choice.Message.Content);
				return _result.ToList();
            }
			else
			{
				if (result.Error == null)
				{
					throw new Exception("Unknown Error");
				}

				return	new List<string> { result.Error.Message ?? $"Unknown Error Message", result.Error.Code ?? $"Unknown Error Code." };
			}
		}
	}
}
