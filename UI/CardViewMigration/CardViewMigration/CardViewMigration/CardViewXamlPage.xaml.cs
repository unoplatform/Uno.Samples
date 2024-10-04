namespace CardViewMigration;

public sealed partial class CardViewXamlPage : Page
{
    public CardViewXamlPage()
    {
        InitializeComponent();
    }

    public void GoBack()
    {
        Frame.GoBack();
    }
}
