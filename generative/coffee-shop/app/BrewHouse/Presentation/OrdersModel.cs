using Uno.Extensions.Reactive;

namespace BrewHouse.Presentation;

public partial record OrdersModel(IOrderService OrderService)
{
	public IListFeed<Order> Orders => ListFeed.AsyncEnumerable(OrderService.ObserveOrdersAsync);
}
