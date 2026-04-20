using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation.MockData;

public class CartPageData : INotifyPropertyChanged
{
    private string _orderPlacedMessage = "";
    private Visibility _orderPlacedVisibility = Visibility.Collapsed;
    private Visibility _emptyCartVisibility = Visibility.Visible;
    private Visibility _cartContentVisibility = Visibility.Collapsed;
    private double _subtotal;
    private double _tax;
    private double _total;

    public CartPageData()
    {
        CartItems = AppState.Current.Cart;
        AppState.Current.Cart.CollectionChanged += (s, e) => RefreshTotals();

        IncrementCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                AppState.Current.IncrementItem(ci.ProductId);
            RefreshTotals();
        });

        DecrementCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                AppState.Current.DecrementItem(ci.ProductId);
            RefreshTotals();
        });

        RemoveItemCommand = new RelayCommand<object>(item =>
        {
            if (item is CartItem ci)
                AppState.Current.RemoveFromCart(ci.ProductId);
            RefreshTotals();
        });

        PlaceOrderCommand = new RelayCommand(() =>
        {
            AppState.Current.PlaceOrder();
            RefreshTotals();
            OrderPlacedMessage = "Your order has been placed! ☕";
            OrderPlacedVisibility = Visibility.Visible;
        });

        RefreshTotals();
    }

    public ObservableCollection<CartItem> CartItems { get; }

    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand PlaceOrderCommand { get; }

    public string OrderPlacedMessage
    {
        get => _orderPlacedMessage;
        set { _orderPlacedMessage = value; OnPropertyChanged(); }
    }

    public Visibility OrderPlacedVisibility
    {
        get => _orderPlacedVisibility;
        set { _orderPlacedVisibility = value; OnPropertyChanged(); }
    }

    public Visibility EmptyCartVisibility
    {
        get => _emptyCartVisibility;
        set { _emptyCartVisibility = value; OnPropertyChanged(); }
    }

    public Visibility CartContentVisibility
    {
        get => _cartContentVisibility;
        set { _cartContentVisibility = value; OnPropertyChanged(); }
    }

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

        bool hasItems = CartItems.Count > 0;
        EmptyCartVisibility = hasItems ? Visibility.Collapsed : Visibility.Visible;
        CartContentVisibility = hasItems ? Visibility.Visible : Visibility.Collapsed;
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(ItemCountText));
        OnPropertyChanged(nameof(SubtotalFormatted));
        OnPropertyChanged(nameof(TaxFormatted));
        OnPropertyChanged(nameof(TotalFormatted));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
