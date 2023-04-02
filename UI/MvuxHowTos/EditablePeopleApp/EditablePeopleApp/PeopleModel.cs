using Uno.Extensions.Reactive;

namespace EditablePeopleApp;


// It's important to add the partial modifier to enable MVUX
// to add another partial class with generated code.
public partial record PeopleModel(PeopleService DataStore)
{
    // Reading the following line, Uno Platform MVUX is going to generate
    // an underlying bindable Person collection that's easy for the UI to interact with.
    public IListState<Person> People => ListState.Async(this, DataStore.GetPeople);

    // This is a placeholder for a new person to be added.
    // Again MVUX will generate a bindable object for this property.
    public IState<Person> NewPerson => State<Person>.Empty(this);

    // MVUX will generate an async command for each public task.
    // In this case MVUX generates an AddPerson command.
    // The CancellationToken parameter is optional.
    public async ValueTask AddPerson(CancellationToken ct = default)
    {
        // get the current state of the new person
        var newPerson = (await NewPerson)!;

        // save the new person to our 'fancy server'
        // the ID of the newly created Person is returned from server
        var newId = await DataStore.AddPerson(newPerson, ct);

        // We can either refresh the whole list of people from server
        // or just manually add the new person with its new Id
        await People.AddAsync(newPerson with { Id = newId }, ct);

        // clear the NewPerson placeholder
        var emptyPerson = new Person(Id: 0, FirstName: string.Empty, LastName: string.Empty);
        await NewPerson.Update(current => emptyPerson, ct);
    }

    // A RemovePerson command is generated.
    public async ValueTask RemovePerson(Person person, CancellationToken ct = default)
    {
        var personId = person.Id;

        // remove person from server
        await DataStore.RemovePerson(personId, ct);

        // remove person from UI
        await People.RemoveAllAsync(p => p.Id == personId, ct);
    }
}