using GridWatch.Models;

namespace GridWatch.Presentation;

public sealed partial class AlertsPage : Page
{
    private readonly List<Alert> _allAlerts;

    public AlertsPage()
    {
        this.InitializeComponent();
        _allAlerts = MockData.GetAlerts();
        AlertsList.ItemsSource = _allAlerts;
        var criticalCount = _allAlerts.Count(a => a.Severity == AlertSeverity.Critical);
        CriticalCountBadge.Text = criticalCount.ToString();
    }

    private void OnFilterAll(object sender, RoutedEventArgs e)
    {
        AlertsList.ItemsSource = _allAlerts;
    }

    private void OnFilterCritical(object sender, RoutedEventArgs e)
    {
        AlertsList.ItemsSource = _allAlerts.Where(a => a.Severity == AlertSeverity.Critical).ToList();
    }

    private void OnFilterWarning(object sender, RoutedEventArgs e)
    {
        AlertsList.ItemsSource = _allAlerts.Where(a => a.Severity == AlertSeverity.Warning).ToList();
    }

    private void OnFilterInfo(object sender, RoutedEventArgs e)
    {
        AlertsList.ItemsSource = _allAlerts.Where(a => a.Severity == AlertSeverity.Info).ToList();
    }
}
