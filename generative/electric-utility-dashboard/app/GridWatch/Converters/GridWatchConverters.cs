using GridWatch.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace GridWatch.Converters;

internal static class ThemeBrushes
{
	public static SolidColorBrush Get(string key) =>
		Application.Current.Resources.TryGetValue(key, out var resource) && resource is SolidColorBrush brush
			? brush
			: new SolidColorBrush(Microsoft.UI.Colors.Transparent);
}

public sealed class AlertSeverityBrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is AlertSeverity s ? s switch
		{
			AlertSeverity.Critical => ThemeBrushes.Get("SeverityCriticalBrush"),
			AlertSeverity.Warning => ThemeBrushes.Get("SeverityWarningBrush"),
			AlertSeverity.Info => ThemeBrushes.Get("SeverityInfoBrush"),
			_ => ThemeBrushes.Get("SecondaryTextBrush")
		} : ThemeBrushes.Get("SecondaryTextBrush");

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class AlertRowBackgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is AlertSeverity s ? s switch
		{
			AlertSeverity.Critical => ThemeBrushes.Get("AlertRowCriticalBrush"),
			AlertSeverity.Warning => ThemeBrushes.Get("AlertRowWarningBrush"),
			_ => new SolidColorBrush(Microsoft.UI.Colors.Transparent)
		} : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class FacilityStatusBrushConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is FacilityStatus s ? s switch
		{
			FacilityStatus.Online => ThemeBrushes.Get("StatusOnlineBrush"),
			FacilityStatus.Warning => ThemeBrushes.Get("StatusWarningBrush"),
			FacilityStatus.Critical => ThemeBrushes.Get("StatusCriticalBrush"),
			_ => ThemeBrushes.Get("StatusOnlineBrush")
		} : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class FacilityStatusBackgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is FacilityStatus s ? s switch
		{
			FacilityStatus.Online => ThemeBrushes.Get("StatusOnlineBackgroundBrush"),
			FacilityStatus.Warning => ThemeBrushes.Get("StatusWarningBackgroundBrush"),
			FacilityStatus.Critical => ThemeBrushes.Get("StatusCriticalBackgroundBrush"),
			_ => new SolidColorBrush(Microsoft.UI.Colors.Transparent)
		} : new SolidColorBrush(Microsoft.UI.Colors.Transparent);

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class FacilityStatusForegroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is FacilityStatus s ? s switch
		{
			FacilityStatus.Online => ThemeBrushes.Get("StatusOnlineBrush"),
			FacilityStatus.Warning => ThemeBrushes.Get("StatusWarningBrush"),
			FacilityStatus.Critical => ThemeBrushes.Get("StatusCriticalBrush"),
			_ => ThemeBrushes.Get("OnAccentBrush")
		} : ThemeBrushes.Get("OnAccentBrush");

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class DeltaBackgroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is DeltaDirection d ? d switch
		{
			DeltaDirection.Up => ThemeBrushes.Get("DeltaUpBgBrush"),
			DeltaDirection.Down => ThemeBrushes.Get("DeltaDownBgBrush"),
			_ => ThemeBrushes.Get("DeltaNeutralBgBrush")
		} : ThemeBrushes.Get("DeltaNeutralBgBrush");

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}

public sealed class DeltaForegroundConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		value is DeltaDirection d ? d switch
		{
			DeltaDirection.Up => ThemeBrushes.Get("StatusOnlineBrush"),
			DeltaDirection.Down => ThemeBrushes.Get("StatusCriticalBrush"),
			_ => ThemeBrushes.Get("SecondaryTextBrush")
		} : ThemeBrushes.Get("SecondaryTextBrush");

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}
