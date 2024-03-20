namespace ToDo.Converters;

/// <summary>
/// This converter formats the value with <see cref="string.Format(IFormatProvider, string, object)" />.
/// </summary>
/// <remarks>
/// Compared to <seealso cref="Binding.ConverterParameter" />, this converter allows for prefix and suffix to be added,
/// but requires encasing the format with '{0}'.
/// <see cref="Binding.ConverterParameter" /> can also be used instead of <see cref="Format" /> if localization is not required.
/// </remarks>
public class StringFormatter : IValueConverter
{
	/// <summary>
	/// Format to be used.
	/// </summary>
	public string? Format { get; set; }

	public object? Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is null) return null;
		if ((Format ?? parameter as string) is not { } format) return value.ToString();

		return string.Format(CultureInfo.CurrentUICulture, format, value);
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
		 => throw new NotSupportedException("Only one-way conversion is supported.");
}
