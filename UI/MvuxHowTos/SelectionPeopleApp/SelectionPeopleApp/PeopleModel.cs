using Uno.Extensions.Reactive;
using Windows.UI.Popups;

namespace SelectionPeopleApp;

public partial record PeopleModel
{
    private IPeopleService _peopleService;

    public PeopleModel(IPeopleService peopleService)
    {
        _peopleService = peopleService;

        SelectedPerson.ForEachAsync(action: SelectionChanged);
    }

    public IListFeed<Person> People =>
        ListFeed
        .Async(_peopleService.GetPeopleAsync)
        .Selection(SelectedPerson);
    // multi selection
    //.Selection(SelectedPeople);

    public IState<Person> SelectedPerson => State<Person>.Empty(this);

    public IState<IImmutableList<Person>> SelectedPeople => State<IImmutableList<Person>>.Empty(this);

    public IFeed<string> GreetingSelect =>
        SelectedPerson
        .Where(person => person != null)
        .Select(person => $"Hello {person.FirstName} {person.LastName}!");


    public IState<string> GreetingForEach => State.Value(this, () => string.Empty);
    public async ValueTask SelectionChanged(Person? selectedPerson, CancellationToken ct)
    {
        if (selectedPerson == null)
            return;

        await GreetingForEach.Set($"Hello {selectedPerson.FirstName} {selectedPerson.LastName}!", ct);
    }
}