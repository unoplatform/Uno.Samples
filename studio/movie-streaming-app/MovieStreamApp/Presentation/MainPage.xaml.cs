namespace MovieStreamApp.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        // For Hot Design Previews (which bypass Navigation). At runtime the template's
        // ViewMap<MainPage, MainModel> wires the DataContext; replacing this is expected.
        this.DataContext = new MainModel();
    }
}