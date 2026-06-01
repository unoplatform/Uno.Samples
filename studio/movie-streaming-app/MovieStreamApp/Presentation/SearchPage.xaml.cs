namespace MovieStreamApp.Presentation;

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        this.InitializeComponent();
        this.DataContext = new SearchModel();
    }
}
