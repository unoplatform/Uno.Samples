namespace MovieStreamApp.Presentation;

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; Navigation overrides this.DataContext at runtime.
        this.DataContext = new SearchViewModel();
    }
}
