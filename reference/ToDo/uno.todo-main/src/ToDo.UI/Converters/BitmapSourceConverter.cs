
using System.IO;
using Microsoft.UI.Xaml.Media.Imaging;

namespace ToDo.Converters;

public class BitmapSourceConverter : IValueConverter
{
	public object? Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is not byte[] data) return null;
		using var buffer = new MemoryStream(data);
		var bitmap = new BitmapImage();
		bitmap.SetSource(buffer.AsRandomAccessStream());
		return bitmap;
	}
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException("Only one-way conversion is supported.");
}
