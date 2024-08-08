namespace Navigation.Presentation;

public class QueryUser
{
	public Guid Id { get; set; }

	public string Name { get; set; }

	public QueryUser(Guid id, string name)
	{
		Id = id;
		Name = name;
	}
}
