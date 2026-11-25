namespace QuoteCraft.Presentation;

public sealed partial class SendQuoteDialog : ContentDialog
{
    public SendQuoteDialog()
    {
        this.InitializeComponent();
        DialogTheme.FollowWindow(this);
    }
}
