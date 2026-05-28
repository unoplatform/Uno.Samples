using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace FieldOpsPro.Presentation.Utils
{
    public static class ColorUtils
    {
        /// <summary>
        /// Resolves a <see cref="Brush"/> from the merged application resources by key.
        /// Resolving at call time keeps code-behind colors in sync with the active theme,
        /// so controls adapt automatically once the brushes become theme-aware.
        /// </summary>
        public static Brush GetBrush(string key)
        {
            if (Application.Current?.Resources is { } resources
                && resources.TryGetValue(key, out var value)
                && value is Brush brush)
            {
                return brush;
            }

            return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }

        /// <summary>Resolves a <see cref="Windows.UI.Color"/> resource by key, falling back to transparent.</summary>
        public static Windows.UI.Color GetColor(string key)
        {
            if (Application.Current?.Resources is { } resources
                && resources.TryGetValue(key, out var value)
                && value is Windows.UI.Color color)
            {
                return color;
            }

            return Microsoft.UI.Colors.Transparent;
        }
    }
}
