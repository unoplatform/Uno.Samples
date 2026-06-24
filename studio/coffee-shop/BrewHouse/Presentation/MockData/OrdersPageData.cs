using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BrewHouse.Presentation.MockData;

public class OrdersPageData : INotifyPropertyChanged
{
    private readonly AppState _state;

    public OrdersPageData(AppState state, INavigator? navigator = null)
    {
        _state = state;

        // Only the navigation-injected instance listens to the shared singleton; the ctor-built
        // Hot Design fallback (navigator == null) stays side-effect-free.
        if (navigator is not null)
            _state.Orders.CollectionChanged += (_, _) => OnPropertyChanged(nameof(IsEmpty));
    }

    public ObservableCollection<OrderRecord> Orders => _state.Orders;

    // Data, not UI: XAML maps this to the empty-state visibility.
    public bool IsEmpty => Orders.Count == 0;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
