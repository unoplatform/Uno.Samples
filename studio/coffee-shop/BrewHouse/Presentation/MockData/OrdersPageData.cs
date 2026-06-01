using System.Collections.ObjectModel;

namespace BrewHouse.Presentation.MockData;

public class OrdersPageData
{
    public string EmptyStateVisibility => Orders.Count == 0 ? "Visible" : "Collapsed";
    public string OrdersListVisibility => Orders.Count > 0 ? "Visible" : "Collapsed";

    public ObservableCollection<OrderRecord> Orders { get; set; } = new()
    {
        new OrderRecord
        {
            Id = "ORD-1042",
            PlacedAt = "Today, 9:14 AM",
            Status = "Ready for Pickup",
            StatusColor = "#FFFFFFFF",
            StatusBackground = "#FF2E7D32",
            Total = 12.25,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { Name = "Latte", Quantity = 1, Price = 5.50, ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
                new OrderLineItem { Name = "Croissant", Quantity = 2, Price = 3.25, ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
            }
        },
        new OrderRecord
        {
            Id = "ORD-1041",
            PlacedAt = "Today, 8:02 AM",
            Status = "Preparing",
            StatusColor = "#FFFFFFFF",
            StatusBackground = "#FFF57C00",
            Total = 10.75,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { Name = "Cappuccino", Quantity = 1, Price = 4.75, ImageUrl = "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
                new OrderLineItem { Name = "Iced Matcha", Quantity = 1, Price = 6.00, ImageUrl = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
            }
        },
        new OrderRecord
        {
            Id = "ORD-1039",
            PlacedAt = "Yesterday, 3:45 PM",
            Status = "Completed",
            StatusColor = "#FF2C1A0E",
            StatusBackground = "#FFE8DDD0",
            Total = 5.50,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { Name = "Latte", Quantity = 1, Price = 5.50, ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
            }
        },
        new OrderRecord
        {
            Id = "ORD-1037",
            PlacedAt = "Mar 22, 11:30 AM",
            Status = "Completed",
            StatusColor = "#FF2C1A0E",
            StatusBackground = "#FFE8DDD0",
            Total = 15.50,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { Name = "Cappuccino", Quantity = 2, Price = 4.75, ImageUrl = "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
                new OrderLineItem { Name = "Croissant", Quantity = 1, Price = 3.25, ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
                new OrderLineItem { Name = "Iced Matcha", Quantity = 1, Price = 6.00, ImageUrl = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
            }
        },
        new OrderRecord
        {
            Id = "ORD-1033",
            PlacedAt = "Mar 20, 8:55 AM",
            Status = "Completed",
            StatusColor = "#FF2C1A0E",
            StatusBackground = "#FFE8DDD0",
            Total = 9.25,
            Items = new List<OrderLineItem>
            {
                new OrderLineItem { Name = "Iced Matcha", Quantity = 1, Price = 6.00, ImageUrl = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
                new OrderLineItem { Name = "Croissant", Quantity = 1, Price = 3.25, ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940" },
            }
        },
    };

}
