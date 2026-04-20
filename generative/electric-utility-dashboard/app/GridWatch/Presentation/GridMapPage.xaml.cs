using GridWatch.Models;
using System.Collections.ObjectModel;

namespace GridWatch.Presentation;

public sealed partial class GridMapPage : Page
{
    public ObservableCollection<FacilityRow> Facilities { get; } = new();

    private string _sortColumn = "Name";
    private bool _sortAscending = true;

    private static readonly Dictionary<string, string> _columnHeaders = new()
    {
        ["Name"]     = "FACILITY",
        ["Region"]   = "REGION",
        ["Type"]     = "TYPE",
        ["Capacity"] = "CAPACITY",
        ["Output"]   = "OUTPUT",
        ["Status"]   = "STATUS",
    };

    public GridMapPage()
    {
        this.InitializeComponent();
        this.DataContext = this;
        LoadMockData();
        UpdateStatusCounts();
        ApplySort();
    }

    private void LoadMockData()
    {
        var data = new[]
        {
            new FacilityRow { Id = "F01", Name = "Hoover Dam",       Region = "Southwest", Type = "Hydro",   Capacity = "2080", Output = "1843", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F02", Name = "Grand Coulee",     Region = "Northwest", Type = "Hydro",   Capacity = "6809", Output = "6201", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F03", Name = "Palo Verde",       Region = "Southwest", Type = "Nuclear", Capacity = "3942", Output = "3610", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F04", Name = "Robert Moses",     Region = "Northeast", Type = "Hydro",   Capacity = "2429", Output = "2115", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F05", Name = "Moss Landing",     Region = "West",      Type = "Battery", Capacity = "1060", Output = "820",  Status = FacilityStatus.Warning  },
            new FacilityRow { Id = "F06", Name = "Navajo Station",   Region = "Southwest", Type = "Coal",    Capacity = "2250", Output = "0",    Status = FacilityStatus.Critical },
            new FacilityRow { Id = "F07", Name = "Browns Ferry",     Region = "Southeast", Type = "Nuclear", Capacity = "3456", Output = "3100", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F08", Name = "Diablo Canyon",    Region = "West",      Type = "Nuclear", Capacity = "2256", Output = "1980", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F09", Name = "Comanche Peak",    Region = "South",     Type = "Nuclear", Capacity = "2430", Output = "2200", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F10", Name = "Prairie Island",   Region = "Midwest",   Type = "Nuclear", Capacity = "1100", Output = "870",  Status = FacilityStatus.Warning  },
            new FacilityRow { Id = "F11", Name = "Lake Keowee",      Region = "Southeast", Type = "Hydro",   Capacity = "760",  Output = "710",  Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F12", Name = "Calvert Cliffs",   Region = "Northeast", Type = "Nuclear", Capacity = "1825", Output = "0",    Status = FacilityStatus.Critical },
            new FacilityRow { Id = "F13", Name = "Mojave Solar",     Region = "Southwest", Type = "Solar",   Capacity = "1600", Output = "1420", Status = FacilityStatus.Online   },
            new FacilityRow { Id = "F14", Name = "Pecos Wind Farm",  Region = "South",     Type = "Wind",    Capacity = "900",  Output = "610",  Status = FacilityStatus.Warning  },
            new FacilityRow { Id = "F15", Name = "Beacon Hill Gas",  Region = "Northeast", Type = "Gas",     Capacity = "800",  Output = "760",  Status = FacilityStatus.Online   },
        };

        foreach (var row in data)
            Facilities.Add(row);
    }

    private void UpdateStatusCounts()
    {
        int online   = Facilities.Count(f => f.Status == FacilityStatus.Online);
        int warning  = Facilities.Count(f => f.Status == FacilityStatus.Warning);
        int critical = Facilities.Count(f => f.Status == FacilityStatus.Critical);
        int total    = Facilities.Count;

        OnlineCountText.Text   = $"{online} Online";
        WarningCountText.Text  = $"{warning} Warning";
        CriticalCountText.Text = $"{critical} Critical";
        TotalCountText.Text    = $"{total} facilities total";
    }

    private void ApplySort()
    {
        IEnumerable<FacilityRow> sorted = _sortColumn switch
        {
            "Region"   => _sortAscending ? Facilities.OrderBy(f => f.Region)   : Facilities.OrderByDescending(f => f.Region),
            "Type"     => _sortAscending ? Facilities.OrderBy(f => f.Type)     : Facilities.OrderByDescending(f => f.Type),
            "Capacity" => _sortAscending ? Facilities.OrderBy(f => int.TryParse(f.Capacity, out var c) ? c : 0) : Facilities.OrderByDescending(f => int.TryParse(f.Capacity, out var c) ? c : 0),
            "Output"   => _sortAscending ? Facilities.OrderBy(f => int.TryParse(f.Output, out var o) ? o : 0)   : Facilities.OrderByDescending(f => int.TryParse(f.Output, out var o) ? o : 0),
            "Status"   => _sortAscending ? Facilities.OrderBy(f => f.Status)   : Facilities.OrderByDescending(f => f.Status),
            _          => _sortAscending ? Facilities.OrderBy(f => f.Name)     : Facilities.OrderByDescending(f => f.Name),
        };

        FacilityList.ItemsSource = sorted.ToList();
        UpdateColumnHeaders();
    }

    private void SortByColumn(string column)
    {
        if (_sortColumn == column)
            _sortAscending = !_sortAscending;
        else
        {
            _sortColumn = column;
            _sortAscending = true;
        }

        ApplySort();
    }

    private void UpdateColumnHeaders()
    {
        string arrow = _sortAscending ? " ▲" : " ▼";

        BtnSortName.Content     = "FACILITY"  + (_sortColumn == "Name"     ? arrow : "");
        BtnSortRegion.Content   = "REGION"    + (_sortColumn == "Region"   ? arrow : "");
        BtnSortType.Content     = "TYPE"      + (_sortColumn == "Type"     ? arrow : "");
        BtnSortCapacity.Content = "CAPACITY"  + (_sortColumn == "Capacity" ? arrow : "");
        BtnSortOutput.Content   = "OUTPUT"    + (_sortColumn == "Output"   ? arrow : "");
        BtnSortStatus.Content   = "STATUS"    + (_sortColumn == "Status"   ? arrow : "");
    }

    private void SortName_Click(object sender, RoutedEventArgs e)     => SortByColumn("Name");
    private void SortRegion_Click(object sender, RoutedEventArgs e)   => SortByColumn("Region");
    private void SortType_Click(object sender, RoutedEventArgs e)     => SortByColumn("Type");
    private void SortCapacity_Click(object sender, RoutedEventArgs e) => SortByColumn("Capacity");
    private void SortOutput_Click(object sender, RoutedEventArgs e)   => SortByColumn("Output");
    private void SortStatus_Click(object sender, RoutedEventArgs e)   => SortByColumn("Status");
}
