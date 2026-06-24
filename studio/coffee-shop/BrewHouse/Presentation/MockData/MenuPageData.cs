using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BrewHouse.Presentation.MockData;

public class MenuPageData : INotifyPropertyChanged
{
    private readonly AppState _state;

    private string _categoryId = "all";
    private string _searchText = "";

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
            _categoryId = string.IsNullOrEmpty(categoryId) ? "all" : categoryId;
            ApplyFilter();

            foreach (var cat in Categories)
                cat.IsSelected = cat.Id == _categoryId;
        });
    }

    public string PageTitle { get; } = "Our Menu";

    public IReadOnlyList<CategoryItem> Categories { get; } = AppState.CreateCategories();

    public ObservableCollection<ProductItem> FilteredProducts { get; }

    // Text search, combined with the category filter; re-filters live as the user types.
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value ?? "";
                OnPropertyChanged();
                ApplyFilter();
            }
        }
    }

    public ICommand AddToCartCommand { get; }
    public ICommand FilterByCategoryCommand { get; }

    // Sync the bound collection in place — remove dropped items, insert/move the rest to match the
    // filtered order — instead of Clear()+Add(). Items that stay keep their containers and loaded
    // images, so switching filters/search doesn't tear the whole list down and rebuild it (no flicker).
    private void ApplyFilter()
    {
        var search = _searchText.Trim();
        var target = _state.AllProducts
            .Where(p => (_categoryId == "all" || p.CategoryId == _categoryId)
                        && (search.Length == 0
                            || p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                            || p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
