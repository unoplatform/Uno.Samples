using GridWatch.Models;

namespace GridWatch.Presentation;

public sealed partial class DashboardPage : Page
{
    public DashboardPage()
    {
        this.InitializeComponent();
        this.Loaded += DashboardPage_Loaded;
    }

    private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        var vm = new DashboardViewModel
        {
            KpiMetrics = new List<KpiMetric>
            {
                new KpiMetric { Id = "demand",    Label = "TOTAL DEMAND",   Value = "48,320", Unit = "MW", Delta = "+1.2%",  DeltaDirection = DeltaDirection.Up      },
                new KpiMetric { Id = "supply",    Label = "TOTAL SUPPLY",   Value = "51,140", Unit = "MW", Delta = "+0.8%",  DeltaDirection = DeltaDirection.Up      },
                new KpiMetric { Id = "reserve",   Label = "RESERVE MARGIN", Value = "5.83",   Unit = "%",  Delta = "-0.4%",  DeltaDirection = DeltaDirection.Down    },
                new KpiMetric { Id = "frequency", Label = "GRID FREQUENCY", Value = "59.98",  Unit = "Hz", Delta = "-0.02",  DeltaDirection = DeltaDirection.Down    },
            },
            Facilities = MockData.GetFacilities(),
            Alerts = MockData.GetAlerts(),
        };

        this.DataContext = vm;

        KpiItemsControl.ItemsSource        = vm.KpiMetrics;
        FacilitiesItemsControl.ItemsSource = vm.Facilities;
        AlertsItemsControl.ItemsSource     = vm.Alerts;

        FacilityCountText.Text = vm.Facilities.Count.ToString();
        AlertCountText.Text    = vm.Alerts.Count(a => a.Severity == AlertSeverity.Critical).ToString();
    }
}
