using System;
using Microsoft.UI.Xaml.Data;

namespace TimeEntryUno.Shared.Helpers
{
    public class NotOperatorValueConverter : IValueConverter
    {
        ///// <summary>
        ///// Converts the given <paramref name="value"/> to be its inverse
        ///// </summary>
        ///// <param name="value">The <c>bool</c> value to convert.</param>
        ///// <param name="targetType">The type to convert to (ignored).</param>
        ///// <param name="parameter">Optional parameter (ignored).</param>
        ///// <param name="culture">The culture of the conversion (ignored).</param>
        ///// <returns>The inverse of the input <paramref name="value"/>.</returns>
        //public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    return !((bool)value);
        //}

        ///// <summary>
        ///// The inverse of the <see cref="Convert"/>.
        ///// </summary>
        ///// <param name="value">The value to convert back.</param>
        ///// <param name="targetType">The type to convert to (ignored).</param>
        ///// <param name="parameter">Optional parameter (ignored).</param>
        ///// <param name="culture">The culture of the conversion (ignored).</param>
        ///// <returns>The inverse of the input <paramref name="value"/>.</returns>
        //public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    return !((bool)value);
        //}

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !((bool)value);
        }
    }
}