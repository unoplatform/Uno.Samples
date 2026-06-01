namespace MovieStreamApp.Presentation;

public sealed partial class MovieDetailPage : Page
{
    public MovieDetailPage()
    {
        this.InitializeComponent();
        this.DataContext = new MovieDetailModel();
    }
}
