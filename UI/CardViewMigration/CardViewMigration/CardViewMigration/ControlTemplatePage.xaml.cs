namespace CardViewMigration;

public sealed partial class ControlTemplatePage : Page
{
    public ControlTemplatePage()
    {
        InitializeComponent();
    }

    public void GoBack()
    {
        Frame.GoBack();
    }
}
