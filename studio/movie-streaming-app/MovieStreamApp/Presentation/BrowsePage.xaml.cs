namespace MovieStreamApp.Presentation;

public sealed partial class BrowsePage : Page
{
    public BrowsePage()
    {
        this.InitializeComponent();
        this.DataContext = new BrowseModel();
    }
}
