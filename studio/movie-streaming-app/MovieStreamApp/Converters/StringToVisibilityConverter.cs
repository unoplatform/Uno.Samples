using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MovieStreamApp.Converters;

/// <summary>
/// string -> Visibility: a non-empty string is Visible, null/empty is Collapsed. Use it to collapse an
/// optional line (a subtitle, a comment) when its text is empty. Binding <c>Visibility</c> directly to a
/// string appears to work but throws on every non-empty value — the framework tries to parse the text as
/// the <c>Visibility</c> enum and fails — so route optional-text visibility through this converter.
/// ConverterParameter="invert" flips it (empty is Visible).
/// </summary>
public sealed partial class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var hasText = !string.IsNullOrEmpty(value as string);
        if (parameter as string == "invert")
        {
            hasText = !hasText;
        }
        return hasText ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
