namespace GrapeCityApp.Presentation;

internal class FlexGridViewModel
{
    public FlexGridViewModel()
    {
        Customers = Customer.GetCustomerList(100);
    }

    public IEnumerable<Customer> Customers { get; }
}
