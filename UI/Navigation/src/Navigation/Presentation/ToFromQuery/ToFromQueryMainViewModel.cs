namespace Navigation.Presentation;

public partial class ToFromQueryMainViewModel : ObservableObject
{
	[ObservableProperty]
	private IDictionary<string, object> user1;

	[ObservableProperty]
	private IDictionary<string, object> user2;

	public ToFromQueryMainViewModel()
	{
		User1 = new Dictionary<string, object>
		{
			{ "QueryUser.Id", Guid.Parse("8a5c5b2e-ff96-474b-9e4d-65bde598f6bc") }
		};

		User2 = new Dictionary<string, object>
		{
			{ "QueryUser.Id", Guid.Parse("2b64071a-2c8a-45e4-9f48-3eb7d7aace41") }
		};
	}
}
