namespace QuoteCraft.Presentation;

public sealed partial class PhotoViewerDialog : ContentDialog
{
    public PhotoViewerDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
