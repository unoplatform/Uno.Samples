namespace QuoteCraft.Presentation;

public sealed partial class CatalogBrowserDialog : ContentDialog
{
    public CatalogBrowserDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
