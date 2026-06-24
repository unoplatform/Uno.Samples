using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BrewHouse.Presentation.MockData;

public class MenuPageData
{
    private readonly AppState _state;

    public MenuPageData(AppState state, INavigator? navigator = null)
    {
        _state = state;

        FilteredProducts = new ObservableCollection<ProductItem>(_state.AllProducts);

        AddToCartCommand = new RelayCommand<ProductItem>(product =>
        {
            if (product is not null)
                _state.AddToCart(product);
        });

        FilterByCategoryCommand = new RelayCommand<string>(categoryId =>
        {
            var id = string.IsNullOrEmpty(categoryId) ? "all" : categoryId;

            FilteredProducts.Clear();
            foreach (var p in _state.AllProducts)
            {
                if (id == "all" || p.CategoryId == id)
                    FilteredProducts.Add(p);
            }

            foreach (var cat in Categories)
                cat.IsSelected = cat.Id == id;
        });
    }

    public string PageTitle { get; } = "Our Menu";

    public IReadOnlyList<CategoryItem> Categories { get; } = AppState.CreateCategories();

    public ObservableCollection<ProductItem> FilteredProducts { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand FilterByCategoryCommand { get; }
}
