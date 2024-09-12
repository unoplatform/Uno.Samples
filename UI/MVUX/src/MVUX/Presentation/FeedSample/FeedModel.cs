using Uno.Extensions.Reactive;

namespace MVUX.Presentation.FeedSample;

public partial record FeedModel
{
	public IFeed<Person> Person { get; }
	
	public FeedModel()
	{
		Person = Feed.Async(async ct =>
		{
			await Task.Delay(2000, ct);
			
			var people = new[]
			{
				new Person("Master", "Yoda"),
				new Person("Darth", "Vader"),
				new Person("Luke", "Skywalker")
			};
			
			return people[new Random().Next(people.Length)];
		});
	}
	
}
