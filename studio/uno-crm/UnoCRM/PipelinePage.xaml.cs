namespace UnoCRM;

public sealed partial class PipelinePage : Page
{
    public PipelinePage()
    {
        this.InitializeComponent();
        DataContext = CrmData.Pipeline;
    }
}