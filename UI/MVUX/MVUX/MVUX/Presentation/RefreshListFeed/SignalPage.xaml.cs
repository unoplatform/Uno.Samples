namespace MVUX.Presentation.RefreshListFeed;
public sealed partial class SignalPage : Page
{
    public SignalPage()
    {
        this.InitializeComponent();
        this.DataContext = new RefreshSignalModel();
    }
}
