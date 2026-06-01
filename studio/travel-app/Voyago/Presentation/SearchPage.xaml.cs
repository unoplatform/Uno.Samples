namespace Voyago.Presentation;

public sealed partial class SearchPage : Page
{
    public SearchPage()
    {
        this.InitializeComponent();
        this.DataContext = new SearchModel();

        // Set the DataContext so Hot Design Previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions.Navigation resolves the model from the ViewMap<TPage, TModel>
        // and assigns its own instance; replacing this one is expected and harmless.
        this.DataContext = new SearchModel();
    }
}