namespace MovieStreamApp.Presentation;

public sealed partial class PlaybackPage : Page
{
    public PlaybackPage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; the DataViewMap injects the playing movie at runtime, overriding this.
        this.DataContext = new PlaybackModel(MovieData.Featured);
    }
}
