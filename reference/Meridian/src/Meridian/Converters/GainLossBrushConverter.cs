using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Meridian.Converters;

public sealed class GainLossBrushConverter : IValueConverter
{
    // Lazy-loaded from theme resources for consistency
    private static SolidColorBrush? _gain;
    private static SolidColorBrush? _loss;

    private static SolidColorBrush GainBrush =>
        _gain ??= (SolidColorBrush)Application.Current.Resources["MeridianGainBrush"];

    private static SolidColorBrush LossBrush =>
        _loss ??= (SolidColorBrush)Application.Current.Resources["MeridianLossBrush"];

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            bool b => b ? GainBrush : LossBrush,
            decimal d => d >= 0 ? GainBrush : LossBrush,
            double d => d >= 0 ? GainBrush : LossBrush,
            _ => GainBrush
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
