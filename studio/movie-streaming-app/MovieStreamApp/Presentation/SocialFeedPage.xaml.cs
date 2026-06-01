namespace MovieStreamApp.Presentation;

public sealed partial class SocialFeedPage : Page
{
    public SocialFeedPage()
    {
        this.InitializeComponent();
        this.DataContext = new SocialFeedModel();
    }
}
