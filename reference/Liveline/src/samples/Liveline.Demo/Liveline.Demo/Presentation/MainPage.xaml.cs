using Microsoft.UI.Xaml.Controls;

namespace Liveline.Demo.Presentation;

public sealed partial class MainPage : Page
{
    // Navigation extensions set DataContext to the resolved MainViewModel
    // (registered via ViewMap<MainPage, MainViewModel>).
    public MainPage()
    {
        this.InitializeComponent();
    }
}
