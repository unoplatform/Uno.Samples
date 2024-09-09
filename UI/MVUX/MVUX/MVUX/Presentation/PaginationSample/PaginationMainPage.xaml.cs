namespace MVUX.Presentation.PaginationSample;
public sealed partial class PaginationMainPage : Page
{
	public PaginationMainPage()
	{
		this.InitializeComponent();

		this.DataContext = new BindablePaginationPeopleModel(new PaginationPeopleService());
	}
}
