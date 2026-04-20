using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation.MockData;

public class HomePageData : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public List<HeroBanner> HeroBanners { get; set; } =
    [
        new()
        {
            ImageUrl = "https://images.pexels.com/photos/1002740/pexels-photo-1002740.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            Title = "Start Your Morning Right",
            Subtitle = "Freshly brewed specialties crafted with love, every single day."
        },
        new()
        {
            ImageUrl = "https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            Title = "Artisan Cappuccinos",
            Subtitle = "Perfectly balanced espresso with velvety micro-foam."
        },
        new()
        {
            ImageUrl = "https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            Title = "Single Origin Beans",
            Subtitle = "Ethically sourced from the world's finest coffee farms."
        }
    ];

    public List<ProductItem> Specials { get; set; } =
    [
        new()
        {
            Id = "p-001",
            Name = "Caramel Latte",
            Description = "Espresso with steamed milk and rich caramel syrup",
            Category = "Hot Drinks",
            CategoryId = "hot",
            Price = "5.50",
            PriceValue = 5.50,
            ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        },
        new()
        {
            Id = "p-003",
            Name = "Butter Croissant",
            Description = "Buttery, flaky pastry baked fresh every morning",
            Category = "Pastries",
            CategoryId = "pastries",
            Price = "3.25",
            PriceValue = 3.25,
            ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        }
    ];

    public List<CategoryItem> Categories { get; set; } =
    [
        new() { Id = "all",      Name = "All",        Icon = "☕", IsSelected = true,  ChipBackground = "#FF4A2810", ChipForeground = "#FFFDF6EE" },
        new() { Id = "hot",      Name = "Hot Drinks",  Icon = "🔥", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
        new() { Id = "cold",     Name = "Cold Drinks", Icon = "🧊", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
        new() { Id = "pastries", Name = "Pastries",    Icon = "🥐", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
    ];

    public List<ProductItem> FeaturedProducts { get; set; } =
    [
        new()
        {
            Id = "p-001",
            Name = "Caramel Latte",
            Description = "Espresso with steamed milk and rich caramel syrup, topped with whipped cream",
            Category = "Hot Drinks",
            CategoryId = "hot",
            Price = "5.50",
            PriceValue = 5.50,
            ImageUrl = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        },
        new()
        {
            Id = "p-002",
            Name = "Cappuccino",
            Description = "Classic Italian espresso with equal parts steamed milk and thick velvety foam",
            Category = "Hot Drinks",
            CategoryId = "hot",
            Price = "4.75",
            PriceValue = 4.75,
            ImageUrl = "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = false
        },
        new()
        {
            Id = "p-003",
            Name = "Butter Croissant",
            Description = "Buttery, flaky pastry baked fresh every morning — best enjoyed warm",
            Category = "Pastries",
            CategoryId = "pastries",
            Price = "3.25",
            PriceValue = 3.25,
            ImageUrl = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = true
        },
        new()
        {
            Id = "p-004",
            Name = "Iced Matcha",
            Description = "Premium ceremonial-grade matcha with oat milk over ice, lightly sweetened",
            Category = "Cold Drinks",
            CategoryId = "cold",
            Price = "6.00",
            PriceValue = 6.00,
            ImageUrl = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured = true,
            IsSpecial = false
        }
    ];

    private CartSummaryData _cart = new();
    public CartSummaryData Cart
    {
        get => _cart;
        set { _cart = value; OnPropertyChanged(); }
    }

    public ICommand AddToCartCommand { get; }
    public ICommand OrderNowCommand { get; }
    public ICommand GoToCartCommand { get; }
    public ICommand GoToMenuCommand { get; }

    public HomePageData()
    {
        AddToCartCommand = new RelayCommand<ProductItem>(product =>
        {
            if (product is null) return;
            AppState.Current.AddToCart(product);
            RefreshCart();
        });

        OrderNowCommand = new RelayCommand(() => { });
        GoToCartCommand = new RelayCommand(() => { });
        GoToMenuCommand = new RelayCommand(() => { });

        AppState.Current.Cart.CollectionChanged += (_, _) => RefreshCart();
        RefreshCart();
    }

    private void RefreshCart()
    {
        int count = 0;
        double total = 0;
        foreach (var item in AppState.Current.Cart)
        {
            count += item.Quantity;
            total += item.LineTotal;
        }
        Cart.ItemCount = count;
        Cart.Total = total.ToString("0.00");
    }
}

public class CartSummaryData : INotifyPropertyChanged
{
    private int _itemCount;
    private string _total = "0.00";
    private Visibility _hasItems = Visibility.Collapsed;
    private Visibility _isEmpty = Visibility.Visible;

    public int ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            OnPropertyChanged();
            HasItems = value > 0 ? Visibility.Visible : Visibility.Collapsed;
            IsEmpty = value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    public string Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); }
    }

    public Visibility HasItems
    {
        get => _hasItems;
        set { _hasItems = value; OnPropertyChanged(); }
    }

    public Visibility IsEmpty
    {
        get => _isEmpty;
        set { _isEmpty = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
