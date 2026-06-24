using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BrewHouse.Presentation.MockData;

public class AppState : INotifyPropertyChanged
{
    private static AppState? _current;
    public static AppState Current => _current ??= new AppState();

    public List<ProductItem> AllProducts { get; } = new()
    {
        new ProductItem
        {
            Id = "p-001",
            Name = "Classic Latte",
            Description = "Smooth espresso with velvety steamed milk and a delicate foam layer.",
            Category = "Hot Drinks",
            CategoryId = "hot",
        Price = "5.50",
        PriceValue = 5.50,
            ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        },
        new ProductItem
        {
            Id = "p-002",
            Name = "Cappuccino",
            Description = "Equal parts espresso, steamed milk and thick microfoam — a true Italian classic.",
            Category = "Hot Drinks",
            CategoryId = "hot",
        Price = "4.75",
        PriceValue = 4.75,
            ImageUrl = "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = false
        },
        new ProductItem
        {
            Id = "p-003",
            Name = "Butter Croissant",
            Description = "Flaky, golden-baked croissant with premium French butter. Freshly baked each morning.",
            Category = "Pastries",
            CategoryId = "pastries",
        Price = "3.25",
        PriceValue = 3.25,
            ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        },
        new ProductItem
        {
            Id = "p-004",
            Name = "Iced Matcha",
            Description = "Premium ceremonial matcha blended with oat milk over crushed ice.",
            Category = "Cold Drinks",
            CategoryId = "cold",
        Price = "6.00",
        PriceValue = 6.00,
            ImageUrl = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = false
        },
        new ProductItem
        {
            Id = "p-005",
            Name = "Flat White",
            Description = "Ristretto shots with a thin layer of velvety micro-foam — bold and smooth.",
            Category = "Hot Drinks",
            CategoryId = "hot",
        Price = "4.50",
        PriceValue = 4.50,
            ImageUrl = "https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = false,
            IsSpecial = false
        },
        new ProductItem
        {
            Id = "p-006",
            Name = "Cold Brew",
            Description = "Steeped for 18 hours in cold water for a naturally sweet, low-acid brew.",
            Category = "Cold Drinks",
            CategoryId = "cold",
        Price = "5.00",
        PriceValue = 5.00,
            ImageUrl = "https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = false,
            IsSpecial = false
        },
        new ProductItem
        {
            Id = "p-007",
            Name = "Almond Muffin",
            Description = "Moist almond muffin with a crunchy streusel topping and hint of vanilla.",
            Category = "Pastries",
            CategoryId = "pastries",
        Price = "2.95",
        PriceValue = 2.95,
            ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = false,
            IsSpecial = false
        },
        new ProductItem
        {
            Id = "p-008",
            Name = "Caramel Macchiato",
            Description = "Layers of vanilla syrup, steamed milk, espresso and drizzled caramel.",
            Category = "Hot Drinks",
            CategoryId = "hot",
        Price = "5.75",
        PriceValue = 5.75,
            ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = false,
            IsSpecial = true
        },
    };

    // Shared category definitions; each page builds its own instances so per-page
    // selection state never leaks between screens.
    public static List<CategoryItem> CreateCategories() =>
    [
        new() { Id = "all",      Name = "All",         IsSelected = true },
        new() { Id = "hot",      Name = "Hot Drinks" },
        new() { Id = "cold",     Name = "Cold Drinks" },
        new() { Id = "pastries", Name = "Pastries" },
    ];

    public ObservableCollection<CartItem> Cart { get; } = new();
    public ObservableCollection<OrderRecord> Orders { get; } = new();

    private double _cartTotal;
    public double CartTotal
    {
        get => _cartTotal;
        private set
        {
            if (_cartTotal != value)
            {
                _cartTotal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CartTotalFormatted));
            }
        }
    }

    public string CartTotalFormatted => $"{CartTotal:F2}";

    private int _cartItemCount;
    public int CartItemCount
    {
        get => _cartItemCount;
        private set
        {
            if (_cartItemCount != value)
            {
                _cartItemCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CartHasItems));
            }
        }
    }

    // Drives the cart-tab count badge's visibility (data, not UI).
    public bool CartHasItems => CartItemCount > 0;

    private AppState()
    {
        Cart.CollectionChanged += (_, _) => RecalcCart();
        SeedOrderHistory();
    }

    // A little order history so the Orders tab has content on first run; orders placed from the
    // cart are inserted at the top of this same collection (see PlaceOrder), so the screens share
    // one source of truth.
    private void SeedOrderHistory()
    {
        Orders.Add(new OrderRecord
        {
            Id = "ORD-1042", PlacedAt = "Today, 9:14 AM", Status = "Ready for Pickup", Total = 12.25,
            Items = [ new() { Name = "Latte", Quantity = 1, Price = 5.50 }, new() { Name = "Croissant", Quantity = 2, Price = 3.25 } ]
        });
        Orders.Add(new OrderRecord
        {
            Id = "ORD-1041", PlacedAt = "Today, 8:02 AM", Status = "Preparing", Total = 10.75,
            Items = [ new() { Name = "Cappuccino", Quantity = 1, Price = 4.75 }, new() { Name = "Iced Matcha", Quantity = 1, Price = 6.00 } ]
        });
        Orders.Add(new OrderRecord
        {
            Id = "ORD-1039", PlacedAt = "Yesterday, 3:45 PM", Status = "Completed", Total = 5.50,
            Items = [ new() { Name = "Latte", Quantity = 1, Price = 5.50 } ]
        });
        Orders.Add(new OrderRecord
        {
            Id = "ORD-1037", PlacedAt = "Mar 22, 11:30 AM", Status = "Completed", Total = 15.50,
            Items = [ new() { Name = "Cappuccino", Quantity = 2, Price = 4.75 }, new() { Name = "Croissant", Quantity = 1, Price = 3.25 }, new() { Name = "Iced Matcha", Quantity = 1, Price = 6.00 } ]
        });
        Orders.Add(new OrderRecord
        {
            Id = "ORD-1033", PlacedAt = "Mar 20, 8:55 AM", Status = "Completed", Total = 9.25,
            Items = [ new() { Name = "Iced Matcha", Quantity = 1, Price = 6.00 }, new() { Name = "Croissant", Quantity = 1, Price = 3.25 } ]
        });
    }

    public void AddToCart(ProductItem product)
    {
        CartItem? existing = null;
        foreach (var item in Cart)
        {
            if (item.ProductId == product.Id) { existing = item; break; }
        }

        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            Cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.PriceValue,
                ImageUrl = product.ImageUrl,
                Quantity = 1
            });
        }

        RecalcCart();
    }

    public void IncrementItem(string productId)
    {
        foreach (var item in Cart)
        {
            if (item.ProductId == productId)
            {
                item.Quantity++;
                break;
            }
        }
        RecalcCart();
    }

    public void DecrementItem(string productId)
    {
        CartItem? target = null;
        foreach (var item in Cart)
        {
            if (item.ProductId == productId) { target = item; break; }
        }
        if (target == null) return;

        if (target.Quantity > 1)
            target.Quantity--;
        else
            Cart.Remove(target);

        RecalcCart();
    }

    public void RemoveFromCart(string productId)
    {
        CartItem? target = null;
        foreach (var item in Cart)
        {
            if (item.ProductId == productId) { target = item; break; }
        }
        if (target != null)
            Cart.Remove(target);

        RecalcCart();
    }

    public void PlaceOrder()
    {
        if (Cart.Count == 0) return;

        var items = new List<CartItem>(Cart);
        var total = CartTotal;

        var orderItems = new List<OrderLineItem>();
        foreach (var ci in items)
        {
            orderItems.Add(new OrderLineItem
            {
                Name = ci.Name,
                Quantity = ci.Quantity,
                Price = ci.Price,
                ImageUrl = ci.ImageUrl
            });
        }

        Orders.Insert(0, new OrderRecord
        {
            Id = $"ORD-{1042 + Orders.Count + 1:D4}",
            PlacedAt = DateTime.Now.ToString("MMM d, yyyy h:mm tt"),
            Status = "Confirmed",
            Total = total,
            Items = orderItems
        });

        Cart.Clear();
        RecalcCart();
    }

    private void RecalcCart()
    {
        double total = 0;
        int count = 0;
        foreach (var item in Cart)
        {
            total += item.LineTotal;
            count += item.Quantity;
        }
        CartTotal = total;
        CartItemCount = count;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
