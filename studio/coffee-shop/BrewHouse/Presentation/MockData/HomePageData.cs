using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BrewHouse.Presentation.MockData;

public class HomePageData : INotifyPropertyChanged
{
    private readonly AppState _state;
    private readonly INavigator? _navigator;

    public HomePageData(AppState state, INavigator? navigator = null)
    {
        _state = state;
        _navigator = navigator;

        AddToCartCommand = new RelayCommand<ProductItem>(product =>
        {
            if (product is null) return;
            _state.AddToCart(product);
            RefreshCart();
        });

        // Cross-tab navigation (no-ops at design time, where there is no navigator).
        OrderNowCommand = new RelayCommand(() => _ = _navigator?.NavigateRouteAsync(this, "Menu"));
        GoToCartCommand = new RelayCommand(() => _ = _navigator?.NavigateRouteAsync(this, "Cart"));
        GoToMenuCommand = new RelayCommand(() => _ = _navigator?.NavigateRouteAsync(this, "Menu"));

        // Only the navigation-injected instance listens to the shared singleton; the ctor-built
        // Hot Design fallback (navigator == null) stays side-effect-free. AppState raises
        // PropertyChanged on every cart mutation (new line, quantity +/-, remove, clear), so the
        // summary strip stays current even when an existing item's quantity is bumped elsewhere.
        if (_navigator is not null)
            _state.PropertyChanged += (_, _) => RefreshCart();

        RefreshCart();
    }

    public IReadOnlyList<HeroBanner> HeroBanners { get; } =
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

    // Derived from the shared catalogue so Home stays in sync with the Menu.
    public IReadOnlyList<ProductItem> Specials => _state.AllProducts.Where(p => p.IsSpecial).ToList();
    public IReadOnlyList<ProductItem> FeaturedProducts => _state.AllProducts.Where(p => p.IsFeatured).ToList();
    public IReadOnlyList<CategoryItem> Categories { get; } = AppState.CreateCategories();

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

    private void RefreshCart()
    {
        int count = 0;
        double total = 0;
        foreach (var item in _state.Cart)
        {
            count += item.Quantity;
            total += item.LineTotal;
        }
        Cart.ItemCount = count;
        Cart.Total = total.ToString("0.00");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class CartSummaryData : INotifyPropertyChanged
{
    private int _itemCount;
    private string _total = "0.00";
    private bool _hasItems;

    public int ItemCount
    {
        get => _itemCount;
        set
        {
            _itemCount = value;
            OnPropertyChanged();
            HasItems = value > 0;
        }
    }

    public string Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); }
    }

    // Data, not UI: XAML maps this to the summary-strip vs empty-state visibility.
    public bool HasItems
    {
        get => _hasItems;
        set { _hasItems = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
