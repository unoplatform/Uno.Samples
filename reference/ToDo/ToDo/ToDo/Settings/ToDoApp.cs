namespace ToDo.Configuration;

public partial record ToDoApp
{
	public bool? IsDark { get; init; }
	public string? LastTaskList { get; init; }
}
