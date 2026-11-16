namespace UnoFSharp.Presentation;

using UnoFSharp.FSharpModels;

/// <summary>
/// Model that tests F# record integration with MVUX
/// </summary>
public partial record FSharpTestModel
{
    public FSharpTestModel()
    {
        Title = "F# Records with MVUX Test";
    }

    public string Title { get; }

    // Test 1: Simple F# record as state
    public IState<Person> CurrentPerson => State<Person>.Async(this, async ct =>
    {
        await Task.Delay(100, ct); // Simulate async operation
        return PersonModule.create("John", "Doe", 30);
    });

    // Test 2: F# record with optional fields
    public IState<Product> CurrentProduct => State<Product>.Async(this, async ct =>
    {
        await Task.Delay(100, ct);
        var id = Guid.NewGuid();
        var product = new Product(id, "Test Product", 99.99m, Microsoft.FSharp.Core.FSharpOption<string>.Some("A test product from F#"));
        return product;
    });

    // Test 3: Nested F# records
    public IState<Customer> CurrentCustomer => State<Customer>.Async(this, async ct =>
    {
        await Task.Delay(100, ct);
        var address = new Address("123 Main St", "Springfield", "12345");
        return new Customer(1, "Jane Smith", "jane@example.com", address);
    });

    // Test 4: List/Feed of F# records
    public IListFeed<Person> People => ListFeed<Person>.Async(async (ct) =>
    {
        await Task.Delay(100, ct);
        IImmutableList<Person> people = new[]
        {
            PersonModule.create("Alice", "Anderson", 25),
            PersonModule.create("Bob", "Brown", 35),
            PersonModule.create("Charlie", "Chen", 40)
        }.ToImmutableList();
        return people;
    });

    // Test 5: Computed property using F# helper functions
    public IState<string> CurrentPersonFullName => State<string>.Async(this, async ct =>
    {
        var person = await CurrentPerson;
        return PersonModule.fullName(person);
    });
}
