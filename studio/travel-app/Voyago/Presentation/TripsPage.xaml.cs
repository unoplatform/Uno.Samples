namespace Voyago.Presentation;

public sealed partial class TripsPage : Page
{
    public TripsPage()
    {
        this.InitializeComponent();

        // Set the DataContext so Hot Design Previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions.Navigation resolves the model from the ViewMap<TPage, TModel>
        // and assigns its own instance (with the DI-singleton trip book); replacing this one is
        // expected and harmless.
        this.DataContext = new TripsModel(new Services.TripsService());
    }
}