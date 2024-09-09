namespace MVUX.Presentation.ListFeedSample;
public sealed partial class ListFeedPage : Page
{
	public ListFeedPage()
	{
		this.InitializeComponent();

		this.DataContext = new BindableListFeedModel();
	}
}
