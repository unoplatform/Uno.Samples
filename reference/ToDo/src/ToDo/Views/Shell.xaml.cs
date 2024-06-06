namespace ToDo.Views;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public ContentControl ContentControl => Splash;

    public Shell()
    {
        this.InitializeComponent();
    }
}
