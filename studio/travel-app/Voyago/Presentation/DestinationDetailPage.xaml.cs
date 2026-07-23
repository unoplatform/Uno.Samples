namespace Voyago.Presentation;

public sealed partial class DestinationDetailPage : Page
{
    public DestinationDetailPage()
    {
        this.InitializeComponent();

        // Hot Design renders without Navigation, so seed a representative destination on the PAGE
        // DataContext. At runtime the DataViewMap injects the tapped Destination's model onto this
        // page and overrides this, so each card opens its own detail.
        this.DataContext = new DestinationDetailModel(Catalog.Santorini, new Services.TripsService());
    }
}
