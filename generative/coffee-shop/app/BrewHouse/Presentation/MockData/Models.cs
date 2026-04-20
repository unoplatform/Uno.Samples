using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;

namespace BrewHouse.Presentation.MockData;

public class HeroBanner
{
    public string ImageUrl { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
}

public class ProductItem
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string CategoryId { get; set; } = "";
    public string Price { get; set; } = "";
    public double PriceValue { get; set; }
    public string ImageUrl { get; set; } = "";
    public bool IsFeatured { get; set; }
    public bool IsSpecial { get; set; }
}

public class CategoryItem : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _chipBackground = "#FFF5EFE6";
    private string _chipForeground = "#FF2C1A0E";

    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Icon { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set { _isSelected = value; OnPropertyChanged(); }
    }

    public string ChipBackground
    {
        get => _chipBackground;
        set { _chipBackground = value; OnPropertyChanged(); }
    }

    public string ChipForeground
    {
        get => _chipForeground;
        set { _chipForeground = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class CartItem : INotifyPropertyChanged
{
    private int _quantity;

    public string ProductId { get; set; } = "";
    public string Name { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public double Price { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LineTotal));
                OnPropertyChanged(nameof(LineTotalFormatted));
            }
        }
    }

    public double LineTotal => Price * Quantity;
    public string LineTotalFormatted => LineTotal.ToString("F2");
    public string PriceFormatted => Price.ToString("F2");

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class OrderLineItem
{
    public string Name { get; set; } = "";
    public int Quantity { get; set; } = 1;
    public double Price { get; set; }
    public string ImageUrl { get; set; } = "";
}

public class OrderRecord
{
    public string Id { get; set; } = "";
    public string PlacedAt { get; set; } = "";
    public string Status { get; set; } = "";
    public string StatusColor { get; set; } = "#FF8C5A28";
    public string StatusBackground { get; set; } = "#FFF5EFE6";
    public double Total { get; set; }
    public string TotalFormatted => Total.ToString("F2");
    public List<OrderLineItem> Items { get; set; } = new();

    public string ItemSummary
    {
        get
        {
            if (Items == null || Items.Count == 0) return "No items";
            var names = new List<string>();
            foreach (var item in Items)
                names.Add($"{item.Name} x{item.Quantity}");
            return string.Join(", ", names);
        }
    }
}
