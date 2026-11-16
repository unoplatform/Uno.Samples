namespace UnoFSharp.Presentation;

using UnoFSharp.FSharpModels;

/// <summary>
/// Interactive model demonstrating MVUX commands and state management with F# records
/// </summary>
public partial record InteractiveModel
{
    private readonly IListState<TodoItem> _todoItems;
    
    public InteractiveModel()
    {
        Title = "Interactive F# + MVUX Demo";
        _todoItems = ListState<TodoItem>.Empty(this);
    }

    public string Title { get; }

    // Input state for new todo items
    public IState<string> NewTodoTitle => State<string>.Value(this, () => string.Empty);
    public IState<string> NewTodoDescription => State<string>.Value(this, () => string.Empty);

    // Todo list state
    public IListState<TodoItem> TodoItems => _todoItems;

    // Commands
    public async ValueTask AddTodo()
    {
        var title = await NewTodoTitle;
        var description = await NewTodoDescription;

        if (string.IsNullOrWhiteSpace(title))
            return;

        var newItem = TodoModule.createItem(title, description ?? string.Empty);
        var currentItems = await TodoItems;
        
        await TodoItems.UpdateAsync(_ => currentItems.Add(newItem));
        await NewTodoTitle.UpdateAsync(_ => string.Empty);
        await NewTodoDescription.UpdateAsync(_ => string.Empty);
    }

    public async ValueTask ToggleTodo(TodoItem item)
    {
        var currentItems = await TodoItems;
        var index = currentItems.IndexOf(item);
        
        if (index >= 0)
        {
            var updatedItem = new TodoItem(item.Id, item.Title, item.Description, !item.IsCompleted, item.CreatedAt);
            await TodoItems.UpdateAsync(_ => currentItems.SetItem(index, updatedItem));
        }
    }

    public async ValueTask DeleteTodo(TodoItem item)
    {
        var currentItems = await TodoItems;
        await TodoItems.UpdateAsync(_ => currentItems.Remove(item));
    }

    public async ValueTask ClearCompleted()
    {
        var currentItems = await TodoItems;
        var remaining = currentItems.Where(item => !item.IsCompleted).ToImmutableList();
        await TodoItems.UpdateAsync(_ => remaining);
    }

    public async ValueTask CompleteAll()
    {
        var currentItems = await TodoItems;
        var allCompleted = currentItems.Select(item => new TodoItem(item.Id, item.Title, item.Description, true, item.CreatedAt)).ToImmutableList();
        await TodoItems.UpdateAsync(_ => allCompleted);
    }

    // Sample data loader
    public async ValueTask LoadSampleData()
    {
        var sampleItems = new[]
        {
            TodoModule.createItem("Learn F# records", "Understand immutable record types"),
            TodoModule.createItem("Integrate with MVUX", "Test state management with F# types"),
            TodoModule.createItem("Build interactive UI", "Create a todo app demo")
        }.ToImmutableList();

        await TodoItems.UpdateAsync(_ => sampleItems);
    }
}
