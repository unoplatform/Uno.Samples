namespace EditablePeopleApp;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        DataContext = new BindablePeopleModel(new PeopleService());
    }
}