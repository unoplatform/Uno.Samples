using Microsoft.UI.Xaml.Data;

namespace MovieStreamApp.Converters;

/// <summary>
/// Resolves a movie title string to the full <see cref="Movie"/> from the shared catalogue, so a card
/// that only carries a title (e.g. a friend's activity) can pass a real entity as Navigation.Data and
/// open the detail. Never returns null (falls back to the featured movie).
/// </summary>
public sealed partial class TitleToMovieConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        MovieData.ByTitle(value as string ?? string.Empty);

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotSupportedException();
}
