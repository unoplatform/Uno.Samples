namespace ToDo.Converters;

/// <summary>
/// This converter formats the object with the given string format, provided the object implements <see cref="IFormattable" />.
/// </summary>
/// <remarks>
/// <see cref="Binding.ConverterParameter" /> can also be used
/// instead of <see cref="Format" /> if localization is not required.
/// </remarks>
public class FormatConverter : IValueConverter
{
	/// <summary>
	/// Format to be used.
	/// </summary>
	public string? Format { get; set; }

	public object? Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is null) return null;
		if (value is not IFormattable formattable) return value.ToString();
		if ((Format ?? parameter as string) is not { } format) return value.ToString();

		return formattable.ToString(format, CultureInfo.CurrentUICulture);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		 => throw new NotSupportedException("Only one-way conversion is supported.");
}
