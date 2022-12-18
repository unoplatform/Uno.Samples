using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using Windows.Storage.Streams;

namespace PetAdoptUI.Image.Converter
{
    public class ByteToImageConverter : IValueConverter
    {
        public BitmapImage ConvertByteArrayToBitMapImage(byte[] imageByteArray)
        {
            BitmapImage img = new BitmapImage();
            using (MemoryStream memStream = new MemoryStream(imageByteArray))
            {
                
                img.SetSource(this.ToRandomAccessStream(memStream));
//#if !(NET6_0_OR_GREATER && WINDOWS)
//                img.SetSource(memStream);
//#else
//                img.SetSource(this.ToRandomAccessStream(memStream));

//#endif

                //img.SetSource(memStream.AsRandomAccessStream());
            }
            return img;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage img = new BitmapImage();
            if (value != null)
            {
                img = this.ConvertByteArrayToBitMapImage(value as byte[]);
            }
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }

        private IRandomAccessStream ToRandomAccessStream(MemoryStream memStream) => memStream.AsRandomAccessStream();
    }
}