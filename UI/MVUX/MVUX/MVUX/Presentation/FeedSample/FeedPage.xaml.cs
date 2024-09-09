namespace MVUX.Presentation.FeedSample;

public sealed partial class FeedPage : Page
{
	public FeedPage()
	{
		this.InitializeComponent();
		this.DataContext = new BindableFeedModel();

	}
}
