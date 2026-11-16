namespace MVUX.Presentation.SelectionSample;

public partial record SelectionModel
{
	public IListFeed<Person> People => ListFeed
		.Async(async ct =>
		{
			await Task.Delay(1000, ct);
			return ImmutableList.Create(
				new Person("Master", "Yoda"),
				new Person("Darth", "Vader"),
				new Person("Luke", "Skywalker")
			);
		})
		.Selection(SelectedPerson);
	public IState<Person> SelectedPerson => State<Person>.Empty(this);
}
