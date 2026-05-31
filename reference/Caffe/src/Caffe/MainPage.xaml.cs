using Caffe.Models;
using Caffe.ViewModels;

namespace Caffe;

public sealed partial class MainPage : Page
{
    // Framework wires DataContext via ViewMap<MainPage, MainViewModel>().
    // x:Bind paths read `ViewModel.X` off this property; we re-evaluate the
    // compiled bindings when DataContext lands.
    public MainViewModel? ViewModel => DataContext as MainViewModel;

    public MainPage()
    {
        this.InitializeComponent();
        this.DataContextChanged += (_, _) => Bindings.Update();
    }

    private void OnEspressoCardTapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is Controls.EspressoCard { Espresso: { } espresso } && ViewModel is { } vm)
        {
            vm.SelectedEspresso = espresso;
        }
    }

    // x:Bind function binding (see MainPage.xaml) — drives each card's selected visual.
    private bool IsCardSelected(EspressoItem? item, EspressoItem? selected) => item == selected;

    private async void OnBrewRequested(object sender, EventArgs e)
    {
        if (ViewModel is { } vm && vm.BrewCommand.CanExecute(null))
        {
            await vm.BrewCommand.ExecuteAsync(null);
        }
    }
}
