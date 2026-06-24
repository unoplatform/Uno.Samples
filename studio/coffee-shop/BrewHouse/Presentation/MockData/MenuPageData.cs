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
            ApplyFilter(id);

            foreach (var cat in Categories)
                cat.IsSelected = cat.Id == id;
        });
    }

    public string PageTitle { get; } = "Our Menu";

    public IReadOnlyList<CategoryItem> Categories { get; } = AppState.CreateCategories();

    public ObservableCollection<ProductItem> FilteredProducts { get; }

    public ICommand AddToCartCommand { get; }
    public ICommand FilterByCategoryCommand { get; }

    // Sync the bound collection in place — remove dropped items, insert/move the rest to match the
    // filtered order — instead of Clear()+Add(). Items that stay keep their containers and loaded
    // images, so switching filters doesn't tear the whole list down and rebuild it (no flicker).
    private void ApplyFilter(string id)
    {
        var target = _state.AllProducts
            .Where(p => id == "all" || p.CategoryId == id)
            .ToList();

        for (int i = FilteredProducts.Count - 1; i >= 0; i--)
        {
            if (!target.Contains(FilteredProducts[i]))
                FilteredProducts.RemoveAt(i);
        }

        for (int i = 0; i < target.Count; i++)
        {
            var item = target[i];
            int current = FilteredProducts.IndexOf(item);
            if (current < 0)
                FilteredProducts.Insert(i, item);
            else if (current != i)
                FilteredProducts.Move(current, i);
        }
    }
}
