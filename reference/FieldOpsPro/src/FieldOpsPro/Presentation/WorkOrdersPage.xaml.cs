using Microsoft.UI.Xaml.Controls;

namespace FieldOpsPro.Presentation;

public sealed partial class WorkOrdersPage : Page
{
    public WorkOrdersPage()
    {
        this.InitializeComponent();
    }

    public WorkOrdersModel? ViewModel => DataContext as WorkOrdersModel;
}
