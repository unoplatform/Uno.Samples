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
            "https://picsum.photos/seed/santorini%20greece%20island/1280/720",
            "From EUR 399", 4.8, 1562));
    }
}
