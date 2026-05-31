using Microsoft.UI.Xaml.Controls;

namespace EnterpriseDashboard.Observatory.Views;

public sealed partial class EngagementPage : Page, IObservatorySection
{
    public EngagementPage()
    {
        InitializeComponent();
    }

    // Navigation system resolves EngagementViewModel from DI and assigns DataContext.
    public void Refresh()
    {
        if (DataContext is ObservatoryViewModel vm) vm.ResetKey++;
    }
}
