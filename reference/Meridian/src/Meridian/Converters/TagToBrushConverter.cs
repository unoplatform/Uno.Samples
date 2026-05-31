using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Meridian.Converters;

public sealed class TagToBrushConverter : IValueConverter
{
    private static SolidColorBrush? _gainTint;
    private static SolidColorBrush? _lossTint;
    private static SolidColorBrush? _accentTint;

    private static SolidColorBrush GainTint =>
        _gainTint ??= (SolidColorBrush)Application.Current.Resources["MeridianGainBgTintBrush"];

    private static SolidColorBrush LossTint =>
        _lossTint ??= (SolidColorBrush)Application.Current.Resources["MeridianLossBgTintBrush"];

    private static SolidColorBrush AccentTint =>
        _accentTint ??= (SolidColorBrush)Application.Current.Resources["MeridianAccentBgTintBrush"];

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (value as string) switch
        {
            "Earnings" => GainTint,
            "Bonds" => LossTint,
            _ => AccentTint
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
