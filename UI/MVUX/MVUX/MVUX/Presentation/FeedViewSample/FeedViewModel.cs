using Uno.Extensions.Reactive;

namespace MVUX.Presentation.FeedViewSample;

public partial record FeedViewModel
{
	private readonly Random _random = new();
	
	public IFeed<Person> DefaultPerson { get; }
	
	public IFeed<Person> CustomLoadingPerson { get; }
	
	public IFeed<Person> ErrorPerson { get; }
	
	public FeedViewModel()
	{
		DefaultPerson = Feed.Async(GetRandomPersonAsync);
		
		CustomLoadingPerson = Feed.Async(GetRandomPersonAsync);
		
		ErrorPerson = Feed.Async(ThrowErrorAsync);
	}
	
	private async ValueTask<Person> GetRandomPersonAsync(CancellationToken ct)
	{
		await Task.Delay(10000, ct);
		
		var people = new[]
		{
			new Person("Master", "Yoda"),
			new Person("Darth", "Vader"),
			new Person("Luke", "Skywalker"),
		};
		
		return people[_random.Next(0, people.Length)];
	}
	
	private async ValueTask<Person> ThrowErrorAsync(CancellationToken ct)
	{
		await Task.Delay(10000, ct);
		throw new Exception("An error occurred while fetching the person details.");
	}
}
