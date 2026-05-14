using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace TaskFlow.Converters;

public class PriorityBrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is not TaskPriority priority)
		{
			return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
		}

		var key = priority switch
		{
			TaskPriority.High => "PriorityHighBrush",
			TaskPriority.Medium => "PriorityMediumBrush",
			TaskPriority.Low => "PriorityLowBrush",
			_ => (string?)null
		};

		if (key is not null && Application.Current.Resources.TryGetValue(key, out var resource) && resource is SolidColorBrush brush)
		{
			return brush;
		}

		return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException();
}

public class CategoryBrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is not TaskCategory category)
		{
			return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
		}

		var key = category switch
		{
			TaskCategory.Work => "CategoryWorkBrush",
			TaskCategory.Personal => "CategoryPersonalBrush",
			TaskCategory.Shopping => "CategoryShoppingBrush",
			TaskCategory.Health => "CategoryHealthBrush",
			_ => (string?)null
		};

		if (key is not null && Application.Current.Resources.TryGetValue(key, out var resource) && resource is SolidColorBrush brush)
		{
			return brush;
		}

		return new SolidColorBrush(Microsoft.UI.Colors.Transparent);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException();
}

public class BoolToStrikethroughConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is true)
		{
			return Windows.UI.Text.TextDecorations.Strikethrough;
		}

		return Windows.UI.Text.TextDecorations.None;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException();
}

public class BoolToOpacityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		return value is true ? 0.5 : 1.0;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotSupportedException();
}
