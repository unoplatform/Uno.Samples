namespace QuoteCraft.Presentation;

public sealed partial class NewQuoteDialog : ContentDialog
{
    public NewQuoteDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
