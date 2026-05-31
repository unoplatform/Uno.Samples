using EnterpriseDashboard.Observatory.Services;

namespace EnterpriseDashboard.Observatory.Views;

// Thin subclass so the navigation ViewMap can map CohortsPage to a distinct VM
// type. All chart bindings live on the base class.
public sealed class CohortsViewModel : ObservatoryViewModel
{
    public CohortsViewModel(IChartDataService data) : base(data) { }
}
