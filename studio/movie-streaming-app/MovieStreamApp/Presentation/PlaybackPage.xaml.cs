namespace MovieStreamApp.Presentation;

public sealed partial class PlaybackPage : Page
{
    public PlaybackPage()
    {
        this.InitializeComponent();
        this.DataContext = new PlaybackModel();
    }
}
