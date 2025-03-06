using System.Globalization;
using TelerikApp.MauiControls.Common;
using TelerikApp.MauiControls.Helpers;

namespace TelerikApp.MauiControls.Converters;

public class ModelToConfigurationAreaConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length == 2 && values[0] is Example example)
        {
            object bindingContext = values[1];

            if (example != null && example.IsConfigurable && bindingContext != null)
            {
                View configView = Utils.CreateConfigurationArea(example);
                configView.BindingContext = bindingContext;
                return configView;
            }
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
