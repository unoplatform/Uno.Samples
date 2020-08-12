using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UnoChat.Client.Xaml
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = parameter switch
            {
                string format => string.Format(format.Replace('[', '{').Replace(']', '}'), value),
                _ => DependencyProperty.UnsetValue
            };

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
