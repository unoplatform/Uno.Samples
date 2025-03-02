using System.Collections.ObjectModel;
using TelerikApp.Business.Services;

namespace TelerikApp.Presentation;

internal class DataGridSampleViewModel
{
    private const string OrdersPath = "OrdersDataSource.xml";
    private const string PeoplePath = "PeopleDataSource.xml";

    public DataGridSampleViewModel(DataGenerator generator)
    {
        OrderDetails = generator.GetItems<ObservableCollection<Order>>(OrdersPath);
        People = generator.GetItems<ObservableCollection<SalesPerson>>(PeoplePath);
    }

    public ObservableCollection<Order> OrderDetails { get; }
    public ObservableCollection<SalesPerson> People { get; }
}
