namespace Voyago.Presentation;

public sealed partial class DestinationDetailPage : Page
{
    public DestinationDetailPage()
    {
        this.InitializeComponent();

        // Hot Design renders without Navigation, so seed a representative destination on the PAGE
        // DataContext (lesson 43). At runtime the DataViewMap injects the tapped Destination's
        // model onto this page and overrides this, so each card opens its own detail.
        this.DataContext = new DestinationDetailModel(new Destination(
            "d-004", "Santorini", "Greece", "Cliffs, caldera views, and unforgettable sunsets",
            "https://images.pexels.com/photos/1010657/pexels-photo-1010657.jpeg?auto=compress&cs=tinysrgb&w=1200",
            "From EUR 399", 4.8, 1562));
    }
}
