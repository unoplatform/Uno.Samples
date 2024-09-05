
namespace MVUX.Presentation.FeedViewSample;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class FeedViewPage : Page
{
	public FeedViewPage()
	{
		this.InitializeComponent();
		this.DataContext = new BindableFeedViewModel();
	}
}