using Microsoft.UI.Xaml.Controls;

namespace Voyago.Presentation;

public sealed partial class DestinationDetailDialog : ContentDialog
{
    public DestinationDetailDialog()
    {
        this.InitializeComponent();

        // Hot Design renders without Navigation, so seed a representative destination. At runtime the
        // DataViewMap injects the tapped Destination's model and overrides this.
        this.DataContext = new DestinationDetailModel(Catalog.Santorini, new Services.TripsService());
    }
}
