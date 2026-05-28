using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation;

public sealed partial class TeamPage : Page
{
    public TeamPage()
    {
        this.InitializeComponent();
    }

    public TeamModel? ViewModel => DataContext as TeamModel;
}
