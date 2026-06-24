using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation.MockData;

public class CartPageData : INotifyPropertyChanged
{
    private readonly AppState _state;
    private readonly INavigator? _navigator;
    private readonly DispatcherTimer? _confirmTimer;

    private string _orderPlacedMessage = "";
    private bool _showOrderConfirmed;
    private bool _isCartEmpty = true;
    private double _subtotal;
    private double _tax;
    private double _total;

    public CartPageData(AppState state, INavigator? navigator = null)
    {
        _state = state;
        _navigator = navigator;
        CartItems = _state.Cart;

        IncrementCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                _state.IncrementItem(ci.ProductId);
            RefreshTotals();
        });

        DecrementCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                _state.DecrementItem(ci.ProductId);
            RefreshTotals();
        });

        RemoveItemCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                _state.RemoveFromCart(ci.ProductId);
            RefreshTotals();
        });

        PlaceOrderCommand = new RelayCommand(() =>
        {
            _state.PlaceOrder();
            RefreshTotals();
            OrderPlacedMessage = "Your order has been placed!";
            ShowOrderConfirmed = true;
            _confirmTimer?.Start(); // auto-dismiss (transient toast)
        });

        GoToMenuCommand = new RelayCommand(() => _ = _navigator?.NavigateRouteAsync(this, "Menu"));

        // Only the navigation-injected instance wires into the shared singleton's event / a timer;
        // the ctor-built Hot Design fallback (navigator == null) stays side-effect-free.
        if (_navigator is not null)
        {
            _state.Cart.CollectionChanged += (_, _) => RefreshTotals();
            _confirmTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _confirmTimer.Tick += (_, _) => { _confirmTimer.Stop(); ShowOrderConfirmed = false; };
        }

        RefreshTotals();
    }

    public ObservableCollection<CartItem> CartItems { get; }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand PlaceOrderCommand { get; }
    public ICommand GoToMenuCommand { get; }

    public string OrderPlacedMessage
    {
        get => _orderPlacedMessage;
        set { _orderPlacedMessage = value; OnPropertyChanged(); }
    }

    // View state as data; XAML maps these bools to Visibility.
    public bool ShowOrderConfirmed
    {
        get => _showOrderConfirmed;
        set { _showOrderConfirmed = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEmptyStateVisible)); }
    }

    public bool IsCartEmpty
    {
        get => _isCartEmpty;
        set { _isCartEmpty = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsEmptyStateVisible)); }
    }

    // The empty-cart hero and the "order placed" confirmation must never show together.
    public bool IsEmptyStateVisible => IsCartEmpty && !ShowOrderConfirmed;

    public double Subtotal
    {
        get => _subtotal;
        set { _subtotal = value; OnPropertyChanged(); OnPropertyChanged(nameof(SubtotalFormatted)); }
    }

    public double Tax
    {
        get => _tax;
        set { _tax = value; OnPropertyChanged(); OnPropertyChanged(nameof(TaxFormatted)); }
    }

    public double Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(); OnPropertyChanged(nameof(TotalFormatted)); }
    }

    public string SubtotalFormatted => _subtotal.ToString("F2");
    public string TaxFormatted => _tax.ToString("F2");
    public string TotalFormatted => _total.ToString("F2");

    public int ItemCount => CartItems.Sum(i => i.Quantity);
    public string ItemCountText => ItemCount == 1 ? "1 item" : $"{ItemCount} items";

    private void RefreshTotals()
    {
        Subtotal = CartItems.Sum(i => i.LineTotal);
        Tax = Math.Round(Subtotal * 0.08, 2);
        Total = Subtotal + Tax;

        IsCartEmpty = CartItems.Count == 0;

        // Dismiss the "order placed" confirmation as soon as the cart has items again.
        if (!IsCartEmpty)
        {
            ShowOrderConfirmed = false;
            _confirmTimer?.Stop();
        }

        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(ItemCountText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
