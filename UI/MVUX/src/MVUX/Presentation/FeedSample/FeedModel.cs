using Uno.Extensions.Reactive;

namespace MVUX.Presentation.FeedSample;

public partial record FeedModel
{
	private static readonly Person[] _people = new[]
			{
				new Person("Master", "Yoda"),
				new Person("Darth", "Vader"),
				new Person("Luke", "Skywalker")
			};

	public IFeed<Person> Person => Feed.Async(async ct =>
		{
			await Task.Delay(2000, ct); // Simulate network delay
						
			return _people[Random.Shared.Next(_people.Length)];
		});
	
}
