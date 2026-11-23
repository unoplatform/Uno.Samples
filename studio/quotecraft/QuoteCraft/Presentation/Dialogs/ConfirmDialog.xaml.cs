namespace QuoteCraft.Presentation;

public sealed partial class ConfirmDialog : ContentDialog
{
    public ConfirmDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
