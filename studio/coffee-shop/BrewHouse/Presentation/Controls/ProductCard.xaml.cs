using System.Windows.Input;
using BrewHouse.Presentation.MockData;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BrewHouse.Presentation.Controls;

public sealed partial class ProductCard : UserControl
{
    public ProductCard()
    {
        this.InitializeComponent();
    }

    public static readonly DependencyProperty ProductProperty =
        DependencyProperty.Register(nameof(Product), typeof(ProductItem), typeof(ProductCard), new PropertyMetadata(null));

    public ProductItem? Product
    {
        get => (ProductItem?)GetValue(ProductProperty);
        set => SetValue(ProductProperty, value);
    }

    public static readonly DependencyProperty AddCommandProperty =
        DependencyProperty.Register(nameof(AddCommand), typeof(ICommand), typeof(ProductCard), new PropertyMetadata(null));

    public ICommand? AddCommand
    {
        get => (ICommand?)GetValue(AddCommandProperty);
        set => SetValue(AddCommandProperty, value);
    }

    public static readonly DependencyProperty ShowSpecialBadgeProperty =
        DependencyProperty.Register(nameof(ShowSpecialBadge), typeof(bool), typeof(ProductCard), new PropertyMetadata(true));

    public bool ShowSpecialBadge
    {
        get => (bool)GetValue(ShowSpecialBadgeProperty);
        set => SetValue(ShowSpecialBadgeProperty, value);
    }

    // x:Bind helpers — instance methods so the generated binding can call them
    // (re-evaluate via Mode=OneWay when the Product / flag changes).
    public string Money(string? price) => string.IsNullOrEmpty(price) ? string.Empty : "$" + price;

    public Visibility BadgeVisibility(ProductItem? product, bool show)
        => show && product?.IsSpecial == true ? Visibility.Visible : Visibility.Collapsed;
}
