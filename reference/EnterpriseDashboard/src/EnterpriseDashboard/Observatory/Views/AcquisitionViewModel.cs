using EnterpriseDashboard.Observatory.Services;

namespace EnterpriseDashboard.Observatory.Views;

// Thin subclass so the navigation ViewMap can map AcquisitionPage to a distinct VM
// type. All chart bindings live on the base class.
public sealed class AcquisitionViewModel : ObservatoryViewModel
{
    public AcquisitionViewModel(IChartDataService data) : base(data) { }
}
