using Uno.Extensions.Reactive;

namespace MVUX.Presentation.FeedViewSample;

public partial record FeedViewModel
{
	private static readonly Person[] _people = new[]
	{
		new Person("Master", "Yoda"),
		new Person("Darth", "Vader"),
		new Person("Luke", "Skywalker"),
	};

	public IFeed<Person> DefaultPerson => Feed.Async(GetRandomPersonAsync);

	public IFeed<Person> CustomLoadingPerson => Feed.Async(GetRandomPersonWithLargerDelayAsync);

	public IFeed<Person> ErrorPerson => Feed.Async(ThrowErrorAsync);

	private async ValueTask<Person> GetRandomPersonAsync(CancellationToken ct)
	{
		// This simulates a network delay
		await Task.Delay(2000, ct);
		return _people[Random.Shared.Next(_people.Length)];
	}

	private async ValueTask<Person> GetRandomPersonWithLargerDelayAsync(CancellationToken ct)
	{
		// This simulates a network delay
		await Task.Delay(5000, ct);
		return _people[Random.Shared.Next(_people.Length)];
	}

	private async ValueTask<Person> ThrowErrorAsync(CancellationToken ct)
	{
		// This simulates a network delay
		await Task.Delay(2000, ct);
		throw new Exception("An error occurred while fetching the person details.");
	}
}
