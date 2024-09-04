namespace MVUX.Presentation.ListFeedSample;

public partial record ListFeedModel
{
    public IListFeed<Person> People { get; }
    public ListFeedModel()
    {
        People = ListFeed.Async(async ct =>
        {
            await Task.Delay(500, ct);
            return ImmutableList.Create(
                new Person("Master", "Yoda"),
                new Person("Darth", "Vader"),
                new Person("Luke", "Skywalker")
            );
        });
    }
}
