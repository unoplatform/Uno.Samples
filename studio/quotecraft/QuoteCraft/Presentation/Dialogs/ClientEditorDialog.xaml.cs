namespace QuoteCraft.Presentation;

public sealed partial class ClientEditorDialog : ContentDialog
{
    public ClientEditorDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
