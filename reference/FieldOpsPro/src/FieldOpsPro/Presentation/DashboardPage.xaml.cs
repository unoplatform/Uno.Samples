using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();
    }

    public DashboardModel? ViewModel => DataContext as DashboardModel;
}
