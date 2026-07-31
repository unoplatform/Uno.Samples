namespace MovieStreamApp.Presentation;

public sealed partial class MovieDetailPage : Page
{
    public MovieDetailPage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; the DataViewMap injects the tapped movie at runtime, overriding this.
        this.DataContext = new MovieDetailViewModel(MovieData.Featured, new WatchlistService());
    }
}
