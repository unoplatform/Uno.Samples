using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation;

public sealed partial class MapPage : Page
{
    public MapPage()
    {
        this.InitializeComponent();
    }

    public MapModel? ViewModel => DataContext as MapModel;
}
