namespace UnoChatGPT.Presentation
{
	public sealed partial class MainPage : Page
	{

        internal OpenAIClient Client { get; set; }
        public MainPage()
		{
			Client = new OpenAIClient();
			this.InitializeComponent();
		}

		private async void chatCompletionButton_Click(object sender, RoutedEventArgs e)
		{
			if(!string.IsNullOrWhiteSpace(chatCompletionPrompt.Text))
			{
				var results = await Client.ChatCompletions(chatCompletionPrompt.Text);

				if(results != null && results.Count > 0)
				{
					firstAnswer.Text = results[0];

					chatCompletionResults.Children.Clear();
					foreach (var result in results.Skip(1))
					{
						var textBlock = new TextBlock()
						{
							Text = result,
						};
						chatCompletionResults.Children.Add(textBlock);
					}
				}
			}
        }

		private async void textCompletionButton_Click(object sender, RoutedEventArgs e)
		{
			if(!string.IsNullOrWhiteSpace(textCompletionPrompt.Text))
			{
				var results = await Client.CreateCompletions(textCompletionPrompt.Text);

				if (results != null && results.Count > 0)
				{
					firstChoice.Text = results[0];
					
					textCompletionResults.Children.Clear();
					foreach (var result in results.Skip(1))
					{
						var textBlock = new TextBlock()
						{
							Text = result,
							TextWrapping = TextWrapping.WrapWholeWords,
							MinHeight = 100
						};

						textCompletionResults.Children.Add(textBlock);
					}
				}
			}
		}
	}
}