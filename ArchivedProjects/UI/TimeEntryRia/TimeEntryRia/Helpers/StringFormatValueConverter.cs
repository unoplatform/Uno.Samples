namespace TimeEntryRia
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Two way IValueConverter that lets you bind a property on a bindable object
    /// that can be an empty string value to a dependency property that should 
    /// be set to null in that case
    /// </summary>
    public class StringFormatValueConverter : IValueConverter
    {
        private readonly string formatString;

        /// <summary>
        /// Creates a new <see cref="StringFormatValueConverter"/>
        /// </summary>
        /// <param name="formatString">Format string, it can take zero or one parameters, the first one being replaced by the source value</param>
        public StringFormatValueConverter(string formatString)
        {
            this.formatString = formatString;
        }

        /// <summary>
        /// Converts the <paramref name="value"/> to a formatted string using the
        /// format specified in the constructor.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <param name="targetType">The target output type (ignored).</param>
        /// <param name="parameter">Optional parameter (ignored).</param>
        /// <param name="culture">The culture to use in the format operation.</param>
        /// <returns>The formatted string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format(System.Globalization.CultureInfo.CurrentUICulture, this.formatString, value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="value">Ignored.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">Ignored.</param>
        /// <returns>No value is returned.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
