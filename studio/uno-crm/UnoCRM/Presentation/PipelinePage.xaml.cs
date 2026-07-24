namespace UnoCRM.Presentation;

public sealed partial class PipelinePage : Page
{
    public PipelinePage()
    {
        this.InitializeComponent();

        // Design-time DataContext for Hot Design / Studio. At runtime Uno.Extensions
        // Navigation injects the mapped PipelineModel, which overrides this.
        this.DataContext = new PipelineModel();
    }
}
