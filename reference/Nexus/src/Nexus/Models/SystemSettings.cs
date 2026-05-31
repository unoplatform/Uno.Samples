namespace Nexus.Models;

public partial record SystemSettings(
    bool AutoBackup,
    bool EmailAlerts,
    bool SoundAlerts,
    string DataRetention,
    string TemperatureUnit,
    int RefreshRate,
    double TemperatureThresholdHigh,
    double TemperatureThresholdLow,
    double PressureThresholdHigh,
    double PressureThresholdLow
);
