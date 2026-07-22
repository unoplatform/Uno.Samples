using Microsoft.UI.Xaml.Controls;

namespace Voyago.Presentation;

public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();

        // Set the DataContext so Hot Design Previews — which construct the page directly,
        // without running Navigation — render with the model's data. At runtime
        // Uno.Extensions.Navigation resolves the model from the ViewMap<TPage, TModel>
        // and assigns its own instance; replacing this one is expected and harmless.
        this.DataContext = new HomeModel();
    }

    // Mirror the hero carousel onto the (display-only) pager on the one event that is reliable on
    // Uno — the TwoWay PipsPager<->FlipView binding is not (lesson 49).
    private void OnHeroSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (HeroPager.SelectedPageIndex != Hero.SelectedIndex)
        {
            HeroPager.SelectedPageIndex = Hero.SelectedIndex;
        }
    }
}
