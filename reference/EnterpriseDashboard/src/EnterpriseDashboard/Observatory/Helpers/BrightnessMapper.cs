using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace EnterpriseDashboard.Observatory.Helpers;

// Brightness-as-magnitude per Design Brief §1.2.
public static class BrightnessMapper
{
    public static SolidColorBrush FromValue(double value)
    {
        var key = value > 80 ? "ObsWhiteBrush"
                : value > 60 ? "ObsLightBrush"
                : value > 40 ? "ObsMidBrush"
                : "ObsGreyBrush";
        return (SolidColorBrush)Application.Current.Resources[key];
    }
}
