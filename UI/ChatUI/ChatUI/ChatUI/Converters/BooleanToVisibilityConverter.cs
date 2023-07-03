using System;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace ChatUI.Converters
{
    /// <summary>
    /// Copied/adapted from https://github.com/unoplatform/uno/blob/master/src/Uno.UI.Tests/Converters/FromNullableBoolToVisibilityConverter.cs
    /// This converter will output a visibility based on if a nullable bool is set to true or otherwise.
    /// 
    /// VisibilityIfTrue (VisibilityIfTrue) : Determines the visibility value that will be returned if the value is true.
    /// 
    /// By default, VisibilityIfTrue is set to visible.
    /// 
    /// This converter may be used to display or hide some content based on a nullable boolean value.
    /// </summary>
    public class FromNullableBoolToVisibilityConverter : IValueConverter
    {
        public FromNullableBoolToVisibilityConverter()
        {
            VisibilityIfTrue = VisibilityIfTrue.Visible;
        }

        public VisibilityIfTrue VisibilityIfTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null)
            {
                throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
            }

            bool inverse = VisibilityIfTrue == VisibilityIfTrue.Collapsed;

            Visibility visibilityOnTrue = !inverse ? Visibility.Visible : Visibility.Collapsed;
            Visibility visibilityOnFalse = !inverse ? Visibility.Collapsed : Visibility.Visible;

            if (value != null && !(value is bool))
            {
                throw new ArgumentException($"Value must either be null or of type bool. Got {value} ({value.GetType().FullName})");
            }

            var valueToConvert = value != null && System.Convert.ToBoolean(value, CultureInfo.InvariantCulture);

            return valueToConvert ? visibilityOnTrue : visibilityOnFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            if (parameter != null)
            {
                throw new ArgumentException($"This converter does not use any parameters. You should remove \"{parameter}\" passed as parameter.");
            }

            bool inverse = VisibilityIfTrue == VisibilityIfTrue.Collapsed;

            Visibility visibilityOnTrue = !inverse ? Visibility.Visible : Visibility.Collapsed;

            var visibility = (Visibility)value;

            return visibilityOnTrue.Equals(visibility);
        }
    }

    public enum VisibilityIfTrue
    {
        Visible,
        Collapsed
    }
}
