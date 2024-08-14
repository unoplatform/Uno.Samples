namespace Navigation.Presentation;

public class QueryUserService : IQueryUserService
{
	private readonly List<QueryUser> _users;

	public QueryUserService()
	{
		_users = new List<QueryUser>
		{
			new QueryUser(ConvertFromString("8a5c5b2e-ff96-474b-9e4d-65bde598f6bc"), "João Rodrigues"),
			new QueryUser(ConvertFromString("2b64071a-2c8a-45e4-9f48-3eb7d7aace41"), "Ross Polard")
		};
	}

	public QueryUser? GetById(Guid id)
	{
		return _users.FirstOrDefault(user => user.Id == id);
	}

	private Guid ConvertFromString(string value)
	{
		Guid guid = Guid.Parse(value);
		return guid;
	}
}
