namespace UnoCRM.Presentation;

public sealed partial class LeadsPage : Page
{
    public LeadsPage()
    {
        this.InitializeComponent();

        // Design-time DataContext for Hot Design / Studio. At runtime Uno.Extensions
        // Navigation injects the mapped LeadsModel (which builds the LiveCharts series from the
        // shared dataset), overriding this.
        this.DataContext = new LeadsModel();
    }
}
