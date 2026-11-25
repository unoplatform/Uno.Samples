namespace QuoteCraft.Presentation;

public sealed partial class CatalogItemEditorDialog : ContentDialog
{
    public CatalogItemEditorDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
