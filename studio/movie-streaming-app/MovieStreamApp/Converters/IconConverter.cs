using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace MovieStreamApp.Converters;

/// <summary>
/// Resolves an icon key (or a domain value) to a <see cref="Geometry"/> for a PathIcon/Path, so
/// the whole app draws vector icons instead of FontIcon glyphs. Usage:
///   Data="{Binding Source=play, Converter={StaticResource Icon}}"                 (static icon key)
///   Data="{Binding Source=browse, Converter={StaticResource Icon}, ConverterParameter=filled}"
///   Data="{Binding Name,  Converter={StaticResource Icon}, ConverterParameter=genre}"    (genre value)
///   Data="{Binding Label, Converter={StaticResource Icon}, ConverterParameter=setting}"  (settings label)
/// The value/Source carries only a domain string; the glyph choice lives here, never in the Model.
/// </summary>
public sealed partial class IconConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        var mode = parameter as string;

        // Star-rating rows bind a bool per position: true -> filled star, false -> outline star.
        if (mode == "star")
        {
            var on = value is bool b && b;
            var starData = on ? Icons.GetFilled("star") : Icons.Get("star");
            return (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), starData);
        }

        var input = value as string ?? string.Empty;

        var data = mode switch
        {
            "genre" => Icons.Get(Icons.ForGenre(input)),
            "setting" => Icons.Get(Icons.ForSetting(input)),
            "filled" => Icons.GetFilled(input),
            _ => Icons.Get(input),
        };

        return string.IsNullOrEmpty(data)
            ? null
            : (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), data);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
