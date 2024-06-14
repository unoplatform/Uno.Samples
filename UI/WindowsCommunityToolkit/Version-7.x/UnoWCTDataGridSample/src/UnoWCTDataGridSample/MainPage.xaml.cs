using System.Runtime.CompilerServices;
using UnoWCTDataGridSample.Data;

namespace UnoWCTDataGridSample;

public sealed partial class MainPage : Page
{
    private DataGridDataSource viewModel = new DataGridDataSource();

    public MainPage()
    {
        this.InitializeComponent();

        Loading += MainPage_Loading;
    }

    private async void MainPage_Loading(object sender, object args)
    {
        dataGrid.ItemsSource = await viewModel.GetDataAsync();
    }
}
