namespace Navigation.Presentation;

public partial class ToFromQueryViewModel : ObservableObject
{
	[ObservableProperty]
	private QueryUser user;

	public ToFromQueryViewModel(QueryUser user)
	{
		User = user;
	}
}
