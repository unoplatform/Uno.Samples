namespace UnoCRM.Presentation;

public sealed partial class DealDetailPage : Page
{
    public DealDetailPage()
    {
        this.InitializeComponent();

        // Design-time DataContext for Hot Design / Studio. At runtime Uno.Extensions
        // Navigation injects the mapped DealDetailModel with the tapped deal.
        this.DataContext = new DealDetailModel(CrmData.Deals[0]);
    }
}
