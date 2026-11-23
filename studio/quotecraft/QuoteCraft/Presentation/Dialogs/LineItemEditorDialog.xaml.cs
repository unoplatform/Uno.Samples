namespace QuoteCraft.Presentation;

public sealed partial class LineItemEditorDialog : ContentDialog
{
    public LineItemEditorDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
