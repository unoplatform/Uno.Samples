using System.Runtime.CompilerServices;
using System.Text;

namespace ChatGPT.Services;

public class MockChatService : IChatService
{
	private const string Default = "Insert ApiKey in appsettings.json.\n\nHello! I am Uno ChatGPT, a helpful assistant. I am here to assist you with any questions you have about developing applications using Uno Platform. Feel free to ask me anything!";

	private const string Recipe = "Insert ApiKey in appsettings.json.\n\nVegan Hot Chocolate Recipe:\n\nIngredients:\n- 2 cups plant-based milk (almond, soy, oat, etc.)\n- 3 tablespoons cocoa powder\n- 2 tablespoons maple syrup or agave nectar\n- 1/2 teaspoon vanilla extract\n- A pinch of salt\n- Optional toppings: vegan whipped cream, cinnamon, or dairy-free chocolate shavings\n\nInstructions:\n1. In a saucepan, heat the plant-based milk over medium heat until warm but not boiling.\n2. In a small bowl, mix the cocoa powder with a few tablespoons of the warm milk to create a smooth paste.\n3. Add the cocoa paste, maple syrup, vanilla extract, and a pinch of salt to the saucepan. Whisk continuously until well combined.\n4. Continue heating the mixture until it reaches your desired temperature, but do not let it boil.\n5. Once heated, pour the hot chocolate into mugs.\n6. Top with vegan whipped cream, a sprinkle of cinnamon, or dairy-free chocolate shavings if desired.\n7. Enjoy your delicious vegan hot chocolate!\n\nNote: Adjust the sweetness and thickness according to your preference by adding more or less maple syrup and cocoa powder.";

	private const string Joke1 = "Insert ApiKey in appsettings.json.\n\nWhy don't scientists trust atoms?\nBecause they make up everything!";
	private const string Joke2 = "Insert ApiKey in appsettings.json.\n\nWhy did the scarecrow win an award?\nBecause he was outstanding in his field!";
	private const string Joke3 = "Insert ApiKey in appsettings.json.\n\nWhat's orange and sounds like a parrot?\nA carrot!";

	private const string FunFact1 = "Insert ApiKey in appsettings.json.\n\nFun Fact: Honey never spoils. Archaeologists have found pots of honey in ancient Egyptian tombs that are over 3,000 years old and still perfectly edible!";
	private const string FunFact2 = "Insert ApiKey in appsettings.json.\n\nFun Fact: The Eiffel Tower can be 15 cm taller during the summer due to thermal expansion of the iron!";
	private const string FunFact3 = "Insert ApiKey in appsettings.json.\n\nFun Fact: A group of flamingos is called a 'flamboyance.'";

	private const string ProgrammingTip1 = "Insert ApiKey in appsettings.json.\n\nProgramming Tip: Take breaks and rest your mind. It helps to step away from your code, go for a walk, or engage in a different activity. You'll often come back with a fresh perspective!";
	private const string ProgrammingTip2 = "Insert ApiKey in appsettings.json.\n\nProgramming Tip: Use version control regularly. Tools like Git help you track changes, collaborate with others, and revert to previous versions if needed.";
	private const string ProgrammingTip3 = "Insert ApiKey in appsettings.json.\n\nProgramming Tip: Write clear and concise code. Well-named variables and functions, along with proper comments, make your code more maintainable and understandable.";


	private List<string> ResponseList =
	[
		Default, Recipe, Joke1, Joke2, Joke3, FunFact1, FunFact2, FunFact3, ProgrammingTip1, ProgrammingTip2, ProgrammingTip3
	];

	public async ValueTask<ChatResponse> AskAsync(ChatRequest request, CancellationToken ct = default)
	{
		await Task.Delay(1000, ct);
		return new ChatResponse(ResponseList[new Random().Next(0, ResponseList.Count)], false);
	}

	public async IAsyncEnumerable<ChatResponse> AskAsStream(ChatRequest request, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var response = new ChatResponse();
		var content = new StringBuilder();

		var message = ResponseList[new Random().Next(0, ResponseList.Count)];

		await Task.Delay(500, ct);

		foreach (var letter in message)
		{
			await Task.Delay(10, ct);

			content.Append(letter);

			response = response with { Message = content.ToString() };

			yield return response;
		}
	}
}