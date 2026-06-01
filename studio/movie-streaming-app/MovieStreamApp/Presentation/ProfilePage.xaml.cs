namespace MovieStreamApp.Presentation;

public sealed partial class ProfilePage : Page
{
    public ProfilePage()
    {
        this.InitializeComponent();
        this.DataContext = new ProfileModel();
    }
}
