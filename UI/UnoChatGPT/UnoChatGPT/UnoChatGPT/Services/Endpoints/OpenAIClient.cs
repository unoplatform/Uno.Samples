using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI.GPT3.ObjectModels;

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
					ApiKey = "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" // "<insert api key here>"
				});
		}

		public async Task<string> CreateCompletions(string prompt)
		{
			var result = await _client.Completions.CreateCompletion(new CompletionCreateRequest() { Prompt = prompt, Model = OpenAI.GPT3.ObjectModels.Models.TextDavinciV3 });

			if (result.Successful)
			{
				return result.Choices.Select(choice => choice.Text).FirstOrDefault(string.Empty);
			}
			else
			{
				return "Error";
			}
		}

		public async Task<string[]> ChatCompletions(string prompt)
		{
			var result = await _client.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
			{
				Messages = new List<ChatMessage>
				{
					ChatMessage.FromSystem("You are a helpful assistant."),
					ChatMessage.FromUser("Who won the world series in 2020?"),
					ChatMessage.FromAssistant("The Los Angeles Dodgers won the World Series in 2020."),
					ChatMessage.FromUser("Where was it played?")
				},
				Model = Models.ChatGpt3_5Turbo
			});

            if (result.Successful)
            {
                var _result = result.Choices.Select( choice => choice.Message.Content);
				return _result.ToArray();
            }
			else
			{
				if (result.Error == null)
				{
					throw new Exception("Unknown Error");
				}

				return	new string[] { result.Error.Message ?? result.Error.Message : string.Empty };
			}
		}
	}
}
