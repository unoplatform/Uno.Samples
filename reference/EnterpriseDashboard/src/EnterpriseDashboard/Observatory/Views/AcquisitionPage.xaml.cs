using Microsoft.UI.Xaml.Controls;

namespace EnterpriseDashboard.Observatory.Views;

public sealed partial class AcquisitionPage : Page, IObservatorySection
{
    public AcquisitionPage()
    {
        InitializeComponent();
    }

    // Navigation system resolves AcquisitionViewModel from DI and assigns DataContext.
    public void Refresh()
    {
        if (DataContext is ObservatoryViewModel vm) vm.ResetKey++;
    }
}
