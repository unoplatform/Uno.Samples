using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;

using System.Globalization;

namespace CountryDataSample.Business.Converters
{
    internal class CountryToStatesConverter : IValueConverter
    {
        public UIElement UIParameter { get; set; }
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comboBox = UIParameter as ComboBox;
            var country = (string)value;

            if (country == null)
            {
                return null;
            }

            var countryCode = CountryConstants.GetCountryCode(country);
            var regions = CountryConstants.GetRegions(countryCode);
            
            if(comboBox != null)
            {
                comboBox.SelectedItem = regions.FirstOrDefault();
            }            
            return regions;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
