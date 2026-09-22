namespace Navigation.Models;

// TODO: Should be a record
public partial class Entity : ObservableObject
{
	[ObservableProperty]
	private string? name;

	public Entity(string name)
	{
		Name = name;
	}
}
