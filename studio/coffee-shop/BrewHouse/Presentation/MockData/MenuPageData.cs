using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrewHouse.Presentation.MockData;

public class MenuPageData
{
    public string PageTitle { get; set; } = "Our Menu";

    public ObservableCollection<CategoryItem> Categories { get; set; } =
    [
        new() { Id = "all",      Name = "All",        Icon = "☕", IsSelected = true,  ChipBackground = "#FF4A2810", ChipForeground = "#FFFDF6EE" },
        new() { Id = "hot",      Name = "Hot Drinks",  Icon = "🔥", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
        new() { Id = "cold",     Name = "Cold Drinks", Icon = "🧊", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
        new() { Id = "pastries", Name = "Pastries",    Icon = "🥐", IsSelected = false, ChipBackground = "#FFF5EFE6", ChipForeground = "#FF4A2810" },
    ];

    public string SelectedCategoryId { get; set; } = "all";

    public ObservableCollection<ProductItem> AllProducts { get; set; } =
    [
        new()
        {
            Id          = "p-001",
            Name        = "Classic Latte",
            Description = "Smooth espresso with velvety steamed milk and a delicate foam layer.",
            Category    = "Hot Drinks",
            CategoryId  = "hot",
            Price       = "5.50",
            PriceValue  = 5.50,
            ImageUrl    = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = true,
            IsSpecial   = true,
        },
        new()
        {
            Id          = "p-002",
            Name        = "Cappuccino",
            Description = "Equal parts espresso, steamed milk and thick microfoam — a true Italian classic.",
            Category    = "Hot Drinks",
            CategoryId  = "hot",
            Price       = "4.75",
            PriceValue  = 4.75,
            ImageUrl    = "https://images.pexels.com/photos/1694874/pexels-photo-1694874.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = true,
            IsSpecial   = false,
        },
        new()
        {
            Id          = "p-003",
            Name        = "Flat White",
            Description = "Ristretto shots with a thin layer of velvety micro-foam — bold and smooth.",
            Category    = "Hot Drinks",
            CategoryId  = "hot",
            Price       = "4.50",
            PriceValue  = 4.50,
            ImageUrl    = "https://images.pexels.com/photos/131053/pexels-photo-131053.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = false,
            IsSpecial   = false,
        },
        new()
        {
            Id          = "p-004",
            Name        = "Caramel Macchiato",
            Description = "Layers of vanilla syrup, steamed milk, espresso and drizzled caramel.",
            Category    = "Hot Drinks",
            CategoryId  = "hot",
            Price       = "5.75",
            PriceValue  = 5.75,
            ImageUrl    = "https://images.pexels.com/photos/3646111/pexels-photo-3646111.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = false,
            IsSpecial   = true,
        },
        new()
        {
            Id          = "p-005",
            Name        = "Iced Matcha",
            Description = "Premium ceremonial matcha blended with oat milk over crushed ice.",
            Category    = "Cold Drinks",
            CategoryId  = "cold",
            Price       = "6.00",
            PriceValue  = 6.00,
            ImageUrl    = "https://images.pexels.com/photos/18553404/pexels-photo-18553404.png?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = true,
            IsSpecial   = false,
        },
        new()
        {
            Id          = "p-006",
            Name        = "Cold Brew",
            Description = "Steeped for 18 hours in cold water for a naturally sweet, low-acid brew.",
            Category    = "Cold Drinks",
            CategoryId  = "cold",
            Price       = "5.00",
            PriceValue  = 5.00,
            ImageUrl    = "https://images.pexels.com/photos/261434/pexels-photo-261434.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = false,
            IsSpecial   = false,
        },
        new()
        {
            Id          = "p-007",
            Name        = "Butter Croissant",
            Description = "Flaky, golden-baked croissant with premium French butter. Freshly baked each morning.",
            Category    = "Pastries",
            CategoryId  = "pastries",
            Price       = "3.25",
            PriceValue  = 3.25,
            ImageUrl    = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = true,
            IsSpecial   = true,
        },
        new()
        {
            Id          = "p-008",
            Name        = "Almond Muffin",
            Description = "Moist almond muffin with a crunchy streusel topping and hint of vanilla.",
            Category    = "Pastries",
            CategoryId  = "pastries",
            Price       = "2.95",
            PriceValue  = 2.95,
            ImageUrl    = "https://images.pexels.com/photos/20212456/pexels-photo-20212456.jpeg?auto=compress&cs=tinysrgb&dpr=2&h=650&w=940",
            IsFeatured  = false,
            IsSpecial   = false,
        },
    ];

    public ObservableCollection<ProductItem> FilteredProducts { get; set; }

    public ICommand AddToCartCommand { get; }
    public ICommand FilterByCategoryCommand { get; }

    public MenuPageData()
    {
        FilteredProducts = new ObservableCollection<ProductItem>(AllProducts);

        AddToCartCommand = new RelayCommand<ProductItem>(product =>
        {
            if (product is not null)
                AppState.Current.AddToCart(product);
        });

        FilterByCategoryCommand = new RelayCommand<string>(categoryId =>
        {
            var id = string.IsNullOrEmpty(categoryId) ? "all" : categoryId;

            FilteredProducts.Clear();
            foreach (var p in AllProducts)
            {
                if (id == "all" || p.CategoryId == id)
                    FilteredProducts.Add(p);
            }

            SelectedCategoryId = id;

            foreach (var cat in Categories)
            {
                cat.IsSelected = cat.Id == id;
                cat.ChipBackground = cat.IsSelected ? "#FF4A2810" : "#FFF5EFE6";
                cat.ChipForeground = cat.IsSelected ? "#FFFDF6EE" : "#FF4A2810";
            }
        });
    }
}
