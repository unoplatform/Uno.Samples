using EnterpriseDashboard.Observatory.Services;

namespace EnterpriseDashboard.Observatory.Views;

// Thin subclass so the navigation ViewMap can map EngagementPage to a distinct VM
// type. All chart bindings live on the base class.
public sealed class EngagementViewModel : ObservatoryViewModel
{
    public EngagementViewModel(IChartDataService data) : base(data) { }
}
