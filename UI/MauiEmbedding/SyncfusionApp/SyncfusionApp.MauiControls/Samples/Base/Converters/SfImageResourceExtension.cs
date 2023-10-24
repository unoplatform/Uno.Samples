using System.Reflection;
using System.Xml;

namespace SyncfusionApp.MauiControls.Samples.Base.Converters;

[ContentProperty("Source")]
public class SfImageResourceExtension : IMarkupExtension<ImageSource>, IMarkupExtension
{
    public string? Source { get; set; }

    //
    // Parameters:
    //   serviceProvider:
    //
    // Exceptions:
    //   T:Microsoft.Maui.Controls.Xaml.XamlParseException:
    public ImageSource ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Source))
        {
            IXmlLineInfoProvider xmlLineInfoProvider = serviceProvider.GetService(typeof(IXmlLineInfoProvider)) as IXmlLineInfoProvider;
            IXmlLineInfo xmlLineInfo2;
            if (xmlLineInfoProvider == null)
            {
                IXmlLineInfo xmlLineInfo = new XmlLineInfo();
                xmlLineInfo2 = xmlLineInfo;
            }
            else
            {
                xmlLineInfo2 = xmlLineInfoProvider.XmlLineInfo;
            }

            IXmlLineInfo xmlInfo = xmlLineInfo2;
            throw new XamlParseException("ImageResourceExtension requires Source property to be set", xmlInfo);
        }

        return ImageSource.FromResource(typeof(SfImageResourceExtension).GetTypeInfo().Assembly.GetName().Name + ".Resources.Images." + Source, typeof(SfImageResourceExtension).GetTypeInfo().Assembly);
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ((IMarkupExtension<ImageSource>)this).ProvideValue(serviceProvider);
    }
}