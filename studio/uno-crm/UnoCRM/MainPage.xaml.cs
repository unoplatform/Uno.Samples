namespace UnoCRM;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        DataContext = CrmData.Dashboard;
    }
}
