namespace MovieStreamApp.Presentation;

public sealed partial class ProfilePage : Page
{
    public ProfilePage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; Navigation overrides this.DataContext at runtime.
        this.DataContext = new ProfileViewModel(new WatchlistService());
    }
}
