namespace MovieStreamApp.Presentation;

public sealed partial class OnboardingPage : Page
{
    public OnboardingPage()
    {
        this.InitializeComponent();
        this.DataContext = new OnboardingModel();
    }
}
