namespace Commerce.Presentation;
using Uno.Extensions.Reactive;

public partial record FiltersViewModel
{
	public FiltersViewModel(Filters filters)
	{
		Filter = State.Value(this, () => filters);
	}

	public IState<Filters> Filter { get; }
}
