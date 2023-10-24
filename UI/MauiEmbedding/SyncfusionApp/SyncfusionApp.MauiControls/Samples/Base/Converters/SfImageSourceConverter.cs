using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SyncfusionApp.MauiControls.Samples.Base.Converters;

public class SfImageSourceConverter : IValueConverter
{
    //
    // Parameters:
    //   value:
    //
    //   targetType:
    //
    //   parameter:
    //
    //   culture:
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value as string;
        return ImageSource.FromResource(typeof(SfImageSourceConverter).GetTypeInfo().Assembly.GetName().Name + ".Resources.Images." + text, typeof(SfImageSourceConverter).GetTypeInfo().Assembly);
    }

    //
    // Parameters:
    //   value:
    //
    //   targetType:
    //
    //   parameter:
    //
    //   culture:
    //
    // Exceptions:
    //   T:System.NotImplementedException:
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
