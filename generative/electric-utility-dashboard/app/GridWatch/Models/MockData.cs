namespace GridWatch.Models;

public static class MockData
{
    public static List<Alert> GetAlerts()
    {
        var now = DateTimeOffset.Now;
        return new List<Alert>
        {
            new Alert
            {
                Id = "a1",
                Severity = AlertSeverity.Critical,
                Message = "Four Corners Plant: Unit 3 tripped offline — automatic shutdown initiated.",
                FacilityName = "Four Corners Plant",
                Timestamp = now.AddMinutes(-4)
            },
            new Alert
            {
                Id = "a2",
                Severity = AlertSeverity.Critical,
                Message = "Navajo Generating Station: All units offline — emergency grid import required.",
                FacilityName = "Navajo Generating",
                Timestamp = now.AddMinutes(-11)
            },
            new Alert
            {
                Id = "a3",
                Severity = AlertSeverity.Critical,
                Message = "Calvert Cliffs: Reactor coolant pressure anomaly — unit placed in safe shutdown.",
                FacilityName = "Calvert Cliffs",
                Timestamp = now.AddMinutes(-19)
            },
            new Alert
            {
                Id = "a4",
                Severity = AlertSeverity.Warning,
                Message = "Pecos Wind Farm: Output below forecast by 32% — sustained wind speed dropping.",
                FacilityName = "Pecos Wind Farm",
                Timestamp = now.AddMinutes(-25)
            },
            new Alert
            {
                Id = "a5",
                Severity = AlertSeverity.Warning,
                Message = "Reserve margin approaching 5% threshold — standby peakers placed on notice.",
                FacilityName = "System",
                Timestamp = now.AddMinutes(-33)
            },
            new Alert
            {
                Id = "a6",
                Severity = AlertSeverity.Warning,
                Message = "Moss Landing: Battery storage SOC below 20% — curtailing exports.",
                FacilityName = "Moss Landing",
                Timestamp = now.AddMinutes(-48)
            },
            new Alert
            {
                Id = "a7",
                Severity = AlertSeverity.Warning,
                Message = "Prairie Island: Unit 2 scheduled maintenance overrun by 6 hours.",
                FacilityName = "Prairie Island",
                Timestamp = now.AddHours(-1).AddMinutes(-10)
            },
            new Alert
            {
                Id = "a8",
                Severity = AlertSeverity.Info,
                Message = "Mojave Solar: Daily generation target reached 18 minutes ahead of schedule.",
                FacilityName = "Mojave Solar",
                Timestamp = now.AddHours(-1).AddMinutes(-42)
            },
            new Alert
            {
                Id = "a9",
                Severity = AlertSeverity.Info,
                Message = "Diablo Canyon: Routine coolant system inspection completed — no anomalies detected.",
                FacilityName = "Diablo Canyon",
                Timestamp = now.AddHours(-2).AddMinutes(-5)
            },
            new Alert
            {
                Id = "a10",
                Severity = AlertSeverity.Info,
                Message = "Grand Coulee: Turbine 7 maintenance window completed — unit returned to service.",
                FacilityName = "Grand Coulee",
                Timestamp = now.AddHours(-3).AddMinutes(-20)
            }
        };
    }

    public static List<Facility> GetFacilities()
    {
        return new List<Facility>
        {
            new Facility { Id = "f1",  Name = "Hoover Dam",         Type = "Hydro",   Region = "Southwest", Capacity = "2,080", Output = "1,950", Status = FacilityStatus.Online   },
            new Facility { Id = "f2",  Name = "Diablo Canyon",      Type = "Nuclear", Region = "West",      Capacity = "2,256", Output = "2,200", Status = FacilityStatus.Online   },
            new Facility { Id = "f3",  Name = "Mojave Solar",       Type = "Solar",   Region = "Southwest", Capacity = "1,600", Output = "1,420", Status = FacilityStatus.Online   },
            new Facility { Id = "f4",  Name = "Pecos Wind Farm",    Type = "Wind",    Region = "South",     Capacity = "900",   Output = "610",   Status = FacilityStatus.Warning  },
            new Facility { Id = "f5",  Name = "Four Corners Plant", Type = "Coal",    Region = "Southwest", Capacity = "2,040", Output = "1,200", Status = FacilityStatus.Critical },
            new Facility { Id = "f6",  Name = "Beacon Hill Gas",    Type = "Gas",     Region = "Northeast", Capacity = "800",   Output = "760",   Status = FacilityStatus.Online   },
            new Facility { Id = "f7",  Name = "Altamont Wind",      Type = "Wind",    Region = "West",      Capacity = "580",   Output = "490",   Status = FacilityStatus.Online   },
            new Facility { Id = "f8",  Name = "Navajo Generating",  Type = "Coal",    Region = "Southwest", Capacity = "2,250", Output = "0",     Status = FacilityStatus.Critical },
        };
    }
}
