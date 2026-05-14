using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace BrewHouse.Converters;

public sealed class OrderStatusBgConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		var key = value is OrderStatus s ? s switch
		{
			OrderStatus.Preparing => "StatusPreparingBgBrush",
			OrderStatus.ReadyForPickup => "StatusReadyBgBrush",
			_ => "StatusCompletedBrush"
		} : "StatusCompletedBrush";

		return Application.Current.Resources.TryGetValue(key, out var resource) && resource is SolidColorBrush brush
			? brush
			: new SolidColorBrush(Microsoft.UI.Colors.Transparent);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class OrderStatusFgConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		var key = value is OrderStatus s ? s switch
		{
			OrderStatus.Preparing => "StatusPreparingBrush",
			OrderStatus.ReadyForPickup => "StatusReadyBrush",
			_ => "StatusCompletedFgBrush"
		} : "StatusCompletedFgBrush";

		return Application.Current.Resources.TryGetValue(key, out var resource) && resource is SolidColorBrush brush
			? brush
			: new SolidColorBrush(Microsoft.UI.Colors.Transparent);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}
