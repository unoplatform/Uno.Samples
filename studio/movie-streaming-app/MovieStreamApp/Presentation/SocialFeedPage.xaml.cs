namespace MovieStreamApp.Presentation;

public sealed partial class SocialFeedPage : Page
{
    public SocialFeedPage()
    {
        this.InitializeComponent();

        // Seed a sample DataContext so the Hot Design Previews gallery (which renders without
        // Navigation) populates; Navigation overrides this.DataContext at runtime.
        this.DataContext = new SocialFeedModel();
    }
}
